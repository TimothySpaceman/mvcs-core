using System.IO.Hashing;

namespace Core.Storage.Blob;

public class BlobStore : IBlobStore
{
    private readonly IBlobStorageBackend _storage;
    private readonly Dictionary<Guid, BlobMetadata> _blobs = new();

    public BlobStore(IBlobStorageBackend storage)
    {
        _storage = storage;
    }

    public BlobMetadata? Get(Guid id)
    {
        return _blobs.GetValueOrDefault(id);
    }

    public Stream? GetContent(Guid id)
    {
        return _storage.GetBlob(id);
    }

    public BlobMetadata Add(Stream contentStream)
    {
        var id = Guid.NewGuid();

        var hash = new XxHash128();
        hash.Append(contentStream);
        contentStream.Seek(0, SeekOrigin.Begin);

        _storage.PutBlob(id, contentStream);

        var metadata = new BlobMetadata(id, hash.GetHashAndReset(), contentStream.Length);
        _blobs.Add(id, metadata);
        return metadata;
    }

    public bool Remove(Guid id)
    {
        var removed = _blobs.Remove(id);
        if (removed)
        {
            _storage.RemoveBlob(id);
        }

        return removed;
    }
}