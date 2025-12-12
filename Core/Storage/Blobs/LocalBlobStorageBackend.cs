using Core.Config;
using Core.Exceptions;

namespace Core.Storage.Blobs;

public class LocalBlobStorageBackend : IBlobStorageBackend
{
    private readonly IConfigService _configService;

    public LocalBlobStorageBackend(IConfigService configService)
    {
        _configService = configService;
    }

    private string GetBlobDirPath()
    {
        var rootDir = _configService.Get("repo.dir");
        if (rootDir == null)
        {
            throw new InvalidConfigException("repo.dir config must be set when working with LocalBlobStorageBackend");
        }

        var blobDir = _configService.Get("blob.dir");
        if (blobDir == null)
        {
            throw new InvalidConfigException("blob.dir config must be set when working with LocalBlobStorageBackend");
        }

        return Path.Combine(rootDir, blobDir);
    }

    private void EnsureBlobDir()
    {
        var dirPath = GetBlobDirPath();
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }

    public Stream? GetBlob(Guid id)
    {
        EnsureBlobDir();
        var blobPath = Path.Combine(GetBlobDirPath(), id.ToString());
        if (!File.Exists(blobPath)) return null;
        return File.OpenRead(blobPath);
    }

    public void PutBlob(Guid id, Stream contentStream)
    {
        EnsureBlobDir();
        var blobPath = Path.Combine(GetBlobDirPath(), id.ToString());
        using var fileStream = File.Open(blobPath, FileMode.Create);
        contentStream.CopyTo(fileStream);
        contentStream.Seek(0, SeekOrigin.Begin);
    }

    public bool RemoveBlob(Guid id)
    {
        var blobPath = Path.Combine(GetBlobDirPath(), id.ToString());

        if (!File.Exists(blobPath)) return false;

        File.Delete(blobPath);
        return true;
    }
}