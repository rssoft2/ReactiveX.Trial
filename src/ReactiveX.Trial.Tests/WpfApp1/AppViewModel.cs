﻿using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
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
                .ObserveOn(DispatcherScheduler.Current)
                .Bind(TargetCollection)
                .Subscribe();
        }

        private SourceList<string> ValuesDyn { get; }

        public IObservableCollection<string> TargetCollection { get; } = new ObservableCollectionExtended<string>();
    }
}