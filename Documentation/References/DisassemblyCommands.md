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


