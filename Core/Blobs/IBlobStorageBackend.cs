using Core.Storage;

namespace Core.Blobs;

public interface IBlobStorageBackend
{
    public Stream? GetBlob(HashId id);
    public void PutBlob(HashId id, Stream content);
    public bool RemoveBlob(HashId id);
}