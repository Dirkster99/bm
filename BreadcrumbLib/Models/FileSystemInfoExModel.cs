
namespace BreadcrumbLib.Models
{
    using Profile;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FileSystemInfoExModel : DiskEntryModelBase
    {
        #region Cosntructor

        public FileSystemInfoExModel(IProfile profile, FileSystemInfoEx fsi)
            : base(profile)
        {
            this.Label = fsi.Label;
            this.Attributes = fsi.Attributes;
            this.FullPath = fsi.FullName;
            this.Name = fsi.Name;
            this._isRenamable = true;
            if (fsi is DirectoryInfoEx)
                this.DirectoryType = (fsi as DirectoryInfoEx).DirectoryType;
            this.IsDirectory = fsi.IsFolder;
            if (fsi is FileInfoEx)
                Size = (fsi as FileInfoEx).Length;
            ParentFullPath = fsi.Parent == null ? null : fsi.Parent.FullName;
            this._parentFunc =
                () =>
                {
                    return String.IsNullOrEmpty(ParentFullPath) ? null :
                    new FileSystemInfoExModel(profile,
                        (profile as FileSystemInfoExProfile).createDirectoryInfo(ParentFullPath));
                };

            this.Description = IsDirectory ? "Directory" :
                System.IO.Tools.FileTypeInfoProvider
                .GetFileTypeInfo(profile.Path.GetExtension(Name)).FileType;
            //fsi.GetType().ToString();

            this.CreationTimeUtc = fsi.CreationTimeUtc;
            this.LastUpdateTimeUtc = fsi.LastWriteTimeUtc;
        }

        #endregion

        #region Methods   


        #endregion

        #region Data

        #endregion

        #region Public Properties

        public FileAttributes Attributes { get; protected set; }
        public string ParentFullPath { get; protected set; }
        public DirectoryInfoEx.DirectoryTypeEnum DirectoryType { get; protected set; }

        #endregion
    }
}
