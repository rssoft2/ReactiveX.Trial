using System;
using System.Threading;
using NUnit.Framework;
using ReactiveX.Logic;

namespace ReactiveX.Trial.Tests
{
    public class DataProviderTests
    {
        [Test]
        public void ChartData_NotStarted_NotNull()
        {
            var dataProvider = new DataProvider();
            Assert.That(dataProvider.ChartData, Is.Not.Null);
        }

        [Test]
        public void ChartData_NotStartedSubscribed_NoChartDataReceived()
        {
            var dataProvider = new DataProvider();
            var received = false;

            dataProvider.ChartData.Subscribe(data => received = true);
            Thread.Sleep(100);

            Assert.That(received, Is.False);
        }

        [Test]
        public void ChartData_StartedSubscribed_ChartDataReceived()
        {
            var dataProvider = new DataProvider();
            var received = false;

            dataProvider.Start(TimeSpan.FromMilliseconds(10), TimeSpan.Zero, TimeSpan.Zero);
            dataProvider.ChartData.Subscribe(data =>
            {
                Console.WriteLine(data);
                received = true;
            });
            Thread.Sleep(100);

            Assert.That(received, Is.True);
        }

        [Test]
        public void ChartData_StartedStopped_ChartDataCompleted()
        {
            var dataProvider = new DataProvider();
            var completed = false;

            dataProvider.Start(TimeSpan.FromMilliseconds(10), TimeSpan.Zero, TimeSpan.Zero);
            dataProvider.ChartData
                .Subscribe(Console.WriteLine, () => completed = true);
            dataProvider.Stop();
            Thread.Sleep(100);

            Assert.That(completed, Is.True);
        }

        [Test]
        public void WindowedChartData_Subscribed_WindowsReceived()
        {
            IDataProvider dataProvider = new DataProvider();
            var received = 0;
            var w = 0;

            dataProvider.Start(
                TimeSpan.FromMilliseconds(10),
                TimeSpan.FromMilliseconds(100),
                TimeSpan.FromMilliseconds(10));
            dataProvider.WindowedChartData.Subscribe(window =>
            {
                var x = w++;
                Console.WriteLine($"new window: {DateTime.Now:ss:fff}");
                window.Subscribe(data => Console.WriteLine($"new data: w={x}, {DateTime.Now:ss:fff}, {data}"));
                received++;
            });
            Thread.Sleep(1000);

            Assert.That(received, Is.GreaterThan(1));
        }

        [Test]
        public void BufferedChartData_Subscribed_BuffersReceived()
        {
            IDataProvider dataProvider = new DataProvider();
            var received = 0;
            var w = 0;

            dataProvider.Start(
                TimeSpan.FromMilliseconds(10),
                TimeSpan.FromMilliseconds(100),
                TimeSpan.FromMilliseconds(10));
            dataProvider.BufferedChartData.Subscribe(buffer =>
            {
                var x = w++;
                Console.WriteLine($"new buffer: {DateTime.Now:ss:fff}");
                foreach (var data in buffer) Console.WriteLine($"new data: b={x}, {DateTime.Now:ss:fff}, {data}");
                received++;
            });
            Thread.Sleep(1000);

            Assert.That(received, Is.GreaterThan(1));
        }
    }
}