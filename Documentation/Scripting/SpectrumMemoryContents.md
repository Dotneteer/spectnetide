# SpectrumMemoryContents class

This class provides access to the addressable 64KBytes memory contents of the Spectrum virtual machine.
An instance of the class allows reading and writeng the memory.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public sealed class SpectrumMemoryContents
```

### this[ushort]

```CSharp
public byte this[ushort address] { get; set; }
```

Gets or sets the contents of the specified _`address`_. Setting the content behaves as
in the real hardware: if you try to set a ROM address, the contents of the particular
memory address won't change after the write operation.

### ReadTrackingState

```CSharp
public AddressTrackingState ReadTrackingState { get; }
```

The property's value is an [`AddressTrackingState`](AddressTrackingState.md) 
instance. It provides a bit for each memory address in the `#0000`-`#FFFF` range to check if the 
particular byte in the memory has been read during a CPU operation.

### ResetReadTracking()

```CSharp
public void ResetReadTracking()
```

Resets the `ReadTrackingState` property as if no memory had been read.

### WriteTrackingState

```CSharp
public AddressTrackingState WriteTrackingState { get; }
```

The property's value is an [`AddressTrackingState`](AddressTrackingState.md) 
instance. It provides a bit for each memory address in the `#0000`-`#FFFF` range to check if the 
particular byte in the memory has been written during a CPU operation.

### ResetWriteTracking()

```CSharp
public void ResetWriteTracking()
```

Resets the `WriteTrackingState` property as if no memory had been read.
