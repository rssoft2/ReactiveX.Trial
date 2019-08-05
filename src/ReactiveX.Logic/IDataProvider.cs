using System;

namespace ReactiveX.Logic
{
    public interface IDataProvider
    {
        IObservable<IObservable<ChartData>> BufferedChartData { get; }
        void Restart();
        void Start();
        void Stop();
    }
}