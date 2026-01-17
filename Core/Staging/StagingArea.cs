using Core.FileChanges;

namespace Core.Staging;

public class StagingArea : IStagingArea
{
    private readonly List<FileChange> _changes = new();

    public bool IsEmpty => _changes.Count == 0;

    public void Clear()
    {
        _changes.Clear();
    }

    public void Add(IEnumerable<FileChange> changes)
    {
        
    }

    public void Remove(string filePath)
    {
        _changes.RemoveAll(change => change.After?.FilePath == filePath);
    }

    public FileChange? GetByBefore(string filePath)
    {
        return _changes.FirstOrDefault(change => change!.Before?.FilePath == filePath, null);
    }

    public FileChange? GetByAfter(string filePath)
    {
        return _changes.FirstOrDefault(change => change!.After?.FilePath == filePath, null);
    }

    public IEnumerable<FileChange> GetAll()
    {
        return _changes.AsEnumerable();
    }
}