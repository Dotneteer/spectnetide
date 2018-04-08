# CpuZ80 class

This class represents the Z80 CPU of a Spectrum virtual machine. Using this class you can get and 
set the value of each register, register pair, flag, and other state holders. The class also provides
a few contol methods and events that are raised at different stages of the CPU's execution cycle.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public sealed class CpuZ80
```

## Contents at a glance

* [Register properties]
  * [8-bit registers]
  * [16-bit register pairs]
  * [Z80 CPU flags]
* [CPU state properties]
  * [Tacts]
  * [IFF1]
  * [IFF2]
  * [InterruptMode]
  * [IsInterruptBlocked]
  * [IsInOpExecution]
  * [MaskableInterruptModeEntered]
* [Control methods]
  * [Reset()]
  * [DisableInterrupt()]
  * [EnableInterrupt()]
* [Operation tracking]
* [Z80 CPU events]
  * [InterruptExecuting]
  * [NmiExecuting]
  * [MemoryReading]
  * [MemoryRead]
  * [MemoryWriting]
  * [MemoryWritten]
  * [PortReading]
  * [PortRead]
  * [PortWriting]
  * [PortWritten]
  * [OperationExecuting]
  * [OperationExecuted]
  * [OperationExecuting and OperationExecuted sample]

## Register properties

The class has separate properties for all 8-bit registers and 16-bit register pairs of the CPU. 
An 8-bit register's value is represented with a `System.Byte`, while a 16-bit register pair's value
with a `System.UInt16` instance.

### 8-bit registers

Name | Description
-----|------------
`A` | Accummulator
`B` | Register __B__
`C` | Register __C__
`D` | Register __D__
`E` | Register __E__
`H` | Register __H__
`L` | Register __L__
`F` | Flags register
`I` | Interrupt Page Address Register
`R` | Memory Refresh Register
`XH` | Higher 8 bits of the IX index register
`XL` | Lower 8 bits of the IX index register
`YH` | Higher 8 bits of the IY index register
`YL` | Lower 8 bits of the IY index register
`WZh` | Higher 8 bits of the internal WZ register
`WZl` | Lower 8 bits of the internal WZ register

### 16-bit register pairs

Name | Description
-----|------------
`AF` | Register pair __AF__
`BC` | Register pair __BC__
`DE` | Register pair __DE__
`HL` | Register pair __HL__
`_AF_` | Register pair __AF'__
`_BC_` | Register pair __BC'__
`_DE_` | Register pair __DE'__
`_HL_` | Register pair __HL'__
`PC` | Program Counter Register
`SP` | Stack Pointer Register
`IR` | I/R 8-bit registers as a register pair
`IX` | Index register __IX__
`IY` | Index register __IY__
`WZ` | The internal __WZ__ register

### Z80 CPU flags

With these properties, you can query the individual flags of the __F__ register. These properties
retrieve a `System.Boolean` value.

Name | Description
-----|------------
`SFlag` | __S__ (Sign) Flag
`ZFlag` | __Z__ (Zero) Flag
`R5Flag` | __R5__ Flag, Bit 5 of the __F__ register
`HFlag` | __H__ (Half Carry) Flag
`R3Flag` | __R3__ Flag, Bit 3 of the __F__ register
`PVFlag` | __P/V__ (Parity/Overflow) Flag
`NFlag` | __N__ (Add/Subtract) Flag
`CFlag` | __C__ (Carry) Flag

## CPU state properties

There are various properties that indicate the internal state of the CPU

### Tacts

```CSharp
public long Tacts { get; }
```

Gets the current T-state tacts of the CPU &mdash; the clock cycles since 
the CPU was powered on/reset last time

### IFF1

```CSharp
public bool IFF1 { get; }
```

Interrupt Enable Flip-Flop #1. Disables interrupts from being accepted.

### IFF2

```CSharp
public bool IFF2 { get; }
```

Interrupt Enable Flip-Flop #2. Temporary storage location for IFF1.

### InterruptMode

```CSharp
public byte InterruptMode { get; }
```

Gets the current Interrupt mode (`IM 0`, `IM 1`, or `IM 2`)

### IsInterruptBlocked

```CSharp
public bool IsInterruptBlocked { get; }
```

Indicates that the CPU internally blocks the interrupts, even if the maskable interrupt is enabled.
When the CPU is within processing a multi-byte statement, this flag is set.

### IsInOpExecution

```CSharp
public bool IsInOpExecution { get; }
```

Indicates that the CPU still needs to read more bytes to decode a multi-byte operation.

### MaskableInterruptModeEntered

```CSharp
public bool MaskableInterruptModeEntered { get; }
```

Indicates that the instructions the CPU is executing are the part of the maskable interrupt method.

## Control methods

The `CpuZ80` class provides a few control operation that you can use in the scripts.


### Reset()

```CSharp
public void Reset()
```

Issues a Reset (`RST`) signal to the CPU.

### DisableInterrupt()

```CSharp
public void DisableInterrupt()
```

Disables the maskable interrupt.

### EnableInterrupt()

```CSharp
public void EnableInterrupt()
```

Enables the maskable interrupt.

## Operation tracking

The `CpuZ80` class allows you to track the addresses the CPU executes an operation for.

```CSharp
public AddressTrackingState OperationTrackingState { get; }
```

The `OperationTrackingState` property's value is an [`AddressTrackingState`](AddressTrackingState.md) 
instance. It provides a bit for each memory address in the `#0000`-`#FFFF` range to check if the 
particular byte in the memory has been read as a part of CPU's M1 machine cycle.

```CSharp
public void ResetOperationTracking()
```

The `ResetOperationTracking()` method resets the `OperationTrackingState` property as if no 
operations had been executed.

The following sampe demonstrates how you can use these members of `CpuZ80` to check which instructions
have been executed. (This code snippet is an extract for a unit test of __SpectNetIde__.)

```CSharp
[TestMethod]
public async Task OperationTrackingWorks()
{
    // --- Arrange
    var sm = SpectrumVmFactory.CreateSpectrum48Pal();
    sm.Breakpoints.AddBreakpoint(0x11CB);
    sm.StartDebug();
    await sm.CompletionTask;
    var pcBefore = sm.Cpu.PC;
    sm.ExecutionCompletionReason.ShouldBe(ExecutionCompletionReason.BreakpointReached);
    sm.MachineState.ShouldBe(VmState.Paused);

    // --- Act
    sm.Cpu.ResetOperationTracking();
    sm.Breakpoints.ClearAllBreakpoints();
    sm.Breakpoints.AddBreakpoint(0x11CE);
    sm.StartDebug();
    await sm.CompletionTask;
    var pcAfter = sm.Cpu.PC;

    // --- Assert
    pcBefore.ShouldBe((ushort)0x11CB);
    pcAfter.ShouldBe((ushort)0x11CE);
    sm.Cpu.OperationTrackingState.TouchedAny(0x0000, 0x11CA).ShouldBeFalse();
    sm.Cpu.OperationTrackingState[0x11CB].ShouldBeTrue();
    sm.Cpu.OperationTrackingState[0x11CC].ShouldBeTrue();
    sm.Cpu.OperationTrackingState[0x11CD].ShouldBeTrue();
    sm.Cpu.OperationTrackingState.TouchedAny(0x11CE, 0xFFFF).ShouldBeFalse();
}
```

## Z80 CPU events

The `CpuZ80` class provides about a dozen events that you can use in script.

### InterruptExecuting

```CSharp
public event EventHandler InterruptExecuting
```

This event is raised just before a maskable interrupt is about to execute.

### NmiExecuting

```CSharp
public event EventHandler NmiExecuting
```

This event is raised just before a non-maskable interrupt is about to execute.

### MemoryReading

```CSharp
public event EventHandler<AddressEventArgs> MemoryReading
```

This event is raised just before the memory is being read. The event argument (`AddressEventArgs`) 
has a property, `Address` that tells the memory address going to be read.

```CSharp
public ushort Address { get; }
```

> When this operation is being executed, the contents of the memory has not been read yet.

### MemoryRead

```CSharp
public event EventHandler<AddressAndDataEventArgs> MemoryRead
```

This event is raised right after the memory has been read. The event argument (`AddressAndDataEventArgs`) 
contains these properties:

```CSharp
public ushort Address { get; }
public byte Data { get; }
```

The `Address` keeps the address read, `Data` holds the data byte resulted from the read operation.

### MemoryWriting

```CSharp
public event EventHandler<AddressAndDataEventArgs> MemoryWriting
```

This event is raised just before the memory is being written. The `Address` and `Data` properties of the
event argument (`AddressAndDataEventArgs`) contain the memory address, and the data byte to write, 
respectively.

### MemoryWritten

```CSharp
public event EventHandler<AddressAndDataEventArgs> MemoryWritten
```

This event is raised just after the memory has been written. The `Address` and `Data` properties of the
event argument (`AddressAndDataEventArgs`) contain the memory address, and the data byte written, 
respectively.

### PortReading

```CSharp
public event EventHandler<AddressEventArgs> PortReading
```

This event is raised just before a port is being read. The event argument (`AddressEventArgs`) 
has a property, `Address` that tells the port address going to be read.

```CSharp
public ushort Address { get; }
```

> When this operation is being executed, the port has not been read yet.

### PortRead

```CSharp
public event EventHandler<AddressAndDataEventArgs> PortRead
```

This event is raised just after a port has been read. The event argument (`AddressAndDataEventArgs`) 
contains these properties:

```CSharp
public ushort Address { get; }
public byte Data { get; }
```

The `Address` keeps the address of the port read, `Data` holds the data byte resulted from the read operation.

### PortWriting

```CSharp
public event EventHandler<AddressAndDataEventArgs> PortWriting
```

This event is raised just before a port is being written. The `Address` and `Data` properties of the
event argument (`AddressAndDataEventArgs`) contain the port address, and the data byte to write, 
respectively.

### PortWritten

```CSharp
public event EventHandler<AddressAndDataEventArgs> PortWritten
```

This event is raised just after a port has been written. The `Address` and `Data` properties of the
event argument (`AddressAndDataEventArgs`) contain the port address, and the data byte written, 
respectively.

### OperationExecuting

```CSharp
public event EventHandler<Z80InstructionExecutionEventArgs> OperationExecuting
```

This event is raised just before a Z80 operation is being executed. The event arguments
instance ([`Z80InstructionExecutionEventArgs`](Z80InstructionExecutionEventArgs.md)) provides 
properties you can check the operation being executed.

> When this event is raised, the CPU might not have read all operation code bytes, just those one that
> are enough to decode the type of operation to execute. If the operation contains arguments that are
> part of the operation code, those are not yet read.  
> For example when this event is signed, the opcode part for `#80` `LD A,#80`, or the #23 opcode part
> of `LD (IX+#23),C` operation is not read.

### OperationExecuted

```CSharp
public event EventHandler<Z80InstructionExecutionEventArgs> OperationExecuted
```

This event is raised just after a Z80 operation has been executed. The event arguments
instance ([`Z80InstructionExecutionEventArgs`](Z80InstructionExecutionEventArgs.md)) provides 
properties you can check the operation executed.

> When this event is raised, the full operation is already read.

### OperationExecuting and OperationExecuted sample

To demonstrate the difference between these events, the following sample code snippets (extracts from
__SpectNetIde__ unit tests) provide more details.

```CSharp
[TestMethod]
public async Task OperationExecutingIsInvoked()
{
    // --- Arrange
    var sm = SpectrumVmFactory.CreateSpectrum48Pal();
    sm.CachedVmStateFolder = STATE_FOLDER;
    await sm.StartAndRunToMain(true);
    var events = new List<Z80InstructionExecutionEventArgs>();

    // --- Act
    var entryAddress = sm.InjectCode(@"
        .org #8000
        di;              8000: No prefix
        bit 3,a;         8001: CB prefix
        ld a,(ix+2);     8003: DD prefix
        ld a,(iy+6);     8006: FD prefix
        bit 2,(ix+2);    8009: DD CB prefixes
        bit 2,(iy+6);    800D: FD CB prefixes
        in d,(c);        8011: ED prefix
        ret
    ");
    sm.Cpu.OperationExecuting += (s, e) => { events.Add(e); };
    sm.CallCode(entryAddress);
    await sm.CompletionTask;

    // --- Assert
    var sampleIndex = events.Count - 1 - 7; // -1 for the RET operation, -7 for the 7 operations
    var op = events[sampleIndex];
    op.PcBefore.ShouldBe((ushort)0x8000);
    op.Instruction.SequenceEqual(new byte[] { 0xF3 }).ShouldBeTrue();
    op.OpCode.ShouldBe((byte)0xF3);
    op.PcAfter.ShouldBeNull();

    op = events[sampleIndex + 1];
    op.PcBefore.ShouldBe((ushort)0x8001);
    op.Instruction.SequenceEqual(new byte []{ 0xCB, 0x5F}).ShouldBeTrue();
    op.OpCode.ShouldBe((byte)0x5F);
    op.PcAfter.ShouldBeNull();

    op = events[sampleIndex + 2];
    op.PcBefore.ShouldBe((ushort)0x8003);
    op.Instruction.SequenceEqual(new byte[] { 0xDD, 0x7E }).ShouldBeTrue();
    op.OpCode.ShouldBe((byte)0x7E);
    op.PcAfter.ShouldBeNull();

    op = events[sampleIndex + 3];
    op.PcBefore.ShouldBe((ushort)0x8006);
    op.Instruction.SequenceEqual(new byte[] { 0xFD, 0x7E }).ShouldBeTrue();
    op.OpCode.ShouldBe((byte)0x7E);
    op.PcAfter.ShouldBeNull();

    op = events[sampleIndex + 4];
    op.PcBefore.ShouldBe((ushort)0x8009);
    op.Instruction.SequenceEqual(new byte[] { 0xDD, 0xCB, 0x56 }).ShouldBeTrue();
    op.OpCode.ShouldBe((byte)0x56);
    op.PcAfter.ShouldBeNull();

    op = events[sampleIndex + 5];
    op.PcBefore.ShouldBe((ushort)0x800D);
    op.Instruction.SequenceEqual(new byte[] { 0xFD, 0xCB, 0x56 }).ShouldBeTrue();
    op.OpCode.ShouldBe((byte)0x56);
    op.PcAfter.ShouldBeNull();

    op = events[sampleIndex + 6];
    op.PcBefore.ShouldBe((ushort)0x8011);
    op.Instruction.SequenceEqual(new byte[] { 0xED, 0x50 }).ShouldBeTrue();
    op.OpCode.ShouldBe((byte)0x50);
    op.PcAfter.ShouldBeNull();
}
```

You can check that the `Instruction` property is tested against the partial opcode.

```CSharp
[TestMethod]
public async Task OperationExecutedIsInvoked()
{
    // --- Arrange
    var sm = SpectrumVmFactory.CreateSpectrum48Pal();
    sm.CachedVmStateFolder = STATE_FOLDER;
    await sm.StartAndRunToMain(true);
    var events = new List<Z80InstructionExecutionEventArgs>();

    // --- Act
    var entryAddress = sm.InjectCode(@"
        .org #8000
        di;              8000: No prefix
        bit 3,a;         8001: CB prefix
        ld a,(ix+2);     8003: DD prefix
        ld a,(iy+6);     8006: FD prefix
        bit 2,(ix+2);    8009: DD CB prefixes
        bit 2,(iy+6);    800D: FD CB prefixes
        in d,(c);        8011: ED prefix
        ret
    ");
    sm.Cpu.OperationExecuted += (s, e) => { events.Add(e); };
    sm.CallCode(entryAddress);
            await sm.CompletionTask;

    // --- Assert
    var sampleIndex = events.Count - 1 - 7; // -1 for the RET operation, -7 for the 7 operations
    var op = events[sampleIndex];
    op.PcBefore.ShouldBe((ushort)0x8000);
    op.Instruction.SequenceEqual(new byte[] { 0xF3 }).ShouldBeTrue();
    op.OpCode.ShouldBe((byte)0xF3);
    op.PcAfter.ShouldBe((ushort)0x8001);

    op = events[sampleIndex + 1];
    op.PcBefore.ShouldBe((ushort)0x8001);
    op.Instruction.SequenceEqual(new byte[] { 0xCB, 0x5F }).ShouldBeTrue();
    op.OpCode.ShouldBe((byte)0x5F);
    op.PcAfter.ShouldBe((ushort)0x8003);

    op = events[sampleIndex + 2];
    op.PcBefore.ShouldBe((ushort)0x8003);
    op.Instruction.SequenceEqual(new byte[] { 0xDD, 0x7E, 0x02}).ShouldBeTrue();
    op.OpCode.ShouldBe((byte)0x7E);
    op.PcAfter.ShouldBe((ushort)0x8006);

    op = events[sampleIndex + 3];
    op.PcBefore.ShouldBe((ushort)0x8006);
    op.Instruction.SequenceEqual(new byte[] { 0xFD, 0x7E, 0x06 }).ShouldBeTrue();
    op.OpCode.ShouldBe((byte)0x7E);
    op.PcAfter.ShouldBe((ushort)0x8009);

    op = events[sampleIndex + 4];
    op.PcBefore.ShouldBe((ushort)0x8009);
    op.Instruction.SequenceEqual(new byte[] { 0xDD, 0xCB, 0x56, 0x02 }).ShouldBeTrue();
    op.OpCode.ShouldBe((byte)0x56);
    op.PcAfter.ShouldBe((ushort)0x800D);

    op = events[sampleIndex + 5];
    op.PcBefore.ShouldBe((ushort)0x800D);
    op.Instruction.SequenceEqual(new byte[] { 0xFD, 0xCB, 0x56, 0x06 }).ShouldBeTrue();
    op.OpCode.ShouldBe((byte)0x56);
    op.PcAfter.ShouldBe((ushort)0x8011);

    op = events[sampleIndex + 6];
    op.PcBefore.ShouldBe((ushort)0x8011);
    op.Instruction.SequenceEqual(new byte[] { 0xED, 0x50 }).ShouldBeTrue();
    op.OpCode.ShouldBe((byte)0x50);
    op.PcAfter.ShouldBe((ushort)0x8013);
}
```

Here, the `Instruction` property is checked against the entire opcode.
