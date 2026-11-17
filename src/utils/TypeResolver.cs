public class TypesResolver
{
    // get array of types T defined in current assembly
    public static Type[] ResolveAll<T>(bool requiredParameterlessCtor = true)
    {
        Type[] types = typeof(T).Assembly.GetTypes()
        .Where(t => typeof(T).IsAssignableFrom(t)
             && !t.IsAbstract
             && !t.IsInterface
             && (!requiredParameterlessCtor || t.GetConstructor(Type.EmptyTypes) != null))
        .ToArray();
        return types;
    }
    public static Type ResolveByName<T>(string name)
    {
        var types = ResolveAll<T>();
        return types.FirstOrDefault(t => t.Name == name)
            ?? throw new InvalidOperationException(
                $"Type '{name}' implementing {typeof(T).Name} not found. " +
                $"Available: {string.Join(", ", types.Select(t => t.Name))}");
    }
    public static Type ResolveRandomly<T>()
    {
        Type[] types = ResolveAll<T>();
        if (types.Length == 0)
            throw new InvalidOperationException($"No implementations of {typeof(T).Name} found.");
        return types[Random.Shared.Next(types.Length)];
    }
}