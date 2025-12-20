using Core.Storage;

namespace Core.FileSnapshots;

public record FileSnapshot(string FilePath, HashId BlobId, DateTimeOffset LastModified);