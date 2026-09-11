using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Arribbaa.MimicParty.ModdingCore.Native;

internal static class RuntimeMemory
{
    private const uint PageExecuteReadWrite = 0x40;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualProtect(
        IntPtr lpAddress,
        UIntPtr dwSize,
        uint flNewProtect,
        out uint lpflOldProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FlushInstructionCache(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        UIntPtr dwSize);

    public static byte[] Read(IntPtr address, int count)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        var bytes = new byte[count];
        if (count > 0)
            Marshal.Copy(address, bytes, 0, count);

        return bytes;
    }

    public static void Write(IntPtr address, ReadOnlySpan<byte> replacement)
    {
        if (replacement.Length == 0)
            return;

        if (!VirtualProtect(address, (UIntPtr)replacement.Length, PageExecuteReadWrite, out uint oldProtection))
            throw new Win32Exception(Marshal.GetLastWin32Error(), "VirtualProtect failed before patching.");

        try
        {
            byte[] buffer = replacement.ToArray();
            Marshal.Copy(buffer, 0, address, buffer.Length);

            using Process process = Process.GetCurrentProcess();
            if (!FlushInstructionCache(process.Handle, address, (UIntPtr)buffer.Length))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "FlushInstructionCache failed.");
        }
        finally
        {
            if (!VirtualProtect(address, (UIntPtr)replacement.Length, oldProtection, out _))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "VirtualProtect failed while restoring page protection.");
        }
    }
}
