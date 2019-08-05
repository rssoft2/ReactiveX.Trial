using System;
using System.Reactive.Linq;
using System.Windows;
using DynamicData;
using ReactiveUI;
using ReactiveX.Logic;

namespace WpfApp1
{
    public class AppViewModel : ReactiveObject, IDisposable
    {
        private IDisposable _valuesSubscription;

        public SourceList<ChartData> Values { get; private set; }

        public void Dispose()
        {
            _valuesSubscription?.Dispose();
            Values?.Dispose();
        }

        public void Subscribe(IObservable<ChartData> observable)
        {
            _valuesSubscription?.Dispose();
            Values?.Dispose();
            
            Values = new SourceList<ChartData>(observable.ToObservableChangeSet());

            _valuesSubscription = Values.Connect()
                .ObserveOn(Application.Current.Dispatcher)
                .Subscribe(set => this.RaisePropertyChanged(nameof(Values)));
        }
    }
}