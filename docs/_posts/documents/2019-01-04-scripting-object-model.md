---
layout: documents
categories: 
  - "Scripting"
title:  "Scripting Object Model"
alias: scripting-object-model
seqno: 20
selector: documents
permalink: "documents/scripting-object-model"
---

__SpectNetIde__ offers a set of .NET objects that you can use to create scripts that automate
everyday tasks with ZX Spectrum virtual machines. You can use these objects in any .NET Framework
application, or in scripting languages that allow accessing .NET Framework objects.

To use __SpectNetIde__ scripting, add these assemblies to your projects:

Assembly | Description
---------| -----------
`Spect.Net.Assembler` | The Z80 Assembler of __SpectNetIde__. With the help of this component, you can use Z80 source code in scripts.
`Spect.Net.RomResources` | This assembly stores the ROMs of the Spectrum models supported by __SpectNetIde__.
`SpectNet.SpectrumEmu` |  This assembly implements the ZX Spectrum emulator and the scripting object model.

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

The table below contains a summary of the .NET types that constitute the ZX Spectrum scripting
object model. To get the shortest path to learn using them, I suggest starting with these documentations:
* [`SpectrumVmFactory`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrumvmfactory-class)
* [`SpectrumVm`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrumvm-class)
* [`CpuZ80`]({{ site.baseurl }}/documents/scripting-z80.html#the-cpuz80-class)

Object type | Description
------------|------------
[`AddressTrackingState`]({{ site.baseurl }}/documents/scripting-z80.html#the-addresstrackingstate-class) | This class represents tracking information regarding memory.
[`AssemblerOptions`]({{ site.baseurl }}/documents/scripting-vms.html#the-assembleroptions-class) | This class represents the options that can be used when the Z80 Assembler compiles the source code to machine code.
[`AudioSamples`]({{ site.baseurl }}/documents/scripting-audio.html#the-audiosamples-class) | This class stores audio samples for the currently rendered virtual machine frame.
[`CodeBreakpoints`]({{ site.baseurl }}/documents/scripting-vms.html#the-codebreakpoints-class) | Represents the breakpoint of the Spectrum virtual machine, at which execution should pause when running in debug mode.
[`CpuZ80`]({{ site.baseurl }}/documents/scripting-z80.html#the-cpuz80-class) | This class represents the Z80 CPU of a Spectrum virtual machine.
[`ExecuteCycleOptions`]({{ site.baseurl }}/documents/scripting-vms.html#the-executecycleoptions-class) | This class provides options for the execution cycle of the Spectrum virtual machine.
[`ExecutionCompletionReason`]({{ site.baseurl }}/documents/scripting-vms.html#the-executioncompletionreason-enum) | The values of this enumeration tell the reason why the virtual machine is in a paused or stopped state.
[`IAudioConfiguration`]({{ site.baseurl }}/documents/scripting-audio.html#the-iaudioconfiguration-interface) | This interface represents the configuration of the beeper/sound device.
[`KeyboardEmulator`]({{ site.baseurl }}/documents/scripting-vms.html#the-keyboardemulator-class) | This class is reserved for future extension. Right now, id does not offer any property or method for scripting.
[`MemoryPagingInfo`]({{ site.baseurl }}/documents/scripting-memory.html#the-memorypaginginfo-class) | This class provides properties and methods to obtain information about ROM and RAM paging.
[`MemorySlice`]({{ site.baseurl }}/documents/scripting-memory.html#the-memoryslice-class) | This class represents a slice of the memory in the Spectrum virtual machine.
[`ReadOnlyMemorySlice`]({{ site.baseurl }}/documents/scripting-memory.html#the-readonlymemoryslice-class) | This class represents a read-only slice of the memory in the Spectrum virtual machine.
[`ScreenBitmap`]({{ site.baseurl }}/documents/scripting-screen.html#the-screenbitmap-class) | This class represents the current screen's pixels, including the border area.
[`ScreenConfiguration`]({{ site.baseurl }}/documents/scripting-screen.html#the-screenconfiguration-class) | This class represents the configuration of the virtual machine's screen
[`ScreenRenderingStatus`]({{ site.baseurl }}/documents/scripting-screen.html#the-screenrenderingstatus-class) | Provides properties about the current screen rendering status of the machine
[`ScreenRenderingTable`]({{ site.baseurl }}/documents/scripting-screen.html#the-screenrenderingtable-class) | Represents the screen rendering table of the virtual machine.
[`ScreenRenderingTact`]({{ site.baseurl }}/documents/scripting-screen.html#the-screenrenderingtact-class) | Provides details about a screen rendering tact.
[`SpectrumMemoryContents`]({{ site.baseurl }}/documents/scripting-memory.html#the-spectrummemorycontents-class) | This class provides access to the addressable 64KBytes memory contents of the Spectrum virtual machine.
[`SpectrumModels`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrummodels-class) | This class is a repository of all Spectrum models and editions supported by __SpectNetIde__.
[`SpectrumVm`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrumvm-class) | This class represents a Spectrum virtual machine.
[`SpectrumVmFactory`]({{ site.baseurl }}/documents/scripting-vms.html#the-spectrumvmfactory-class) | This class provides methods you can use to create ZX Spectrum virtual machine instances.
[`VmState`]({{ site.baseurl }}/documents/scripting-vms.html#the-vmstate-enum) | The values of this enumeration show the possible states of a ZX Spectrum virtual machine.
[`Z80InstructionExecutionEventArgs`]({{ site.baseurl }}/documents/scripting-z80.html#the-z80instructionexecutioneventargs-class) | This class provides event arguments the the `OperationExecuting` and `OperationExecuted` events of the `CpuZ80` class

