namespace BreadcrumbLib.Controls
{
    using BreadcrumbLib.Controls.SuggestBox;
    using BreadcrumbLib.Interfaces;
    using BreadcrumbLib.ViewModels;
    using System.Windows;
    using System.Windows.Controls;

	/// <summary>
	/// Display a ToggleButton and when it's clicked, show it's content as a dropdown.
	/// </summary>
    [TemplatePart(Name = "PART_SuggestBox", Type = typeof(SuggestBoxBase))]
    [TemplatePart(Name = "PART_Switch", Type = typeof(Switch))]
    [TemplatePart(Name = "PART_DropDownList", Type = typeof(DropDownList))]
    public class Breadcrumb : UserControl
    {
        #region fields
        private object mLockObject = new object();
        private bool mIsLoaded = false;
        #endregion fields

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
            this.DataContextChanged += Breadcrumb_DataContextChanged;
            this.Loaded += Breadcrumb_Loaded;
		}
        #endregion constructors

        #region properties
        public SuggestBoxBase Control_SuggestBox { get; set; }

        public Switch Control_Switch { get; set; }

        public DropDownList Control_bexp { get; set; }
        #endregion properties

        #region methods
        /// <summary>
		/// Is called when a control template is applied.
		/// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Control_SuggestBox = this.Template.FindName("PART_SuggestBox", this) as SuggestBoxBase;  // sbox
            this.Control_Switch = this.Template.FindName("PART_Switch", this) as Switch;                 // switch
            this.Control_bexp = this.Template.FindName("PART_DropDownList", this) as DropDownList;      // bexp
        }

        /// <summary>
        /// Method executes when the control has finished loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Breadcrumb_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            lock (this.mLockObject)
            {
                mIsLoaded = true;

                if (DataContext != null)
                    AttachView(DataContext as IViewAttached);
            }
        }

        /// <summary>
        /// Method executes when the datacontext is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Breadcrumb_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            lock (mLockObject)
            {
                if (mIsLoaded == true)
                {
                    if (e.NewValue != null)
                        AttachView(e.NewValue as IViewAttached);
                }              
            }
        }

        /// <summary>
        /// Method executes to simulate the <seealso cref="IAttachView"/>
        /// behaviour from Caliburn.Micro.
        /// </summary>
        /// <param name="viewAttachedViewModel"></param>
        private void AttachView(IViewAttached viewAttachedViewModel)
        {
            if (viewAttachedViewModel == null)
                return;

            viewAttachedViewModel.OnViewAttached(this, null);
        }
        #endregion methods
    }
}
