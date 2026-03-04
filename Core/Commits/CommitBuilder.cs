using System.Buffers.Binary;
using System.Collections.Immutable;
using System.IO.Hashing;
using System.Text;
using Core.FileChanges;
using Core.FileSnapshots;
using Core.Storage;

namespace Core.Commits;

public class CommitBuilder
{
    private HashId? _parentId;
    private string? _message;
    private List<FileChange> _changes = new();
    private DateTimeOffset? _createdAt;

    public CommitBuilder Reset()
    {
        _parentId = null;
        _message = null;
        _changes = new();
        _createdAt = null;

        return this;
    }

    public CommitBuilder AddParentId(HashId parentId)
    {
        _parentId = parentId;
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

        if (_changes.Count == 0)
        {
            throw new InvalidOperationException("Cannot create a commit with empty changes list");
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
        string message,
        IEnumerable<FileChange> changes,
        DateTimeOffset createdAt
    )
    {
        var hasher = new XxHash128();

        hasher.Append(parentId is not null ? ((HashId)parentId).Bytes.Span : [0]);
        hasher.Append(Encoding.UTF8.GetBytes(message!));

        foreach (var change in changes)
        {
            HashFileSnapshot(hasher, change.Before);
            HashFileSnapshot(hasher, change.After);
        }

        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteInt64LittleEndian(buffer, createdAt.UtcTicks);
        hasher.Append(buffer);

        return new HashId(hasher.GetHashAndReset());
    }

    public Commit GetCommit()
    {
        VerifyRequiredFields();

        _createdAt ??= DateTimeOffset.Now;
        var id = GenerateId(_parentId, _message!, _changes, (DateTimeOffset)_createdAt!);

        return new Commit(id, _parentId, _message!, _changes.ToImmutableArray(), (DateTimeOffset)_createdAt!);
    }
}