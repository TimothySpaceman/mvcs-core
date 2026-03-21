using System.Collections.Immutable;
using Core.Blobs;
using Core.Config;
using Core.Exceptions;
using Core.FileSnapshots;
using Core.Snapshots;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Core.WorkingDirectories;

public class LocalWorkingDirectory : IWorkingDirectory
{
    private readonly string _rootPath;
    private readonly IConfigService _configService;
    private readonly IBlobStorageBackend _blobStorageBackend;

    public LocalWorkingDirectory(string rootPath, IConfigService configService, IBlobStorageBackend blobStorageBackend)
    {
        _rootPath = rootPath;
        _configService = configService;
        _blobStorageBackend = blobStorageBackend;
    }

    private Matcher GetMatcherForRules(IgnoreRuleSet? ignoreRules = null)
    {
        var rootDir = _configService.Get("repo.dir");
        if (rootDir is null)
        {
            throw new InvalidConfigException("repo.dir config must be set when working with LocalWorkingDirectory");
        }

        var matcher = new Matcher();
        matcher.AddInclude("**/*");
        matcher.AddExclude(rootDir);
        if (ignoreRules is not null)
        {
            matcher.AddExcludePatterns(ignoreRules.ExcludeRules);
            matcher.AddIncludePatterns(ignoreRules.IncludeRules);
        }

        return matcher;
    }
    
    public bool IsIgnored(string relativePath, IgnoreRuleSet? ignoreRules = null)
    {
        var matcher = GetMatcherForRules(ignoreRules);
        return !matcher.Match(relativePath).HasMatches;
    }

    private string GetFullPath(string filePath)
    {
        return Path.Combine(Path.GetFullPath(_rootPath), filePath);
    }

    public bool HasFile(string filePath)
    {
        return File.Exists(Path.Combine(_rootPath, filePath));
    }
    
    public Task<Stream?> GetFileContentAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_rootPath, filePath);
        if (!File.Exists(fullPath)) return Task.FromResult<Stream?>(null);

        Stream stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            FileOptions.Asynchronous
        );

        return Task.FromResult<Stream?>(stream);
    }

    public async Task PutFileContentAsync(
        string filePath,
        Stream content,
        CancellationToken cancellationToken = default
    )
    {
        var fullPath = GetFullPath(filePath);

        var fileDir = Path.GetDirectoryName(fullPath);
        if (fileDir is not null && !Directory.Exists(fileDir))
        {
            Directory.CreateDirectory(fileDir);
        }

        await using var fileStream = new FileStream(
            fullPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            FileOptions.Asynchronous
        );

        await content.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);

        content.Seek(0, SeekOrigin.Begin);
    }

    public Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        File.Delete(filePath);
        return Task.CompletedTask;
    }

    public async Task<Snapshot> GetCurrentSnapshotAsync(
        IgnoreRuleSet? ignoreRules = null,
        CancellationToken cancellationToken = default
    )
    {
        var matcher = GetMatcherForRules(ignoreRules);
        var filePaths = matcher.GetResultsInFullPath(_rootPath);
        var files = new Dictionary<string, FileSnapshot>();

        foreach (var filePath in filePaths)
        {
            var relativePath = Path.GetRelativePath(Path.GetFullPath(_rootPath), filePath);

            await using var stream = await GetFileContentAsync(filePath, cancellationToken).ConfigureAwait(false);

            var blobMetadata = await BlobMetadataFactory
                .CreateMetadataAsync(stream!, cancellationToken)
                .ConfigureAwait(false);

            var fileSnapshot = new FileSnapshot(
                relativePath,
                blobMetadata.Id,
                File.GetLastWriteTimeUtc(filePath)
            );

            files.Add(relativePath, fileSnapshot);
        }

        return new Snapshot(files.ToImmutableDictionary());
    }

    public async Task ApplySnapshotAsync(
        Snapshot snapshot,
        IgnoreRuleSet? ignoreRules = null,
        CancellationToken cancellationToken = default
    )
    {
        var matcher = GetMatcherForRules(ignoreRules);
        var currentFiles = matcher.GetResultsInFullPath(_rootPath).ToList();

        foreach (var (filePath, fileSnapshot) in snapshot.Files)
        {
            await using var blobStream = await _blobStorageBackend
                .GetBlobAsync(fileSnapshot.BlobId, cancellationToken)
                .ConfigureAwait(false);

            if (blobStream is null)
            {
                throw new BlobContentNotFoundException($"Blob content for {fileSnapshot.BlobId} not found");
            }

            var fullPath = GetFullPath(filePath);
            currentFiles.Remove(fullPath);

            await PutFileContentAsync(filePath, blobStream, cancellationToken).ConfigureAwait(false);
        }

        foreach (var file in currentFiles)
        {
            await DeleteFileAsync(file, cancellationToken).ConfigureAwait(false);
        }
    }
}