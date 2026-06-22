using Core.Commits;
using Core.Storage;

namespace Core.Events;

public class CheckoutEventArgs : EventArgs
{
    public HashId TargetId { get; }
    public bool IsForced { get; }

    public CheckoutEventArgs(HashId targetId, bool isForced)
    {
        TargetId = targetId;
        IsForced = isForced;
    }
}