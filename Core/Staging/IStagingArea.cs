using Core.FileChanges;

namespace Core.Staging;

public interface IStagingArea
{
    public bool IsEmpty { get; }
    public void Clear();

    public void Add(IEnumerable<FileChange> changes);

    public void Remove(string filePath);

    public FileChange? GetByBefore(string filePath);
    public FileChange? GetByAfter(string filePath);
    public IEnumerable<FileChange> GetAll();
}