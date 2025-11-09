namespace Core.Tests.Utils;

public static class StreamExtensions
{
    /// <summary>
    /// Reads all bytes from the stream.
    /// </summary>
    /// <returns>A byte array containing the contents of this stream.</returns>
    public static byte[] ReadAllBytes(this Stream stream)
    {
        using (var ms = new MemoryStream())
        {
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}