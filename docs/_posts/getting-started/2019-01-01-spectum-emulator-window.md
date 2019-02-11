---
layout: getting-started
title:  "The ZX Spectrum Emulator"
id: zx-spectrum-emulator-window
seqno: 80
selector: tutorial
permalink: "getting-started/zx-spectrum-emulator-window"
---

__SpectNetIDE__ would not be useful without an emulator to develop programs. The ZX Spectrum Emulator tool window shows the screen of the embedded ZX Spectrum virtual machine that runs in the background.

![Spectrum Emulator]({{ site.baseurl }}/assets/images/tutorials/run-completed.png)

As you see, the toolbar at the top of the window contains buttons to control the virtual machine.

### Start

![Start]({{ site.baseurl }}/assets/images/tutorials/start-button.png)


This button turns of the virtual machine if it stopped, or continues running it if the machine is stopped.

As you see, the toolbar at the top of the window contains buttons to control the virtual machine. You can use the `Ctrl + F5` key as a shortcut.

> __Note__: shortcut keys are available only when the focus is in one of the emulator-related tool windows, including the emulator, ZX Spectrum Memory, Z80 Disassembly, Z80 Registers, and Watch Memory tool windows. You cannot use them while you are in the code editor or in Solution Explorer.

### Stop

![Stop]({{ site.baseurl }}/assets/images/tutorials/stop-button.png)

Immediately turns off the virtual machine (`Ctrl + M`, then `Ctrl + S`). It is as if you turned the hardware off.

### Pause

![Pause]({{ site.baseurl }}/assets/images/tutorials/pause-button.png)

You can pause the running virtual machine at any time. Pausing suspends the execution, and the emulator-related tool windows display the state of the virtual machine, such as the current register values, the memory contents, or the execution point in the Disassembly view.

When you use step-by-step debugging, the virtual machine pauses every time it reaches a new breakpoint.

### Restart

![restart]({{ site.baseurl }}/assets/images/tutorials/restart-button.png)


Restarts the virtual machine, just as if you turned it off and on again.

### Start Debugging

![Debug]({{ site.baseurl }}/assets/images/tutorials/debug-button.png)

You can start the virtual machine in debug mode (`F5`). In this mode, the virtual machine pauses when it reaches a breakpoint (and the optional breakpoint condition is satisfied). If you run the virtual machine with __Start__ (`Ctrl + F5`), it won't pause at breakpoints.

> __Note__: The caption of the ZX Spectrum Emulator window shows the __(Debugging)__ label when it runs in debug mode.

### Step-Into

![Step-into]({{ site.baseurl }}/assets/images/tutorials/step-into-button.png)

While the machine is paused, the Step-Into function (`F11`) executes the next Z80 instruction. If the subsequent operation is a subroutine call (such as `CALL` or `RST`), _Step-Into_ pauses at the first operation of the subroutine. Should the next one be a block instruction (such as `LDIR`), _Step-Into_ breaks after the first loop of the block. Thus, if the block takes ten iterations, you have to _Step-Into_ ten time to get to the following operation. 

### Step-Over

![Step over]({{ site.baseurl }}/assets/images/tutorials/step-over-button.png)

_Step-Over_ (`F10`) works the same for most instructions as _Step-Into_. Nonetheless, if the next operation is a `CALL` (or `RST`), _Step-Over_ executes the entire subroutine and pauses at the next instruction following the call. If there's a block instruction (e.g. `LDIR`), _Step-Over_ breaks at the next statement, completing the entire block operation.

> __Note__: If a subroutine call manipulates the stack and returns to somewhere else, _Step-Over_ may not pause at the location you expect.  

### Step-Out

![Sep out]({{ site.baseurl }}/assets/images/tutorials/step-out-button.png)

When you are within a subroutine call, _Step-Out_ continues the execution flow until the current subroutine returns to the caller, and pauses the machine right after returning. 

> __Note__: If a subroutine call manipulates the stack and returns to somewhere else, _Step-Out_ may not pause at the location you expect. 

### Save Virtual Machine State

![Save state]({{ site.baseurl }}/assets/images/tutorials/save-state-button.png)

Any time the virtual machine is stopped, you can save its state including the CPU, memory, execution flow, devices, etc. into a `.vmstate` file.

### Load Virtual Machine State

![Load state]({{ site.baseurl }}/assets/images/tutorials/load-state-button.png)

When the virtual machine is paused, you can restore its former state by loading it from a `.vmstate` file. Saving and loading the state allows you to suspend debugging between Visual Studio sessions.

### Add Virtual Machine State to the project

![Add state]({{ site.baseurl }}/assets/images/tutorials/add-state-button.png)

If you are analyzing code, it is useful if the virtual machine state files are the part of your project so that you can handle them with the same source control procedures as other project files. You can not only save a `.vmstate` file, but also add it to your project's `VmStates` folder.

> __Note__: In other articles, you can find more details about working with `.vmstate` files.

## Emulator Behavior

As soon as you load a ZX Spectrum project into Visual Studio, the IDE creates a new ZX Spectrum virtual machine according to the type specified when you created the project. This machine does not run; you need to start it explicitly.

Even if the ZX Spectrum Emulator tool window is closed, the virtual machine still runs in the background once you've started it. It halts if you explicitly stop it, close the current solutions, or exit the Visual Studio IDE.

While the ZX Spectrum Emulator tool window has the focus, the IDE forward the keystrokes to the emulator that senses them as if you pressed a key on the ZX Spectrum hardware. 
