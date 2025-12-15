using System.IO.Hashing;
using Core.Storage;

namespace Core.Blobs;

public class BlobStore : IBlobStore
{
    private readonly IBlobStorageBackend _storage;
    private readonly Dictionary<HashId, BlobMetadata> _blobs = new();

    public BlobStore(IBlobStorageBackend storage)
    {
        _storage = storage;
    }

    private static HashId GenerateId(Stream contentStream)
    {
        var hasher = new XxHash128();
        hasher.Append(contentStream);
        var hash = new HashId(hasher.GetHashAndReset());

        contentStream.Seek(0, SeekOrigin.Begin);
        return hash;
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
        var id = GenerateId(contentStream);
        if (_blobs.TryGetValue(id, out var existing)) return existing;

        _storage.PutBlob(id, contentStream);

        var metadata = new BlobMetadata(id, contentStream.Length);
        _blobs.Add(id, metadata);
        return metadata;
    }

    public bool Remove(HashId id)
    {
        var removed = _blobs.Remove(id);
        _storage.RemoveBlob(id);
        return removed;
    }
}