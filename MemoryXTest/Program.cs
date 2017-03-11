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
            var address = 0x0162DB00;

            // for open our process
            myProc.GetProcessHandle(procName);

            // for write memory string value to memory
            //myProc.WriteMemory(address, "Hello");

            //// for write memory int value to memory
            //myProc.WriteMemory(address, 12345);

            //// for write memory float or single value to memory
            //myProc.WriteMemory(address, 3.1415928f);

            //// for write memory double value to memory
            //myProc.WriteMemory(address, 7.1474d);

            //// for write memory byte value to memory
            //myProc.WriteMemory(address, 0xba);

            //// for write memory array of bytes value to memory
            //myProc.WriteMemory(address, new byte[] { 0xaa, 0xbb, 0xcc });



            myProc.WriteMemory(address, new byte[] { 0xaa, 0xbb, 0xcc });




            // for read a memory address value and return to array of bytes value
            byte[] arrBytes = myProc.ReadMemory(address, 5);


            // for print byte values
            foreach (byte b in arrBytes)
                Console.WriteLine(b.ToString("X"));

            // for read a memory address value and return to double
            Console.WriteLine(myProc.ReadDouble(address));


            // for read a memory address value and return to flot or Single
            Console.WriteLine(myProc.ReadFloat(address));

            // for read memory and return to string value

            Console.WriteLine(myProc.ReadString(address, 11));


            Console.ReadLine();


        }
    }
}
