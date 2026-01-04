using Core.Storage;

namespace Core.Blobs;

public class BlobService : IBlobService
{
    private readonly IBlobMetadataStore _metadataStore;
    private readonly IBlobStorageBackend _storageBackend;

    public BlobService(IBlobMetadataStore metadataStore, IBlobStorageBackend storageBackend)
    {
        _metadataStore = metadataStore;
        _storageBackend = storageBackend;
    }

    public BlobMetadata AddMetadata(Stream content)
    {
        var metadata = BlobMetadataFactory.CreateMetadata(content);
        _metadataStore.Add(metadata);
        return metadata;
    }

    public BlobMetadata? GetMetadata(HashId id)
    {
        return _metadataStore.Get(id);
    }

    public void AddContent(HashId id, Stream content)
    {
        _storageBackend.PutBlob(id, content);
    }

    public Stream? GetContent(HashId id)
    {
        return _storageBackend.GetBlob(id);
    }

    public BlobMetadata Add(Stream content)
    {
        var metadata = AddMetadata(content);
        AddContent(metadata.Id, content);
        return metadata;
    }
}