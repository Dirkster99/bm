namespace Breadcrumb.IconExtractors
{
    using Breadcrumb.IconExtractors.Enums;
    using DirectoryInfoExLib.Interfaces;
    using DirectoryInfoExLib.IO.FileSystemInfoExt;
    using DirectoryInfoExLib.Tools;
    using System;
	using System.Drawing;

	public class ExIconExtractor : IconExtractor<IDirectoryInfoEx>
	{
		protected static string fileBasedFSFilter = ".zip,.7z,.lha,.lzh,.sqx,.cab,.ace";

		#region Methods
		protected override Bitmap GetIconInner(IDirectoryInfoEx entry, string key, IconSize size)
		{
			if (key.StartsWith("."))
				throw new Exception("ext item is handled by IconExtractor");

////			if (entry is FileInfoEx)
////			{
////				Bitmap retVal = null;
////
////				string ext = PathEx.GetExtension(entry.Name);
////				if (IconExtractor.IsJpeg(ext))
////				{
////					retVal = IconExtractor.GetExifThumbnail(entry.FullName);
////				}
////				if (IconExtractor.IsImageIcon(ext))
////					try
////					{
////						retVal = new Bitmap(entry.FullName);
////					}
////					catch { retVal = null; }
////
////				if (retVal != null)
////					return retVal;
////			}

			return entry.RequestPIDL(pidl => this.GetBitmap(size, pidl.Ptr, entry is IDirectoryInfoEx, false));
		}

		protected override void GetIconKey(IDirectoryInfoEx entry, IconSize size, out string fastKey, out string slowKey)
		{
			string ext = PathEx.GetExtension(entry.Name);
			if (entry is IDirectoryInfoEx)
			{
				fastKey = entry.FullName;
				slowKey = entry.FullName;
			}
			else
			{
				if (this.IsGuidPath(entry.Name))
				{
					fastKey = entry.FullName;
					slowKey = entry.FullName;
				}
				else
				{
					if (IconExtractor.IsImageIcon(ext) || IconExtractor.IsSpecialIcon(ext))
					{
						fastKey = ext;
						slowKey = entry.FullName;
					}
					else
					{
						fastKey = slowKey = ext;
					}
				}
			}
		}

		private bool IsGuidPath(string fullName)
		{
			return fullName.StartsWith("::{");
		}
		#endregion
	}
}
