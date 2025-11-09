using Core.Tests.Utils;

namespace Core.Tests.FileSystem.LocalFileSystemProvider;

public partial class LocalFileSystemProviderTests
{
    [Fact]
    public void DirectoryExists_ReturnsTrue_WhenDirectoryExists()
    {
        var dirPath = _tempDir.GetPath("existing_dir");
        Directory.CreateDirectory(dirPath);

        Assert.True(_provider.DirectoryExists(dirPath));
    }

    [Fact]
    public void DirectoryExists_ReturnsFalse_WhenDirectoryIsMissing()
    {
        var dirPath = _tempDir.GetPath("missing_dir");
        if (Directory.Exists(dirPath))
            Directory.Delete(dirPath);

        Assert.False(_provider.DirectoryExists(dirPath));
    }

    [Fact]
    public async Task DirectoryExistsAsync_ReturnsTrue_WhenDirectoryExists()
    {
        var dirPath = _tempDir.GetPath("existing_async_dir");
        Directory.CreateDirectory(dirPath);

        Assert.True(await _provider.DirectoryExistsAsync(dirPath));
    }

    [Fact]
    public async Task DirectoryExistsAsync_ReturnsFalse_WhenDirectoryIsMissing()
    {
        var dirPath = _tempDir.GetPath("missing_async_dir");
        if (Directory.Exists(dirPath))
            Directory.Delete(dirPath);

        Assert.False(await _provider.DirectoryExistsAsync(dirPath));
    }

    [Fact]
    public void CreateDirectory_WorksCorrectly()
    {
        var dirPath = _tempDir.GetPath("new_dir");

        _provider.CreateDirectory(dirPath);

        Assert.True(Directory.Exists(dirPath));
    }

    [Fact]
    public async Task CreateDirectoryAsync_WorksCorrectly()
    {
        var dirPath = _tempDir.GetPath("new_async_dir");

        await _provider.CreateDirectoryAsync(dirPath);

        Assert.True(Directory.Exists(dirPath));
    }

    [Fact]
    public void DeleteDirectory_WorksCorrectly()
    {
        var dirPath = _tempDir.GetPath("deleted_dir");
        Directory.CreateDirectory(dirPath);

        _provider.DeleteDirectory(dirPath);

        Assert.False(Directory.Exists(dirPath));
    }

    [Fact]
    public async Task DeleteDirectoryAsync_WorksCorrectly()
    {
        var dirPath = _tempDir.GetPath("deleted_async_dir");
        Directory.CreateDirectory(dirPath);

        await _provider.DeleteDirectoryAsync(dirPath);

        Assert.False(Directory.Exists(dirPath));
    }

    [Fact]
    public void MoveDirectory_WorksCorrectly()
    {
        var fromPath = _tempDir.GetPath("move_from_dir");
        var toPath = _tempDir.GetPath("move_to_dir");
        var fromContentPath = Path.Combine(fromPath, "content.txt");
        var toContentPath = Path.Combine(toPath, "content.txt");
        var contentData = "Test data";
        Directory.CreateDirectory(fromPath);
        File.WriteAllText(fromContentPath, contentData);
        if (Directory.Exists(toPath))
            Directory.Delete(toPath);

        _provider.MoveDirectory(fromPath, toPath);

        Assert.False(Directory.Exists(fromPath));
        Assert.Equal(File.ReadAllText(toContentPath), contentData);
    }

    [Fact]
    public async Task MoveDirectoryAsync_WorksCorrectly()
    {
        var fromPath = _tempDir.GetPath("move_from_async_dir");
        var toPath = _tempDir.GetPath("move_to_async_dir");
        var fromContentPath = Path.Combine(fromPath, "content.txt");
        var toContentPath = Path.Combine(toPath, "content.txt");
        var contentData = "Test data";
        Directory.CreateDirectory(fromPath);
        File.WriteAllText(fromContentPath, contentData);
        if (Directory.Exists(toPath))
            Directory.Delete(toPath);

        await _provider.MoveDirectoryAsync(fromPath, toPath);

        Assert.False(Directory.Exists(fromPath));
        Assert.Equal(File.ReadAllText(toContentPath), contentData);
    }

    [Fact]
    public void EnumerateFiles_ReturnsExpected()
    {
        var dirPath = _tempDir.GetPath("enum_files_dir");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "a.txt"), "Test");
        File.WriteAllText(Path.Combine(dirPath, "b.txt"), "Test");
        Directory.CreateDirectory(Path.Combine(dirPath, "sub"));

        var files = Enumerable.ToArray<string>(_provider.EnumerateFiles(dirPath));

        Assert.Collection(
            files,
            f => Assert.EndsWith("a.txt", f),
            f => Assert.EndsWith("b.txt", f)
        );
    }

    [Fact]
    public async Task EnumerateFilesAsync_ReturnsExpected()
    {
        var dirPath = _tempDir.GetPath("enum_files_async_dir");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "a.txt"), "Test");
        File.WriteAllText(Path.Combine(dirPath, "b.txt"), "Test");
        Directory.CreateDirectory(Path.Combine(dirPath, "sub"));

        var files = new List<string>();
        await foreach (var file in _provider.EnumerateFilesAsync(dirPath))
        {
            files.Add(file);
        }

        Assert.Collection(
            files,
            f => Assert.EndsWith("a.txt", f),
            f => Assert.EndsWith("b.txt", f)
        );
    }

    [Fact]
    public void EnumerateDirectories_ReturnsExpected()
    {
        var dirPath = _tempDir.GetPath("enum_dirs_dir");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "a.txt"), "Test");
        File.WriteAllText(Path.Combine(dirPath, "b.txt"), "Test");
        Directory.CreateDirectory(Path.Combine(dirPath, "sub"));

        var dirs = Enumerable.ToArray<string>(_provider.EnumerateDirectories(dirPath));

        Assert.Collection(
            dirs,
            d => Assert.EndsWith("sub", d)
        );
    }

    [Fact]
    public async Task EnumerateDirectoriesAsync_ReturnsExpected()
    {
        var dirPath = _tempDir.GetPath("enum_dirs_async_dir");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "a.txt"), "Test");
        File.WriteAllText(Path.Combine(dirPath, "b.txt"), "Test");
        Directory.CreateDirectory(Path.Combine(dirPath, "sub"));

        var dirs = new List<string>();
        await foreach (var dir in _provider.EnumerateDirectoriesAsync(dirPath))
        {
            dirs.Add(dir);
        }

        Assert.Collection(
            dirs,
            d => Assert.EndsWith("sub", d)
        );
    }

    [Fact]
    public void EnumerateEntries_ReturnsExpected()
    {
        var dirPath = _tempDir.GetPath("enum_entries_dir");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "a.txt"), "Test");
        File.WriteAllText(Path.Combine(dirPath, "b.txt"), "Test");
        Directory.CreateDirectory(Path.Combine(dirPath, "sub"));

        var entries = Enumerable.ToArray<string>(_provider.EnumerateEntries(dirPath));

        Assert.Collection(
            entries,
            e => Assert.EndsWith("a.txt", e),
            e => Assert.EndsWith("b.txt", e),
            e => Assert.EndsWith("sub", e)
        );
    }

    [Fact]
    public async Task EnumerateEntriesAsync_ReturnsExpected()
    {
        var dirPath = _tempDir.GetPath("enum_entries_async_dir");
        Directory.CreateDirectory(dirPath);
        File.WriteAllText(Path.Combine(dirPath, "a.txt"), "Test");
        File.WriteAllText(Path.Combine(dirPath, "b.txt"), "Test");
        Directory.CreateDirectory(Path.Combine(dirPath, "sub"));

        var entries = new List<string>();
        await foreach (var entry in _provider.EnumerateEntriesAsync(dirPath))
        {
            entries.Add(entry);
        }

        Assert.Collection(
            entries,
            e => Assert.EndsWith("a.txt", e),
            e => Assert.EndsWith("b.txt", e),
            e => Assert.EndsWith("sub", e)
        );
    }

    [Fact]
    public async Task AsyncDirectoryOperations_RespectCancellation()
    {
        var pathA = _tempDir.GetPath("cancellation_a");
        var pathB = _tempDir.GetPath("cancellation_b");
        Directory.CreateDirectory(pathA);
        Directory.CreateDirectory(pathB);

        List<Func<CancellationToken, Task>> operations = new()
        {
            token => _provider.DirectoryExistsAsync(pathA, token),
            token => _provider.CreateDirectoryAsync(pathA, token),
            token => _provider.DeleteDirectoryAsync(pathA, cancellationToken: token),
            token => _provider.MoveDirectoryAsync(pathA, pathB, cancellationToken: token),
        };
        await CancellationTestHelper.ShouldAllRespectCancellationAsync(operations);

        List<Func<CancellationToken, IAsyncEnumerable<string>>> enumerators = new()
        {
            token => _provider.EnumerateFilesAsync(pathA, cancellationToken: token),
            token => _provider.EnumerateDirectoriesAsync(pathA, cancellationToken: token),
            token => _provider.EnumerateEntriesAsync(pathA, cancellationToken: token)
        };
        await CancellationTestHelper.ShouldAllRespectCancellationAsync(enumerators);
    }
}