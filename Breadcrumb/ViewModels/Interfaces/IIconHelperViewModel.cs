using Breadcrumb.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breadcrumb.ViewModels.Interfaces
{
    public interface IIconHelperViewModel
    {
        IIconHelper Size16 { get; }
        IIconHelper Size24 { get; }
        IIconHelper Size32 { get; }
        IIconHelper Size48 { get; }
        IIconHelper Size64 { get; }
        IIconHelper Size128 { get; }
        IIconHelper Size256 { get; }

        Task RefreshAsync();
    }
}
