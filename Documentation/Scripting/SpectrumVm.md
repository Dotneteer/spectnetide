# SpectrumVm class

This class represents a Spectrum virtual machine. You cannot instantiate a machine
with the constructor of `SpectrumVm`, for it is not accessible. To create a Spectrum machine,
use the [`SpectrumVmFactory`](SpectrumVmFactory) class.

`SpectrumVm` is a pivotal type in the scripting feature of __SpectNetIde__. When you have an
object instance in your code, it provides a plethora of properties and methods to access and control
the Spectrum virtual machine.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public sealed class SpectrumVm: IDisposable, ISpectrumVmController
```



## Contents at a glance

* [Machine properties]
  * [ModelKey]
  * [EditionKey]
  * [Cpu]
  * [Roms]
  * [RomCount]
  * [PagingInfo]
  * [Memory]
  * [RamBanks]
  * [RamBankCount]
  * [Keyboard]
  * [ScreenConfiguration]
  * [ScreenRenderingTable]
  * [ScreenBitmap]
  * [ScreenRenderingStatus]
  * [BeeperConfiguration]
  * [BeeperSamples]
  * [SoundConfiguration]
  * [AudioSamples]
  * [Breakpoints]
  * [TimeoutInMs]
  * [TimeoutTacts]
  * [RealTimeMode]
  * [DisableScreenRendering]
  * [MachineState]
  * [ExecutionCompletionReason]
  * [RunsInDebugMode]
  * [CompletionTask]
  * [CachedVmStateFolder]
* [Control Methods and Properties]
  * [IsFirstStart]
  * [IsFirstPause]
  * [Start()]
  * [Start(ExecuteCycleOptions)]
  * [StartDebug()]
  * [RunUntilHalt()]
  * [RunUntilFrameCompletion()]
  * [RunUntilTerminationPoint(ushort, int)]
  * [StartAndRunToMain(bool)]
  * [Pause()]
  * [Stop()]
  * [StepInto()]
  * [StepOver()]
* [Machine state management methods]
  * [SaveMachineStateTo(string)]
  * [RestoreMachineState(string)]
* [Code manipulation methods]
  * [InjectCode(ushort, byte array)]
  * [InjectCode(string, AssemblerOptions)]
  * [CallCode(ushort, ushort?)]
* [Virtual machine events] 
  * [VmStateChanged]
  * [VmStoppedWithException]
  * [VmFrameCompleted]

## Machine properties

With these properties, you can access all important components of the virtual machine.

### ModelKey

```CSharp
public string ModelKey { get; }
```
Gets the model key of the virtual machine (see [`SpectrumModels`](SpectrumModels)).

### EditionKey

```CSharp
public string EditionKey { get; }
```
Gets the edition key of the virtual machine (see [`SpectrumModels`](SpectrumModels)).

### Cpu

```CSharp
public CpuZ80 Cpu { get; }
```

Gets the object that represents the current state of the Z80 CPU. 
You can also use this instance to control the CPU (see also [`CpuZ80`](CpuZ80)).

### Roms

```CSharp
public IReadOnlyList<ReadOnlyMemorySlice> Roms { get; }
```

Provides access to the binary contents of each individual ROM of
the virtual machine. Each item of the list provides a ROM of the machine.
For example, in a ZX Spectrum 128K machine instance, there are two ROMs
with index 0, and 1. (see also [`ReadOnlyMemorySlice`](ReadOnlyMemorySlice))

### RomCount

```CSharp
public int RomCount { get; }
```

Retrieves the number of ROMs.

### PagingInfo

```CSharp
public MemoryPagingInfo PagingInfo { get; }
```

Allows you to obtain information about memory paging. There's no reason
to use this property for a ZX Spectrum 48K virtual machine, as this model 
does not support memory paging. However, it is useful for other models. 
See [`MemoryPagingInfo`](MemoryPagingInfo) for more details.

### Memory

```CSharp
public SpectrumMemoryContents Memory { get; }
```

Represents the current contents of the addressable 64K memory of the
virtual machine. You can use this property to read and write the memory.
See [`SpectrumMemoryContents`](SpectrumMemoryContents) for details.

### RamBanks

```CSharp
public IReadOnlyList<MemorySlice> RamBanks { get; }
```

With this property, you can access the contents of each RAM bank, independently 
whether that particular bank is paged in. The elements of the list represent the
memory banks from #0 to #7. Check [MemorySlice]() for details.

### RamBankCount

```CSharp
public int RamBankCount { get; }
```

Gets the number of RAM banks available.

### Keyboard

```CSharp
public KeyboardEmulator Keyboard { get; }
```

You can access the state of the virtual keyboard through this class. You can also
use it to emulate keystrokes. See [KeyboardEmulator](KeyboardEmulator) for details.

### ScreenConfiguration

```CSharp
public ScreenConfiguration ScreenConfiguration { get; }
```

Allows access to the details of the screen configuration that determine the tacts 
of screen rendering, such as number of raster lines, non-visible screen area, border pixels
above and below the display area, and many more. Check [`ScreenConfiguration`](ScreenConfiguration) 
for all available properties of the configuration object.

### ScreenRenderingTable

```CSharp
public ScreenRenderingTable ScreenRenderingTable { get; }
```

The virtual machine uses a table that contains an item for each screen
rendering tact. You can access the information about an individual tact
through this porperty. The [`ScreenRenderingTable`]() contains more
details about the information you can access.

### ScreenBitmap

```CSharp
public ScreenBitmap ScreenBitmap { get; }
```

Provides access to each individual pixels of the visible screen. This object
represents not only the display area (the 256 x 192 pixels for a ZX Spectrum 48K 
model), but also the border area. To check the dimensions, use the properties of
[`ScreenConfiguration`](ScreenConfiguration). The reference documentation of
[`ScreenBitmap`](ScreenBitmap) offers more details about addressing the pixels
within this object.

### ScreenRenderingStatus

```CSharp
public ScreenRenderingStatus ScreenRenderingStatus { get; }
```

Provides information about the current screen rendering status. 
See [`ScreenRenderingStatus`]() for more details.

### BeeperConfiguration

```CSharp
public IAudioConfiguration BeeperConfiguration { get; }
```

Gets the configuration of the beeper device (such as sampling frequency, and others).
[`IAudioConfiguration`](IAudioConfiguration) provides more details.

### BeeperSamples

```CSharp
public AudioSamples BeeperSamples { get; }
```

Gets the beeper samples of the current screen rendering frame. The 
[`AudioSamples`](AudioSamples) type describes how you can access individual samples.

### SoundConfiguration

```CSharp
public IAudioConfiguration SoundConfiguration { get; }
```

Gets the configuration of the PSG (AY-3-8912 chip) sound device (such as sampling frequency, and others).
[`IAudioConfiguration`](IAudioConfiguration) provides more details.

### AudioSamples

```CSharp
public AudioSamples AudioSamples { get; }
```

Gets the PSG (AY-3-8912 chip) sound samples of the current screen rendering frame. The 
[`AudioSamples`](AudioSamples) type describes how you can access individual samples.

### Breakpoints

```CSharp
public CodeBreakpoints Breakpoints { get; }
```

This property allows you to manage the breakpoints. When you run the virtual machine
in debug mode (see the `StartDebug()` method), the virtual machine is paused whenever
it reaches a breakpoint. The [`CodeBreakpoints`](CodeBreakpoints) documentation gives 
you more clues.

### TimeoutInMs

```CSharp
public long TimeoutInMs { get; set; }
```

You can start the virtual machine with a timeout value. When the specified
timeout expires, the virtual machine pauses. With the `TimeoutInMs` property,
you can set up this value given in milliseconds.

> Internally, the virtual machine transforms this value into CPU T-states according to
> the clock frequency.

### TimeoutTacts

```CSharp
public long TimeoutTacts { get; set; }
```

With this property, you can specify the timeout value in CPU T-states. Providing the CPU uses
a
3.5 MHz clock, setting `TimeoutTacts` to 3500 equals to set `TimeoutInMs` to 1.

> Specifying zero value for `TimeoutInMs` or `TimeoutTacts` means that there's no explicit
> timeout, thus the virtual machine runs until explicitly stopped or paused in the code.
### RealTimeMode

```CSharp
public bool RealTimeMode { get; set; }
``` 

Normally, scripting runs the virtual machine in quick mode, so it continuously runs the
CPU and renders the screen. It does not sync the machine with the screen rendering
frequency. If you set this property to true, the virtual machine will run with the same speed
as a physical machine would do.

### DisableScreenRendering

```CSharp
public bool DisableScreenRendering { get; set; }
```

Setting this property to true would disable the screen rendering, and so the virtual
machine would faster. Nonetheless, in this mode &mdash; as the screen is not remdered
&mdash; you cannot use `ScreenBitmap` to check the contents of the rendered frame. 
After the machine starts, the contents remains the same.

### MachineState

```CSharp
public VmState MachineState { get; private set; }
```

This property allows you to check the current state of the virtual machine.
Take a look at the [`VmState`](VmState) enumeration for details about the
possible states.

### ExecutionCompletionReason

```CSharp
public ExecutionCompletionReason ExecutionCompletionReason { get; }
```

The virtual machine can go into paused or stopped state for many reasons.
The [`ExecutionCompletionReason`](ExecutionCompletionReason) enumeration allows you to check what
event has caused that the machine is in a specific state.

### RunsInDebugMode

```CSharp
public bool RunsInDebugMode { get; }
```

Indicates if the virtual machine runs in debug mode.

### CompletionTask

```CSharp
public Task CompletionTask { get; }
```

After th virtual machine has been started, it runs in a background thread.
While it runs, you can use the UI thread for other activities. 
The `CompletionTask` property can be used to wait for the completion of
the virtual machine (it gets paused or stopped).

For example, this sample starts the virtual machine id debug mode, does
some other activity on the UI thread, and then wait while the machine reaches
its firs breakpoint:

```CSharp
var sm = SpevtrumVmFactory.CreateSpectrum48Pal();
// --- Set up breakpoints (omitted from code)
sm.StartDebug();
// --- Do something on UI thread (omitted from code)
await sm.CompletionTask;
// --- Go on on UI thread
```

### CachedVmStateFolder

```CSharp
public string CachedVmStateFolder { get; set; }
```

You can speed up the virtual machine startup by starting it from its
saved state. The scripting engine stores these state files (with `.vmstate` extension)
in a cache folder. By default, it is the current folder.
With setting `CachedVmStateFolder`, you can specify the storage location
of these state files.

# Control Methods and Properties

The Spectrum virtual machine's execution cycle &mdash; the one that executes CPU instruction,
takes care of rendering the screen and sound &mdash; runs on a background thread. With the control
methods, you can start, pause, and stop the machine in multiple ways. When you need to wait for the
completion of the execution cycle, you can use the [CompletionTask] property.

### IsFirstStart

```CSharp
public bool IsFirstStart { get; }
```
Signs that this is the very first start of the virtual machine.

### IsFirstPause

```CSharp
public bool IsFirstPause { get; }
```
Signs that this is the very first pause of the virtual machine after its first start.

### Start()

```CSharp
public void Start();
```

Starts the virtual machine in continuous execution mode. It runs unless the script pauses or
stops it, or the specified timeout expires.

### Start(ExecuteCycleOptions)

```CSharp
public void Start(ExecuteCycleOptions options)
```

Starts the virtual machine with the specified execution mode (see also [ExecuteCycleOptions](ExecuteCycleOptions)).
The machine runs unless the script pauses or stops it, or the specified timeout expires.

### StartDebug()

```CSharp
public void StartDebug()
```

Starts the virtual machine in debug mode and sets the `RunsInDebugMode` property to true. Whenever the execution flow reaches a breakpoint, the machine gets
paused. Additionally, the machine runs unless the script pauses or stops it, or the specified timeout expires.

### RunUntilHalt()

```CSharp
public void RunUntilHalt();
```

Starts the Spectrum machine and runs it on a background thread until it reaches a HALT instruction, then is gets
paused. Otherwise, the machine runs unless the script pauses or stops it, or the specified timeout expires.

### RunUntilFrameCompletion()

```CSharp
public void RunUntilFrameCompletion()
```

Starts the Spectrum machine and runs it on a background thread until the current screen rendering frame is 
completed. Then it gets paused. Otherwise, the machine runs unless the script pauses or stops it, or 
the specified timeout expires.

### RunUntilTerminationPoint(ushort, int)

```CSharp
public void RunUntilTerminationPoint(ushort address, int romIndex = 0)
```

Starts the Spectrum machine and runs it on a background thread until the CPU reaches the specified 
termination point (address). At that point the machine gets paused. Otherwise, the machine runs unless 
the script pauses or stops it, or the specified timeout expires.

#### Arguments
`address`: The address at which to pause the machine  
`romIndex`: If the address is within the ROM, romIndex specified the index of the ROM that should 
be selected to pause the machine.

#### Sample

You can start and run a ZX Spectrum 48K virtual machine while it reaches the main execution cycle
point (`#12A9`) in the ROM:

```CSharp
var sm = SpectrumVmFactory.CreateSpectrum48Pal();
sm.RunUntilTerminationPoint(0x12A9);
await sm.CompletionTask;
```

### StartAndRunToMain(bool)

```CSharp
public async Task StartAndRunToMain(bool spectrum48Mode = false)
```

A convenience method that starts the virtual machine and pauses it when it reaches its main
execution cycle. The method can be used to wait while the machine pauses.

#### Arguments
`spectrum48Mode`: Setting this flag to true, ZX Spectrum 128K and later models are started in
Spectrum 48 BASIC mode 

#### Remarks

Behind the scenes, this method does a lot of things for you:
1. First it checks, if there's a saved machine state image for the machine model. If there is, 
it simply loads that state and puts the machine into paused state.

2. Otherwise, according to the machine type and the `spectrum48Mode` flag, it determines the 
termination point.
    * It starts the virtual machine and runs it to the desired termination point.
    * While doing this, the method emulates key strokes so that the machine could start
in the desired mode.
    * After the termination mode is reached, pauses the machine and saves its state so that the
next startup would be faster.

3. The method retrieves the `CompletionTask` property so that you can wait for completion.

#### Sample

This code snippet starts a ZX Spectrum 128K virtual machine in 48 mode, and waits while the 
machine reaches the main execution cycle.

```CSharp
var sm = SpectrumVmFactory.CreateSpectrum128Pal();
await sm.StartAndRunToMain(true);
```

### Pause()

```CSharp
public async Task Pause()
```

Pauses the running Spectrum virtual machine. After it is paused, you can continue the execution
with any of these methods: `Start()`, `Start(ExecutionCycleOptions)`, `StartDebug`, 
`RunUntilHalt()`, `RunUntilFrameCompletion()`, `RunUntilTerminationPoint()`.

### Stop()

```CSharp
public async Task Stop()
```

Stops the running or paused Spectrum virtual machine. After it is stopped, you can resttart 
the execution with any of these methods: `Start()`, `Start(ExecutionCycleOptions)`, `StartDebug`, 
`RunUntilHalt()`, `RunUntilFrameCompletion()`, `RunUntilTerminationPoint()`.

### StepInto()

```CSharp
public void StepInto()
```

Executes the subsequent Z80 instruction, and then pauses the machine. As the execution happens 
on a background thread, you should wait for the completion with the `CompletionTask` property:

```CSharp
var sm = SpectrumVmFactory.CreateSpectrum48Pal();
// --- Do something with the machine and then pause (code omitted)
sm.StepInto();
await sm.CompletionTask;
```

### StepOver()

```CSharp
public void StepOver()
```

Executes the subsequent Z80 CALL, RST, or block instruction (such as LDIR, LDDR, CPIR, etc.) entirely.
For example, when the subsequent instruction is a CALL, the machine gets paused as the engine detects
that the control flow is right on the next instruction after CALL (when the subroutine returned). By
using it, you do not have to step through all instructions one-by-one with `StepInto()`.

> This method has a potential issue. If the method does not return to the expected location, for example,
> it manipulates the stack, the virtual machine would not pause. It is definitely an issue with `RST #08` on
> a ZX Spectrum 128K model, or with `RST #28` on ZX Spectrum 48K. In the future, I am going to examine how
> I can handle this issue &mdash; now, it is not perfect.

## Machine state management methods

The scripting provides simple methods to save and loade the current state of the virtual machine. This state
is composed from the state of each individual devices of the machine, including the CPU, memory, screen, 
beeper, sound devices, and all the others. WHile the machine is running, you cannot save the state, first you
should pause it.

You can load the state when the machine is newly created, paused, or stopped.

> There are some limitations of state file handling. When you pause the machine while it's loading a program 
> from the tape and save its state, after restoring it cannot continue loading from the tape. The reason
> behind this behavior is that the state file does not contain the data that represents the tape.
> I'm investigating the possible resolutions.

> The virtual machine state files (with `.vmstate` extension) are simple JSON files, so you can open them
> and check their structure to get more details on what's within them. You can recognize that the contents 
> of the memory is compressed. A Spectrum 48K state file takes about 25KBytes.

### SaveMachineStateTo(string)

```CSharp
public void SaveMachineStateTo(string filename)
```

Saves the state of the paused virtual machine to the specified file.

#### Arguments
`filename`: Name of the file to save the machine state

### RestoreMachineState(string)

```CSharp
public void RestoreMachineState(string filename)
```

Loads the state of the paused or stopped machine from the specified file. When you continue the 
execution, the machine will start from the freshly loaded state.

#### Arguments
`filename`: Name of the file that contains the machine state

## Code manipulation methods

While the virtual machine is stopped, you can inject code into the memory so that you can execute it.
You do not need to use the [Memory] property of `SpectrumVm` to do this. There are a few methods
that makes it extremely easy.

### InjectCode(ushort, byte array)

```CSharp
public void InjectCode(ushort address, byte[] codeArray)
```

Injects code into the memory. Keeps the machine in paused state.

#### Arguments
`address`: Start address of the injection  
`codeArray`: A byte array that contains the machine code bytes in the order of injection

### InjectCode(string, AssemblerOptions)

```CSharp
public ushort InjectCode(string asmSource, AssemblerOptions options = null)
```

This powerful method allows you to define the code to inject, in Z80 assembler language, using
all features the language provides. In the __SpectNetIde__ assembler, you can create multiple 
code segments, and this method injects all cade segments.

#### Arguments
`asmSource`: The Z80 Assembler source code  
`options`: Optional options to compile the source code. See [`AssemblerOptions`]() more details.

#### Returns

The address marked as the entry point (`ENT` directive in the source code), or the start 
address (`ORG` directive) of the first code block in the source code.

### CallCode(ushort, ushort?)

```CSharp
public void CallCode(ushort startAddress, ushort? callStubAddress = null)
```

Calls the code at the specified subroutine start address.

#### Arguments
`startAddress`: Start address of the subroutine  
`callStubAddress`: Optional address for the call stub

#### Remarks

This method creates a Z80 CALL instruction, and stores it to the specified `callStubAddress`.
If this parameter is omitted (set to `null`), the `#5BA0` address (empty area after system variables)
is used. Then, the virtual machine starts and pauses at the termination point of 
`callStubAddress + 3` (by default, `#5BA3`).

### Sample using code injection

The following sample injects and executes code that sets the color of the background to blue.

```CSharp
var sm = SpectrumVmFactory.CreateSpectrum48Pal();
await sm.StartAndRunToMain();
var entryAddr = sm.InjectCode(@"
    .org #8000
    ld a,1       ; BLUE
    out (#FE),a  ; Set the border colour to blue
    ret          ; Finished
");
sm.CallCode(entryAddr);
await sm.CompletionTask;
```

Instead of call, you can jump directly to the code. Nonetheless, in this case you cannot complete
your code with `RET`, you should jump back to the main execution cycle of the machine, as this
sample shows:

```CSharp
var sm = SpectrumVmFactory.CreateSpectrum48Pal();
await sm.StartAndRunToMain();
var entryAddr = sm.InjectCode(@"
    .org #8000
    ld a,1       ; BLUE
    out (#FE),a  ; Set the border colour to blue
    jp #12A2     ; Finished
");
sm.Cpu.PC = entryAddr;
// --- Set up breakpoints (omitted from code)
sm.StartDebug();
await sm.CompletionTask;
```

## Virtual machine events

The virtual machine provides events you can use to examine and analyze the state of the machine.

### VmStateChanged

```CSharp
public event EventHandler<VmStateChangedEventArgs> VmStateChanged;
```

This event is raised whenever the state of the virtual machine changes.

The `VmStateChangedEventArgs` class has these properties:

```CSharp
public VmState OldState { get; }
public VmState NewState { get; }
```

As theor names suggest, you can obtain the old and the new state ([`VmState`](VmState)) of the machine.

### VmStoppedWithException

```CSharp
public event EventHandler<VmStoppedWithExceptionEventArgs> VmStoppedWithException;
```

Whenever the Spectrum virtual machine stops because of an exception, this event is raised. Normally,
this event should not happen, it signs that there's some unexpected issue (programming error) within
__SpectNetIde__. The `VmStoppedWithExceptionEventArgs` has this property to check what is the exception
that caused the machine stop:

```CSharp
public Exception Exception { get; }
```

### VmFrameCompleted

```CSharp
public event EventHandler VmFrameCompleted;
```

To sign that the virtual machine just has completed a new screen rendering frame, this event is raised.
