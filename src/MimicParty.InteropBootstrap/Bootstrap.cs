using BepInEx;
using BepInEx.Configuration;
using BepInEx.Preloader.Core.Patching;

namespace Arribbaa.MimicParty.InteropBootstrap;

[PatcherPluginInfo("com.arribbaa.mimicparty.interopbootstrap", "Mimic Party Interop Bootstrap", "1.0.0")]
public sealed class Bootstrap : BasePatcher
{
    public const string GameHash = "44bbc82bdae73c1c86559a1f091ee9c7a3ae510a02c2d83f16b686ecdd9c8b11";
    public const string MetadataHash = "1586b9dd69e488706671d35490cc16377a612ca3af521031ac44464929b21e94";
    // In #788 constructors run after interop generation and BEFORE
    // LoadAssemblyDirectories opens generated DLLs. Initialize is too late for
    // transactional disk replacement on Windows. This ordering is intentional.
    public Bootstrap()
    {
        try
        {
            if (!string.Equals(Paths.ProcessName, "Mimic Party", StringComparison.OrdinalIgnoreCase)) return;
            string game = Path.Combine(Paths.GameRootPath, "GameAssembly.dll");
            string metadata = Path.Combine(Paths.GameRootPath, "Mimic Party_Data", "il2cpp_data", "Metadata", "global-metadata.dat");
            if (!File.Exists(game) || !File.Exists(metadata) || MetadataRepair.Sha256(File.ReadAllBytes(game)) != GameHash || MetadataRepair.Sha256(File.ReadAllBytes(metadata)) != MetadataHash)
            {
                Log.LogWarning("INTEROP BOOTSTRAP: unsupported game fingerprint; no files were changed. Check the supported-build notes.");
                return;
            }
            string interop = Path.GetFullPath(Path.Combine(Paths.BepInExRootPath, "interop"));
            string configured = Environment.GetEnvironmentVariable("IL2CPP_INTEROP_DATABASES_LOCATION") ?? interop;
            if (!string.Equals(interop.TrimEnd(Path.DirectorySeparatorChar), Path.GetFullPath(configured).TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Custom interop directories require manual review.");
            if (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "UnityEngine.CoreModule"))
                throw new InvalidOperationException("UnityEngine.CoreModule was loaded before the compatibility bootstrap.");
            string target = Path.Combine(interop, "UnityEngine.CoreModule.dll");
            if (!File.Exists(target)) throw new FileNotFoundException("Interop generation did not produce CoreModule.", target);
            var result = MetadataRepair.RepairFile(target);
            RemoveKnownLegacyCopy();
            ConfigFile.CoreConfig.Bind("Logging", "UnityLogListening", true).Value = false;
            Log.LogInfo($"INTEROP COMPATIBILITY READY: renamed {result.RenamedTypes} duplicate empty helpers; SHA256={MetadataRepair.Sha256(result.Bytes)}. No game binary was modified.");
        }
        catch (Exception error) { Log.LogError("INTEROP COMPATIBILITY FAILED: " + error); }
    }
    private static void RemoveKnownLegacyCopy()
    {
        string target = Path.Combine(Paths.BepInExAssemblyDirectory, "UnityEngine.CoreModule.dll");
        if (!File.Exists(target)) return;
        var bytes = File.ReadAllBytes(target);
        string hash = MetadataRepair.Sha256(bytes);
        if (hash != "d9a5bd86b75ac44a188e41ef67cdf2834714417a44444e33b159715ae880e7f6" && hash != "fac42323e3313920dacca0db6120804573e085fc65990842e90b7d7d491fff39" && hash != "e379f3241c91cd533f5dd5ee9cd0df9d99628cfa561163ad180dfe9015321b9e")
            throw new InvalidOperationException("An unrecognized CoreModule copy exists in BepInEx/core; it was not removed.");
        string backupDir = Path.Combine(Paths.BepInExAssemblyDirectory, ".arribbaa-backups");
        Directory.CreateDirectory(backupDir);
        string backup = Path.Combine(backupDir, "UnityEngine.CoreModule." + hash + ".original");
        if (File.Exists(backup)) {
            if (MetadataRepair.Sha256(File.ReadAllBytes(backup)) != hash) throw new IOException("Legacy backup conflict.");
        } else File.WriteAllBytes(backup, bytes);
        File.Delete(target);
    }
}
