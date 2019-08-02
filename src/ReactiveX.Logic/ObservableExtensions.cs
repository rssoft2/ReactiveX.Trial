using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveX.Logic
{
    public static class ObservableExtensions
    {
        public static IObservable<T> AsObservable<T>(this IClassicHotlink<T> classicHotlink)
        {
            return Observable.Create<T>(classicHotlink.CreateHotlinkSingle);
        }

        public static IConnectableObservable<T> AsConnectableObservable<T>(this IClassicHotlink<T> classicHotlink)
        {
            return classicHotlink.AsObservable().Publish();
        }
    }
}
