using Core.Storage;

namespace Core.Blobs;

public interface IBlobMetadataStore
{
    public bool Has(HashId id);
    public BlobMetadata? Get(HashId id);
    public void Add(BlobMetadata blobMetadata);
    public bool Remove(HashId id);
}