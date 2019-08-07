using System;

namespace ReactiveX.Logic
{
    public interface IClassicHotlink<out T>
    {
        IDisposable CreateHotlinkSingle(IObserver<T> observer);
    }
}