# ScreenRenderingStatus class

Provides properties about the current screen rendering status of the machine.

__Namespace__: `Spect.Net.SpectrumEmu.Devices.Screen`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public sealed class ScreenRenderingStatus
```

### CurrentFrameTact

```CSharp
public int CurrentFrameTact { get; }
```

Gets the number of the current tact within the frame being rendered.

### RasterLine

```CSharp
public int RasterLine { get; }
```

Gets the number of the current raster line in the frame being rendered.

### CurrentRenderingTact

```CSharp
public ScreenRenderingTact CurrentRenderingTact { get; }
```

Gets details about the current tact in the frame being rendered. See more details
in the description of [`ScreenRenderingTact`](ScreenRenderingTact.md).
