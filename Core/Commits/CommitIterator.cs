using System.Collections;
using Core.Exceptions;
using Core.Storage;

namespace Core.Commits;

public class CommitIterator : IEnumerable<Commit>
{
    private readonly ICommitStore _store;
    private readonly HashId _startId;
    private readonly HashId? _stopId;

    public CommitIterator(ICommitStore store, HashId startId, HashId? stopId = null)
    {
        _store = store;
        _startId = startId;
        _stopId = stopId;
    }

    public IEnumerator<Commit> GetEnumerator()
    {
        var currentId = _startId;

        while (!currentId.IsEmpty)
        {
            var commit = _store.Get(currentId);

            if (commit == null)
                throw new CommitNotFoundException($"Commit {currentId} not found");

            yield return commit;
            if (currentId == _stopId || commit.ParentId == null) break;

            currentId = (HashId)commit.ParentId;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}