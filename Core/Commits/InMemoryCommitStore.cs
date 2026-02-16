using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IO.Hashing;
using System.Text;
using Core.FileChanges;
using Core.Storage;

namespace Core.Commits;

public class InMemoryCommitStore : ICommitStore
{
    private readonly ConcurrentDictionary<HashId, Commit> _commits = new();

    public Task<bool> HasAsync(HashId id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_commits.ContainsKey(id));
    }

    public Task<Commit?> GetAsync(HashId id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_commits.GetValueOrDefault(id));
    }

    public Task AddAsync(Commit commit, CancellationToken cancellationToken = default)
    {
        _commits.TryAdd(commit.Id, commit);
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(HashId id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_commits.TryRemove(id, out _));
    }
}