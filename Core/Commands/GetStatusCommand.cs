using Core.FileChanges;
using Core.Repositories;
using Core.Snapshots;
using Core.Storage;

namespace Core.Commands;

public class GetStatusCommand : IRepositoryCommand<IEnumerable<FileChange>>
{
    public async Task<IEnumerable<FileChange>> ExecuteAsync(RepositoryContext context,
        CancellationToken cancellationToken = default)
    {
        var headRef = await context.GetHeadRef(cancellationToken);
        var commitSnapshot = Snapshot.Empty();

        if (headRef is not null && !((HashId)headRef).IsEmpty)
        {
            var rawSnapshot = await context.CommitService.GetSnapshotForCommitAsync(
                (HashId)headRef,
                cancellationToken
            ).ConfigureAwait(false);
            
            commitSnapshot = rawSnapshot.WithoutFiles(
                path => context.WorkingDirectory.IsIgnored(path, context.IgnoreRuleSet)
            );
        }

        var workDirSnapshot = await context.WorkingDirectory.GetCurrentSnapshotAsync(
            context.IgnoreRuleSet,
            cancellationToken
        ).ConfigureAwait(false);

        return context.DiffService.DiffSnapshots(commitSnapshot, workDirSnapshot);
    }
}