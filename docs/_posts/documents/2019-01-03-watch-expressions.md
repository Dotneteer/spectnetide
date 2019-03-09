---
layout: documents
categories: 
  - "Tool Commands"
title:  "Watch Expressions"
alias: watch-expressions
seqno: 70
selector: documents
permalink: "documents/watch-expressions"
---

Although its name suggests that the Watch Memory window can display only values in the memory, the capabilities of this
tool are richer.

You can define *watch expressions* and define their *display format*. For example, if you want to display for consecutive
bytes in the memory pointed by the HL register in bitvector format (32 bits), you can do this with this command:

```
+ [HL :DW] :%32
```

Later, you need to display the 16-bit value in the memory address pointed by HL plus BC. You can use this command:

```
+ [HL + BC :W] :W
```

Watch expressions are arithmetic expression using C-like expression syntax. The engine behind the Watch Memory window
continuously evalutes these expressions, formats them and displays their values.

## Expression Types

When working with expressions, the engine uses for integral types, and automatically converts them:
* __Boolean__: Simple `TRUE` or `FALSE`. Any non-zero value is converted to `TRUE`, zero is `FALSE`.
* __Byte__: 8-bit unsigned value
* __Word__: 16-bit unsigned value
* __Double word__: 32-bit unsigned value

When working with values, the expression engine automatically converts operation result to keep all valuable bits.
For example, when you multiply two bytes, the result will be a word. Multiplying a byte and a word results a double word.
Nonetheless, when you multiply two double words, the result will be a double word, so only the last 32 bits are kept.

Comparisons and other logical operations result in booleans.

> You do not need to deal with types, the evaluation engine does it for you. Sometimes you need signed types. With the
help of the `:-B`, `:-W`, and `:-DW` format specifiers, as you will learn later, you can display values in signed form.

## Value Sources

You have several value sources that you can use in expressions:
* Literal values (decimal, hexadecimal, binary, character)
* Compilation symbols
* Z80 register values
* Z80 CPU flag values
* ZX Spectrum memory contents

> In the future you may use ULA-specific values, or memory values of non-paged memory banks.

## Literals

The expression syntax provides these types of literals:
* __Decimal numbers.__ You can use up to 5 digits (0..9) to declare a decimal number. Examples:
16, 32768, 2354.

* __Hexadecimal numbers.__ You can use up to 4 hexadecimal digits (0..9, a..f or A..F) to declare
a hexadecimal literal. The engine recognizes one of the `#` `0x` or `$` prefix, or one of 
the `h` or `H` suffixes. If you use the `h` or `H`
suffixes, the hexadecimal number should start with a decimal digit `0`...`9`.

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

You can use escape sequences to define non-visible or control characters:

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

To declare a character by its binary code, you can use the `\xH` or  
`\xHH` sequences (`H` is a hexadecimal digit). For example, these
escape sequence pairs are equivalent:

```
'\i'
'\x10'
```

### Symbols

You can use symbols to refer to labels and other constants used in Z80 programs. Identifiers must start with 
a letter (a..z or A..Z) or the underscore character (`_`). The subsequent characters 
letters, digits, or underscores. Here are a few samples:

```
MyCycle
ERR_NO
Cycle_4_Wait  
```
> There are strings that can be both identifiers or hexadecimal literals with the `H` or `h` suffix, like
`AC0Fh` or `FADH`. The engine considers such strings as symbols. To use hexadecimal literal, use a `0`
prefix: `0FADH` is a hexadecimal literal, while `FADH` is an identifier.

The engine usus the symbols within the Z80 program you compile, inject, run or debug.

> The SpecteNetIde Z80 Assembler handles symbols with string values. These symbols retrieve
> 0 (word) value.

### Z80 Registers

The expression engine recognizes the standard 8-bit and 16-bit register names, as specified in the official 
Zilog Z80 documentation:

* 8-bit registers: `A`, `B`, `C`, `D`, `E`, `H`, `L`, `I`, `R`
* 16-bit registers: `AF`, `BC`, `DE`, `HL`, `SP`, `IX`, `IY`
* For the 8-bit halves of the `IX` and `IY` index registers, the engine uses these names:
`XL`, `XH`, `YL`, `YH`. Alternatively, the compiler accepts these names, too: 
`IXL`, `IXH`, `IYL`, `IYH`. As a kind of exception to general naming conventions, 
these mixed-case names are also accepted: `IXl`, `IXh`, `IYl`, `IYh`.
* The engine recognizes the `WZ` (internal Z80 register, not accessible programmatically) register, too.

### Z80 Flags

The expression engine can access the Z80 CPU flags individually, and it also has tokens for the inverted flag values:

Token | Flag
-------|------
{% raw %}`{% endraw %} __P__ | True, if __S__ flag (Bit 7) is set  
{% raw %}`{% endraw %} __M__ | True, if __S__ flag (Bit 7) is reset
{% raw %}`{% endraw %} __Z__ | True, if __Z__ flag (Bit 6) is set
{% raw %}`{% endraw %} __NZ__ | True, if __Z__ flag (Bit 6) is reset
{% raw %}`{% endraw %} __5__ | True, if __R5__ flag (Bit 5) is set
{% raw %}`{% endraw %} __N5__ | True, if __R5__ flag (Bit 5) is reset
{% raw %}`{% endraw %} __H__ | True, if __H__ flag (Bit 4) is set
{% raw %}`{% endraw %} __NH__ | True, if __H__ flag (Bit 4) is reset
{% raw %}`{% endraw %} __3__ | True, if __R3__ flag (Bit 3) is set
{% raw %}`{% endraw %} __N3__ | True, if __R3__ flag (Bit 3) is reset
{% raw %}`{% endraw %} __PE__ | True, if __PV__ flag (Bit 2) is set
{% raw %}`{% endraw %} __PO__ | True, if __PV__ flag (Bit 2) is reset
{% raw %}`{% endraw %} __N__ | True, if __N__ flag (Bit 1) is set
{% raw %}`{% endraw %} __NN__ | True, if __N__ flag (Bit 1) is reset
{% raw %}`{% endraw %} __C__ | True, if __C__ flag (Bit 0) is set
{% raw %}`{% endraw %} __NC__ | True, if __C__ flag (Bit 0) is reset

### ZX Spectrum Memory Contents

You can query the entire 64K memory available by the Z80 CPU. With a memory indirection expression, 
you can query byte, word, and double word values, respectively:

__`[`__ `expression` [`access specifier`] __`]`__

You wrap an *expression* and an optional *access specifier* between square brackets. 
The expression specifies the memory address, the *access specifier* sets the number of bytes to
query from the memory:

* __`:B`__ &mdash; A single byte stored in the specified memory address.
* __`:W`__ &mdash; Two consequtive bytes stored in the specified address and the next one.
* __`:DW`__ &mdash; Four consequtive bytes stored in the specified memory address and the next three.

If you omit the access specifier, the default is `:B`.

When you query multiple bytes from the memory, the first byte is the LSB (least significant byte), 
the last is the MSB (most significant byte). Lets assume, you store these four bytes at the address #4000:

```
#12, #4A, #C3, #78
```
* The `[#4000]` (and `[#4000:B]`) expressions retrieve `#12`.
* The `[#4000:W]` expression results in `#4A12` (16 bits).
* The `[#4000:DW]` expression results in `#78C34A12` (32 bits).

## Operators

You can use about a dozen operators, including unary, binary and ternary ones. In this section
you will learn about them. I will introduce them in descending order of their precendence.

### Conditional Operator

The engine supports using only one ternary operator, the conditional operator:

_conditional-expression_ __`?`__ _true-value_ __`:`__ _false-value_

This operation results in -1:

`2 > 3 ? 2 : -1`

When _conditional-expression_ evaluates to true, the operation results 
in _true-value_; otherwise in _false-value_.

> Conditional expressions are evaluated from right to left, in contrast to binary operators,
> which use left-to-right evaluation.

### Binary Bitwise Operators

Operator token | Precedence | Description
---------------|------------|------------
`|` | 1 | Bitwise OR
`^` | 2 | Bitwise XOR
`&` | 3 | Bitwise AND

### Relational Operators

Operator token | Precedence | Description
---------------|------------|------------
`==` | 4 | Equality
`!=` | 4 | Non-equality
`<`  | 5 | Less than
`<=` | 5 | Less than or equal
`>`  | 5 | Greater than
`>=` | 5 | Greater than or equal

### Shift Operators

The bits of the left operand are shifted by the number of bits given by the right operand.

Operator token | Precedence | Description
---------------|------------|------------
`<<` | 6 | Shift left
`>>` | 6 | Shift right

### Basic Arithmetic Operators

Operator token | Precedence | Description
---------------|------------|------------
`+` | 7 | Addition
`-` | 7 | Subtraction
`*` | 8 | Multiplication
`/` | 8 | Division
`%` | 8 | Modulo calculation

### Unary operators

Operator token | Precedence | Description
---------------|------------|------------
`+` | 9 | Unary plus
`-` | 9 | Unary minus
`~` | 9 | Unary bitwise NOT
`!` | 9 | Unary logical NOT

> Do not forget, you can change the default precendence with `(` and `)`, for example:
> `(4 + 2) * 3`.

## Format Specifiers

With format specifiers, you can declare how would you like to display the results of watch expressions.
These are optional, if you do not use them, the engine will select the format according to the type of 
the expressions's value:

* Boolean values &mdash; __`:F`__ format
* Byte values &mdash; __`:B`__ format
* Word values &mdash; __`:W`__ format
* Double word values &mdash; __`:DW`__ format

This table lists the format specifiers supported by the watch engine:

Specifier | Description
----------| -----------
__`:F`__ | Flag format. When the value is zero, the output is `FALSE`; otherwise, `TRUE`.
__`:B`__ | Byte format. The engine displays both the hexadecimal value in two hexadecimal digits, plus the decimal value. For example: `#7C (124)`
__`:-B`__ | Signed byte format. Similar to __`:B`__, nonetheless, the decimal value is signed. For example, `#FE (-2)`
__`:C`__ | Character format. The value is converted to a byte, and the ASCII character value of the byte is displayed. When the character code is between 32 and 126, the character is displayed, otherwise a hexadecimal character escape is used. For example, `#41` results in __`'A'`__, while `#8D` in `'\0x8D'`.
__`:W`__ | Word format. The engine displays both the hexadecimal value in four hexadecimal digits, plus the decimal value. For example: `#317C (12668)`
__`:-W`__ | Signed word format. Similar to __`:W`__, nonetheless, the decimal value is signed. For example, `#FFFE (-2)`
__`:DW`__ | Double word format. The engine displays both the hexadecimal value in eight hexadecimal digits, plus the decimal value. For example: `#43CA317C (‭1137324412‬)`
__`:-DW`__ | Signed double word format. Similar to __`:DW`__, nonetheless, the decimal value is signed. For example, `#FFFFFFFE (-2)`
__`:H4`__ | 4 hexadecimal digits. The result is converted to a word and displayed in hexadecimal format, bytes in LSB/MSB order. For example, `#4F3C` results in `#3C #4F`.
__`:H8`__ | 8 hexadecimal digits. The result is converted to a double word and displayed in hexadecimal format, bytes in LSB/MSB order. For example, `#67D24F3C` results in `#3C #4F #D2 #67`.
__`:%8`__ | 8 binary digits. The result is converted to a byte and displayed in binary format. For example, `#4F` results in `01001111`.
__`:%16`__ | 16 binary digits. The result is converted to a word and displayed in binary format, bytes in MSB/LSB order. For example, `#324F` results in `00110010 01001111`.
__`:%32`__ | 32 binary digits. The result is converted to a double word and displayed in binary format, bytes in MSB/LSB order. For example, `#AA55324F` results in `10101010 01010101 00110010 01001111`.

> In the future, the list of format specifiers may extend.

Please note, while __`:H4`__ and __`:H8`__ use LSB/MSB byte order, __`:%8`__, __`:%16`__,
and __`:%32`__ apply MSB/LSB order. You may use the __`:H4`__ and __`:H8`__ formats to display memory
contents in the order of bytes as they are stored in the memory. Lets assume, you store these
four bytes at the address #4000:

```
#12, #4A, #C3, #78
```

This is how watch expressions display the content:

* `[#4000]` &mdash; `#12 (18)`
* `[#4000:B]` &mdash; `#12 (18)`
* `[#4000:B] :H4` &mdash; `#12 #00` (Do not forget, the expression reads only a single byte from the memory!)
* `[#4000:W]` &mdash; `#4A12 (18962)` (This expression reads two bytes from the memory)
* `[#4000:W] :H4` &mdash; `#12 #4A` (This expression reads two bytes from the memory)
* `[#4000:DW]` &mdash; `#78C34A12 (‭2026064402‬)` (This expression reads four bytes from the memory)
* `[#4000:DW] :H8` &mdash; `#12 #4A #C3 #78` (This expression reads four bytes from the memory)
