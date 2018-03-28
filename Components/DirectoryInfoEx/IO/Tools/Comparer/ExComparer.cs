using System;
using System.Collections.Generic;
using System.Text;
using NaturalSorting;
using System.IO;
using System.Collections;

namespace System.IO.Tools
{
    public class ExComparer : IComparer<FileSystemInfoEx>, IComparer
    {
        public enum SortCriteria { sortByName, sortByFullName, sortByLabel, sortByType, sortByLength, sortByCreationTime, sortByLastWriteTime, sortByLastAccessTime };
        public enum SortDirectionType { sortAssending, sortDescending };
        public SortCriteria SortBy = SortCriteria.sortByName;
        public SortDirectionType SortDirection = SortDirectionType.sortAssending;
        public bool IsFolderFirst = true;

        private static NaturalComparer nc = new NaturalComparer();

        public ExComparer(SortCriteria criteria)
        {
            SortBy = criteria;
        }

        public ExComparer(SortCriteria criteria, SortDirectionType direction)
        {
            SortBy = criteria;
            SortDirection = direction;
        }

        #region IComparer<FileSystemInfoEx> Members

        public int Compare(FileSystemInfoEx x, FileSystemInfoEx y)
        {
            int retVal = 0;
            if (IsFolderFirst && (x.IsFolder != y.IsFolder))
                retVal = x.IsFolder.CompareTo(y.IsFolder) * -1;
            else
            {
                switch (SortBy)
                {
                    case SortCriteria.sortByType: retVal = nc.Compare(PathEx.GetExtension(x.Name).ToLower(),
                      PathEx.GetExtension(y.Name).ToLower()); break;
                    case SortCriteria.sortByName: retVal = nc.Compare(x.Name, y.Name); break;
                    case SortCriteria.sortByFullName: retVal = nc.Compare(x.FullName, y.FullName); break;
                    case SortCriteria.sortByLabel: retVal = nc.Compare(x.Label, y.Label); break;
                    case SortCriteria.sortByLength:
                        long xSize = x is FileInfoEx ? (x as FileInfoEx).Length : 0;
                        long ySize = y is FileInfoEx ? (y as FileInfoEx).Length : 0;
                        retVal = xSize.CompareTo(ySize); break;
                    case SortCriteria.sortByCreationTime:
                        retVal = x.CreationTime.CompareTo(y.CreationTime); break;
                    case SortCriteria.sortByLastWriteTime:
                        retVal = x.LastWriteTime.CompareTo(y.LastWriteTime); break;
                    case SortCriteria.sortByLastAccessTime:
                        retVal = x.LastAccessTime.CompareTo(y.LastAccessTime); break;
                }
            }
            return SortDirection == SortDirectionType.sortAssending ? retVal : -retVal;
        }

        #endregion

        #region IComparer Members

        public int Compare(object x, object y)
        {            
            if (x is FileSystemInfoEx && y is FileSystemInfoEx)
                return Compare(x as FileSystemInfoEx, y as FileSystemInfoEx);
            return 0;
        }
        #endregion
    }
}
