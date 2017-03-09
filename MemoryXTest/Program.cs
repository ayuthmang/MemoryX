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
            myProc.GetProcessHandle(procName);

            //http://stackoverflow.com/questions/3046784/byte-to-integer-in-c-sharp

            // If the system architecture is little-endian (that is, little end first),
            // reverse the byte array.
            //if (BitConverter.IsLittleEndian)
            //    Array.Reverse(bytes);
            //int i = BitConverter.ToInt32(bytes, 0);

            // test a REAL number
            float f = myProc.ReadFloat(address);
            double d = myProc.ReadDouble(address);
            Single s = myProc.ReadSingle(address);
            byte[] bs = myProc.ReadMemory(address, 4);
            Console.WriteLine("Single {0} , float {1}", s, f);
            Console.WriteLine("double {0}", d);
            Console.Write("Array of Bytes ");
            foreach (byte b in bs)
                Console.Write(b.ToString("x") + " ");
            Console.Write("\n");

            //WriteProcessMemory 
            myProc.WriteMemory(address, BitConverter.GetBytes(1650.0d)); // for write a double value
            myProc.WriteMemory(address, BitConverter.GetBytes(1650.0f)); // for write a float value
            myProc.WriteMemory(address, "Hello World"); // for write a string value
            myProc.WriteMemory(address, new byte[] { 0xaa, 0xbb, 0xcc}); // for write a array of bytes value

            Console.WriteLine(myProc.WriteInt(address, 9919));



            myProc.CloseProcessHandle();

            Console.ReadLine();
       
        }
    }
}
