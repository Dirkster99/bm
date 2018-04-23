namespace BreadcrumbTestLib.Demo.Converter
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Windows.Media;

    /// <summary>
    /// XAML markup extension to convert <seealso cref="FSItemType"/> enum members
    /// into <seealso cref="ImageSource"/> from ResourceDictionary or fallback from static resource.
    /// </summary>
    [ValueConversion(typeof(System.Environment.SpecialFolder), typeof(ImageSource))]
    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    public class BrowseItemTypeToImageConverter : MarkupExtension, IMultiValueConverter
    {
        #region fields
        private static BrowseItemTypeToImageConverter converter;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public BrowseItemTypeToImageConverter()
        {
        }
        #endregion constructor

        #region methods
        /// <summary>
        /// Returns an object that is provided
        /// as the value of the target property for this markup extension.
        /// 
        /// When a XAML processor processes a type node and member value that is a markup extension,
        /// it invokes the ProvideValue method of that markup extension and writes the result into the
        /// object graph or serialization stream. The XAML object writer passes service context to each
        /// such implementation through the serviceProvider parameter.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (converter == null)
            {
                converter = new BrowseItemTypeToImageConverter();
            }

            return converter;
        }

        /// <summary>
        /// Converts a <seealso cref="FSItemType"/> enumeration member
        /// into a dynamic resource or a fallback image Url (if dynamic resource is not available).
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            if (values.Length != 1)
                return Binding.DoNothing;

            ////bool? bIsExpanded = values[0] as bool?;
            System.Environment.SpecialFolder? itemType = values[0] as System.Environment.SpecialFolder?;

            ////if (bIsExpanded == null && itemType == null)
            ////{
            ////  bIsExpanded = values[0] as bool?;
            itemType = values[0] as System.Environment.SpecialFolder?;

            if (itemType == null) //// bIsExpanded == null && 
                return Binding.DoNothing;
            ////}

            ////if (bIsExpanded == true)
            return this.GetExpandedImages((System.Environment.SpecialFolder)itemType);
            ////else
            ////  return this.GetNotExpandedImages((FSItemType)itemType);
        }

        /// <summary>
        /// Converts back method is not implemented and will throw an exception.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a DynamicResource from ResourceDictionary or a static ImageSource (as fallback) for not expanded folder item.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        /*
            private object GetNotExpandedImages(FSItemType itemType)
            {
              string uriPath = null;

              switch (itemType)
              {
                case FSItemType.Folder:
                  uriPath = string.Format("FolderItem_Image_{0}", "FolderClosed");
                  break;

                case FSItemType.LogicalDrive:
                  uriPath = string.Format("FolderItem_Image_{0}", "HardDisk");
                  break;

                default:
                case FSItemType.File:
                case FSItemType.Unknown:
                  Logger.Error("Type of item is not supported:" + itemType.ToString());
                  break;
              }

              object item = null;

              if (uriPath != null)
              {
                item = Application.Current.Resources[uriPath];

                if (item != null)
                  return item;
              }

              string pathValue = null;

              switch (itemType)
              {
                case FSItemType.Folder:
                  pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/FolderClosed.png";
                  break;

                case FSItemType.LogicalDrive:
                  pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/HardDisk.ico";
                  break;

                default:
                case FSItemType.File:
                case FSItemType.Unknown:
                  Logger.Error("Type of item is not supported:" + itemType.ToString());
                  break;
              }

              if (pathValue != null)
              {
                try
                {
                  Uri imagePath = new Uri(pathValue, UriKind.RelativeOrAbsolute);
                  ImageSource source = new System.Windows.Media.Imaging.BitmapImage(imagePath);

                  return source;
                }
                catch
                {
                }
              }

              // Attempt to load fallback folder from ResourceDictionary
              item = Application.Current.Resources[string.Format("FolderItem_Image_{0}", "FolderClosed")];

              if (item != null)
                return item;
              else
              {
                // Attempt to load fallback folder from fixed Uri
                pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/FolderClosed.png";

                try
                {
                  Uri imagePath = new Uri(pathValue, UriKind.RelativeOrAbsolute);
                  ImageSource source = new System.Windows.Media.Imaging.BitmapImage(imagePath);

                  return source;
                }
                catch
                {
                }
              }

              return null;
            }
        */
        /// <summary>
        /// Get a DynamicResource from ResourceDictionary or a static ImageSource (as fallback) for expanded folder item.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        private object GetExpandedImages(System.Environment.SpecialFolder itemType)
        {
            string uriPath = null;

            const string prefix = "SpecialFolder";

            switch (itemType)
            {
                case Environment.SpecialFolder.AdminTools:
                    uriPath = string.Format("{0}_{1}", prefix, "Default");
                    break;
                case Environment.SpecialFolder.ApplicationData:
                    uriPath = string.Format("{0}_{1}", prefix, "ApplicationData");
                    break;

                case Environment.SpecialFolder.CDBurning:
                case Environment.SpecialFolder.CommonAdminTools:
                case Environment.SpecialFolder.CommonApplicationData:
                case Environment.SpecialFolder.CommonDesktopDirectory:
                case Environment.SpecialFolder.CommonDocuments:
                case Environment.SpecialFolder.CommonMusic:
                case Environment.SpecialFolder.CommonOemLinks:
                case Environment.SpecialFolder.CommonPictures:
                case Environment.SpecialFolder.CommonProgramFiles:
                case Environment.SpecialFolder.CommonProgramFilesX86:
                case Environment.SpecialFolder.CommonPrograms:
                case Environment.SpecialFolder.CommonStartMenu:
                case Environment.SpecialFolder.CommonStartup:
                case Environment.SpecialFolder.CommonTemplates:
                case Environment.SpecialFolder.CommonVideos:
                case Environment.SpecialFolder.Cookies:
                    uriPath = string.Format("{0}_{1}", prefix, "Default");
                    break;

                case Environment.SpecialFolder.Desktop:
                case Environment.SpecialFolder.DesktopDirectory:
                    uriPath = string.Format("{0}_{1}", prefix, "DesktopFolder");
                    break;

                case Environment.SpecialFolder.Favorites:
                    uriPath = string.Format("{0}_{1}", prefix, "Favourites");
                    break;

                case Environment.SpecialFolder.Fonts:
                    uriPath = string.Format("{0}_{1}", prefix, "Fonts");
                    break;

                case Environment.SpecialFolder.History:
                case Environment.SpecialFolder.InternetCache:
                case Environment.SpecialFolder.LocalApplicationData:
                case Environment.SpecialFolder.LocalizedResources:
                    uriPath = string.Format("{0}_{1}", prefix, "Default");
                    break;

                case Environment.SpecialFolder.MyComputer:
                    // SpecialFolder_MyComputer
                    uriPath = string.Format("{0}_{1}", prefix, "MyComputer");
                    break;

                case Environment.SpecialFolder.MyDocuments:
                    uriPath = string.Format("{0}_{1}", prefix, "MyDocuments");
                    break;

                case Environment.SpecialFolder.MyMusic:
                    uriPath = string.Format("{0}_{1}", prefix, "MyMusic");
                    break;

                case Environment.SpecialFolder.MyPictures:
                    uriPath = string.Format("{0}_{1}", prefix, "MyPictures");
                    break;

                case Environment.SpecialFolder.MyVideos:
                    uriPath = string.Format("{0}_{1}", prefix, "MyVideos");
                    break;

                case Environment.SpecialFolder.NetworkShortcuts:
                    uriPath = string.Format("{0}_{1}", prefix, "Default");
                    break;

                ////case Environment.SpecialFolder.Personal: Enum Id is same as MyDocuments = 5 ?
                //// MyDocuments
                ////  break;
                case Environment.SpecialFolder.PrinterShortcuts:
                case Environment.SpecialFolder.ProgramFiles:
                case Environment.SpecialFolder.ProgramFilesX86:
                case Environment.SpecialFolder.Programs:
                    uriPath = string.Format("{0}_{1}", prefix, "Default");
                    break;

                case Environment.SpecialFolder.Recent:
                    uriPath = string.Format("{0}_{1}", prefix, "RecentPlaces");
                    break;

                case Environment.SpecialFolder.Resources:
                case Environment.SpecialFolder.SendTo:
                case Environment.SpecialFolder.StartMenu:
                case Environment.SpecialFolder.Startup:
                case Environment.SpecialFolder.System:
                case Environment.SpecialFolder.SystemX86:
                case Environment.SpecialFolder.Templates:
                    uriPath = string.Format("{0}_{1}", prefix, "Default");
                    break;

                case Environment.SpecialFolder.UserProfile:
                    uriPath = string.Format("{0}_{1}", prefix, "UserProfile");
                    break;

                case Environment.SpecialFolder.Windows:
                    uriPath = string.Format("{0}_{1}", prefix, "Default");
                    break;

                default:
                    uriPath = string.Format("{0}_{1}", prefix, "Default");
                    break;
            }

            object item = null;

            if (uriPath != null) // Load image from DynamicResource (bingo -> ideal result)
            {
                item = Application.Current.Resources[uriPath];

                if (item != null)
                    return item;
            }

            // Last ressort to finally get an image if everything else fails
            string pathValue = null;

            pathValue = "pack://application:,,,/Themes;component/FolderImages/Light/SpecialFolder_Default.ico";

            try
            {
                Uri imagePath = new Uri(pathValue, UriKind.RelativeOrAbsolute);
                ImageSource source = new System.Windows.Media.Imaging.BitmapImage(imagePath);

                return source;
            }
            catch
            {
            }

            // Attempt to load fallback folder from ResourceDictionary
            item = Application.Current.Resources[string.Format("{0}_{1}.ico", prefix, "Default")];

            if (item != null)
                return item;
            else
            {
                // Attempt to load fallback folder from fixed Uri
                pathValue = "pack://application:,,,/Themes;component/FolderImages/Light/SpecialFolder_Default.ico";

                try
                {
                    Uri imagePath = new Uri(pathValue, UriKind.RelativeOrAbsolute);
                    ImageSource source = new System.Windows.Media.Imaging.BitmapImage(imagePath);

                    return source;
                }
                catch
                {
                }
            }

            return null;
        }
        #endregion methods
    }
}
