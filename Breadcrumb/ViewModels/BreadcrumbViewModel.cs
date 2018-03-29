namespace Breadcrumb.ViewModels
{
    using Breadcrumb.DirectoryInfoEx;
    using Breadcrumb.ViewModels.Interfaces;
    using BreadcrumbLib.Controls;
    using BreadcrumbLib.Utils;
    using BreadcrumbLib.ViewModels;
    using System.IO;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Class implements the viewmodel that manages the complete breadcrump control.
    /// </summary>
    public class BreadcrumbViewModel : ViewAttached
	{
		#region fields
		private bool mEnableBreadcrumb;
		private string _suggestedPath;

///        private SuggestBoxBase _sbox;   // Controls attached view IAttachView
        private Switch _switch;
        private DropDownList _bexp;
        
////        private IEnumerable<ISuggestSource> _suggestSources;
        #endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		public BreadcrumbViewModel()
		{
			this.BreadcrumbSubTree = new ExTreeNodeViewModel();
			(this.BreadcrumbSubTree.Selection as ITreeRootSelector<ExTreeNodeViewModel, FileSystemInfoEx>).SelectAsync(DirectoryInfoEx.FromString(@"C:\temp"));

			mEnableBreadcrumb = true;
		}
		#endregion constructors

		#region properties
		/// <summary>
		/// Gets a viewmodel that manages the sub-tree brwosing and
		/// selection within the sub-tree component
		/// </summary>
		public ExTreeNodeViewModel BreadcrumbSubTree { get; private set; }

		/// <summary>
		/// Gets/sets a property that determines whether a breadcrumb
		/// switch is turned on or off.
		/// 
		/// On false: A Breadcrumb switch turned off shows the text editable path
		///  On true: A Breadcrumb switch turned  on shows the BreadcrumbSubTree for browsing
		/// </summary>
		public bool EnableBreadcrumb
		{
			get
			{ 
				return mEnableBreadcrumb;
			}

			set
			{
				if (mEnableBreadcrumb != value)
				{
					mEnableBreadcrumb = value;
					NotifyOfPropertyChange(() => EnableBreadcrumb);
				}
			}
		}

		public string SuggestedPath
		{
			get { return _suggestedPath; }
			set
			{
				_suggestedPath = value;

				NotifyOfPropertyChange(() => SuggestedPath);
				OnSuggestPathChanged();
			}
		}

        /// <summary>
        /// Contains a list of items that maps into the SuggestBox control.
        /// </summary>
////        public IEnumerable<ISuggestSource> SuggestSources
////        {
////            get
////            {
////                return _suggestSources;
////            }
////            set
////            {
////                _suggestSources = value;
////                NotifyOfPropertyChange(() => SuggestSources);
////            }
////        }
        #endregion properties

		#region methods
        /// <summary>
        /// Caliburn Micro Framework class method
        /// </summary>
        /// <param name="view"></param>
        /// <param name="context"></param>
        public override void OnViewAttached(object view, object context)
        {
            if (!IsViewAttached)
            {
                var userControl = view as UserControl;
                if (userControl == null)
                    return;

///                _sbox = FindNamedControl(userControl, "PART_SuggestBox") as SuggestBoxBase;    // sbox
                _switch = FindNamedControl(userControl, "PART_Switch") as Switch;             // switch
                _bexp = FindNamedControl(userControl, "PART_DropDownList") as DropDownList;  // bexp

                _bexp.AddValueChanged(ComboBox.SelectedValueProperty, (o, e) =>
                {
                    IEntryViewModel evm = _bexp.SelectedItem as IEntryViewModel;
////                    if (evm != null)
////                        BroadcastDirectoryChanged(evm);

                    _switch.Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        _switch.IsSwitchOn = true;
                    }));
                });

                _switch.AddValueChanged(Switch.IsSwitchOnProperty, (o, e) =>
                {
                    if (!_switch.IsSwitchOn)
                    {
////                        _sbox.Dispatcher.BeginInvoke(new System.Action(() =>
////                        {
////                            Keyboard.Focus(_sbox);
////                            _sbox.Focus();
////                            _sbox.SelectAll();
////                        }), System.Windows.Threading.DispatcherPriority.Background);

                    }
                });

                var uiEle = view as System.Windows.UIElement;
////                this.Commands.RegisterCommand(uiEle, ScriptBindingScope.Local);
            }

            base.OnViewAttached(view, context);

////            _sbox.AddHandler(TextBlock.LostFocusEvent, (RoutedEventHandler)((s, e) =>
////            {
////                if (!_switch.IsSwitchOn)
////                {
////                    _switch.Dispatcher.BeginInvoke(new System.Action(() =>
////                    {
////                        _switch.IsSwitchOn = true;
////                    }));
////                }
////
////            }));
        }

        /// <summary>
        /// Finds a control in the Template property of the given <seealso cref="UserControl"/>
        /// or in the given <seealso cref="UserControl"/>.
        /// </summary>
        /// <param name="userControl"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private object FindNamedControl(UserControl userControl, string name)
        {
            if (userControl.Template != null)
            {
                object item = userControl.Template.FindName(name, userControl);  // sbox

                if (item != null)
                    return item;
            }

            return userControl.FindName(name);
        }

		/// <summary>
		/// Method exectues when the text path portion in the
		/// Breadcrumb control has been edit.
		/// </summary>
		private void OnSuggestPathChanged()
		{
			/***
			if (!ShowBreadcrumb)
			{
				Task.Run(async () =>
				{
					foreach (var p in _profiles)
						if (p.MatchPathPattern(SuggestedPath))
						{
							if (String.IsNullOrEmpty(SuggestedPath) && Entries.AllNonBindable.Count() > 0)
								SuggestedPath = Entries.AllNonBindable.First().EntryModel.FullPath;

							var found = await p.ParseThenLookupAsync(SuggestedPath, CancellationToken.None);
							if (found != null)
							{
								_sbox.Dispatcher.BeginInvoke(new System.Action(() => { SelectAsync(found); }));
								ShowBreadcrumb = true;
								BroadcastDirectoryChanged(EntryViewModel.FromEntryModel(found));
							}
							//else not found
						}
				});//.Start();
			}
			 ***/
		}
		#endregion methods
	}
}
