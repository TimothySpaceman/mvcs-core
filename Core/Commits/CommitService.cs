using System.Collections.Immutable;
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

    public void AddCommit(Commit commit)
    {
        if (_commitStore.Has(commit.Id)) return;

        if (commit.ParentId != null && !_commitStore.Has((HashId)commit.ParentId!))
        {
            throw new CommitNotFoundException($"Parent commit with ID {commit.Id} not found");
        }

        _commitStore.Add(commit);
    }

    public Commit? GetCommit(HashId id)
    {
        return _commitStore.Get(id);
    }

    public IEnumerable<Commit> GetCommitsChain(HashId idTo, HashId? idFrom = null)
    {
        if (!_commitStore.Has(idTo))
        {
            throw new CommitNotFoundException($"Target commit with ID {idTo} not found");
        }

        if (idFrom != null && !_commitStore.Has((HashId)idFrom))
        {
            throw new CommitNotFoundException($"Beginning commit with ID {idFrom} not found");
        }

        return new CommitIterator(_commitStore, idTo, idFrom);
    }

    public Snapshot GetSnapshotForCommit(HashId commitId)
    {
        var history = GetCommitsChain(commitId).Reverse();

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