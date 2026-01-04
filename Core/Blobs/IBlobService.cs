using Core.Storage;

namespace Core.Blobs;

public interface IBlobService
{
    public BlobMetadata AddMetadata(Stream content);
    public BlobMetadata? GetMetadata(HashId id);

    public void AddContent(HashId id, Stream content);
    public Stream? GetContent(HashId id);
    
    public BlobMetadata Add(Stream content);
}