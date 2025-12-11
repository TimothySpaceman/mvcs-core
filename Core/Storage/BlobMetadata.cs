namespace Core.Storage;

public class BlobMetadata
{
    public readonly Guid Id;
    public readonly byte[] Hash;
    public readonly long Length;

    public BlobMetadata(Guid id, byte[] hash, long length)
    {
        Id = id;
        Hash = hash;
        Length = length;
    }
};