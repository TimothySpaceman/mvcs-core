using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Core.Exceptions;
using Core.FileChanges;
using Core.FileSnapshots;
using Core.Snapshots;
using Core.Storage;

namespace Core.Commits;

public class CommitService : ICommitService
{
    private readonly ICommitStore _commitStore;

    public CommitService(ICommitStore commitStore)
    {
        _commitStore = commitStore;
    }

    public async Task AddCommitAsync(Commit commit, CancellationToken cancellationToken = default)
    {
        if (await _commitStore.HasAsync(commit.Id, cancellationToken).ConfigureAwait(false)) return;

        if (
            commit.ParentId is not null &&
            !await _commitStore.HasAsync((HashId)commit.ParentId!, cancellationToken).ConfigureAwait(false)
        )
        {
            throw new CommitNotFoundException($"Parent commit with ID {commit.Id} not found");
        }

        await _commitStore.AddAsync(commit, cancellationToken);
    }

    public async Task<Commit?> GetCommitAsync(HashId id, CancellationToken cancellationToken = default)
    {
        return await _commitStore.GetAsync(id, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Dictionary<HashId, Commit>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _commitStore.GetAllAsync(cancellationToken).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<Commit> GetCommitsChainAsync(
        HashId idTo,
        HashId? idFrom = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        await foreach (var commit in GetCommitsChainCoreAsync(idTo, idFrom, null, cancellationToken))
            yield return commit;
    }

    public async IAsyncEnumerable<Commit> GetCommitsChainAsync(
        HashId idTo,
        HashId? idFrom,
        IReadOnlyList<Commit> supplement,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        await foreach (var commit in GetCommitsChainCoreAsync(idTo, idFrom, supplement, cancellationToken))
            yield return commit;
    }

    private async IAsyncEnumerable<Commit> GetCommitsChainCoreAsync(
        HashId idTo,
        HashId? idFrom,
        IReadOnlyList<Commit>? supplement,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var supplementMap = supplement?.ToDictionary(c => c.Id);

        async Task<Commit?> Resolve(HashId id) =>
            supplementMap?.GetValueOrDefault(id) ?? await _commitStore.GetAsync(id, cancellationToken);

        async Task<bool> Has(HashId id) =>
            supplementMap?.ContainsKey(id) == true || await _commitStore.HasAsync(id, cancellationToken);

        if (!await Has(idTo))
        {
            throw new CommitNotFoundException($"Target commit with ID {idTo} not found");
        }

        if (idFrom is not null && !await Has((HashId)idFrom))
        {
            throw new CommitNotFoundException($"Beginning commit with ID {idFrom} not found");
        }

        var currentId = idTo;
        while (!currentId.IsEmpty)
        {
            var commit = await Resolve(currentId);
            if (commit is null) throw new CommitNotFoundException($"Commit {currentId} not found");

            yield return commit;

            if (currentId == idFrom || commit.ParentId is null) break;
            currentId = (HashId)commit.ParentId;
        }
    }

    public async Task<Snapshot> GetSnapshotForCommitAsync(
        HashId commitId,
        CancellationToken cancellationToken = default
    )
    {
        return await GetSnapshotForCommitCoreAsync(commitId, null, cancellationToken);
    }

    public async Task<Snapshot> GetSnapshotForCommitAsync(
        HashId commitId,
        IReadOnlyList<Commit> supplement,
        CancellationToken cancellationToken = default
    )
    {
        return await GetSnapshotForCommitCoreAsync(commitId, supplement, cancellationToken);
    }

    private async Task<Snapshot> GetSnapshotForCommitCoreAsync(
        HashId commitId,
        IReadOnlyList<Commit>? supplement,
        CancellationToken cancellationToken = default
    )
    {
        var chain = GetCommitsChainCoreAsync(commitId, null, supplement, cancellationToken);

        var history = new List<Commit>();
        await foreach (var commit in chain.ConfigureAwait(false))
        {
            history.Add(commit);
        }

        history.Reverse();

        var files = new Dictionary<string, FileSnapshot>();
        foreach (var commit in history)
        {
            foreach (var change in commit.Changes)
            {
                ApplyFileChange(files, change);
            }
        }

        return new Snapshot(files.ToImmutableDictionary());
    }

    public async Task<Commit?> FindCommonAncestorAsync(
        HashId idA,
        HashId idB,
        IReadOnlyList<Commit>? supplement = null,
        CancellationToken cancellationToken = default
    )
    {
        var ancestorsA = new HashSet<HashId>();
        await foreach (
            var commit in GetCommitsChainCoreAsync(idA, null, supplement, cancellationToken)
        )
        {
            ancestorsA.Add(commit.Id);
        }

        await foreach (
            var commit in GetCommitsChainCoreAsync(idB, null, supplement, cancellationToken)
        )
        {
            if (ancestorsA.Contains(commit.Id)) return commit;
        }

        return null;
    }

    private static void ApplyFileChange(Dictionary<string, FileSnapshot> files, FileChange change)
    {
        if (change.IsCreation)
        {
            files.Add(change.After!.FilePath, change.After);
        }
        else if (change.IsRemoval)
        {
            files.Remove(change.Before!.FilePath);
        }
        else
        {
            files.Remove(change.Before!.FilePath);
            if (change.IsFilePathChanged) files.Remove(change.After!.FilePath);
            files.Add(change.After!.FilePath, change.After);
        }
    }
}