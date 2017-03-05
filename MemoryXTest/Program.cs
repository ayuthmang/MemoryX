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
        static extern bool WriteProcessMemory(int hProcess, long lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;
        static void Main(string[] args)
        {
            MemoryX.Memory.WriteMemoryInt();
            Console.ReadLine();
            Console.Read();
            //Process process = Process.GetProcessesByName("notepad")[0];
            //IntPtr processHandle = OpenProcess(0x1F0FFF, false, process.Id);

            //int bytesWritten = 0;
            //byte[] buffer = Encoding.Unicode.GetBytes("อะไรของมึง\0");
            //// '\0' marks the end of string
            ////00000175DF9B5C70  47 00 6F 00 6F 00 64 00 20 00 42 00 79 00 65 00  G.o.o.d. .B.y.e.  

            //// replace 0x0046A3B8 with your address
            //WriteProcessMemory((int)processHandle, 0x00000175DF9F20B0, buffer, buffer.Length, ref bytesWritten);

            //Console.ReadLine();
        
        }
    }
}
