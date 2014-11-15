using System.Threading.Tasks;
using System.Linq;

namespace Breadcrumb.ViewModels.Interfaces
{
	/// <summary>
	/// Implement by ViewModel that has Tree based structure and support LookupProcessing.
	/// </summary>
	/// <typeparam name="VM">Sub-node viewmodel type.</typeparam>
	/// <typeparam name="T">Type to identify a node, commonly string.</typeparam>
	public interface ISupportTreeSelector<VM, T> : ISupportEntriesHelper<VM>
	{
		ITreeSelector<VM, T> Selection { get; set; }
	}

    public static partial class ExtensionMethods
    {
        public static async Task RefreshIconsAsync<VM>(this ISupportEntriesHelper<VM> vm)
        {
            if (vm is ISupportIconHelper)
            {
                await (vm as ISupportIconHelper).Icons.RefreshAsync();
                if (vm.Entries.IsLoaded)
                    await Task.WhenAll(
                    vm.Entries.AllNonBindable
                        .Where(subVm => subVm is ISupportEntriesHelper<VM>)
                        .Select(subVm => (subVm as ISupportEntriesHelper<VM>).RefreshIconsAsync()));                
            }
        }
    }
}
