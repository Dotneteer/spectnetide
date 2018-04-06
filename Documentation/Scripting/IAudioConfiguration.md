# IAudioConfiguration interface

This interface represents the configuration of the beeper/sound device. The object type that implements
it has these properties:

### AudioSampleRate

```CSharp
int AudioSampleRate { get; }
```

The audio sample rate used to generate sound wave form.

### TactsPerSample

```CSharp
int TactsPerSample { get; }
```

The number of ULA tacts per audio sample.

### SamplesPerFrame

```CSharp
int SamplesPerFrame { get; }
```

The number of samples per ULA video frame. It's value is the quotient of `AudioSampleRate`
and `TacsPerSample`. However, because it is an integer value, certain frames contain this 
amount of audio samples, while other frames may contain one more sample.

