using Core.Blobs;
using Core.Commits;
using Core.Diffing;
using Core.Events;
using Core.Refs;
using Core.Storage;
using Core.WorkingDirectories;

namespace Core.Repositories;

public class RepositoryContext : IRepositoryContext
{
    public IRepositoryEvents Events { get; }
    public IBlobService BlobService { get; }
    public ICommitService CommitService { get; }
    public IDiffService DiffService { get; }
    public IWorkingDirectory WorkingDirectory { get; }
    public IRefLog RefLog { get; }
    public IgnoreRuleSet IgnoreRuleSet { get; }

    public RepositoryContext(
        IBlobService blobService,
        ICommitService commitService,
        IDiffService diffService,
        IWorkingDirectory workingDirectory,
        IRefLog refLog,
        IgnoreRuleSet ignoreRuleSet,
        IRepositoryEvents events
    )
    {
        BlobService = blobService;
        CommitService = commitService;
        DiffService = diffService;
        WorkingDirectory = workingDirectory;
        RefLog = refLog;
        IgnoreRuleSet = ignoreRuleSet;
        Events = events;
    }

    public Task<HashId?> GetHeadRef(CancellationToken cancellationToken = default)
    {
        return RefLog.GetAsync("HEAD", cancellationToken);
    }

    public Task SetHeadRef(HashId headRef, string message, CancellationToken cancellationToken = default)
    {
        return RefLog.SetAsync("HEAD", headRef, message, cancellationToken);
    }
}