using System.IO.Hashing;
using Core.FileSnapshots;
using Core.Storage;

namespace Core.FileChanges;

public class FileChangeStore : IFileChangeStore
{
    private readonly Dictionary<HashId, FileChange> _changes = new();

    public FileChange? Get(HashId id)
    {
        return _changes.GetValueOrDefault(id);
    }

    public void Add(FileChange fileChange)
    {
        _changes.Add(fileChange.Id, fileChange);
    }

    public bool Remove(HashId id)
    {
        return _changes.Remove(id);
    }
}