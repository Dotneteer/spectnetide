# ScreenRenderingTable class

Represents the screen rendering table of the virtual machine

__Namespace__: `Spect.Net.SpectrumEmu.Devices.Screen`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public sealed class ScreenRenderingTable
```

### Count

```CSharp
public int Count { get; }
```

Gets the number of items in the screen rendering table.

### this[int]

```CSharp
public ScreenRenderingTact this[int index] { get; }
```

Gets the entry of the screen rendering table specified by _`index`_. See more details
in the description of [`ScreenRenderingTact`](ScreenRenderingTact).
