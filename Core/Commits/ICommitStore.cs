using System.Collections.Immutable;
using Core.FileChanges;
using Core.Storage;

namespace Core.Commits;

public interface ICommitStore
{
    Task<bool> HasAsync(HashId id, CancellationToken cancellationToken = default);
    Task<Commit?> GetAsync(HashId id, CancellationToken cancellationToken = default);
    Task<Dictionary<HashId, Commit>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Commit commit, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(HashId id, CancellationToken cancellationToken = default);
}