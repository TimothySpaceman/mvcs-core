using System.Runtime.CompilerServices;
using Core.Commands;
using Core.Commits;
using Core.FileChanges;
using Core.Storage;
using Core.WorkingDirectories;

namespace Core.Repositories;

public interface IRepository
{
    public IgnoreRuleSet IgnoreRuleSet { get; }

    public Task<T> ExecuteAsync<T>(IRepositoryCommand<T> command, CancellationToken cancellationToken = default);

    public Task<Commit> CommitAsync(
        string message,
        IEnumerable<FileChange> changes,
        CancellationToken cancellationToken = default
    );

    public Task CheckoutCommitAsync(HashId commitId, bool force = false, CancellationToken cancellationToken = default);

    public Task<IEnumerable<FileChange>> GetStatusAsync(CancellationToken cancellationToken = default);

    public IAsyncEnumerable<Commit> GetCommitsHistoryAsync(CancellationToken cancellationToken = default);
}