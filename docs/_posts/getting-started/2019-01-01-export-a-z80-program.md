---
layout: getting-started
title:  "Export a Z80 Program"
id: export-a-z80-program
seqno: 50
selector: tutorial
permalink: "getting-started/export-a-z80-program"
---

After you created your Z80 Assembler program, you can easily export it into a `.TAP` or `.TZX` file so that you
could load it into a ZX Spectrum emulator, or into a real hardware, such as a ZX Spectrum Next, or ZX Spectrum
(with the help of TZXDuino or CASDuino hardware).

To try how easy it is, create a simple Z80 program:

1. Create a new ZX Spectrum 48 project (see details [here]({{ site.baseurl }}/getting-started/create-zx-spectrum-48k-project.html#article)).
The __`CodeFiles `__ project of the folder contains a __`Code.z80asm`__ file:

```
; Code file
start:
    .org #8000
```

{:start="2"}
2. Extend to the code with a few lines:

```
; Code file
start:
    .org #8000
    ld a,2
    out (#fe),a
    jp #12a2
```

{:start="3"}
3. In Solution Explorer, right-click the __`Code.z80asm`__ file, and invoke the __Run Z80 program__ command (or use the __Ctrl+M__, __Ctrl+R__ double shortcut keys):

![Run Z80 code command]({{ site.baseurl }}/assets/images/tutorials/run-z80-code-command.png)

This command compiles the Z80 assembly code to binary machine code, starts (or restarts) the Spectrum virtual machine,
injects the binary code, and runs it:

![Z80 code runs]({{ site.baseurl }}/assets/images/tutorials/z80-code-runs.png)

You can easily export the code with these steps:

1. In Solution Explorer, right-click the __`Code.z80asm`__ file, and invoke the __Export Z80 program__ command:

![Export Z80 code command]({{ site.baseurl }}/assets/images/tutorials/export-z80-code-command.png)

{:start="2"}
2. The IDE pops up the __Export Z80 Program__ dialog:

![Export Z80 code command]({{ site.baseurl }}/assets/images/tutorials/export-z80-program-dialog.png)

You can change the attributes of the exported program. The default settings of the dialog will create a loader program that automatically loads and runs the code. Beside the default options, I set the __Add the exported tape file__ to the project checkbox.

{:start="3"}
3. Click Export.

Now, you can find the exported code file in the `C:\Temp` folder (`Code.tzx`), and also in your project, within the `TapeFile` folder:

![Exported Z80 code in project]({{ site.baseurl }}/assets/images/tutorials/exported-code-in-project.png)

You have many other options to export your code, I will explain them in another tutorial.