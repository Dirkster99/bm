namespace Breadcrumb.ViewModels
{
	using System.IO;
	using Breadcrumb.DirectoryInfoEx;
	using Breadcrumb.SystemIO;
	using Breadcrumb.ViewModels.Interfaces;

	public class AppViewModel : Breadcrumb.Viewmodels.Base.NotifyPropertyChanged
	{
		#region fields
		private DiskTreeNodeViewModel mDiskTest;
		////private ExTreeNodeViewModel mExTest;
		#endregion fields

		#region constructors
		public AppViewModel()
		{
			this.DiskTest = new DiskTreeNodeViewModel(new DirectoryInfo(@"C:\\"), new DirectoryInfo(@"D:\\"));
			(this.DiskTest.Selection as ITreeRootSelector<DiskTreeNodeViewModel, string>).SelectAsync(@"C:\Temp");

			this.ExTest = new ExTreeNodeViewModel();
			(this.ExTest.Selection as ITreeRootSelector<ExTreeNodeViewModel, FileSystemInfoEx>).SelectAsync(DirectoryInfoEx.FromString(@"C:\temp"));

			this.ExTest1 = new ExTreeNodeViewModel();
			(this.ExTest1.Selection as ITreeRootSelector<ExTreeNodeViewModel, FileSystemInfoEx>).SelectAsync(DirectoryInfoEx.FromString(@"C:\temp"));

			// If you want to show only root directories, try Toggle this line in TreeRootSelector.
			////updateRootItemsAsync(this, _rootItems, 2);
		}
		#endregion constructors

		#region properties
		public DiskTreeNodeViewModel DiskTest
		{
			get
			{
				return this.mDiskTest;
			}
			
			private set
			{
				this.mDiskTest = value;
			}
		}

		public ExTreeNodeViewModel ExTest { get; private set; }

		public ExTreeNodeViewModel ExTest1 { get; private set; }

		#endregion properties
	}
}
