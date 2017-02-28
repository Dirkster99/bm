namespace BreadcrumbLib.Models
{
    using BreadcrumbLib.Profile;
    using Caliburn.Micro;
    using DiskIO;

    public abstract class DiskProfileBase : ProfileBase, IDiskProfile
    {

        public DiskProfileBase(IEventAggregator events, params IConverterProfile[] converters)
            : base(converters)
        {
        }
        public DiskProfileBase(params IConverterProfile[] converters)
            : base(converters)
        {
////            MetadataProvider = new MetadataProviderBase(new BasicMetadataProvider(), new FileBasedMetadataProvider());
        }

        public IDiskIOHelper DiskIO { get; protected set; }
    }
}
