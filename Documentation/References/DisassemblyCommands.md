# Disassembly Tool Window Commands

This article describes the commands you can use in the Z80 Disassembly tool window.

## Syntax

Each _command_ has a name, and zero, one or more _arguments_. Command names are one or two characters,
you can use both lowercase and uppercase letters. Commands may use literals, which can be hexadecimal
numbers, decimal numbers or identifiers.

_Hexadecimal numbers_ must use the 0 to 9 digits, or letters from __`A`__ to __`F`__ or __`a`__ to __`f`__.
If a hexadecimal number would start with a letter, you should add a __`0`__ prefix so that the parser
consider it as a number and not as an identifier.

_Decimal numbers_ should start with a colon (__`:`__) and followed by digits.

_Identifiers_ should start with one a letter or an underscore (__`_`__) and may continue with digits, letters,
or underscore characters.

Here are a few examples of literals:

```
1234      (hexadecimal number!)
0FA12     (hexadecimal number)
FA12      (identifier: it starts with a letter!)
:123      (decimal number)
MySymbol  (identifier)
```

### Legend

*number*: a hexadecimal or decimal number.

*identifier*: an identifier, as specified earlier.

*literal*: a hexadecimal number, a decimal number, or an identifier. 

*text*: a text that contains arbitrary characters, including spaces, punctuations, and so on.

*[optional]*: The argument is optional

### Identifier Resolution

When executing a command, identifiers are translated into addresses. During the resolution process, the command parsing engine resolves 
identifiers in these steps:
1. Checks the output of the last compilation. If the identifier is found, its value is taken from the Assembler's symbol table.
2. Checks the labels and symbols in the user annotations (by default stored in the Annotations.disann file). If the identifier is found, its value is taken from the symbol table of the annotation.
3. Checks the labels and symbols in the current ROM's annotations. If the identifier is found, its value is taken from the symbol table of the ROM annotation.
4. The identifier cannot be resolved, the command parsing engine signs an error.

## Navigation Command

__`G`__ *`literal`*

Sets the top position of the Disassembly tool window to the specified address.

## Memory Section Commands

These commands instructs the disassembly tool how to map a certain memory section into disassembly
output. The *literal1* and *literal2* values specify the memory range. Be aware, *literal1* is inclusive, 
but *literal2* is exclusive.

Command | Description
--------|------------
__`MD`__ *`literal1`* *`literal2`* | Creates a disassembly section &mdash; this section of memory is represented as a flow id Z80 instructions.
__`MB`__ *`literal1`* *`literal2`* | Creates a byte section. The contents of this memory area is displayed as bytes with the __`.defb`__ pragma.
__`MW`__ *`literal1`* *`literal2`* | Creates a word section. The contents of this memory area is displayed as bytes with the __`.defw`__ pragma.
__`MS`__ *`literal1`* *`literal2`* | Skip section &mdash; the view displays a simple __`.skip`__ pragma for the section.
__`MC`__ *`literal1`* *`literal2`* | Calculator section. The disassembler takes these bytes into account as the byte code of the ZX Spectrum's RST #28 calculator.

## Annotation Commands

These commands allow you to edit the code annotation (both for the ROM and the code in the RAM).
The information you enter move into the annotation file.

Command | Description
--------|------------
__`L`__ *`number`* *[`identifier`]* | Adds a label with the given identifier for the specified address. If the identifier is omitted, removes the label from the address.
__`C`__ *`number`* *[`text`]* | Adds a tail comment with the given text to the instruction at the specified addres. If no text is specified, the comment is removed.
__`P`__ *`number`* *[`text`]* | Adds a prefix comment with the given text to the instruction at the specified addres. If no text is specified, the comment is removed.

## Literal Manipulation Commands

With these commands you can create identifiers for literals, and replace literals with their
corresponding identifiers.

Command | Description
--------|------------
__`D`__ *`literal`* *`identifier`* | Replaces the literal value within the instruction that starts at the specified address with the given *identifier*.
__`D`__ *`literal`*  | Removes the literal replacements from instruction that starts at the specified address.
__`D`__ *`literal`* __`#`__  | Replaces the literal value within the instruction that starts at the specified address with the first available identifier in the symbol table that has the value of the literal.

## Breakpoint Commands

With these commands, you can set up breakpoint according to their addresses. These breakpoints are not persisted,
they are removed when you close the solution.

Command | Description
--------|------------
__`SB`__ *`literal`* [ __`H`__ *`hit-condition`*] [ __`C`__ *`filter-condition`*] | Sets a breakpoint at the specified address with the optional *hit condition* and/or *filter condition*.
__`TB`__ *`literal`* | Toggles a breakpoint at the specified address.
__`RB`__ *`literal`* | Removes the breakpoint from the specified address.
__`UB`__ *`literal`* | Retrieves the breakpoint at the specified address so that you can update it.
__`EB`__ | Erases all breakpoints.

### Hit Condition

The __`SB`__ command allows you to specify a hit condition to define when the program should stop at the specified breakpoint.
The debugger counts how many times the program code reaches the breakpoint and stops when the hit condition meets:

__`H`__ *`condition-type`* *`condition-value`*

The *condition-value* is an integer number. You can apply one of these *condition-type* tokens:

Type | Description
-----|------------
__`<`__ | Execution stops when the current hit counter is less than *condition-value*
__`<=`__ | Execution stops when the current hit counter is less than or equal to *condition-value*
__`=`__ | Execution stops when the current hit counter is equal to *condition-value*
__`>`__ | Execution stops when the current hit counter is greater than *condition-value*
__`>=`__ | Execution stops when the current hit counter is greater than or equal to *condition-value*
__`*`__ | Execution stops when the current hit counter is a multiple of *condition-value*

The following example defines a hit condition that stops at the `$8010` address when the 
hit counter s greater than 10:

```
SB 8010 H>10
```

These command sets a breakpoint at `$6100` to stop at every fifth hit:

```
SB 6000 H*5*
```

### Filter Condition

You can apply not only hit conditions, but also *filter conditions* to a breakpoint. When
the execution reaches the breakpoint, the debugger evaluates the expression. If it is a `true`
value (non-zero integer), the execution flow pauses; otherwise it goes on without stoping.

You can use the same syntax for defining a filter condition as for watch items in the 
[Watch Memory tool window](./WatchCommands.md).

> When the watch expression results an evaluation error, the debug engine pauses as if there
> were no filter condition.

Let's see a few examples. The following command defines a breakpoint at `$6800` that stops when
the contents of the __HL__ register is `$4020`:

```
SB 6800 C HL==#4020
```

This condition breakpoint tests if the value of the memory address `$4100` equals to `$FF`:

```
SB 7A00 C [#4100]==#FF
```

You can use the condition to check if there's a `$20` value at the __IX+12__ address:

```
SB 6500 C [IX+12]==#20
```

The following condition results in a "Divide by zero" error, so it stops every time the execution
flow reaches the `$6200` address:

```
SB 6200 C HL/0==2
```

### Combining Hit Conditions and Filter Conditions

You can apply both hit and filter conditions for the same breakpoint. You have to define the hit
condition first, filter condition next:

```
SB 6400 H>5 C B<=10
```

If you exchange the condition order, the command prompt will indicate syntax error:

```
SB 6400 C B<=10 H>5
```

When you apply both conditions, they must be both satisfied to pause at that particular breakpoint.

## Banking Commands

_Note:_ These commands are available with the Spectrum 128K, Spectrum +3E, and Spectrum Next models only.

### Select ROM Page

__`R`__ *`number`*

Displays the disassembly for the ROM with the specified index. When showing the disassembly, the addresses between `#0000` 
and `#3fff` display the contents of this ROM. With a Spectrum 128K, you can use indexes `0` or `1`, as this model has two ROMs.
With a Spectrum +3E model, you can use indexes from `0` to `3`, supporting the four ROMs of such a model.

In this mode, the tool window displays only the disassembly of the selected ROM.

### Select Memory Bank

__`B`__ *`number`*

Displays the disassembly of RAM bank with the specified index. The disassembly uses the addresses between `#0000` 
and `#3fff`. Indexes can be between `0` and `7`.

In this mode, the tool window displays only the contents of the selected RAM bank, and no other parts of the memory.

### Select Full Memory Mode

__`M`__

Displays the disassembly for the entire addressable (64K) memory, exactly as the Z80 CPU sees it. Displays the currently selected RAM
in the `#0000`..`#3FFF` address range, Bank 5 in the `#4000`..`#7FFF` range, Bank 2 between `#8000` and `#BFFF`.
Uses the currently paged bank for the `#C000`..`#FFFF` range.

_Note_: Right now, the disassembly view cannot handle the special banking modes available in the Spectrum +3E model (through the `#1ffd` port).
This feature will be implemented later.

## Disassembly Type Command

__`T`__ (`48`|`128`|`P3`|`NEXT`)

You can choose the disassembly type for the current view. ZX Spectrum models have their disassembly peculiarities. For example, 
the Spectrum 48K model uses the `RST #28` instructions to implement a floating point calculator with bytecode that follows the 
call. Spectrum 128K uses the `RST #28` instruction to call into ROM 0 with the 2-byte address that follows the RST call.

With this command you can select which model type to use for the disassembly displayed in the tool window.

## Re-Disassembly Command

__`RD`__

You can force a re-disassembly of the current view. If the contents of the RAM changes, you might need this command to 
refresh the view so that you can see the latests changes.

## Jump Command

__`J`__ *`literal`*

This command works only when the Spectrum VM is paused. It sets the Program Counter (PC) to the
specified address. When the Spectum VM continues running &mdash; after the Start command &mdash;
it will carry on from the specified address.
