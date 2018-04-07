# MemorySlice class

This class represents a slice of the memory in the Spectrum virtual machine. Right now, it is used to
represent the contents of a RAM bank.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public sealed class MemorySlice
```

### Size

```CSharp
public int Size { get; }
```

The size of the memory slice, given in number of bytes.

### this[int]

```CSharp
public byte this[int index] { get; set; }
```

You can get and set the byte at the _`index`_ position in the memory slice.

> Be aware, _`index`_ is not the absolute memory address of the byte, but a relative address from `0` to
> `Size - 1`. For RAM banks, `Size` is `#4000`, thus you can address bytes from `0` to `#3FFF`.

