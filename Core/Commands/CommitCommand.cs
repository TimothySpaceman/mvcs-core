using Core.Commits;
using Core.Events;
using Core.FileChanges;
using Core.Repositories;
using Core.Storage;
using FileNotFoundException = Core.Exceptions.FileNotFoundException;

namespace Core.Commands;

public class CommitCommand : IRepositoryCommand<Commit>
{
    private readonly string _message;
    private readonly IEnumerable<FileChange> _changes;

    public CommitCommand(string message, IEnumerable<FileChange> changes)
    {
        _message = message;
        _changes = changes;
    }

    public async Task<Commit> ExecuteAsync(RepositoryContext context, CancellationToken cancellationToken = default)
    {
        var commitBuilder = new CommitBuilder();
        var changesArray = _changes.ToArray();

        commitBuilder.AddMessage(_message).AddFileChanges(changesArray);

        var headRef = await context.GetHeadRef(cancellationToken);
        if (headRef is not null && !((HashId)headRef).IsEmpty)
        {
            commitBuilder.AddParentId((HashId)headRef);
        }

        var commit = commitBuilder.GetCommit();

        foreach (var change in changesArray)
        {
            if (change.After is null) continue;

            await using var content = await context.WorkingDirectory.GetFileContentAsync(
                change.After.FilePath,
                cancellationToken
            ).ConfigureAwait(false);

            if (content is null)
            {
                throw new FileNotFoundException($"File {change.After.FilePath} not found in working directory");
            }

            await context.BlobService.AddAsync(
                content,
                cancellationToken
            ).ConfigureAwait(false);
        }

        await context.CommitService.AddCommitAsync(commit, cancellationToken).ConfigureAwait(false);

        await context.SetHeadRef(commit.Id, "COMMIT", cancellationToken);

        var eventArgs = new CommitEventArgs(commit);
        await context.Events.NotifyOnCommitAsync(eventArgs, cancellationToken).ConfigureAwait(false);

        return commit;
    }
}