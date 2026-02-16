using System.Collections.Concurrent;
using System.IO.Hashing;
using Core.Storage;

namespace Core.Blobs;

public class BlobMetadataStore : IBlobMetadataStore
{
    private readonly ConcurrentDictionary<HashId, BlobMetadata> _blobs = new();

    public Task<bool> HasAsync(HashId id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_blobs.ContainsKey(id));
    }

    public Task<BlobMetadata?> GetAsync(HashId id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_blobs.GetValueOrDefault(id));
    }

    public Task AddAsync(BlobMetadata blobMetadata, CancellationToken cancellationToken = default)
    {
        _blobs.TryAdd(blobMetadata.Id, blobMetadata);
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(HashId id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_blobs.TryRemove(id, out _));
    }
}