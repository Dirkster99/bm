namespace BreadcrumbTestLib.ViewModels.Breadcrumbs
{
    using BmLib.Interfaces;

    /// <summary>
    /// Exposes the properties of a Progress Display (ProgressBar) to enable
    /// UI feedback on long running processings.
    /// </summary>
    internal class ProgressViewModel : Base.ViewModelBase, IProgress
    {
        #region fields
        /// <summary>
        /// Log4net logger facility for this class.
        /// </summary>
        protected static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool _IsIndeterminate = true;
        private bool _IsProgressbarVisible = false;
        private double _Progress;
        private double _MinimumValue;
        private double _MaximumValue;
        #endregion fields

        #region properties
        /// <summary>
        /// Gets whether the progress indicator is currenlty displayed or not.
        /// </summary>
        public bool IsProgressbarVisible
        {
            get { return _IsProgressbarVisible; }

            protected set
            {
                if (_IsProgressbarVisible != value)
                {
                    Logger.InfoFormat("Old value {0} -> New value {1}", _IsProgressbarVisible, value);
                    _IsProgressbarVisible = value;

                    NotifyPropertyChanged(() => IsProgressbarVisible);
                }
            }
        }

        /// <summary>
        /// Gets whether the current progress display <seealso cref="IsProgressbarVisible"/>
        /// is indeterminate or not.
        /// </summary>
        public bool IsIndeterminate
        {
            get { return _IsIndeterminate; }

            protected set
            {
                if (_IsIndeterminate != value)
                {
                    Logger.InfoFormat("Old value {0} -> New value {1}", _IsIndeterminate, value);
                    _IsIndeterminate = value;

                    NotifyPropertyChanged(() => IsIndeterminate);
                }
            }
        }

        /// <summary>
        /// Gets the current progress value that should be displayed if the progress is turned on
        /// via <seealso cref="IsProgressbarVisible"/> and is not indeterminated as indicated in
        /// <seealso cref="IsIndeterminate"/>.
        /// </summary>
        public double ProgressValue
        {
            get { return _Progress; }

            protected set
            {
                if (_Progress != value)
                {
                    Logger.InfoFormat("Old value {0} -> New value {1}", _Progress, value);
                    _Progress = value;

                    NotifyPropertyChanged(() => ProgressValue);
                }
            }
        }

        /// <summary>
        /// Gets the minimum progress value that should be displayed if the progress is turned on
        /// via <seealso cref="IsProgressbarVisible"/> and is not indeterminated as indicated in
        /// <seealso cref="IsIndeterminate"/>.
        /// </summary>
        public double MinimumProgressValue
        {
            get { return _MinimumValue; }

            protected set
            {
                if (_MinimumValue != value)
                {
                    Logger.InfoFormat("Old value {0} -> New value {1}", _MinimumValue, value);
                    _MinimumValue = value;

                    NotifyPropertyChanged(() => MinimumProgressValue);
                }
            }
        }

        /// <summary>
        /// Gets the maximum progress value that should be displayed if the progress is turned on
        /// via <seealso cref="IsProgressbarVisible"/> and is not indeterminated as indicated in
        /// <seealso cref="IsIndeterminate"/>.
        /// </summary>
        public double MaximumProgressValue
        {
            get { return _MaximumValue; }

            protected set
            {
                if (_MaximumValue != value)
                {
                    Logger.InfoFormat("Old value {0} -> New value {1}", _MaximumValue, value);
                    _MaximumValue = value;

                    NotifyPropertyChanged(() => MaximumProgressValue);
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Method enables properties such that display of
        /// indeterminate progress is turned on.
        /// </summary>
        public void ShowIndeterminatedProgress()
        {
            Logger.InfoFormat("_");

            IsIndeterminate = true;
            IsProgressbarVisible = true;
        }

        /// <summary>
        /// Method enables properties such that display of
        /// determinate progress is turned on.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        public void ShowDeterminatedProgress(double value,
                                             double minimum = 0,
                                             double maximum = 100)
        {
            Logger.InfoFormat("_");

            ProgressValue = value;
            MinimumProgressValue = minimum;
            MaximumProgressValue = maximum;

            IsIndeterminate = false;
            IsProgressbarVisible = true;
        }

        /// <summary>
        /// Method updates a display of determinate progress
        /// which should previously been turned on via
        /// <seealso cref="ShowDeterminatedProgress(double, double, double)"/>
        /// is turned on.
        /// </summary>
        /// <param name="value"></param>
        public void UpdateDeterminatedProgress(double value)
        {
            Logger.InfoFormat("_");

            ProgressValue = value;

            IsIndeterminate = false;
            IsProgressbarVisible = true;
        }

        /// <summary>
        /// Method turns the current progress display off.
        /// </summary>
        public void ProgressDisplayOff()
        {
            Logger.InfoFormat("_");

            IsProgressbarVisible = false;
        }
        #endregion methods
    }
}
