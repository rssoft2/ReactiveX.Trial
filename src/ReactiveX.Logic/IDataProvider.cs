using System;

namespace ReactiveX.Logic
{
    public interface IDataProvider
    {
        IObservable<ChartData> ChartData { get; }
        IObservable<IObservable<ChartData>> SlidingChartData { get; }
        void Restart(TimeSpan sampleInterval, TimeSpan windowLength);
        void Start(TimeSpan sampleInterval, TimeSpan windowLength);
        void Stop();
    }
}