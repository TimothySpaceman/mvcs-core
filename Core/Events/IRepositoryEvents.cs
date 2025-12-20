namespace Core.Events;

public interface IRepositoryEvents
{
    public event Action<CommitEventArgs>? OnCommit;
    public event Action<CheckoutEventArgs>? OnCheckout;
    public void NotifyOnCommit(CommitEventArgs args);
    public void NotifyOnCheckout(CheckoutEventArgs args);
}