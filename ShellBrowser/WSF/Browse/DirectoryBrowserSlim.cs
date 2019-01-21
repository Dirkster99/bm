namespace WSF.Browse
{

    /// <summary>
    /// Implements a simple PoCo type class that can be quickly enumerated
    /// and be used to view a large list of objects.
    /// 
    /// The large list of objects case can be handled by 
    /// </summary>
    public class DirectoryBrowserSlim
    {
        #region ctors
        /// <summary>
        /// Parameterized class constructor
        /// </summary>
        public DirectoryBrowserSlim(int parId,
                                         string parItemPath,

                                         string parName,
                                         string parParseName,
                                         string parLabelName)
            : this()
        {
            ID = parId;
            ItemPath = parItemPath;
            Name = parName;
            ParseName = parParseName;
            LabelName = parLabelName;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected DirectoryBrowserSlim()
        {
        }
        #endregion ctors

        #region properties
        /// <summary>
        /// Gets an id for this item
        /// (the generator of the item should ensure uniqueness
        /// for the overall collection).
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets the path of this item.
        /// </summary>
        public object ItemPath { get; }

        /// <summary>
        /// Gets the Name of this item.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the parse name of this item.
        /// </summary>
        public string ParseName { get; }

        /// <summary>
        /// Gets the label string (for usage in UI) of this item.
        /// </summary>
        public string LabelName { get; }
        #endregion properties

        #region methods

        #endregion methods
    }
}
