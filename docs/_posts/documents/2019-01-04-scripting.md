---
layout: documents
categories: 
  - "Scripting"
title:  "Overview"
alias: scripting-overview
seqno: 10
selector: documents
permalink: "documents/scripting-overview"
---

__SpectNetIde__ is not just a simple ZX Spectrum emulator
and a set of development tools integrated with Visual Studio,
it offers a collection of .NET objects that you can use to automate
ZX Spectrum related tasks.

Should it be looking for infinite
lives for a game, understanding the structure of a complex
application, finding out how a particular code works within 
special conditions &mdash; scripting makes it easy to automate
mechanical tasks.

The `Spect.Net.SpectrumEmu.Scripting` namespace provides a dozen of object types suited for automation. You can use these
types from any .NET language, and scripting engines that support
directly.

> Many popular scripting languages may support 
.NET objects through wrappers. Although __SpectNetIde__ does not
have any direct support for these languages, contributors are
welcomed to create such adapters. If you are interested, please
contact me at [dotneteer@hotmail.com](mailto:dotneteer@hotmail.com).  

## A Sample Script

Just for the sake of demonstration, here is a short sample that sets
the border to red, and checks that the screen is rendered accordingly:

```
using System;
using System.Threading.Tasks;
using Spect.Net.SpectrumEmu.Scripting;

namespace MyFirstScript
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var sm = SpectrumVmFactory.CreateSpectrum48Pal();
      Console.WriteLine("Setting the border to red");

      await sm.StartAndRunToMain();
      var entryAddr = sm.InjectCode(@"
        .org #8000
        ld a,2       ; RED
        out (#FE),a  ; Set the border colour to red
        halt         ; Wait for 2 frames (rendering)
        halt
        ret          ; Finished
      ");
      sm.CallCode(entryAddr);
      await sm.CompletionTask;

      var screen = sm.ScreenConfiguration;
      var redCount = 0;
      for (var i = 0; i < screen.ScreenLines; i++)
        for (var j = 0; j < screen.ScreenWidth; j++)
          redCount += sm.ScreenBitmap[i,j] == 0x02 ? 1 : 0;

      Console.WriteLine($"#of red pixels: {redCount}");
      var borderPixels = screen.ScreenLines * screen.ScreenWidth
        - screen.DisplayLines * screen.DisplayWidth;
      Console.WriteLine($"#of border pixels: {borderPixels}");
    }
  }
}
```

The script starts with creating a ZX Spectrum 48 virtual machine:

```
var sm = SpectrumVmFactory.CreateSpectrum48Pal();
```

Then the code starts the virtual machine (asynchronously in the background) and waits while it initializes the operating system, and enters into its main execution cycle:

```
await sm.StartAndRunToMain();
```

As it reaches the desired state, the virtual machine pauses. At this point, it is ready for code injection. As the next code snippet shows,  _SpectNetIde__ makes it extremely easy to inject source code into the machine:

```
var entryAddr = sm.InjectCode(@"
  .org #8000
  ld a,2       ; RED
  out (#FE),a  ; Set the border colour to red
  halt         ; Wait for 2 frames (rendering)
  halt
  ret          ; Finished
");
```

You can utilize the [Z80 Assembler syntax]({{ site.baseurl }}/documents/language-structure.html)
(the same as the IDE uses) to define the machine code to inject. 
The `InjectCode()` method returns the entry address of the code, so
you can explicitly start the code in the virtual machine:

```
sm.CallCode(entryAddr);
await sm.CompletionTask;
```

The `CallCode()` method continues the execution cycle of the machine and pauses it when the code returns. The `sm.CompletionTask` provides an awaiter to wait while the machine gets paused.

The last part of the code uses the `ScreenConfiguration` property to
get information about the screen. The `ScreenBitmap` object can be used to check every pixel in the rendered screen (including the border pixels):

```
var screen = sm.ScreenConfiguration;
var redCount = 0;
for (var i = 0; i < screen.ScreenLines; i++)
    for (var j = 0; j < screen.ScreenWidth; j++)
        redCount += sm.ScreenBitmap[i,j] == 0x02 ? 1 : 0;

Console.WriteLine($"#of red pixels: {redCount}");
var borderPixels = screen.ScreenLines * screen.ScreenWidth
    - screen.DisplayLines * screen.DisplayWidth;
Console.WriteLine($"#of border pixels: {borderPixels}");
```

When you start this code from Visual Studio, it produces this output:

```
Setting the border to red
#of red pixels: 52224
#of border pixels: 52224
```

The result proves that the virtual machine renders all border pixels with red color.
