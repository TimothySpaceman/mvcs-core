namespace Core.Storage.Blob;

public interface IBlobStorageBackend
{
    public Stream? GetBlob(Guid id);
    public void PutBlob(Guid id, Stream content);
    public bool RemoveBlob(Guid id);
}