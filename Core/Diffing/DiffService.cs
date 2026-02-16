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

        var paths = new HashSet<string>(snapshotA.Files.Keys);
        paths.UnionWith(snapshotB.Files.Keys);
        foreach (var path in paths)
        {
            var before = snapshotA.Files.GetValueOrDefault(path);
            var after = snapshotB.Files.GetValueOrDefault(path);

            if (before?.BlobId == after?.BlobId && before?.FilePath == after?.FilePath) continue;

            changes.Add(new FileChange(before, after));
        }

        return changes.ToImmutableArray();
    }
}