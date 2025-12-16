using Core.FileSnapshots;
using Core.Storage;

namespace Core.FileChanges;

public interface IFileChangeService
{
    public FileChange AddFileChange(FileSnapshot? before = null, FileSnapshot? after = null);
    public FileChange? GetFileChange(HashId id);
}