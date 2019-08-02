using System;
using System.Collections.Generic;

namespace ReactiveX.Logic
{
    public interface IDataProvider
    {
        IObservable<ChartData> ChartData { get; }
        IObservable<IObservable<ChartData>> WindowedChartData { get; }
        IObservable<IList<ChartData>> BufferedChartData { get; }
        void Restart(TimeSpan sampleInterval, TimeSpan bufferLength, TimeSpan timeShift);
        void Start(TimeSpan sampleInterval, TimeSpan bufferLength, TimeSpan timeShift);
        void Stop();
    }
}