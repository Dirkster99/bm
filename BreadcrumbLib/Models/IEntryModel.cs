namespace BreadcrumbLib.Models
{
    using BreadcrumbLib.Profile;
    using System;
    using System.ComponentModel;

    public interface IEntryModel : INotifyPropertyChanged, IEquatable<IEntryModel>
    {
        IProfile Profile { get; }

        bool IsDirectory { get; }
        IEntryModel Parent { get; }
        string Label { get; }
        string Name { get; set; }
        string Description { get; }
        string FullPath { get; }
        bool IsRenamable { get; }

        DateTime CreationTimeUtc { get; }
        DateTime LastUpdateTimeUtc { get; }
    }

    public interface IConvertedEntryModel : IEntryModel
    {
        IEntryModel OriginalEntryModel { get; }
    }

    public class NullEntryModel : IEntryModel
    {
        public static NullEntryModel Instance = new NullEntryModel();


        public IProfile Profile { get { return null; } }

        public bool IsDirectory { get { return false; } }
        public IEntryModel Parent { get { return null; } }
        public string Label { get { return null; } }
        public string Name { get { return null; } set { } }
        public string Description { get { return null; } }
        public string FullPath { get { return null; } }
        public bool IsRenamable { get { return false; } }

        public DateTime CreationTimeUtc { get { return DateTime.MinValue; } }
        public DateTime LastUpdateTimeUtc { get { return DateTime.MinValue; } }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Equals(IEntryModel other)
        {
            return false;
        }
    }
}
