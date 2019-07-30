using System;
using System.Collections.Generic;
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

            Assert.Pass();
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

            Assert.Pass();
        }

        [Test]
        public void Test3()
        {
            var observable = Observable.Create<int>(Subscribe);

            var subscription = observable.Subscribe(Console.WriteLine);
            subscription.Dispose();

            Assert.Pass();
        }

        [Test]
        public void Test4()
        {
            using (var data = new Subject<int>())
            using (var process = new Subject<char>())
            {
                IObservable<int> observableData = data;
                IObservable<char> observableProcess = process;

                var observable = observableData;

                IList<int> actual = new List<int>();
                var subscription = observable.Subscribe(value =>
                {
                    actual.Add(value);
                    Console.WriteLine(value);
                });

                data.OnNext(1);
                data.OnNext(2);
                data.OnNext(3);

                subscription.Dispose();

                Assert.That(actual, Is.EqualTo(new[] {1, 2, 3}));
            }
        }

        private static IDisposable Subscribe(IObserver<int> observer)
        {
            for (var i = 0; i < 10; i++) observer.OnNext(i + 1);
            return Disposable.Empty;
        }
    }
}