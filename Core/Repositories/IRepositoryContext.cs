using Core.Blobs;
using Core.Commits;
using Core.Config;
using Core.Diffing;
using Core.Events;
using Core.Refs;
using Core.Storage;
using Core.WorkingDirectories;

namespace Core.Repositories;

public interface IRepositoryContext
{
    public IRepositoryEvents Events { get; }

    public IConfigService ConfigService { get; }
    public IBlobService BlobService { get; }
    public ICommitService CommitService { get; }
    public IDiffService DiffService { get; }
    public IWorkingDirectory WorkingDirectory { get; }
    public IRefLog RefLog { get; }
    public IgnoreRuleSet IgnoreRuleSet { get; }

    public Task<HashId?> GetHeadRef(CancellationToken cancellationToken = default);
    public Task SetHeadRef(HashId newValue, string message, CancellationToken cancellationToken = default);
}