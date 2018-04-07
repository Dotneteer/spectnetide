# MemoryPagingInfo class

This class provides properties and methods to obtain information about ROM and RAM paging.
Though it works with ZX Spectrum 48K model, it provides benefits only for higher models.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public sealed class MemoryPagingInfo
```

### SelectedRomIndex

```CSharp
public int SelectedRomIndex { get; }
```

Gets the index of the currently selected ROM.

### SelectedRamBankIndex

```CSharp
public int SelectedRamBankIndex { get; }
```

Gets the index of the selecter RAM bank for the `#C000`-`#FFFF` address range.

### GetBankIndexForSlot(int)

```CSharp
public int GetBankIndexForSlot(int slot)
```

Gets the RAM bank index for the specified _`slot`_.

> There are four slots, for the pages starting at `#0000`, `#4000`, `#8000`, and `#C000`, respectively.
> Slot index should be set accordingly, from `0` to `3`.

### UsesShadowScreen

```CSharp
public bool UsesShadowScreen { get; }
```

Indicates if the virtual machine uses the shadow screen (Bit 3 of port #7FFD). If false, the normal
screen (Bank #5) is used. If true, the shadow screen (Bank #7) is displayed.

### IsInAllRamMode

```CSharp
public bool IsInAllRamMode { get; }
```

Indicates if the virtul machine (ZX Spectrum +3E and ZX Spectrum Next) machine is
in the special ALL RAM mode (Bit 0 of port #1FFD).

