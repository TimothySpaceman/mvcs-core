using System.Collections.Immutable;
using Core.FileChanges;
using Core.FileSnapshots;
using Core.Snapshots;

namespace Core.Diffing;

public interface IDiffService
{
    public ImmutableArray<FileChange> DiffSnapshots(Snapshot snapshotA, Snapshot snapshotB);
    public ImmutableArray<FileChange> InvertChanges(ImmutableArray<FileChange> changes);
}