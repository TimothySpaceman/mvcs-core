namespace Core.Events;

public interface IRepositoryEvents
{
    public event Func<CommitEventArgs, CancellationToken, Task>? OnCommitAsync;
    public event Func<CheckoutEventArgs, CancellationToken, Task>? OnCheckoutAsync;
    public Task NotifyOnCommitAsync(CommitEventArgs args, CancellationToken cancellationToken = default);
    public Task NotifyOnCheckoutAsync(CheckoutEventArgs args, CancellationToken cancellationToken = default);
}