# SpectrumModels class

This class is a repository of all Spectrum models and editions supported 
by __SpectNetIde__.

__Namespace__: `Spect.Net.SpectrumEmu`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public static class SpectrumModels
```

## The StockModels property

You can use the `StockModels` property to access the dictionary of
available ZX Spectrum models.

```CSharp
public static IReadOnlyDictionary<string, SpectrumModelEditions> StockModels;
```

Each model has a unique name to look up the associated editions in the dictionary.
A `SpectrumModelEditions` instance is a collection of all editions of the 
particular model. A single edition is represented by a `SpectrumModelEdition` instance.

## Constants used for models and editions

The `SpectrumModels` class provides these constants for the unique model names and editions:

Constant value | Description
---------------|------------
`ZX_SPECTRUM_48` | `"ZX Spectrum 48K"`
`ZX_SPECTRUM_128` | `"ZX Spectrum 128K"`
`ZX_SPECTRUM_P3_E` | `"ZX Spectrum +3E"`
`ZX_SPECTRUM_NEXT` | `"ZX Spectrum Next"`
`PAL` | `"PAL"`
`NTSC` | `"NTSC"`
`PAL_2_X` | `"PAL2X"`
`NTSC_2_X` | `"NTSC2X"`

## Model access

`SpectrumModels` provides properties to directly access the 
`SpectrumModelEdition` instances of frequently used machine types:

```CSharp
public static SpectrumEdition ZxSpectrum48Pal;
public static SpectrumEdition ZxSpectrum48Ntsc;
public static SpectrumEdition ZxSpectrum48Pal2X;
public static SpectrumEdition ZxSpectrum48Ntsc2X;
public static SpectrumEdition ZxSpectrum128Pal;
public static SpectrumEdition ZxSpectrumP3EPal;
public static SpectrumEdition ZxSpectrumNextPal;
```

These properties are associated with these model and edition key constants:


Property | Model key | Edition key
---------|-----------|------------
`ZxSpectrum48Pal` | `ZX_SPECTRUM_48` | `PAL`
`ZxSpectrum48Ntsc` | `ZX_SPECTRUM_48` | `NTSC`
`ZxSpectrum48Pal2X` | `ZX_SPECTRUM_48` | `PAL2X`
`ZxSpectrum48Ntsc2X` | `ZX_SPECTRUM_48` | `NTSC2X`
`ZxSpectrum128Pal` | `ZX_SPECTRUM_128` | `PAL`
`ZxSpectrumP3EPal` | `ZX_SPECTRUM_128` | `PAL`
`ZxSpectrumNext` | `ZX_SPECTRUM_NEXT` | `PAL`
