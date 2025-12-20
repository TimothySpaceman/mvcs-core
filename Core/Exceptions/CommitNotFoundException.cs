namespace Core.Exceptions;

[Serializable]
public class CommitNotFoundException : Exception
{
    public CommitNotFoundException()
    {
    }

    public CommitNotFoundException(string message) : base(message)
    {
    }

    public CommitNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }
}