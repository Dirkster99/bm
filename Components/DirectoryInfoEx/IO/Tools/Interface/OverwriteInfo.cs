namespace DirectoryInfoExLib.IO.Tools.Interface
{
    using System;
    using System.Drawing;
    using DirectoryInfoExLib.Tools;

    public class OverwriteInfo
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public long Length { get; set; }
        public DateTime Time { get; set; }

        private string _crc = null;
        public string CRC { get { return _crc != null ? _crc :
            FileEx.Exists(Path) ? Helper.GetFileCRC(Path) : ""; }
            set { _crc = value; }
        }

        public Bitmap Icon { get { return Helper.GetFileIcon(Path); } }       
    }
   
}
