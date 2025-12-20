using Core.Blobs;
using Core.Commits;
using Core.Diffing;
using Core.Exceptions;
using Core.FileChanges;
using Core.Refs;
using Core.Snapshots;
using Core.Storage;
using Core.WorkingDirectories;

namespace Core.Repositories;

public class Repository : IRepository
{
    private IBlobService _blobService;
    private ICommitService _commitService;
    private IDiffService _diffService;
    private IWorkingDirectory _workingDirectory;
    private IRefStore _refStore;

    public IgnoreRuleSet IgnoreRuleSet { get; set; }

    public Repository(
        IBlobService blobService,
        ICommitService commitService,
        IDiffService diffService,
        IWorkingDirectory workingDirectory,
        IRefStore refStore
    )
    {
        _blobService = blobService;
        _commitService = commitService;
        _diffService = diffService;
        _workingDirectory = workingDirectory;
        _refStore = refStore;

        IgnoreRuleSet = new IgnoreRuleSet();
    }

    private HashId GetHeadRef()
    {
        return _refStore.Get<HashId>("HEAD");
    }

    private void SetHeadRef(HashId headRef)
    {
        _refStore.Set("HEAD", headRef);
    }

    public IEnumerable<Commit> GetCommitsHistory()
    {
        return _commitService.GetCommitsChain(GetHeadRef());
    }

    public IEnumerable<FileChange> GetStatus()
    {
        var headRef = GetHeadRef();

        var commitSnapshot = Snapshot.Empty();
        if (headRef != null && !headRef.IsEmpty)
        {
            commitSnapshot = _commitService.GetSnapshotForCommit(headRef);
        }

        var workDirSnapshot = _workingDirectory.GetCurrentSnapshot(IgnoreRuleSet);
        return _diffService.DiffSnapshots(commitSnapshot, workDirSnapshot);
    }

    public Commit Commit(string message, IEnumerable<FileChange> changes)
    {
        var commitBuilder = new CommitBuilder();

        var changesArray = changes.ToArray();
        commitBuilder
            .AddMessage(message)
            .AddFileChanges(changesArray);

        var headRef = GetHeadRef();
        if (headRef != null && !headRef.IsEmpty) commitBuilder.AddParentId(headRef);

        var commit = commitBuilder.GetCommit();

        foreach (var change in changesArray)
        {
            if (change.After == null) continue;

            using var contentStream = _workingDirectory.GetFileContent(change.After.FilePath);
            _blobService.Add(contentStream);
        }

        _commitService.AddCommit(commit);
        SetHeadRef(commit.Id);
        return commit;
    }

    public void CheckoutCommit(HashId commitId, bool force = false)
    {
        if (!force && GetStatus().Any())
        {
            throw new WorkdirUnsavedException("Unable to checkout with unsaved changes in working directory");
        }

        var snapshot = _commitService.GetSnapshotForCommit(commitId);
        _workingDirectory.ApplySnapshot(snapshot, IgnoreRuleSet);
        SetHeadRef(commitId);
    }
}