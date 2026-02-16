namespace Core.Storage;

public readonly record struct HashId
{
    public HashId(byte[] hashBytes)
    {
        Bytes = new ReadOnlyMemory<byte>(hashBytes.ToArray());
    }

    public static HashId Empty => new HashId(new byte[128]);

    public ReadOnlyMemory<byte> Bytes { get; init; }

    public bool IsEmpty => Bytes.IsEmpty;

    public string ToBase64String()
    {
        return Convert.ToBase64String(Bytes.Span);
    }

    public string ToHexString()
    {
        return Convert.ToHexString(Bytes.Span);
    }

    public override string ToString()
    {
        return ToHexString();
    }

    public bool Equals(HashId other)
    {
        return Bytes.Span.SequenceEqual(other.Bytes.Span);
    }

    public override int GetHashCode()
    {
        if (Bytes.IsEmpty) return 0;

        var hash = new HashCode();
        hash.AddBytes(Bytes.Span);
        return hash.ToHashCode();
    }
}