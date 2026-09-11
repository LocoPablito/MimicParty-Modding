using BepInEx.Logging;

namespace Arribbaa.MimicParty.ModdingCore.Native;

public sealed class PatchTransaction : IDisposable
{
    private static readonly object GlobalPatchGate = new();

    private readonly string _owner;
    private readonly GameAssemblyImage _image;
    private readonly ManualLogSource _log;
    private readonly List<PatternPatchRequest> _requests = new();
    private readonly List<RuntimePatchHandle> _handles = new();

    private bool _applied;
    private bool _disposed;

    internal PatchTransaction(string owner, GameAssemblyImage image, ManualLogSource log)
    {
        _owner = string.IsNullOrWhiteSpace(owner) ? "unknown" : owner;
        _image = image;
        _log = log;
    }

    public PatchTransaction Add(
        string name,
        string signature,
        int patchOffset,
        byte[] expected,
        byte[] replacement)
    {
        ThrowIfDisposed();

        if (_applied)
            throw new InvalidOperationException("Cannot add patches after the transaction has been applied.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Patch name is empty.", nameof(name));

        if (patchOffset < 0)
            throw new ArgumentOutOfRangeException(nameof(patchOffset));

        ArgumentNullException.ThrowIfNull(expected);
        ArgumentNullException.ThrowIfNull(replacement);

        if (expected.Length == 0 || expected.Length != replacement.Length)
            throw new ArgumentException("Expected and replacement bytes must have the same non-zero length.");

        _requests.Add(new PatternPatchRequest(
            name,
            signature,
            patchOffset,
            expected.ToArray(),
            replacement.ToArray()));

        return this;
    }

    public IReadOnlyList<RuntimePatchHandle> Apply()
    {
        ThrowIfDisposed();

        if (_applied)
            return _handles;

        lock (GlobalPatchGate)
        {
            var resolved = new List<ResolvedPatch>(_requests.Count);

            foreach (PatternPatchRequest request in _requests)
            {
                GameAssemblyImage.PatternMatch match = _image.FindUnique(request.Signature);
                IntPtr target = IntPtr.Add(match.Address, request.PatchOffset);
                byte[] current = RuntimeMemory.Read(target, request.Expected.Length);

                if (!current.SequenceEqual(request.Expected))
                {
                    throw new InvalidOperationException(
                        $"Patch '{request.Name}' resolved at RVA 0x{match.Rva:X}, but the expected bytes do not match.");
                }

                resolved.Add(new ResolvedPatch(request, match, target, current));
            }

            try
            {
                foreach (ResolvedPatch patch in resolved)
                {
                    try
                    {
                        RuntimeMemory.Write(patch.Target, patch.Request.Replacement);
                    }
                    catch
                    {
                        try
                        {
                            byte[] current = RuntimeMemory.Read(patch.Target, patch.Request.Replacement.Length);
                            if (current.SequenceEqual(patch.Request.Replacement))
                                RuntimeMemory.Write(patch.Target, patch.Original);
                        }
                        catch (Exception restoreError)
                        {
                            _log.LogError(
                                $"[{_owner}] Failed to roll back partially-applied patch '{patch.Request.Name}': {restoreError}");
                        }

                        throw;
                    }

                    var handle = new RuntimePatchHandle(
                        patch.Request.Name,
                        patch.Match.Rva + checked((uint)patch.Request.PatchOffset),
                        patch.Target,
                        patch.Original,
                        patch.Request.Replacement);

                    _handles.Add(handle);
                    _log.LogInfo(
                        $"[{_owner}] Applied runtime patch '{handle.Name}' at RVA 0x{handle.Rva:X}.");
                }

                _applied = true;
                return _handles;
            }
            catch
            {
                for (int i = _handles.Count - 1; i >= 0; i--)
                {
                    try { _handles[i].Restore(); }
                    catch (Exception restoreError)
                    {
                        _log.LogError($"[{_owner}] Failed to roll back '{_handles[i].Name}': {restoreError}");
                    }
                }

                _handles.Clear();
                throw;
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        lock (GlobalPatchGate)
        {
            for (int i = _handles.Count - 1; i >= 0; i--)
            {
                try { _handles[i].Restore(); }
                catch (Exception ex)
                {
                    _log.LogError($"[{_owner}] Failed to restore '{_handles[i].Name}' during unload: {ex}");
                }
            }

            _handles.Clear();
            _disposed = true;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(PatchTransaction));
    }

    private sealed record PatternPatchRequest(
        string Name,
        string Signature,
        int PatchOffset,
        byte[] Expected,
        byte[] Replacement);

    private sealed record ResolvedPatch(
        PatternPatchRequest Request,
        GameAssemblyImage.PatternMatch Match,
        IntPtr Target,
        byte[] Original);
}

public sealed class RuntimePatchHandle
{
    private bool _restored;

    internal RuntimePatchHandle(
        string name,
        uint rva,
        IntPtr address,
        byte[] original,
        byte[] replacement)
    {
        Name = name;
        Rva = rva;
        Address = address;
        Original = original.ToArray();
        Replacement = replacement.ToArray();
    }

    public string Name { get; }
    public uint Rva { get; }
    public IntPtr Address { get; }
    public byte[] Original { get; }
    public byte[] Replacement { get; }
    public bool IsRestored => _restored;

    public void Restore()
    {
        if (_restored)
            return;

        byte[] current = RuntimeMemory.Read(Address, Replacement.Length);

        if (!current.SequenceEqual(Replacement))
        {
            throw new InvalidOperationException(
                $"Cannot restore '{Name}' because the runtime bytes were changed after this patch was applied.");
        }

        RuntimeMemory.Write(Address, Original);
        _restored = true;
    }
}
