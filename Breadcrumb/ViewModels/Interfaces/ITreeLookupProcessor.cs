namespace Breadcrumb.ViewModels.Interfaces
{
	using Breadcrumb.Defines;

	/// <summary>
	/// Used with ITreeSelector and ITreeLookup, when ITreeSelector.LookupAsync return any HierarchicalResult, 
	/// it will be processed by these processors.    
	/// </summary>
	/// <typeparam name="VM"></typeparam>
	/// <typeparam name="T"></typeparam>
	public interface ITreeLookupProcessor<VM, T>
	{
		/// <summary>
		/// Called by ITreeSelector.LookupAsync() to process returned HierarchicalResult from ITreeLookup.
		/// </summary>
		/// <param name="hr">Returned hierarchicalresult</param>
		/// <param name="parentSelector">Parent viewmodel's ITreeSelector.</param>
		/// <param name="selector">Current viewmodel's ITreeSelector.</param>
		/// <returns>Stop process and return false if any processor return false.</returns>
		bool Process(HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector);
	}

	public static class ITreeSelectionProcessorExtension
	{
		public static bool Process<VM, T>(this ITreeLookupProcessor<VM, T>[] processors,
				HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector)
		{
			foreach (var p in processors)
				if (!p.Process(hr, parentSelector, selector))
					return false;
			return true;
		}
	}
}
