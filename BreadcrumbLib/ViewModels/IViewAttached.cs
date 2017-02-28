namespace BreadcrumbLib.ViewModels
{
    using System;

    /// <summary>
    /// Simulates the Caliburn Micro interface with same name.
    /// </summary>
    public interface IViewAttached
    {
        bool IsViewAttached { get; }
        void OnViewAttached(object view, object context);
    }
}
