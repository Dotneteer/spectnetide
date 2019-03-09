---
layout: documents
categories: 
  - "Z80 Assembler"
title:  "Z80 Instructions"
alias: instructions
seqno: 50
selector: documents
permalink: "documents/z80-instructions"
---

__SpectNetIDE__ implements Every officially documented Z80 instruction as well as the 
non-official ones. During the implementation I used [ClrHome.org](http://clrhome.org/table/)
as a reference.

Z80 instructions may start with a label. Labels are identifiers that can be terminated by an
optional colon (`:`). Both labels in these samples are accepted by the compiler:

```
Start: ld b,#f0
Wait   djnz Wait
```

## Z80 Mnemonics

The compiler accepts these mnemonics:

`ADC`, `ADD`, `AND`, `BIT`, `CALL`, `CCF`, `CP`, `CPD`,
`CPDR`, `CPI`, `CPIR`, `CPL`, `DAA`, `DEC`, `DI`, `DJNZ`,
`EI`, `EX`, `EXX`, `HALT`, `IM`, `IN`, `INC`, `IND`,
`INDR`, `INI`, `INIR`, `JP`, `JR`, `LD`, `LDD`, `LDDR`, `LDDRX`\*, `LDDX`\*, 
`LDI`, `LDIR`, `LDIRSCALE`\*, `LDIRX`\*, `LDIX`\*, `LDPIRX`\*, `MIRROR`\*, `MUL`\*, `NEG`, 
`NEXTREG`\*, `NOP`, `OR`, `OTDR`, `OTIR`, `OUT`, `OUTINB`\*,
`OUTD`, `OUTI`, `PIXELAD`\*, `PIXELDN`\*, `POP`, `PUSH`, `RES`, `RET`, `RETI`, `RETN`,
`RL`, `RLA`, `RLC`, `RLCA`, `RLD`, `RR`, `RRA`, `RRC`,
`RRCA`, `RRD`, `RST`, `SBC`, `SCF`, `SET`, `SETAE`\*, `SLA`, `SLL`
`SRA`, `SRL`, `SUB`, `SWAPNIB`\*, `TEST`\*, `XOR`.

> The instructions marked with \* can be used only with the ZX Spectrum Next model.

## Z80 Registers

The compiler uses the standard 8-bit and 16-bit register names, as specified in the official 
Zilog Z80 documentation:

* 8-bit registers: `A`, `B`, `C`, `D`, `E`, `H`, `L`, `I`, `R`
* 16-bit registers: `AF`, `BC`, `DE`, `HL`, `SP`, `IX`, `IY`
* For the 8-bit halves of the `IX` and `IY` index registers, the compiler uses these names:
`XL`, `XH`, `YL`, `YH`. Alternatively, the compiler accepts these names, too: 
`IXL`, `IXH`, `IYL`, `IYH`. As a kind of exception to general naming conventions, 
these mixed-case names are also accepted: `IXl`, `IXh`, `IYl`, `IYh`.

## JP Syntax

Z80 assemblers use two different syntax for the indirect `JP` statements:

```
; Notation #1
jp hl
jp ix
jp iy

; Notation #2
jp (hl)
jp (ix)
jp (iy)
```
The __SpectNetIDE__ compiler accepts both notation.

## ALU operations syntax

Three standard ALU operations between `A` and other operands (`ADD`, `ADC`, and `SBC`) sign `A`
as their first operand:

```
add a,b
adc a,(hl)
sbc a,e
```

Hovewer, the five other standard ALU operations between `A` and other operands (`SUB`, `AND`, `XOR`, 
`OR`, and `CP`) omit `A` from their notation:

```
sub e
and (hl)
xor e
or c
cp b
```

The __SpectNetIDE__ compiler accepts the second group of ALU operations with using the explicit 
`A` operand, too:

```
sub a,e
and a,(hl)
xor a,e
or a,c
cp a,b
```
