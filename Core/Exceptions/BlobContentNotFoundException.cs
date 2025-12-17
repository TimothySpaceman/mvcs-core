namespace Core.Exceptions;

[Serializable]
public class BlobContentNotFoundException : Exception
{
    public BlobContentNotFoundException()
    {
    }

    public BlobContentNotFoundException(string message) : base(message)
    {
    }

    public BlobContentNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }
}