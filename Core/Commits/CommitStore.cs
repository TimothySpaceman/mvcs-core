using System.Buffers.Binary;
using System.Collections.Immutable;
using System.IO.Hashing;
using System.Text;
using Core.FileChanges;
using Core.Storage;

namespace Core.Commits;

public class CommitStore : ICommitStore
{
    private readonly Dictionary<HashId, Commit> _commits = new();

    private static HashId GenerateId(
        HashId? parentId,
        string message,
        ImmutableArray<FileChange> changes,
        DateTimeOffset createdAt
    )
    {
        var hasher = new XxHash128();

        hasher.Append(parentId != null ? ((HashId)parentId).Bytes.Span : [0]);
        hasher.Append(Encoding.UTF8.GetBytes(message));

        foreach (var change in changes)
        {
            hasher.Append(change.Id.Bytes.Span);
        }

        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteInt64LittleEndian(buffer, createdAt.UtcTicks);
        hasher.Append(buffer);

        return new HashId(hasher.GetHashAndReset());
    }

    public Commit? Get(HashId id)
    {
        return _commits.GetValueOrDefault(id);
    }

    public Commit Add(
        HashId? parentId,
        string message,
        ImmutableArray<FileChange> changes,
        DateTimeOffset createdAt
    )
    {
        if (parentId != null && !_commits.ContainsKey((HashId)parentId))
        {
            throw new InvalidOperationException($"Cannot create commit. Parent commit {parentId} does not exist.");
        }

        var id = GenerateId(parentId, message, changes, createdAt);
        if (_commits.TryGetValue(id, out var existing)) return existing;

        var commit = new Commit(id, parentId, message, changes, createdAt);
        _commits.Add(id, commit);
        return commit;
    }

    public Commit Add(
        HashId? parentId,
        string message,
        ImmutableArray<FileChange> changes
    )
    {
        return Add(parentId, message, changes, DateTimeOffset.UtcNow);
    }

    public bool Remove(HashId id)
    {
        return _commits.Remove(id);
    }
}