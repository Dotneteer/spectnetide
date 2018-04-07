# CodeBreakpoints class

Represents the breakpoint of the Spectrum virtual machine, at which execution should be
paused when running in debug mode.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public sealed class CodeBreakpoints
```

### Count

```CSharp
public int Count { get; }
```

The number of breakpoint defined.

### AddBreakpoint(ushort)

```CSharp
public void AddBreakpoint(ushort address)
```

Adds a breakpoint for the specified _`address`_.

### RemoveBreakpoint(ushort)

```CSharp
public void RemoveBreakpoint(ushort address)
```

Removes the breakpoint from the specified _`address`_.

### ClearAllBreakpoints()

```CSharp
public void ClearAllBreakpoints()
```

Clears all previously declared breakpoints.


### HasBreakpointAt(ushort)

```CSharp
public bool HasBreakpointAt(ushort address)
```

Checks if there is a breakpoint definied for the given _`address`_.
