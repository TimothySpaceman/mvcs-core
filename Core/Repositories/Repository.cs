using Core.Blobs;
using Core.Commits;
using Core.Diffing;
using Core.FileChanges;
using Core.Refs;
using Core.Commands;
using Core.Events;
using Core.Storage;
using Core.WorkingDirectories;

namespace Core.Repositories;

public class Repository : IRepository
{
    private readonly RepositoryContext _context;

    public IgnoreRuleSet IgnoreRuleSet
    {
        get => _context.IgnoreRuleSet;
    }

    public event EventHandler<CommitEventArgs> OnCommit;
    public event EventHandler<CheckoutEventArgs> OnCheckout;

    public Repository(
        IBlobService blobService,
        ICommitService commitService,
        IDiffService diffService,
        IWorkingDirectory workingDirectory,
        IRefStore refStore
    )
    {
        var ignoreRules = new IgnoreRuleSet();
        var eventBus = new RepositoryEvents();

        _context = new RepositoryContext(
            blobService, commitService, diffService, workingDirectory, refStore, ignoreRules, eventBus
        );

        SetupEventProxies();
    }

    private void SetupEventProxies()
    {
        _context.Events.OnCommit += (args) => OnCommit?.Invoke(this, args);
        _context.Events.OnCheckout += (args) => OnCheckout?.Invoke(this, args);
    }

    public T Execute<T>(IRepositoryCommand<T> command)
    {
        return command.Execute(_context);
    }

    public Commit Commit(string message, IEnumerable<FileChange> changes)
    {
        return Execute(new CommitCommand(message, changes));
    }

    public void CheckoutCommit(HashId commitId, bool force = false)
    {
        Execute(new CheckoutCommand(commitId, force));
    }

    public IEnumerable<FileChange> GetStatus()
    {
        return Execute(new GetStatusCommand());
    }

    public IEnumerable<Commit> GetCommitsHistory()
    {
        return Execute(new GetHistoryCommand());
    }
}