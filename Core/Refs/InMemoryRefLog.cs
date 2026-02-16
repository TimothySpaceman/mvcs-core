using System.Collections.Concurrent;
using Core.Storage;

namespace Core.Refs;

public class InMemoryRefLog : IRefLog
{
    private readonly ConcurrentDictionary<string, HashId> _currentRefs = new();
    private readonly ConcurrentDictionary<string, List<ReflogEntry>> _logs = new();

    public Task<HashId?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var exists = _currentRefs.TryGetValue(key, out var value);
        return Task.FromResult<HashId?>(exists ? value : null);
    }

    public Task SetAsync(
        string key,
        HashId newValue,
        string message,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var exists = _currentRefs.TryGetValue(key, out var oldValue);
        oldValue = exists ? oldValue : HashId.Empty;

        var entry = new ReflogEntry(
            oldValue,
            newValue,
            DateTimeOffset.Now,
            message
        );

        _currentRefs[key] = newValue;

        _logs.AddOrUpdate(
            key,
            _ => [entry],
            (_, list) =>
            {
                lock (list)
                {
                    list.Add(entry);
                }

                return list;
            }
        );

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ReflogEntry>> GetHistoryAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_logs.TryGetValue(key, out var list))
        {
            return Task.FromResult<IReadOnlyList<ReflogEntry>>([]);
        }

        lock (list)
        {
            var history = list.AsEnumerable().Reverse().ToList();
            return Task.FromResult<IReadOnlyList<ReflogEntry>>(history);
        }
    }
}