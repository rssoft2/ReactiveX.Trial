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
        private DataProvider _dataProvider;

        [SetUp]
        public void Setup()
        {
            _dataProvider = new DataProvider(_sampleIntervall, _bufferLength, _timeShift);

        }
        [Test]
        public void ChartData_NotStarted_NotNull()
        {
            Assert.That(_dataProvider.BufferedChartData, Is.Not.Null);
        }

        [Test]
        public void ChartData_NotStartedSubscribed_NoChartDataReceived()
        {
            var received = false;

            _dataProvider.BufferedChartData.Subscribe(data => received = true);
            Thread.Sleep(100);

            Assert.That(received, Is.False);
        }

        [Test]
        public void ChartData_StartedSubscribed_ChartDataReceived()
        {
            var received = false;

            _dataProvider.Start();
            _dataProvider.BufferedChartData
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
            var completed = false;

            _dataProvider.Start();
            _dataProvider.BufferedChartData
                .Subscribe(
                    buffer =>
                    {
                        buffer.Subscribe(data => Console.WriteLine($"new data: {DateTime.Now:ss:fff}, {data}"));
                    },
                    () => completed = true);
            _dataProvider.Stop();
            Thread.Sleep(100);

            Assert.That(completed, Is.True);
        }

        [Test]
        public void BufferedChartData_Subscribed_BuffersReceived()
        {
            var received = 0;
            var w = 0;

            Console.WriteLine($"start: {DateTime.Now:ss:fff}");
            _dataProvider.Start();
            _dataProvider.BufferedChartData
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