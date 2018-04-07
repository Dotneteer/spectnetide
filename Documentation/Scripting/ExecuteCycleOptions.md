# ExecuteCycleOptions class

This class provides options for the execution cycle of the Spectrum virtual machine. When you start
the cycle, you can pass execution options that influence the machine cycle and specifies when the
machine should be paused.

__Namespace__: `Spect.Net.SpectrumEmu.Machine`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public class ExecuteCycleOptions
```

### EmulationMode

```CSharp
public EmulationMode EmulationMode { get; }
```

The emulation mode that should be used. The values of the `EmulationMode` enumeration are these:

Value | Description
------|------------
`Continuous` | Runs the virtual machine until stopped
`Debugger` | Runs the virtual machine in debug mode
`UntilHalt` | Run the virtual machine until the CPU is halted
`UntilFrameEnds` | Runs the machine until the current ULA rendering frame ends
`UntilExecutionPoint` | Run the machine until the specified value of the PC register is reached

### DebugStepMode

```CSharp
public DebugStepMode DebugStepMode { get; }
```

When the emulation mode is set to `Debugger`, this property specifies the mode to run a debug-mode
execution cycle. The values of `DebugStepMode`:

Value | Description
------|------------
`StopAtBreakpoint` | Execution stops at the next breakpoint
`StepInto` | Execution stops after the next instruction
`StepOver` | Execution stops after the next instruction. If that should be a subroutine call or a block statement, the execution stops after returning from the subroutine or completing the block statement.

### FastTapeMode

```CSharp
public bool FastTapeMode { get; }
```

Indicates if fast loading from the tape is allowed.

### TermnationRom
 
```CSharp
public int TerminationRom { get; }
```

The index of the ROM when a termination point is defined.

### TerminationPoint

```CSharp
public ushort TerminationPoint { get; }
```

The value of the PC register to reach when `EmulationMode` is set to `UntilExecutionPoint`.

### SkipInterruptRoutine

```CSharp
public bool SkipInterruptRoutine { get; }
```

Signs if the instructions within the maskable interrupt routine should be skipped during debugging.

### FastVmMode

```CSharp
public bool FastVmMode { get; }
```

This flag indicates that the virtual machine should run in hidden mode (no screen, no sound, no delays).

### DisableScreenRendering

```CSharp
public bool DisableScreenRendering { get; }
```

This flag shows whether the virtual machine should render the screen when runs in `FastVmMode`.

### TimeoutTacts

```CSharp
public long TimeoutTacts { get; }
```

You can specify a timeout value (given in CPU tacts). If this is set to zero, no timeout is applied.
If set to a value greater than zero, after the specified number of CPU cycles ellapsed, the execution of
the virtual machine is paused.
