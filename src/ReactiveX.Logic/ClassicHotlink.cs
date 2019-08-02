using System;
using System.Reactive.Disposables;

namespace ReactiveX.Logic
{
    public class ClassicHotlink<T> : IClassicHotlink<T>
    {
        private IObserver<T> _observer;

        public IDisposable CreateHotlinkSingle(IObserver<T> observer)
        {
            _observer = observer;
            return Disposable.Create(() => Console.Write("hotlink disposed, "));
        }

        public void Emit(T value)
        {
            _observer.OnNext(value);
        }

        public void Fail()
        {
            _observer.OnError(new Exception("callback failed"));
        }

        public void Complete()
        {
            _observer.OnCompleted();
        }
    }
}