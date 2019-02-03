---
layout: post
title:  "ZX Spectrum IDE — Part #4: Emulating the Z80 CPU"
date:   2018-01-26 13:20:00 +0100
categories: "ZX-Spectrum"
abstract: >- 
  In this article, you will learn how I used .NET and C# to implement the Z80 Emulator.
---

Starting with this article, you will see tons of C# code. Here, I treat the main concepts and high-level implementation details of the Z80 CPU emulation.

{: class="note"}
__Note__: You may ask, why I have chosen the C# programming language—and why not another, e.g., C++. I have a short and a long answer. The short answer is this: I’ve been working with .NET since 2000, and I’m a rabid fan of the framework and the C# programming language. I will share the longer answer as a separate blog post in the future.

In the [previous post](/zx-spectrum/2018/01/23/zxspectrum-part-3.html), I already treated the fundamentals of the Z80 CPU, those that were essential when I designed the emulation.

### Design and Implementation Principles Used

I graduated as a software engineer, back in 1992, and participated over 50 software development projects in almost every role, excluding sales and marketing related positions. In the recent years I’ve been working as an agile coach and architect, still very close to software construction.

When I started [SpectNetIde](https://github.com/Dotneteer/spectnetide), I decided to use my favorite software design principles, namely S.O.L.I.D., and K.I.S.S. I could tell a lot about them, but I’m sure, you know them too—or if not, you can find the info on the internet in a minute. To summarize the value of these principles, I’d say, they help to design and implement software with automatic testing in mind.

### Devices

Although this post is about Z80 CPU emulation, the long-term objective is a ZX Spectrum IDE, which is a combination of a ZX Spectrum Emulator and a set of Development Tools.

Keeping this thought in mind, a ZX Spectrum emulator is a cohesive set of devices working together. Such a device is the Z80 CPU, the memory, the keyboard, the video display, the tape, and so on. So, one of the most important abstraction is `IDevice`:

```csharp
namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    public interface IDevice
    {
        void Reset();
    }
}
```

As you can see, `IDevice` is a simple concept: you can `Reset()` it.

{: class="note"}
__Note__: In the __SpectNetIde__ source code, you will find a lot of comments. In the blog post, I omit most of the comments for the sake of brevity. Whenever it has value, I include the namespaces of types, as they help you to lookup the corresponding source code file.

### The Z80 CPU as a Device

Z80 is a more versatile device, as the definition of `IZ80Cpu` shows it:

```csharp
namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    public interface IZ80Cpu : IClockBoundDevice
    {
        // --- CPU State
        Registers Registers { get; }
        Z80StateFlags StateFlags { get; set; }
        bool UseGateArrayContention { get; set; }
        bool IFF1 { get; }
        bool IFF2 { get; }
        byte InterruptMode { get; }
        bool IsInterruptBlocked { get; }
        bool IsInOpExecution { get; }

        // --- Devices closely related to CPU
        IMemoryDevice MemoryDevice { get; }
        IPortDevice PortDevice { get; }

        // --- Common actions
        void ExecuteCpuCycle();
        void Delay(int ticks);
        void SetResetSignal();
        void ReleaseResetSignal();

        // --- Tooling support
        int GetCallInstructionLength();
        IStackDebugSupport StackDebugSupport { get; set; }
        IBranchDebugSupport BranchDebugSupport { get; }
        bool MaskableInterruptModeEntered { get; }
    }
}
```

The CPU is a state machine. Several properties (such as `Registers`, `StateFlags`, and the others) define its current state. When the CPU executes an instruction (this is the responsibility of `ExecuteCpuCycle()`), the current state changes accordingly.

`IZ80Cpu` derives from an interface, `IClockBoundDevice`:

```csharp
namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    public interface IClockBoundDevice : IDevice
    {
        long Tacts { get; }
    }
}
```

`IClockBoundDevice` represents a general device that works with a clock signal. Its `Tacts` properties show the number of clock cycles spent since the system started.

### The State of the CPU

As a state machine, the Z80 needs to store its current state vector that is composed of registers, internal state flags of the CPU, and a few other attributes. Some instructions read and write the memory, transfer data between the CPU and I/O devices. You can take them into account as a part of the CPU’s state, too.

#### Registers and Flags

As you already learned, the 8-bit registers of Z80 can be paired into 16-bit registers, for example, __B__ and __C__ together give __BC__. Because of performance reason, I use `StructLayout`, and `FieldOffset` attributes to define the data structure for Z80 registers:

```csharp
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Spect.Net.SpectrumEmu.Cpu
{
    [StructLayout(LayoutKind.Explicit)]
    public class Registers
    {
        // --- Main register set
        [FieldOffset(0)]
        public ushort AF;
        [FieldOffset(2)]
        public ushort BC;
        [FieldOffset(4)]
        public ushort DE;
        [FieldOffset(6)]
        public ushort HL;

        // --- Alternate register set
        [FieldOffset(8)]
        public ushort _AF_;
        [FieldOffset(10)]
        public ushort _BC_;
        [FieldOffset(12)]
        public ushort _DE_;
        [FieldOffset(14)]
        public ushort _HL_;

        // ---Special purpose registers
        [FieldOffset(16)]
        public ushort IX;
        [FieldOffset(18)]
        public ushort IY;
        [FieldOffset(20)]
        [FieldOffset(22)]
        public ushort PC;
        [FieldOffset(24)]
        public ushort SP;
        [FieldOffset(26)]
        public ushort WZ;

        // --- 8-bit register access
        [FieldOffset(1)]
        public byte A;
        [FieldOffset(0)]
        public byte F;
        [FieldOffset(3)]
        public byte B;
        [FieldOffset(2)]
        public byte C;
        [FieldOffset(5)]
        public byte D;
        [FieldOffset(4)]
        public byte E;
        [FieldOffset(7)]
        public byte H;
        [FieldOffset(6)]
        public byte L;

        [FieldOffset(17)]
        public byte XH;
        [FieldOffset(16)]
        public byte XL;
        [FieldOffset(19)]
        public byte YH;
        [FieldOffset(18)]
        public byte YL;

        [FieldOffset(21)]
        public byte I;
        [FieldOffset(20)]
        public byte R;

        [FieldOffset(27)]
        public byte WZh;
        [FieldOffset(26)]
        public byte WZl;

        public bool SFlag => (F & FlagsSetMask.S) != 0;
        public bool ZFlag => (F & FlagsSetMask.Z) != 0;
        public bool R5Flag => (F & FlagsSetMask.R5) != 0;
        public bool HFlag => (F & FlagsSetMask.H) != 0;
        public bool R3Flag => (F & FlagsSetMask.R3) != 0;
        public bool PFlag => (F & FlagsSetMask.PV) != 0;
        public bool NFlag => (F & FlagsSetMask.N) != 0;
        public bool CFlag => (F & FlagsSetMask.C) != 0;

        // ...
    }
}
```

The `[StructLayout(LayoutKind.Explicit)]` annotation of the `Registers` class takes care that we can explicitly control the precise position of each member of the class in unmanaged memory. As you see from the listing, I decorated all fields with the `FieldOffset` attribute to indicate the position of that field within Registers.

This is how 16-bit registers and their constituting 8-bit pairs are mapped together:

```csharp
[FieldOffset(2)]
public ushort BC;
[FieldOffset(3)]
public byte B;
[FieldOffset(2)]
public byte C;
```

The __B__ and __C__ fields take the locations at offset 3 and 2, respectively, so they precisely overlay with __BC__. When I assign a value to __BC__, it affects the memory area of __B__ and __C__, and thus immediately changes the value of these 8-bit registers, and vice-versa.

{: class="note"}
__Note__: `StructLayout` and `FieldOffset` together can help to implement the `union` construct of C/C++.

You can see a register you probably have not heard about yet, its __WZ__. Well, this is an internal register of the Z80 CPU that helps to put a 16-bit register’s value onto the address bus. The only way to load the contents of these 16-bit registers is via the data bus. Two transfers will be necessary along the data bus to transfer 16 bits, and this is where __WZ__ helps. You cannot reach the contents of this internal register programmatically.

The registers class also has read-only properties to obtain field values. These accessors utilize the `FlagSetMask` type to get the bits to mask out the individual flags:

```csharp
namespace Spect.Net.SpectrumEmu.Cpu
{
    public static class FlagsSetMask
    {
        public const byte S = 0x80;
        public const byte Z = 0x40;
        public const byte R5 = 0x20;
        public const byte H = 0x10;
        public const byte R3 = 0x08;
        public const byte PV = 0x04;
        public const byte N = 0x02;
        public const byte C = 0x01;
        public const byte SZPV = S | Z | PV;
        public const byte NH = N | H;
        public const byte R3R5 = R3 | R5;
    }
}
```

{: class="note"}
__Note__: Initially I used an enum type, but later refactored it to byte constants. This approach made my flag-related operations shorter as I could avoid unnecessary type casts.

Similarly to `FlagSetMask`, I have a collection of byte constants that are more useful when setting or resetting individual flags:

```csharp
namespace Spect.Net.SpectrumEmu.Cpu
{
    public static class FlagsResetMask
    {
        public const byte S = 0x7F;
        public const byte Z = 0xBF;
        public const byte R5 = 0xDF;
        public const byte H = 0xEF;
        public const byte R3 = 0xF7;
        public const byte PV = 0xFB;
        public const byte N = 0xFD;
        public const byte C = 0xFE;
    }
}
```

#### Signal and Interrupt Status

In each execution cycle, the Z80 checks signals. I created an enum type, `Z80StateFlags`, to represent them:

```csharp
namespace Spect.Net.SpectrumEmu.Cpu
{
    [Flags]
    public enum Z80StateFlags
    {
        None = 0,
        Int = 0x01,
        Nmi = 0x02,
        Reset = 0x04,
        Halted = 0x08,
        InvInt = 0xFF - Int,
        InvNmi = 0xFF - Nmi,
        InvReset = 0xFF - Reset,
        InvHalted = 0xFF - Halted
    }
}
```

As you see, `Z80StateFlags` contains value members (with the `Inv` prefix) that can mask out the individual flag values. The benefit of this way is that I can keep the states of all signals in a variable of `Z80StateFlags` and use a simple condition (`state == 0`, where `state` is a `Z80StateFlags`) to check if any of the signals is set.

{: class="note"}
__Note__: `Int`, `Nmi`, and `Reset` represent the CPU signals with the same names. `Halted` is an output signal that the CPU uses to tell the external devices it is in HALTed state.

Earlier, you saw that the `IZ80Cpu` interface defines a few members related to interrupt state:

```csharp
public interface IZ80Cpu : IClockBoundDevice
{
    // ...
    bool IFF1 { get; }
    bool IFF2 { get; }
    byte InterruptMode { get; }
    bool IsInterruptBlocked { get; }
    // ...
}
```

__IFF1__ and __IFF2__ are two flip-flops (flags) within the Z80. The CPU uses __IFF1__ to check if a maskable interrupt is enabled. When the CPU starts, this flag is set to zero (disabled). The __EI__ instruction sets it to 1. The purpose of __IFF2__ is to save the status of __IFF1__ when a non-maskable interrupt occurs. When a non-maskable interrupt is accepted, __IFF1__ resets to prevent further interrupts until re-enabled by the program. Therefore, after a non-maskable interrupt is accepted, maskable interrupts are disabled, but the previous state of __IFF1__ is saved so that the complete state of the CPU just before the non-maskable interrupt can be restored when the interrupt routine completes.

The `InterruptMode` property retrieves the current mode set by any of the __IM 0__, __IM 1__, or __IM 2__ instructions.

The CPU samples the __INT__ signal with the rising edge of the final clock at the end of any instruction. Even if the maskable interrupt is enabled (__IFF1__ is true), the normal flow of execution cannot be immediately interrupted. The implementation of the CPU uses the `IsInterruptBlocked` property to handle this situation.

### Clock Cycles

Timing is everything. The Spectrum emulator could not work without it—precisely timed graphics effects would fail, so your favorite games would not run the way you expect.

The `IZ80Cpu` interface uses it ancestor’s (`IClockBoundDevice`) member, `Tacts` to manage the time spent since the system had started:

```csharp
long Tacts { get; }
```

`Tacts` counts the clock cycles. Its 64 bits are long enough to count the clock beats until the end of times. You can quickly check this statement: divide the __$7fff_ffff_ffff_ffff__ value with 3.500.000, the clock frequency, then with 86.400, the seconds in a day. The result shows the number or days (almost 30.5 million) `Tacts` can be used without overflow. It is a surprisingly high number!

There’s no reason to measure the CPU time in absolute units (let’s say, in nanoseconds). It would just make the things more complicated. Of course, when emulating real-time behavior, the time indicated by `Tacts` should be converted to absolute time. As you will learn it from a future article, you need this conversion about 50 or 60 times in a single second. These numbers are almost nothing compared to the 3.500.000 clock cycles per second.

### Opcode Processing State

In the [previous post](/zx-spectrum/2018/01/23/zxspectrum-part-3.html), you learned that Z80 has instructions with one, two, or three-byte opcodes.

In a single CPU cycle (__M1__ machine cycle) the CPU reads only one byte from the program. I use the values of two enum types (`OpPrefixMode`, and `OpIndexMode`) to keep up the current opcode processing state:

```csharp
public  partial class Z80Cpu: IZ80Cpu, IZ80CpuTestSupport
{
    // ...
    public enum OpIndexMode
    {
        None = 0,
        IX,
        IY
    }

    public enum OpPrefixMode : byte
    {
        None = 0,
        Extended,
        Bit
    }
}
```

### The Memory and I/O Devices

As I mentioned, the memory and the I/O devices are the part of the CPU’s state. The result of operations may depend on the values read from the memory or a device. Similarly, calculated values are persisted in the memory or sent to devices.

Just like the Z80 CPU, the memory and I/O port are devices, and thus implement the `IDevice` interface. I represent these components with the `IMemoryDevice` and `IPortDevice` interfaces, respectively.

Here, I show you only those interface methods that the CPU uses. This is `IMemoryDevice`:

```csharp
namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    public interface IMemoryDevice : ISpectrumBoundDevice
    {
        // ...
        byte Read(ushort addr, bool noContention = false);
        void Write(ushort addr, byte value);
        // ...
    }
}
```

I guess this definition is straightforward. The only thing you may not understand at this moment is the `noContention` argument of `Read()`. Right now, just take it as if it were not there. In a future article—not very long time from now—I will explain it with all other aspects of memory and I/O contention.

The definition of `IPortDevice` is very similar to `IMemoryDevice`:

```csharp
namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    public interface IPortDevice : ISpectrumBoundDevice
    {
        // ...
        byte ReadPort(ushort addr);
        void WritePort(ushort addr, byte data);
        // ...
    }
}
```

Now, you know how the state of the Z80 is stored. Let’s see how the emulation works!

### CPU Implementation

I encapsulated all functionality of the Z80 CPU into the `Z80Cpu` class, which has this definition:

```csharp
namespace Spect.Net.SpectrumEmu.Cpu
{
    public  partial class Z80Cpu: IZ80Cpu, IZ80CpuTestSupport
    {
        // ...
    }
}
```

You already know that `IZ80Cpu` is an abstraction of the CPU. However, you can see that the `Z80Cpu` class implements another interface, `IZ80CpuTestSupport`. But why?

The __S__ in S.O.L.I.D stands for _Single Responsibility_. `IZ80CpuTestSupport` defines those methods that are not part of the CPU’s abstraction, but implementing them helps in testing if the implementation works correctly:

```csharp
namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    public interface IZ80CpuTestSupport
    {
        void SetTacts(long tacts);
        void SetInterruptMode(byte im);
        Z80Cpu.OpPrefixMode PrefixMode { get; set; }
        Z80Cpu.OpIndexMode IndexMode { get; set; }
        void BlockInterrupt();
    }
}
```

With the methods of `IZ80CpuTestSupport`, I can easily disturb the normal operation of the CPU; for example, I can modify its clock, or externally block interrupts. If I’d add these operations to `IZ80Cpu`, I could make a programming error, since through an `IZ80Cpu` instance I could change the clock. Putting it into a separate interface, I can avoid these issues. Of course, in the concrete implementation of the Spectrum emulator, I must use a reference to an `IZ80Cpu` object and not to a `Z80Cpu` instance to prevent such a mistake.

### Multiple Files

I defined `Z80Cpu` as a partial class, because I implemented it in multiple files:

File | Role
--- | ---
__Z80Cpu.cs__ | Core routines
__Z80AluHelpers.cs__ | Helper methods I use in ALU operations
__Z80Operations.cs__ | The implementation of standard Z80 instructions (with no opcode prefix)
__Z80ExtendedOperations.cs__ | The implementation of extended Z80 instructions (with __$ED__ opcode prefix)
__Z80BitOperations.cs__ | The implementation of Z80 bit manipulation instructions (with __$CB__ opcode prefix)
__Z80IndexedOperations.cs__ | The implementation of indexed Z80 instructions (with __$DD__ or __$FD__ opcode prefix)
__Z80IndexedBitOperations.cs__ | The implementation of indexed Z80 bit manipulation instructions (with __$DD__, __$CB__, or __$FD__, __$CB__ opcode prefixes)
__Z80Debug.cs__ | The part of the CPU implementation used by the debugger tooling in __SpectNetIde__.

### Rock Around the Clock

Earlier you saw that the `Tacts` property of the CPU is crucial in measuring the number of clock cycles. The `Z80Cpu` class contains several helpers to make clocking fast and smooth in the code:

```csharp
public  partial class Z80Cpu
{
    private long _tacts;
    // ...

    public long Tacts => _tacts;
    // ...

    public void SetTacts(long tacts) // --- IZ80CpuTestSupport method
    {
        _tacts = tacts;
    }
    // ...

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Delay(int ticks)
    {
        _tacts += ticks;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClockP1()
    {
        _tacts += 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClockP2()
    {
        _tacts += 2;
    }
    // ...

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClockP7()
    {
        _tacts += 7;
    }
    // ...
}
```

As you see, I added methods that emulate the time passes (by means of increasing clock cycle counts). Because in the implementation of concrete Z80 instructions it is a typical operation to use delays with 1, 2, … 7 clock cycles, I created named methods for them. To make them as fast as possible, I decorated them with the `[MethodImpl(MethodImplOptions.AggressiveInlining)]` attribute to let the JIT-compiler create inline code when invoking them.

{: class="note"}
__Note__: Code inlining of means that the compiler inserts the entire function body into the code wherever you invoke the particular function—instead merely creating the invocation code. In C++, creating inline code is easy. In .NET, it is the task of the JIT compiler. With the `MethodImpl` attribute, you can give a hint to the JIT-compiler to inline the code, but you cannot force it.

### The Main Execution Cycle

The CPU (as a state machine) works continuously executing a loop, its main execution cycle. Here is how I implement it:

```csharp
public void ExecuteCpuCycle()
{
    if (ProcessCpuSignals()) return;

    // --- Get operation code and refresh the memory
    MaskableInterruptModeEntered = false;
    var opCode = ReadMemory(_registers.PC);
    ClockP3();
    _registers.PC++;
    RefreshMemory();

    if (_prefixMode == OpPrefixMode.None)
    {
        // -- The CPU is about to execute a standard operation
        switch (opCode)
        {
            case 0xDD:
                // --- An IX index prefix received
                _indexMode = OpIndexMode.IX;
                _isInOpExecution = _isInterruptBlocked = true;
                return;

            case 0xFD:
                // --- An IY index prefix received
                _indexMode = OpIndexMode.IY;
                _isInOpExecution = _isInterruptBlocked = true;
                return;

            case 0xCB:
                // --- A bit operation prefix received
                _prefixMode = OpPrefixMode.Bit;
                _isInOpExecution = _isInterruptBlocked = true;
                return;

            case 0xED:
                // --- An extended operation prefix received
                _prefixMode = OpPrefixMode.Extended;
                _isInOpExecution = _isInterruptBlocked = true;
                return;

            default:
                // --- Normal (8-bit) operation code received
                _isInterruptBlocked = false;
                _opCode = opCode;
                ProcessStandardOrIndexedOperations();
                _prefixMode = OpPrefixMode.None;
                _indexMode = OpIndexMode.None;
                _isInOpExecution = false;
                return;
        }
    }

    if (_prefixMode == OpPrefixMode.Bit)
    {
        // --- The CPU is already in BIT operations (0xCB) prefix mode
        _isInterruptBlocked = false;
        _opCode = opCode;
        ProcessCBPrefixedOperations();
        _indexMode = OpIndexMode.None;
        _prefixMode = OpPrefixMode.None;
        _isInOpExecution = false;
        return;
    }

    if (_prefixMode == OpPrefixMode.Extended)
    {
        // --- The CPU is already in Extended operations (0xED) prefix mode
        _isInterruptBlocked = false;
        _opCode = opCode;
        ProcessEDOperations();
        _indexMode = OpIndexMode.None;
        _prefixMode = OpPrefixMode.None;
        _isInOpExecution = false;
    }
}
```

The execution cycle starts with checking whether the CPU receives any new active signals (__INT__, __NMI__, or __RESET__). If it is so—`ProcessCpuSignals()` returns true—the CPU processed a signal, and thus this execution cycle completes.

Otherwise, the __M1__ machine cycle starts:

```csharp
// --- Get operation code and refresh the memory
MaskableInterruptModeEntered = false;
var opCode = ReadMemory(_registers.PC);
ClockP3();
_registers.PC++;
RefreshMemory();
```

I use the `MaskableInterruptModeEntered` flag in the integrated debugger so that I can step over Z80 statements that are the part of the currently running maskable interrupt routine. It does not play any role in the Z80 emulation.

The first real task is reading the subsequent opcode from the memory and incrementing the Program Counter. These operations consume the first three clock cycles of __M1__. Then, as you learned in the [previous article](/zx-spectrum/2018/01/23/zxspectrum-part-3.html), at the end of __M1__, the CPU refreshes the subsequent memory page (according to __R__). This is how it happens:

```csharp
private void RefreshMemory()
{
    _registers.R = (byte)(((_registers.R + 1) & 0x7F) | (_registers.R & 0x80));
    ClockP1();
}
```

Altogether, the __M1__ cycle consumes four clock cycles (`ClockP3() + ClockP1()`).

The other parts of the `ExecuteCpuCycle()` method manage the opcodes and prefixes. When the prefixes and opcodes form a full operation to carry on, one of these three methods is called according to the prefix: `ProcessStandardOrIndexedOperations()`, `ProcessCBPrefixedOperations()`, or `ProcessEDOperations()`.

The CPU takes care that an interrupt cannot suspend the normal operation of the CPU while the opcode bytes are not entirely collected:

```csharp
if (_prefixMode == OpPrefixMode.None)
{
    switch (opCode)
    {
        case 0xDD:
            // ... 
            _isInOpExecution = _isInterruptBlocked = true;
            return;
        case 0xFD:
            // ...
            _isInOpExecution = _isInterruptBlocked = true;
            return;
        case 0xCB:
            // ...
            _isInOpExecution = _isInterruptBlocked = true;
            return;
        case 0xED:
           // ...
            _isInOpExecution = _isInterruptBlocked = true;
            return;
        default:
            // --- Normal (8-bit) operation code received
            _isInterruptBlocked = false;
            // ...
            return;
    }
}
if (_prefixMode == OpPrefixMode.Bit)
{
    // --- The CPU is already in BIT operations (0xCB) prefix mode
    _isInterruptBlocked = false;
    // ...
    return;
}
if (_prefixMode == OpPrefixMode.Extended)
{
    // --- The CPU is already in Extended operations (0xED) prefix mode
    _isInterruptBlocked = false;
    // ...
}
```

### Processing CPU Signals

Every machine cycle starts with examining whether there is a signal the CPU can process. As its name suggests, the `ProcessCpuSignal()` method carries out this procedure. Its logic is straightforward:

```csharp
private bool ProcessCpuSignals()
{
    if (_stateFlags == Z80StateFlags.None) return false;

    if ((_stateFlags & Z80StateFlags.Int) != 0 && !_isInterruptBlocked && _iff1)
    {
        ExecuteInterrupt();
        return true;
    }

    if ((_stateFlags & Z80StateFlags.Halted) != 0)
    {
        ClockP3();
        RefreshMemory();
        return true;
    }

    if ((_stateFlags & Z80StateFlags.Reset) != 0)
    {
        ExecuteReset();
        return true;
    }

    if ((_stateFlags & Z80StateFlags.Nmi) != 0)
    {
        ExecuteNmi();
        return true;
    }

    return false;
}
```

However, there is one thing I have not mentioned yet. The CPU can be halted with the __HALT__ instruction. It this state, the CPU executes __NOP__ (_no operation_) instructions silently until a maskable interrupt is accepted or the CPU is reset. During that time (this is what the __M1__ machine cycle does), the CPU still takes care of refreshing the memory.

When the CPU receives a non-maskable interrupt, this is what it does:

```csharp
private void ExecuteNmi()
{
    if ((_stateFlags & Z80StateFlags.Halted) != 0)
    {
        _registers.PC++;
        _stateFlags &= Z80StateFlags.InvHalted;
    }
    _iff2 = _iff1;
    _iff1 = false;
    _registers.SP--;
    ClockP1();
    WriteMemory(_registers.SP, (byte)(_registers.PC >> 8));
    ClockP3();
    _registers.SP--;
    WriteMemory(_registers.SP, (byte)(_registers.PC & 0xFF));
    ClockP3();

    _registers.PC = 0x0066;
}
```

Provided the processor is in halted mode, it steps to the next instruction that follows __HALT__ and retrieves from this mode. It saves the __IFF1__ flag to __IFF2__ to preserve the __IFF1__ value while the interrupt routine completes. Sets __IFF1__ to false to disable any further interrupt during that time.

Then, saves the current value of __PC__ to the stack, and jumps to the __NMI__ routine address at __$0066__.

When a maskable interrupt is accepted, the logic is similar with some additional tasks:

```csharp
private void ExecuteInterrupt()
{
    if ((_stateFlags & Z80StateFlags.Halted) != 0)
    {
        _registers.PC++;
        _stateFlags &= Z80StateFlags.InvHalted;
    }
    _iff1 = false;
    _iff2 = false;
    _registers.SP--;
    ClockP1();
    WriteMemory(_registers.SP, (byte)(_registers.PC >> 8));
    ClockP3();
    _registers.SP--;
    WriteMemory(_registers.SP, (byte)(_registers.PC & 0xFF));
    ClockP3();

    switch (_interruptMode)
    {
        case 0:
        case 1:
            _registers.MW = 0x0038;
            ClockP5();
            break;
        default:
            ClockP2();
            var adr = (ushort)(_registers.IR & 0xFF00);
            ClockP5();
            var l = ReadMemory(adr);
            ClockP3();
            var h = ReadMemory(++adr);
            ClockP3();
            _registers.MW = (ushort)(h * 0x100 + l);
            ClockP6();
            break;
    }
    _registers.PC = _registers.MW;
    MaskableInterruptModeEntered = true;
}
```

Here, we handle the halted state and saving the current __PC__ address exactly as earlier. The `switch` statement handles the three interrupt modes (__IM 0__, __IM 1__, and __IM 2__), respectively. __IM 1__ (case 1) is the simplest; it merely sets the execution address to __$0038__. The __IM 0__ and __IM 2__ cases are a bit trickier. Both read data from the peripheral device that has raised the interrupt signal. If there is no such device, or it does not put any value to the data bus, the CPU sees a __$FF__ value. ZX Spectrum with no special devices attached works precisely this way.

__IM 0__ reads one byte from the device and executes the corresponding instruction. The __$FF__ code is the __RST $38__ instruction, and it calls the routine at the __$0038__ address. Thus, our code handles __IM 0__ and __IM 1__ the same way.

As you remember, __IM 2__ uses __I__ as the higher-order byte and the value read from the device as the lower-order byte to create a 16-bit address and then uses this vector to read the interrupt handler’s routine address. In the `switch` statement, the default case handles __IM 2__. It assumes that the device did not respond with any data (and so the CPU sees __$FF__), and calculates the routine address accordingly.

{: class="note"}
__Note__: This method sets the `MaskableInterruptModeEntered` flag to true to tell the debugging tool that we are executed into the maskable interrupt routine. This setting has nothing to do with Z80 emulation.

### Executing Instructions

When the CPU has the opcode for an entire operation, it calls one of these methods according to the operation prefix:

* No prefix, __$DD__ or __$FD__: `ProcessStandardOrIndexedOperations()`
* __$ED__ prefix: `ProcessEDOperations()`
* __$CB__ prefix; __$DD__ or __$FD__ followed by __$CB__: `ProcessCBPrefixedOperations()`

Each method uses a jump table with addresses of methods that process the operation with the opcode that matches the entry’s index. This is how `ProcessEDOperations()` works:

```csharp
public partial class Z80Cpu
{
    private Action[] _extendedOperations;

    private void ProcessEDOperations()
    {
        var opMethod = _extendedOperations[_opCode];
        opMethod?.Invoke();
    }
    // ...

    private void InitializeExtendedOpsExecutionTable()
    {
        _extendedOperations = new Action[]
        {
            null, null, null, null, null, null, null, null, // 00..07
            null, null, null, null, null, null, null, null, // 08..0F
            null, null, null, null, null, null, null, null, // 10..17
            null, null, null, null, null, null, null, null, // 18..1F
            null, null, null, null, null, null, null, null, // 20..27
            null, null, null, null, null, null, null, null, // 28..2F
            null, null, null, null, null, null, null, null, // 30..37
            null, null, null, null, null, null, null, null, // 38..3F

            IN_B_C, OUT_C_B, SBCHL_QQ, LDNNi_QQ, NEG, RETN, IM_N, LD_XR_A, // 40..47
            IN_C_C, OUT_C_C, ADCHL_QQ, LDQQ_NNi, NEG, RETI, IM_N, LD_XR_A, // 48..4F
            IN_D_C, OUT_C_D, SBCHL_QQ, LDNNi_QQ, NEG, RETN, IM_N, LD_A_XR, // 50..57
            IN_E_C, OUT_C_E, ADCHL_QQ, LDQQ_NNi, NEG, RETN, IM_N, LD_A_XR, // 58..5F
            IN_H_C, OUT_C_H, SBCHL_QQ, LDNNi_QQ, NEG, RETN, IM_N, RRD, // 60..67
            IN_L_C, OUT_C_L, ADCHL_QQ, LDQQ_NNi, NEG, RETN, IM_N, RLD, // 60..6F
            IN_F_C, OUT_C_0, SBCHL_QQ, LDNNi_QQ, NEG, RETN, IM_N, null, // 70..77
            IN_A_C, OUT_C_A, ADCHL_QQ, LDSP_NNi, NEG, RETN, IM_N, null, // 78..7F

            null, null, null, null, null, null, null, null, // 80..87
            null, null, null, null, null, null, null, null, // 88..8F
            null, null, null, null, null, null, null, null, // 90..97
            null, null, null, null, null, null, null, null, // 98..9F
            LDI, CPI, INI, OUTI, null, null, null, null, // A0..A7
            LDD, CPD, IND, OUTD, null, null, null, null, // A8..AF
            LDIR, CPIR, INIR, OTIR, null, null, null, null, // B0..B7
            LDDR, CPDR, INDR, OTDR, null, null, null, null, // B0..BF

            null, null, null, null, null, null, null, null, // C0..C7
            null, null, null, null, null, null, null, null, // C8..CF
            null, null, null, null, null, null, null, null, // D0..D7
            null, null, null, null, null, null, null, null, // D8..DF
            null, null, null, null, null, null, null, null, // E0..E7
            null, null, null, null, null, null, null, null, // E8..EF
            null, null, null, null, null, null, null, null, // F0..F7
            null, null, null, null, null, null, null, null, // F8..FF
        };
    }
    // ...
}
```

The constructor of `Z80Cpu` invokes the methods that initialize the jump tables (in the listing, it is the `InitializeExtendedOpsExecutionTable()` method).

A `null` entry in the jump table is an equivalent operation with the NOP instruction. It means that the CPU does not change its state. Wherever there is an operation method, that method executes the action that represents the associated instruction. For example, this is the action method for the __IM 0__, __IM 1__, and __IM 2__ operations:

```csharp
private void IM_N()
{
    var mode = (byte)((_opCode & 0x18) >> 3);
    if (mode < 2) mode = 1;
    mode--;
    _interruptMode = mode;
}
```

The binary opcodes (after the __$DE__ prefix) for the __IM 0__, __IM 1__ and __IM 2__ operations are these:

* `x1x0_0110`
* `x1x0_1110`
* `x1x1_0110`

Bit 4 and Bit 3 of the opcode define the value for the interrupt mode: __00__: __IM 0__; __01__: undefined (we set it to 0); __10__: __IM 1__; __11__: __IM 2__.

### Indexed Instructions

Processing an indexed instruction add some twist to the story. They use separate jump tables, as this code snippet shows:

```csharp
public partial class Z80Cpu
{
    private Action[] _standarOperations;

    private void ProcessStandardOrIndexedOperations()
    {
        var opMethod = _indexMode == OpIndexMode.None
            ? _standarOperations[_opCode]
            : _indexedOperations[_opCode];
        opMethod?.Invoke();
    }
    // ...
}
```

With the __$DD__ or __$FD__ prefix, the `ProcessStandardOrIndexedOperations()` method uses the `_indexedOperations` table.

One entry in that table is `INC_IX()`. Although the method name suggests it works with __IX__, it is responsible for processing the index register determined by the current prefix:

```csharp
private void INC_IX()
{
    SetIndexReg((ushort)(GetIndexReg() + 1));
    ClockP2();
}
```

The helper methods, `SetIndexReg()` and `GetIndexReg()` take care of using the appropriate register:

```csharp
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private ushort GetIndexReg()
{
    return _indexMode == OpIndexMode.IY ? _registers.IY : _registers.IX;
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
private void SetIndexReg(ushort value)
{
    if (_indexMode == OpIndexMode.IY)
    {
        _registers.IY = value;
    }
    else
    {
        _registers.IX = value;
    }
}
```


### Next: Instruction Details

By now, you know the fundamentals of the Z80 CPU emulation. You understand state management with registers and other state vector elements. You also have an overview of the execution cycle, the details of processing interrupt requests.

Some details that can be best covered by treating the details of individual Z80 instructions. In the next post, you will read about such nitty-gritty things.