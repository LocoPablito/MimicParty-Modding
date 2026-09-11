using System.Buffers.Binary;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace Arribbaa.MimicParty.InteropBootstrap;

// Only the Name columns of three empty duplicate TypeDef rows are changed.
// Existing string-heap suffixes keep all tokens, offsets, signatures and IL intact.
public static class MetadataRepair
{
    public sealed record Result(byte[] Bytes, int RenamedTypes);
    public static string Sha256(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    public static Result Prepare(byte[] input, bool requireCapturedLayout = true)
    {
        ArgumentNullException.ThrowIfNull(input);
        using var pe = new PEReader(new MemoryStream(input, writable: false));
        if (!pe.HasMetadata) throw new InvalidDataException("Not a managed assembly.");
        var md = pe.GetMetadataReader();
        if (!md.IsAssembly || md.GetString(md.GetAssemblyDefinition().Name) != "UnityEngine.CoreModule")
            throw new InvalidDataException("Not the generated UnityEngine.CoreModule assembly.");
        var all = md.TypeDefinitions.ToArray();
        string Identity(TypeDefinitionHandle handle)
        {
            var t = md.GetTypeDefinition(handle);
            return MetadataTokens.GetRowNumber(t.GetDeclaringType()) + ":" + md.GetString(t.Namespace) + ":" + md.GetString(t.Name);
        }
        var groups = all.GroupBy(Identity, StringComparer.Ordinal).Where(g => g.Count() > 1).ToArray();
        if (groups.Length == 0) return new Result(input, 0);
        if (groups.Length != 2 || !groups.Any(g => g.Key == "0::<>O" && g.Count() == 3) || !groups.Any(g => g.Key == "0::<>c" && g.Count() == 2))
            throw new InvalidDataException("Unrecognized duplicate layout; no changes were made.");
        if (requireCapturedLayout && (all.Length != 4035 || md.GetTableRowCount(TableIndex.MethodDef) != 22570 || md.GetTableRowCount(TableIndex.Field) != 14625))
            throw new InvalidDataException("Unrecognized interop layout; no changes were made.");
        int indexSize = md.GetHeapSize(HeapIndex.String) > ushort.MaxValue ? 4 : 2;
        int tableOffset = checked(pe.PEHeaders.MetadataStartOffset + md.GetTableMetadataOffset(TableIndex.TypeDef));
        int rowSize = md.GetTableRowSize(TableIndex.TypeDef);
        var names = new HashSet<string>(all.Select(Identity), StringComparer.Ordinal);
        var plan = new List<(int Offset, int OldIndex, int NewIndex, int Row)>();
        foreach (var group in groups)
        {
            int suffix = 0;
            foreach (var handle in group.OrderBy(h => MetadataTokens.GetRowNumber(h)))
            {
                var t = md.GetTypeDefinition(handle);
                if (!t.GetDeclaringType().IsNil || md.GetString(t.Namespace) != "" || t.GetMethods().Count != 0 || t.GetFields().Count != 0 || t.GetProperties().Count != 0 || t.GetEvents().Count != 0 || t.GetNestedTypes().Length != 0 || t.GetGenericParameters().Count != 0)
                    throw new InvalidDataException("A duplicate is not an empty top-level helper; no changes were made.");
                if (suffix++ == 0) continue;
                int oldIndex = MetadataTokens.GetHeapOffset(t.Name);
                int newIndex = checked(oldIndex + suffix - 1);
                string expected = md.GetString(t.Name).Substring(suffix - 1);
                if (md.GetString(MetadataTokens.StringHandle(newIndex)) != expected || !names.Add("0::" + expected))
                    throw new InvalidDataException("Replacement name would collide; no changes were made.");
                int row = MetadataTokens.GetRowNumber(handle);
                int offset = checked(tableOffset + (row - 1) * rowSize + 4);
                var oldBytes = input.AsSpan(offset, indexSize);
                int stored = indexSize == 4 ? checked((int)BinaryPrimitives.ReadUInt32LittleEndian(oldBytes)) : BinaryPrimitives.ReadUInt16LittleEndian(oldBytes);
                if (stored != oldIndex) throw new InvalidDataException("Unexpected metadata offset.");
                plan.Add((offset, oldIndex, newIndex, row));
            }
        }
        if (requireCapturedLayout && !plan.Select(p => p.Row).OrderBy(x => x).SequenceEqual(new[] { 0x322, 0x3c3, 0x3e3 }))
            throw new InvalidDataException("Unexpected helper tokens; no changes were made.");
        var output = (byte[])input.Clone();
        foreach (var p in plan)
        {
            if (indexSize == 4) BinaryPrimitives.WriteUInt32LittleEndian(output.AsSpan(p.Offset, indexSize), checked((uint)p.NewIndex));
            else BinaryPrimitives.WriteUInt16LittleEndian(output.AsSpan(p.Offset, indexSize), checked((ushort)p.NewIndex));
        }
        using var checkPe = new PEReader(new MemoryStream(output, writable: false));
        var after = checkPe.GetMetadataReader();
        var afterIds = after.TypeDefinitions.Select(h => {
            var t = after.GetTypeDefinition(h);
            return MetadataTokens.GetRowNumber(t.GetDeclaringType()) + ":" + after.GetString(t.Namespace) + ":" + after.GetString(t.Name);
        }).ToArray();
        if (afterIds.Distinct(StringComparer.Ordinal).Count() != all.Length || output.Length != input.Length)
            throw new InvalidDataException("Post-repair validation failed.");
        for (int i = 0; i < input.Length; i++)
            if (input[i] != output[i] && !plan.Any(p => i >= p.Offset && i < p.Offset + indexSize))
                throw new InvalidDataException("Unexpected byte change outside a TypeDef name.");
        return new Result(output, plan.Count);
    }
    public static Result RepairFile(string path, bool requireCapturedLayout = true)
    {
        if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0) throw new IOException("Refusing a redirected target.");
        byte[] original = File.ReadAllBytes(path);
        var result = Prepare(original, requireCapturedLayout);
        if (result.RenamedTypes == 0) return result;
        string directory = Path.GetDirectoryName(Path.GetFullPath(path))!;
        string backupDir = Path.Combine(directory, ".arribbaa-backups");
        Directory.CreateDirectory(backupDir);
        string backup = Path.Combine(backupDir, "UnityEngine.CoreModule." + Sha256(original) + ".original");
        if (File.Exists(backup)) {
            if (Sha256(File.ReadAllBytes(backup)) != Sha256(original)) throw new IOException("Existing backup differs.");
        } else File.WriteAllBytes(backup, original);
        string temporary = path + ".repair-" + Guid.NewGuid().ToString("N");
        try {
            File.WriteAllBytes(temporary, result.Bytes);
            if (Sha256(File.ReadAllBytes(temporary)) != Sha256(result.Bytes) || Sha256(File.ReadAllBytes(path)) != Sha256(original))
                throw new IOException("File changed while preparing repair.");
            File.Replace(temporary, path, null);
        } finally { if (File.Exists(temporary)) File.Delete(temporary); }
        return result;
    }
}
