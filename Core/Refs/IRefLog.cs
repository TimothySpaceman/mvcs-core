using Core.Storage;

namespace Core.Refs;

public interface IRefLog
{
    Task<IReadOnlyDictionary<string, HashId?>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<HashId?> GetAsync(string key, CancellationToken cancellationToken = default);
    Task SetAsync(string key, HashId? newValue, string message, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReflogEntry>> GetHistoryAsync(string key, CancellationToken cancellationToken = default);
}