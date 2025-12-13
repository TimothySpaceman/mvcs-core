using System.IO.Hashing;
using System.Text;

namespace Core.Storage.FileSnapshots;

public class FileSnapshotStore : IFileSnapshotStore
{
    private readonly Dictionary<HashId, FileSnapshot> _snapshots = new();

    private static HashId GenerateId(string filePath, HashId blobId, DateTime modified)
    {
        var hasher = new XxHash128();
        hasher.Append(Encoding.UTF8.GetBytes(filePath));
        hasher.Append(blobId.Bytes.Span);
        hasher.Append(BitConverter.GetBytes(modified.Ticks));
        return new HashId(hasher.GetHashAndReset());
    }

    public FileSnapshot? Get(HashId id)
    {
        return _snapshots.GetValueOrDefault(id);
    }

    public FileSnapshot Add(string filePath, HashId blobId, DateTime modified)
    {
        var id = GenerateId(filePath, blobId, modified);

        if (_snapshots.TryGetValue(id, out var existing)) return existing;

        var snapshot = new FileSnapshot(id, filePath, blobId, modified);
        _snapshots.Add(snapshot.Id, snapshot);
        return snapshot;
    }

    public bool Remove(HashId id)
    {
        return _snapshots.Remove(id);
    }
}