namespace Core.Exceptions;

[Serializable]
public class RefNotFoundException : Exception
{
    public RefNotFoundException()
    {
    }

    public RefNotFoundException(string message) : base(message)
    {
    }

    public RefNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }
}