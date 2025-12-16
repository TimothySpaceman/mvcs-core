using Core.FileSnapshots;
using Core.Storage;

namespace Core.FileChanges;

public record FileChange
{
    public FileChange(HashId id, FileSnapshot? before = null, FileSnapshot? after = null)
    {
        if (before == null && after == null)
        {
            throw new ArgumentException("Both before and after cannot be null");
        }

        Id = id;
        Before = before;
        After = after;
    }

    public HashId Id { get; }
    public FileSnapshot? Before { get; }
    public FileSnapshot? After { get; }

    public bool IsCreation => Before == null && After != null;
    public bool IsRemoval => Before != null && After == null;
    public bool IsModification => Before != null && After != null;

    public bool IsContentChanged =>
        IsModification && ((FileSnapshot)Before!).BlobId != ((FileSnapshot)After!).BlobId;

    public bool IsFilePathChanged =>
        IsModification && ((FileSnapshot)Before!).FilePath != ((FileSnapshot)After!).FilePath;
}