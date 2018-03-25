### Virtual machine commands

```
Spectrum.Start;
Spectrum.Stop;
Spectrum.Pause;
Spectrum.RunTo(addr); // --- Runs till thye specified breakpoint
Spectrum.RunTo(addr, rom); // --- Runs till the specified breakpoint in the specified ROM
Spectrum.RunTo(addr1, addr2, ..., addrN); --- Runs until one of the breakpoints is accessed
Spectrum.RunToHalt; // --- Runs the code until a Halt instruction is executed
Spectrum.RemoveFromHalt; // --- Removes the CPU from the halted state
Spectrum.Step; // --- Executes the next instruction, then pauses
Spectrum.StepOver; // --- Steps over the next instruction, then pauses
Spectrum.Di; // --- Disables the interrupt;
Spectrum.Ei; // --- Enables the interrupt;
Spectrum.Call(addr);
Spectrum.InjectCode(byte[]);
Spectrum.InjectCode(string); // Complies the string, and injects it into the memory
Spectrum.Timeout = value; // --- Sets a timeout value to stop running when exceeded.
Spectrum.ResetOperationTracking;

Cpu.A = byte; // --- Sets the contents of A
Cpu.BC = word; // --- Sets the contents of BC

Memory[addr] = value; // --- Sets the value of the specified memory address
Bank[bank][addr] = value; // --- Sets the memory value of the specified bank

Memory[startAddr .. endAddr] = byte[]; // --- Sets the specified area of the memory
Memory.ResetReadTracking;
Memory.ResetWriteTracking;
Memory.Find(byte[]); // --- Searches for the specified memory pattern
Memory.Find(from, to, byte[]); // --- Searches for the specified memory pattern in the given range
Memory.FindNext(); Finds the next occurrance;

// --- Finds the first "ld a,3" instruction
addr = Memory.Find([0x3E, 0x03]);
if (addr < 0) 
{
    WriteLine("Not found");
}
else
{
    WriteLine("Found: {0}", addr);
}

// --- Checks the HL register value
if (Cpu.HL == #1234)
{
    Write("HL={0}",Cpu.HL); // --- Displays a console message
    WriteLine("Stopping script.");
    Exit; // --- Exits the script
}

// --- Displays the T-cycles of a routine
startPoint = Cpu.Tacts;
Spectrum.Timeout = 100; // --- 100ms timeout
Spectrum.Call(rutineToTest);
if (Spectrum.TimeoutExpired)
{
    WriteLine("The routine did not end");
    Exit;
}
WriteLine(Cpu.Tacts - startpoint);

// --- For loop
for (i= 0; i < 0xFF; i++)
{
    // ...
}

// --- Foreach loop
foreach (var value in Memory[0x4000 .. 0x57ff])
{
    // ...
}

// --- While cycle
while (!Cpu.Flags.S) 
{
    Spectrum.Step;
}

// --- Do..while loop
do 
{
    Spectrum.Step;
} while (!Cpu.Flags.S)



```