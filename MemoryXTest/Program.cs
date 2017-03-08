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
            myProc.GetProcessHandle("Tutorial-i386");

            //http://stackoverflow.com/questions/3046784/byte-to-integer-in-c-sharp
            byte[] bytes = myProc.ReadMemory(0x01783A70);

            // If the system architecture is little-endian (that is, little end first),
            // reverse the byte array.
            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(bytes);
            //int i = BitConverter.ToInt32(bytes, 0);

            // test a REAL number
            float f = myProc.ReadFloat(0x01783A70);
            double d = myProc.ReadDouble(0x01783A70);
            Single s = myProc.ReadSingle(0x01783A70);
            Console.WriteLine("Single {0} , float {1}", s, f);
            Console.WriteLine("double {0}", d);

            //WriteProcessMemory 
            myProc.WriteMemory(0x01783A70, BitConverter.GetBytes(1650.0d)); // for write a double value
            myProc.WriteMemory(0x01783A70, BitConverter.GetBytes(1650.0f)); // for write a float value
            myProc.WriteMemory(0x01783A70, "Hello World"); // for write a string value
            myProc.WriteMemory(0x01783A70, new byte[] { 0xaa, 0xbb, 0xcc }); // for write a array of bytes value
            myProc.CloseProcessHandle();

       
        }
    }
}
