namespace Core.Refs;

public class RefStore : IRefStore
{
    private readonly Dictionary<string, object?> _config = new();

    public T? Get<T>(string key, T? defaultValue = default)
    {
        if (!_config.TryGetValue(key, out var value))
        {
            return defaultValue;
        }

        try
        {
            return (T)value!;
        }
        catch
        {
            return defaultValue;
        }
    }

    public void Set<T>(string key, T value)
    {
        _config[key] = value;
    }

    public bool Remove(string key)
    {
        return _config.Remove(key);
    }
}