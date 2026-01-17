using Core.FileChanges;

namespace Core.Staging;

public record struct PathPair(string? Before, string? After)
{
    public readonly string? Before = Before;
    public readonly string? After = After;

    public static PathPair OfChange(FileChange change)
    {
        return new PathPair(change.Before?.FilePath, change.After?.FilePath);
    }
}