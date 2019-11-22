---
layout: documents
categories: 
  - "Tutorials v2"
title:  "Use ZX BASIC"
alias: use-zx-basic
seqno: 1052
selector: tutorial
permalink: "getting-started/use-zx-basic"
---

> To use __ZX BASIC__ &mdash; as it is a separate product from __SpectNetIDE__, first, you should install it on your computer. The previous tutorial explains the setup procedure.

SpectNetIDE supports ZX BASIC (Boriel's BASIC). In this tutorial, you will learn how to create and run a ZX BASIC program.

To create your first ZX BASIC program, follow these steps:

1. Create a new ZX Spectrum 48 project (see details [here]({{ site.baseurl }}/getting-started/create-zx-spectrum-48k-project-2.html#article)).
The __`ZxBasicFiles `__ folder of the project contains a __`Program.zxbas`__ file:

```
cls
print at 4,6; ink 7; paper 1; flash 1;" HELLO, ZX BASIC! "
```

{:start="2"}
2. Right-click the `Program.zxbas` file in Solution Explorer, and use the __Run program__ command.

![Run ZX BASIC]({{ site.baseurl }}/assets/images/tutorials/run-zx-basic-command.png)

The IDE compiles the ZX BASIC code, displays the emulator window, and runs the code:

![Run ZX BASIC]({{ site.baseurl }}/assets/images/tutorials/zx-basic-runs.png)

> __Note__: You can use the __Ctrl+M__, __Ctrl+R__ double shortcut keys to execute the __Run Z80 program__.

Let's create a new ZX BASIC program!

1. Right-click the __ZxBasicFiles__ folder in Solution Explorer, and select the __Add &rarr; New item__ command:

![New ZX BASIC item]({{ site.baseurl }}/assets/images/tutorials/new-zx-basic-item.png)

{:start="2"}
2. Select the __ZX BASIC Program__ item type, set the name to `Clock.zxbas`, and click __Add__.

3. Type (copy) this code into the new `Cloxk.zxbas` file:

```
CLS
CIRCLE 132, 105, 86
FOR n = 1 to 12
    PRINT AT 10 - (10 * COS(n * PI / 6) - 0.5), 16 + (0.5 + 10 * SIN(n * PI / 6)); n
NEXT n
CIRCLE 132, 105, 70

PRINT AT 23, 0; "PRESS ANY KEY TO EXIT";

FUNCTION t AS ULONG
    RETURN INT((65536 * PEEK (23674) + 256 * PEEK(23673) + PEEK (23672))/50)
END FUNCTION

DIM t1 as FLOAT

OVER 1
WHILE INKEY$ = ""
    LET t1 = t()
    LET a = t1 / 30 * PI
    LET sx = 72 * SIN a : LET sy = 72 * COS a
    PLOT 131, 107: DRAW sx, sy

    LET t2 = t()
    WHILE (t2 <= t1) AND (INKEY$ = "")
        let t2 = t()
    END WHILE

    PLOT 131, 107: DRAW sx, sy
END WHILE
```

{:start="4"}
4. Run the program (either with the Run program command or the __Ctrl+M__, __Ctrl+R__ shortcut keys):

![ZX BASIC clock runs]({{ site.baseurl }}/assets/images/tutorials/zx-basic-clock-runs.png)

> __*Note*__: For more details about ZX BASIC, visit this link: [https://zxbasic.readthedocs.io/](https://zxbasic.readthedocs.io/). You can find more example within the `examples` folder within your ZX BASIC instalation folder, or [here](https://zxbasic.readthedocs.io/en/latest/sample_programs/).

## ZX BASIC Compilation

When you run any command that requires compiling a ZX BASIC file, SpectNetIDE carries out the compilation in two steps:

1. First, it transpiles the ZX BASIC program to Z80 Assembly
2. Then, it compiles the Z80 Assembly to machine code that runs in SpectNetIDE.

By default, the IDE stored the temporary Z80 Assembly file in a working folder within the project structure. However, you can add it to the project files, if you want to work with it:

Open the __Tools &rarr; Options__ dialog. Under the __ZX BASIC Options__ category set the __Store generated .z80asm file__ option to __true__:

![ZX BASIC store option]({{ site.baseurl }}/assets/images/tutorials/zxb-store-option.png)

From now on, the compilation will add the generated Z80 Assembly file to the project folder, in the same folder as the ZX BASIC file:

![ZX BASIC flat]({{ site.baseurl }}/assets/images/tutorials/zxb-asm-flat.png)

You can use the `.z80asm` file the same way as if you had created it manually.

Alternatively, you can assign the generated `.z80asm` file to be the nested (child) project item of the `.zxbas` files by setting the __Nest generated .z80asm file__ option to __true__:

![ZX BASIC nest option]({{ site.baseurl }}/assets/images/tutorials/zxb-nest-option.png)

Remove the generated `.zxbas.z80asm` file, and next time you compile the ZX BASIC file, it will be the child of the ZX BASIC file:

![ZX BASIC nested]({{ site.baseurl }}/assets/images/tutorials/zxb-asm-nested.png)

> **_Note_**: Though the icon of the nested file does not indicate that it is a standard `.z80asm` file &mdash; this is a Visual Studio limitation &mdash; you can still use the file normally. However, you cannot mark it as the default code file.

When you examine the `.z80asm` file generated during a ZX BASIC compilation, it starts with a `.zxbasic` pragma like this snippet shows:

```
    .zxbasic
    org 32768
    ; Defines HEAP SIZE
ZXBASIC_HEAP_SIZE EQU 4096
__START_PROGRAM:
    di
    push ix
    push iy
    exx
    push hl
    ...
```

The pragma instructs the Z80 Assembler to handle this code file with special options:
- Label and symbol names are case-sensitive. (Normally, those are case-insensitive.)
- All labels within a `PROC` are global unless specified local explicitly with `LOCAL`. (Normally, labels in a `PROC` are local by default.)

ZX BASIC compiles the code as a subroutine that can be CALL-ed. Z80 Assembler creates and injects code that can be started with jumping to the start address. The `.zxbasic` pragma tells the IDE that the compiled code should be called as a subroutine. When you inject and run ZX BASIC code, the virtual machine returns to the $12AC (ZX Spectrum 48 main execution cycle) address and waits for a keypress. When any key is pressed, it clears the screen.
