using System;
using System.Threading;
using NUnit.Framework;
using ReactiveX.Logic;

namespace ReactiveX.Trial.Tests
{
    public class DataProviderTests
    {
        private readonly TimeSpan _bufferLength = TimeSpan.FromMilliseconds(100);
        private readonly TimeSpan _sampleIntervall = TimeSpan.FromMilliseconds(10);
        private readonly TimeSpan _timeShift = TimeSpan.FromMilliseconds(10);

        [Test]
        public void ChartData_NotStarted_NotNull()
        {
            var dataProvider = new DataProvider();
            Assert.That(dataProvider.BufferedChartData, Is.Not.Null);
        }

        [Test]
        public void ChartData_NotStartedSubscribed_NoChartDataReceived()
        {
            var dataProvider = new DataProvider();
            var received = false;

            dataProvider.BufferedChartData.Subscribe(data => received = true);
            Thread.Sleep(100);

            Assert.That(received, Is.False);
        }

        [Test]
        public void ChartData_StartedSubscribed_ChartDataReceived()
        {
            var dataProvider = new DataProvider();
            var received = false;

            dataProvider.Start(_sampleIntervall, _bufferLength, _timeShift);
            dataProvider.BufferedChartData
                .Subscribe(
                    buffer =>
                    {
                        buffer.Subscribe(data => Console.WriteLine($"new data: {DateTime.Now:ss:fff}, {data}"));
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

            dataProvider.Start(_sampleIntervall, _bufferLength, _timeShift);
            dataProvider.BufferedChartData
                .Subscribe(
                    buffer =>
                    {
                        buffer.Subscribe(data => Console.WriteLine($"new data: {DateTime.Now:ss:fff}, {data}"));
                    },
                    () => completed = true);
            dataProvider.Stop();
            Thread.Sleep(100);

            Assert.That(completed, Is.True);
        }

        [Test]
        public void BufferedChartData_Subscribed_BuffersReceived()
        {
            IDataProvider dataProvider = new DataProvider();
            var received = 0;
            var w = 0;

            Console.WriteLine($"start: {DateTime.Now:ss:fff}");
            dataProvider.Start(_sampleIntervall, _bufferLength, _timeShift);
            dataProvider.BufferedChartData
                .Subscribe(buffer =>
                {
                    var x = w++;
                    Console.WriteLine($"new window: {DateTime.Now:ss:fff}");
                    buffer.Subscribe(data => Console.WriteLine($"new data: w={x}, {DateTime.Now:ss:fff}, {data}"));
                    received++;
                });
            Thread.Sleep(1000);

            Assert.That(received, Is.GreaterThan(1));
        }
    }
}