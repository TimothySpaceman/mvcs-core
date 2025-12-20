using Core.Storage;

namespace Core.Blobs;

public interface IBlobService
{
    public BlobMetadata AddMetadata(Stream contentStream);
    public BlobMetadata? GetMetadata(HashId id);

    public void AddContent(HashId id, Stream contentStream);
    public Stream? GetContent(HashId id);
    
    public BlobMetadata Add(Stream contentStream);
}