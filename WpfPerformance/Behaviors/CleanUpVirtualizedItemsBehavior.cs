namespace WpfPerformance.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public static class CleanUpVirtualizedItemsBehavior
    {
        // Using a DependencyProperty as the backing store for CleanUpItemCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CleanUpItemCommandProperty =
            DependencyProperty.RegisterAttached("CleanUpItemCommand",
                typeof(ICommand), typeof(CleanUpVirtualizedItemsBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static ICommand GetCleanUpItemCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CleanUpItemCommandProperty);
        }

        public static void SetCleanUpItemCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CleanUpItemCommandProperty, value);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListBox;
            if (control == null)
                return;

            VirtualizingStackPanel.RemoveCleanUpVirtualizedItemHandler(control, OnCleanUpEvent);

            if ((e.NewValue as ICommand) != null)
                VirtualizingStackPanel.AddCleanUpVirtualizedItemHandler(control, OnCleanUpEvent);
        }

        private static void OnCleanUpEvent(object sender, CleanUpVirtualizedItemEventArgs e)
        {
            var control = sender as ListBox;
            if (control == null)
                return;

            ICommand cleanupCommand = CleanUpVirtualizedItemsBehavior.GetCleanUpItemCommand(control);

            // Check whether this attached behaviour is bound to a RoutedCommand
            if (cleanupCommand is RoutedCommand)
            {
                // Execute the routed command
                (cleanupCommand as RoutedCommand).Execute(e.Value, control);
            }
            else
            {
                // Execute the Command as bound delegate
                cleanupCommand.Execute(e.Value);
            }
        }
    }
}
