using Core.Storage;

namespace Core.Blobs;

public interface IBlobStorageBackend
{
    public Stream? GetBlob(HashId id);
    public HashId PutBlob(Stream content);
    public bool RemoveBlob(HashId id);
}