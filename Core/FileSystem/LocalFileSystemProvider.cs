using System.Runtime.CompilerServices;

namespace Core.FileSystem;

/// <inheritdoc />
public class LocalFileSystemProvider : IFileSystemProvider
{
    /// <inheritdoc />
    public char DirectorySeparatorChar => Path.DirectorySeparatorChar;

    /// <inheritdoc />
    public char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;

    /// <inheritdoc />
    public string GetFullPath(string path)
    {
        return Path.GetFullPath(path);
    }

    /// <inheritdoc />
    public string GetFileName(string path)
    {
        return Path.GetFileName(path);
    }

    /// <inheritdoc />
    public string GetFileNameWithoutExtension(string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }

    /// <inheritdoc />
    public string GetExtension(string path)
    {
        return Path.GetExtension(path);
    }

    /// <inheritdoc />
    public string? GetDirectoryName(string path)
    {
        return Path.GetDirectoryName(path);
    }

    /// <inheritdoc />
    public string CombinePath(params string[] paths)
    {
        return Path.Combine(paths);
    }

    /// <inheritdoc />
    public bool IsPathRooted(string path)
    {
        return Path.IsPathRooted(path);
    }

    /// <inheritdoc />
    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    /// <inheritdoc />
    public void WriteAllBytes(string path, byte[] bytes)
    {
        File.WriteAllBytes(path, bytes);
    }

    /// <inheritdoc />
    public byte[] ReadAllBytes(string path)
    {
        return File.ReadAllBytes(path);
    }

    /// <inheritdoc />
    public void DeleteFile(string path)
    {
        File.Delete(path);
    }

    /// <inheritdoc />
    public void CopyFile(string sourcePath, string destinationPath, bool overwrite = false)
    {
        File.Copy(sourcePath, destinationPath, overwrite);
    }

    /// <inheritdoc />
    public void MoveFile(string sourcePath, string destinationPath, bool overwrite = false)
    {
        if (overwrite && File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        File.Move(sourcePath, destinationPath);
    }

    /// <inheritdoc />
    public Stream OpenRead(string path)
    {
        return new FileStream(path, FileMode.Open, FileAccess.Read);
    }

    /// <inheritdoc />
    public Stream OpenWrite(string path)
    {
        return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
    }

    /// <inheritdoc />
    public Stream Create(string path)
    {
        return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
    }

    /// <inheritdoc />
    public DateTime? GetLastWriteTimeUtc(string path)
    {
        return File.Exists(path) ? File.GetLastWriteTimeUtc(path) : null;
    }

    /// <inheritdoc />
    public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
    {
        File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
    }

    /// <inheritdoc />
    public long GetFileSize(string path)
    {
        return new FileInfo(path).Length;
    }

    /// <inheritdoc />
    public Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(File.Exists(path));
    }

    /// <inheritdoc />
    public Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
    {
        return File.WriteAllBytesAsync(path, bytes, cancellationToken);
    }

    /// <inheritdoc />
    public Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
    {
        return File.ReadAllBytesAsync(path, cancellationToken);
    }

    /// <inheritdoc />
    public Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        File.Delete(path);
        return Task.CompletedTask;
    }

    public async Task CopyFileAsync(string sourcePath, string destinationPath, bool overwrite = false,
        CancellationToken cancellationToken = default)
    {
        const int bufferSize = 256 * 1024;

        try
        {
            using var sourceStream = new FileStream(
                sourcePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize,
                useAsync: true
            );
            using var destinationStream = new FileStream(
                destinationPath,
                overwrite ? FileMode.Create : FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize,
                useAsync: true
            );

            await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            try
            {
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }
            }
            catch
            {
                // ignored
            }

            throw;
        }
    }

    /// <inheritdoc />
    public Task MoveFileAsync(string sourcePath, string destinationPath, bool overwrite = false,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        File.Move(sourcePath, destinationPath, overwrite);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true
        );

        return Task.FromResult<Stream>(stream);
    }

    /// <inheritdoc />
    public Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var stream = new FileStream(
            path,
            FileMode.OpenOrCreate,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true
        );

        return Task.FromResult<Stream>(stream);
    }

    /// <inheritdoc />
    public Task<Stream> CreateAsync(string path, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var stream = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true
        );

        return Task.FromResult<Stream>(stream);
    }

    /// <inheritdoc />
    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    /// <inheritdoc />
    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    /// <inheritdoc />
    public void DeleteDirectory(string path, bool recursive = false)
    {
        Directory.Delete(path, recursive);
    }

    /// <inheritdoc />
    public void MoveDirectory(string sourcePath, string destinationPath)
    {
        Directory.Move(sourcePath, destinationPath);
    }

    /// <inheritdoc />
    public IEnumerable<string> EnumerateFiles(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return Directory.EnumerateFiles(path, searchPattern, searchOption);
    }

    /// <inheritdoc />
    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return Directory.EnumerateDirectories(path, searchPattern, searchOption);
    }

    /// <inheritdoc />
    public IEnumerable<string> EnumerateEntries(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);
    }

    /// <inheritdoc />
    public Task<bool> DirectoryExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Directory.Exists(path));
    }

    /// <inheritdoc />
    public Task CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Directory.CreateDirectory(path);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteDirectoryAsync(string path, bool recursive = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Directory.Delete(path, recursive);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task MoveDirectoryAsync(string sourcePath, string destinationPath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Directory.Move(sourcePath, destinationPath);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> EnumerateFilesAsync(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();
        foreach (var file in Directory.EnumerateFiles(path, searchPattern, searchOption))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return file;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> EnumerateDirectoriesAsync(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();
        foreach (var directory in Directory.EnumerateDirectories(path, searchPattern, searchOption))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return directory;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<string> EnumerateEntriesAsync(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();
        foreach (var entry in Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return entry;
        }
    }
}