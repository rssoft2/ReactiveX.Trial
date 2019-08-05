using System.Collections.Generic;
using ReactiveUI;

namespace WpfApp1
{
    public class AppViewModel : ReactiveObject
    {
        //public AppViewModel(IObservable<string> observable)
        //{
        //    ValuesDyn = new SourceList<string>(observable.ToObservableChangeSet());

        //    ValuesDyn.Connect()
        //        .ObserveOn(Application.Current.Dispatcher)
        //        .Bind(TargetCollection)
        //        .Subscribe();
        //}

        //public AppViewModel(IEnumerable<string> list)
        //{
        //    TargetCollection.Clear();
        //    TargetCollection.Add(list);
        //}

        // private SourceList<string> ValuesDyn { get; }

        // public IObservableCollection<string> TargetCollection { get; } = new ObservableCollectionExtended<string>();
        
        private static IEnumerable<string> _targetCollection;

        public AppViewModel(IEnumerable<string> list)
        {
            this.RaiseAndSetIfChanged(ref _targetCollection, list, nameof(TargetCollection));
        }

        public IEnumerable<string> TargetCollection { get; } = _targetCollection;
    }
}