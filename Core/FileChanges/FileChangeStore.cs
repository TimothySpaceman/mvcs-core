using System.IO.Hashing;
using Core.FileSnapshots;
using Core.Storage;

namespace Core.FileChanges;

public class FileChangeStore : IFileChangeStore
{
    private readonly Dictionary<HashId, FileChange> _changes = new();

    private static HashId GenerateId(FileSnapshot? before, FileSnapshot? after)
    {
        var hasher = new XxHash128();

        hasher.Append(before != null ? before.Id.Bytes.Span : [0]);
        hasher.Append(after != null ? after.Id.Bytes.Span : [0]);

        return new HashId(hasher.GetHashAndReset());
    }

    public FileChange? Get(HashId id)
    {
        return _changes.GetValueOrDefault(id);
    }

    public FileChange Add(FileSnapshot? before, FileSnapshot? after)
    {
        var id = GenerateId(before, after);
        if (_changes.TryGetValue(id, out var existing)) return existing;

        var change = new FileChange(id, before, after);
        _changes.Add(change.Id, change);
        return change;
    }

    public FileChange Add(FileSnapshot before)
    {
        return Add(before, null);
    }

    public bool Remove(HashId id)
    {
        return _changes.Remove(id);
    }
}