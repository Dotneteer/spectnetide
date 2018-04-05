# MemoryPagingInfo class


### UsesShadowScreen

```CSharp
public bool UsesShadowScreen { get; }
```

Indicates if the virtual machine uses the shadow screen (Bit 3 of port #7FFD). If false, the normal
screen (Bank #5) is used. If true, the shadow screen (Bank #7) is displayed.

### IsInAllRamMode

```CSharp
public bool IsInAllRamMode { get; }
```

Indicates if the virtul machine (ZX Spectrum +3E and ZX Spectrum Next) machine is
in the special ALL RAM mode (Bit 0 of port #1FFD).

