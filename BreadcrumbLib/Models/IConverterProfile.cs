namespace BreadcrumbLib.Models
{
    using BreadcrumbLib.Profile;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Profile that reside in another profile to convert entry model when listing to a different entry model.    
    /// </summary>
    public interface IConverterProfile : IProfile
    {
        /// <summary>
        /// This is set in ProfileBase.
        /// </summary>
        /// <param name="profile"></param>
        void SetOwner(IProfile profile);
        IEntryModel Convert(IEntryModel entryModel);
    }
}
