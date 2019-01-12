namespace WpfPerformance
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using WpfPerformance.ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_LoadedAsync;
        }

        private async void MainWindow_LoadedAsync(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_LoadedAsync;

            var appVM = new ViewModels.AppViewModel();
            DataContext = appVM;
            await appVM.InitLoadAsync();
        }

        private static bool IsUserVisible(FrameworkElement element, FrameworkElement container)
        {
            if (!element.IsVisible)
                return false;

            Rect bounds =
                element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));

            var rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);

            return rect.Contains(bounds.TopLeft) || rect.Contains(bounds.BottomRight);
        }

        private List<IItemViewModel> GetVisibleItemsFromListbox(ListBox listBox,
                                                                FrameworkElement parentToTestVisibility)
        {
            var items = new List<IItemViewModel>();

            foreach (var item in listBox.Items)
            {
                var lstBoxItem = listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;

                if (lstBoxItem != null)
                {
                    if (IsUserVisible(lstBoxItem, parentToTestVisibility))
                    {
                        var viewmodelItem = item as IItemViewModel;

                        if (viewmodelItem != null)
                            items.Add(viewmodelItem);
                    }
                    else if (items.Any())
                    {
                        break;
                    }
                }
            }

            return items;
        }

        private void ItemsListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var list = GetVisibleItemsFromListbox(ItemsListBox, this);

            //System.Console.WriteLine("ListBoxItems Visible: {0}", list.Count);

            var t = Task.Run(async () =>
            {
                foreach (var item in list)
                {
                        if (item.IsLoaded == false && item.IsLoading == false)
                        {
                            await item.LoadModel();
                        }
                }
            });

            t.Wait();
        }
    }
}
