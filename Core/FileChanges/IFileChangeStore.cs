using Core.FileSnapshots;
using Core.Storage;

namespace Core.FileChanges;

public interface IFileChangeStore
{
    public FileChange? Get(HashId id);
    public FileChange Add(FileSnapshot? before, FileSnapshot? after);
    public FileChange Add(FileSnapshot before);
    public bool Remove(HashId id);
}