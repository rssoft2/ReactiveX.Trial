using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

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
            var observable = Observable
                .Interval(TimeSpan.FromMilliseconds(100))
                .Select(i => $"New item {i}");
            
            ViewModel = new AppViewModel(observable);

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