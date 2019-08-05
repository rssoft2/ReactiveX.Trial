using System;
using System.Reactive.Linq;
using System.Windows;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace WpfApp1
{
    public class AppViewModel : ReactiveObject
    {
        public AppViewModel(IObservable<string> observable)
        {
            ValuesDyn = new SourceList<string>(observable.ToObservableChangeSet());

            ValuesDyn.Connect()
                .ObserveOn(Application.Current.Dispatcher)
                .Bind(TargetCollection)
                .Subscribe();
        }

        private SourceList<string> ValuesDyn { get; }

        public IObservableCollection<string> TargetCollection { get; } = new ObservableCollectionExtended<string>();
    }
}