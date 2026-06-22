using Core.Commits;
using Core.Repositories;
using Core.Storage;

namespace Core.Commands;

public class GetHistoryCommand : IRepositoryCommand<IAsyncEnumerable<Commit>>
{
    public async Task<IAsyncEnumerable<Commit>> ExecuteAsync(
        RepositoryContext context,
        CancellationToken cancellationToken = default
    )
    {
        var headRef = await context.GetHeadRef(cancellationToken);

        if (headRef is null) return EmptyStream();

        return context.CommitService.GetCommitsChainAsync((HashId)headRef, null, cancellationToken);
    }

    private static async IAsyncEnumerable<Commit> EmptyStream()
    {
        yield break;
    }
}