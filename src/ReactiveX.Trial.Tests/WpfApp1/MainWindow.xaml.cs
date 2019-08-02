using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
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
            

            Observable.FromEventPattern<MouseEventArgs>(this, "MouseMove")
                .Where(eventArgs => Math.Abs(eventArgs.EventArgs.GetPosition(this).X) < 10)
                .Subscribe(eventArgs =>
                {
                    dataProvider.Restart(TimeSpan.FromMilliseconds(200), TimeSpan.FromSeconds(2));
                    ViewModel = new AppViewModel(dataProvider.ChartData.Select(data => data.ToString()));
                });

            Observable.FromEventPattern<MouseEventArgs>(this, "MouseMove")
                .Where(eventArgs => Math.Abs(eventArgs.EventArgs.GetPosition(this).Y) < 10)
                .Subscribe(eventArgs => dataProvider.Stop());

            Observable.FromEventPattern(this, "Closed")
                .Subscribe(pattern => dataProvider.Stop());

            Observable.FromEventPattern(Start, "Click")
                .Subscribe(pattern =>
                {
                    dataProvider.Restart();
                    ViewModel = new AppViewModel(dataProvider.ChartData.Select(data => data.ToString()));
                });

            Observable.FromEventPattern(Stop, "Click")
                .Subscribe(pattern => dataProvider.Stop());

            this.WhenActivated(disposableRegistration =>
                {
                    this.OneWayBind(ViewModel,
                            viewModel => viewModel.TargetCollection,
                            view => view.list.ItemsSource)
                        .DisposeWith(disposableRegistration);
                }
            );
        }
    }
}