using BepInEx;
using BepInEx.Unity.IL2CPP;

namespace Arribbaa.MimicParty.ModdingCore;

[BepInPlugin(CoreConstants.Guid, CoreConstants.Name, CoreConstants.Version)]
[BepInProcess(CoreConstants.ProcessName)]
public sealed class Plugin : BasePlugin
{
    public override void Load()
    {
        CoreApi.Initialize(Log);

        Log.LogInfo($"{CoreConstants.Name} v{CoreConstants.Version} by arribbaa loaded.");
        Log.LogInfo($"GameAssembly: {CoreApi.Build.GameAssemblySha256}");
        Log.LogInfo($"Metadata:     {CoreApi.Build.MetadataSha256}");
    }

    public override bool Unload()
    {
        CoreApi.Shutdown();
        return true;
    }
}
