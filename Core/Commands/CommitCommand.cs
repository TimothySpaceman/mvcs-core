using Core.Commits;
using Core.FileChanges;
using Core.Repositories;
using Core.Storage;

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

    public Commit Execute(RepositoryContext context)
    {
        var commitBuilder = new CommitBuilder();
        var changesArray = _changes.ToArray();

        commitBuilder.AddMessage(_message).AddFileChanges(changesArray);

        var headRef = context.GetHeadRef();
        if (headRef != null && !((HashId)headRef).IsEmpty)
        {
            commitBuilder.AddParentId((HashId)headRef);
        }

        var commit = commitBuilder.GetCommit();

        foreach (var change in changesArray)
        {
            if (change.After == null) continue;
            using var contentStream = context.WorkingDirectory.GetFileContent(change.After.FilePath);
            context.BlobService.Add(contentStream);
        }

        context.CommitService.AddCommit(commit);
        context.SetHeadRef(commit.Id);

        return commit;
    }
}