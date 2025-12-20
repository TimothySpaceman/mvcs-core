using Core.Blobs;
using Core.Commits;
using Core.Diffing;
using Core.Refs;
using Core.Storage;
using Core.WorkingDirectories;

namespace Core.Repositories;

public class RepositoryContext : IRepositoryContext
{
    public IBlobService BlobService { get; }
    public ICommitService CommitService { get; }
    public IDiffService DiffService { get; }
    public IWorkingDirectory WorkingDirectory { get; }
    public IRefStore RefStore { get; }
    public IgnoreRuleSet IgnoreRuleSet { get; }

    public RepositoryContext(
        IBlobService blobService,
        ICommitService commitService,
        IDiffService diffService,
        IWorkingDirectory workingDirectory,
        IRefStore refStore,
        IgnoreRuleSet ignoreRuleSet
    )
    {
        BlobService = blobService;
        CommitService = commitService;
        DiffService = diffService;
        WorkingDirectory = workingDirectory;
        RefStore = refStore;
        IgnoreRuleSet = ignoreRuleSet;
    }

    public HashId? GetHeadRef() => RefStore.Get<HashId>("HEAD");
    public void SetHeadRef(HashId headRef) => RefStore.Set("HEAD", headRef);
}