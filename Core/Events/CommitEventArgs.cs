using Core.Commits;

namespace Core.Events;

public class CommitEventArgs : EventArgs
{
    public Commit Commit { get; set; }
    public DateTimeOffset TimeStamp { get; set; }

    public CommitEventArgs(Commit commit)
    {
        Commit = commit;
        TimeStamp = DateTimeOffset.Now;
    }
}