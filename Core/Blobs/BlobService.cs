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

    public async Task<BlobMetadata> AddMetadataAsync(Stream content, CancellationToken cancellationToken = default)
    {
        var metadata = await BlobMetadataFactory.CreateMetadataAsync(content, cancellationToken).ConfigureAwait(false);

        await _metadataStore.AddAsync(metadata, cancellationToken).ConfigureAwait(false);

        return metadata;
    }

    public async Task<BlobMetadata?> GetMetadataAsync(HashId id, CancellationToken cancellationToken = default)
    {
        return await _metadataStore.GetAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task AddContentAsync(HashId id, Stream content, CancellationToken cancellationToken = default)
    {
        await _storageBackend.PutBlobAsync(id, content, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Stream?> GetContentAsync(HashId id, CancellationToken cancellationToken = default)
    {
        return await _storageBackend.GetBlobAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<BlobMetadata> AddAsync(Stream content, CancellationToken cancellationToken = default)
    {
        var metadata = await AddMetadataAsync(content, cancellationToken).ConfigureAwait(false);

        await AddContentAsync(metadata.Id, content, cancellationToken).ConfigureAwait(false);

        return metadata;
    }
}