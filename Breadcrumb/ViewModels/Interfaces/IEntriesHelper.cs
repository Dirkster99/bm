namespace Breadcrumb.ViewModels.Interfaces
{
    using Breadcrumb.Utils;
    using BreadcrumbLib.Defines;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public interface ISupportEntriesHelper<VM>
	{
		IEntriesHelper<VM> Entries { get; }
	}

	/// <summary>
	/// Helper view model class that provide support of loading sub-entries.
	/// </summary>
	/// <typeparam name="VM"></typeparam>
	public interface IEntriesHelper<VM> : INotifyPropertyChanged
	{
		#region events
		event EventHandler EntriesChanged;
		#endregion events

		#region properties
		/// <summary>
		/// Load when expand the first time.
		/// </summary>
		bool IsExpanded { get; set; }

		/// <summary>
		/// Whether subentries loaded.
		/// </summary>
		bool IsLoaded { get; }

		/// <summary>
		/// Whether subentries is loading.
		/// </summary>
		bool IsLoading { get; }

		/// <summary>
		/// A list of sub-entries, after loaded, used by VM thread only.
		/// </summary>
		IEnumerable<VM> AllNonBindable { get; }

		/// <summary>
		/// A list of sub-entries, after loaded, used by UI thread only (e.g. Binding).
		/// </summary>
		ObservableCollection<VM> All { get; }

		/// <summary>
		/// Internal lock object when loading.
		/// </summary>
		AsyncLock LoadingLock { get; }
		#endregion properties

		#region methods
		/// <summary>
		/// Call to load sub-entries.
		/// </summary>
		/// <param name="force">Load sub-entries even if it's already loaded.</param>
		/// <returns></returns>
		Task<IEnumerable<VM>> LoadAsync(UpdateMode updateMode = UpdateMode.Replace, bool force = false, object parameter = null);

		Task UnloadAsync();

		/// <summary>
		/// Used to preload sub-entries, fully overwrite entries stored in the helper.
		/// </summary>
		/// <param name="viewModels"></param>
		void SetEntries(UpdateMode updateMode = UpdateMode.Replace, params VM[] viewModels);
		#endregion methods
	}
}
