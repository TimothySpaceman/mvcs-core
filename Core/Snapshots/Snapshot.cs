using System.Collections.Immutable;
using Core.FileSnapshots;

namespace Core.Snapshots;

public record Snapshot(ImmutableDictionary<string, FileSnapshot> Files)
{
    public readonly ImmutableDictionary<string, FileSnapshot> Files = Files;
}