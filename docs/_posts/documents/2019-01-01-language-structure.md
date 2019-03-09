---
layout: documents
categories: 
  - "Z80 Assembler"
title:  "Language structure"
alias: language-structure
seqno: 30
selector: documents
permalink: "documents/language-structure"
---

Each line of the source code is a declaration unit and is parsed in its own context. Such a 
source code line can be one of these constructs:
* A Z80 *instruction*, which can be directly compiled to binary code (such as ```ld bc,#12AC```)
* A *directive* that is used by the preprocessor of the compiler (e.g. ```#include```, ```#if```, etc.)
* A *pragma* that emits binary output or instructs the compiler for about code emission (```.org```, ```.defb```, etc.)
* A *compiler statement* (or shortly, a *statement*) that implements control flow operations for the compiler
(e.g. `.loop`, `.repeat`..`.until`, `.if`..`.elif`..`.else`..`.endif`)
* A *comment* that helps the understanding of the code.

## The Two Set of Symbols

The compiler works with two set of symbols. It uses the first set during the preprocessing phase in the
only in the directives. For example, with the `#define` directive, you define a symbol, with `#undef`
you remove it. Within the expressions you use in directives (such as `#if`), you can refer only to these symbols.

The __SpectNetIde__ option pages provide two options to declare your predefined symbols. When you compile
the code in the IDE, it will use these symbols as if you'd declare them with `#define`.

![Predefined symbols]({{ site.baseurl }}/assets/images/z80-assembler/predefined-symbols.png)

> You can declare multiple symbols and separate them with the `;` character.

The other set of symbols are the one you declare as *labels*, or with the `.equ` or `.var` pragmas
You can use this set everywhere except directives.

This duality is related to the way the compiler works: in the first, preprocessing phase it only
analyses directives. In the second, code emission phase, the compiler does not have any information
about directives, and thus it does not accesses the symbols used in the preprocessor.

## Assembly Language Flavors

I've designed the assembler with supporting multiple syntax flavors in mind. You do not have 
to explicitly declare the type of the syntax you intend to use, just use the flavor you prefer
&mdash; or mix muliple flavors, as you wish.

For example, you can use several mnemonics for defining a series of bytes, such as `.db`, `.defb`,
`db`, `defb`, and both lowecase or uppercase versions are welcome.

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

> In the future, I might implement a compiler option that allows turning off case-insensitivity.

## Comments

__SpectNetIDE__ support two types of comments: _end-of-line_ and _block_ comments.

En-of-line comments start with a semicolon (```;```) or with a double forward slash (`//`). The compiler takes the rest of the line into account as the body
of the comment. This sample illustrates this concept:

```
; This line is comment-only line
Wait:   ld b,8     ; Set the counter
Wait1:  djnz Wait1 // wait while the counter reaches zero
```

Block comments can be put anywhere within an instruction line betwen `/*` and `*/` tokens, until they do not break other tokens. Nonetheless, block comments cannot span multiple lines, they must start and end within the same source code line. All of the block comments in this code snippet are correct:

```
SetAttr:
	ld b,32
fill:
  /* block */
  /* b2 */ ld (hl),a
  inc /* b3 */ hl
  djnz /* b4 */ fill /* b5 */
  ret
```

However, this will result in syntax error:

```
/* 
  This block comment spans multiple lines,
  and thus, it is invalid
*/
SetAttr:
	ld b,32
```


> If you need multi-line comments, you can add single-line comments after each other. 
> The Z80 assembly in __SpectNetIDE__ does not have separate multi-line comment syntax.

## Literals

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

* __Binary numbers.__ Literal starting with the one of the `%` or `0b` prefix, (or, alternatively with the `b` or `B` suffix) are taken into 
account as binary literals. You can follow the prefix with up to 16 `0` or `1` digits. To make
them more readable, you can separate adjacent digits with the underscore (`_`) character. These 
are all valid binary literals:

```
%01011111
0b01011111
0b_0101_1111
0101_1111b
```

* __Octal numbers.__ You can use up to 6 digits (0..7) with an `o`, `O` (letter O), `q`, or `Q` suffix to declare an octal number. Examples:
16, 327, 2354.

> You can use negative number with the minus sign in front of them. Actually, the sign is not
> the part of the numeric literal, it is an operator. 

* __Characters__. You can put a character between single quotes (for example: `'Q'`). 
* __Strings__. You can put a series of character between double quotes (for example: `"Sinclair"`).

> You can use escape sequences to define non-visible or control characters, as you will learn soon.

* __The `$`, `*` or `.` tokens__. These literals are equivalent; all represent the current assembly address.

## Identifiers

You can use identifiers to refer to labels and other constants. Identifiers must start with 
a letter (a..z or A..Z) or with one of these characters: __`` ` ``__ (backtick), __`_`__ (underscore), __`@`__, __`!`__, __`?`__, and __`#`__. The subsequent ones can be digits and any of the start characters except backtick. Here are a few samples:

```
MyCycle
ERR_NO
Cycle_4_Wait
`MyTemp
@ModLocal
IsLastLine?
```
> There are strings that can be both identifiers or hexadecimal literals with the `H` or `h` suffix, like
`AC0Fh` or `FADH`. The assembler considers such strings as identifiers. To sign a hexadecimal literal, use a `0`
prefix: `0FADH` is a hexadecimal literal, while `FADH` is an identifier.

> Theoretically, you can use as long identifiers as you want. I suggest you to make them no longer than
32 characters so that readers may read your code easily.

## Scoped Identifiers

As you will later learn, the SpectNetIDE assembler supports modules that work like namespaces in other languages (Java, C#, C++, etc.) to encapsulate labels and symbols. To access symbols within modules, you can use scoped identifiers with this syntax:

__`::`__? _identifier_ (__`.`__ _identifier_)*

The optional `::` token means that name should start in the outermost (global) scope. The module and identifier segments are separated with a dot. Examples:

```
::FirstLevelModule.Routine1
NestedModule.ClearScreen
FirstLevelModule.NestedModule.ClearScreen
```

## Characters and Strings

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
