# AssemblerOptions class

This class represents the options that can be used when the Z80 Assembler compiles
the source code to machine code.

__Namespace__: `Spect.Net.Assembler.Assembler`  
__Assembly__: `Spect.Net.Assembler`

```CSharp
public class AssemblerOptions
```

### PredefinedSymbols

```CSharp
public List<string> PredefinedSymbols { get; }
```

Predefined compilation symbols that can be checked with the `#ifdef`, `#ifndef`, and other
directives.

### DefaultStartAddress

```CSharp
public ushort? DefaultStartAddress { get; set; }
```

The default start address of the compilation. If there's no `ORG` pragma specified in 
the source code, this address is used.

### DefaultDisplacement

```CSharp
public int? DefaultDisplacement { get; set; }
```

The default displacement of the compilation. If there's no `ORG` pragma specified in 
the source code, this address is used. If set to null, no displacement is used.

## CurrentModel

```CSharp
public SpectrumModelType CurrentModel { get; set; }
```

Specifies the Spectrum model to use in the source code. By default it is set to Spectrum 48K. 
The `SpectrumModelType` enumeration has these values:

Value | Machine type
------|-------------
`Spectrum48` | ZX Spectrum 48K
`Spectrum128` | ZX Spectrum 128K
`SpectrumP3` | ZX Spectrum +3E
`Next` | ZX Spectrum Next
