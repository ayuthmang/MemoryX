using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MemoryX
{
    public class Memory
    {
        private IntPtr proc_Handle;
        private int proc_ID;
        private int bytesWritten;

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

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        public int getBytesWritten()
        {
            return bytesWritten;
        }
        public void closeHandle()
        {
            CloseHandle(proc_Handle);
        }
        public IntPtr getProcessHandle()
        {
            return proc_Handle;
        }
        public int getProcessID()
        {
            return this.proc_ID;
        }
        public Boolean getProcessHandle(int PID)
        {
            try
            {
                Process proc = Process.GetProcessById(PID);
                this.proc_ID = proc.Id;
                this.proc_Handle = OpenProcess(0x1F0FFF, false, proc_ID);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public Boolean getProcessHandle(String procName)
        {
            try
            {
                // for search all processes by name
                foreach (Process proc in Process.GetProcessesByName(procName))
                {
                    //take the first process 
                    this.proc_ID = proc.Id;
                    this.proc_Handle = OpenProcess(0x1F0FFF, false, this.proc_ID);
                    return true;
                }
                return false;

            }
            catch
            {
                return false;
            }

        }

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

        public int WriteMemory(long lpBaseAddress, String str)
        {
            //http://stackoverflow.com/questions/16072709/converting-string-to-byte-array-in-c-sharp
            //byte[] toBytes = Encoding.ASCII.GetBytes(somestring);

            //You will need to turn it back into a string like this:

            //string something = Encoding.ASCII.GetString(toBytes);
            var arr = Encoding.ASCII.GetBytes(str);
            return WriteProcessMemory(proc_Handle, lpBaseAddress, arr, arr.Length, ref bytesWritten);
        }

    }
}
