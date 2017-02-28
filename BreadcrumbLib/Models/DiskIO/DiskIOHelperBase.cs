namespace BreadcrumbLib.Models.DiskIO
{
    using IO;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class DiskIOHelperBase : IDiskIOHelper
    {

        protected DiskIOHelperBase(IDiskProfile profile)
        {
            Mapper = new IODiskPatheMapper();
            Profile = profile;
        }

        public IDiskProfile Profile { get; protected set; }
        public IDiskPathMapper Mapper { get; protected set; }


        public virtual Task<Stream> OpenStreamAsync(IEntryModel entryModel,
            BreadcrumbLib.Defines.FileAccess access, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(IEntryModel entryModel, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEntryModel> RenameAsync(IEntryModel entryModel, string newName, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEntryModel> CreateAsync(string fullPath, bool isDirectory, CancellationToken ct)
        {
            throw new NotImplementedException();
        }


////        [Obsolete]
////        public virtual IScriptCommand GetTransferCommand(IEntryModel srcModel, IEntryModel destDirModel, bool removeOriginal)
////        {
////            return IOScriptCommands.DiskTransfer(srcModel, destDirModel, removeOriginal, false);
////        }
////
////        public virtual IScriptCommand GetTransferCommand(string sourceKey, string destinationDirKey, string destinationKey,
////            bool removeOriginal, IScriptCommand nextCommand)
////        {
////            return IOScriptCommands.DiskTransfer(sourceKey, destinationDirKey, destinationKey, removeOriginal, false, nextCommand);
////        }
    }
}
