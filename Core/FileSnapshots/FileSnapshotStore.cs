using System.Buffers.Binary;
using System.IO.Hashing;
using System.Text;
using Core.Storage;

namespace Core.FileSnapshots;

public class FileSnapshotStore : IFileSnapshotStore
{
    private readonly Dictionary<HashId, FileSnapshot> _snapshots = new();

    public FileSnapshot? Get(HashId id)
    {
        return _snapshots.GetValueOrDefault(id);
    }

    public void Add(FileSnapshot snapshot)
    {
        _snapshots.Add(snapshot.Id, snapshot);
    }

    public bool Remove(HashId id)
    {
        return _snapshots.Remove(id);
    }
}