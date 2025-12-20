using Core.FileChanges;
using Core.Repositories;
using Core.Snapshots;
using Core.Storage;

namespace Core.Commands;

public class GetStatusCommand : IRepositoryCommand<IEnumerable<FileChange>>
{
    public IEnumerable<FileChange> Execute(RepositoryContext context)
    {
        var headRef = context.GetHeadRef();
        var commitSnapshot = Snapshot.Empty();

        if (headRef != null && !((HashId)headRef).IsEmpty)
        {
            commitSnapshot = context.CommitService.GetSnapshotForCommit((HashId)headRef);
        }

        var workDirSnapshot = context.WorkingDirectory.GetCurrentSnapshot(context.IgnoreRuleSet);
        return context.DiffService.DiffSnapshots(commitSnapshot, workDirSnapshot);
    }
}