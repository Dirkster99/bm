namespace BreadcrumbLib.Models
{
    using BreadcrumbLib.Profile;
    using DiskIO;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IDiskProfile : IProfile
    {
        IDiskIOHelper DiskIO { get; }
    }
}
