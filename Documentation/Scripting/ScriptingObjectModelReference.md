# Scripting object model reference

__SpectNetIde__ offers a set of .NET objects that you can use to create scripts that automate
common tasks with ZX Spectrum virtual machines. You can use these objects in any .NET Framework
application, or in scripting languages that allow accessing .NET Framweork objects.

To use __SpectNetIde__ scripting, add these assembies to your projects:

Assembly | Description
---------| -----------
`Spect.Net.Assembler` | The Z80 Assembler of __SpectNetIde__. With the help of this component, you can use Z80 source code in scripts.
`Spect.Net.RomResources` | This assembly stores the ROMs of the Spectrum models supported by __SpectNetIde__
`SpectNet.SpectrumEmu` |  This assembly implements the ZX Spectrum emulator and the scripting object model

## ZX Spectrum virtual machines

The scripting object model supports these ZX Spectrum models:
* ZX Spectrum 48K
* ZX Spectrum 128K
* ZX Spectrum +3E
* _ZX Spectrum Next (still in development)_

The virtual machines use a background thread for their execution cycle. You can run the machine
with synchronous methods, but you need to pause or stop them, or waiting while their cycle completes
with the `async/await` pattern.

## Scripting objects

The table below contains a brief summary of the .NET types that constitute the ZX Spectrum scripting
object model. To get the shortest path to learn using them, I suggest starting with these reference
documentations:
* [`SpectrumVmFactory`](SpectrumVmFactory.md)
* [`SpectrumVm`](SpectrumVm.md)
* [`CpuZ80`](CpuZ80.md)

Object type | Description
------------|------------
[`AddressTrackingState`](AddressTrackingState.md) | This class represents tracking information regarding memory
[`AssemblerOptions`](AssemblerOptions.md) | This class represents the options that can be used when the Z80 Assembler compiles the source code to machine code
[`AudioSamples`](AudioSamples.md) | This class stores audio samples for the currently rendered virtual machine frame
[`CodeBreakpoints`](CodeBreakpoints.md) | Represents the breakpoint of the Spectrum virtual machine, at which execution should be paused when running in debug mode
[`CpuZ80`](CpuZ80.md) | This class represents the Z80 CPU of a Spectrum virtual machine
[`ExecuteCycleOptions`](ExecuteCycleOptions.md) | This class provides options for the execution cycle of the Spectrum virtual machine
[`ExecutionCompletionReason`](ExecutionCompletionReason.md) | The values of this enumeration tells the reason the virtual machine is in paused or in stopped state
[`IAudioConfiguration`](IAudioConfiguration.md) | This interface represents the configuration of the beeper/sound device
[`KeyboardEmulator`](KeyboardEmulator.md) | This class is reserved for future extension. Right now, id does not offer any property or method for scripting.
[`MemoryPagingInfo`](MemoryPagingInfo.md) | This class provides properties and methods to obtain information about ROM and RAM paging
[`MemorySlice`](MemorySlice.md) | This class represents a slice of the memory in the Spectrum virtual machine
[`ReadOnlyMemorySlice`](ReadOnlyMemorySlice.md) | This class represents a read-only slice of the memory in the Spectrum virtual machine
[`ScreenBitmap`](ScreenBitmap.md) | This class represents the current screen's pixels, including the border
[`ScreenConfiguration`](ScreenConfiguration.md) | This class represents the configuration of the virtual machine's screen
[`ScreenRenderingStatus`](ScreenRenderingStatus.md) | Provides properties about the current screen rendering status of the machine
[`ScreenRenderingTable`](ScreenRenderingTable.md) | Represents the screen rendering table of the virtual machine
[`ScreenRenderingTact`](ScreenRenderingTact.md) | Provides details about a screen rendering tact
[`SpectrumMemoryContents`](SpectrumMemoryContents.md) | This class provides access to the addressable 64KBytes memory contents of the Spectrum virtual machine
[`SpectrumModels`](SpectrumModels.md) | This class is a repository of all Spectrum models and editions supported by __SpectNetIde__.
[`SpectrumVm`](SpectrumVm.md) | This class represents a Spectrum virtual machine
[`SpectrumVmFactory`](SpectrumVmFactory.md) | This class provides methods you can use to create ZX Spectrum virtual machine instances
[`VmState`](VmState.md) | The values of this enumeration show the possible states of a ZX Spectrum virtual machine
[`Z80InstructionExecutionEventArgs`](Z80InstructionExecutionEventArgs.md) | This class provides event arguments the the `OperationExecuting` and `OperationExecuted` events of the `CpuZ80` class










