using Core.Storage;

namespace Core.Blobs;

public interface IBlobStore
{
    public BlobMetadata? Get(HashId id);
    public Stream? GetContent(HashId id);
    public BlobMetadata Add(Stream contentStream);
    public bool Remove(HashId id);
}