using Core.Storage;

namespace Core.Blobs;

public record BlobMetadata(HashId Id, long Length);