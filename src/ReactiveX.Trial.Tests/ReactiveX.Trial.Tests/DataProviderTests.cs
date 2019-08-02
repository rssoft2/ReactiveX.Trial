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

            dataProvider.Start();
            dataProvider.ChartData.Subscribe(data =>
            {
                Console.WriteLine(data);
                received = true;
            });
            Thread.Sleep(200);

            Assert.That(received, Is.True);
        }

        [Test]
        public void ChartData_StartedStopped_ChartDataCompleted()
        {
            var dataProvider = new DataProvider();
            var completed = false;

            dataProvider.Start();
            dataProvider.ChartData
                .Subscribe(Console.WriteLine, () => completed = true);
            dataProvider.Stop();
            Thread.Sleep(100);

            Assert.That(completed, Is.True);
        }
    }
}