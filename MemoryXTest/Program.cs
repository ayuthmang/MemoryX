using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MemoryXTest
{
    class Program
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess,  bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, int value, int dwSize, ref int lpNumberOfBytesWritten);

        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;
        static void Main(string[] args)
        {
            MemoryX.Memory myProc = new MemoryX.Memory();
            myProc.getProcessHandle("Tutorial-x86_64");

            //Console.WriteLine(myProc.WriteInt(0x014FDAD0, 1650.0123f));
            //Console.WriteLine(myProc.WriteMemory(0x014FDAD0, Encoding.ASCII.GetBytes("HELLO WORLD!")));
            Console.WriteLine(myProc.WriteMemory(0x014FDAD0, "Good bye World"));
            myProc.closeHandle();
            Console.Read();

       
        }
    }
}
