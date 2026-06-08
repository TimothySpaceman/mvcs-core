using System.Collections.Immutable;
using Core.FileChanges;
using Core.Identities;
using Core.Storage;

namespace Core.Commits;

public enum CommitKind
{
    Default = 0,
    Merge = 1,
    Revert = 2
}

public record Commit(
    HashId Id,
    HashId? ParentId,
    HashId? SecondParentId,
    CommitKind Kind,
    string Message,
    ImmutableArray<FileChange> Changes,
    UserIdentity Author,
    DateTimeOffset CreatedAt
)
{
    public bool IsInitial => ParentId is null;
}