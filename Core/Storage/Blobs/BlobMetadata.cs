namespace Core.Storage.Blobs;

public readonly record struct BlobMetadata
{
    public BlobMetadata(HashId id, long length)
    {
        Id = id;
        Length = length;
    }

    public HashId Id { get; }
    public long Length { get; }
};