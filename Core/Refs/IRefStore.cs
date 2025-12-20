namespace Core.Refs;

public interface IRefStore
{
    T? Get<T>(string key, T? defaultValue = default);
    void Set<T>(string key, T value);
    bool Remove(string key);
}