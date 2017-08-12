using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace MemoryX
{
    public class Memory
    {

        //Good article for this source: https://www.codeproject.com/Articles/670373/Csharp-Read-Write-another-Process-Memory
        private IntPtr proc_Handle;
        private int proc_ID;
        private int bytesWritten;
        private int bytesRead;


        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress,
        UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [Flags]
        public enum MemoryProtection
        {
            PAGE_NOACCESS = 1 ,
            PAGE_READONLY = 2 ,
            PAGE_READWRITE = 4 ,
            PAGE_WRITECOPY = 8 ,
            PAGE_EXECUTE = 16 ,
            PAGE_EXECUTE_READ = 32 ,
            PAGE_EXECUTE_READWRITE = 64 ,
            PAGE_EXECUTE_WRITECOPY = 128 ,
            PAGE_GUARD = 256 ,
            PAGE_NOCACHE = 512
        }

        [Flags]
        public enum ProcessAccess
        {

            /// <summary>
            /// Required to create a thread.
            /// </summary>
            CreateThread = 0x0002,

            /// <summary>
            ///
            /// </summary>
            SetSessionId = 0x0004,

            /// <summary>
            /// Required to perform an operation on the address space of a process
            /// </summary>
            VmOperation = 0x0008,

            /// <summary>
            /// Required to read memory in a process using ReadProcessMemory.
            /// </summary>
            VmRead = 0x0010,

            /// <summary>
            /// Required to write to memory in a process using WriteProcessMemory.
            /// </summary>
            VmWrite = 0x0020,

            /// <summary>
            /// Required to duplicate a handle using DuplicateHandle.
            /// </summary>
            DupHandle = 0x0040,

            /// <summary>
            /// Required to create a process.
            /// </summary>
            CreateProcess = 0x0080,

            /// <summary>
            /// Required to set memory limits using SetProcessWorkingSetSize.
            /// </summary>
            SetQuota = 0x0100,

            /// <summary>
            /// Required to set certain information about a process, such as its priority class (see SetPriorityClass).
            /// </summary>
            SetInformation = 0x0200,

            /// <summary>
            /// Required to retrieve certain information about a process, such as its token, exit code, and priority class (see OpenProcessToken).
            /// </summary>
            QueryInformation = 0x0400,

            /// <summary>
            /// Required to suspend or resume a process.
            /// </summary>
            SuspendResume = 0x0800,

            /// <summary>
            /// Required to retrieve certain information about a process (see GetExitCodeProcess, GetPriorityClass, IsProcessInJob, QueryFullProcessImageName).
            /// A handle that has the PROCESS_QUERY_INFORMATION access right is automatically granted PROCESS_QUERY_LIMITED_INFORMATION.
            /// </summary>
            QueryLimitedInformation = 0x1000,

            /// <summary>
            /// Required to wait for the process to terminate using the wait functions.
            /// </summary>
            Synchronize = 0x100000,

            /// <summary>
            /// Required to delete the object.
            /// </summary>
            Delete = 0x00010000,

            /// <summary>
            /// Required to read information in the security descriptor for the object, not including the information in the SACL.
            /// To read or write the SACL, you must request the ACCESS_SYSTEM_SECURITY access right. For more information, see SACL Access Right.
            /// </summary>
            ReadControl = 0x00020000,

            /// <summary>
            /// Required to modify the DACL in the security descriptor for the object.
            /// </summary>
            WriteDac = 0x00040000,

            /// <summary>
            /// Required to change the owner in the security descriptor for the object.
            /// </summary>
            WriteOwner = 0x00080000,

            StandardRightsRequired = 0x000F0000,

            /// <summary>
            /// All possible access rights for a process object.
            /// </summary>
            AllAccess = StandardRightsRequired | Synchronize | 0xFFFF
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WriteProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WriteProcessMemory(IntPtr hProcess, long lpBaseAddress, int value, int dwSize, ref int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern int ReadProcessMemory(IntPtr hProcess,  int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        public static extern int ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        public int GetBytesWritten()
        {
            return bytesWritten;
        }
        public bool CloseProcessHandle()
        {
            return CloseHandle(proc_Handle);
        }
        public IntPtr GetProcessHandle()
        {
            return proc_Handle;
        }
        public int GetProcessID()
        {
            return this.proc_ID;
        }
        public Boolean GetProcessHandle(int PID)
        {
            try
            {
                Process proc = Process.GetProcessById(PID);
                this.proc_ID = proc.Id;
                this.proc_Handle = OpenProcess((int)ProcessAccess.AllAccess, false, proc_ID);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public Boolean GetProcessHandle(String procName)
        {
            try
            {
                // for search all processes by name
                foreach (Process proc in Process.GetProcessesByName(procName))
                {
                    //take the first process 
                    this.proc_ID = proc.Id;
                    this.proc_Handle = OpenProcess((int)ProcessAccess.AllAccess, false, this.proc_ID);
                    return true;
                }
                return false;

            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Return a BaseAddress of module
        /// </summary>
        public long GetBaseAddress(String moduleName)
        {
            IntPtr baseAddress = IntPtr.Zero;
            try
            {
                foreach (ProcessModule PM in Process.GetProcessById(proc_ID).Modules)
                {
                    if (moduleName.ToLower() == PM.ModuleName.ToLower())
                        baseAddress = PM.BaseAddress;
                }
                return (long)baseAddress;
            }
            catch (Exception ex)
            {
                return (long)IntPtr.Zero;
            }
        }

        /// <summary>
        /// Changes the protection of the page with the specified starting address to PAGE_EXECUTE_READWRITE
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa366786(v=vs.85).aspx
        /// </summary>
        public bool RemoveProtection(long lpBaseAddress)
        {
            uint oldProtect;
            return VirtualProtectEx(proc_Handle, new IntPtr(lpBaseAddress), new UIntPtr(2048),Convert.ToUInt32( MemoryProtection.PAGE_EXECUTE_READWRITE ), out oldProtect);
        }
        //Public Sub RemoveProtection(ByVal AddressOfStart As Integer) 
        //    On Error Resume Next
        //    Dim oldProtect As Integer
        //    If Not VirtualProtectEx(pHandle, New IntPtr(AddressOfStart), New IntPtr(2048), MemroyProtection.PAGE_EXECUTE_READWRITE, oldProtect) Then Throw New Win32Exception
        //End Sub


        public int WriteMemory(long lpBaseAddress , byte[] value)
        {
            //Console.WriteLine("Process id: " + this.proc_ID);
            //Console.WriteLine("Process Handle : " + this.proc_Handle);

            //var arr = BitConverter.GetBytes(value);
            return WriteProcessMemory(proc_Handle, lpBaseAddress, value, value.Length, ref bytesWritten);


            //https://msdn.microsoft.com/en-us/library/bb383973.aspx
            //http://stackoverflow.com/questions/4271291/writeprocessmemory-with-an-int-value
            //var array = BitConverter.GetBytes(i);
            //int bytesWritten;
            //WriteProcessMemory(GameHandle, WriteAddress, array, (uint)array.Length, out bytesWritten);
        }

        public int WriteMemory(long lpBaseAddress, String value)
        {
            //http://stackoverflow.com/questions/16072709/converting-string-to-byte-array-in-c-sharp
            //byte[] toBytes = Encoding.ASCII.GetBytes(somestring);
            //You will need to turn it back into a string like this:
            //string something = Encoding.ASCII.GetString(toBytes);
            var arr = Encoding.ASCII.GetBytes(value);
            return WriteProcessMemory(proc_Handle, lpBaseAddress, arr, arr.Length, ref bytesWritten);
        }

        public int WriteMemory(long lpBaseAddress , int value)
        {
            return WriteMemory(lpBaseAddress, BitConverter.GetBytes(value));
        }

        public int WriteMemory(long lpBaseAddress, float value)
        {
            return WriteMemory(lpBaseAddress, BitConverter.GetBytes(value));
        }

        public int WriteMemory(long lpBaseAddress, double value)
        {
            return WriteMemory(lpBaseAddress, BitConverter.GetBytes(value));
        }

        public int WriteMemory(long lpBaseAddress, byte value)
        {
            return WriteMemory(lpBaseAddress, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Read a memory address value and return to array of bytes value
        /// </summary>
        public byte[] ReadMemory(long lpBaseAddress, int dwSize)
        {
            var buffer = new byte[dwSize];
            ReadProcessMemory(proc_Handle, lpBaseAddress, buffer, buffer.Length, ref bytesRead);
            return buffer;
        }

        /// <summary>
        /// Return a memory address and return to int value
        /// </summary>
        public int ReadInt32(long lpBaseAddress)
        {
            //http://www.pinvoke.net/default.aspx/kernel32.readprocessmemory
            byte[] buffer = new byte[8];
            ReadProcessMemory(proc_Handle, lpBaseAddress, buffer, 4, ref bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>
        /// Return a memory address and return to float or single value
        /// </summary>
        public Single ReadSingle(long lpBaseAddress)
        {
            //http://www.pinvoke.net/default.aspx/kernel32.readprocessmemory
            //http://stackoverflow.com/questions/30694922/modify-function-to-read-float-c-sharp
            byte[] buffer = new byte[8];
            ReadProcessMemory(proc_Handle, lpBaseAddress, buffer, 8, ref bytesRead);
            return BitConverter.ToSingle(buffer, 0); ;
        }

        /// <summary>
        /// Return a memory address and return to float or single value
        /// </summary>
        public float ReadFloat(long lpBaseAddress) //float and single is the same value, so we can use readSingle
        {
            return ReadSingle(lpBaseAddress);
        }

        /// <summary>
        /// Return a memory address and return to double value
        /// </summary>
        public Double ReadDouble(long lpBaseAddress)
        {
            byte[] buffer = new byte[8];
            ReadProcessMemory(proc_Handle, lpBaseAddress, buffer, 8, ref bytesRead);
            return BitConverter.ToDouble(buffer, 0); ;
        }

        /// <summary>
        /// Read a memory address and return to String
        /// </summary>
        public String ReadString(long lpBaseAddress, int length)
        {
            //http://stackoverflow.com/questions/1003275/how-to-convert-byte-to-string
            byte[] buffer = new byte[length];
            ReadProcessMemory(proc_Handle, lpBaseAddress, buffer, length, ref bytesRead);
            return System.Text.Encoding.UTF8.GetString(buffer); ;
        }

        /// <summary>
        /// Read memory pointer and return into int
        /// </summary>
        public int ReadMemoryPointerInt(long lpBaseAddress, int[] offsets)
        {
            int ptr = ReadInt32(lpBaseAddress);
            foreach (int offset in offsets)
            {
                ptr += offset;
                ptr = ReadInt32(ptr);
            }
            return ptr;
        }
    }
}
