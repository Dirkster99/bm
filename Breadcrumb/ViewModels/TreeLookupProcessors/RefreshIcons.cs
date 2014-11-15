namespace Breadcrumb.ViewModels.TreeLookupProcessors
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Breadcrumb.Defines;
	using Breadcrumb.ViewModels.Interfaces;

	/// <summary>
	/// Refresh Icons if loaded.
	/// </summary>
	/// <typeparam name="VM"></typeparam>
	/// <typeparam name="T"></typeparam>
	public class RefreshIcons<VM, T> : ITreeLookupProcessor<VM, T>
	{
		/// <summary>
		/// Set ViewModel's EntryHelper.IsExpanded to false if it's child of current lookup.
		/// </summary>
		public static RefreshIcons<VM, T> IfLoaded = new RefreshIcons<VM, T>();

		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="matchResult"></param>
        public RefreshIcons()
		{			
		}

		public bool Process(HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector)
		{            
            if (selector.ViewModel is ISupportIconHelper)
                (selector.ViewModel as ISupportIconHelper).Icons.RefreshAsync();

			return true;
		}
	}
}
