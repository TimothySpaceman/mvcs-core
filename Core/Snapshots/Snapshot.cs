using System.Collections.Immutable;
using Core.FileSnapshots;

namespace Core.Snapshots;

public record Snapshot(ImmutableDictionary<string, FileSnapshot> Files)
{
    public readonly ImmutableDictionary<string, FileSnapshot> Files = Files;

    public static Snapshot Empty() => new(ImmutableDictionary<string, FileSnapshot>.Empty);

    public Snapshot WithoutFiles(Func<string, bool> predicate) => new(
        Files.Where(kvp => !predicate(kvp.Key)).ToImmutableDictionary()
    );
}