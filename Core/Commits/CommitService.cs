using System.Collections.Immutable;
using Core.Exceptions;
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

    public List<Commit> GetCommitsChain(HashId idTo, HashId? idFrom = null)
    {
        if (!_commitStore.Has(idTo))
        {
            throw new CommitNotFoundException($"Target commit with ID {idTo} not found");
        }

        if (idFrom != null && !_commitStore.Has((HashId)idFrom))
        {
            throw new CommitNotFoundException($"Beginning commit with ID {idFrom} not found");
        }

        var commit = _commitStore.Get(idTo)!;
        var parentId = commit.ParentId;
        var chain = new List<Commit>();

        while (commit != null)
        {
            chain.Insert(0, commit);
            if (commit.Id == idFrom || parentId == null) break;

            if (!_commitStore.Has((HashId)parentId))
            {
                throw new CommitNotFoundException($"Parent ({(HashId)parentId}) for commit {commit.Id} not found");
            }

            commit = _commitStore.Get((HashId)parentId);
            parentId = commit?.ParentId;
        }

        return chain;
    }

    public Snapshot GetSnapshotForCommit(HashId commitId)
    {
        var history = GetCommitsChain(commitId);

        var files = new Dictionary<string, FileSnapshot>();

        foreach (var commit in history)
        {
            foreach (var change in commit.Changes)
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

        return new Snapshot(files.ToImmutableDictionary());
    }
}