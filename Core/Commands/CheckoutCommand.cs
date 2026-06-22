using Core.Events;
using Core.Exceptions;
using Core.Repositories;
using Core.Snapshots;
using Core.Storage;
using Core.WorkingDirectories;

namespace Core.Commands;

public class CheckoutCommand : IRepositoryCommand<bool>
{
    private readonly HashId _commitId;
    private readonly bool _force;
    private readonly bool _silent;

    public CheckoutCommand(HashId commitId, bool force = false, bool silent = false)
    {
        _commitId = commitId;
        _force = force;
        _silent = silent;
    }

    public async Task<bool> ExecuteAsync(RepositoryContext context, CancellationToken cancellationToken = default)
    {
        var currentCommitId = await context.GetHeadRef(cancellationToken);
        var currentSnapshot = new Snapshot([]);
        if (currentCommitId is not null)
        {
            currentSnapshot = await context.CommitService.GetSnapshotForCommitAsync(
                (HashId)currentCommitId,
                cancellationToken
            ).ConfigureAwait(false);
        }

        var targetSnapshot = await context.CommitService.GetSnapshotForCommitAsync(
            _commitId,
            cancellationToken
        ).ConfigureAwait(false);

        var targetIgnoreRules = await GetIgnoreRulesForTargetAsync(context, targetSnapshot, cancellationToken);

        await context.WorkingDirectory.ApplySnapshotAsync(
            currentSnapshot,
            targetSnapshot,
            targetIgnoreRules,
            _force,
            cancellationToken
        );

        context.IgnoreRuleSet.FillFrom(targetIgnoreRules);
        await context.SetHeadRef(_commitId, "CHECKOUT", cancellationToken);

        if (!_silent)
        {
            var eventArgs = new CheckoutEventArgs(_commitId, _force);
            await context.Events.NotifyOnCheckoutAsync(eventArgs, cancellationToken).ConfigureAwait(false);
        }

        return true;
    }

    private async Task<IgnoreRuleSet> GetIgnoreRulesForTargetAsync(
        IRepositoryContext context,
        Snapshot targetSnapshot,
        CancellationToken cancellationToken = default
    )
    {
        var ignoreFilePath = context.ConfigService.Get("repo.ignore.path");
        var targetIgnoreRules = new IgnoreRuleSet();
        if (ignoreFilePath is null || !targetSnapshot.Files.TryGetValue(ignoreFilePath, out var ignoreFileSnapshot))
        {
            return targetIgnoreRules;
        }

        await using var blobStream = await context.BlobService.GetContentAsync(
            ignoreFileSnapshot.BlobId,
            cancellationToken
        );

        if (blobStream is null)
        {
            throw new BlobContentNotFoundException($"Cannot find ignore file content for commit {_commitId}");
        }

        using var reader = new StreamReader(blobStream);
        var rules = (await reader.ReadToEndAsync(cancellationToken)).Split(["\r\n", "\n"], StringSplitOptions.None);
        IgnoreRuleSetParser.HydrateIgnoreRuleSet(targetIgnoreRules, rules);

        return targetIgnoreRules;
    }
}