using Core.FileSnapshots;
using Core.Storage;

namespace Core.FileChanges;

public class FileChangeService : IFileChangeService
{
    private readonly IFileChangeStore _fileChangeStore;

    public FileChangeService(IFileChangeStore fileChangeStore)
    {
        _fileChangeStore = fileChangeStore;
    }

    public FileChange AddFileChange(FileSnapshot? before = null, FileSnapshot? after = null)
    {
        var fileChange = FileChangeFactory.CreateSnapshot(before, after);
        _fileChangeStore.Add(fileChange);
        return fileChange;
    }

    public FileChange? GetFileChange(HashId id)
    {
        return _fileChangeStore.Get(id);
    }
}