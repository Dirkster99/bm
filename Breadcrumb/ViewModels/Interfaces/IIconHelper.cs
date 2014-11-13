using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Breadcrumb.ViewModels.Interfaces
{
    public interface IIconHelper
    {
        int? Size { get; }
        ImageSource Value { get; }
        void Refresh();
    }
}
