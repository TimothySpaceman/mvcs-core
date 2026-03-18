using System.IO.Hashing;
using Core.Config;
using Core.Exceptions;
using Core.Storage;
using Core.Blobs;

namespace Core.Blobs;

public class LocalBlobStorageBackend : IBlobStorageBackend
{
    protected readonly IConfigService ConfigService;

    public LocalBlobStorageBackend(IConfigService configService)
    {
        ConfigService = configService;
    }

    protected virtual string GetBlobDirPath()
    {
        var rootDir = ConfigService.Get("repo.dir");
        if (rootDir is null)
        {
            throw new InvalidConfigException("repo.dir config must be set when working with LocalBlobStorageBackend");
        }

        var blobDir = ConfigService.Get("blob.dir");
        if (blobDir is null)
        {
            throw new InvalidConfigException("blob.dir config must be set when working with LocalBlobStorageBackend");
        }

        return Path.Combine(rootDir, blobDir);
    }

    protected virtual string GetBlobPath(HashId id)
    {
        return Path.Combine(EnsureBlobDir(), id.ToHexString());
    }

    protected virtual string EnsureBlobDir()
    {
        var dirPath = GetBlobDirPath();

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        return dirPath;
    }

    public Task<Stream?> GetBlobAsync(HashId id, CancellationToken cancellationToken = default)
    {
        var blobPath = GetBlobPath(id);
        if (!File.Exists(blobPath)) return Task.FromResult<Stream?>(null);

        Stream stream = new FileStream(
            blobPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            FileOptions.Asynchronous
        );

        return Task.FromResult<Stream?>(stream);
    }

    public async Task PutBlobAsync(HashId id, Stream content, CancellationToken cancellationToken = default)
    {
        EnsureBlobDir();
        var blobPath = GetBlobPath(id);

        if (File.Exists(blobPath)) return;

        await using var fileStream = new FileStream(
            blobPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            FileOptions.Asynchronous
        );

        await content.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);

        content.Seek(0, SeekOrigin.Begin);
    }

    public Task<bool> RemoveBlobAsync(HashId id, CancellationToken cancellationToken = default)
    {
        var blobPath = Path.Combine(GetBlobDirPath(), id.ToHexString());

        if (!File.Exists(blobPath)) return Task.FromResult(false);

        File.Delete(blobPath);
        return Task.FromResult(true);
    }
}