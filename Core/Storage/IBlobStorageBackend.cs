namespace Core.Storage;

public interface IBlobStorageBackend
{
    public Stream? GetBlob(Guid id);
    public void PutBlob(Guid id, Stream content);
}