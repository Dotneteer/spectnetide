---
layout: documents
categories: 
  - "Z80 Assembler"
title:  "Directives"
alias: directives
seqno: 90
selector: documents
permalink: "documents/directives"
---

The directives of the __SpectNetIDE__ Z80 Assembler representation are used for preprocessing
&mdash; similarly as in the C and C++ programming languages &mdash; though their semantics are
different.

> Although you can add comments to the end of directives, they may not have labels.

## The #IF Directive

You can use this directive for conditional compilation. The argument of the directive is a
conditional expression, and it determines on which branch the compilation goes on. __#IF__
works in concert with __#ELSE__ and __#ENDIF__:

```
; Block #1
#if 2 > 3
    ld a,b
#endif

; Block #2;
#if 2 < 3
    nop
#else
    ld b,c
#endif

; Block #3
#if $ > $+2
    nop
#else
    ld b,c
#endif
```

Here, __Block #1__ does not generate output, since the condition is false. __Block #2__ emits
a `nop`, as the condition is true. The fals condition value in __Block #3__ moves code
parsing to the `#else` branch, so it emits a `ld b,c` instruction.

## The #IFDEF and #IFNDEF Directives

These directives works similarly to #IF. However, these check if a particular symbol has 
(__#IFDEF__) or has not (__#IFNDEF__) defined. So their single argument is an identifier name.

## The #IFMOD and #IFNMOD Directives

These directives works similarly to #IF. However, these check if the code's current model is the one 
specified with the identifier following the __#IFMOD__ or __#IFNMOD__ pragma. Here is a short sample of
using this directive:

```
    .model Spectrum48

#ifmod Spectrum128
    BorderColor: .equ 5
    RetAddr: .equ #2604
#else
    BorderColor: .equ 4
    RetAddr: .equ #12a2
#endif


Start:
	.org #8000
    ld a,BorderColor
    out (#fe),a
    jp RetAddr
```

You can use only these identifiers with this pragma (case-insensitively): `SPECTRUM48`, 
`SPECTRUM128`, `SPECTRUMP3`, `NEXT`.

## The #DEFINE and #UNDEF Directives

With the __#DEFINE__ directive, you can explicitly define a symbol. Such a symbol has no concrete value, 
just its existence. With __#UNDEF__ you may declare a symbol undefined.

```
#define SYMB

; Block #1
#ifdef SYMB
    ld a,b
#endif

#undef SYMB

; Block #2;
#ifdef SYMB
    nop
#else
    ld b,c
#endif
```

According to this definition, the first block emits a `ld, a,b` instruction, the second one a
`ld b,c` instruction.

## The #INCLUDE Directive

You can use this directive to load and process a source file from within another source file.

__#INCLUDE__ accepts a string that names a file with its extension. The file name may contain either
an absolute or a relative path. When a relative path is provided, its strating point is always
the source file that holds the __#INCLUDE__ directive.

Assume that this code is in the `C:\Work` folder:

```
#include "Symbol.z80Asm"
#include "./MyRules.z80Asm"
#include "/Common/scroll.z80Asm"
```

The compiler will check the ```C:\Work``` folder for the first two include filem and
```C:\Work\Commmon``` for the third one.
