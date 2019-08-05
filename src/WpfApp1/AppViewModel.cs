using System;
using System.Reactive.Linq;
using System.Windows;
using DynamicData;
using ReactiveUI;

namespace WpfApp1
{
    public class AppViewModel : ReactiveObject, IDisposable
    {
        public SourceList<string> Values { get; private set; }

        public void Dispose()
        {
            Values?.Dispose();
        }

        public IDisposable Subscribe(IObservable<string> observable)
        {
            Values = new SourceList<string>(observable.ToObservableChangeSet());

            return Values.Connect()
                .ObserveOn(Application.Current.Dispatcher)
                .Subscribe(set => this.RaisePropertyChanged(nameof(Values)));
        }
    }
}