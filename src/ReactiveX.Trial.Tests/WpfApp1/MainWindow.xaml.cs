using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveX.Logic;

namespace WpfApp1
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            IDataProvider dataProvider = new DataProvider();
            var subscription = Disposable.Empty;

            //Observable.FromEventPattern<MouseEventArgs>(this, "MouseMove")
            //    .Where(eventArgs => Math.Abs(eventArgs.EventArgs.GetPosition(this).X) < 10)
            //    .Subscribe(eventArgs =>
            //    {
            //        dataProvider.Restart(TimeSpan.FromMilliseconds(200), TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(200));

            //        dataProvider.WindowedChartData
            //            .ObserveOn(DispatcherScheduler.Current)
            //            .Subscribe(window => ViewModel = new AppViewModel(window.Select(data => data.ToString())));
            //    });

            //Observable.FromEventPattern<MouseEventArgs>(this, "MouseMove")
            //    .Where(eventArgs => Math.Abs(eventArgs.EventArgs.GetPosition(this).Y) < 10)
            //    .Subscribe(eventArgs => dataProvider.Stop());


            Observable.FromEventPattern(Start, "Click")
                .Subscribe(pattern => { subscription = RestartDataProvider(dataProvider, subscription); });

            Observable.FromEventPattern(Stop, "Click")
                .Subscribe(pattern => StopDataProvider(dataProvider, subscription));

            Observable.FromEventPattern(this, "Closed")
                .Subscribe(pattern => StopDataProvider(dataProvider, subscription));
            
            this.WhenActivated(disposableRegistration =>
                {
                    this.OneWayBind(ViewModel,
                            viewModel => viewModel.TargetCollection,
                            view => view.List.ItemsSource)
                        .DisposeWith(disposableRegistration);

                    subscription = RestartDataProvider(dataProvider, subscription);
                }
            );
        }

        private static void StopDataProvider(IDataProvider dataProvider, IDisposable subscription)
        {
            subscription.Dispose();
            dataProvider.Stop();
        }

        private IDisposable RestartDataProvider(IDataProvider dataProvider, IDisposable subscription)
        {
            subscription.Dispose();
            dataProvider.Restart(TimeSpan.FromMilliseconds(40), TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(40));

            return dataProvider.BufferedChartData
                .ObserveOn(DispatcherScheduler.Current)
                .Subscribe(window => ViewModel = new AppViewModel(window.Select(data => data.ToString())));

            //dataProvider.WindowedChartData
            //    .ObserveOn(DispatcherScheduler.Current)
            //    .Subscribe(window => ViewModel = new AppViewModel(window.Select(data => data.ToString())));
        }
    }
}