namespace SuggestBoxTestLib.Views
{
    using SuggestLib.Events;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for DemoView.xaml
    /// </summary>
    public partial class DemoView : UserControl
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public DemoView()
        {
            InitializeComponent();

            RecentList.SelectionChanged += RecentList_SelectionChanged;
            DiskPathSuggestBox.NewLocationRequestEvent += Control_SuggestBox_NewLocationRequestEvent;
        }

        private void RecentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DiskPathSuggestComboBox.Focus();
            Keyboard.Focus(DiskPathSuggestComboBox);
        }

        /// <summary>
        /// Method executes when the SuggestionBox signals that editing location
        /// has been OK'ed (user pressed enter) or cancel'ed (user pressed Escape).
        /// 
        /// These signals are then recorded and processed via IsSwitchOn property
        /// handler which can also be invoked via Toggle Button which is processed
        /// as OK.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_SuggestBox_NewLocationRequestEvent(object sender,
                                                                SuggestLib.Events.NextTargetLocationArgs e)
        {
            // The user requests a new location via SuggestBox textbox key gesture:
            // Enter  -> OK
            // Escape -> Cancel
            switch (e.EditResult.Result)
            {
              case EditPathResult.Cancel:
                    NewLocationRequestEventDisplay.Text =
                                        string.Format("(CANCEL) '{0}'", e.EditResult.NewLocation);
              break;
              
              case EditPathResult.OK:
                    NewLocationRequestEventDisplay.Text =
                                        string.Format("    (OK) '{0}'", e.EditResult.NewLocation);
              break;
              
              default:
                throw new System.NotImplementedException(e.EditResult.Result.ToString());
            }
        }
    }
}
