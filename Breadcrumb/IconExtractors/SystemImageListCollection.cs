using Breadcrumb.IconExtractors.Enums;

namespace Breadcrumb.IconExtractors
{
	/// <summary>
	/// Created By LYCJ (2014), released under MIT license
	/// I did some tidy up Based on http://vbaccelerator.com/home/net/code/libraries/Shell_Projects/SysImageList/article.asp
	/// </summary>
	public class SystemImageListCollection
	{
		#region fields
		////private SystemImageList smallImageList = new SystemImageList(IconSize.small);
		////private SystemImageList largeImageList = new SystemImageList(IconSize.large);
		////private SystemImageList exLargeImageList = new SystemImageList(IconSize.extraLarge);
		////private SystemImageList jumboImageList = new SystemImageList(IconSize.jumbo);
		////private Dictionary<IconSize, SystemImageList> imageListDic;

		private SystemImageList _currentImageList = null;
		private IconSize _currentImageListSize = IconSize.large;
		#endregion fields

		#region Constructor
		public SystemImageListCollection()
		{
			////imageListDic = new Dictionary<IconSize, SystemImageList>()
			////{
			////    { IconSize.small, smallImageList },
			////    { IconSize.large, largeImageList },
			////    { IconSize.extraLarge, exLargeImageList },
			////    { IconSize.jumbo, jumboImageList },
			////    { IconSize.thumbnail, jumboImageList }
			////};
		}
		#endregion

		#region properties
		public IconSize CurrentImageListSize
		{
			get
			{
				return this._currentImageListSize;
			}
		}

		public bool IsImageListInited
		{
			get { return this._currentImageList != null; }
		}

		public SystemImageList this[IconSize size]
		{
			get { return this.getImageList(size); }
		}
		#endregion properties

		#region methods
		public SystemImageList getImageList(IconSize size)
		{
			if (size == IconSize.thumbnail)
				size = IconSize.jumbo;

			if (this._currentImageList != null && this._currentImageListSize == size)
				return this._currentImageList;

			if (this._currentImageList != null)
			{
				this._currentImageList.Dispose();
				this._currentImageList = null;
			}

			this._currentImageListSize = size;
			return this._currentImageList = new SystemImageList(this._currentImageListSize);
		}

		public void Dispose()
		{
			////if (smallImageList != null)
			////{
			////    smallImageList.Dispose();
			////    smallImageList = null;
			////}
			////if (largeImageList != null)
			////{
			////    largeImageList.Dispose();
			////    largeImageList = null;
			////}
			////if (exLargeImageList != null)
			////{
			////    exLargeImageList.Dispose();
			////    exLargeImageList = null;
			////}
			////if (jumboImageList != null)
			////{
			////    jumboImageList.Dispose();
			////    jumboImageList = null;
			////}

			if (this._currentImageList != null)
			{
				this._currentImageList.Dispose();
				this._currentImageList = null;
			}
		}
		#endregion methods
	}
}
