///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using ShellDll;
using System.Runtime.InteropServices;
using System.Reflection;

namespace System.IO.Tools
{
    public class CollumnInfo
    {
        public PropertyKey PropertyKey { get; set; }
        public uint ColumnIndex { get; set; }
        public SHCOLSTATEF Flags { get; set; }

        public Type CollumnType
        {
            get
            {
                if ((Flags & SHCOLSTATEF.SHCOLSTATE_TYPE_DATE) == SHCOLSTATEF.SHCOLSTATE_TYPE_DATE) return typeof(ShellAPI.FILETIME);
                else if ((Flags & SHCOLSTATEF.SHCOLSTATE_TYPE_INT) == SHCOLSTATEF.SHCOLSTATE_TYPE_INT) return typeof(int);
                else if ((Flags & SHCOLSTATEF.SHCOLSTATE_TYPE_STR) == SHCOLSTATEF.SHCOLSTATE_TYPE_STR) return typeof(string);
                else return typeof(object);
            }
        }

        /// <summary>
        /// Not displayed in the context menu, but is listed in the More... dialog.
        /// </summary>
        public bool SecondaryUI { get { return (Flags & SHCOLSTATEF.SHCOLSTATE_SECONDARYUI) == SHCOLSTATEF.SHCOLSTATE_SECONDARYUI; } }
        /// <summary>
        /// Handled by separate threads.
        /// </summary>
        public bool SlowProperty { get { return (Flags & SHCOLSTATEF.SHCOLSTATE_SLOW) != SHCOLSTATEF.SHCOLSTATE_SLOW; } }
        /// <summary>
        /// Handled by external handlers.
        /// </summary>
        public bool ExtendedProperty { get { return (Flags & SHCOLSTATEF.SHCOLSTATE_EXTENDED) == SHCOLSTATEF.SHCOLSTATE_EXTENDED; } }
        private string _colName = null;
        public string CollumnName { get { if (_colName == null) _colName = getColName(); return _colName; } }

        private string getColName()
        {
            if (PropertyKey.fmtid.Equals(SummaryInformation.FormatID))
                    foreach (string name in SummaryInformation.PropertyDic.Keys)
                        if (PropertyKey.pid == SummaryInformation.PropertyDic[name].pid)
                            return name;
            if (PropertyKey.fmtid.Equals(DocSummaryInformation.FormatID))
                    foreach (string name in DocSummaryInformation.PropertyDic.Keys)
                        if (PropertyKey.pid == DocSummaryInformation.PropertyDic[name].pid)
                            return name;
            if (PropertyKey.fmtid.Equals(ImageSummaryInformation.FormatID))
                    foreach (string name in ImageSummaryInformation.PropertyDic.Keys)
                        if (PropertyKey.pid == ImageSummaryInformation.PropertyDic[name].pid)
                            return name;
            if (PropertyKey.fmtid.Equals(MusicSummaryInformation.FormatID))
                    foreach (string name in MusicSummaryInformation.PropertyDic.Keys)
                        if (PropertyKey.pid == MusicSummaryInformation.PropertyDic[name].pid)
                            return name;
            if (PropertyKey.fmtid.Equals(VideoSummaryInformation.FormatID))
                    foreach (string name in VideoSummaryInformation.PropertyDic.Keys)
                        if (PropertyKey.pid == VideoSummaryInformation.PropertyDic[name].pid)
                            return name;
            if (PropertyKey.fmtid.Equals(AudioSummaryInformation.FormatID))
                    foreach (string name in AudioSummaryInformation.PropertyDic.Keys)
                        if (PropertyKey.pid == AudioSummaryInformation.PropertyDic[name].pid)
                            return name;

            return "";
        }
    }

    #region SummaryInformationList

    public static class SummaryInformation
    {
        public static Guid FormatID = new Guid("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}");        

        public static PropertyKey Title = new PropertyKey() { fmtid = FormatID, pid = 2 };
        public static PropertyKey Subject = new PropertyKey() { fmtid = FormatID, pid = 3 };
        public static PropertyKey Author = new PropertyKey() { fmtid = FormatID, pid = 4 };
        public static PropertyKey Keywords = new PropertyKey() { fmtid = FormatID, pid = 5 };
        public static PropertyKey Comments = new PropertyKey() { fmtid = FormatID, pid = 6 };
        public static PropertyKey Template = new PropertyKey() { fmtid = FormatID, pid = 7 };
        public static PropertyKey LastAuthor = new PropertyKey() { fmtid = FormatID, pid = 8 };
        public static PropertyKey RevNumber = new PropertyKey() { fmtid = FormatID, pid = 9 };
        public static PropertyKey EditTime = new PropertyKey() { fmtid = FormatID, pid = 10 };
        public static PropertyKey LastPrinted = new PropertyKey() { fmtid = FormatID, pid = 11 };
        public static PropertyKey CreateDtm = new PropertyKey() { fmtid = FormatID, pid = 12 };
        public static PropertyKey PageCount = new PropertyKey() { fmtid = FormatID, pid = 13 };
        public static PropertyKey WordCount = new PropertyKey() { fmtid = FormatID, pid = 14 };
        public static PropertyKey CharCount = new PropertyKey() { fmtid = FormatID, pid = 15 };
        public static PropertyKey Thumbnail = new PropertyKey() { fmtid = FormatID, pid = 16 };
        public static PropertyKey AppName = new PropertyKey() { fmtid = FormatID, pid = 17 };
        public static PropertyKey Security = new PropertyKey() { fmtid = FormatID, pid = 18 };

        public static PropertyKey[] PropertyList = new PropertyKey[] 
        { Title, Subject, Author, Keywords, Comments, Template, LastAuthor, RevNumber, EditTime, LastPrinted, CreateDtm, 
            PageCount, WordCount, CharCount, Thumbnail, AppName, Security };
        public static Dictionary<string, PropertyKey> PropertyDic = initPropertyDic();
        private static Dictionary<string, PropertyKey> initPropertyDic()
        {
            Dictionary<string, PropertyKey> retVal = new Dictionary<string, PropertyKey>();
            foreach (var fi in (typeof(SummaryInformation)).GetFields())
                if (fi.FieldType == typeof(PropertyKey))
                    retVal.Add(fi.Name, (PropertyKey)fi.GetValue(null));
            return retVal;
        }
    }

    public static class DocSummaryInformation
    {
        public static Guid FormatID = new Guid("{D5CDD502-2E9C-101B-9397-08002B2CF9AE}");

        public static PropertyKey Category = new PropertyKey() { fmtid = FormatID, pid = 2 };
        public static PropertyKey PresFormat = new PropertyKey() { fmtid = FormatID, pid = 3 };
        public static PropertyKey ByteCount = new PropertyKey() { fmtid = FormatID, pid = 4 };
        public static PropertyKey LineCount = new PropertyKey() { fmtid = FormatID, pid = 5 };
        public static PropertyKey ParCount = new PropertyKey() { fmtid = FormatID, pid = 6 };
        public static PropertyKey SlideCount = new PropertyKey() { fmtid = FormatID, pid = 7 };
        public static PropertyKey NoteCount = new PropertyKey() { fmtid = FormatID, pid = 8 };
        public static PropertyKey HiddenCount = new PropertyKey() { fmtid = FormatID, pid = 9 };
        public static PropertyKey MMClipCount = new PropertyKey() { fmtid = FormatID, pid = 10 };
        public static PropertyKey Scale = new PropertyKey() { fmtid = FormatID, pid = 11 };
        public static PropertyKey HeadingPair = new PropertyKey() { fmtid = FormatID, pid = 12 };
        public static PropertyKey DocParts = new PropertyKey() { fmtid = FormatID, pid = 13 };
        public static PropertyKey Manager = new PropertyKey() { fmtid = FormatID, pid = 14 };
        public static PropertyKey Company = new PropertyKey() { fmtid = FormatID, pid = 15 };
        public static PropertyKey LinesDirty = new PropertyKey() { fmtid = FormatID, pid = 16 };
        public static PropertyKey CharCount2 = new PropertyKey() { fmtid = FormatID, pid = 17 };

        public static PropertyKey[] PropertyList = new PropertyKey[] {
            Category, PresFormat, ByteCount, LineCount, ParCount, SlideCount, NoteCount, HiddenCount, 
            MMClipCount, Scale, HeadingPair, DocParts, Manager, Company, LinesDirty, CharCount2 
        };
        public static Dictionary<string, PropertyKey> PropertyDic = initPropertyDic();
        private static Dictionary<string, PropertyKey> initPropertyDic()
        {
            Dictionary<string, PropertyKey> retVal = new Dictionary<string, PropertyKey>();
            foreach (var fi in (typeof(DocSummaryInformation)).GetFields())
                if (fi.FieldType == typeof(PropertyKey))
                    retVal.Add(fi.Name, (PropertyKey)fi.GetValue(null));
            return retVal;
        }
    }

    public static class ImageSummaryInformation
    {
        public static Guid FormatID = new Guid("{6444048F-4C8B-11D1-8B70-080036B11A03}");

        public static PropertyKey FileType = new PropertyKey() { fmtid = FormatID, pid = 2 };
        public static PropertyKey CX = new PropertyKey() { fmtid = FormatID, pid = 3 };
        public static PropertyKey CY = new PropertyKey() { fmtid = FormatID, pid = 4 };
        public static PropertyKey ResolutionX = new PropertyKey() { fmtid = FormatID, pid = 5 };
        public static PropertyKey ReslutionY = new PropertyKey() { fmtid = FormatID, pid = 6 };
        public static PropertyKey BitDepth = new PropertyKey() { fmtid = FormatID, pid = 7 };
        public static PropertyKey ColorSpace = new PropertyKey() { fmtid = FormatID, pid = 8 };
        public static PropertyKey Compression = new PropertyKey() { fmtid = FormatID, pid = 9 };
        public static PropertyKey Transparency = new PropertyKey() { fmtid = FormatID, pid = 10 };
        public static PropertyKey GammaValue = new PropertyKey() { fmtid = FormatID, pid = 11 };
        public static PropertyKey FrameCount = new PropertyKey() { fmtid = FormatID, pid = 12 };
        public static PropertyKey Dimensions = new PropertyKey() { fmtid = FormatID, pid = 13 };

        public static PropertyKey[] PropertyList = new PropertyKey[] { 
            FileType, CX, CY, ResolutionX, ReslutionY, BitDepth, ColorSpace, Compression, 
            Transparency, GammaValue, FrameCount, Dimensions };
        public static Dictionary<string, PropertyKey> PropertyDic = initPropertyDic();
        private static Dictionary<string, PropertyKey> initPropertyDic()
        {
            Dictionary<string, PropertyKey> retVal = new Dictionary<string, PropertyKey>();
            foreach (var fi in (typeof(ImageSummaryInformation)).GetFields())
                if (fi.FieldType == typeof(PropertyKey))
                    retVal.Add(fi.Name, (PropertyKey)fi.GetValue(null));
            return retVal;
        }
    }

    public static class MusicSummaryInformation
    {
        public static Guid FormatID = new Guid("{56A3372E-CE9C-11D2-9F0E-006097C686F6}");

        public static PropertyKey Artist = new PropertyKey() { fmtid = FormatID, pid = 2 };
        public static PropertyKey SongTitle = new PropertyKey() { fmtid = FormatID, pid = 3 };
        public static PropertyKey Album = new PropertyKey() { fmtid = FormatID, pid = 4 };
        public static PropertyKey Year = new PropertyKey() { fmtid = FormatID, pid = 5 };
        public static PropertyKey Comment = new PropertyKey() { fmtid = FormatID, pid = 6 };
        public static PropertyKey Track = new PropertyKey() { fmtid = FormatID, pid = 7 };

        public static PropertyKey Genre = new PropertyKey() { fmtid = FormatID, pid = 11 };
        public static PropertyKey Lyrics = new PropertyKey() { fmtid = FormatID, pid = 12 };

        public static PropertyKey[] PropertyList = new PropertyKey[] { 
            Artist, SongTitle, Album, Year, Comment, Track, Genre, Lyrics };
        public static Dictionary<string, PropertyKey> PropertyDic = initPropertyDic();
        private static Dictionary<string, PropertyKey> initPropertyDic()
        {
            Dictionary<string, PropertyKey> retVal = new Dictionary<string, PropertyKey>();
            foreach (var fi in (typeof(MusicSummaryInformation)).GetFields())
                if (fi.FieldType == typeof(PropertyKey))
                    retVal.Add(fi.Name, (PropertyKey)fi.GetValue(null));
            return retVal;
        }
    }

    public static class VideoSummaryInformation
    {
        public static Guid FormatID = new Guid("{64440491-4C8B-11D1-8B70-080036B11A03}");

        public static PropertyKey StreamName = new PropertyKey() { fmtid = FormatID, pid = 2 };
        public static PropertyKey FrameWidth = new PropertyKey() { fmtid = FormatID, pid = 3 };
        public static PropertyKey FrameHeight = new PropertyKey() { fmtid = FormatID, pid = 4 };
        public static PropertyKey TimeLength = new PropertyKey() { fmtid = FormatID, pid = 5 };
        public static PropertyKey FrameCount = new PropertyKey() { fmtid = FormatID, pid = 6 };
        public static PropertyKey FrameRate = new PropertyKey() { fmtid = FormatID, pid = 7 };
        public static PropertyKey DataRate = new PropertyKey() { fmtid = FormatID, pid = 8 };
        public static PropertyKey SampleSize = new PropertyKey() { fmtid = FormatID, pid = 9 };
        public static PropertyKey Compression = new PropertyKey() { fmtid = FormatID, pid = 10 };
        public static PropertyKey StreamNumber = new PropertyKey() { fmtid = FormatID, pid = 11 };

        public static PropertyKey[] PropertyList = new PropertyKey[] { 
            StreamName, FrameWidth, FrameHeight, TimeLength, FrameCount, FrameRate, SampleSize, 
            Compression, StreamNumber };
        public static Dictionary<string, PropertyKey> PropertyDic = initPropertyDic();
        private static Dictionary<string, PropertyKey> initPropertyDic()
        {
            Dictionary<string, PropertyKey> retVal = new Dictionary<string, PropertyKey>();
            foreach (var fi in (typeof(VideoSummaryInformation)).GetFields())
                if (fi.FieldType == typeof(PropertyKey))
                    retVal.Add(fi.Name, (PropertyKey)fi.GetValue(null));
            return retVal;
        }
    }

    public static class AudioSummaryInformation
    {
        public static Guid FormatID = new Guid("{64440490-4C8B-11D1-8B70-080036B11A03}");        

        public static PropertyKey Format = new PropertyKey() { fmtid = FormatID, pid = 2 };
        public static PropertyKey TimeLength = new PropertyKey() { fmtid = FormatID, pid = 3 };
        public static PropertyKey AvgDataRate = new PropertyKey() { fmtid = FormatID, pid = 4 };
        public static PropertyKey SampleRate = new PropertyKey() { fmtid = FormatID, pid = 5 };
        public static PropertyKey SampleSize = new PropertyKey() { fmtid = FormatID, pid = 6 };
        public static PropertyKey ChannelCount = new PropertyKey() { fmtid = FormatID, pid = 7 };
        public static PropertyKey StreamNumber = new PropertyKey() { fmtid = FormatID, pid = 8 };
        public static PropertyKey StreamName = new PropertyKey() { fmtid = FormatID, pid = 9 };
        public static PropertyKey Compression = new PropertyKey() { fmtid = FormatID, pid = 10 };

        public static PropertyKey[] PropertyList = new PropertyKey[] { 
            Format, TimeLength, AvgDataRate, SampleRate, SampleSize, ChannelCount, StreamNumber, 
            StreamName, Compression };
        public static Dictionary<string, PropertyKey> PropertyDic = initPropertyDic();
        private static Dictionary<string, PropertyKey> initPropertyDic()
        {
            Dictionary<string, PropertyKey> retVal = new Dictionary<string, PropertyKey>();
            foreach (var fi in (typeof(AudioSummaryInformation)).GetFields())
                if (fi.FieldType == typeof(PropertyKey))
                    retVal.Add(fi.Name, (PropertyKey)fi.GetValue(null));
            return retVal;
        }
    }

    public static class LinkSummaryInformation
    {
        public static Guid FormatID = new Guid("{B9B4B3FC-2B51-4A42-B5D8-324146AFCF25}");

        public static PropertyKey TargetParsingPath = new PropertyKey() { fmtid = FormatID, pid = 2 };
        public static PropertyKey Status = new PropertyKey() { fmtid = FormatID, pid = 3 };
        public static PropertyKey Comment = new PropertyKey() { fmtid = FormatID, pid = 5 };
        public static PropertyKey TargetSFGAOFlags = new PropertyKey() { fmtid = FormatID, pid = 8 };
        
        public static PropertyKey[] PropertyList = new PropertyKey[] { 
            TargetParsingPath, Status, Comment, TargetSFGAOFlags };
        public static Dictionary<string, PropertyKey> PropertyDic = initPropertyDic();
        private static Dictionary<string, PropertyKey> initPropertyDic()
        {
            Dictionary<string, PropertyKey> retVal = new Dictionary<string, PropertyKey>();
            foreach (var fi in (typeof(AudioSummaryInformation)).GetFields())
                if (fi.FieldType == typeof(PropertyKey))
                    retVal.Add(fi.Name, (PropertyKey)fi.GetValue(null));
            return retVal;
        }
    }
    #endregion


    public static class ExtraPropertiesProvider
    {
        #region Header Information

       
        //based on values on http://code.google.com/p/ruby-ole/source/browse/trunk/data/FileInfo.pas?r=38

        //http://msdn.microsoft.com/en-us/library/Aa380376
        public static Guid FMTID_SummaryInformation = new Guid("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}");
        //http://msdn.microsoft.com/en-us/library/aa380374(v=VS.85).aspx
        public static Guid FMTID_DocSummaryInformation = new Guid("{D5CDD502-2E9C-101B-9397-08002B2CF9AE}");
        public static Guid FMTID_UserDefinedProperties = new Guid("{D5CDD505-2E9C-101B-9397-08002B2CF9AE}");
        public static Guid FMTID_ImageSummaryInformation = new Guid("{6444048F-4C8B-11D1-8B70-080036B11A03}");
        public static Guid FMTID_InternetSite = new Guid("{000214A1-0000-0000-C000-000000000046}");
        public static Guid FMTID_Music = new Guid("{56A3372E-CE9C-11D2-9F0E-006097C686F6}");
        public static Guid FMTID_Audio = new Guid("{64440490-4C8B-11D1-8B70-080036B11A03}");
        public static Guid FMTID_Video = new Guid("{64440491-4C8B-11D1-8B70-080036B11A03}");
        public static Guid FMTID_MediaFile = new Guid("{64440492-4C8B-11D1-8B70-080036B11A03}");
        
        
        #region Unused
        //public static Guid[] FMTIDList = new Guid[] {
        //    FMTID_SummaryInformation, FMTID_DocSummaryInformation, FMTID_UserDefinedProperties, 
        //    FMTID_ImageSummaryInformation, /*FMTID_InternetSite, */ FMTID_Music, FMTID_Audio, FMTID_Video 
        //};

        
        //public enum PID_SummaryInformaion : uint
        //{
        //    Title = 2, Subject, Author, Keywords, Comments, Template, LastAuthor, RevNumber, EditTime,
        //    LastPrinted, CreateDtm, PageCount, WordCount, CharCount, Thumbnail, AppName, Security
        //};

        //public enum PID_DocSummaryInformation : uint
        //{
        //    Category = 2, PresFormat, ByteCount, LineCount, ParCount, SlideCount, NoteCount, HiddenCount, MMClipCount,
        //    Scale, HeadingPair, DocParts, Manager, Company, LinesDirty, CharCount2

        //}

        //public enum PID_ImageSummaryInformation : uint
        //{
        //    FileType = 2, CX, CY, ResolutionX, ReslutionY, BitDepth, ColorSpace, Compression, Transparency,
        //    GammaValue, FrameCount, Dimensions
        //}

        //public enum PID_Music : uint
        //{
        //    Artist = 2, SongTitle, Album, Year, Comment, Track,
        //    Genre = 11, Lyrics
        //}

        //public enum PID_Video : uint
        //{
        //    StreamName = 2, FrameWidth, FrameHeight, TimeLength, FrameCount, FrameRate, DataRate, SampleSize, Compression,
        //    StreamNumber
        //}

        //public enum PID_Audio : uint
        //{
        //    Format = 2, TimeLength, AvgDataRate, SampleRate, SampleSize, ChannelCount, StreamNumber, StreamName, Compression
        //}
        #endregion
        #endregion


        public enum InformationCategory { Summary, DocSummary, ImageSummary, Music, Video, Audio, Link, UserDefined }

        static PropertyKey GetPropertyKey(Guid FMTID, uint PID)
        {
            return new PropertyKey() { fmtid = FMTID, pid = PID };
        }

        static PropertyKey GetPropertyKey(InformationCategory category, uint pid)
        {
            switch (category)
            {
                case InformationCategory.Summary: return GetPropertyKey(FMTID_SummaryInformation, pid);
                case InformationCategory.DocSummary: return GetPropertyKey(FMTID_DocSummaryInformation, pid);
                case InformationCategory.ImageSummary: return GetPropertyKey(FMTID_ImageSummaryInformation, pid);
                case InformationCategory.Music: return GetPropertyKey(FMTID_Music, pid);
                case InformationCategory.Video: return GetPropertyKey(FMTID_Video, pid);
                case InformationCategory.Audio: return GetPropertyKey(FMTID_Audio, pid);
            }
            return GetPropertyKey(FMTID_SummaryInformation, pid);
        }

        private static CollumnInfo[] GetCollumnInfo(ShellFolder2 sf2)
        {
            List<CollumnInfo> retVal = new List<CollumnInfo>();
            uint i = 1;

            while (i < 500)
            {
                ShellDll.SHCOLSTATEF state;
                int hr = sf2.GetDefaultColumnState(i, out state);
                if (hr == -2147316575) //COMException, Out of Bounds
                    break;

                PropertyKey propKey;
                hr = sf2.MapColumnToSCID(i, out propKey);
                if (hr == -2147316575) //COMException, Out of Bounds
                    break;

                retVal.Add(new CollumnInfo() { ColumnIndex = i, Flags = state, PropertyKey = propKey });

                i++;
            }
            return retVal.ToArray();
        }

        public static CollumnInfo[] GetCollumnInfo(DirectoryInfoEx dir)
        {
            using (ShellFolder2 sf2 = dir.ShellFolder)
                return GetCollumnInfo(sf2);
        }

        public static object GetProperty(ShellFolder2 sf2, FileInfoEx file, ref PropertyKey propKey)
        {
            PropertyKey pKey = propKey;
            var output = file.RequestRelativePIDL(relPidl =>
                {
                    object retVal = null;
                    if (relPidl != null)
                    {                        
                        int hr = sf2.GetDetailsEx(relPidl.Ptr, ref pKey, out retVal);
                        if (hr != ShellAPI.S_OK)
                            Marshal.ThrowExceptionForHR(hr);
                    }
                    return retVal;
                });
            propKey = pKey;
            return output;

        }

        public static object GetProperty(FileInfoEx file, PropertyKey propKey)
        {
            if (file.Exists)
                using (ShellFolder2 sf2 = file.Parent.ShellFolder)
                {
                    ShellDll.PropertyKey pkey = propKey;
                    return GetProperty(sf2, file, ref pkey);
                }
            return null;
        }

        public static object GetProperty(string filename, PropertyKey propKey)
        {
            if (File.Exists(filename))
                return GetProperty(new FileInfoEx(filename), propKey);
            return null;
        }


    }
}
