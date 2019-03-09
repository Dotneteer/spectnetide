---
layout: documents
categories: 
  - "Unit Testing"
title:  "Syntax Basics"
alias: unit-testing-basics
seqno: 10
selector: documents
permalink: "documents/unit-testing-basics"
---

__SpectNetIde__ provides a somple programming language to describe Z80-related unit tests. You can add the 
`.z80test` files to your project and run the test in the __Unit Test Explorer__ tool window. This document
treats the syntax and semantics of the language.

## Syntax Basics

The test language uses a special way of case-sensitivity. You can write the reserved
keywords on lowercase. When you refer to Z80 CPU registers or flags, you can use either with lowercase or
uppercase letters, but you cannot mix these cases. You can mix cases for identifiers, though they are
searched for a case-insensitive manner.

## Test Structure

A ZX Spectrum project may have zero, one, or more test files (files with `.z80test` extension). Before 
running them, the engine collects all `.z80test` files from the current project, and compiles them all.

A single test file may contain one or more _test set_. A test set is a collection of cohesive tests that 
all use the very same source code, as the basis of testing:

```
testset Introduction
{
    source "../Z80CodeFiles/CodeSamples.z80asm";

    // --- Other testset attributes    

    test AddAAndBWorksAsExpected
    {
        // --- Other test attributes

        act call AddAAndB;

        assert
        {
            // --- Here we describe the test assertions
        }
    }
}
```

A _test_ can have a default _act_, such as in the sample above, or may have parameterized test cases:

```
// --- Wrapping test set omitted
test AddAAndBCallWorksAsExpected2
{
    params parA, parB, Z;
    case 1, 2, 0;
    case 2, 3, 0;
    case -6, 6, 1;

    arrange
    {
        a: parA;
        b: parB;
    }

    act call AddAAndB;

    assert
    {
        a == parA + parB;
        b == parB;
        .z == Z;
    }
}
```

This test has three cases, as declared by the lines starting with the `case` keyword.
When running them, the engine substitutes the `parA`, `parB`, and `Z` values with the
values after `case` (fore each `case`).

To summarize:

Concept | Description |
--------|-------------|
_test file_ | A single container for _test sets_. Besides keeping test sets together in a single file, there's no additional semantics.
_test set_ | A cohesive set of tests. A test set has a single source code file &mdash; this contains the code to test &mdash; shared between the test within the set.
_test_ | A single test that runs a piece of the source code to test. It may nest test cases.
_test case_ | Parameterized test. It runs the same code (although you can run different code) with the case-related parameters.

## Syntax Elements

The test language contains several constituting elements that you can use in many 
places within the code, such as comments, expressions, identifiers, and so on. Here 
you can learn about them.

### Comments

Comments can be single line or multi-line comments with the same syntax construct as you may 
use them in many curly-brace-languages, such as C++, Java, C#, etc.:

```
// --- This is a single line comment you can add to the end of the code lines

/* This is a multi-line comment the spans accross multiple lines, 
   including empty ones

*/
```

### Literals

The language syntax provides these types of literals:
* __Decimal numbers.__ You can use adjacent digits (0..9) to declare a decimal number. Examples:
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

* __Characters__. You can put a character between double quotes (for example: ```"Q"```). 
* __Strings__. You can put a series of character between double quotes (for example: ```"Sinclair"```).

Here are a few samples:

```
"This is a string. The next sample is a single character"
"c"
```

### Character and String Escapes

ZX Spectrum has a character set with special control characters such as AT, INK, PAPER, and so on.

__SpectNetIde__ allows you to utilize special escape sequences to define ZX Spectrum-specific characters:

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