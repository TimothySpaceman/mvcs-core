namespace Core.Storage.Blobs;

public interface IBlobStore
{
    public BlobMetadata? Get(Guid id);
    public Stream? GetContent(Guid id);
    public BlobMetadata Add(Stream contentStream);
    public bool Remove(Guid id);
}