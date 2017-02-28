namespace BreadcrumbLib.ViewModels
{
    using BreadcrumbLiv.Viewmodels.Base;

    /// <summary>
	/// Class implements ...
	/// </summary>
    public class ViewAttached : NotifyPropertyChanged, IViewAttached
	{
		private bool _isViewAttached = false;

		public bool IsViewAttached
		{
			get
			{
				return _isViewAttached;
			}

			private set
			{
				 _isViewAttached = value;
			}
		}

		////protected override void OnViewAttached(object view, object context)
        /// <summary>
        /// Caliburn Micro Framework class method
        /// </summary>
        /// <param name="view"></param>
        /// <param name="context"></param>
		public virtual void OnViewAttached(object view, object context)
		{
			IsViewAttached = true;
			////base.OnViewAttached(view, context);
		}
	}
}
