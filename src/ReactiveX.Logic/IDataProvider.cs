using System;

namespace ReactiveX.Logic
{
    public interface IDataProvider
    {
        IObservable<ChartData> ChartData { get; }
        void Start();
        void Stop();
        void Restart();
    }
}