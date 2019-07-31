using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NUnit.Framework;

namespace ReactiveX.Trial.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

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
        public void Test3()
        {
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

        private static IDisposable Subscribe(IObserver<int> observer)
        {
            for (var i = 0; i < 10; i++) observer.OnNext(i + 1);
            return Disposable.Empty;
        }

        [Test]
        public void Test5()
        {
            var classicHotlink = new ClassicHotlink<string>();

            void CallbackAction(string value)
            {
                Console.Write(value);
            }

            void DisposedAction()
            {
                Console.WriteLine(" classic hotlink disposed");
            }

            var observable = classicHotlink.AsObservable(DisposedAction);
            var subscription = observable.Subscribe(CallbackAction);

            classicHotlink.RaiseCallback("1");
            classicHotlink.RaiseCallback("2");
            classicHotlink.RaiseCallback("3");
            classicHotlink.RaiseCallback(" der gejid!");

            subscription.Dispose();
        }
    }

    public interface IClassicHotlink<out T>
    {
        IDisposable CreateHotlinkSingle(Action<T> callback, Action disposed);
    }

    public class ClassicHotlink<T> : IClassicHotlink<T>
    {
        private Action<T> _callback;

        public IDisposable CreateHotlinkSingle(Action<T> callback, Action disposed)
        {
            _callback = callback;
            return Disposable.Create(disposed);
        }

        public void RaiseCallback(T value)
        {
            _callback(value);
        }
    }

    public static class ObservableExtensions
    {
        public static IObservable<T> AsObservable<T>(this IClassicHotlink<T> classicHotlink, Action disposed)
        {
            return Observable.Create<T>(observer => classicHotlink.CreateHotlinkSingle(observer.OnNext, disposed));
        }
    }
}