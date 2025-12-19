using Core.FileSnapshots;
using Core.Storage;

namespace Core.FileChanges;

public record FileChange
{
    public FileChange(FileSnapshot? before = null, FileSnapshot? after = null)
    {
        if (before == null && after == null)
        {
            throw new ArgumentException("Both before and after cannot be null");
        }

        Before = before;
        After = after;
    }

    public FileSnapshot? Before { get; }
    public FileSnapshot? After { get; }

    public bool HasBothStates => Before != null && After != null;

    public bool IsCreation => Before == null && After != null;
    public bool IsRemoval => Before != null && After == null;
    
    public bool IsContentChanged => HasBothStates && Before!.BlobId != After!.BlobId;
    public bool IsFilePathChanged => HasBothStates && Before!.FilePath != After!.FilePath;
    public bool IsModification => IsContentChanged || IsFilePathChanged;
    public bool IsRename => IsFilePathChanged && !IsContentChanged;
}