---
layout: documents
categories: 
  - "Scripting"
title:  "Scripting the Screen"
alias: scripting-screen
seqno: 50
selector: documents
permalink: "documents/scripting-screen"
---

# The ScreenConfiguration class

This class represents the configuration of the virtual machine's screen.

__Namespace__: `Spect.Net.SpectrumEmu.Devices.Screen`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public class ScreenConfiguration : IScreenConfiguration
```

The screen rendering mechanism uses a large number of configuration options that provides
the timing so that the CPU and the rendering hardware can work in tandem. The software 
emulation implements the same mechanism.

As the following figure shows, the screen has visible and non-visible areas, just as in the
case of the real hardware. Originally, the non-visible area was required by the cathode-ray tube
so that the electron beam could sync the screen rendering properly.

![Screen Dimensions]({{ site.baseurl }}/assets/images/scripting/spectrum-screen.png)

## ScreenWidth

```
public int ScreenWidth { get; }
```

The width of the visible screen area in pixels.

## ScreenLines

```
public int ScreenLines { get; }
```

The height of the visible screen area in pixels.

## DisplayWidth

```
public int DisplayWidth { get; }
```

The width of the display area in pixels (256).

## DisplayLines

```
public int DisplayLines { get; }
```

The height of the display area in pixels.

## NonVisibleBorderTopLines

```
public int NonVisibleBorderTopLines { get; }
```

The number of top border lines that are not visible when rendering the screen.

## BorderTopLines

```
public int BorderTopLines { get; }
```

The number of border lines above the display area.

## BorderBottomLines

```
public int BorderBottomLines { get; }
```

The number of border lines below the display area.

## NonVisibleBorderBottomLines

```
public int NonVisibleBorderBottomLines { get; }
```

The number of bottom border lines that are not visible when rendering the screen.

## VerticalSyncLines

```
public int VerticalSyncLines { get; }
```

Number of lines used for vertical synch.

## BorderLeftTime

```
public int BorderLeftTime { get; }
```

The number of CPY T-cycles of displaying the left part of the border.

## BorderLeftPixels

```
public int BorderLeftPixels { get; }
```

The number of border pixels to the left of the display (`2 * BorderLeftTime`).

## BorderRightTime

```
public int BorderRightTime { get; }
```

The number of CPY T-cycles of displaying the right part of the border.


## BorderRightPixels

```
public int BorderRightPixels { get; }
```

The number of border pixels to the right of the display (`2 * BorderRightTime`).

## NonVisibleBorderRightTime

```
public int NonVisibleBorderRightTime { get; }
```

The time given in CPY T-cycles to render the nonvisible right part of the border.

## HorizontalBlankingTime

```
public int HorizontalBlankingTime { get; }
```

Horizontal blanking time (HSync + blanking) given in CPU T-cycles.


The `ScreenConfiguration` class provides other properties that are related to screen rendering.

## InterruptTact

```
public int InterruptTact { get; }
```

The tact index of the interrupt signal relative to the top-left screen pixel

## FirstDisplayLine

```
public int FirstDisplayLine { get; }
```

The screen line number of the first display line (`VerticalSyncLines 
+ NonVisibleBorderTopLines + BorderTopLines`).

## LastDisplayLine

```
public int LastDisplayLine { get; }
```

The screen line number of the last display line (`FirstDisplayLine + DisplayLines - 1`).

## DisplayLineTime

```
public int DisplayLineTime { get; }
```

The time of rendering a display pixel row, given in CPU T-cycles.

## ScreenLineTime

```
public int ScreenLineTime { get; }
```

The time of rendering an entire raster line, given in CPU T-cycles (`BorderLeftTime + 
DisplayLineTime + BorderRightTime + NonVisibleBorderRightTime + HorizontalBlankingTime`).

## RasterLines

```
public int RasterLines { get; }
```

The number of raster lines in the screen (`FirstDisplayLine + DisplayLines + BorderBottomLines + NonVisibleBorderBottomLines`).

## FirstDisplayPixelTact

```
public int FirstDisplayPixelTact { get; }
```

The tact in which the top left display pixel should be rendered, given in CPU T-cycles
(`FirstDisplayLine * ScreenLineTime + BorderLeftTime`).

## FirstScreenPixelTact

```
public int FirstScreenPixelTact { get; }
```

The tact in which the top left screen pixel (border) should be displayed
(`[VerticalSyncLines + NonVisibleBorderTopLines] * ScreenLineTime`).

## ScreenRenderingFrameTactCount

```
public int ScreenRenderingFrameTactCount { get; }
```

The number of CPU T-cycles used for the full rendering of the screen
(`RasterLines * ScreenLineTime`).

# Methods

The class provides a few helper methods.

## IsTactVisible(int, int)

```
public bool IsTactVisible(int line, int tactInLine)
```

Tests whether the specified tact is in the visible area of the screen. Returns true, if
the tact renders the visible part of the screen; otherwise, false.

### Arguments

`line`: Raster line index  
`tactInLine`: Tact index within the line

## IsTactInDisplayArea(int, int)

```
public bool IsTactInDisplayArea(int line, int tactInLine)
```

Tests whether the specified tact is in the display area of the screen. Returns true, if
the tact renders the display part of the screen; otherwise, false.

### Arguments

`line`: Raster line index  
`tactInLine`: Tact index within the line

# The ScreenBitmap class

This class represents the current screen's pixels, including the border. Every byte stands for a palette
index. The value of a pixel can be `#00` to `#0F` on a Spectrum 48K, 128K, and +3E model. On ZX Spectrum Next,
the value is the entire byte range from `#00` to `#FF`. The content is read-only, you cannot change it.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public sealed class ScreenBitmap
```

## this[int]

```
public byte[] this[int lineNo] { get; }
```

Retrieves a byte array for the raster line specified in _`lineNo`_. The value at zero index represent 
the leftmost border pixel of the line. The last item in the array is the rightmost pixel in the line.

## this[int, int]

```
public byte this[int row, int column] { get; }
```

Retrieves the byte for the screen pixel in the given _`column`_ of the specified _`row`_.

## GetDisplayPixel(int, int)

```
public byte GetDisplayPixel(int row, int col)
```

While `this[int, int]` retrieves the specified screen pixel, this method obtains the display pixel of 
(_`row`_, _`column`_). 

# The ScreenRenderingStatus class

Provides properties about the current screen rendering status of the machine.

__Namespace__: `Spect.Net.SpectrumEmu.Devices.Screen`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public sealed class ScreenRenderingStatus
```

## CurrentFrameTact

```
public int CurrentFrameTact { get; }
```

Gets the number of the current tact within the frame being rendered.

## RasterLine

```
public int RasterLine { get; }
```

Gets the number of the current raster line in the frame being rendered.

## CurrentRenderingTact

```
public ScreenRenderingTact CurrentRenderingTact { get; }
```

Gets details about the current tact in the frame being rendered. See more details
in the description of [`ScreenRenderingTact`](ScreenRenderingTact.md).

# The ScreenRenderingTable class

Represents the screen rendering table of the virtual machine

__Namespace__: `Spect.Net.SpectrumEmu.Devices.Screen`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public sealed class ScreenRenderingTable
```

## Count

```
public int Count { get; }
```

Gets the number of items in the screen rendering table.

## this[int]

```
public ScreenRenderingTact this[int index] { get; }
```

Gets the entry of the screen rendering table specified by _`index`_. See more details
in the description of [`ScreenRenderingTact`](ScreenRenderingTact.md).

# The ScreenRenderingTact class

Provides details about a screen rendering tact.

__Namespace__: `Spect.Net.SpectrumEmu.Devices.Screen`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public sealed class ScreenRenderingTact
```

## Phase

```
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

## ContentionDelay

```
public byte ContentionDelay { get; }
```

Contention delay during the particular screen rendering tact, provided, the CPU is reading 
or writing data in the screen memory area (`#4000`-`#7FFF`).

## PixelByteToFetchAddress

```
public ushort PixelByteToFetchAddress { get; }
```

Display memory address used in the particular tact to fetch the pixel value.

## AttributeToFetchAddress

```
public ushort AttributeToFetchAddress { get; }
```

Display memory address used in the particular tact to fetch the attribute of a pixel.

## XPos

```
public ushort XPos { get; }
```

The X position of the pixel in screen coordinates.

## YPos

```
public ushort YPos { get; }
```

The Y position of the pixel in screen coordinates.