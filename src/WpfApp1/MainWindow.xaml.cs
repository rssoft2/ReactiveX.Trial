﻿using System;
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

            IDataProvider dataProvider = new DataProvider();
            var subscription = Disposable.Empty;

            Observable.FromEventPattern(Start, "Click")
                .Subscribe(pattern => { subscription = RestartDataProvider(dataProvider, subscription); });

            Observable.FromEventPattern(Stop, "Click")
                .Subscribe(pattern => StopDataProvider(dataProvider, subscription));

            Observable.FromEventPattern(this, "Closed")
                .Subscribe(pattern => StopDataProvider(dataProvider, subscription));

            this.WhenActivated(disposableRegistration =>
                {
                    this.OneWayBind(ViewModel,
                            viewModel => viewModel.Values.Items,
                            view => view.List.ItemsSource)
                        .DisposeWith(disposableRegistration);

                    subscription = RestartDataProvider(dataProvider, subscription);
                }
            );

            Observable.FromEventPattern(this, nameof(Deactivated))
                .Subscribe(pattern => ViewModel?.Dispose());
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
                .Select(list => list.ToObservable())
                .StartWith(dataProvider.ChartData)
                .Subscribe(ViewModel.Subscribe);
        }
    }
}