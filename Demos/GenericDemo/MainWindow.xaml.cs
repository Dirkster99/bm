namespace GenericDemo
{
    using System;
    using System.Windows;
    using BreadcrumbTestLib.ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;

            var appVM = new AppViewModel("Generic");
            DataContext = appVM;

            appVM.InitPathAsync(@"C:\");
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            var appVm = (DataContext as IDisposable);

            if (appVm != null)
                appVm.Dispose();

            DataContext = null;
        }
    }
}
