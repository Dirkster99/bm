namespace BreadcrumbLib.Models
{
    using BreadcrumbLib.Interfaces;
    using BreadcrumbLib.Profile;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class ProfileSuggestionSource : ISuggestSource
    {
        #region Constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="profile"></param>
        public ProfileSuggestionSource(IProfile profile)
        {
            _profile = profile;
        }
        #endregion

        #region Methods

        public async Task<IList<object>> SuggestAsync(object data, string input, IHierarchyHelper helper)
        {
            string dir = input.EndsWith(_profile.Path.Separator + "") ? input : _profile.Path.GetDirectoryName(input);
            string searchStr = _profile.Path.GetFileName(input);

            if (String.IsNullOrEmpty(searchStr) && input.EndsWith(_profile.Path.Separator + ""))
                searchStr += _profile.Path.Separator;

            if (dir == "" && input.EndsWith(_profile.Path.Separator + ""))
                dir = searchStr;
            var found = await _profile.ParseAsync(dir);
            List<object> retVal = new List<object>();

            if (found != null)
            {
                if (_cts != null)
                    _cts.Cancel();
                var cts = _cts = new CancellationTokenSource();
                foreach (var item in await _profile.ListAsync(found, _cts.Token, em => em.IsDirectory))
                {
                    if (cts.IsCancellationRequested)
                        break;
                    if (item.FullPath.StartsWith(input, StringComparison.CurrentCultureIgnoreCase) &&
                        !item.FullPath.Equals(input, StringComparison.CurrentCultureIgnoreCase))
                        retVal.Add(item);
                }
                if (cts.IsCancellationRequested)
                    return new List<object>();
            }

            return retVal;
        }

        #endregion

        #region Data
        private IProfile _profile;
        private CancellationTokenSource _cts;


        #endregion

        #region Public Properties

        #endregion

    }
}
