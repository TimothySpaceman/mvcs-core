using System.Runtime.CompilerServices;
using Core.Blobs;
using Core.Config;

namespace Core.WorkingDirectories;

public class LocalWorkingDirectoryBuilder : IWorkingDirectoryBuilder
{
    private string? _rootPath;
    private readonly IConfigService _configService;
    private readonly IBlobStorageBackend _blobStorageBackend;

    public LocalWorkingDirectoryBuilder(IConfigService configService, IBlobStorageBackend blobStorageBackend)
    {
        _configService = configService;
        _blobStorageBackend = blobStorageBackend;
    }

    public LocalWorkingDirectoryBuilder Reset()
    {
        _rootPath = null;
        return this;
    }

    public LocalWorkingDirectoryBuilder AddRootPath(string path)
    {
        _rootPath = path;
        return this;
    }

    public IWorkingDirectory GetWorkingDirectory()
    {
        return new LocalWorkingDirectory(_rootPath ?? ".", _configService, _blobStorageBackend);
    }
}