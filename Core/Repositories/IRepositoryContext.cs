using Core.Blobs;
using Core.Commits;
using Core.Diffing;
using Core.Refs;
using Core.Storage;
using Core.WorkingDirectories;

namespace Core.Repositories;

public interface IRepositoryContext
{
    public IBlobService BlobService { get; }
    public ICommitService CommitService { get; }
    public IDiffService DiffService { get; }
    public IWorkingDirectory WorkingDirectory { get; }
    public IRefStore RefStore { get; }
    public IgnoreRuleSet IgnoreRuleSet { get; }
    public HashId? GetHeadRef();
    public void SetHeadRef(HashId headRef);
}