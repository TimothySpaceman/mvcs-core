using Core.Events;
using Core.Exceptions;
using Core.Repositories;
using Core.Storage;

namespace Core.Commands;

public class CheckoutCommand : IRepositoryCommand<bool>
{
    private readonly HashId _commitId;
    private readonly bool _force;

    public CheckoutCommand(HashId commitId, bool force = false)
    {
        _commitId = commitId;
        _force = force;
    }

    public bool Execute(RepositoryContext context)
    {
        var statusCommand = new GetStatusCommand();
        var currentStatus = statusCommand.Execute(context);

        if (!_force && currentStatus.Any())
        {
            throw new WorkdirUnsavedException("Unable to checkout with unsaved changes");
        }

        var snapshot = context.CommitService.GetSnapshotForCommit(_commitId);
        context.WorkingDirectory.ApplySnapshot(snapshot, context.IgnoreRuleSet);
        context.SetHeadRef(_commitId);

        var eventArgs = new CheckoutEventArgs(_commitId, _force);
        context.Events.NotifyOnCheckout(eventArgs);

        return true;
    }
}