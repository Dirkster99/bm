namespace Breadcrumb.ViewModels.Interfaces
{
    using BreadcrumbLib.Models;
    using System.Threading.Tasks;

    public interface IBreadcrumbViewModel //// : ISupportTreeSelector<IBreadcrumbItemViewModel, IEntryModel>, ISupportCommandManager
    {
        IEntryModel[] RootModels { set; }
		////IProfile[] Profiles { set; }

        bool EnableBreadcrumb { get; set; }
        bool EnableBookmark { get; set; }

        Task SelectAsync(IEntryModel value);
    }
}
