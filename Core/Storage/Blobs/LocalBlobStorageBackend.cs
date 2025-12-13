using System.IO.Hashing;
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

    public HashId PutBlob(Stream contentStream)
    {
        var blobDirPath = EnsureBlobDir();
        var tempBlobPath = Path.Combine(blobDirPath, $"tmp-{Guid.NewGuid()}");

        var hasher = new XxHash128();

        const int bufferSize = 4 * 1024;
        var buffer = new byte[bufferSize];

        try
        {
            using (var fs = new FileStream(tempBlobPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize))
            {
                int bytesRead;
                while ((bytesRead = contentStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    hasher.Append(buffer.AsSpan(0, bytesRead));
                    fs.Write(buffer, 0, bytesRead);
                }
            }

            var hash = new HashId(hasher.GetHashAndReset());

            var blobPath = Path.Combine(blobDirPath, hash.ToHexString());
            File.Move(tempBlobPath, blobPath, true);

            return hash;
        }
        catch
        {
            if (File.Exists(tempBlobPath))
            {
                File.Delete(tempBlobPath);
            }

            throw;
        }
    }

    public bool RemoveBlob(HashId id)
    {
        var blobPath = Path.Combine(GetBlobDirPath(), id.ToHexString());

        if (!File.Exists(blobPath)) return false;

        File.Delete(blobPath);
        return true;
    }
}