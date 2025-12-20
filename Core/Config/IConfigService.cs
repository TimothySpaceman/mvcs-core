namespace Core.Config;

public interface IConfigService
{
    T? Get<T>(string key, T? defaultValue = default);
    string? Get(string key, string? defaultValue = null);

    void Set<T>(string key, T value);
    void Set(string key, string value);
    
    bool Remove(string key);
}