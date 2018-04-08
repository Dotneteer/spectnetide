# AudioSamples class

This class stores audio samples for the currently rendered virtual machine frame.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```CSharp
public sealed class AudioSamples
```

### Count

```CSharp
public int Count { get; }
```

Gets the number of samples stored

### this[int]

```CSharp
public float this[int index] { get; }
```

Gets the sample at the specified _`index`_. Each sample is a float value between `0.0f` and `1.0f`.
