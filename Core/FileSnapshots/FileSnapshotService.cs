using Core.Blobs;
using Core.Storage;

namespace Core.FileSnapshots;

public class FileSnapshotService : IFileSnapshotService
{
    private readonly IBlobService _blobService;
    private readonly IFileSnapshotStore _snapshotStore;

    public FileSnapshotService(IBlobService blobService, IFileSnapshotStore fileSnapshotStore)
    {
        _blobService = blobService;
        _snapshotStore = fileSnapshotStore;
    }

    public FileSnapshot AddSnapshot(string filePath, HashId blobId, DateTimeOffset lastModified)
    {
        var snapshot = FileSnapshotFactory.CreateSnapshot(filePath, blobId, lastModified);
        _snapshotStore.Add(snapshot);
        return snapshot;
    }

    public FileSnapshot? GetSnapshot(HashId id)
    {
        return _snapshotStore.Get(id);
    }

    public FileSnapshot Add(string filePath, Stream contentStream, DateTimeOffset lastModified)
    {
        var blobMetadata = _blobService.Add(contentStream);
        return AddSnapshot(filePath, blobMetadata.Id, lastModified);
    }
}