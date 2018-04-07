# ScreenConfiguration class

This class represents the configuration of the virtual machine's screen.

__Namespace__: `Spect.Net.SpectrumEmu.Devices.Screen`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public class ScreenConfiguration : IScreenConfiguration
```

The screen rendering mechanism uses a large number of configuration options that provides
the timing so that the CPU and the rendering hardware can work in tandem. The software 
emulation implements the same mechanism.

As the following figure shows, the screen has visible and non-visible areas, just as in the
case of the real hardware. Originally, the non-visible area was required by the cathode-ray tube
so that the electron beam could sync the screen rendering properly.

![Screen Dimensions](Figures/SpectrumScreen.png)

### ScreenWidth

```CSharp
public int ScreenWidth { get; }
```

The width of the visible screen area in pixels.

### ScreenLines

```CSharp
public int ScreenLines { get; }
```

The width of the visible screen area in pixels.



