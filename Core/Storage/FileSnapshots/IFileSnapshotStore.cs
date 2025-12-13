namespace Core.Storage.FileSnapshots;

public interface IFileSnapshotStore
{
    public FileSnapshot? Get(HashId id);
    public FileSnapshot Add(string filePath, HashId blobId, DateTime modified);
    public bool Remove(HashId id);
}