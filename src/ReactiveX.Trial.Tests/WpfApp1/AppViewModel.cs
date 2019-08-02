using System;
using System.Collections.Generic;
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

        public AppViewModel(IEnumerable<string> list)
        {
            TargetCollection.Clear();
            TargetCollection.Add(list);
        }

        private SourceList<string> ValuesDyn { get; }

        public IObservableCollection<string> TargetCollection { get; } = new ObservableCollectionExtended<string>();
    }
}