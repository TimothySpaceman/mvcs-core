namespace Core.Storage.Blobs;

public class BlobMetadata
{
    public Guid Id { get; }
    public byte[] Hash { get; }
    public long Length { get; }

    public BlobMetadata(Guid id, byte[] hash, long length)
    {
        Id = id;
        Hash = hash;
        Length = length;
    }
};