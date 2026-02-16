using System.IO.Hashing;
using Core.Storage;

namespace Core.Blobs;

public class BlobMetadataFactory
{
    private static HashId GenerateId(Stream content)
    {
        var hasher = new XxHash128();
        hasher.Append(content);
        var hash = new HashId(hasher.GetHashAndReset());

        content.Seek(0, SeekOrigin.Begin);
        return hash;
    }

    public static BlobMetadata CreateMetadata(Stream content)
    {
        return new BlobMetadata(GenerateId(content), content.Length);
    }

    private static async Task<HashId> GenerateIdAsync(
        Stream content,
        CancellationToken cancellationToken = default
    )
    {
        var hasher = new XxHash128();

        await hasher.AppendAsync(content, cancellationToken).ConfigureAwait(false);
        var hash = new HashId(hasher.GetHashAndReset());

        content.Seek(0, SeekOrigin.Begin);
        return hash;
    }

    public static async Task<BlobMetadata> CreateMetadataAsync(
        Stream content,
        CancellationToken cancellationToken = default
    )
    {
        var hashId = await GenerateIdAsync(content, cancellationToken).ConfigureAwait(false);

        return new BlobMetadata(hashId, content.Length);
    }
}