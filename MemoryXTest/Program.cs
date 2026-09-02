using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MemoryXTest
{
    /// <summary>
    /// Self-verifying harness for <see cref="MemoryX.Memory"/>.
    ///
    /// It commits a single page inside this very process, opens that process
    /// through the library, and round-trips each read and write overload against
    /// the page. That needs no external target process and no elevation, so it
    /// runs unattended in CI.
    ///
    /// Exits with 0 when every check passes and 1 when any check fails.
    /// </summary>
    internal static class Program
    {
        private const uint MEM_COMMIT = 0x1000;
        private const uint MEM_RESERVE = 0x2000;
        private const uint PAGE_READWRITE = 0x04;

        /// <summary>Exactly one page, so reads past the end land on unmapped memory.</summary>
        private const int PageSize = 4096;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, uint flAllocationType, uint flProtect);

        private static int failures;

        private static int Main()
        {
            // Memory declares lpBaseAddress as a 64-bit long, which only matches
            // the native LPVOID parameter in a 64-bit process.
            if (IntPtr.Size != 8)
            {
                Console.Error.WriteLine("FAIL  the harness must run as a 64-bit process");
                return 1;
            }

            IntPtr page = VirtualAlloc(IntPtr.Zero, new UIntPtr(PageSize), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
            if (page == IntPtr.Zero)
            {
                Console.Error.WriteLine("FAIL  VirtualAlloc failed with error {0}", Marshal.GetLastWin32Error());
                return 1;
            }

            long baseAddress = page.ToInt64();
            var mem = new MemoryX.Memory();

            RunHandleChecks(mem);
            RunRoundTripChecks(mem, baseAddress);
            RunSingleByteWriteCheck(mem, baseAddress);
            RunEndOfPageFloatCheck(mem, baseAddress);
            RunBaseAddressChecks(mem);

            mem.CloseProcessHandle();

            Console.WriteLine();
            if (failures == 0)
            {
                Console.WriteLine("All checks passed.");
                return 0;
            }

            Console.Error.WriteLine("{0} check(s) failed.", failures);
            return 1;
        }

        private static void RunHandleChecks(MemoryX.Memory mem)
        {
            // A process id that cannot exist, so OpenProcess is never reached.
            Check("GetProcessHandle rejects an unknown process id", !mem.GetProcessHandle(-1));

            Check("GetProcessHandle opens the current process",
                mem.GetProcessHandle(Process.GetCurrentProcess().Id));
            Check("GetProcessHandle exposes a non-null handle", mem.GetProcessHandle() != IntPtr.Zero);
            Check("GetProcessID reports the current process",
                mem.GetProcessID() == Process.GetCurrentProcess().Id);
        }

        private static void RunRoundTripChecks(MemoryX.Memory mem, long baseAddress)
        {
            var bytes = new byte[] { 0xAA, 0xBB, 0xCC };
            mem.WriteMemory(baseAddress, bytes);
            Check("byte[] round-trips", SequenceEqual(mem.ReadMemory(baseAddress, bytes.Length), bytes));
            Check("GetBytesWritten reports the byte[] length", mem.GetBytesWritten() == bytes.Length);

            mem.WriteMemory(baseAddress + 16, "Hello");
            Check("string round-trips", mem.ReadString(baseAddress + 16, 5) == "Hello");

            mem.WriteMemory(baseAddress + 32, 12345);
            Check("int round-trips", mem.ReadInt32(baseAddress + 32) == 12345);

            mem.WriteMemory(baseAddress + 48, 3.1415928f);
            Check("float round-trips", mem.ReadFloat(baseAddress + 48) == 3.1415928f);
            Check("ReadSingle agrees with ReadFloat",
                mem.ReadSingle(baseAddress + 48) == mem.ReadFloat(baseAddress + 48));

            mem.WriteMemory(baseAddress + 64, 7.1474d);
            Check("double round-trips", mem.ReadDouble(baseAddress + 64) == 7.1474d);
        }

        /// <summary>
        /// Regression check: WriteMemory(long, byte) once forwarded to
        /// BitConverter.GetBytes, which has no byte overload, so the argument was
        /// promoted to short and clobbered the following byte.
        /// </summary>
        private static void RunSingleByteWriteCheck(MemoryX.Memory mem, long baseAddress)
        {
            long address = baseAddress + 80;
            mem.WriteMemory(address, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });

            mem.WriteMemory(address, (byte)0xBA);

            byte[] after = mem.ReadMemory(address, 4);
            Check("single-byte write stores the value", after[0] == 0xBA);
            Check("single-byte write leaves the next 3 bytes untouched",
                after[1] == 0xFF && after[2] == 0xFF && after[3] == 0xFF);
            Check("GetBytesWritten reports one byte", mem.GetBytesWritten() == 1);
        }

        /// <summary>
        /// Regression check: ReadSingle once requested 8 bytes for a 4-byte value,
        /// so a float at the end of a page failed to read and silently returned 0.
        /// </summary>
        private static void RunEndOfPageFloatCheck(MemoryX.Memory mem, long baseAddress)
        {
            long address = baseAddress + PageSize - sizeof(float);
            mem.WriteMemory(address, 2.71828f);
            Check("float at the end of a page reads back", mem.ReadFloat(address) == 2.71828f);
        }

        private static void RunBaseAddressChecks(MemoryX.Memory mem)
        {
            string moduleName = Process.GetCurrentProcess().MainModule.ModuleName;
            Check("GetBaseAddress resolves the main module", mem.GetBaseAddress(moduleName) != 0);
            Check("GetBaseAddress is case-insensitive",
                mem.GetBaseAddress(moduleName.ToUpperInvariant()) == mem.GetBaseAddress(moduleName));
            Check("GetBaseAddress returns 0 for an unknown module",
                mem.GetBaseAddress("no-such-module-here.dll") == 0);
        }

        private static bool SequenceEqual(byte[] actual, byte[] expected)
        {
            if (actual == null || actual.Length != expected.Length)
                return false;

            for (int i = 0; i < expected.Length; i++)
            {
                if (actual[i] != expected[i])
                    return false;
            }

            return true;
        }

        private static void Check(string description, bool passed)
        {
            if (passed)
            {
                Console.WriteLine("ok    {0}", description);
                return;
            }

            failures++;
            Console.Error.WriteLine("FAIL  {0}", description);
        }
    }
}
