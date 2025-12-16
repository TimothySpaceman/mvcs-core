using System.IO.Hashing;
using Core.Storage;

namespace Core.Blobs;

public class BlobMetadataStore : IBlobMetadataStore
{
    protected readonly Dictionary<HashId, BlobMetadata> _blobs = new();

    public BlobMetadata? Get(HashId id)
    {
        return _blobs.GetValueOrDefault(id);
    }

    public void Add(BlobMetadata blobMetadata)
    {
        _blobs.Add(blobMetadata.Id, blobMetadata);
    }

    public bool Remove(HashId id)
    {
        return _blobs.Remove(id);
    }
}