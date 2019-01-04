namespace BmLib.Interfaces
{
    /// <summary>
    /// Models an interfaces to an item that can
    /// tell whether it has a paren or not.
    /// </summary>
    public interface IParent
    {
        /// <summary>
        /// Gets the parent object where this object is the child in the treeview.
        /// </summary>
        IParent GetParent();
    }
}
