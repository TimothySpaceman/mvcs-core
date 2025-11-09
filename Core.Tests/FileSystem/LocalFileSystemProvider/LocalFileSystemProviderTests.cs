using Core.Tests.Utils;

namespace Core.Tests.FileSystem.LocalFileSystemProvider;

[Collection("LocalFileSystemProviderTests")]
public partial class LocalFileSystemProviderTests : IClassFixture<TempDirectoryFixture>
{
    private readonly TempDirectoryFixture _tempDir;
    private readonly Core.FileSystem.LocalFileSystemProvider _provider;

    public LocalFileSystemProviderTests(TempDirectoryFixture tempDirectoryFixture)
    {
        _tempDir = tempDirectoryFixture;
        _provider = new Core.FileSystem.LocalFileSystemProvider();
    }
}