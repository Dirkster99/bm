namespace Breadcrumb.Demo
{
    using System;
    using System.Collections.ObjectModel;

	public class SpecialFoldersViewModel
	{
		public SpecialFoldersViewModel()
		{
			this.Folders = new ObservableCollection<SpecialFolderViewModel>();

			System.Collections.Generic.SortedDictionary<string, SpecialFolderViewModel> sorter = new System.Collections.Generic.SortedDictionary<string,SpecialFolderViewModel>();

			Array values = Enum.GetValues(typeof(System.Environment.SpecialFolder));

			foreach (System.Environment.SpecialFolder item in values)
			{
				var specialFolder = new SpecialFolderViewModel(item);

				SpecialFolderViewModel obj;
				if (sorter.TryGetValue(specialFolder.FolderName, out obj) == false)
					sorter.Add(specialFolder.FolderName, specialFolder);
			
			}

			// Create final sorted collection
			foreach (var item in sorter.Values)
				this.Folders.Add(item);
		}

		public ObservableCollection<SpecialFolderViewModel> Folders { get; set; }
	}
}
