using Core.Snapshots;
using Core.Storage;

namespace Core.Commits;

public interface ICommitService
{
    public void AddCommit(Commit commit);
    public Commit? GetCommit(HashId id);

    public List<Commit> GetCommitsChain(HashId idTo, HashId? idFrom = null);

    public Snapshot GetSnapshotForCommit(HashId commitId);
}