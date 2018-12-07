namespace GenericSuggestBoxDemo
{
    using log4net;
    using log4net.Config;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region fields
        protected static log4net.ILog Logger;
        #endregion fields

        #region constructors
        /// <summary>
        /// Static class constructor
        /// </summary>
        static App()
        {
            XmlConfigurator.Configure();
            Logger = LogManager.GetLogger("default");
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public App()
        {
        }
        #endregion constructors

    }
}
