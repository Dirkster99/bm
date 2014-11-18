using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Breadcrumb.ViewModels.Interfaces
{
    public interface IIconHelper
    {
        int? Size { get; }
        Stream Value { get; }
        Task RefreshAsync();
    }
}
