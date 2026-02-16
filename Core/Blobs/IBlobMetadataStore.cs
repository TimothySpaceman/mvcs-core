using Core.Storage;

namespace Core.Blobs;

public interface IBlobMetadataStore
{
    Task<bool> HasAsync(HashId id, CancellationToken cancellationToken = default);
    Task<BlobMetadata?> GetAsync(HashId id, CancellationToken cancellationToken = default);
    Task AddAsync(BlobMetadata blobMetadata, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(HashId id, CancellationToken cancellationToken = default);
}