# MemoryX

A memory module for .net application.

This module allow you to use the function WriteProcessMemory or ReadProcessMemory in easy way.



IntPtr GetBaseAddress() -- return BaseAddress of module

For example use:

```csharp
    // New an object , one object per process
    MemoryX.Memory myProc = new MemoryX.Memory();
    
    // for process name without .exe 
    // Example you open task manager and see "notepad.exe" 
    // you can change and put it into "procName" without extensions
    var procName = "notepad"; 
    
    //address for access our process memory
    var address = 0x000D1940;

    // for open our process
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
    
    
    // for read a single byte value from memory
    Console.WriteLine("BYTES IS "  + myProc.ReadMemory(address , 1)[0].ToString("X"));

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


    // get a base address of module and print out
    Console.WriteLine(myProc.GetBaseAddress("notepad.exe").ToString("X"));
```

## Read memory pointer
```cs
    MemoryX.Memory myProc = new MemoryX.Memory();
    var procName = "Tutorial-x86_64";
    myProc.GetProcessHandle(procName);
    
    long baseAddress = myProc.GetBaseAddress(procName + ".exe"); 
    long address = baseAddress + 0x002C4A80; 
    int[] offsets = new int[] {0x10, 0x18 ,0 , 0x18}; 
 
    Console.WriteLine(myProc.ReadMemoryPointerInt(address, offsets)); 
```

