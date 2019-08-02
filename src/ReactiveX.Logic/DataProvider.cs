using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace ReactiveX.Logic
{
    public class DataProvider : IDataProvider
    {
        private readonly Random _random;
        private bool _isRunning;

        public DataProvider()
        {
            _random = new Random();
            ChartData = Observable.Empty<ChartData>();
        }

        public IObservable<ChartData> ChartData { get; private set; }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Start()
        {
            _isRunning = true;

            ChartData = Observable
                .Generate((long) 0, _ => _isRunning, i => i + 1, l => l, NewThreadScheduler.Default)
                .Sample(TimeSpan.FromMilliseconds(20))
                .Timestamp()
                .Select(CreateChartData);
        }

        public void Stop()
        {
            _isRunning = false;

            ChartData = Observable.Empty<ChartData>();
        }

        private ChartData CreateChartData(Timestamped<long> timestamped)
        {
            return new ChartData
            {
                Value = _random.NextDouble(),
                Timestamp = timestamped.Timestamp,
                EventId = timestamped.Value
            };
        }
    }
}