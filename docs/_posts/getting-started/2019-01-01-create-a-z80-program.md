---
layout: getting-started
title:  "Create a Z80 Program"
id: create-a-z80-program
seqno: 40
selector: tutorial
permalink: "getting-started/create-a-z80-program"
---

In this article, you will learn that SpectNetIde provides you a straightforward way to create and run Z80 assembly programs.

To create your first Z80 assembly program, follow these steps:

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
3. In Solution Explorer, right-click the __`Code.z80asm`__ file, and invoke the __Run Z80 program__ command:

![Run Z80 code command]({{ site.baseurl }}/assets/images/tutorials/run-z80-code-command.png)

> __Note__: You can use the __Ctrl+M__, __Ctrl+R__ double shortcut keys to execute the __Run Z80 program__.

This command compiles the Z80 assembly code to binary machine code, starts (or restarts) the Spectrum virtual machine,
injects the binary code, and runs it:

![Z80 code runs]({{ site.baseurl }}/assets/images/tutorials/z80-code-runs.png)

You have just created your first Z80 assembly program in SpectNetIde!

## Errors in the code

If you make an error -- either syntax or semantic error -- the SpectNetIDE assembler gives an error message. Let's assume you make an error in the code:

```
; Code file
start:
    org #8000
    ld a,hl      ; "hl" is a semantic error
    out (#fe),a 
    jp #12a2
```

The Z80 Assembler displays the issue in the Error List:

![Compile error]({{ site.baseurl }}/assets/images/tutorials/compile-error.png)

When you double click the error line, the IDE navigates you to the error line in the source code:

![Error marked in source]({{ site.baseurl }}/assets/images/tutorials/error-marked-in-source.png)

## A Bit Longer Z80 Program

If you're excited, you can try a bit longer Z80 code with border manipulations and delays. When you run it, the code sets the screen colors and uses the __`HALT`__ statement to synchronized border drawing. When you start it, it runs until you press `SPACE`:

![Z80 baner runs]({{ site.baseurl }}/assets/images/tutorials/banner-program-runs.png)

Here is the entire source code:

```
; Define symbols
LAST_K:		.equ #5c08
MAIN_EX:	.equ #12a2

Start:
    .org #8000

; Set Banner lines
SetScreen:
    call ClearScreen
    ld a,0b00_010_010
    ld b,0
    call ClearLine
    ld a,0b00_110_110
    ld b,1
    call ClearLine
    ld a,0b00_101_101
    ld b,2
    call ClearLine
    ld a,0b00_100_100
    ld b,3
    call ClearLine

; Set banner border area
Top:
    ld bc,#200
    call LongDelay
    ld a,2
    call SetBorder
    ld a,6
    call SetBorder
    ld a,5
    call SetBorder
    ld a,4
    call SetBorder
    ld a,0
    call SetBorder
    halt        ; Wait for the next interrupt
    ld hl,LAST_K
    ld a, (hl)	; put last keyboard press into A
    cp #20		; was "space" pressed?
    jr nz,Top	; If not, back to the cycle
    ld a,7
    call SetBorder
    jp MAIN_EX

; Sets the entire screen to black
ClearScreen:
    ld bc,24*32-1 ; #of screen attribute bytes
    ld hl,#5800 ; First attr address
    ld de,#5801 ; Next attr address
    ld (hl),0   ; Black on Black
    ldir        ; Set all attribute bytes
    ret

; A = Attr byte to set
; B = Line index
ClearLine:
    push af     ; Save A
    ld a,b      ; Multiply the line count with 32
    sla a
    sla a
    sla a
    sla a
    sla a
    ld hl,#5800 ; Calculate the attribute address
    ld d,0
    ld e,a
    add hl,de
    pop af      ; Restore A
    ld b,#20    ; Set all the 32 attribute bytes
SetAttr:
    ld (hl),a
    inc hl
    djnz SetAttr
    ret

; Sets the border color
SetBorder:
    out (#fe),a
    ld b,#86
Delay:
    djnz Delay
    ret

; Delays code execution
LongDelay:
    dec bc
    ld a,b
    or c
    jr nz,LongDelay
    ret
```

__SpectNetIDE__ makes it extremely simple to reuse the Z80 assembly code, as you
will learn in the next article.
