using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using NUnit.Framework;

namespace ReactiveX.Trial.Tests
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            var observable = "blubb".ToObservable();

            var subscription = observable.Subscribe(Console.Write);

            subscription.Dispose();
        }

        [Test]
        public void Test2()
        {
            var subject = new Subject<char>();
            IObservable<char> observable = subject;

            var subscription = observable.Subscribe(Console.Write);
            subject.OnNext('b');
            subject.OnNext('l');
            subject.OnNext('u');
            subject.OnNext('b');
            subject.OnNext('b');

            subscription.Dispose();
        }

        [Test]
        public void Test3A()
        {
            var observable = Observable.Create(
                (IObserver<int> observer) =>
                {
                    for (var i = 0; i < 10; i++) observer.OnNext(i + 1);
                    observer.OnCompleted();
                    return () => Console.WriteLine("Observer has unsubscribed");
                });

            var subscription = observable.Subscribe(Console.WriteLine);
            subscription.Dispose();
        }

        [Test]
        public void Test3B()
        {
            IDisposable Subscribe(IObserver<int> observer)
            {
                for (var i = 0; i < 10; i++) observer.OnNext(i + 1);
                observer.OnCompleted();
                return Disposable.Create(() => Console.WriteLine("Observer has unsubscribed"));
            }

            var observable = Observable.Create<int>(Subscribe);

            var subscription = observable.Subscribe(Console.WriteLine);
            subscription.Dispose();
        }

        [Test]
        public void Test4()
        {
            using (var data = new Subject<string>())
            using (var process = new Subject<string>())
            {
                IObservable<string> observableData = data;
                IObservable<string> observableProcess = process;

                var observable =
                    observableData.WithLatestFrom(observableProcess, (data1, process1) => data1 + process1);

                const string expected = "2a3a4a5a6b7b8b";
                var actual = string.Empty;

                var subscription = observable
                    .Subscribe(value =>
                    {
                        actual += value;
                        Console.WriteLine(value);
                    });

                data.OnNext("1");
                process.OnNext("a");
                data.OnNext("2");
                data.OnNext("3");
                data.OnNext("4");
                data.OnNext("5");
                process.OnNext("b");
                data.OnNext("6");
                data.OnNext("7");
                data.OnNext("8");
                process.OnNext("c");
                data.OnCompleted();
                process.OnCompleted();

                subscription.Dispose();

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public void Test5()
        {
            var classicHotlink = new ClassicHotlink<string>();

            void OnNext(string value)
            {
                Console.Write(value);
            }

            void OnError(Exception exception)
            {
                Console.WriteLine("exception, ");
            }

            void OnCompleted()
            {
                Console.Write("sequence completed, ");
            }

            var observable = classicHotlink.AsObservable();
            observable.Subscribe(OnNext, OnError, OnCompleted);

            classicHotlink.Emit("1, ");
            classicHotlink.Emit("2, ");
            classicHotlink.Emit("3, ");
            classicHotlink.Emit("der gejid!, ");
            classicHotlink.Complete();
        }

        [TestCase("fail", true)]
        [TestCase("complete", false)]
        public void Test6(string description, bool fail)
        {
            var classicHotlink = new ClassicHotlink<string>();

            void OnNext(string value)
            {
                Console.Write("a: " + value);
            }

            void OnNext2(string value)
            {
                Console.Write("b: " + value);
            }

            void OnError2(Exception exception)
            {
                Console.Write("b: " + exception + ", ");
            }

            void OnCompleted2()
            {
                Console.Write("b: sequence completed, ");
            }

            var observable = classicHotlink.AsConnectableObservable();
            var subscription = observable.Subscribe(OnNext, _ => { }, () => { });
            observable.Subscribe(OnNext2, OnError2, OnCompleted2);

            var connection = observable.Connect();
            classicHotlink.Emit("1, ");
            connection.Dispose();
            classicHotlink.Emit("lost, ");
            observable.Connect();
            classicHotlink.Emit("2, ");

            subscription.Dispose();

            if (fail)
                classicHotlink.Fail();

            classicHotlink.Emit("3, ");

            if (!fail)
                classicHotlink.Complete();
        }


        private static IObservable<int> GetNewValues()
        {
            var intervalObservable = Observable.Range(0, 5);

            return intervalObservable;
        }


        [Test]
        public void Defer()
        {
            var someObservable = Observable.Defer(GetNewValues);

            var subscription1 = someObservable.Subscribe(i => Console.WriteLine(i.ToString()));
            subscription1.Dispose();

            var subscription2 = someObservable.Subscribe(i => Console.WriteLine(i.ToString()));
        }


        /// <summary>
        ///     see http://davesexton.com/blog/post/To-Use-Subject-Or-Not-To-Use-Subject.aspx
        /// </summary>
        [Test]
        public void ReplaySubject()
        {
            var someObservable = Observable.Interval(TimeSpan.FromMilliseconds(50));

            Thread.Sleep(1000);

            var myReplaySubject = new ReplaySubject<long>();
            var _ = someObservable.Subscribe(myReplaySubject);

            var subscription1 = myReplaySubject.Buffer(5).Subscribe(list =>
            {
                Console.WriteLine("New List......");
                foreach (var i in list) Console.WriteLine($"Subscription 1: {i}");
            });
            Thread.Sleep(1000);
            subscription1.Dispose();
            Console.WriteLine("Subscription 1 disposed.");

            Thread.Sleep(1000);

            Console.WriteLine("Subscription 2 subscribe:");
            var subscription2 = myReplaySubject.Subscribe(i => Console.WriteLine($"Subscription 2: {i}"));
            subscription2.Dispose();

            _.Dispose();
        }
    }

    public interface IClassicHotlink<out T>
    {
        IDisposable CreateHotlinkSingle(IObserver<T> observer);
    }

    public class ClassicHotlink<T> : IClassicHotlink<T>
    {
        private IObserver<T> _observer;

        public IDisposable CreateHotlinkSingle(IObserver<T> observer)
        {
            _observer = observer;
            return Disposable.Create(() => Console.Write("hotlink disposed, "));
        }

        public void Emit(T value)
        {
            _observer.OnNext(value);
        }

        public void Fail()
        {
            _observer.OnError(new Exception("callback failed"));
        }

        public void Complete()
        {
            _observer.OnCompleted();
        }
    }

    public static class ObservableExtensions
    {
        public static IObservable<T> AsObservable<T>(this IClassicHotlink<T> classicHotlink)
        {
            return Observable.Create<T>(classicHotlink.CreateHotlinkSingle);
        }

        public static IConnectableObservable<T> AsConnectableObservable<T>(this IClassicHotlink<T> classicHotlink)
        {
            return classicHotlink.AsObservable().Publish();
        }
    }
}