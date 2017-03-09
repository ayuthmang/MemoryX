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
        static void Main(string[] args)
        {

            MemoryX.Memory myProc = new MemoryX.Memory();

            var procName = "Tutorial-x86_64";
            var address = 0x000D1940;

            // for open our process handle
            myProc.GetProcessHandle(procName);

            // for write memory string value to memory
            myProc.WriteMemory(address, "Hello");

            // for write memory int value to memory
            myProc.WriteMemory(address, 12345);

            // for write memory float or single value to memory
            myProc.WriteMemory(address, 3.1415928f);

            // for write memory double value to memory
            myProc.WriteMemory(address, 7.1474d);

            // for write memory byte value to memory
            myProc.WriteMemory(address, 0xba);

            // for write memory array of bytes value to memory
            myProc.WriteMemory(address, new byte[] { 0xaa, 0xbb, 0xcc });

            Console.ReadLine();
       
        }
    }
}
