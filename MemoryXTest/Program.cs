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
            myProc.getProcessHandle("Tutorial-i386");

            //Console.WriteLine(myProc.getProcessHandle().ToString() + " " + myProc.getProcessID().ToString());

            //myProc.WriteMemory(0x01783A70, BitConverter.GetBytes(1000));


            //http://stackoverflow.com/questions/3046784/byte-to-integer-in-c-sharp
            byte[] bytes = myProc.ReadMemory(0x01783A70);

            // If the system architecture is little-endian (that is, little end first),
            // reverse the byte array.
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            int i = BitConverter.ToInt32(bytes, 0);


            Console.WriteLine(myProc.ReadInt32(0x01783A70));


            myProc.closeHandle();
            Console.Read();

       
        }
    }
}
