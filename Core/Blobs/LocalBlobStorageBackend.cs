using System.IO.Hashing;
using Core.Config;
using Core.Exceptions;
using Core.Storage;
using Core.Blobs;

namespace Core.Blobs;

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

    private string EnsureBlobDir()
    {
        var dirPath = GetBlobDirPath();
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        return dirPath;
    }

    public Stream? GetBlob(HashId id)
    {
        var blobPath = Path.Combine(EnsureBlobDir(), id.ToHexString());
        if (!File.Exists(blobPath)) return null;
        return File.OpenRead(blobPath);
    }

    public void PutBlob(HashId id, Stream content)
    {
        EnsureBlobDir();
        var blobPath = Path.Combine(GetBlobDirPath(), id.ToHexString());

        if (File.Exists(blobPath)) return;

        using var fileStream = File.Open(blobPath, FileMode.Create);
        content.CopyTo(fileStream);
        content.Seek(0, SeekOrigin.Begin);
    }

    public bool RemoveBlob(HashId id)
    {
        var blobPath = Path.Combine(GetBlobDirPath(), id.ToHexString());

        if (!File.Exists(blobPath)) return false;

        File.Delete(blobPath);
        return true;
    }
}