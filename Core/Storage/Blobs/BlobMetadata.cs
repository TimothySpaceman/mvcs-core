namespace Core.Storage.Blobs;

public record BlobMetadata
{
    public BlobMetadata(HashId id, long length)
    {
        Id = id;
        Length = length;
    }

    public HashId Id { get; }
    public long Length { get; }
};