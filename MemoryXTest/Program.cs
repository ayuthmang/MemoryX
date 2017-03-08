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
            myProc.getProcessHandle("Tutorial-i386");

            //http://stackoverflow.com/questions/3046784/byte-to-integer-in-c-sharp
            byte[] bytes = myProc.ReadMemory(0x01783A70);

            // If the system architecture is little-endian (that is, little end first),
            // reverse the byte array.
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            int i = BitConverter.ToInt32(bytes, 0);


            float f = myProc.ReadFloat(0x01783A70);
            double d = myProc.ReadDouble(0x01783A70);
            Single s = myProc.ReadSingle(0x01783A70);
            Console.WriteLine("float {0}\ndouble {1}", f, d);
            Console.WriteLine("{0}", s);

            myProc.closeHandle();
            Console.Read();

       
        }
    }
}
