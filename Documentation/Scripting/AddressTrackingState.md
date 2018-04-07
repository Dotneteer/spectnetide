# AddressTrackingState class

This class represents tracking information in regard to memory. The scripting engine 
([`SpectrumVm`](SpectrumVm) and [`CpuZ80`](CpuZ80) classes) use this type to track 
operation execution, memory reads and memory writes.

This class stores a flag for each memory address in the `#0000`-`#FFFF` range. A false value 
means that the tracking event has not been signed for that particular address. The true value
says that the tracking event has happened.

For example, when you start the Spectrum virtual machine, after executing the first 
instruction (`DI`), the flag for #0000 is set to true, but all the others remains false.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public sealed class AddressTrackingState
```

### this[ushort]

```CSharp
public bool this[ushort address] { get; }
```

Gets the status bit of the specified _`address`_.

### TouchedAll(ushort, ushort)

```CSharp
public bool TouchedAll(ushort startAddr, ushort endAddr)
```

Checks if all tracking flag is set between _`startAddr`_ and _`endAddr`_.
Both addresses are inclusive. Returns true, if all flag is set; otherwise, false.

### TouchedAny(ushort, ushort)

```CSharp
public bool TouchedAny(ushort startAddr, ushort endAddr)
```

Checks if any tracking flag is set between _`startAddr`_ and _`endAddr`_ at all.
Both addresses are inclusive. Returns true, if any of the flags is set; otherwise, false.

