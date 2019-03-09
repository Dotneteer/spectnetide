---
layout: documents
categories: 
  - "Scripting"
title:  "Scripting the Memory"
alias: scripting-memory
seqno: 60
selector: documents
permalink: "documents/scripting-memory"
---

# The MemoryPagingInfo class

This class provides properties and methods to obtain information about ROM and RAM paging.
Though it works with ZX Spectrum 48K model, it provides benefits only for higher models.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public sealed class MemoryPagingInfo
```

## SelectedRomIndex

```
public int SelectedRomIndex { get; }
```

Gets the index of the currently selected ROM.

## SelectedRamBankIndex

```
public int SelectedRamBankIndex { get; }
```

Gets the index of the selecter RAM bank for the `#C000`-`#FFFF` address range.

## GetBankIndexForSlot(int)

```
public int GetBankIndexForSlot(int slot)
```

Gets the RAM bank index for the specified _`slot`_.

> There are four slots, for the pages starting at `#0000`, `#4000`, `#8000`, and `#C000`, respectively.
> Slot index should be set accordingly, from `0` to `3`.

## UsesShadowScreen

```
public bool UsesShadowScreen { get; }
```

Indicates if the virtual machine uses the shadow screen (Bit 3 of port #7FFD). If false, the normal
screen (Bank #5) is used. If true, the shadow screen (Bank #7) is displayed.

## IsInAllRamMode

```
public bool IsInAllRamMode { get; }
```

Indicates if the virtul machine (ZX Spectrum +3E and ZX Spectrum Next) machine is
in the special ALL RAM mode (Bit 0 of port #1FFD).

# The MemorySlice class

This class represents a slice of the memory in the Spectrum virtual machine. Right now, it is used to
represent the contents of a RAM bank.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public sealed class MemorySlice
```

## Size

```
public int Size { get; }
```

The size of the memory slice, given in number of bytes.

## this[int]

```
public byte this[int index] { get; set; }
```

You can get and set the byte at the _`index`_ position in the memory slice.

> Be aware, _`index`_ is not the absolute memory address of the byte, but a relative address from `0` to
> `Size - 1`. For RAM banks, `Size` is `#4000`, thus you can address bytes from `0` to `#3FFF`.

# The ReadOnlyMemorySlice class

This class represents a read-only slice of the memory in the Spectrum virtual machine. Right now, it is used to
represent the contents of a ROM pages.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public sealed class ReadOnlyMemorySlice
```

## Size

```
public int Size { get; }
```

The size of the memory slice, given in number of bytes.

## this[int]

```
public byte this[int index] { get; }
```

You can get the byte at the _`index`_ position in the memory slice. As the name of the class suggest,
you cannot change the contents of a read-only memory slice.

> Be aware, _`index`_ is not the absolute memory address of the byte, but a relative address from `0` to
> `Size - 1`. For RAM banks, `Size` is `#4000`, thus you can address bytes from `0` to `#3FFF`.

# The SpectrumMemoryContents class

This class provides access to the addressable 64KBytes memory contents of the Spectrum virtual machine.
An instance of the class allows reading and writeng the memory.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public sealed class SpectrumMemoryContents
```

## this[ushort]

```
public byte this[ushort address] { get; set; }
```

Gets or sets the contents of the specified _`address`_. Setting the content behaves as
in the real hardware: if you try to set a ROM address, the contents of the particular
memory address won't change after the write operation.

## ReadTrackingState

```
public AddressTrackingState ReadTrackingState { get; }
```

The property's value is an [`AddressTrackingState`]({{ site.baseurl }}/documents/scripting-z80.html#the-addresstrackingstate-class) 
instance. It provides a bit for each memory address in the `#0000`-`#FFFF` range to check if the 
particular byte in the memory has been read during a CPU operation.

## ResetReadTracking()

```
public void ResetReadTracking()
```

Resets the `ReadTrackingState` property as if no memory had been read.

## WriteTrackingState

```
public AddressTrackingState WriteTrackingState { get; }
```

The property's value is an [`AddressTrackingState`]({{ site.baseurl }}/documents/scripting-z80.html#the-addresstrackingstate-class) 
instance. It provides a bit for each memory address in the `#0000`-`#FFFF` range to check if the 
particular byte in the memory has been written during a CPU operation.

## ResetWriteTracking()

```
public void ResetWriteTracking()
```

Resets the `WriteTrackingState` property as if no memory had been read.
