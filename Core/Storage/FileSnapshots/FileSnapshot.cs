namespace Core.Storage.FileSnapshots;

public readonly record struct FileSnapshot
{
    public FileSnapshot(HashId id, string filePath, HashId blobId, DateTime lastModified)
    {
        Id = id;
        FilePath = filePath;
        BlobId = blobId;
        LastModified = lastModified;
    }

    public HashId Id { get; }
    public string FilePath { get; }
    public HashId BlobId { get; }
    public DateTime LastModified { get; }
}