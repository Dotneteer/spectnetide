---
layout: documents
categories: 
  - "Z80 Assembler"
title:  "Main Features"
alias: main-features
seqno: 10
selector: documents
permalink: "documents/main-features"
---

The original goal of the __SpectNetIde__ assembler was to have a simple tool that allows you to compile
Z80 assembly code and inject it into the ZX Spectrum virtual machine. As the community has started
using it, I've been receiving feature requests to add some useful capability to the Assembler.

Here is a list of important features the __SpectNetIde__ suports:

* __Full Z80 instruction set__, including the initially undocumented Z80 registers and instructions
(such as the 8-bit halves of `ix` and `iy`, namely `ixl`, `ixh`, `iyl`, `iyh`).
* __ZX Spectrum Next extended Z80 instruction set__
* __Alternate syntax versions__. All directives, pragmas, and statements have multiple versions so that 
you can use your preferred notation. For example, you can use `.loop`, `loop`, `.LOOP` or `LOOP` to 
declare a loop. All of the `.defb`, `DEFB`, `.db`, `DB` (and a few other) tokens can be used for defining
byte data. The `.endw` and `WEND` tokens can close a WHILE-loop.
* __Z80 Preprocessor__. With preprocessor directives, you can carry out conditional compilation and include
other source files. You can inject symbols for debug time and run time compilations separately. *In __SpectNetIde__
you can use powerful macros, too, notheless, they are not preprocessor constructs (see below)*.
* __Fast compilation__. Of course, it depends on the code, but the compiler can emit code for about 8.000 
source code lines per second.
* __Rich expressions__. The compiler can handle most arithmetic and logic operators we have in C, C++, C#
Java, and JavaScript. You can use integer, float, and string expressions. The language support more than 40
functions that you can use in the expressions (e.g: `Amp * sin($cnt * Pi() / 16))`)
* __Rich literal formats__. Decimal, float, hexadecimal, binary, and string literals are at your displosal.
You can use multiple variants for hexadecimal numbers (`$12ae`, #12AE, 0x12AE, 12AEh), and binary numbers
(0b00111100, %00111100, %0011_1100). In strings, you can use ZX Spectrum specific escape codes, for example,
`\i` for INK, `\P` for the pound sign, and many others.
* __Assembler control flow statements__. You can use loops (`loop`, `repeat`..`until`, `while`..`wend`,
`for`..`next`) and conditional statements (`if`) to create an assembler control flow. These constructs 
can be nested and provide local scope for labels, symbols, and variables.
* __Powerful dynamic Macros__. You can create macros with arguments. In the macro bodies, the current values 
of arguments can replace entire instructions, operands, or parts of expressions. Moreover, through arguments,
you can inject multiline instructions and statements into macro declarations.
* __Modules__. You can use modules to serve both as logical containers to separating partitions of the code and namespaces to create scopes for labels and symbols.