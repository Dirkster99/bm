namespace Breadcrumb.IconExtractors
{
/***
    using Breadcrumb.IconExtractors.Enums;
    using DirectoryInfoExLib.Enums;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// .Net 2.0 WinForms level icon extractor with cache support.
    /// </summary>
    /// <typeparam name="FSI"></typeparam>
    public abstract class IconExtractor<FSI> : IconExtractor // T may be FileSystemInfo, Ex or ExA
    {
        #region fields
        private Dictionary<Tuple<string, IconSize>, Bitmap> _iconCache = new Dictionary<Tuple<string, IconSize>, Bitmap>();
        private ReaderWriterLock _iconCacheLock = new ReaderWriterLock();
        #endregion fields

        #region constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public IconExtractor()
        {
            this.initCache();
        }
        #endregion constructors

        #region methods
        public bool IsDelayLoading(FSI entry, IconSize size)
        {
            string fastKey, slowKey;
            this.GetIconKey(entry, size, out fastKey, out slowKey);

            return fastKey != slowKey;
        }

        public Bitmap GetIcon(FSI entry, string key, bool isDir, IconSize size)
        {
            Func<string, IconSize, Bitmap> getIconFromCache =
            (k, s) =>
            {
                Tuple<string, IconSize> dicKey = new Tuple<string, IconSize>(k, s);

                try
                {
                    _iconCacheLock.AcquireReaderLock(0);

                    if (_iconCache.ContainsKey(dicKey))
                    {
                        lock (_iconCache[dicKey])
                            return _iconCache[dicKey];
                    }
                }
                catch
                {
                    return null;
                }
                finally
                {
                    _iconCacheLock.ReleaseReaderLock();
                }

                return null;
            };

            Action<string, IconSize, Bitmap> addIconToCache =
            (k, s, b) =>
            {
                Tuple<string, IconSize> dicKey = new Tuple<string, IconSize>(k, s);

                if (k.StartsWith("."))
                {
                    try
                    {
                        _iconCacheLock.AcquireWriterLock(Timeout.Infinite);

                        if (!_iconCache.ContainsKey(dicKey))
                            _iconCache.Add(dicKey, b);
                        else
                            _iconCache[dicKey] = b;
                    }
                    finally
                    {
                        _iconCacheLock.ReleaseWriterLock();
                    }
                }
            };

            Bitmap retImg = null;
            retImg = getIconFromCache(key, size);

            ////if (retImg != null && !key.StartsWith("::")) retImg.Save(@"C:\temp\" + "AAA" + key + ".png");

            if (retImg != null)
                return retImg;

            try
            {
                if (key.StartsWith(".")) // ext, retrieve automatically
                    retImg = this.GetGenericIcon(key, size);
                else
                    if (IconExtractor.IsSpecialIcon(key) && File.Exists(key))
                    retImg = this.GetGenericIcon(key, size, isDir, true);
                else
                    retImg = this.GetIconInner(entry, key, size);
            }
            catch (Exception ex)
            {
                retImg = null;
                Debug.WriteLine("IconExtractor.GetIcon" + ex.Message);
            }

            if (retImg != null)
            {
                Size destSize = IconSizeToSize(size);

                if (size == IconSize.jumbo && IconExtractor.IsImageIcon(key))
                    retImg = ImageTools.resizeImage(retImg, destSize, 5);
                else
                    retImg = ImageTools.resizeImage(retImg, destSize, 0);

                addIconToCache(key, size, retImg);
            }

            return retImg;
        }

        public Bitmap GetIcon(string fileName, IconSize size, bool isDir)
        {
            return this.GetBitmap(size, fileName, isDir, true);
        }

        public Bitmap GetIcon(FSI entry, IconSize size, bool isDir, bool fast)
        {
            string fastKey, slowKey;
            this.GetIconKey(entry, size, out fastKey, out slowKey);

            ////return GetIcon(entry, slowKey, size);

            if (fast || size <= IconSize.large)
                return this.GetIcon(entry, fastKey, isDir, size);
            else
            {
                Bitmap icon = this.GetIcon(entry, slowKey, isDir, size);

                return icon;
            }
        }

        protected abstract void GetIconKey(FSI entry, IconSize size, out string fastKey, out string slowKey);

        protected abstract Bitmap GetIconInner(FSI entry, string key, IconSize size);

        protected void initCache()
        {
            Action<IconSize> addToDic = (size) =>
            {
                Tuple<string, IconSize> iconKey = new Tuple<string, IconSize>(tempPath, size);
                _iconCache.Add(iconKey, this.GetGenericIcon(tempPath, size, true, false));
            };

            lock (_iconCache)
            {
                foreach (IconSize size in Enum.GetValues(typeof(IconSize)))
                    addToDic(size);
            }
        }
        #endregion methods
    }
***/
}
