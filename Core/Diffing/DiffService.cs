using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using Core.FileChanges;
using Core.Snapshots;

namespace Core.Diffing;

public class DiffService : IDiffService
{
    public ImmutableArray<FileChange> DiffSnapshots(Snapshot snapshotA, Snapshot snapshotB)
    {
        var changes = new List<FileChange>();

        var paths = snapshotA.Files.Keys.Concat(snapshotB.Files.Keys).Distinct();
        foreach (var path in paths)
        {
            var before = snapshotA.Files.GetValueOrDefault(path);
            var after = snapshotB.Files.GetValueOrDefault(path);

            if (before?.Id == after?.Id) continue;

            changes.Add(new FileChange(before, after));
        }

        return changes.ToImmutableArray();
    }
}