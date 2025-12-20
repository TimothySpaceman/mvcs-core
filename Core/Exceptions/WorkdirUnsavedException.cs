namespace Core.Exceptions;

[Serializable]
public class WorkdirUnsavedException : Exception
{
    public WorkdirUnsavedException()
    {
    }

    public WorkdirUnsavedException(string message) : base(message)
    {
    }

    public WorkdirUnsavedException(string message, Exception inner) : base(message, inner)
    {
    }
}