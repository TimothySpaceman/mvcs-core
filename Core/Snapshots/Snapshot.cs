using System.Collections.Immutable;
using Core.FileSnapshots;

namespace Core.Snapshots;

public record Snapshot(ImmutableDictionary<string, FileSnapshot> Files)
{
    public readonly ImmutableDictionary<string, FileSnapshot> Files = Files;

    public static Snapshot Empty()
    {
        var dict = new Dictionary<string, FileSnapshot>();
        return new Snapshot(dict.ToImmutableDictionary());
    }
}