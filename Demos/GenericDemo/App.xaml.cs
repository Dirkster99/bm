namespace GenericDemo
{
	using System.Windows;
    using log4net.Config;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
	{
        protected static log4net.ILog Logger; // Enable debugging via Log4Net

        static App()
        {
            XmlConfigurator.Configure();   // Read Log4Net config from App.config file
            Logger = log4net.LogManager.GetLogger("default");
        }

        public App()
		{
		}
	}
}
