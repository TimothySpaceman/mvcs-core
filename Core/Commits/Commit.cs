using System.Collections.Immutable;
using Core.FileChanges;
using Core.Storage;

namespace Core.Commits;

public record Commit
{
    public Commit(
        HashId id,
        HashId? parentId,
        string message,
        ImmutableArray<FileChange> changes,
        DateTimeOffset createdAt
    )
    {
        Id = id;
        ParentId = parentId;
        Message = message;
        Changes = changes;
        CreatedAt = createdAt;
    }

    public HashId Id { get; }
    public HashId? ParentId { get; }
    public string Message { get; }
    public ImmutableArray<FileChange> Changes { get; }
    public DateTimeOffset CreatedAt { get; }

    public bool IsInitial => ParentId == null;
}