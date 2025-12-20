using Core.Commits;
using Core.FileChanges;
using Core.Storage;
using Core.WorkingDirectories;

namespace Core.Repositories;

public interface IRepository
{
    public IgnoreRuleSet IgnoreRuleSet { get; }

    public IEnumerable<Commit> GetCommitsHistory();

    public IEnumerable<FileChange> GetStatus();

    public Commit Commit(string message, IEnumerable<FileChange> changes);

    public void CheckoutCommit(HashId commitId, bool force = false);
}