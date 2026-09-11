using Arribbaa.MimicParty.ModdingCore;
using BepInEx;
using BepInEx.Unity.IL2CPP;

namespace ExampleMimicPartyMod;

// Replace the GUID, title, namespace and assembly name before distributing your mod.
[BepInPlugin(ModGuid, "Example Mimic Party Mod", "1.0.0")]
[BepInProcess("Mimic Party.exe")]
[BepInDependency("com.arribbaa.mimicparty.moddingcore", ">=1.0.0")]
public sealed class Plugin : BasePlugin
{
    public const string ModGuid = "com.example.mimicparty.starter";

    public override void Load()
    {
        CoreApi.Mods.Register(ModGuid, "Example Mimic Party Mod", "1.0.0");
        var logFingerprint = Config.Bind("Diagnostics", "LogBuildFingerprint", true,
            "Print the installed game fingerprints. This does not prove your mod is compatible.");
        Log.LogInfo("Starter loaded with Mimic Party Modding Core. No gameplay changes applied.");
        if (logFingerprint.Value)
        {
            Log.LogInfo($"GameAssembly SHA256: {CoreApi.Build.GameAssemblySha256}");
            Log.LogInfo($"Metadata SHA256: {CoreApi.Build.MetadataSha256}");
        }
        // Add your independently validated behaviour here. Do not copy native
        // offsets or signatures from another game build without verification.
    }

    public override bool Unload()
    {
        // Dispose any patch transactions and unpatch your own Harmony ID here.
        CoreApi.Mods.Unregister(ModGuid);
        return true;
    }
}
