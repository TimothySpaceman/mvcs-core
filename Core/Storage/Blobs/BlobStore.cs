using System.IO.Hashing;

namespace Core.Storage.Blobs;

public class BlobStore : IBlobStore
{
    private readonly IBlobStorageBackend _storage;
    private readonly Dictionary<HashId, BlobMetadata> _blobs = new();

    public BlobStore(IBlobStorageBackend storage)
    {
        _storage = storage;
    }

    public BlobMetadata? Get(HashId id)
    {
        return _blobs.GetValueOrDefault(id);
    }

    public Stream? GetContent(HashId id)
    {
        return _storage.GetBlob(id);
    }

    public BlobMetadata Add(Stream contentStream)
    {
        var id = _storage.PutBlob(contentStream);

        if (_blobs.TryGetValue(id, out var existing)) return existing;

        var metadata = new BlobMetadata(id, contentStream.Length);
        _blobs.Add(id, metadata);
        return metadata;
    }

    public bool Remove(HashId id)
    {
        var removed = _blobs.Remove(id);
        if (removed)
        {
            _storage.RemoveBlob(id);
        }

        return removed;
    }
}