namespace BmLib.Interfaces
{
    /// <summary>
    /// Implements a viewmodel interface to determine whether a breadcrumb
    /// item (a crumb) is overflown (thus invisible since there is not enough
    /// space for display) or not.
    /// </summary>
    public interface IOverflown
    {
        bool IsOverflown { get; }
    }
}
