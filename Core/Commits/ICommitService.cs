using Core.Snapshots;
using Core.Storage;

namespace Core.Commits;

public interface ICommitService
{
    public Task AddCommitAsync(Commit commit, CancellationToken cancellationToken = default);
    public Task<Commit?> GetCommitAsync(HashId id, CancellationToken cancellationToken = default);
    public Task<Dictionary<HashId, Commit>> GetAllAsync(CancellationToken cancellationToken = default);
    
    public IAsyncEnumerable<Commit> GetCommitsChainAsync(
        HashId idTo,
        HashId? idFrom = null,
        CancellationToken cancellationToken = default
    );

    public Task<Snapshot> GetSnapshotForCommitAsync(HashId commitId, CancellationToken cancellationToken = default);
}