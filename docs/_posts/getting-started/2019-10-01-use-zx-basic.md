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
The __`ZxBasicFiles `__ project of the folder contains a __`Program.zxbas`__ file:

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

> __*Note*__: For more details about ZX BASIC, visit this link: [http://www.zxbasic.net](http://www.zxbasic.net)
