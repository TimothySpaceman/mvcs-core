using Core.Commits;
using Core.Repositories;
using Core.Storage;

namespace Core.Commands;

public class GetHistoryCommand : IRepositoryCommand<IEnumerable<Commit>>
{
    public IEnumerable<Commit> Execute(RepositoryContext context)
    {
        var headRef = context.GetHeadRef();
        if (headRef == null) return [];
        return context.CommitService.GetCommitsChain((HashId)headRef);
    }
}