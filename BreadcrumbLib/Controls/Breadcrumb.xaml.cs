namespace BreadcrumbLib.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using BreadcrumbLib.Utils;
    using System.Windows.Data;
    using BreadcrumbLib.Interfaces;
    using System.Threading;


    #region ToStringConverter
    public class ToStringConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? "" : value.ToString();
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    #endregion

    /// <summary>
    /// Display a ToggleButton and when it's clicked, show it's content as a dropdown.
    /// </summary>
    public class Breadcrumb : UserControl
    {
        #region fields
        public enum BreadcrumbStates { Refresh, NavigateTo, IsLoading, Updating }

        public static readonly DependencyProperty SelectedValueProperty =
          DependencyProperty.Register("SelectedValue", typeof(object),
          typeof(Breadcrumb), new UIPropertyMetadata(null, OnSelectedValueChanged));

        public static readonly DependencyProperty DisplayMemberPathProperty =
            SuggestBox.SuggestBox2.DisplayMemberPathProperty.AddOwner(typeof(Breadcrumb));

        public static readonly DependencyProperty ValuePathProperty =
            SuggestBox.SuggestBox2.ValuePathProperty.AddOwner(typeof(Breadcrumb));

        /// <summary>
        /// For internal use only.
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
          DependencyProperty.Register("SelectedValuePath", typeof(string),
          typeof(Breadcrumb), new UIPropertyMetadata("", OnSelectedValueChanged));

        public static readonly DependencyProperty PathConverterProperty =
         DependencyProperty.Register("PathConverter", typeof(IValueConverter),
         typeof(Breadcrumb), new UIPropertyMetadata(new ToStringConverter()));

        public static readonly DependencyProperty PathSuggestSourceProperty =
         DependencyProperty.Register("PathSuggestSource", typeof(ISuggestSource),
         typeof(Breadcrumb), new UIPropertyMetadata(null));

        public static readonly DependencyProperty BreadcrumbStateProperty =
                DependencyProperty.Register("BreadcrumbState", typeof(BreadcrumbStates),
                typeof(Breadcrumb), new UIPropertyMetadata(BreadcrumbStates.Refresh));

        public static readonly DependencyProperty IsLoadingProperty =
                DependencyProperty.Register("IsLoading", typeof(bool),
                typeof(Breadcrumb), new UIPropertyMetadata(false, new PropertyChangedCallback(OnStateChanged)));

        public static readonly DependencyProperty IsUpdatingProperty =
             DependencyProperty.Register("IsUpdating", typeof(bool),
             typeof(Breadcrumb), new UIPropertyMetadata(false, new PropertyChangedCallback(OnStateChanged)));

        public static readonly DependencyProperty IsEditingProperty =
                DependencyProperty.Register("IsEditing", typeof(bool),
                typeof(Breadcrumb), new UIPropertyMetadata(false, new PropertyChangedCallback(OnStateChanged)));

        public static readonly DependencyProperty CancelCommandProperty =
                       DependencyProperty.Register("CancelCommand", typeof(ICommand),
                       typeof(Breadcrumb));

        public static readonly DependencyProperty RefreshCommandProperty =
                               DependencyProperty.Register("RefreshCommand", typeof(ICommand),
                               typeof(Breadcrumb));


        #endregion

        #region constructors
        /// <summary>
        /// Static constructor
        /// </summary>
        static Breadcrumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Breadcrumb),
                    new System.Windows.FrameworkPropertyMetadata(typeof(Breadcrumb)));
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public Breadcrumb()
        {
            
        }
        #endregion constructors

        #region methods

        public static void OnStateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            Breadcrumb b = (sender as Breadcrumb);
            if (b.IsUpdating)
                b.BreadcrumbState = BreadcrumbStates.Updating;
            else
                if (b.IsEditing)
                    b.BreadcrumbState = BreadcrumbStates.NavigateTo;
                else if (b.IsLoading)
                    b.BreadcrumbState = BreadcrumbStates.IsLoading;
                else b.BreadcrumbState = BreadcrumbStates.Refresh;
        }

        public static void OnSelectedValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            Breadcrumb b = (sender as Breadcrumb);
            if (b.PathConverter != null && !b.IsUpdating)
            {
                b.IsUpdating = true;

                if (b.IsEditing)
                    b.SelectedValue = b.PathConverter.ConvertBack(b.SelectedValuePath, typeof(object), null, Thread.CurrentThread.CurrentCulture);
                else
                    b.SelectedValuePath = (string)b.PathConverter.Convert(b.SelectedValue, typeof(string), null, Thread.CurrentThread.CurrentCulture);

                b.IsUpdating = false;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var toggle = this.Template.FindName("navtoggle", this) as ToggleButton;
            var sbox = this.Template.FindName("sbox", this) as BreadcrumbLib.Controls.SuggestBox.SuggestBox2;

            toggle.AddValueChanged(ToggleButton.IsCheckedProperty,
               (o, e) =>
               {
                   if (IsEditing)
                   {
                       sbox.Focus();
                       sbox.SelectAll();
                   }
               });
            sbox.AddHandler(SuggestBox.SuggestBox2.ValueChangedEvent, (RoutedEventHandler)((o, e) =>
            {
                IsEditing = false;
            }));

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, (ExecutedRoutedEventHandler)((o,e) =>
            {
                sbox.updateSource();                
            })));
            //sbox.AddHandler(TextBox.LostFocusEvent, (RoutedEventHandler)((o, e) =>
            //{
            //    IsEditing = false;
            //}));
        }

        #endregion


        #region DependencyProperties

        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        internal string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        public IValueConverter PathConverter
        {
            get { return (IValueConverter)GetValue(PathConverterProperty); }
            set { SetValue(PathConverterProperty, value); }
        }

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { this.SetValue(DisplayMemberPathProperty, value); }
        }

        public string ValuePath
        {
            get { return (string)GetValue(ValuePathProperty); }
            set { this.SetValue(ValuePathProperty, value); }
        }

        public ISuggestSource PathSuggestSource
        {
            get { return (ISuggestSource)GetValue(PathSuggestSourceProperty); }
            set { SetValue(PathSuggestSourceProperty, value); }
        }

        public BreadcrumbStates BreadcrumbState
        {
            get { return (BreadcrumbStates)GetValue(BreadcrumbStateProperty); }
            set { this.SetValue(BreadcrumbStateProperty, value); }
        }

        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { this.SetValue(IsEditingProperty, value); }
        }

        public bool IsUpdating
        {
            get { return (bool)GetValue(IsUpdatingProperty); }
            set { this.SetValue(IsUpdatingProperty, value); }
        }

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { this.SetValue(IsLoadingProperty, value); }
        }


        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { this.SetValue(CancelCommandProperty, value); }
        }

        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { this.SetValue(RefreshCommandProperty, value); }
        }

        #endregion
    }
}
