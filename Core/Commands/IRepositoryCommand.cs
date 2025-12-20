using Core.Repositories;

namespace Core.Commands;

public interface IRepositoryCommand<out TResult>
{
    TResult Execute(RepositoryContext context);
}