using System.Reflection;

namespace Arribbaa.MimicParty.ModdingCore.Runtime;

public static class ReflectionResolver
{
    public static Type FindUniqueType(string shortName, params string[] requiredMethodNames)
    {
        IReadOnlyList<Type> matches = FindTypes(shortName, requiredMethodNames);

        if (matches.Count == 0)
            throw new TypeLoadException($"Unable to find runtime type '{shortName}'.");

        if (matches.Count != 1)
        {
            string names = string.Join(", ", matches.Select(t => t.FullName ?? t.Name));
            throw new TypeLoadException($"Runtime type '{shortName}' is ambiguous: {names}");
        }

        return matches[0];
    }

    public static Type? TryFindUniqueType(string shortName, params string[] requiredMethodNames)
    {
        IReadOnlyList<Type> matches = FindTypes(shortName, requiredMethodNames);
        return matches.Count == 1 ? matches[0] : null;
    }

    public static MethodInfo FindUniqueMethod(Type type, string methodName, int? parameterCount = null)
    {
        ArgumentNullException.ThrowIfNull(type);

        MethodInfo[] methods = type
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => string.Equals(m.Name, methodName, StringComparison.Ordinal))
            .Where(m => parameterCount is null || m.GetParameters().Length == parameterCount.Value)
            .ToArray();

        if (methods.Length == 0)
            throw new MissingMethodException(type.FullName, methodName);

        if (methods.Length != 1)
            throw new AmbiguousMatchException(
                $"Method '{type.FullName}.{methodName}' has {methods.Length} matching overloads.");

        return methods[0];
    }

    private static IReadOnlyList<Type> FindTypes(string shortName, string[] requiredMethodNames)
    {
        if (string.IsNullOrWhiteSpace(shortName))
            throw new ArgumentException("Type name is empty.", nameof(shortName));

        var result = new List<Type>();

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in SafeGetTypes(assembly))
            {
                if (!string.Equals(type.Name, shortName, StringComparison.Ordinal))
                    continue;

                if (requiredMethodNames.Length != 0)
                {
                    var methodNames = type
                        .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        .Select(m => m.Name)
                        .ToHashSet(StringComparer.Ordinal);

                    if (requiredMethodNames.Any(required => !methodNames.Contains(required)))
                        continue;
                }

                result.Add(type);
            }
        }

        return result;
    }

    private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null).Cast<Type>();
        }
        catch
        {
            return Array.Empty<Type>();
        }
    }
}
