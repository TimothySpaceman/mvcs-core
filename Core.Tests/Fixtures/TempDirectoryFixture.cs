namespace Core.Tests.Utils;

/// <summary>
/// Creates and cleans up a temporary directory for file system tests.
/// </summary>
public class TempDirectoryFixture : IDisposable
{
    public string RootPath { get; }

    public TempDirectoryFixture()
    {
        RootPath = Path.Combine(Path.GetTempPath(), $"fsprovider_{Guid.NewGuid():N}");
        Directory.CreateDirectory(RootPath);
    }

    public string GetPath(string relativeName) => Path.Combine(RootPath, relativeName);

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(RootPath))
                Directory.Delete(RootPath, true);
        }
        catch
        {
            /* ignore */
        }
    }
}