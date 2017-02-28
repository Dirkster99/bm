
namespace BreadcrumbLib.Models.DiskIO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class HardDriveDiskIOHelper : DiskIOHelperBase
    {
        public HardDriveDiskIOHelper(IDiskProfile profile)
            : base(profile)
        {

        }

        public override async Task<IEntryModel> RenameAsync(IEntryModel entryModel, string newName, CancellationToken ct)
        {
            string destPath = Path.Combine(Path.GetDirectoryName(entryModel.FullPath), newName);
            if (!entryModel.IsDirectory)
                File.Move(entryModel.FullPath, destPath);
            else if (Directory.Exists(entryModel.FullPath))
                Directory.Move(entryModel.FullPath, destPath);
            return await Profile.ParseAsync(destPath);
        }

        public override async Task DeleteAsync(IEntryModel entryModel, CancellationToken ct)
        {
            if (Directory.Exists(entryModel.FullPath))
                Directory.Delete(entryModel.FullPath, true);
            else File.Delete(entryModel.FullPath);
        }

        public override async Task<Stream> OpenStreamAsync(IEntryModel entryModel,
            BreadcrumbLib.Defines.FileAccess access, CancellationToken ct)
        {
            switch (access)
            {
                case BreadcrumbLib.Defines.FileAccess.Read: return File.OpenRead(entryModel.FullPath);
                case BreadcrumbLib.Defines.FileAccess.Write: return File.OpenWrite(entryModel.FullPath);
                case BreadcrumbLib.Defines.FileAccess.ReadWrite: return File.Open(entryModel.FullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            throw new NotImplementedException();
        }

        public override async Task<IEntryModel> CreateAsync(string fullPath, bool isDirectory, CancellationToken ct)
        {
            if (isDirectory)
                Directory.CreateDirectory(fullPath);
            else
                if (!File.Exists(fullPath))
                using (File.Create(fullPath))
                { }
            return await Profile.ParseAsync(fullPath);
        }
    }
}
