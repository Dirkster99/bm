using Breadcrumb.Viewmodels.Base;
using Breadcrumb.ViewModels.Interfaces;
using Breadcrumb.ViewModels.ResourceLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breadcrumb.ViewModels.Helpers
{
    public class IconHelperViewModel : NotifyPropertyChanged, IIconHelperViewModel
    {
        public static IIconHelperViewModel FromResourceLoader(IResourceLoader resourceLoader)
        {            
            return new ResourceIconHelperViewModel(resourceLoader);
        }

        public IconHelperViewModel()
        {
            Size16 = Size24 = Size32 = Size48 = Size64 = Size128 = Size256 = IconHelper.Undefined;
        }

        public void Refresh()
        {
            Size16.Refresh();
            Size24.Refresh();
            Size32.Refresh();
            Size48.Refresh();
            Size64.Refresh();
            Size128.Refresh();
            Size256.Refresh();
        }

        public IIconHelper Size16 { get; protected set; }
        public IIconHelper Size24 { get; protected set; }
        public IIconHelper Size32 { get; protected set; }
        public IIconHelper Size48 { get; protected set; }
        public IIconHelper Size64 { get; protected set; }
        public IIconHelper Size128 { get; protected set; }
        public IIconHelper Size256 { get; protected set; }
    }

}
