using System.Buffers.Binary;
using System.IO.Hashing;
using System.Text;
using Core.Storage;

namespace Core.FileSnapshots;

public class FileSnapshotFactory
{
    private static HashId GenerateId(string filePath, HashId blobId, DateTimeOffset lastModified)
    {
        var hasher = new XxHash128();

        hasher.Append(Encoding.UTF8.GetBytes(filePath));
        hasher.Append(blobId.Bytes.Span);

        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteInt64LittleEndian(buffer, lastModified.UtcTicks);
        hasher.Append(buffer);

        return new HashId(hasher.GetHashAndReset());
    }

    public static FileSnapshot CreateSnapshot(string filePath, HashId blobId, DateTimeOffset lastModified)
    {
        var id = GenerateId(filePath, blobId, lastModified);
        return new FileSnapshot(id, filePath, blobId, lastModified);
    }
}