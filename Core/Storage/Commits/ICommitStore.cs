using System.Collections.Immutable;
using Core.Storage.FileChanges;

namespace Core.Storage.Commits;

public interface ICommitStore
{
    public Commit? Get(HashId id);

    public Commit Add(
        HashId? parentId,
        string message,
        ImmutableArray<FileChange> changes,
        DateTimeOffset createdAt
    );

    public Commit Add(
        HashId? parentId,
        string message,
        ImmutableArray<FileChange> changes
    );

    public bool Remove(HashId id);
}