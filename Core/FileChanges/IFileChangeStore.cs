using Core.FileSnapshots;
using Core.Storage;

namespace Core.FileChanges;

public interface IFileChangeStore
{
    public FileChange? Get(HashId id);
    public void Add(FileChange fileChange);
    public bool Remove(HashId id);
}