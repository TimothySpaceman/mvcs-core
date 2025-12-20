using System.Collections.Immutable;
using Core.FileChanges;
using Core.Storage;

namespace Core.Commits;

public interface ICommitStore
{
    public bool Has(HashId id);
    public Commit? Get(HashId id);
    public void Add(Commit commit);
    public bool Remove(HashId id);
}