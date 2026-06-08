using System.Buffers.Binary;
using System.Collections.Immutable;
using System.IO.Hashing;
using System.Text;
using Core.FileChanges;
using Core.FileSnapshots;
using Core.Identities;
using Core.Storage;

namespace Core.Commits;

public class CommitBuilder
{
    private HashId? _parentId;
    private HashId? _secondParentId;
    private CommitKind _kind;
    private string? _message;
    private List<FileChange> _changes = new();
    private DateTimeOffset? _createdAt;
    private UserIdentity? _author;

    public CommitBuilder Reset()
    {
        _parentId = null;
        _secondParentId = null;
        _kind = CommitKind.Default;
        _message = null;
        _changes = new();
        _author = null;
        _createdAt = null;

        return this;
    }

    public CommitBuilder FromCommit(Commit commit)
    {
        _message = commit.Message;
        _changes = commit.Changes.ToList();
        _author = commit.Author;
        _createdAt = commit.CreatedAt;
        return this;
    }

    public CommitBuilder AddParentId(HashId parentId)
    {
        _parentId = parentId;
        return this;
    }

    public CommitBuilder AddSecondParentId(HashId secondParentId)
    {
        _secondParentId = secondParentId;
        return this;
    }

    public CommitBuilder AddKind(CommitKind kind)
    {
        _kind = kind;
        return this;
    }

    public CommitBuilder AddMessage(string message)
    {
        _message = message;
        return this;
    }

    public CommitBuilder AddFileChange(FileChange fileChange)
    {
        _changes.Add(fileChange);
        return this;
    }

    public CommitBuilder AddFileChanges(IEnumerable<FileChange> fileChanges)
    {
        _changes.AddRange(fileChanges);
        return this;
    }

    public CommitBuilder AddAuthor(UserIdentity author)
    {
        _author = author;
        return this;
    }

    public CommitBuilder AddCreatedAt(DateTimeOffset createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    private void VerifyRequiredFields()
    {
        if (_message is null)
        {
            throw new InvalidOperationException("Cannot create a commit without message");
        }

        if (_author is null)
        {
            throw new InvalidOperationException("Cannot create a commit without author identity");
        }
    }

    private static void HashFileSnapshot(NonCryptographicHashAlgorithm hasher, FileSnapshot? snapshot)
    {
        if (snapshot is null)
        {
            hasher.Append([0]);
            return;
        }

        hasher.Append(snapshot.BlobId.Bytes.Span);
        hasher.Append(Encoding.UTF8.GetBytes(snapshot.FilePath));
    }

    private static HashId GenerateId(
        HashId? parentId,
        HashId? secondParentId,
        string message,
        IEnumerable<FileChange> changes,
        UserIdentity author,
        DateTimeOffset createdAt
    )
    {
        var hasher = new XxHash128();

        hasher.Append(parentId is not null ? ((HashId)parentId).Bytes.Span : [0]);
        hasher.Append(secondParentId is not null ? ((HashId)secondParentId).Bytes.Span : [0]);
        hasher.Append(Encoding.UTF8.GetBytes(message!));

        foreach (var change in changes)
        {
            HashFileSnapshot(hasher, change.Before);
            HashFileSnapshot(hasher, change.After);
        }

        hasher.Append(Encoding.UTF8.GetBytes(author.Name));
        hasher.Append(Encoding.UTF8.GetBytes(author.Email ?? ""));

        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteInt64LittleEndian(buffer, createdAt.UtcTicks);
        hasher.Append(buffer);

        return new HashId(hasher.GetHashAndReset());
    }

    public Commit GetCommit()
    {
        VerifyRequiredFields();

        _createdAt ??= DateTimeOffset.Now;
        var id = GenerateId(_parentId, _secondParentId, _message!, _changes, _author!, (DateTimeOffset)_createdAt!);

        return new Commit(
            id,
            _parentId,
            _secondParentId,
            _kind,
            _message!,
            _changes.ToImmutableArray(),
            _author!,
            (DateTimeOffset)_createdAt!
        );
    }
}