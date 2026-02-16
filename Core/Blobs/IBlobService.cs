using Core.Storage;

namespace Core.Blobs;

public interface IBlobService
{
    public Task<BlobMetadata> AddMetadataAsync(Stream content, CancellationToken cancellationToken = default);
    public Task<BlobMetadata?> GetMetadataAsync(HashId id, CancellationToken cancellationToken = default);
    public Task AddContentAsync(HashId id, Stream content, CancellationToken cancellationToken = default);
    public Task<Stream?> GetContentAsync(HashId id, CancellationToken cancellationToken = default);
    public Task<BlobMetadata> AddAsync(Stream content, CancellationToken cancellationToken = default);
}