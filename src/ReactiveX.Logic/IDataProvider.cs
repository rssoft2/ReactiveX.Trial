using System;

namespace ReactiveX.Logic
{
    public interface IDataProvider
    {
        IObservable<IObservable<ChartData>> BufferedChartData { get; }
        void Restart(TimeSpan sampleInterval, TimeSpan bufferLength, TimeSpan timeShift);
        void Start(TimeSpan sampleInterval, TimeSpan bufferLength, TimeSpan timeShift);
        void Stop();
    }
}