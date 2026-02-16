namespace Core.Events;

public class RepositoryEvents : IRepositoryEvents
{
    public event Func<CommitEventArgs, CancellationToken, Task>? OnCommitAsync;
    public event Func<CheckoutEventArgs, CancellationToken, Task>? OnCheckoutAsync;

    public async Task NotifyOnCommitAsync(CommitEventArgs args, CancellationToken cancellationToken = default)
    {
        if (OnCommitAsync == null) return;

        var handlers = OnCommitAsync.GetInvocationList();
        foreach (Func<CommitEventArgs, CancellationToken, Task> handler in handlers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await handler(args, cancellationToken);
        }
    }

    public async Task NotifyOnCheckoutAsync(CheckoutEventArgs args, CancellationToken cancellationToken = default)
    {
        if (OnCheckoutAsync == null) return;

        var handlers = OnCheckoutAsync.GetInvocationList();
        foreach (Func<CheckoutEventArgs, CancellationToken, Task> handler in handlers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await handler(args, cancellationToken);
        }
    }
}