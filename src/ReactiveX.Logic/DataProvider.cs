using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ReactiveX.Trial.Tests")]

namespace ReactiveX.Logic
{
    public class DataProvider : IDataProvider
    {
        private readonly TimeSpan _sampleInterval;
        private readonly TimeSpan _bufferLength;
        private readonly TimeSpan _timeShift;
        private readonly Random _random;
        private bool _isRunning;

        public DataProvider(TimeSpan sampleInterval, TimeSpan bufferLength, TimeSpan timeShift)
        {
            _sampleInterval = sampleInterval;
            _bufferLength = bufferLength;
            _timeShift = timeShift;
            _random = new Random();
            BufferedChartData = Observable.Empty<IObservable<ChartData>>();
        }

        public IObservable<IObservable<ChartData>> BufferedChartData { get; private set; }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Start()
        {
            _isRunning = true;

            var chartData = Observable
                .Generate(0, _ => _isRunning, _ => _, _ => _, NewThreadScheduler.Default)
                .Sample(_sampleInterval)
                .Select(CreateChartData);

            BufferedChartData = chartData
                .Buffer(_bufferLength, _timeShift)
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