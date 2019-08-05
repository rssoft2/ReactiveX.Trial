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
            WindowedChartData = Observable.Empty<IObservable<ChartData>>();
            BufferedChartData = Observable.Empty<IObservable<ChartData>>();
        }

        public IObservable<ChartData> ChartData { get; private set; }
        public IObservable<IObservable<ChartData>> WindowedChartData { get; private set; }
        public IObservable<IObservable<ChartData>> BufferedChartData { get; private set; }

        public void Restart(TimeSpan sampleInterval, TimeSpan bufferLength, TimeSpan timeShift)
        {
            Stop();
            Start(sampleInterval, bufferLength, timeShift);
        }

        public void Start(TimeSpan sampleInterval, TimeSpan bufferLength, TimeSpan timeShift)
        {
            _isRunning = true;

            ChartData = Observable
                .Generate(0, _ => _isRunning, _ => _, _ => _, NewThreadScheduler.Default)
                .Sample(sampleInterval)
                .Timestamp()
                .Select(CreateChartData);

            WindowedChartData = ChartData.Window(bufferLength, timeShift);

            BufferedChartData = ChartData
                .Buffer(bufferLength, timeShift)
                .Select(list => list.ToObservable())
                .StartWith(ChartData);

        }

        public void Stop()
        {
            _isRunning = false;
        }

        private ChartData CreateChartData(Timestamped<int> timestamped, int index)
        {
            return new ChartData
            {
                Value = _random.NextDouble(),
                Timestamp = timestamped.Timestamp,
                EventId = index
            };
        }
    }
}