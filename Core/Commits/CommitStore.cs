using System.Buffers.Binary;
using System.Collections.Immutable;
using System.IO.Hashing;
using System.Text;
using Core.FileChanges;
using Core.Storage;

namespace Core.Commits;

public class CommitStore : ICommitStore
{
    private readonly Dictionary<HashId, Commit> _commits = new();

    public bool Has(HashId id)
    {
        return _commits.ContainsKey(id);
    }
    
    public Commit? Get(HashId id)
    {
        return _commits.GetValueOrDefault(id);
    }

    public void Add(Commit commit)
    {
        _commits.Add(commit.Id, commit);
    }

    public bool Remove(HashId id)
    {
        return _commits.Remove(id);
    }
}