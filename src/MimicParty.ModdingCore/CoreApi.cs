using BepInEx.Logging;
using Arribbaa.MimicParty.ModdingCore.Native;
using Arribbaa.MimicParty.ModdingCore.Runtime;

namespace Arribbaa.MimicParty.ModdingCore;

public static class CoreApi
{
    private static readonly object Gate = new();
    private static bool _initialized;

    public static ManualLogSource Log { get; private set; } = null!;
    public static GameBuildInfo Build { get; private set; } = null!;
    public static GameAssemblyImage GameAssembly { get; private set; } = null!;
    public static ModRegistry Mods { get; } = new();

    internal static void Initialize(ManualLogSource log)
    {
        lock (Gate)
        {
            if (_initialized)
                return;

            if (!OperatingSystem.IsWindows())
                throw new PlatformNotSupportedException("Mimic Party Modding Core currently supports Windows only.");

            Log = log ?? throw new ArgumentNullException(nameof(log));
            Build = GameBuildInfo.Create();
            GameAssembly = GameAssemblyImage.Capture("GameAssembly.dll");
            Mods.Register(CoreConstants.Guid, CoreConstants.Name, CoreConstants.Version);

            _initialized = true;
        }
    }

    internal static void Shutdown()
    {
        lock (Gate)
        {
            if (!_initialized)
                return;

            GameAssembly.Dispose();
            Mods.Clear();
            _initialized = false;
        }
    }

    public static PatchTransaction CreatePatchTransaction(string owner)
    {
        if (!_initialized)
            throw new InvalidOperationException("Mimic Party Modding Core is not initialized.");

        return new PatchTransaction(owner, GameAssembly, Log);
    }
}
