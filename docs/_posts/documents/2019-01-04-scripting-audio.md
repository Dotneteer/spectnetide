---
layout: documents
categories: 
  - "Scripting"
title:  "Scripting the Audio"
alias: scripting-audio
seqno: 70
selector: documents
permalink: "documents/scripting-audio"
---

# The IAudioConfiguration interface

__Namespace__: `Spect.Net.SpectrumEmu.Abstraction.Devices`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public interface IAudioSamplesDevice
```

This interface represents the configuration of the beeper/sound device. The object type that implements
it has these properties:

## AudioSampleRate

```
int AudioSampleRate { get; }
```

The audio sample rate used to generate sound wave form.

## TactsPerSample

```
int TactsPerSample { get; }
```

The number of ULA tacts per audio sample.

## SamplesPerFrame

```
int SamplesPerFrame { get; }
```

The number of samples per ULA video frame. It's value is the quotient of `AudioSampleRate`
and `TacsPerSample`. However, because it is an integer value, certain frames contain this 
amount of audio samples, while other frames may contain one more sample.

# The AudioSamples class

This class stores audio samples for the currently rendered virtual machine frame.

__Namespace__: `Spect.Net.SpectrumEmu.Scripting`  
__Assembly__: `Spect.Net.SpectrumEmu`

```
public sealed class AudioSamples
```

## Count

```
public int Count { get; }
```

Gets the number of samples stored

## this[int]

```
public float this[int index] { get; }
```

Gets the sample at the specified _`index`_. Each sample is a float value between `0.0f` and `1.0f`.