using Core.Blobs;
using Core.Storage;

namespace Core.FileSnapshots;

public interface IFileSnapshotService
{
    public FileSnapshot AddSnapshot(string filePath, HashId blobId, DateTimeOffset lastModified);
    public FileSnapshot? GetSnapshot(HashId id);

    public FileSnapshot Add(string filePath, Stream contentStream, DateTimeOffset lastModified);
}