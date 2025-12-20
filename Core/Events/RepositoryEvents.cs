namespace Core.Events;

public class RepositoryEvents : IRepositoryEvents
{
    public event Action<CommitEventArgs>? OnCommit;
    public event Action<CheckoutEventArgs>? OnCheckout;

    public void NotifyOnCommit(CommitEventArgs args)
    {
        OnCommit?.Invoke(args);
    }

    public void NotifyOnCheckout(CheckoutEventArgs args)
    {
        OnCheckout?.Invoke(args);
    }
}