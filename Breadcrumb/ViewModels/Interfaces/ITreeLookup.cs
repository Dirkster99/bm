namespace Breadcrumb.ViewModels.Interfaces
{
	using System.Threading.Tasks;

	/// <summary>
	/// Specify a strategy to lookup an item from a multi-level tree based ViewModels, which contains ITreeSelector.
	/// </summary>
	/// <typeparam name="VM"></typeparam>
	/// <typeparam name="T"></typeparam>
	public interface ITreeLookup<VM, T>
	{
		/// <summary>
		/// Lookup an item from a multi-level tree based ViewModels, and run processors on the way.
		/// </summary>
		/// <param name="value">Value of the lookup node.</param>
		/// <param name="parentSelector">Where to start lookup.</param>
		/// <param name="comparer">Compare two value and return it's hierarchy.</param>
		/// <param name="processors">Processors's Process() method is run whether it's parent, child, current or unrelated node of lookup node.</param>
		/// <returns></returns>
		Task LookupAsync(T value, ITreeSelector<VM, T> parentSelector,
				ICompareHierarchy<T> comparer, params ITreeLookupProcessor<VM, T>[] processors);
	}
}
