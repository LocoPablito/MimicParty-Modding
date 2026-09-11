using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Arribbaa.MimicParty.ModdingCore.Native;

public sealed class GameAssemblyImage : IDisposable
{
    private const uint ImageScnMemExecute = 0x20000000;
    private readonly List<ExecutableSection> _sections;

    private GameAssemblyImage(IntPtr moduleBase, int moduleSize, List<ExecutableSection> sections)
    {
        ModuleBase = moduleBase;
        ModuleSize = moduleSize;
        _sections = sections;
    }

    public IntPtr ModuleBase { get; }
    public int ModuleSize { get; }
    public IReadOnlyList<ExecutableSection> Sections => _sections;

    public static GameAssemblyImage Capture(string moduleName)
    {
        using Process process = Process.GetCurrentProcess();

        ProcessModule? module = null;
        foreach (ProcessModule candidate in process.Modules)
        {
            if (string.Equals(candidate.ModuleName, moduleName, StringComparison.OrdinalIgnoreCase))
            {
                module = candidate;
                break;
            }
        }

        if (module is null)
            throw new InvalidOperationException($"Loaded module '{moduleName}' was not found.");

        IntPtr imageBase = module.BaseAddress;
        int imageSize = module.ModuleMemorySize;

        int peOffset = Marshal.ReadInt32(imageBase, 0x3C);
        if (Marshal.ReadInt32(imageBase, peOffset) != 0x00004550)
            throw new InvalidDataException("GameAssembly has an invalid PE signature.");

        ushort sectionCount = unchecked((ushort)Marshal.ReadInt16(imageBase, peOffset + 6));
        ushort optionalHeaderSize = unchecked((ushort)Marshal.ReadInt16(imageBase, peOffset + 20));
        int sectionTable = peOffset + 24 + optionalHeaderSize;

        var sections = new List<ExecutableSection>();

        for (int i = 0; i < sectionCount; i++)
        {
            int header = sectionTable + (i * 40);

            var nameBytes = new byte[8];
            Marshal.Copy(IntPtr.Add(imageBase, header), nameBytes, 0, nameBytes.Length);
            string name = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0');

            uint virtualSize = unchecked((uint)Marshal.ReadInt32(imageBase, header + 8));
            uint virtualAddress = unchecked((uint)Marshal.ReadInt32(imageBase, header + 12));
            uint rawSize = unchecked((uint)Marshal.ReadInt32(imageBase, header + 16));
            uint characteristics = unchecked((uint)Marshal.ReadInt32(imageBase, header + 36));

            if ((characteristics & ImageScnMemExecute) == 0)
                continue;

            long requested = Math.Max((long)virtualSize, rawSize);
            long remaining = imageSize - (long)virtualAddress;
            int length = checked((int)Math.Max(0, Math.Min(requested, remaining)));
            if (length <= 0)
                continue;

            IntPtr address = IntPtr.Add(imageBase, checked((int)virtualAddress));
            var bytes = new byte[length];
            Marshal.Copy(address, bytes, 0, length);

            sections.Add(new ExecutableSection(name, virtualAddress, address, bytes));
        }

        if (sections.Count == 0)
            throw new InvalidDataException("GameAssembly contains no readable executable PE sections.");

        return new GameAssemblyImage(imageBase, imageSize, sections);
    }

    public IReadOnlyList<PatternMatch> FindAll(MemoryPattern pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);

        var results = new List<PatternMatch>();

        foreach (ExecutableSection section in _sections)
        {
            byte[] data = section.Snapshot;
            int lastStart = data.Length - pattern.Length;
            if (lastStart < 0)
                continue;

            int anchor = pattern.AnchorIndex;
            byte anchorValue = pattern.Values[anchor];

            int searchIndex = anchor;
            while (searchIndex < data.Length)
            {
                int anchorHit = Array.IndexOf(data, anchorValue, searchIndex);
                if (anchorHit < 0)
                    break;

                int offset = anchorHit - anchor;
                searchIndex = anchorHit + 1;

                if (offset < 0 || offset > lastStart)
                    continue;

                bool match = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (pattern.Exact[j] && data[offset + j] != pattern.Values[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (!match)
                    continue;

                results.Add(new PatternMatch(
                    section.Name,
                    checked(section.VirtualAddress + (uint)offset),
                    IntPtr.Add(section.Address, offset)));
            }
        }

        return results;
    }

    public PatternMatch FindUnique(string signature)
    {
        MemoryPattern pattern = MemoryPattern.Parse(signature);
        IReadOnlyList<PatternMatch> matches = FindAll(pattern);

        if (matches.Count == 0)
            throw new PatternResolutionException($"Pattern was not found: {signature}");

        if (matches.Count != 1)
        {
            string locations = string.Join(", ", matches.Select(m => $"{m.SectionName}+0x{m.Rva:X}"));
            throw new PatternResolutionException(
                $"Pattern is not unique ({matches.Count} matches): {signature}. Matches: {locations}");
        }

        return matches[0];
    }

    public void Dispose()
    {
        foreach (ExecutableSection section in _sections)
            Array.Clear(section.Snapshot, 0, section.Snapshot.Length);

        _sections.Clear();
    }

    public sealed record ExecutableSection(string Name, uint VirtualAddress, IntPtr Address, byte[] Snapshot);
    public sealed record PatternMatch(string SectionName, uint Rva, IntPtr Address);
}

public sealed class PatternResolutionException : Exception
{
    public PatternResolutionException(string message) : base(message) { }
}
