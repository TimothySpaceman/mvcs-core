using Core.Storage;

namespace Core.FileSnapshots;

public record FileSnapshot
{
    public FileSnapshot(HashId id, string filePath, HashId blobId, DateTimeOffset lastModified)
    {
        Id = id;
        FilePath = filePath;
        BlobId = blobId;
        LastModified = lastModified;
    }

    public HashId Id { get; }
    public string FilePath { get; }
    public HashId BlobId { get; }
    public DateTimeOffset LastModified { get; }
}