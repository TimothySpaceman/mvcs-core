using Core.Storage;

namespace Core.Blobs;

public interface IBlobStorageBackend
{
    public Task<Stream?> GetBlobAsync(HashId id, CancellationToken cancellationToken = default);

    public Task PutBlobAsync(HashId id, Stream content, CancellationToken cancellationToken = default);

    public Task<bool> RemoveBlobAsync(HashId id, CancellationToken cancellationToken = default);
}