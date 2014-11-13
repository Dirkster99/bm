using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Breadcrumb.ViewModels.Helpers
{
    public class NullIconHelper : IconHelper
    {
        public NullIconHelper()
            : base(null)
        {

        }

        protected override Task<ImageSource> loadIconAsync()
        {
            return null;
        }
    }
}
