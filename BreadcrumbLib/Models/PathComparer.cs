namespace BreadcrumbLib.Models
{
    using BreadcrumbLib.Defines;
    using System;

    public class PathComparer : IEntryHierarchyComparer
    {
        #region Cosntructor

        public static PathComparer LocalDefault = new PathComparer('\\', StringComparison.InvariantCultureIgnoreCase);
        public static PathComparer WebDefault = new PathComparer('/', StringComparison.InvariantCultureIgnoreCase);

        public PathComparer(char separator, StringComparison comparsion)
        {
            _separator = separator;
            _stringComparsion = comparsion;
        }

        #endregion

        #region Methods

        public HierarchicalResult CompareHierarchy(IEntryModel a, IEntryModel b)
        {
            if (a == null || b == null || a.FullPath == null || b.FullPath == null)
                return HierarchicalResult.Unrelated;

            string apath = a.FullPath.TrimEnd(_separator);
            string bpath = b.FullPath.TrimEnd(_separator);
            if (apath.Equals(bpath, _stringComparsion))
                return HierarchicalResult.Current;

            var aSplit = apath.Split(new char[] { _separator }, StringSplitOptions.RemoveEmptyEntries);
            var bSplit = bpath.Split(new char[] { _separator }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < Math.Min(aSplit.Length, bSplit.Length); i++)
            {
                if (!(aSplit[i].Equals(bSplit[i], _stringComparsion)))
                    return HierarchicalResult.Unrelated;
            }

            if (aSplit.Length > bSplit.Length)
                return HierarchicalResult.Parent;
            else return HierarchicalResult.Child;
        }

        #endregion

        #region Data

        private StringComparison _stringComparsion = StringComparison.InvariantCultureIgnoreCase;
        private char _separator = '\\';

        #endregion

        #region Public Properties

        #endregion

    }
}
