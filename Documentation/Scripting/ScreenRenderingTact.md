# ScreenRenderingTact class

Provides details about a screen rendering tact.

__Namespace__: `Spect.Net.SpectrumEmu.Devices.Screen`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public sealed class ScreenRenderingTact
```

### Phase

```CSharp
public ScreenRenderingPhase Phase { get; }
```

The rendering phase to be applied for the particular tact. The `ScreenRenderingPhase` enum
has these values:

Value | Description
------|------------
`None` | The ULA does not do any rendering
`Border` | The ULA sets the border color to display the current pixel.
`BorderFetchPixel` | The ULA sets the border color to display the current pixel. It prepares to display the fist pixel in the row with prefetching the corresponding byte from the display memory.
`BorderFetchPixelAttr` | The ULA sets the border color to display the current pixel. It has already fetched the 8 pixel bits to display. It carries on preparing to display the fist pixel in the row with prefetching the corresponding attribute byte from the display memory.
`DisplayB1` | The ULA displays the next two pixels of Byte1 sequentially during a single Z80 clock cycle.
`DisplayB1FetchB2` | The ULA displays the next two pixels of Byte1 sequentially during a single Z80 clock cycle. It prepares to display the pixels of the next byte in the row with prefetching the corresponding attribute from the display memory.
`DisplayB2` | The ULA displays the next two pixels of Byte2 sequentially during a single Z80 clock cycle.
`DisplayB2FetchB1` | The ULA displays the next two pixels of Byte2 sequentially during a single Z80 clock cycle. It prepares to display the pixels of the next byte in the row with prefetching the corresponding byte from the display memory.
`DisplayB2FetchA1` | The ULA displays the next two pixels of Byte2 sequentially during a single Z80 clock cycle. It prepares to display the pixels of the next byte in the row with prefetching the corresponding attribute from the display memory.

### ContentionDelay

```CSharp
public byte ContentionDelay { get; }
```

Contention delay during the particular screen rendering tact, provided, the CPU is reading 
or writing data in the screen memory area (`#4000`-`#7FFF`).

### PixelByteToFetchAddress

```CSharp
public ushort PixelByteToFetchAddress { get; }
```

Display memory address used in the particular tact to fetch the pixel value.

### AttributeToFetchAddress

```CSharp
public ushort AttributeToFetchAddress { get; }
```

Display memory address used in the particular tact to fetch the attribute of a pixel.

### XPos

```CSharp
public ushort XPos { get; }
```

The X position of the pixel in screen coordinates.

### YPos

```CSharp
public ushort YPos { get; }
```

The Y position of the pixel in screen coordinates.
