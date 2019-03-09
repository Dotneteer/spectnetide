---
layout: documents
categories: 
  - "Scripting"
title:  "Managing Virtual machines"
alias: scripting-vms
seqno: 30
selector: documents
permalink: "documents/scripting-vms"
---

# The SpectrumVmFactory class

This class provides methods you can use to create ZX Spectrum virtual machine instances. 
Each factory methods retrieves a [`SpectrumVm`](SpectrumVm.md) instance you can use to control and query
the particular machine.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public static class SpectrumVmFactory
```

## Create(string, string)

```
public static SpectrumVm Create(string modelKey, string editionKey)
```

Creates a new [`SpectrumVm`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrumvm-class) instance according to the specified *`modelKey`* 
and *`editionKey`* values.

### Arguments
`modelkey`: The name of the Spectrum model.  
`editionKey`: The name of the Spectrum edition
> The [`SpectrumModels`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrummodels-class) class defines several string constants that you can use for these arguments. 

### Returns

The newly created ZX Spectrum virtual machine instance.

### Exceptions

`ArgumentNullException`: `modelKey` or `editionKey` is `null`.  
`KeyNotFoundException`: The `SpectrumModels` class does not have a definition
for `modelKey`, or the model does not have an edition for `editionKey`.

### Sample

```
var sm = Create(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.NTSC);
```

## CreateSpectrum48Pal()

```
public static SpectrumVm CreateSpectrum48Pal()
```

Creates a new ZX Spectrum 48K PAL virtual machine and retrieves it as 
a [`SpectrumVm`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrumvm-class) instance.

## CreateSpectrum48PalTurbo()

```
public static SpectrumVm CreateSpectrum48PalTurbo()
```

Creates a new ZX Spectrum 48K PAL virtual machine with double CPU speed (7MHz) and retrieves it as 
a [`SpectrumVm`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrumvm-class) instance.

## CreateSpectrum48Ntsc()

```
public static SpectrumVm CreateSpectrum48Ntsc()
```

Creates a new ZX Spectrum 48K NTSC virtual machine and retrieves it as 
a [`SpectrumVm`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrumvm-class) instance.

## CreateSpectrum48NtscTurbo()

```
public static SpectrumVm CreateSpectrum48NtscTurbo()
```

Creates a new ZX Spectrum 48K NTSC virtual machine with double CPU speed (7MHz) and retrieves it as 
a [`SpectrumVm`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrumvm-class) instance.

## CreateSpectrum128()

```
public static SpectrumVm CreateSpectrum128()
```

Creates a new ZX Spectrum 128K PAL virtual machine and retrieves it as 
a [`SpectrumVm`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrumvm-class) instance.

## CreateSpectrumP3E()

```
public static SpectrumVm CreateSpectrum128()
```

Creates a new ZX Spectrum +3E PAL virtual machine and retrieves it as 
a [`SpectrumVm`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrumvm-class) instance.

# Provider configuration

The `SpectrumVmFactory` class allows to change providers to customize
the virtual machine to your needs. Doing this requires a bit deeper knowledge
of how the emulator in SpectNetIde works. Probably, you do not need to
use your own providers at all.

Nonetheless, these are the methods you can use to change the default provider
configuration:

## Reset()

Resets the `SpectrumVmFactory` class, clears the predefined providers.

```
public static void Reset()
```

After invoking `Reset()` you need to set up your own providers.

## RegisterProvider&lt;TProvider&gt;(Func&lt;TProvider&gt;)

Registers a factory method for the `TProvider` type.

```
public static void RegisterProvider<TProvider>(Func<TProvider> factory)
```
You can register your own _`factory`_ method that creates a provider for 
`TProvider`.

## RegisterDefaultProviders()

Allows you to register the default providers.

```
public static void RegisterDefaultProviders()
```

When you need to restore the original behavior of `SpectrumVmFactory`,
use this code snippet:

```
SpectrumVmFactory.Reset();
SpectrumVmFactory.RegisterDefaultProviders();
```

By default, `SpectrumVmFactory` uses these provider registrations:

```
RegisterProvider<IRomProvider>(() 
    => new ResourceRomProvider(typeof(RomResourcesPlaceHolder).Assembly));
RegisterProvider<IKeyboardProvider>(() 
    => new ScriptingKeyboardProvider());
RegisterProvider<IBeeperProvider>(() 
    => new NoAudioProvider());
RegisterProvider<ITapeProvider>(() 
    => new ScriptingTapeProvider());
RegisterProvider<ISoundProvider>(() 
    => new NoAudioProvider());
```

As the names suggest, the virtual machine created by `SpectrumVmFactory` does not 
generate audio (neither for the beeper nor for the PSG).

# The SpectrumVm class

This class represents a Spectrum virtual machine. You cannot instantiate a machine
with the constructor of `SpectrumVm`, for it is not accessible. To create a Spectrum machine,
use the [`SpectrumVmFactory`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrumvmfactory-class) class.

`SpectrumVm` is a pivotal type in the scripting feature of __SpectNetIde__. When you have an
object instance in your code, it provides a plethora of properties and methods to access and control
the Spectrum virtual machine.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public sealed class SpectrumVm: IDisposable, ISpectrumVmController
```

With the properties of this class, you can access all important components of the virtual machine.

## ModelKey

```
public string ModelKey { get; }
```
Gets the model key of the virtual machine (see [`SpectrumModels`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrummodels-class)).

## EditionKey

```
public string EditionKey { get; }
```
Gets the edition key of the virtual machine (see [`SpectrumModels`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrummodels-class)).

## Cpu

```
public CpuZ80 Cpu { get; }
```

Gets the object that represents the current state of the Z80 CPU. 
You can also use this instance to control the CPU (see also [`CpuZ80`]({{ site.baseurl }}/documents/scripting-z80.html#the-cpuz80-class)).

## Roms

```
public IReadOnlyList<ReadOnlyMemorySlice> Roms { get; }
```

Provides access to the binary contents of each individual ROM of
the virtual machine. Each item of the list provides a ROM of the machine.
For example, in a ZX Spectrum 128K machine instance, there are two ROMs
with index 0, and 1. (see also [`ReadOnlyMemorySlice`]({{ site.baseurl }}/documents/scripting-memory.html#the-readonlymemoryslice-class))

## RomCount

```
public int RomCount { get; }
```

Retrieves the number of ROMs.

## PagingInfo

```
public MemoryPagingInfo PagingInfo { get; }
```

Allows you to obtain information about memory paging. There's no reason
to use this property for a ZX Spectrum 48K virtual machine, as this model 
does not support memory paging. However, it is useful for other models. 
See [`MemoryPagingInfo`]({{ site.baseurl }}/documents/scripting-memory.html#the-memorypaginginfo-class) for more details.

## Memory

```
public SpectrumMemoryContents Memory { get; }
```

Represents the current contents of the addressable 64K memory of the
virtual machine. You can use this property to read and write the memory.
See [`SpectrumMemoryContents`]({{ site.baseurl }}/documents/scripting-memory.html#the-spectrummemorycontents-class) for details.

## RamBanks

```
public IReadOnlyList<MemorySlice> RamBanks { get; }
```

With this property, you can access the contents of each RAM bank, independently 
whether that particular bank is paged in. The elements of the list represent the
memory banks from #0 to #7. Check [MemorySlice]({{ site.baseurl }}/documents/scripting-memory.html#the-memoryslice-class) for details.

## RamBankCount

```
public int RamBankCount { get; }
```

Gets the number of RAM banks available.

## Keyboard

```
public KeyboardEmulator Keyboard { get; }
```

You can access the state of the virtual keyboard through this class. You can also
use it to emulate keystrokes. See [KeyboardEmulator]({{ site.baseurl }}/documents/scripting-vms.html#the-keyboardemulator-class) for details.

## ScreenConfiguration

```
public ScreenConfiguration ScreenConfiguration { get; }
```

Allows access to the details of the screen configuration that determine the tacts 
of screen rendering, such as number of raster lines, non-visible screen area, border pixels
above and below the display area, and many more. Check [`ScreenConfiguration`]({{ site.baseurl }}/documents/scripting-screen.html#the-screenconfiguration-class) 
for all available properties of the configuration object.

## ScreenRenderingTable

```
public ScreenRenderingTable ScreenRenderingTable { get; }
```

The virtual machine uses a table that contains an item for each screen
rendering tact. You can access the information about an individual tact
through this porperty. The [`ScreenRenderingTable`]({{ site.baseurl }}/documents/scripting-screen.html#the-screenrenderingtable-class) contains more
details about the information you can access.

## ScreenBitmap

```
public ScreenBitmap ScreenBitmap { get; }
```

Provides access to each individual pixels of the visible screen. This object
represents not only the display area (the 256 x 192 pixels for a ZX Spectrum 48K 
model), but also the border area. To check the dimensions, use the properties of
[`ScreenConfiguration`]({{ site.baseurl }}/documents/scripting-screen.html#the-screenconfiguration-class). The reference documentation of
[`ScreenBitmap`]({{ site.baseurl }}/documents/scripting-screen.html#the-screenbitmap-class) offers more details about addressing the pixels
within this object.

## ScreenRenderingStatus

```
public ScreenRenderingStatus ScreenRenderingStatus { get; }
```

Provides information about the current screen rendering status. 
See [`ScreenRenderingStatus`]({{ site.baseurl }}/documents/scripting-screen.html#the-screenrenderingstatus-class) for more details.

## BeeperConfiguration

```
public IAudioConfiguration BeeperConfiguration { get; }
```

Gets the configuration of the beeper device (such as sampling frequency, and others).
[`IAudioConfiguration`]({{ site.baseurl }}/documents/scripting-audio.html#the-iaudioconfiguration-interface) provides more details.

## BeeperSamples

```
public AudioSamples BeeperSamples { get; }
```

Gets the beeper samples of the current screen rendering frame. The 
[`AudioSamples`]({{ site.baseurl }}/documents/scripting-audio.html#the-audiosamples-class) type describes how you can access individual samples.

## SoundConfiguration

```
public IAudioConfiguration SoundConfiguration { get; }
```

Gets the configuration of the PSG (AY-3-8912 chip) sound device (such as sampling frequency, and others).
[`IAudioConfiguration`]({{ site.baseurl }}/documents/scripting-audio.html#the-iaudioconfiguration-interface) provides more details.

## AudioSamples

```
public AudioSamples AudioSamples { get; }
```

Gets the PSG (AY-3-8912 chip) sound samples of the current screen rendering frame. The 
[`AudioSamples`]({{ site.baseurl }}/documents/scripting-audio.html#the-audiosamples-class) type describes how you can access individual samples.

## Breakpoints

```
public CodeBreakpoints Breakpoints { get; }
```

This property allows you to manage the breakpoints. When you run the virtual machine
in debug mode (see the `StartDebug()` method), the virtual machine is paused whenever
it reaches a breakpoint. The [`CodeBreakpoints`]({{ site.baseurl }}/documents/scripting-vms.html#the-codebreakpoints-class) documentation gives 
you more clues.

## TimeoutInMs

```
public long TimeoutInMs { get; set; }
```

You can start the virtual machine with a timeout value. When the specified
timeout expires, the virtual machine pauses. With the `TimeoutInMs` property,
you can set up this value given in milliseconds.

> Internally, the virtual machine transforms this value into CPU T-states according to
> the clock frequency.

## TimeoutTacts

```
public long TimeoutTacts { get; set; }
```

With this property, you can specify the timeout value in CPU T-states. Providing the CPU uses
a
3.5 MHz clock, setting `TimeoutTacts` to 3500 equals to set `TimeoutInMs` to 1.

> Specifying zero value for `TimeoutInMs` or `TimeoutTacts` means that there's no explicit
> timeout, thus the virtual machine runs until explicitly stopped or paused in the code.

## RealTimeMode

```
public bool RealTimeMode { get; set; }
``` 

Normally, scripting runs the virtual machine in quick mode, so it continuously runs the
CPU and renders the screen. It does not sync the machine with the screen rendering
frequency. If you set this property to true, the virtual machine will run with the same speed
as a physical machine would do.

## DisableScreenRendering

```
public bool DisableScreenRendering { get; set; }
```

Setting this property to true would disable the screen rendering, and so the virtual
machine would faster. Nonetheless, in this mode &mdash; as the screen is not remdered
&mdash; you cannot use `ScreenBitmap` to check the contents of the rendered frame. 
After the machine starts, the contents remains the same.

## MachineState

```
public VmState MachineState { get; private set; }
```

This property allows you to check the current state of the virtual machine.
Take a look at the [`VmState`]({{ site.baseurl }}/documents/scripting-vms.html#the-vmstate-enum) enumeration for details about the
possible states.

## ExecutionCompletionReason

```
public ExecutionCompletionReason ExecutionCompletionReason { get; }
```

The virtual machine can go into paused or stopped state for many reasons.
The [`ExecutionCompletionReason`]({{ site.baseurl }}/documents/scripting-vms.html#the-executioncompletionreason-enum) enumeration allows you to check what
event has caused that the machine is in a specific state.

## RunsInDebugMode

```
public bool RunsInDebugMode { get; }
```

Indicates if the virtual machine runs in debug mode.

## CompletionTask

```
public Task CompletionTask { get; }
```

After th virtual machine has been started, it runs in a background thread.
While it runs, you can use the UI thread for other activities. 
The `CompletionTask` property can be used to wait for the completion of
the virtual machine (it gets paused or stopped).

For example, this sample starts the virtual machine id debug mode, does
some other activity on the UI thread, and then wait while the machine reaches
its firs breakpoint:

```
var sm = SpectrumVmFactory.CreateSpectrum48Pal();
// --- Set up breakpoints (omitted from code)
sm.StartDebug();
// --- Do something on UI thread (omitted from code)
await sm.CompletionTask;
// --- Go on on UI thread
```

## CachedVmStateFolder

```
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

## IsFirstStart

```
public bool IsFirstStart { get; }
```
Signs that this is the very first start of the virtual machine.

## IsFirstPause

```
public bool IsFirstPause { get; }
```
Signs that this is the very first pause of the virtual machine after its first start.

## Start()

```
public void Start();
```

Starts the virtual machine in continuous execution mode. It runs unless the script pauses or
stops it, or the specified timeout expires.

## Start(ExecuteCycleOptions)

```
public void Start(ExecuteCycleOptions options)
```

Starts the virtual machine with the specified execution mode (see also [ExecuteCycleOptions]({{ site.baseurl }}/documents/scripting-vms.html#the-executecycleoptions-class)).
The machine runs unless the script pauses or stops it, or the specified timeout expires.

## StartDebug()

```
public void StartDebug()
```

Starts the virtual machine in debug mode and sets the `RunsInDebugMode` property to true. Whenever the execution flow reaches a breakpoint, the machine gets
paused. Additionally, the machine runs unless the script pauses or stops it, or the specified timeout expires.

## RunUntilHalt()

```
public void RunUntilHalt();
```

Starts the Spectrum machine and runs it on a background thread until it reaches a HALT instruction, then is gets
paused. Otherwise, the machine runs unless the script pauses or stops it, or the specified timeout expires.

## RunUntilFrameCompletion()

```
public void RunUntilFrameCompletion()
```

Starts the Spectrum machine and runs it on a background thread until the current screen rendering frame is 
completed. Then it gets paused. Otherwise, the machine runs unless the script pauses or stops it, or 
the specified timeout expires.

## RunUntilTerminationPoint(ushort, int)

```
public void RunUntilTerminationPoint(ushort address, int romIndex = 0)
```

Starts the Spectrum machine and runs it on a background thread until the CPU reaches the specified 
termination point (address). At that point the machine gets paused. Otherwise, the machine runs unless 
the script pauses or stops it, or the specified timeout expires.

### Arguments

`address`: The address at which to pause the machine  
`romIndex`: If the address is within the ROM, romIndex specified the index of the ROM that should 
be selected to pause the machine.

### Sample

You can start and run a ZX Spectrum 48K virtual machine while it reaches the main execution cycle
point (`#12A9`) in the ROM:

```
var sm = SpectrumVmFactory.CreateSpectrum48Pal();
sm.RunUntilTerminationPoint(0x12A9);
await sm.CompletionTask;
```

## StartAndRunToMain(bool)

```
public async Task StartAndRunToMain(bool spectrum48Mode = false)
```

A convenience method that starts the virtual machine and pauses it when it reaches its main
execution cycle. The method can be used to wait while the machine pauses.

### Arguments

`spectrum48Mode`: Setting this flag to true, ZX Spectrum 128K and later models are started in
Spectrum 48 BASIC mode 

### Remarks

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

### Sample

This code snippet starts a ZX Spectrum 128K virtual machine in 48 mode, and waits while the 
machine reaches the main execution cycle.

```
var sm = SpectrumVmFactory.CreateSpectrum128Pal();
await sm.StartAndRunToMain(true);
```

## Pause()

```
public async Task Pause()
```

Pauses the running Spectrum virtual machine. After it is paused, you can continue the execution
with any of these methods: `Start()`, `Start(ExecutionCycleOptions)`, `StartDebug`, 
`RunUntilHalt()`, `RunUntilFrameCompletion()`, `RunUntilTerminationPoint()`.

## Stop()

```
public async Task Stop()
```

Stops the running or paused Spectrum virtual machine. After it is stopped, you can resttart 
the execution with any of these methods: `Start()`, `Start(ExecutionCycleOptions)`, `StartDebug`, 
`RunUntilHalt()`, `RunUntilFrameCompletion()`, `RunUntilTerminationPoint()`.

## StepInto()

```
public void StepInto()
```

Executes the subsequent Z80 instruction, and then pauses the machine. As the execution happens 
on a background thread, you should wait for the completion with the `CompletionTask` property:

```
var sm = SpectrumVmFactory.CreateSpectrum48Pal();
// --- Do something with the machine and then pause (code omitted)
sm.StepInto();
await sm.CompletionTask;
```

## StepOver()

```
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

# Machine state management methods

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

## SaveMachineStateTo(string)

```
public void SaveMachineStateTo(string filename)
```

Saves the state of the paused virtual machine to the specified file.

### Arguments
`filename`: Name of the file to save the machine state

## RestoreMachineState(string)

```
public void RestoreMachineState(string filename)
```

Loads the state of the paused or stopped machine from the specified file. When you continue the 
execution, the machine will start from the freshly loaded state.

### Arguments
`filename`: Name of the file that contains the machine state

# Code manipulation methods

While the virtual machine is stopped, you can inject code into the memory so that you can execute it.
You do not need to use the [Memory] property of `SpectrumVm` to do this. There are a few methods
that makes it extremely easy.

## InjectCode(ushort, byte array)

```
public void InjectCode(ushort address, byte[] codeArray)
```

Injects code into the memory. Keeps the machine in paused state.

### Arguments
`address`: Start address of the injection  
`codeArray`: A byte array that contains the machine code bytes in the order of injection

## InjectCode(string, AssemblerOptions)

```
public ushort InjectCode(string asmSource, AssemblerOptions options = null)
```

This powerful method allows you to define the code to inject, in Z80 assembler language, using
all features the language provides. In the __SpectNetIde__ assembler, you can create multiple 
code segments, and this method injects all cade segments.

### Arguments
`asmSource`: The Z80 Assembler source code  
`options`: Optional options to compile the source code. See [`AssemblerOptions`]({{ site.baseurl }}/documents/scripting-vms.html#the-assembleroptions-class) more details.

### Returns

The address marked as the entry point (`ENT` directive in the source code), or the start 
address (`ORG` directive) of the first code block in the source code.

## CallCode(ushort, ushort?)

```
public void CallCode(ushort startAddress, ushort? callStubAddress = null)
```

Calls the code at the specified subroutine start address.

### Arguments
`startAddress`: Start address of the subroutine  
`callStubAddress`: Optional address for the call stub

### Remarks

This method creates a Z80 CALL instruction, and stores it to the specified `callStubAddress`.
If this parameter is omitted (set to `null`), the `#5BA0` address (empty area after system variables)
is used. Then, the virtual machine starts and pauses at the termination point of 
`callStubAddress + 3` (by default, `#5BA3`).

## Sample using code injection

The following sample injects and executes code that sets the color of the background to blue.

```
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

```
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

# Virtual machine events

The virtual machine provides events you can use to examine and analyze the state of the machine.

## VmStateChanged

```
public event EventHandler<VmStateChangedEventArgs> VmStateChanged;
```

This event is raised whenever the state of the virtual machine changes.

The `VmStateChangedEventArgs` class has these properties:

```
public VmState OldState { get; }
public VmState NewState { get; }
```

As theor names suggest, you can obtain the old and the new state ([`VmState`]({{ site.baseurl }}/documents/scripting-vms.html#the-vmstate-enum)) of the machine.

## VmStoppedWithException

```
public event EventHandler<VmStoppedWithExceptionEventArgs> VmStoppedWithException;
```

Whenever the Spectrum virtual machine stops because of an exception, this event is raised. Normally,
this event should not happen, it signs that there's some unexpected issue (programming error) within
__SpectNetIde__. The `VmStoppedWithExceptionEventArgs` has this property to check what is the exception
that caused the machine stop:

```
public Exception Exception { get; }
```

## VmFrameCompleted

```
public event EventHandler VmFrameCompleted;
```

To sign that the virtual machine just has completed a new screen rendering frame, this event is raised.

# The AssemblerOptions class

This class represents the options that can be used when the Z80 Assembler compiles
the source code to machine code.

__Namespace__: `Spect.Net.Assembler.Assembler`  
__Assembly__: `Spect.Net.Assembler`

```
public class AssemblerOptions
```

## PredefinedSymbols

```
public List<string> PredefinedSymbols { get; }
```

Predefined compilation symbols that can be checked with the `#ifdef`, `#ifndef`, and other
directives.

## DefaultStartAddress

```
public ushort? DefaultStartAddress { get; set; }
```

The default start address of the compilation. If there's no `ORG` pragma specified in 
the source code, this address is used.

## DefaultDisplacement

```
public int? DefaultDisplacement { get; set; }
```

The default displacement of the compilation. If there's no `ORG` pragma specified in 
the source code, this address is used. If set to null, no displacement is used.

## CurrentModel

```
public SpectrumModelType CurrentModel { get; set; }
```

Specifies the Spectrum model to use in the source code. By default it is set to Spectrum 48K. 
The `SpectrumModelType` enumeration has these values:

Value | Machine type
------|-------------
`Spectrum48` | ZX Spectrum 48K
`Spectrum128` | ZX Spectrum 128K
`SpectrumP3` | ZX Spectrum +3E
`Next` | ZX Spectrum Next

# The CodeBreakpoints class

Represents the breakpoint of the Spectrum virtual machine, at which execution should be
paused when running in debug mode.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public sealed class CodeBreakpoints
```

## Count

```
public int Count { get; }
```

The number of breakpoint defined.

## AddBreakpoint(ushort)

```
public void AddBreakpoint(ushort address)
```

Adds a breakpoint for the specified _`address`_.

## RemoveBreakpoint(ushort)

```
public void RemoveBreakpoint(ushort address)
```

Removes the breakpoint from the specified _`address`_.

## ClearAllBreakpoints()

```
public void ClearAllBreakpoints()
```

Clears all previously declared breakpoints.

## HasBreakpointAt(ushort)

```
public bool HasBreakpointAt(ushort address)
```

Checks if there is a breakpoint definied for the given _`address`_.

# The ExecuteCycleOptions class

This class provides options for the execution cycle of the Spectrum virtual machine. When you start
the cycle, you can pass execution options that influence the machine cycle and specifies when the
machine should be paused.

__Namespace__: `Spect.Net.SpectrumEmu.Machine`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public class ExecuteCycleOptions
```

## EmulationMode

```
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

## DebugStepMode

```
public DebugStepMode DebugStepMode { get; }
```

When the emulation mode is set to `Debugger`, this property specifies the mode to run a debug-mode
execution cycle. The values of `DebugStepMode`:

Value | Description
------|------------
`StopAtBreakpoint` | Execution stops at the next breakpoint
`StepInto` | Execution stops after the next instruction
`StepOver` | Execution stops after the next instruction. If that should be a subroutine call or a block statement, the execution stops after returning from the subroutine or completing the block statement.

## FastTapeMode

```
public bool FastTapeMode { get; }
```

Indicates if fast loading from the tape is allowed.

## TerminationRom
 
```
public int TerminationRom { get; }
```

The index of the ROM when a termination point is defined.

## TerminationPoint

```
public ushort TerminationPoint { get; }
```

The value of the PC register to reach when `EmulationMode` is set to `UntilExecutionPoint`.

## SkipInterruptRoutine

```
public bool SkipInterruptRoutine { get; }
```

Signs if the instructions within the maskable interrupt routine should be skipped during debugging.

## FastVmMode

```
public bool FastVmMode { get; }
```

This flag indicates that the virtual machine should run in hidden mode (no screen, no sound, no delays).

## DisableScreenRendering

```
public bool DisableScreenRendering { get; }
```

This flag shows whether the virtual machine should render the screen when runs in `FastVmMode`.

## TimeoutTacts

```
public long TimeoutTacts { get; }
```

You can specify a timeout value (given in CPU tacts). If this is set to zero, no timeout is applied.
If set to a value greater than zero, after the specified number of CPU cycles ellapsed, the execution of
the virtual machine is paused.

# The ExecutionCompletionReason enum

The values of this enumeration tells the reason the virtual machine is in paused or in stopped state.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public enum ExecutionCompletionReason
```

Value | Description
------|------------
`None` | The machine is still executing, or it has not been ever started
`Cancelled` | The execution has explicitly cancelled by the user or by the scripting code
`Timeout` | The specified timeout period expired
`TerminationPointReached` | The virtual machine reached its termintation point specified by its start
`BreakpointReached` | The virtual machine reached a breakpoint during its execution in debug mode
`Halted` | The virtaul machine reached a `HALT` statement and paused
`FrameCompleted` | The virtual machine has just rendered a new screen frame and paused
`Exception` | The virtual machine stopped because of an unexpected exception

# The KeyboardEmulator class

This class is reserved for future extension. Right now, id does not offer any property or method for scripting.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public sealed class KeyboardEmulator
```

# The VmState enum

The values of this enumeration show the possible states of a ZX Spectrum virtual machine.

Value | Description
------|------------
`None` | The virtual machine has just been created, but has not run yet
`Runnig` | The virtual machine is successfully started in the background
`Pausing` | The pause request has been sent to the virtual machine, now it prepares to get paused
`Paused` | The virtual machine has been paused
`Stopping` | The stop request has been sent to the virtual machine, now it prepares to get stopped
`Stopped` | The virtual machine has been stopped


# The SpectrumModels class

This class is a repository of all Spectrum models and editions supported 
by __SpectNetIde__.

__Namespace__: `Spect.Net.SpectrumEmu`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public static class SpectrumModels
```

## The StockModels property

You can use the `StockModels` property to access the dictionary of
available ZX Spectrum models.

```
public static IReadOnlyDictionary<string, SpectrumModelEditions> StockModels;
```

Each model has a unique name to look up the associated editions in the dictionary.
A `SpectrumModelEditions` instance is a collection of all editions of the 
particular model. A single edition is represented by a `SpectrumModelEdition` instance.

## Constants used for models and editions

The `SpectrumModels` class provides these constants for the unique model names and editions:

Constant value | Description
---------------|------------
`ZX_SPECTRUM_48` | `"ZX Spectrum 48K"`
`ZX_SPECTRUM_128` | `"ZX Spectrum 128K"`
`ZX_SPECTRUM_P3_E` | `"ZX Spectrum +3E"`
`ZX_SPECTRUM_NEXT` | `"ZX Spectrum Next"`
`PAL` | `"PAL"`
`NTSC` | `"NTSC"`
`PAL_2_X` | `"PAL2X"`
`NTSC_2_X` | `"NTSC2X"`

## Model access

`SpectrumModels` provides properties to directly access the 
`SpectrumModelEdition` instances of frequently used machine types:

```
public static SpectrumEdition ZxSpectrum48Pal;
public static SpectrumEdition ZxSpectrum48Ntsc;
public static SpectrumEdition ZxSpectrum48Pal2X;
public static SpectrumEdition ZxSpectrum48Ntsc2X;
public static SpectrumEdition ZxSpectrum128Pal;
public static SpectrumEdition ZxSpectrumP3EPal;
public static SpectrumEdition ZxSpectrumNextPal;
```

These properties are associated with these model and edition key constants:

Property | Model key | Edition key
---------|-----------|------------
`ZxSpectrum48Pal` | `ZX_SPECTRUM_48` | `PAL`
`ZxSpectrum48Ntsc` | `ZX_SPECTRUM_48` | `NTSC`
`ZxSpectrum48Pal2X` | `ZX_SPECTRUM_48` | `PAL2X`
`ZxSpectrum48Ntsc2X` | `ZX_SPECTRUM_48` | `NTSC2X`
`ZxSpectrum128Pal` | `ZX_SPECTRUM_128` | `PAL`
`ZxSpectrumP3EPal` | `ZX_SPECTRUM_128` | `PAL`
`ZxSpectrumNext` | `ZX_SPECTRUM_NEXT` | `PAL`