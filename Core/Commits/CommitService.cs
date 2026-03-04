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
        ;
    }

    public async IAsyncEnumerable<Commit> GetCommitsChainAsync(
        HashId idTo,
        HashId? idFrom = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        if (!await _commitStore.HasAsync(idTo, cancellationToken).ConfigureAwait(false))
        {
            throw new CommitNotFoundException($"Target commit with ID {idTo} not found");
        }

        if (idFrom is not null && !await _commitStore.HasAsync((HashId)idFrom, cancellationToken))
        {
            throw new CommitNotFoundException($"Beginning commit with ID {idFrom} not found");
        }

        var currentId = idTo;
        while (!currentId.IsEmpty)
        {
            var commit = await _commitStore.GetAsync(currentId, cancellationToken).ConfigureAwait(false);
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
        var chain = GetCommitsChainAsync(commitId, null, cancellationToken).ConfigureAwait(false);
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