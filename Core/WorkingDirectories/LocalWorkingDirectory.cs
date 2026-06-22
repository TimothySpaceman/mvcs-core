using System.Collections.Immutable;
using System.Diagnostics;
using Core.Blobs;
using Core.Config;
using Core.Exceptions;
using Core.FileSnapshots;
using Core.Snapshots;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Core.WorkingDirectories;

internal enum CheckoutActions
{
    Write,
    Delete,
    Skip,
    Conflict
}

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
        return Path.Combine(Path.GetFullPath(_rootPath), DenormalizePath(filePath));
    }

    public bool HasFile(string filePath)
    {
        return File.Exists(Path.Combine(_rootPath, filePath));
    }

    public async Task<Stream?> GetFileContentAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(filePath);
        if (!File.Exists(fullPath)) return null;

        Stream stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            FileOptions.Asynchronous
        );

        if (!IsTextStream(stream)) return stream;

        var normalized = NormalizeLineEndings(stream);
        await stream.DisposeAsync();
        return normalized;
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

        Stream sourceStream = content;
        MemoryStream? denormalizedStream = null;
        if (IsTextStream(content))
        {
            denormalizedStream = DenormalizeLineEndings(content);
            sourceStream = denormalizedStream;
        }

        await using var fileStream = new FileStream(
            fullPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            FileOptions.Asynchronous
        );
        await sourceStream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);

        if (denormalizedStream is not null) await denormalizedStream.DisposeAsync();
    }

    public Task DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(filePath);
        File.Delete(fullPath);
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
            var relativePath = NormalizePath(Path.GetRelativePath(Path.GetFullPath(_rootPath), filePath));

            await using var stream = await GetFileContentAsync(relativePath, cancellationToken).ConfigureAwait(false);

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
        Snapshot currentSnapshot,
        Snapshot targetSnapshot,
        IgnoreRuleSet? targetIgnoreRules = null,
        bool force = false,
        CancellationToken cancellationToken = default
    )
    {
        var localSnapshot = await GetCurrentSnapshotAsync(targetIgnoreRules, cancellationToken);

        var paths = targetSnapshot.Files.Keys
            .Concat(currentSnapshot.Files.Keys)
            .Concat(localSnapshot.Files.Keys)
            .Distinct()
            .ToList();

        var pathsActions = paths.ToDictionary(
            path => path,
            path => GetCheckoutAction(
                localSnapshot.Files.GetValueOrDefault(path),
                currentSnapshot.Files.GetValueOrDefault(path),
                targetSnapshot.Files.GetValueOrDefault(path),
                targetIgnoreRules
            )
        );

        var conflicts = pathsActions.Where(x => x.Value == CheckoutActions.Conflict).ToList();

        if (force)
        {
            foreach (var conflict in conflicts)
            {
                pathsActions[conflict.Key] = targetSnapshot.Files.ContainsKey(conflict.Key)
                    ? CheckoutActions.Write
                    : CheckoutActions.Delete;
            }
        }
        else if (conflicts.Count != 0)
        {
            throw new WorkdirUnsavedException("Unable to checkout with unsaved changes");
        }

        var pathsToWrite = pathsActions.Where(x => x.Value == CheckoutActions.Write).Select(x => x.Key);
        var blobsToWrite = await Task.WhenAll(pathsToWrite.Select(async path =>
        {
            var fileSnapshot = targetSnapshot.Files[path];
            var blobStream = await _blobStorageBackend
                .GetBlobAsync(fileSnapshot.BlobId, cancellationToken)
                .ConfigureAwait(false);

            if (blobStream is null)
            {
                throw new BlobContentNotFoundException($"Blob content for {fileSnapshot.BlobId} not found");
            }

            return (path, blobStream);
        }));

        foreach (var pair in blobsToWrite)
        {
            await using var blobStream = pair.blobStream;
            await PutFileContentAsync(pair.path, blobStream, cancellationToken).ConfigureAwait(false);
        }

        var toDelete = pathsActions
            .Where(x => x.Value == CheckoutActions.Delete)
            .Select(x => GetFullPath(x.Key))
            .ToList();
        foreach (var fullPath in toDelete)
        {
            await DeleteFileAsync(DenormalizePath(fullPath), cancellationToken).ConfigureAwait(false);
        }
    }

    private CheckoutActions GetCheckoutAction(
        FileSnapshot? local,
        FileSnapshot? current,
        FileSnapshot? target,
        IgnoreRuleSet? targetIgnoreRules
    )
    {
        bool hasLocal = local is not null, hasCurrent = current is not null, hasTarget = target is not null;
        var filePath = (local?.FilePath ?? current?.FilePath ?? target?.FilePath)!;

        if (!hasLocal && !hasCurrent && !hasTarget || IsIgnored(filePath, targetIgnoreRules))
        {
            return CheckoutActions.Skip;
        }

        var isDirty = hasCurrent && local?.BlobId != current!.BlobId;
        var isNew = !hasCurrent && hasLocal;

        if ((isDirty || (isNew && hasTarget)) && local?.BlobId != target?.BlobId)
        {
            return CheckoutActions.Conflict;
        }

        if (hasTarget) return CheckoutActions.Write;
        if (hasCurrent && local?.BlobId == current!.BlobId) return CheckoutActions.Delete;

        return CheckoutActions.Skip;
    }

    private static string NormalizePath(string path)
    {
        return path.Replace('\\', '/');
    }

    private static string DenormalizePath(string path)
    {
        return path.Replace('/', Path.DirectorySeparatorChar);
    }

    private static bool IsTextStream(Stream stream)
    {
        var buffer = new byte[8000];
        var read = stream.Read(buffer, 0, buffer.Length);
        if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
        return Array.IndexOf(buffer, (byte)0, 0, read) == -1;
    }

    private static MemoryStream NormalizeLineEndings(Stream stream)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        var content = reader.ReadToEnd().Replace("\r\n", "\n").Replace("\r", "\n");
        var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
        return ms;
    }

    private static MemoryStream DenormalizeLineEndings(Stream stream)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        var content = reader.ReadToEnd()
            .Replace("\r\n", "\n")
            .Replace("\n", Environment.NewLine);
        var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
        return ms;
    }
}