# SpectrumVm class

This class represents a Spectrum virtual machine. You cannot instantiate a machine
with the constructor of `SpectrumVm`, for it is not accessible. To create a Spectrum machine,
use the [`SpectrumVmFactory`](SpectrumVmFactory) class.

`SpectrumVm` is a pivotal type in the scripting feature of __SpectNetIde__. When you have an
object instance in your code, it provides a plethora of properties and methods to access and control
the Spectrum virtual machine.

## Contents at a glance

* [Machine properties]
  * [ModelKey]
  * [EditionKey]
  * [Cpu]
  * [Roms]
  * [RomCount]

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


