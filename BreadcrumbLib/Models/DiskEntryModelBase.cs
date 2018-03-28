namespace BreadcrumbLib.Models
{
    using Profile;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using WPF;

    public class DiskEntryModelBase : EntryModelBase
    {
        public DiskEntryModelBase(IProfile profile)
            : base(profile)
        {
            DiskProfile = profile as IDiskProfile;
            if (DiskProfile == null)
                throw new ArgumentException();
        }

        protected override void OnRenamed(string orgName, string newName)
        {
            //string dirName = this.Profile.Path.GetDirectoryName(this.FullPath);
            DiskProfile.DiskIO.RenameAsync(this, newName, CancellationToken.None);
            string newPath = DiskProfile.Path.Combine(DiskProfile.Path.GetDirectoryName(FullPath), newName);
///            DiskProfile.NotifyEntryChanges(this, newPath, BreadcrumbLib.Defines.ChangeType.Moved, this.FullPath);
            //new ScriptRunner().RunAsync(new ParameterDic(), this.Rename(newName));
            //Then DiskIO raise NotifyChange, and refresh the ExplorerViewModel.
        }

        public IDiskProfile DiskProfile { get; private set; }
        public long Size { get; protected set; }

    }
}
