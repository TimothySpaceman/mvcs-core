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
        if (rootDir == null)
        {
            throw new InvalidConfigException("repo.dir config must be set when working with LocalWorkingDirectory");
        }

        var matcher = new Matcher();
        matcher.AddInclude("**/*");
        matcher.AddExclude(rootDir);
        if (ignoreRules != null)
        {
            matcher.AddExcludePatterns(ignoreRules.ExcludeRules);
            matcher.AddIncludePatterns(ignoreRules.IncludeRules);
        }

        return matcher;
    }

    public Stream GetFileContent(string filePath)
    {
        return File.OpenRead(Path.Combine(_rootPath, filePath));
    }

    public void PutFileContent(string filePath, Stream content)
    {
        var fullPath = Path.Combine(Path.GetFullPath(_rootPath), filePath);

        var fileDir = Path.GetDirectoryName(fullPath);
        if (fileDir != null && !Directory.Exists(fileDir))
        {
            Directory.CreateDirectory(fileDir);
        }

        using var fileStream = File.Create(fullPath);
        content.CopyTo(fileStream);
    }

    public Snapshot GetCurrentSnapshot(IgnoreRuleSet? ignoreRules = null)
    {
        var matcher = GetMatcherForRules(ignoreRules);
        var filePaths = matcher.GetResultsInFullPath(_rootPath);
        var files = new Dictionary<string, FileSnapshot>();

        foreach (var filePath in filePaths)
        {
            var relativePath = Path.GetRelativePath(Path.GetFullPath(_rootPath), filePath);

            using var stream = File.OpenRead(filePath);
            var blobMetadata = BlobMetadataFactory.CreateMetadata(stream);
            var fileSnapshot = new FileSnapshot(
                relativePath,
                blobMetadata.Id,
                File.GetLastWriteTimeUtc(filePath)
            );

            files.Add(relativePath, fileSnapshot);
        }

        return new Snapshot(files.ToImmutableDictionary());
    }

    public void ApplySnapshot(Snapshot snapshot, IgnoreRuleSet? ignoreRules = null)
    {
        var matcher = GetMatcherForRules(ignoreRules);
        var currentFiles = matcher.GetResultsInFullPath(_rootPath).ToList();

        foreach (var fileEntry in snapshot.Files)
        {
            var fileSnapshot = fileEntry.Value;
            using var blobStream = _blobStorageBackend.GetBlob(fileSnapshot.BlobId);
            if (blobStream == null)
            {
                throw new BlobContentNotFoundException($"Blob content for {fileSnapshot.BlobId} not found");
            }

            var fullPath = Path.Combine(Path.GetFullPath(_rootPath), fileEntry.Key);
            currentFiles.Remove(fullPath);

            PutFileContent(fileEntry.Key, blobStream);
        }

        foreach (var file in currentFiles)
        {
            File.Delete(file);
        }
    }
}