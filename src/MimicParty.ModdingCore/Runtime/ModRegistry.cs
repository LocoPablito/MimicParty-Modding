using System.Collections.Concurrent;

namespace Arribbaa.MimicParty.ModdingCore.Runtime;

public sealed class ModRegistry
{
    private readonly ConcurrentDictionary<string, RegisteredMod> _mods =
        new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<RegisteredMod> Registered =>
        _mods.Values.OrderBy(m => m.Guid, StringComparer.OrdinalIgnoreCase).ToArray();

    public void Register(string guid, string name, string version)
    {
        if (string.IsNullOrWhiteSpace(guid))
            throw new ArgumentException("Mod GUID is empty.", nameof(guid));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Mod name is empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Mod version is empty.", nameof(version));

        _mods[guid] = new RegisteredMod(guid, name, version);
    }

    public bool IsRegistered(string guid) => _mods.ContainsKey(guid);

    public bool Unregister(string guid)
    {
        if (string.IsNullOrWhiteSpace(guid))
            return false;

        return _mods.TryRemove(guid, out _);
    }

    internal void Clear() => _mods.Clear();
}

public sealed record RegisteredMod(string Guid, string Name, string Version);
