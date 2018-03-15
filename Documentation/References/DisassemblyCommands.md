# Disassembly Tool Window Commands

__Legend:__

*hexnum*: Up to 4 hexadecimal digits forming a hexadecimal address.

*identifier*: An identifier that start with an underscore (`_`), or with a letter, 
and goes on with an underscore, a letter, or a digit. 

*text*: A text that contains arbitrary characters, including spaces, punctuations, and so on.

*[optional]*: The argument is optional

The commands and hexadecimal numbers can be written either in lowercase or uppercase.

## Navigation Command

__`G`__ *`hexnum`*

Sets the top position of the Disassembly tool window to the specified address.

## Memory Section Commands

These commands instructs the disassembly tool how to map a certain memory section into disassembly
output. The *hexnum1* and *hexnum2* values specify the memory range, both values are inclusive.

Command | Description
--------|------------
__`MD`__ *`hexnum1`* *`hexnum2`* | Creates a disassembly section &mdash; this section of memory is represented as a flow id Z80 instructions.
__`MB`__ *`hexnum1`* *`hexnum2`* | Creates a byte section. The contents of this memory area is displayed as bytes with the __`.defb`__ pragma.
__`MW`__ *`hexnum1`* *`hexnum2`* | Creates a word section. The contents of this memory area is displayed as bytes with the __`.defw`__ pragma.
__`MS`__ *`hexnum1`* *`hexnum2`* | Skip section &mdash; the view displays a simple __`.skip`__ pragma for the section.
__`MC`__ *`hexnum1`* *`hexnum2`* | Calculator section. The disassembler takes these bytes into account as the byte code of the ZX Spectrum's RST #28 calculator.

## Annotation Commands

These commands allow you to edit the code annotation (both for the ROM and the code in the RAM).
The information you enter move into the annotation file.

Command | Description
--------|------------
__`L`__ *`hexnum`* *[`identifier`]* | Adds a label with the given identifier for the specified address. If the identifier is omitted, removes the label from the address.
__`C`__ *`hexnum`* *[`text`]* | Adds a tail comment with the given text to the instruction at the specified addres. If no text is specified, the comment is removed.
__`P`__ *`hexnum`* *[`text`]* | Adds a prefix comment with the given text to the instruction at the specified addres. If no text is specified, the comment is removed.

## Literal Manipulation Commands

With these commands you can create identifiers for literals, and replace literals with their
corresponding identifiers.

Command | Description
--------|------------
__`D`__ *`hexnum`* *`identifier`* | Replaces the literal value within the instruction that starts at the *hexnum* address with the specified *identifier*.
__`D`__ *`hexnum`*  | Removes the literal replacements from instruction that starts at the *hexnum* address.
__`D #`__  | Replaces the literal value within the instruction that starts at the *hexnum* address with the first available identifier in the symbol table that has the value of the literal.

## Breakpoint Commands

With these commands, you can set up breakpoint according to their addresses. These breakpoints are not persisted,
they are removed when you close the solution.

Command | Description
--------|------------
__`SB`__ *`hexnum`* | Sets a breakpoint at the address specified by *hexnum*.
__`TB`__ *`hexnum`* | Toggles a breakpoint at the address specified by *hexnum*.
__`RB`__ *`hexnum`* | Removes the breakpoint from the address specified by *hexnum*.
__`EB`__ | Erases all breakpoints.

## Banking Commands

_Note:_ This commands are available with the Spectrum 128K, Spectrum +3E, and Spectrum Next models only.

### Select ROM Page

__`R`__ *`romindex`*

Displays the disassembly for the ROM with the specified index. When showing the disassembly, the addresses between `#0000` 
and `#3fff` display the contents of this ROM. With a Spectrum 128K, you can use indexes `0` or `1`, as this model has two ROMs.
With a Spectrum +3E model, you can use indexes from `0` to `3`, supporting the four ROMs of such a model.

In this mode, the tool window displays only the disassembly of the selected ROM.

### Select Memory Bank

__`B`__ *`bankindex`*

Displays the disassembly of RAM bank with the specified index. The disassembly uses the addresses between `#0000` 
and `#3fff`. Indexes can be between `0` and `7`.

In this mode, the tool window displays only the contents of the selected RAM bank, and no other parts of the memory.

### Select Full Mmemory Mode

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

__`R`__

You can force a re-disassembly of the current view. If the contents of the RAM changes, you might need this command to 
refresh the view so that you can see the latests changes.

## Jump Command

__`J`__ *`hexnum`*

This command works only when the Spectrum VM is paused. It sets the Program Counter (PC) to the
specified *`hexnum` address. When the Spectum VM continues running &mdash; after the Start command &mdash;
it will carry on from the specified address.