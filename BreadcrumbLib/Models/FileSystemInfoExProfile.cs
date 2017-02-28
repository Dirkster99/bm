namespace BreadcrumbLib.Models
{
    using BreadcrumbLib.Defines;
    using Caliburn.Micro;
    using DiskIO;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Utils.WPF;

    public class FileSystemInfoExProfile : DiskProfileBase, IWPFProfile
    {
        #region Constructor

        private class ExHierarchyComparer : IEntryHierarchyComparer
        {
            public ExHierarchyComparer(FileSystemInfoExProfile profile)
            {
                _profile = profile;
            }

            //Store special directories (start with ::{) only.
            private ConcurrentDictionary<string, HierarchicalResult> _hierarchyResultCache
                = new ConcurrentDictionary<string, HierarchicalResult>();
            private FileSystemInfoExProfile _profile;
            private bool HasParent(FileSystemInfoEx child, DirectoryInfoEx parent)
            {
                DirectoryInfoEx current = child.Parent;
                while (current != null)
                {
                    if (current.Equals(parent))
                        return true;
                    current = current.Parent;
                }
                return false;
            }

            public HierarchicalResult CompareHierarchyInner(IEntryModel a, IEntryModel b)
            {
                if (a == null || b == null)
                    return HierarchicalResult.Unrelated;
                if (!_profile.MatchPathPattern(a.FullPath) || !_profile.MatchPathPattern(b.FullPath))
                    return HierarchicalResult.Unrelated;

                if (!a.FullPath.Contains("::") && !b.FullPath.Contains("::"))
                    return PathComparer.LocalDefault.CompareHierarchy(a, b);

                if (a.FullPath.Equals(b.FullPath))
                    return HierarchicalResult.Current;

                while (a != null && !(a is FileSystemInfoExModel))
                    a = a.Parent;
                while (b != null && !(b is FileSystemInfoExModel))
                    b = b.Parent;

                if (a is FileSystemInfoExModel && b is FileSystemInfoExModel)
                {
                    string key = String.Format("{0}-compare-{1}", a.FullPath, b.FullPath);
                    if (_hierarchyResultCache.ContainsKey(key))
                        return _hierarchyResultCache[key];

                    FileSystemInfoEx fsia = FileSystemInfoEx.FromString(a.FullPath);
                    FileSystemInfoEx fsib = FileSystemInfoEx.FromString(b.FullPath);
                    if (a.FullPath == b.FullPath)
                        _hierarchyResultCache[key] = HierarchicalResult.Current;
                    else if (IOTools.HasParent(fsib, fsia.FullName))
                        _hierarchyResultCache[key] = HierarchicalResult.Child;
                    else if (IOTools.HasParent(fsia, fsib.FullName))
                        _hierarchyResultCache[key] = HierarchicalResult.Parent;
                    else _hierarchyResultCache[key] = HierarchicalResult.Unrelated;

                    return _hierarchyResultCache[key];
                }

                return HierarchicalResult.Unrelated;


                //if (fsia is DirectoryInfoEx && HasParent(fsib, fsia as DirectoryInfoEx))
                //    return HierarchicalResult.Child;
                //if (fsib is DirectoryInfoEx && HasParent(fsia, fsib as DirectoryInfoEx))
                //    return HierarchicalResult.Parent;

                //if (a.FullPath.EndsWith(":\\") &&
                //    b.IsDirectory && (b as FileSystemInfoExModel).DirectoryType
                //    != DirectoryInfoEx.DirectoryTypeEnum.dtFolder)
                //    return HierarchicalResult.Unrelated;

                //if (b.FullPath.EndsWith(":\\") &&
                //    a.IsDirectory && (a as FileSystemInfoExModel).DirectoryType
                //    != DirectoryInfoEx.DirectoryTypeEnum.dtFolder)
                //    return HierarchicalResult.Unrelated;

                //if (a.FullPath.EndsWith(":\\") &&
                //   b.FullPath.StartsWith("c:\\Temp\\Cofe3", StringComparison.CurrentCultureIgnoreCase))
                //    return HierarchicalResult.Unrelated;

                //if (b.FullPath.EndsWith(":\\") &&
                //    (a as FileSystemInfoExModel).ParentFullPath != null &&
                //    (!(a as FileSystemInfoExModel).ParentFullPath.Equals(b.FullPath)))
                //    return HierarchicalResult.Unrelated;


            }

            public HierarchicalResult CompareHierarchy(IEntryModel a, IEntryModel b)
            {
                HierarchicalResult retVal = CompareHierarchyInner(a, b);
                //Debug.WriteLine(String.Format("{2} {0},{1}", a.FullPath, b.FullPath, retVal));
                return retVal;
            }
        }

        public static FileSystemInfoExProfile Instance = CreateNew();
        public static FileSystemInfoExProfile CreateNew(params IConverterProfile[] converters)
        {
            return new FileSystemInfoExProfile(null, null, converters);
        }

        public FileSystemInfoExProfile(IEventAggregator events,
            IWindowManager windowManager,
            params IConverterProfile[] converters)
            : base(events, converters)
        {
            ProfileName = "FileSystem (Ex)";
            ProfileIcon = ResourceUtils.GetEmbeddedResourceAsByteArray(this, "/Model/DirectoryInfoEx/My_Computer.png");

            DiskIO = new HardDriveDiskIOHelper(this);
            HierarchyComparer = new ExHierarchyComparer(this);
////            MetadataProvider = new ExMetadataProvider();
////            CommandProviders = new List<ICommandProvider>()
////            {
////                new ExCommandProvider(this)
////            };
            //DragDrop = new FileSystemInfoExDragDropHandler(this);
////            DragDrop = new FileBasedDragDropHandler(this, em => false);
            //PathMapper = IODiskPatheMapper.Instance;

            PathPatterns = new string[] { RegexPatterns.FileSystemGuidPattern, RegexPatterns.FileSystemPattern };
        }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            return obj is FileSystemInfoExProfile;
        }


////        public IComparer<IEntryModel> GetComparer(ColumnInfo column)
////        {
////            return new ValueComparer<IEntryModel>(p => p.FullPath);
////        }

        internal DirectoryInfoEx createDirectoryInfo(string path)
        {
            if (path.EndsWith(":"))
                return new DirectoryInfoEx(path + "\\");
            else return new DirectoryInfoEx(path);
        }

        internal FileInfoEx createFileInfo(string path)
        {
            return new FileInfoEx(path);
        }

        public override async Task<IEntryModel> ParseAsync(string path)
        {
            return await Task<IEntryModel>.Factory.StartNew(() =>
            {
                IEntryModel retVal = null;
                if (String.IsNullOrEmpty(path))
                    retVal = new FileSystemInfoExModel(this, DirectoryInfoEx.DesktopDirectory);
                else
                    if (path.StartsWith("::"))
                    {
                        if (DirectoryEx.Exists(path))
                            retVal = new FileSystemInfoExModel(this, createDirectoryInfo(path));
                        else
                            if (FileEx.Exists(path))
                                retVal = new FileSystemInfoExModel(this, createFileInfo(path));
                    }
                    else
                    {
                        if (Directory.Exists(path))
                            retVal = new FileSystemInfoExModel(this, createDirectoryInfo(path));
                        else
                            if (File.Exists(path))
                                retVal = new FileSystemInfoExModel(this, createFileInfo(path));
                    }
                return Converters.Convert(retVal);
            });
        }

        public override async Task<IList<IEntryModel>> ListAsync(IEntryModel entry, CancellationToken ct, Func<IEntryModel, bool> filter = null, bool refresh = false)
        {
            return await Task<IEntryModel>.Run(() =>
            {
                //await Task.Delay(2000);
                if (filter == null)
                    filter = (m) => true;
                List<IEntryModel> retVal = new List<IEntryModel>();
                if (entry.IsDirectory)
                {
                    DirectoryInfoEx di = createDirectoryInfo(entry.FullPath);
                    retVal.AddRange(from fsi in di.EnumerateFileSystemInfos()
                                    let m = Converters.Convert(new FileSystemInfoExModel(this, fsi))
                                    where filter(m)
                                    select m);
                }
                return (IList<IEntryModel>)retVal;
            });
        }

////        public override IEnumerable<IModelIconExtractor<IEntryModel>> GetIconExtractSequence(IEntryModel entry)
////        {
////            yield return GetDefaultIcon.Instance;
////            yield return GetFromSystemImageList.Instance;
////            if (!entry.IsDirectory)
////                if (entry.FullPath.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) ||
////                    entry.FullPath.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase) ||
////                    entry.FullPath.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase)
////                    )
////                    yield return GetImageFromImageExtractor.Instance;
////        }

        #endregion

        #region Data

        private Bitmap _folderBitmap;
////        private IconExtractor _iconExtractor = new ExIconExtractor();


        #endregion

        #region Public Properties


        #endregion
    }
}
