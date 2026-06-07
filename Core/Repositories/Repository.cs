using System.Runtime.CompilerServices;
using Core.Blobs;
using Core.Commits;
using Core.Diffing;
using Core.FileChanges;
using Core.Refs;
using Core.Commands;
using Core.Config;
using Core.Events;
using Core.Identities;
using Core.Storage;
using Core.WorkingDirectories;

namespace Core.Repositories;

public class Repository : IRepository
{
    private readonly RepositoryContext _context;

    public IgnoreRuleSet IgnoreRuleSet => _context.IgnoreRuleSet;

    public event Func<CommitEventArgs, CancellationToken, Task>? OnCommitAsync;
    public event Func<CheckoutEventArgs, CancellationToken, Task>? OnCheckoutAsync;

    public Repository(
        IConfigService configService,
        IBlobService blobService,
        ICommitService commitService,
        IDiffService diffService,
        IWorkingDirectory workingDirectory,
        IRefLog refLog
    )
    {
        var ignoreRules = new IgnoreRuleSet();
        var eventBus = new RepositoryEvents();

        _context = new RepositoryContext(
            configService,
            blobService,
            commitService,
            diffService,
            workingDirectory,
            refLog,
            ignoreRules,
            eventBus
        );

        SetupEventProxies();
    }

    private void SetupEventProxies()
    {
        _context.Events.OnCommitAsync += async (args, token) =>
        {
            if (OnCommitAsync is null) return;

            var handlers = OnCommitAsync.GetInvocationList();
            foreach (Func<CommitEventArgs, CancellationToken, Task> handler in handlers)
            {
                token.ThrowIfCancellationRequested();
                await handler(args, token);
            }
        };

        _context.Events.OnCheckoutAsync += async (args, token) =>
        {
            if (OnCheckoutAsync is null) return;

            var handlers = OnCheckoutAsync.GetInvocationList();
            foreach (Func<CheckoutEventArgs, CancellationToken, Task> handler in handlers)
            {
                token.ThrowIfCancellationRequested();
                await handler(args, token);
            }
        };
    }

    public async Task<T> ExecuteAsync<T>(IRepositoryCommand<T> command, CancellationToken cancellationToken = default)
    {
        return await command.ExecuteAsync(_context, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Commit> CommitAsync(
        string message, 
        IEnumerable<FileChange> changes,
        UserIdentity author,
        CancellationToken cancellationToken = default
        )
    {
        return await ExecuteAsync(new CommitCommand(message, changes, author), cancellationToken).ConfigureAwait(false);
    }

    public async Task CheckoutCommitAsync(HashId commitId, bool force = false,
        CancellationToken cancellationToken = default)
    {
        await ExecuteAsync(new CheckoutCommand(commitId, force), cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<FileChange>> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(new GetStatusCommand(), cancellationToken).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<Commit> GetCommitsHistoryAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var commitStream = await ExecuteAsync(new GetHistoryCommand(), cancellationToken).ConfigureAwait(false);

        await foreach (var commit in commitStream.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            yield return commit;
        }
    }

    public void Dispose()
    {
    }
}