using System;
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
            ViewModel = new AppViewModel();

            IDataProvider dataProvider = new DataProvider(TimeSpan.FromMilliseconds(40), TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(40));
            var subscription = Disposable.Empty;

            Observable.FromEventPattern(Start, "Click")
                .Subscribe(pattern => { subscription = RestartDataProvider(dataProvider, subscription); });

            Observable.FromEventPattern(Stop, "Click")
                .Subscribe(pattern => StopDataProvider(dataProvider, subscription));

            Observable.FromEventPattern(this, "Closed")
                .Subscribe(pattern =>
                {
                    StopDataProvider(dataProvider, subscription);
                    ViewModel?.Dispose();
                });
            
            this.WhenActivated(disposableRegistration =>
                {
                    this.OneWayBind(ViewModel,
                            viewModel => viewModel.Values.Items,
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
            dataProvider.Restart();

            return dataProvider.BufferedChartData
                .ObserveOn(DispatcherScheduler.Current)
                .Subscribe(ViewModel.Subscribe);
        }
    }
}