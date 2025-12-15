using Core.Storage;

namespace Core.FileSnapshots;

public record FileSnapshot(HashId Id, string FilePath, HashId BlobId, DateTimeOffset LastModified);