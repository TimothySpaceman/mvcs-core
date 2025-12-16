using Core.Storage;

namespace Core.FileSnapshots;

public interface IFileSnapshotStore
{
    public FileSnapshot? Get(HashId id);
    public void Add(FileSnapshot snapshot);
    public bool Remove(HashId id);
}