using System.IO.Hashing;
using Core.Storage;

namespace Core.Blobs;

public class BlobMetadataFactory
{
    private static HashId GenerateId(Stream contentStream)
    {
        var hasher = new XxHash128();
        hasher.Append(contentStream);
        var hash = new HashId(hasher.GetHashAndReset());

        contentStream.Seek(0, SeekOrigin.Begin);
        return hash;
    }

    public static BlobMetadata CreateMetadata(Stream contentStream)
    {
        return new BlobMetadata(GenerateId(contentStream), contentStream.Length);
    }
}