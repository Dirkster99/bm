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
       
        public IconHelperViewModel()
        {
            Size16 = Size24 = Size32 = Size48 = Size64 = Size128 = Size256 = IconHelper.Undefined;
        }

        public async Task RefreshAsync()
        {
            await Task.WhenAll(
            Size16.RefreshAsync(),
            Size24.RefreshAsync(),
            Size32.RefreshAsync(),
            Size48.RefreshAsync(),
            Size64.RefreshAsync(),
            Size128.RefreshAsync(),
            Size256.RefreshAsync());
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
