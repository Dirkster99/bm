namespace BreadcrumbLib.Models.DiskIO
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDiskIOHelper
    {
        IDiskPathMapper Mapper { get; }
        IDiskProfile Profile { get; }

////        [Obsolete]
////        IScriptCommand GetTransferCommand(IEntryModel srcModel, IEntryModel destDirModel, bool removeOriginal);

////        IScriptCommand GetTransferCommand(string sourceKey, string destinationDirKey, string destinationKey, bool removeOriginal, IScriptCommand nextCommand);

        Task<Stream> OpenStreamAsync(IEntryModel entryModel, BreadcrumbLib.Defines.FileAccess access, CancellationToken ct);
        Task DeleteAsync(IEntryModel entryModel, CancellationToken ct);
        Task<IEntryModel> RenameAsync(IEntryModel entryModel, string newName, CancellationToken ct);
        Task<IEntryModel> CreateAsync(string fullPath, bool isDirectory, CancellationToken ct);

    }
}
