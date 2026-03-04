using System.Collections.Immutable;
using Core.FileChanges;
using Core.Storage;

namespace Core.Commits;

public record Commit(
    HashId Id,
    HashId? ParentId,
    string Message,
    ImmutableArray<FileChange> Changes,
    DateTimeOffset CreatedAt
)
{
    public bool IsInitial => ParentId is null;
}