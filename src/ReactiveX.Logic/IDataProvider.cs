using System;
using System.Collections.Generic;

namespace ReactiveX.Logic
{
    public interface IDataProvider
    {
        IObservable<ChartData> ChartData { get; }
        IObservable<IObservable<ChartData>> WindowedChartData { get; }
        IObservable<IList<ChartData>> BufferedChartData { get; }
        void Restart(TimeSpan sampleInterval, TimeSpan windowLength, TimeSpan timeShift);
        void Start(TimeSpan sampleInterval, TimeSpan windowLength, TimeSpan timeShift);
        void Stop();
    }
}