# Z80 Assembler Reference

This article is a complete reference of the __SpectNetIde__'s Z80 Assembler implementation. 
The assembler was designed with high performance in mind. Internally it uses the 
[__Antlr__](http://www.antlr.org/) tool for parsing the code.

I was inspired by the ideas of [__Simon Brattel__](http://www.desdes.com/index.htm). Many features 
I've implemented in the __SpectNetIde__ assembler were suggested by the community according to 
Simon's outstanding [Zeus Z80 Assembler](http://www.desdes.com/products/oldfiles/zeus.htm).
I honor his ideas and work.

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
* __Boolean values.__ The following tokens represent Booleans: `.false`, `false`, `.true`, `true`.
* __Decimal numbers.__ You can use up to 5 digits (0..9) to declare a decimal number. Examples:
16, 32768, 2354.
* __Floating point numbers.__ You can use the same notation for floating point numbers as in C/C++/Java/C#.
Here are a few samples:

```
.25
123.456
12.45E34
12.45e-12
3e+4
```

* __Hexadecimal numbers.__ You can use up to 4 hexadecimal digits (0..9, a..f or A..F) to declare
a hexadecimal literal. The compiler looks for one of the `#` `0x` or `$` prefix, or one of 
the `h` or `H` suffixes to recognize them as hexadecimal. If you use the `h` or `H`
suffixes, the hexadecimal number should start with a decimal digit `0`...`9`; otherwise the 
assembler interprets it as an identifier (label).
Here are a few samples:

```
    #12AC
    0x12ac
    $12Ac
    12ACh
    12acH
    0AC34H
```

* __Binary numbers.__ Literal starting with the one of the `%` or `0b` prefix are taken into 
account as binary literals. You can follow the prefix with up to 16 `0` or `1` digits. To make
them more readable, you can separate adjacent digits with the underscore (`_`) character. These 
are all valid binary literals:

```
    %01011111
    0b01011111
    0b_0101_1111
```

> You can use negative number with the minus sign in front of them. Actually, the sign is not
> the part of the numeric literal, it is an operator. 

* __Characters__. You can put a character between single quotes (for example: `'Q'`). 
* __Strings__. You can put a series of character between double quotes (for example: `"Sinclair"`).

> You can use escape sequences to define non-visible or control characters, as you will learn soon.

* __The `$` or `.` tokens__. These literals are equivalent; both represent the current assembly address.

### Identifiers

You can use identifiers to refer to labels and other constants. Identifiers must start with 
a letter (a..z or A..Z) or the underscore character (`_`). The subsequent characters 
letters, digits, or underscores. Here are a few samples:

```
MyCycle
ERR_NO
Cycle_4_Wait  
```
> There are strings that can be both identifiers or hexadecimal literals with the `H` or `h` suffix, like
`AC0Fh` or `FADH`. The assembler considers such strings as identifiers. To use hexadecimal literal, use a `0`
prefix: `0FADH` is a hexadecimal literal, while `FADH` is an identifier.

> Theoretically, you can use as long identifiers as you want. I suggest you to make them no longer than
32 characters so that readers may read your code easily.

### Characters and Strings

You have already learned that you can utilize character and string literals (wrapped into single, or double quotes, 
respectively), such as in these samples:

```
"This is a string. The next sample is a single character:"
'c'
```

ZX Spectrum has a character set with special control characters such as AT, INK, PAPER, and so on.

The __SpectNetIde__ assembler allows you to define them with special escape sequences:

Escape | Code | Character
-------|------|----------
`\i` | 0x10 | INK 
`\p` | 0x11 | PAPER
`\f` | 0x12 | FLASH
`\b` | 0x13 | BRIGHT
`\I` | 0x14 | INVERSE
`\o` | 0x15 | OVER
`\a` | 0x16 | AT
`\t` | 0x17 | TAB
`\P` | 0x60 | pound sign
`\C` | 0x7F | copyright sign
`\\` | 0x5C | backslash
`\'` | 0x27 | single quote
`\"` | 0x22 | double quote
`\0` | 0x00 | binary zero

> Observe, some of these sequences have different values than their corresponding
> pairs in other languages, such as C, C++, C#, or Java.

To declare a character by its binary code, you can use the `\xH` or  
`\xHH` sequences (`H` is a hexadecimal digit). For example, these
escape sequence pairs are equivalent:

```
"\i"
"\x10"

"\C by me"
"\x7f \x62y me"
```

## Expressions
The Z80 assembler has a rich syntax for evaluating expressions. You can use operands 
and operators just like in most programming languages. Nevertheless, the __SpectNetIde__
implementation has its particular way of evaluating expressions:

* Expressions can be one of these types: 
  * *Booleans* (`true` or `false`)
  * *integers* (64-bit) 
  * *floating point numbers* (64-bit precision)
  * *strings* (with 8-bit characters)

* The assembler applies implicit conversion whenever it's possible.
  * Floating point numbers are truncated to integer values.
  * The `true` Boolean literal is represented with the integer value `1`; `false` with `0`.
  * When the assembler needs a Boolean value, `0` is taken into account as `false`, any other values as `true`.
  * There is no implicit conversion between strings and any numeric values.
* When the compiler needs a 16-bit value (for example, `ld hl,NNNN`), it uses the rightmost 
16 bits of an expression's value.
* When a Z80 operation (for example, `ld a,NN`) needs an 8-bit value, it utilizes the 
rightmost 8 bits.

> In the future, these compiler features may change by issuing a warning in case of
> arithmetic overflow.

* Instead of parentheses &mdash; `(` and `)` &mdash; you can use square brackets 
&mdash; `[` and `]` &mdash; to group operations and change operator precedence.

```
; This is a syntax error
ld hl,(Offset+#20)*2+BaseAddr

; This is valid
ld hl,[Offset+#20]*2+BaseAddr
```

### Operands
You can use the following operands in epressions:
* Boolean, Decimal and hexadecimal literals
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

### Functions

The Z80 assembler provides a number of functions that can have zero, one, or more arguments. 
Several functions (for example as `rnd()`) have overloads with different signatures. Each 
function has a name and a parameter list wrapped into parentheses, the parameters are separated
by a comma. Of course, parameters can be expressions, and they may invoke other functions, too.
Here are a few samples:

```
lenght("Hello" + " world")
max(value1, value2)
sin(pi()/2)
sqrt(pear + 3.0)
```

The __SpectNetIde__ support these function signatures:

Signature | Value | Description 
----------|-------|------------
`abs(integer)` | `integer` |The absolute value of an *integer* number.
`abs(float)` | `float` | The absolute value of a *float* number.
`acos(float)` | `float` | The angle whose cosine is the specified number.
`asin(float)` | `float` | The angle whose sine is the specified number.
`atan(float)` | `float` | The angle whose tangent is the specified number.
`atan2(float, float)` | `float` | The angle whose tangent is the quotient of two specified numbers.
`ceiling(float)` | `float` | The smallest integral value that is greater than or equal to the specified number.
`cos(float)` | `float` | The cosine of the specified angle.
`cosh(float)` | `float` | The hyperbolic cosine of the specified angle.
`exp(float)` | `float` | __e__ raised to the specified power.
`fill(string, integer)` | `string` | Creates a new string by concatenating the specified one with the given times.
`floor(float)` | `float` | The largest integer less than or equal to the specified number.
`frac(float)` | `float` | The fractional part of the specified number.
`high(integer)` | `integer` | The leftmost 8 bits (MSB) of a 16-bit integer number.
`int(float)` | `integer` | The integer part of the specified number.
`left(string, integer)` | `string` | Takes the leftmost characters of the string with the length specified.
`len(string)` | `integer` | The length of the specified string.
`length(string)` | `integer` | The length of the specified string.
`log(float)` | `float` | The natural (base __e__) logarithm of a specified number.
`log(float, float)` | `float` | The logarithm of a specified number in a specified base.
`log10(float)` | `float` | The base 10 logarithm of a specified number.
`low(integer)` | `integer` | The rightmost 8 bits (LSB) of an integer number.
`max(integer, integer)` | `integer` |  The larger of two *integer* numbers.
`max(float, float)` | `float` | The larger of two *float* numbers.
`min(integer, integer)` | `integer` |  The smaller of two *integer* numbers.
`min(float, float)` | `float` | The smaller of two *float* numbers.
`nat()` | `float` | Represents the natural logarithmic base, specified by the constant, __e__.
`pi()` | `float` | Represents the ratio of the circumference of a circle to its diameter, specified by the constant, __&pi;__.
`pow(float, float)` | `float` | The specified number raised to the specified power.
`right(string, integer)` | `string` | Takes the rightmost characters of the string with the length specified.
`round(float)` | `float` | Rounds a *float* value to the nearest integral value.
`round(float, int)` | `float` | Rounds a *float* value to a specified number of fractional digits.
`rnd()` | `integer` | Returns a random 32-bit number.
`rnd(integer, integer)` | `integer` | Returns a random 32-bit integer between the first and second number.
`sign(integer)` | `integer` | Returns an integer that indicates the sign of an *integer* number.
`sign(float)` | `integer` | Returns an integer that indicates the sign of a *float* number.
`sin(float)` | `float` | The sine of the specified angle.
`sinh(float)` | `float` | The hyperbolic sine of the specified angle.
`sqrt(float)` | `float` | The square root of a specified number.
`substr(string, integer, integer)` | `string` | Takes a substring of the specified string from the given position (zero-based) and length.
`tan(float)` | `float` | The tangent of the specified angle.
`tanh(float)` | `float` | The hyperbolic tangent of the specified angle.
`truncate(float)` | `integer` | Calculates the integral part of a specified number.
`word(integer)` | `integer` | The rightmost 16 bits of an integer number.

Functions have the same precedence as the unary operators (such as the unary `+` and `-`).

## Z80 Instructions

__SpectNetIde__ implements Every officially documented Z80 instruction as well as the 
non-official ones. During the implementation I used [ClrHome.org](http://clrhome.org/table/)
as a reference.

Z80 instructions may start with a label. Labels are identifiers that can be terminated by an
optional colon (`:`). Both labels in these samples are accepted by the compiler:

```
Start: ld b,#f0
Wait   djnz Wait
```

### Z80 Mnemonics

The compiler accepts these mnemonics:

`ADC`, `ADD`, `AND`, `BIT`, `CALL`, `CCF`, `CP`, `CPD`,
`CPDR`, `CPI`, `CPIR`, `CPL`, `DAA`, `DEC`, `DI`, `DJNZ`,
`EI`, `EX`, `EXX`, `HALT`, `IM`, `IN`, `INC`, `IND`,
`INDR`, `INI`, `INIR`, `JP`, `JR`, `LD`, `LDD`, `LDDR`, `LDDRX`&ast;, `LDDX`&ast;, 
`LDI`, `LDIR`, `LDIRSCALE`&ast;, `LDIRX`&ast;, `LDIX`&ast;, `LDPIRX`&ast;, `MIRROR`&ast;, `MUL`&ast;, `NEG`, 
`NEXTREG`&ast;, `NOP`, `OR`, `OTDR`, `OTIR`, `OUT`, `OUTINB`&ast;,
`OUTD`, `OUTI`, `PIXELAD`&ast;, `PIXELDN`&ast;, `POP`, `PUSH`, `RES`, `RET`, `RETI`, `RETN`,
`RL`, `RLA`, `RLC`, `RLCA`, `RLD`, `RR`, `RRA`, `RRC`,
`RRCA`, `RRD`, `RST`, `SBC`, `SCF`, `SET`, `SETAE`&ast;, `SLA`, `SLL`
`SRA`, `SRL`, `SUB`, `SWAPNIB`&ast;, `TEST`&ast;, `XOR`.

> The instructions marked with &ast; can be used only with the ZX Spectrum Next model.

### Z80 Registers

The compiler uses the standard 8-bit and 16-bit register names, as specified in the official 
Zilog Z80 documentation:

* 8-bit registers: `A`, `B`, `C`, `D`, `E`, `H`, `L`, `I`, `R`
* 16-bit registers: `AF`, `BC`, `DE`, `HL`, `SP`, `IX`, `IY`
* For the 8-bit halves of the `IX` and `IY` index registers, the compiler uses these names:
`XL`, `XH`, `YL`, `YH`. Alternatively, the compiler accepts these names, too: 
`IXL`, `IXH`, `IYL`, `IYH`. As a kind of exception to general naming conventions, 
these mixed-case names are also accepted: `IXl`, `IXh`, `IYl`, `IYh`.

### JP Syntax

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
The __SpectNetIde__ compiler accepts both notation.

### ALU operations syntax

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

The __SpectNetIde__ compiler accepts the second group of ALU operations with using the explicit 
`A` operand, too:

```
sub a,e
and a,(hl)
xor a,e
or a,c
cp a,b
```

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

When the compiler emits the code for the `ld hl,Table` instruction, it does not know the
value of `Table`, as this symbol will receive its value only later. In the first trip, the
compiler records the fact that later it should use the value of `Table` to complete the 
`ld hl,NNNN` instruction.
While emitting the code, the compiler reaches the `ld bc,#4000` instruction and at that point 
it has a value for `Table`.
After the code is emitted, in the second &mdash; fixup &mdash; trip, the compiler is able to replace
`Table` with its value.

There might be situations when the compiler cannot resolve symbolic values. In this case it signs an error.
For example, in this example there is a circular reference between `Addr1` and `Addr2`:

```
Addr1: .equ Addr2+#20
       ; ...
Addr2  .equ Addr1-#10
```

## Pragmas

The compiler understands several pragmas that &mdash; thought they are not Z80 
instructions &mdash; they influence the emitted code. Each pragma has two alternative syntax,
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

You can apply multiple __ORG__ pragmas in your source code. Each usage creates a new segment in the
assembler output. Take a look at this code:

```
    ld h,a
    .org #8100
    ld d,a
    .org #8200
    ld b,a
```

This code generates three output segment, each with one emitted byte that represents the 
corresponding `LD` operation. The first segment will start at 0x8000 (default), 
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

The `.ent $` pragma will sign the address of the `jp #6100` isntruction as the entry
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
`$` token that represents the current assembly address. Your code is placed according 
to the __ORG__ of the particular output segment, but the assembly address is always displaced
with the value according to __DISP__. Take a look at this sample:

```
    .org #6000
    .disp #1000
    ld hl,$
```

The `ld hl,$` instruction will be placed to the 0x6000 address, but it will be equivalent
with the `ld hl,#7000` statement due to the `.disp #1000` displacement.

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

```
        .org #6200
        ld hl,#4000 ; Sym1 <-- #4000
        ld bc,#620a ; Sym2 <-- #620a as an ld bc,NNNN operation and
                               an ld hl,NNNN each takes 3 bytes
```

### The VAR pragma

The __VAR__ pragma works similarly to __EQU__. However, while __EQU__ does not allow using the same symbol
with mulitple value assignments, __VAR__ assigns a new value to the symbol every time it is used.

> The VAR pragma accepts extra syntax alternatives: `=`, `:=`

### The DEFB pragma

The __DEFB__ pragma emits 8-bit expressions (bytes) from the current assembly position.
here is a sample:

```
    .org #6000
    .defb #01, #02, $, #04
```

The __DEFB__ pragma will emit these four bytes starting at 0x6000: 0x01, 0x02, 0x03, 0x04.
The `$` expression will emit 0x03, because at the emission point the current assembly
address is 0x6003. The __DEFB__ program takes into account only the rightmost 8 bits of any
expression: this is how `$` results in 0x03.

> __DEFB__ has extra syntax variants: `db`, `.db`, `DB`, and `.DB` are accepted, too.

### The DEFW pragma

The __DEFW__ pragma is similar to __DEFB__, but it emits 16-bit values with LSB, MSB order.

```
    .defw #1234, #abcd
```

This simple code above will emit these four bytes: 0x34, 0x12, 0xcd, 0xab.

> __DEFW__ has extra syntax variants: `dw`, `.dw`, `DW`, and `.DW` are accepted, too.

### The DEFM pragma

The __DEFM__ pragma emits the byte-array representation of a string. Each character
in the string is replaced with the correcponding byte. Tak a look at this code:

```
    .defm "\C by me"
```

Here, the __DEFM__ pragma emits 7 bytes for the seven characters (the first escape 
sequence represents the copyrigh sign) : 0x7f, 0x20, 0x62, 0x69, 0x20, 0x6d, 0x65.

> __DEFM__ has extra syntax variants: `dm`, `.dm`, `DM`, and `.DM` are accepted, too.

### The DEFS pragma

You can emit zero (`0x00`) bytes with this pragma. It accepts a single argument,
the number of zeros to emit. This code sends 16 zeros to the generated output:

```
    .defs 16
```

### The FILLB pragma

With __FILLB__, you can emit a particular count of a specific byte. The first argument
of the pragma sets the count, the second specifies the byte to emit. This code emits 24
bytes of `#A5` values:
```
    .fillb 24,#a5
```

### The FILLW pragma

With __FILLW__, you can emit a particular count of a specific 16-bit word. The first argument
of the pragma sets the count, the second specifies the word to emit. This code emits 8
words (16 bytes) of `#12A5` values:
```
    .fillw 8,#12a5
```

Of course, the bytes of a word are emitted in LSB/MSB order.


### The SKIP pragma

The __SKIP__ pragma &mdash; as its name suggests &mdash; skips the number of bytes
as specified in its argument. It fills up the skipped bytes with 0xFF.

### The EXTERN pragma

The __EXTERN__ pragma is kept for future extension. The current compiler accepts it, but
does not do any action when observing this pragma.

### The MODEL pragma

This pragma is used when you run or debug your Z80 code within the emulator. With Spectrum 128K, Spectrum +3, 
and Spectrum Next models, you can run the Z80 code in differend contexts. The __MODEL__ pragma lets you
specify on which model you want to run the code. You can use the `SPECTRUM48`, `SPECTRUM128`, 
`SPECTRUMP3`, or `NEXT` identifiers to choose the model (identifiers are case-insensitive):

```
.model Spectrum48
.model Spectrum128
.model SpectrumP3
.model Next
```

For example, when you create code for Spectrum 128K, and add the `.model Spectrum48` pragma to the code,
the __Run Z80 Code__ command will start the virtual machine, turns the machine into Spectrum 48K mode, and ignites
the code just after that.

_Note_: With the `#ifmod` and `#ifnmod` directives, you can check the model type. For example, the following
Z80 code results green background on Spectrum 48K, cyan an Spectrum 128K:

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

### The ALIGN pragma

This pragma allows you to align the current assembly counter to the specified byte boundary. 
You can use this pragma with an optional expression. Look at these samples:

```
.org #8000
    nop
.align 4
    nop
.align
```

The first pragma aligns the assembly counter to #8004, as this one is the next 4-byte boundary.
With no value specified, `.align` uses #100, and thus the second `.align` in the sample sets
the current assembly counter to the next page boundary, #8100.

### The TRACE and TRACEHEX pragmas

These pragmas send trace information to the assembler output. In the Visual Studio IDE, these
messages are displayed in the Z80 Build Output window pane. You can list one or more expressions 
separated by a comma after the `.trace` token. TRACEHEX works just like TRACE, but id displays 
integer numbers and strings in hexadecimal format.

Let'assume, you add these lines to the source code:

```
.trace "Hello, this is: ", 42
.tracehex "Hello, this is: ", 42
```

When you compile the source, the lines above display these messages:

```
TRACE: Hello, this is: 42
TRACE: 48656C6C6F2C20746869732069733A20002A
```

### The RNDSEED pragma

With the `rnd()` function, you can generate random numbers. The RNDSEED pragma sets the seed
value to use for random number generation. If you use this pragma with an integer expression,
the seed is set to tha value of that expression. If you do not provide the expression, the compiler
uses the system clock to set up the seed.

```
.rndseed ; sets the seed according to the system clock
.rndseed 123 ; sets the seed to 123
```

### The DEFG pragma

This pragma helps you define bitmaps in the code. This pragma excepts a string expression 
and utilizes that string as a pattern to generate bytes for the bitmap.

> __DEFG__ has extra syntax variants: `dg`, `.dg`, `DG`, and `.DG` are accepted, too.

If the very first character of the string pattern is `<`, the pattern is left aligned, 
and starts with the second character. Should the first character be `>`, the pattern is 
right aligned and starts with the second character. By default, (if no `<` or `>` is used)
the pattern is left-aligned.

Any space within the pattern are ignored, taken into account as helpers. Other characters
are converted into bits one-by-one.

Before the conversion, the pragma checks if the pattern constitutes multiples of 8 bits.
If not, it uses zero bit prefixes (right-aligned), or zero-bit suffixes (left-aligned)
so that the pattern would be adjusted to contain entire bytes.

The `.` (dot), `-` (dash), and `_` (underscore) sign 0, any other characters stand for 1. 
Every 8 bits in the pattern emit a byte.

Here are a few samples:

```
.dg "....OOOO"         ; #0F
.dg ">....OOOO"        ; #0F
.dg "<----OOOO"        ; #0F
.dg "___OOOO"          ; #1E
.dg "....OOOO ..OO"    ; #0F, #30
.dg ">....OO OO..OOOO" ; #03, #CF
```

## Directives

The directives of the __SpectNetIde__ Z80 Assembler representation are used for preprocessing
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
a `nop`, as the condition is true. The fals condition value in __Block #3__ moves code
parsing to the `#else` branch, so it emits a `ld b,c` instruction.

### The #IFDEF and #IFNDEF Directives

These directives works similarly to #IF. However, these check if a particular symbol has 
(__#IFDEF__) or has not (__#IFNDEF__) defined. So their single argument is an identifier name.

### The #IFMOD and #IFNMOD Directives

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

According to this definition, the first block emits a `ld, a,b` instruction, the second one a
`ld b,c` instruction.

### The #INCLUDE Directive

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

 