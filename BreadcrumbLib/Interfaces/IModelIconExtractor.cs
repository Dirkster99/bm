namespace BreadcrumbLib.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IModelIconExtractor<T>
    {
        Task<byte[]> GetIconBytesForModelAsync(T model, CancellationToken ct);
    }
}
