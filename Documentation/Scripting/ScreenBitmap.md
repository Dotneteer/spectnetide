# ScreenBitmap class

This class represents the current screen's pixels, including the border. Every byte stands for a palette
index. The value of a pixel can be `#00` to `#0F` on a Spectrum 48K, 128K, and +3E model. On ZX Spectrum Next,
the value is the entire byte range from `#00` to `#FF`. The content is read-only, you cannot change it.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public sealed class ScreenBitmap
```

### this[int]

```CSharp
public byte[] this[int lineNo] { get; }
```

Retrieves a byte array for the raster line specified in _`lineNo`_. The value at zero index represent 
the leftmost border pixel of the line. The last item in the array is the rightmost pixel in the line.

### this[int, int]

```CSharp
public byte this[int row, int column] { get; }
```

Retrieves the byte for the screen pixel in the given _`column`_ of the specified _`row`_.

### GetDisplayPixel(int, int)

```CSharp
public byte GetDisplayPixel(int row, int col)
```

While `this[int, int]` retrieves the specified screen pixel, this method obtains the display pixel of 
(_`row`_, _`column`_). 
