# Z80 Assembler Reference

This article is a complete reference of the __spectnetide__'s Z80 Assembler implementation. 
The assembler was designed with high performance in mind. Internally it uses the 
[__Antlr__](http://www.antlr.org/) tool for parsing the code.

## Syntax Basics

The assembler language uses a special way of case-sensitivity. You can write the reserved
words (such as assembly instructions, pragmas, or directives) either with lowercase or
uppercase letters, but you cannot mix these cases. For example, this instructions use
proper syntax:

```
    LD c,A
    JP #12ac
    ldir
    djnz MyLabel
```

However, in these samples, character cases are mixed, and do the compiler will refuse them:

```
    Ld c,A
    Jp #12ac
    ldIR
    djNZ MyLabel
```

In symbolic names (labels, identifiers, etc.), you can mix lowercase and uppercase letters. Nonetheless, the compiler applies
case-insensitive comparison when mathcing symbolic names. So, these statement pairs are totally equivalent with
each other:
```
    jp MainEx
    jp MAINEX

    djnz mylabel
    djnz MyLabel

    ld hl,ErrNo
    ld hl,errNo
```

### Language Structure

Each line of the source code is a declaration unit and is parsed in its own context. Such a 
source code line can be one of these constructs:
* A Z80 instruction can be directly compiled to binary code (such as ```ld bc,#12AC```)
* A directive that is used by the preprocessor of the compiler (e.g ```#include```, ```#if```, etc.)
* A pragma that emits binary output or instructs the compiler for something (```.org```, ```.defb```, etc)
* A comment that helps the understanding of the code.

### Comments

Comments start with a semicolon (```;```). The compiler takes the rest of the line into account as the body
of the comment. This sample illustrates this concept:

```
; This line is comment-only line
Wait:   ld b,8
Wait1:  djnz Wait1 ; wait while the counter reaches zero
```

> If you need multi-line comments, you can add single-line comments after each other. 
> The Z80 assembly in __spectnetide__ does not have separate multi-line comment syntax.

### Literals

The language syntax provides these types of literals:
* __Decimal numbers.__ You can use up to 5 digits (0..9) to declare a decimal number. Examples:
16, 32768, 2354.
* __Hexadecimal numbers.__ You can use up to 4 hexadecimal digits (0..9, a..f or A..F) to declare
a hexadecimal literal. The compiler looks for one of the ```#``` or ```0x``` prefix, or one of 
the ```h``` or ```H``` suffixes to recognize them as hexadecimal. Here are a few samples:

```
    #12AC
    0x12ac
    12ACh
    12acH
```

> You can use negative number with the minus sign in front of them. Actually, the sign is not
> the part of the numeric literal, it is an operator. 

* __Characters__. You can put a character between double quotes (for example: ```"Q"```). 
* __Strings__. You can put a series of character between double quotes (for example: ```"Sinclair"```).

> You can use escape sequences to define non-visible or control characters, as you will learn soon.

* __The ```$``` token__. This literal represents the current assembly address.

### Identifiers

You can use identifiers to refer to labels and other constants. Identifiers must start with 
a letter (a..z or A..Z) or the underscore character (```_```). The subsequent characters 
can be digits (0..9), too. Here are a few samples:

```
MyCycle
ERR_NO
Cycle_4_Wait  
```

> Theoretically, you can use as long identifiers as you want. I suggest you to make them no longer than
32 characters so that readers may read your code easily.

### Characters and Strings

You have already learned that you can utilize character and string literals (wrapped into double quotes), such as in these samples:

```
"This is a string. The next sample is a single character"
"c"
```

ZX Spectrum has a character set with special control characters such as AT, INK, PAPER, and so on.

The __spectnetide__ assembler allows you special escape sequences to define them:

Escape | Code | Character
-------|------|----------
```\i``` | 0x10 | INK 
```\p``` | 0x11 | PAPER
```\f``` | 0x12 | FLASH
```\b``` | 0x13 | BRIGHT
```\I``` | 0x14 | INVERSE
```\o``` | 0x15 | OVER
```\a``` | 0x16 | AT
```\t``` | 0x17 | TAB
```\P``` | 0x60 | pound sign
```\C``` | 0x7F | copyright sign
```\\``` | 0x5C | backslash
```\'``` | 0x27 | single quote
```\"``` | 0x22 | double quote
```\0``` | 0x00 | binary zero

> Observe, some of these sequences have different values than their corresponding
> pairs in other languages, such as C, C++, C#, or Java.

To declare a character by its binary code, you can use the ```\xH``` or  
```\xHH``` sequences (```H``` is a hexadecimal digit). For example, these
escape sequence pairs are equivalent:

```
"\i"
"\x10"

"\C by me"
"\x7f \x62y me"
```

## Expressions
The Z80 assembler has a rich syntax for evaluating expressions. You can use operands 
and operators just like in most programming languages. Nevertheless, the __spectnetide__
implementation has its particular way of evaluating expressions:
* All expressions are evaluated as 16-bit unsigned integers. Overflows are ignored: the compiler
internally uses 32-bit integers during the evaluations and keeps only the rightmost 16 bits
of the result.
* When a Z80 operation (for example, ```ld a,NN```) needs an 8-bit value, the rightmost 8-bits
are used.

> In the future, these compiler features may change by issuing a warning in case of
> arithmetic overflow.

* In logical expressions, any non-zero value represents ```true```; the zero value 
* stands for ```false```.
* Instead of parentheses &mdash; ```(``` and ```)``` &mdash; you can use square brackets 
&mdash; ```[``` and ```]``` &mdash; to group operations.

```
; This is a syntax error
ld hl,(Offset+#20)*2+BaseAddr

; This is valid
ld hl,[Offset+#20]*2+BaseAddr
```

### Operands
You can use the following operands in epressions:
* Decimal and hexadecimal literals
* Character literals
* Identifiers
* The ```$``` token

> String literals cannot be used as operands.

### Operators

You can use about a dozen operators, including unary, binary and ternary ones. In this section
you will learn about them. I will introduce them in descending order of their precendence.

#### Conditional Operator

The assembler supports using only one ternary operator, the conditional operator:

_conditional-expression_ __```?```__ _true-value_ __```:```__ _false-value_

This operation results in -1:

```2 > 3 ? 2 : -1```

When the _conditional-expression_ evaluates to true, the operation results 
in _true-value_; otherwise in _false-value_.

> Conditional expressions are evaluated from right to left, in contrast to binary operators,
> which use left-to-right evaluation.

#### Binary Bitwise Operators

Operator token | Precedence | Description
---------------|------------|------------
```|``` | 1 | Bitwise OR
```^``` | 2 | Bitwise XOR
```&``` | 3 | Bitwise AND

#### Relational Operators

Operator token | Precedence | Description
---------------|------------|------------
```==``` | 4 | Equality
```!=``` | 4 | Non-equality
```<```  | 5 | Less than
```<=``` | 5 | Less than or equal
```>```  | 5 | Greater than
```>=``` | 5 | Greater than or equal

#### Shift Operators

The bits of the left operand are shifted by the number of bits given by the right operand.

Operator token | Precedence | Description
---------------|------------|------------
```<<``` | 6 | Shift left
```>>``` | 6 | Shift right

#### Basic Arithmetic Operators

Operator token | Precedence | Description
---------------|------------|------------
```+``` | 7 | Addition
```-``` | 7 | Subtraction
```*``` | 8 | Multiplication
```/``` | 8 | Division
```%``` | 8 | Modulo calculation

#### Unary operators

Operator token | Precedence | Description
---------------|------------|------------
```+``` | 9 | Unary plus
```-``` | 9 | Unary minus

> Do not forget, you can change the defult precendence with ```[``` and ```]```.

## Z80 Instructions

__spectnetide__ implements Every officially documented Z80 instruction as well as the 
non-official ones. During the implementation I used [ClrHome.org](http://clrhome.org/table/)
as a reference.

Z80 instructions may start with a label. Labels are identifiers that can be terminated by an
optional colon (```:```). Both labels in these samples are accepted by the compiler:

```
Start: ld b,#f0
Wait   djnz Wait
```

### Z80 Mnemonics

The compiler accepts these mnemonics:

```ADC```, ```ADD```, ```AND```, ```BIT```, ```CALL```, ```CCF```, ```CP```, ```CPD```,
```CPDR```, ```CPI```, ```CPIR```, ```CPL```, ```DAA```, ```DEC```, ```DI```, ````DJNZ````,
```EI```, ```EX```, ```EXX```, ```HALT```, ```IM```, ```IN```, ```INC```, ```IND```,
```INDR```, ```INI```, ```INIR```, ```JP```, ```JR```, ```LD```, ```LDD```, ```LDDR```,
```LDI```, ```LDIR```, ```NEG```, ```NOP```, ```OR```, ```OTDR```, ```OTIR```, ```OUT```,
```OUTD```, ```OUTI```, ```POP```, ```PUSH```, ```RES```, ```RET```, ```RETI```, ```RETN```,
```RL```, ```RLA```, ```RLC```, ```RLCA```, ```RLD```, ```RR```, ```RRA```, ```RRC```,
```RRCA```, ```RRD```, ```RST```, ```SBC```, ```SCF```, ```SET```, ```SLA```, ```SLL```
```SRA```, ```SRL```, ```SUB```, ```XOR```.

### Z80 Registers

The compiler uses the standard 8-bit and 16-bit register names, as specified in the official 
Zilog Z80 documentation:

* 8-bit registers: ```A```, ```B```, ```C```, ```D```, ```E```, ```H```, 
```L```, ```I```, ```R```
* 16-bit registers: ```AF```, ```BC```, ```DE```, ```HL```, ```SP```, ```IX```, ```IY```
* For the 8-bit halves of the ```IX``` and ```IY``` index registers, the compiler uses these names:
```XL```, ```XH```, ```YL```, ```YH```

### JP Syntax

Z80 assemblers use two different syntax for the indirect ```JP``` statements:

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
The __spectnetide__ compiler accepts both notation.

### Expression Evaluation

The compiler evaluates expressions in two trips. First, when the lines of the assembly codes are
successfully parsed, and the compiler emits the output. This time might be expressions that cannot
be promptly evaluated. So, after the code is emitted, the compiler start fixup trip to resolve
the values of symbols that remained unresolved in the first trip.
This a sample demonstrates this situation:

```
        ld hl,Table
        ld a,#20
        ; ...
Table:  ld bc,#4000
```

When the compiler emits the code for the ```ld hl,Table``` instruction, it does not know the
value of ```Table```, as this symbol will receive its value only later. In the first trip, the
compiler records the fact that later it should use the value of ```Table``` to complete the 
```ld hl,NNNN``` instruction.
While emitting the code, the compiler reaches the ```ld bc,#4000``` instruction and at that point 
it has a value for ```Table```.
After the code is emitted, in the second &mdash; fixup &mdash; trip, the compiler is able to replace
```Table``` with its value.

There might be situations where the compiler cannot resolve symbolic values. In this case it signs an error.
For example, in this example there is a circular reference between ```Addr1``` and ```Addr2```:

```
Addr1: .equ Addr2+#20
       ; ...
Addr2  .equ Addr1-#10
```

## Pragmas

The compiler understands several pragmas that &mdash; thought they are not Z80 
instructions &mdash; influence the emitted code. Each pragma has two alternative syntax,
one with a dot prefix and another without it. 

For example, you can write ```ORG``` or ```.ORG``` to use the __ORG__ pragma.

> I prefer using the dot-prefixed versions of pragmas.

### The ORG pragma

With the __ORG__ pragma, you define where to place the compiled Z80 code when you run it.
For example, the following line sets this location to the 0x6000 address:

```
.org #6000
```

If you do not use __ORG__, the default address is 0x8000.

You can apply multiple __ORG__ in your source code. Each usage creates a new segment in the
assembler output. Take a look at this code:

```
    ld h,a
    .org #8100
    ld d,a
    .org #8200
    ld b,a
```

This code generates three output segment, each with one emitted byte that represents the 
corresponding ```LD``` operation. The first segment will start at 0x8000 (default), 
the second at 0x8100, whilst the third at 0x8200.

### The ENT pragma

The __ENT__ pragma defines the entry code of the program when you run it from Visual Studio.
If you do not apply __ENT__ in your code, the entry point will be the first address of the 
very first output code segment. Here's a sample:

```
    .org #6200
    ld hl,#4000
    .ent $
    jp #6100

    .org #6100
    call MyCode
    ...
```

The ```.ent $``` pragma will sign the address of the ```jp #6100``` isntruction as the entry
address of the code. Should you omit the __ENT__ pragma from this code, the entry point would be
0x6200, for this is the start of the very first output segment, even though there is another
segment starting at 0x6100.

### The XENT pragma

The IDE provides a command, __Export Z80 Program__, which allows you to create a LOAD block
that automatically starts the code. The __Run Z80 Program__ and __Debug Z80 Program__ command
simply jump to the address you specify with the __ENT__ pragma. However, the auto LOAD block uses
the __`RANDOMIZE USR address`__ pattern where you need to define a different entry address that
can be closed with a __`RET`__ statement. The __XENT__ pragma sets this address.
 Here's a sample:

```
start: 
	.org #8000
	.ent #8000
	call SetBorder
	jp #12ac
SetBorder:
	.xent $
	ld a,4
	out (#fe),a
	ret
```

The IDE will use #8000 &mdash; according to the `.ent #8000` pragma &mdash; when starting
the code with the __Run Z80 Program__. Nonetheless, the __Export Z80 Program__ will offer #8006
&mdash; according to the `.xent $` pragma &mdash; as the startup code address.

### The DISP pragma

The __DISP__ pragma allows you to define a displacement for the code. The value affects the
```$``` token that represents the current assembly address. Your code is placed according 
to the __ORG__ of the particular output segment, but the assembly address is always displaced
with the value according to __DISP__. Take a look at this sample:

```
    .org #6000
    .disp #1000
    ld hl,$
```

The ```ld hl,$``` instruction will be placed to the 0x6000 address, but it will be equivalent
with the ```ld hl,#7000``` statement due to the ```.disp #1000``` displacement.

> Of course, you can use negative displacement, too.

### The EQU pragma

The __EQU__ pragma allows you assign a value to an identifier. The label before __EQU__ is the
name of the identifier (or symbol), the exression used in __EQU__ is the value of the variable.
This is a short sample:

```
        .org #6200
        ld hl,Sym1
Sym1:   .equ #4000
        ld bc,Sym2
Sym2:   .equ $+4
```

This sample is equivalent with this one:

        .org #6200
        ld hl,#4000 ; Sym1 <-- #4000
        ld bc,#620a ; Sym2 <-- #620a as an ld bc,NNNN operation and
                               an ld hl,NNNN each takes 3 bytes

### The DEFB pragma

The __DEFB__ pragma emits 8-bit expressions (bytes) from the current assembly position.
here is a sample:

```
    .org #6000
    .defb #01, #02, $, #04
```

The __DEFB__ pragma will emit these four bytes starting at 0x6000: 0x01, 0x02, 0x03, 0x04.
The ```$``` expression will emit 0x03, because at the emission point the current assembly
address is 0x6003. The __DEFB__ program takes into account only the rightmost 8 bits of any
expression: this is how ```$``` results in 0x03.

### The DEFW pragma

The __DEFW__ pragma is similar to __DEFB__, but it emits 16-bit values with LSB, MSB order.

```
    .defw #1234, #abcd
```

This simple code above will emit these four bytes: 0x34, 0x12, 0xcd, 0xab.

### The DEFM pragma

The __DEFM__ pragma emits the byte-array representation of a string. Each character
in the string is replaced with the correcponding byte. Tak a look at this code:

```
    .defm "\C by me"
```

Here, the __DEFM__ pragma emits 7 bytes for the seven characters (the first escape 
sequence represents the copyrigh sign) : 0x7f, 0x20, 0x62, 0x69, 0x20, 0x6d, 0x65.

### The DEFS pragma

You can emit zero (```0x00```) bytes with this pragma. It accepts a single argument,
the number of zeros to emit. This code sends 16 zeros to the generated output:

```
    .defs 16
```

### The FILLB pragma

With __FILLB__, you can emit a particular count of a specific byte. The first argument
of the pragma sets the count, the second specifies the byte to emit. This code emits 24
bytes of ```#A5``` values:
```
    .fillb 24,#a5
```

### The FILLW pragma

With __FILLW__, you can emit a particular count of a specific 16-bit word. The first argument
of the pragma sets the count, the second specifies the word to emit. This code emits 8
words (16 bytes) of ```#12A5``` values:
```
    .fillw 8,#12a5
```

Of course, the bytes of a word are emitted in LSB/MSB order.


### The SKIP pragma

The __SKIP__ pragma &mdash; as its name suggests &mdash; skips the number of bytes
as specified in its argument. It fills up the skipped bytes with 0xff.

### The EXTERN pragma

The __EXTERN__ pragma is kept for future extension. The current compiler accepts it, but
does not do any action when observing this pragma.

## Directives

The directives of the __spectnetide__ Z80 Assembler representation are used for preprocessing
&mdash; similarly as in the C and C++ programming languages &mdash; though their semantics are
different.

> Although you can add comments to the end of directives, they may not have labels.

### The #IF Directive

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
a ```nop```, as the condition is true. The fals condition value in __Block #3__ moves code
parsing to the ```#else``` branch, so it emits a ```ld b,c``` instruction.

### The #IFDEF and #IFNDEF Directives

These directives works similarly to #IF. However, these check if a particular symbol has 
(__#IFDEF__) or has not (__#IFNDEF__) defined. So their single argument is an identifier name.

### The #DEFINE and #UNDEF Directives

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

According to this definition, the first block emits a ```ld, a,b``` instruction, the second one a
```ld b,c``` instruction.

### The #INCLUDE Directive

You can use this directive to load and process a source file from within another source file.

__#INCLUDE__ accepts a string that names a file with its extension. The file name may contain either
an absolute or a relative path. When a relative path is provided, its strating point is always
the source file that holds the __#INCLUDE__ directive.

Assume that this code is in the ```C:\Work``` folder:

```
#include "Symbol.z80Asm"
#include "./MyRules.z80Asm"
#include "/Common/scroll.z80Asm"
```

The compiler will check the ```C:\Work``` folder for the first two include filem and
```C:\Work\Commmon``` for the third one.

 