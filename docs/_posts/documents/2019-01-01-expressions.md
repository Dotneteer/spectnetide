---
layout: documents
categories: 
  - "Z80 Assembler"
title:  "Expressions"
alias: expressions
seqno: 40
selector: documents
permalink: "documents/expressions"
---

The __SpectNetIde__ assembler has a rich syntax for evaluating expressions. You can use the very
same syntax with the `#if` directives, the Z80 instructions, and the compiler statements.

You can use operands and operators just like in most programming languages. Nevertheless, 
the __SpectNetIde__ implementation has its particular way of evaluating expressions:

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

* Besides the parentheses &mdash; `(` and `)` &mdash; you can use square brackets 
&mdash; `[` and `]` &mdash; to group operations and change operator precedence.

```
; This is valid
ld hl,(Offset+#20)*2+BaseAddr

; Just like this
ld hl,[Offset+#20]*2+BaseAddr
```

## Instant and Late Expression Evaluation

Depending on the context in which an expression is used, the compiler evaluates it instantly or
decides to postpone the evaluation. For example, when you use the `.org` pragma, the compiler applies
immediate evaluation. Let's assume, this is your code:

```
Start: .org #8000 + Later
; code body (omitted)
Later: .db #ff
```

The value of `Later` depends on the address in `.org`, and the `.org` address depends on `Later`, 
so this declaration could not be resolved properly, it's like a deadlock. To avoid such situations, 
the `.org` pragma would raise an error, as the moment of its evaluation the `Later` symbol's value is 
unknown.

For most Z80 instructions the compiler uses late evaluation:

```
Start: .org #6000
    ld hl,(MyVar)
    ; code body omitted
    ret
MyVar: .defs 2
```

When the compiler reaches the `ld hl,(MyVar)` instruction, it does not know the value of `MyVar`. Nonetheless,
it does not stop with an error, but generates the machine code for `ld hl,(0)`, namely #21, #00, and #00; 
takes a note (it is called a *fixup*) when `MyVal` gets a value, the two #00 bytes generated at address #6001 
should be updated accordingly.

## Operands
You can use the following operands in epressions:
* Boolean, Decimal and hexadecimal literals
* Character literals
* Identifiers
* The current assembly address

> String literals cannot be used as operands.

## Operators

You can use about a dozen operators, including unary, binary and ternary ones. In this section
you will learn about them. I will introduce them in descending order of their precendence.

### Conditional Operator

The assembler supports using only one ternary operator, the conditional operator:

_conditional-expression_ __`?`__ _true-value_ __`:`__ _false-value_

This operation results in -1:

`2 > 3 ? 2 : -1`

When the _conditional-expression_ evaluates to true, the operation results 
in _true-value_; otherwise in _false-value_.

> Conditional expressions are evaluated from right to left, in contrast to binary operators,
> which use left-to-right evaluation.

### Binary Bitwise Operators

Operator token | Precedence | Description
---------------|------------|------------
`|` | 1 | Bitwise OR
`^` | 2 | Bitwise XOR
`&` | 3 | Bitwise AND &mdash; string concatenation with new line

> The `&` operator can be applied on two strings. If you do so, the compiler concatenates the two 
> strings and puts a `\r\n` (next line) character pair between them. 
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
`+` | 7 | Addition &mdash; string concatenation
`-` | 7 | Subtraction
`*` | 8 | Multiplication
`/` | 8 | Division
`%` | 8 | Modulo calculation

# Min-Max operators

Operator token | Precedence | Description
---------------|------------|------------
`<?` | 9 | Minimum of the left and right operand
`>?` | 9 | Maximum of the left and right operand

### Unary operators

Operator token | Precedence | Description
---------------|------------|------------
`+` | 10 | Unary plus
`-` | 10 | Unary minus
`~` | 10 | Unary bitwise NOT
`!` | 10 | Unary logical NOT

> Do not forget, you can change the defult precendence with `(` and `)`, or with `[` and `]`.

## Functions

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
`attr(integer, integer, boolean, boolean)` | `integer` | Retrieves the color attribute byte value defined by `ink` (first argument, 0 to 7), `paper` (second argument, 0 to 7), `bright` (third argument, 0 - non-zero), and `flash` (fourth argument, 0 - non-zero). The `bright` and `flash` values are optional.
`attraddr(integer, integer)` | `integer` | Returns the memory address of the byte specified screen attribute in the given line (first argument, from top to bottom, 0-192) and column (second argumment, from left to right, 0-255).`ceiling(float)`
`bright(boolean)` | `integer` | Retrieves the bright flag defined by the attribute (0 - non-zero). Can be ORed to create color attribute value.
`ceiling(float`) | `float` | The smallest integral value that is greater than or equal to the specified number.
`cos(float)` | `float` | The cosine of the specified angle.
`cosh(float)` | `float` | The hyperbolic cosine of the specified angle.
`exp(float)` | `float` | __e__ raised to the specified power.
`fill(string, integer)` | `string` | Creates a new string by concatenating the specified one with the given times.
`flash(boolean)` | `integer` | Retrieves the flash flag defined by the argument (0 - non-zero). Can be ORed to create color attribute value.
`floor(float)` | `float` | The largest integer less than or equal to the specified number.
`frac(float)` | `float` | The fractional part of the specified number.
`high(integer)` | `integer` | The leftmost 8 bits (MSB) of a 16-bit integer number.
`ink(integer)` | `integer` | Retrieves the three ink bits defined by the color argument (0 to 7). Can be ORed to create color attribute value.
`int(float)` | `integer` | The integer part of the specified number.
`lcase(string)` | `string` | The lowercase version of the input string.
`left(string, integer)` | `string` | Takes the leftmost characters of the string with the length specified.
`len(string)` | `integer` | The length of the specified string.
`length(string)` | `integer` | The length of the specified string.
`log(float)` | `float` | The natural (base __e__) logarithm of a specified number.
`log(float, float)` | `float` | The logarithm of a specified number in a specified base.
`log10(float)` | `float` | The base 10 logarithm of a specified number.
`low(integer)` | `integer` | The rightmost 8 bits (LSB) of an integer number.
`lowercase(string)` | `string` | The lowercase version of the input string.
`max(integer, integer)` | `integer` |  The larger of two *integer* numbers.
`max(float, float)` | `float` | The larger of two *float* numbers.
`min(integer, integer)` | `integer` |  The smaller of two *integer* numbers.
`min(float, float)` | `float` | The smaller of two *float* numbers.
`nat()` | `float` | Represents the natural logarithmic base, specified by the constant, __e__.
`paper(integer)` | `integer` | retrieves the three paper bits defined by the argument (0 to 7). Can be ORed to create color attribute value.
`pi()` | `float` | Represents the ratio of the circumference of a circle to its diameter, specified by the constant, __&pi;__.
`pow(float, float)` | `float` | The specified number raised to the specified power.
`right(string, integer)` | `string` | Takes the rightmost characters of the string with the length specified.
`round(float)` | `float` | Rounds a *float* value to the nearest integral value.
`round(float, int)` | `float` | Rounds a *float* value to a specified number of fractional digits.
`rnd()` | `integer` | Returns a random 32-bit number.
`rnd(integer, integer)` | `integer` | Returns a random 32-bit integer between the first and second number.
`scraddr(integer, integer)` | `integer` | Retrieves the memory address of the screen pixel byte in the specified line (first argument, from top to bottom, 0-192) and in the specified column (second argument, from left to right, 0-255).
`sign(integer)` | `integer` | Returns an integer that indicates the sign of an *integer* number.
`sign(float)` | `integer` | Returns an integer that indicates the sign of a *float* number.
`sin(float)` | `float` | The sine of the specified angle.
`sinh(float)` | `float` | The hyperbolic sine of the specified angle.
`sqrt(float)` | `float` | The square root of a specified number.
`str(bool)` | `string` | Convert the input value to a string.
`str(integer)` | `string` | Convert the input value to a string.
`str(float)` | `string` | Convert the input value to a string.
`substr(string, integer, integer)` | `string` | Takes a substring of the specified string from the given position (zero-based) and length.
`tan(float)` | `float` | The tangent of the specified angle.
`tanh(float)` | `float` | The hyperbolic tangent of the specified angle.
`truncate(float)` | `integer` | Calculates the integral part of a specified number.
`ucase(string)` | `string` | The uppercase version of the input string.
`uppercase(string)` | `string` | The uppercase version of the input string.
`word(integer)` | `integer` | The rightmost 16 bits of an integer number.

Functions have the same precedence as the unary operators (such as the unary `+` and `-`).

## Parse Time Functions

The compiler provides a construct, *parse time functions*. These functions can receive a Z80 assembly language token
and transform them into other language constructs. As the name suggests, these function run in the parsing phase, before
the compiler emits code.

### The `lreg()` and `hreg()` Parse Time Functions

These functions accept a 16-bit register pair token (__BC__, __DE__, __HL__, __IX__, or __IY__), and retrieve the lower
or higher 8-bit register half of their input. Here is a sample code snippet:

```
ld a,lreg(bc)
ld c,hreg(hl)
ld a,lreg(ix)
ld l,hreg(de)
```

The compiler sees as if you wrote this:

```
ld a,c
ld c,h
ld a,ixl
ld l,d
```

### The `textof()` Parse Time Function

You can use `textof()`, which accepts these kinds of tokens: mnemonic, register, register indirection, C port, 
or condition. This function translates these tokens into uppercase string constants that represent them.
Here is a sample:

```
.dm textof(ldir)
.dm textof(bc)
.dm textof((de))
.dm textof((c))
.dm textof(nz)
```

The compiler sees as if you wrote this code:

```
.dm "LDIR"
.dm "BC"
.dm "(DE)"
.dm "(C)"
.dm "NZ"
```
