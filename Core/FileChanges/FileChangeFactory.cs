using System.Buffers.Binary;
using System.IO.Hashing;
using System.Text;
using Core.FileSnapshots;
using Core.Storage;

namespace Core.FileChanges;

public class FileChangeFactory
{
    private static HashId GenerateId(FileSnapshot? before, FileSnapshot? after)
    {
        var hasher = new XxHash128();

        hasher.Append(before != null ? before.Id.Bytes.Span : [0]);
        hasher.Append(after != null ? after.Id.Bytes.Span : [0]);

        return new HashId(hasher.GetHashAndReset());
    }

    public static FileChange CreateSnapshot(FileSnapshot? before = null, FileSnapshot? after = null)
    {
        var id = GenerateId(before, after);
        return new FileChange(id, before, after);
    }
}