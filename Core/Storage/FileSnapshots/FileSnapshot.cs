namespace Core.Storage.FileSnapshots;

public class FileSnapshot
{
    public Guid Id { get; }
    public string FilePath { get; }
    public Guid BlobId { get; }
    public DateTime LastModified { get; }

    public FileSnapshot(Guid id, string filePath, Guid blobId, DateTime lastModified)
    {
        Id = id;
        FilePath = filePath;
        BlobId = blobId;
        LastModified = lastModified;
    }
}