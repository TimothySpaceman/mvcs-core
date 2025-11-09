using System.Text;
using Core.Tests.Utils;

namespace Core.Tests.FileSystem.LocalFileSystemProvider;

public partial class LocalFileSystemProviderTests
{
    [Fact]
    public void FileExists_ReturnsTrue_WhenFileExists()
    {
        var filePath = _tempDir.GetPath("existing.txt");
        File.WriteAllText(filePath, "Test");

        Assert.True(_provider.FileExists(filePath));
    }

    [Fact]
    public void FileExists_ReturnsFalse_WhenFileIsMissing()
    {
        var filePath = _tempDir.GetPath("missing.txt");
        File.Delete(filePath);

        Assert.False(_provider.FileExists(filePath));
    }

    [Fact]
    public async Task FileExistsAsync_ReturnsTrue_WhenFileExists()
    {
        var filePath = _tempDir.GetPath("existing_async.txt");
        File.WriteAllText(filePath, "Test");

        Assert.True(await _provider.FileExistsAsync(filePath));
    }

    [Fact]
    public async Task FileExistsAsync_ReturnsFalse_WhenFileIsMissing()
    {
        var filePath = _tempDir.GetPath("missing_async.txt");
        File.Delete(filePath);

        Assert.False(await _provider.FileExistsAsync(filePath));
    }

    [Fact]
    public void CreateFile_WorksCorrectly()
    {
        var filePath = _tempDir.GetPath("new_file.txt");

        var stream = _provider.Create(filePath);
        stream.Close();

        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public async Task CreateFileAsync_WorksCorrectly()
    {
        var filePath = _tempDir.GetPath("new_file_async.txt");

        var stream = await _provider.CreateAsync(filePath);
        stream.Close();

        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void DeleteFile_WorksCorrectly()
    {
        var filePath = _tempDir.GetPath("deleted_file.txt");
        File.WriteAllText(filePath, "Test");

        _provider.DeleteFile(filePath);

        Assert.False(File.Exists(filePath));
    }

    [Fact]
    public async Task DeleteFileAsync_WorksCorrectly()
    {
        var filePath = _tempDir.GetPath("deleted_file_async.txt");
        File.WriteAllText(filePath, "Test");

        await _provider.DeleteFileAsync(filePath);

        Assert.False(File.Exists(filePath));
    }

    [Fact]
    public void OpenWrite_WorksCorrectly()
    {
        var filePath = _tempDir.GetPath("open_write_me.txt");
        var data = Encoding.UTF8.GetBytes("Test Data");

        var stream = _provider.OpenWrite(filePath);
        stream.Write(data, 0, data.Length);
        stream.Close();

        Assert.Equal(File.ReadAllBytes(filePath), data);
    }

    [Fact]
    public async Task OpenWriteAsync_WorksCorrectly()
    {
        var filePath = _tempDir.GetPath("open_write_me.txt");
        var data = Encoding.UTF8.GetBytes("Test Data");

        var stream = await _provider.OpenWriteAsync(filePath);
        stream.Write(data, 0, data.Length);
        stream.Close();

        Assert.Equal(File.ReadAllBytes(filePath), data);
    }

    [Fact]
    public void OpenRead_WorksCorrectly()
    {
        var filePath = _tempDir.GetPath("open_read_me.txt");
        var data = Encoding.UTF8.GetBytes("Test Data");
        File.WriteAllBytes(filePath, data);

        var stream = _provider.OpenRead(filePath);

        Assert.Equal(stream.ReadAllBytes(), data);
        stream.Close();
    }

    [Fact]
    public async Task OpenReadAsync_WorksCorrectly()
    {
        var filePath = _tempDir.GetPath("open_read_me.txt");
        var data = Encoding.UTF8.GetBytes("Test Data");
        File.WriteAllBytes(filePath, data);

        var stream = await _provider.OpenReadAsync(filePath);

        Assert.Equal(stream.ReadAllBytes(), data);
        stream.Close();
    }

    [Fact]
    public void WriteAllBytes_WorksCorrectly()
    {
        var filePath = _tempDir.GetPath("write_me.txt");
        var data = Encoding.UTF8.GetBytes("Test Data");

        _provider.WriteAllBytes(filePath, data);

        var result = File.ReadAllBytes(filePath);
        Assert.Equal(data, result);
    }

    [Fact]
    public async Task WriteAllBytesAsync_WorksCorrectly()
    {
        var filePath = _tempDir.GetPath("write_me_async.txt");
        var data = Encoding.UTF8.GetBytes("Test Data");

        await _provider.WriteAllBytesAsync(filePath, data);

        var result = File.ReadAllBytes(filePath);
        Assert.Equal(data, result);
    }

    [Fact]
    public void ReadAllBytes_WorksCorrectly()
    {
        var filePath = _tempDir.GetPath("read_me.txt");
        var data = Encoding.UTF8.GetBytes("Test Data");
        File.WriteAllBytes(filePath, data);

        var result = _provider.ReadAllBytes(filePath);

        Assert.Equal(data, result);
    }

    [Fact]
    public async Task ReadAllBytesAsync_WorksCorrectly()
    {
        var filePath = _tempDir.GetPath("read_me_async.txt");
        var data = Encoding.UTF8.GetBytes("Test Data");
        File.WriteAllBytes(filePath, data);

        var result = await _provider.ReadAllBytesAsync(filePath);

        Assert.Equal(data, result);
    }

    [Fact]
    public void CopyFile_WorksCorrectly()
    {
        var fromPath = _tempDir.GetPath("copy_from.txt");
        var toPath = _tempDir.GetPath("copy_to.txt");
        var data = "Test Data";
        File.WriteAllText(fromPath, data);

        _provider.CopyFile(fromPath, toPath);

        Assert.Equal(File.ReadAllText(fromPath), data);
        Assert.Equal(File.ReadAllText(toPath), data);
    }

    [Fact]
    public async Task CopyFileAsync_WorksCorrectly()
    {
        var fromPath = _tempDir.GetPath("copy_from_async.txt");
        var toPath = _tempDir.GetPath("copy_to_async.txt");
        var data = "Test Data";
        File.WriteAllText(fromPath, data);

        await _provider.CopyFileAsync(fromPath, toPath);

        Assert.Equal(File.ReadAllText(fromPath), data);
        Assert.Equal(File.ReadAllText(toPath), data);
    }

    [Fact]
    public void MoveFile_WorksCorrectly()
    {
        var fromPath = _tempDir.GetPath("move_from.txt");
        var toPath = _tempDir.GetPath("move_to.txt");
        var data = "Test Data";
        File.WriteAllText(fromPath, data);

        _provider.MoveFile(fromPath, toPath);

        Assert.False(File.Exists(fromPath));
        Assert.Equal(File.ReadAllText(toPath), data);
    }

    [Fact]
    public async Task MoveFileAsync_WorksCorrectly()
    {
        var fromPath = _tempDir.GetPath("move_from_async.txt");
        var toPath = _tempDir.GetPath("move_to_async.txt");
        var data = "Test Data";
        File.WriteAllText(fromPath, data);

        await _provider.MoveFileAsync(fromPath, toPath);

        Assert.False(File.Exists(fromPath));
        Assert.Equal(File.ReadAllText(toPath), data);
    }

    [Fact]
    public async Task AsyncFileOperations_RespectCancellation()
    {
        var pathA = _tempDir.GetPath("cancellation_a.txt");
        var pathB = _tempDir.GetPath("cancellation_b.txt");
        File.WriteAllText(pathA, "Test Data");
        File.WriteAllText(pathB, "Test Data");

        List<Func<CancellationToken, Task>> operations = new()
        {
            token => _provider.FileExistsAsync(pathA, token),
            token => _provider.WriteAllBytesAsync(pathA, [], token),
            token => _provider.ReadAllBytesAsync(pathA, token),
            token => _provider.DeleteFileAsync(pathA, token),
            token => _provider.CopyFileAsync(pathA, pathB, true, token),
            token => _provider.MoveFileAsync(pathA, pathB, true, token),
            token => _provider.OpenReadAsync(pathA, token),
            token => _provider.OpenWriteAsync(pathA, token),
            token => _provider.CreateAsync(pathA, token),
        };

        await CancellationTestHelper.ShouldAllRespectCancellationAsync(operations);
    }
}