using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ReactiveX.Trial.Tests")]

namespace ReactiveX.Logic
{
    public class DataProvider : IDataProvider
    {
        private readonly Random _random;
        private bool _isRunning;

        public DataProvider()
        {
            _random = new Random();
            BufferedChartData = Observable.Empty<IObservable<ChartData>>();
        }

        public IObservable<IObservable<ChartData>> BufferedChartData { get; private set; }

        public void Restart(TimeSpan sampleInterval, TimeSpan bufferLength, TimeSpan timeShift)
        {
            Stop();
            Start(sampleInterval, bufferLength, timeShift);
        }

        public void Start(TimeSpan sampleInterval, TimeSpan bufferLength, TimeSpan timeShift)
        {
            _isRunning = true;

            var chartData = Observable
                .Generate(0, _ => _isRunning, _ => _, _ => _, NewThreadScheduler.Default)
                .Sample(sampleInterval)
                .Select(CreateChartData);

            BufferedChartData = chartData
                .Buffer(bufferLength, timeShift)
                .Select(list => list.ToObservable())
                .StartWith(chartData);
        }

        public void Stop()
        {
            _isRunning = false;
        }

        private ChartData CreateChartData(int index)
        {
            return new ChartData
            {
                Value = _random.NextDouble(),
                Timestamp = DateTimeOffset.Now,
                EventId = index
            };
        }
    }
}