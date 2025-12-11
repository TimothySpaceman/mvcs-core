namespace Core.Config;

public class ConfigService : IConfigService
{
    private readonly Dictionary<string, string?> _config = new();

    public T? Get<T>(string key, T? defaultValue = default)
    {
        if (!_config.TryGetValue(key, out var valueStr))
        {
            return defaultValue;
        }

        try
        {
            return (T)Convert.ChangeType(valueStr, typeof(T))!;
        }
        catch
        {
            return defaultValue;
        }
    }

    public string? Get(string key, string? defaultValue = null)
    {
        return Get<string>(key, defaultValue);
    }

    public void Set<T>(string key, T? value)
    {
        if (value == null)
        {
            _config.Remove(key);
        }
        else
        {
            _config[key] = value.ToString();
        }
    }

    public void Set(string key, string? value)
    {
        Set<string>(key, value);
    }
}