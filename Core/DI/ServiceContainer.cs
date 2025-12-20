using System.Reflection;

namespace Core.DI;

public class ServiceContainer
{
    private readonly Dictionary<Type, Type> _types = new();
    private readonly Dictionary<Type, object> _instances = new();

    public void Use<TInterface, TImplementation>() where TImplementation : TInterface
    {
        _types[typeof(TInterface)] = typeof(TImplementation);
    }

    public void Use<TInterface>(TInterface instance)
    {
        _instances[typeof(TInterface)] = instance!;
    }

    private void CacheInstance(Type type, object instance)
    {
        _instances[type] = instance!;
    }

    public T Resolve<T>()
    {
        return (T)Resolve(typeof(T));
    }

    private object Resolve(Type type)
    {
        var implementationType = _types.GetValueOrDefault(type, type);
        if (_instances.TryGetValue(implementationType, out var existing))
        {
            return existing;
        }

        var constructor = implementationType.GetConstructors().FirstOrDefault();
        if (constructor == null)
        {
            throw new InvalidOperationException($"No public constructor found for {implementationType.Name}");
        }

        var parameters = constructor.GetParameters();
        var dependencyInstances = new List<object>();
        foreach (var param in parameters)
        {
            dependencyInstances.Add(Resolve(param.ParameterType));
        }

        var instance = Activator.CreateInstance(implementationType, dependencyInstances.ToArray())!;
        CacheInstance(implementationType, instance);
        return instance;
    }
}