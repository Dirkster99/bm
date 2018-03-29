namespace Breadcrumb.ViewModels.TreeLookupProcessors
{
    using Breadcrumb.ViewModels.Interfaces;
    using BreadcrumbLib.Defines;
    using System;

    /// <summary>
    /// Implementation of ITreeLookupProcessor, which used with ITreeSelector and ITreeLookup, 
    /// when ITreeSelector.LookupAsync return any HierarchicalResult, it will be processed by these processors.    
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class TreeLookupProcessor<VM, T> : ITreeLookupProcessor<VM, T>
	{
		#region fields
		private Func<HierarchicalResult, ITreeSelector<VM, T>, ITreeSelector<VM, T>, bool> _processFunc;
		private HierarchicalResult _appliedResult;
		#endregion fields

		#region constructors
		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="appliedResult"></param>
		/// <param name="processFunc"></param>
		public TreeLookupProcessor(HierarchicalResult appliedResult,
				Func<HierarchicalResult, ITreeSelector<VM, T>, ITreeSelector<VM, T>, bool> processFunc)
		{
			this._processFunc = processFunc;
			this._appliedResult = appliedResult;
		}
		#endregion constructors

		#region methodss
		public bool Process(HierarchicalResult hr, ITreeSelector<VM, T> parentSelector, ITreeSelector<VM, T> selector)
		{
			if (this._appliedResult.HasFlag(hr))
				return this._processFunc(hr, parentSelector, selector);

			return true;
		}
		#endregion methodss
	}
}
