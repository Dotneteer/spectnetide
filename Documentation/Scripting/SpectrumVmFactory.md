# SpectrumVmFactory class

This class provides methods you can use to create ZX Spectrum virtual machine instances. 
Each factory methods retrieves a [`SpectrumVm`](SpectrumVm.md) instance you can use to control and query
the particular machine.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public static class SpectrumVmFactory
```

## Factory methods

### Create(string, string)

```CSharp
public static SpectrumVm Create(string modelKey, string editionKey)
```

Creates a new [`SpectrumVm`](SpectrumVm.md) instance according to the specified *`modelKey`* 
and *`editionKey`* values.

#### Arguments
`modelkey`: The name of the Spectrum model.  
`editionKey`: The name of the Spectrum edition
> The [`SpectrumModels`](SpectrumModels) class defines several string constants that you can use for these arguments. 

#### Returns

The newly created ZX Spectrum virtual machine instance.

#### Exceptions

`ArgumentNullException`: `modelKey` or `editionKey` is `null`.  
`KeyNotFoundException`: The `SpectrumModels` class does not have a definition
for `modelKey`, or the model does not have an edition for `editionKey`.

#### Sample

```CSharp
var sm = Create(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.NTSC);
```

### CreateSpectrum48Pal()

```CSharp
public static SpectrumVm CreateSpectrum48Pal()
```

Creates a new ZX Spectrum 48K PAL virtual machine and retrieves it as 
a [`SpectrumVm`](SpectrumVm.md) instance.

### CreateSpectrum48PalTurbo()

```CSharp
public static SpectrumVm CreateSpectrum48PalTurbo()
```

Creates a new ZX Spectrum 48K PAL virtual machine with double CPU speed (7MHz) and retrieves it as 
a [`SpectrumVm`](SpectrumVm.md) instance.

### CreateSpectrum48Ntsc()

```CSharp
public static SpectrumVm CreateSpectrum48Ntsc()
```

Creates a new ZX Spectrum 48K NTSC virtual machine and retrieves it as 
a [`SpectrumVm`](SpectrumVm.md) instance.

### CreateSpectrum48NtscTurbo()

```CSharp
public static SpectrumVm CreateSpectrum48NtscTurbo()
```

Creates a new ZX Spectrum 48K NTSC virtual machine with double CPU speed (7MHz) and retrieves it as 
a [`SpectrumVm`](SpectrumVm.md) instance.

### CreateSpectrum128()

```CSharp
public static SpectrumVm CreateSpectrum128()
```

Creates a new ZX Spectrum 128K PAL virtual machine and retrieves it as 
a [`SpectrumVm`](SpectrumVm.md) instance.

### CreateSpectrumP3E()

```CSharp
public static SpectrumVm CreateSpectrum128()
```

Creates a new ZX Spectrum +3E PAL virtual machine and retrieves it as 
a [`SpectrumVm`](SpectrumVm.md) instance.

## Provider configuration

The `SpectrumVmFactory` class allows to change providers to customize
the virtual machine to your needs. Doing this requires a bit deeper knowledge
of how the emulator in SpectNetIde works. Probably, you do not need to
use your own providers at all.

Nonetheless, these are the methods you can use to change the default provider
configuration:

### Reset()

Resets the `SpectrumVmFactory` class, clears the predefined providers.

```CSharp
public static void Reset()
```

After invoking `Reset()` you need to set up your own providers.

### RegisterProvider&lt;TProvider&gt;(Func&lt;TProvider&gt;)

Registers a factory method for the `TProvider` type.

```CSharp
public static void RegisterProvider<TProvider>(Func<TProvider> factory)
```
You can register your own _`factory`_ method that creates a provider for 
`TProvider`.

### RegisterDefaultProviders()

Allows you to register the default providers.

```CSharp
public static void RegisterDefaultProviders()
```

When you need to restore the original behavior of `SpectrumVmFactory`,
use this code snippet:

```CSharp
SpectrumVmFactory.Reset();
SpectrumVmFactory.RegisterDefaultProviders();
```

By default, `SpectrumVmFactory` uses these provider registrations:

```CSharp
RegisterProvider<IRomProvider>(() 
    => new ResourceRomProvider(typeof(RomResourcesPlaceHolder).Assembly));
RegisterProvider<IKeyboardProvider>(() 
    => new ScriptingKeyboardProvider());
RegisterProvider<IBeeperProvider>(() 
    => new NoAudioProvider());
RegisterProvider<ITapeProvider>(() 
    => new ScriptingTapeProvider());
RegisterProvider<ISoundProvider>(() 
    => new NoAudioProvider());
```

As the names suggest, the virtual machine created by `SpectrumVmFactory` does not 
generate audio (neither for the beeper nor for the PSG).
