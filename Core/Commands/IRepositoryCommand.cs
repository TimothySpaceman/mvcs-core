using Core.Repositories;

namespace Core.Commands;

public interface IRepositoryCommand<TResult>
{
    Task<TResult> ExecuteAsync(RepositoryContext context, CancellationToken cancellationToken = default);
}