using System.Diagnostics;
using System.Security.Cryptography;

namespace Arribbaa.MimicParty.ModdingCore;

public sealed record GameBuildInfo(
    string GameRoot,
    string GameVersion,
    string GameAssemblyPath,
    string GameAssemblySha256,
    string MetadataPath,
    string MetadataSha256)
{
    public static GameBuildInfo Create()
    {
        using var process = Process.GetCurrentProcess();
        string? exePath = process.MainModule?.FileName;
        if (string.IsNullOrWhiteSpace(exePath))
            throw new InvalidOperationException("Unable to resolve the Mimic Party executable path.");

        string root = Path.GetDirectoryName(exePath)
                      ?? throw new InvalidOperationException("Unable to resolve the Mimic Party game directory.");

        string assemblyPath = Path.Combine(root, "GameAssembly.dll");
        string metadataPath = Path.Combine(root, "Mimic Party_Data", "il2cpp_data", "Metadata", "global-metadata.dat");

        if (!File.Exists(assemblyPath))
            throw new FileNotFoundException("GameAssembly.dll was not found.", assemblyPath);

        if (!File.Exists(metadataPath))
            throw new FileNotFoundException("global-metadata.dat was not found.", metadataPath);

        string version = FileVersionInfo.GetVersionInfo(exePath).FileVersion ?? "unknown";

        return new GameBuildInfo(
            root,
            version,
            assemblyPath,
            Sha256(assemblyPath),
            metadataPath,
            Sha256(metadataPath));
    }

    private static string Sha256(string path)
    {
        using var stream = File.OpenRead(path);
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(stream)).ToLowerInvariant();
    }
}
