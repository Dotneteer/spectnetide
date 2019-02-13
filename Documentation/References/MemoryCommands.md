# ZX Spectrum Memory Tool Window Commands

This article describes the commands you can use in the ZX Spectrum Memory tool window.

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

Sets the top position of the ZX Spectrum tool window to the specified address. _Note:_ The tool window displays 
the memory contents in 8-byte sections, so the given address is aligned to the closest segment.

## Banking Commands

_Note:_ This commands are available with the Spectrum 128K, Spectrum +3E, and Spectrum Next models only.

### Select ROM Page

__`R`__ *`number`*

Displays the ROM with the specified index. When showing the memory contents, the addresses between `#0000` 
and `#3fff` display the contents of this ROM. With a Spectrum 128K, you can use indexes `0` or `1`, as this model has two ROMs.
With a Spectrum +3E model, you can use indexes from `0` to `3`, supporting the four ROMs of such a model.

In this mode, the tool window displays only the contents of the selected ROM, and no other parts of the memory.

### Select Memory Bank

__`B`__ *`number`*

Displays the RAM bank with the specified index. When displaying the memory contents, the addresses between `#0000` 
and `#3fff` display the contents of this RAM bank. Indexes can be between `0` and `7`.

In this mode, the tool window displays only the contents of the selected RAM bank, and no other parts of the memory.

### Select Full Memory Mode

__`M`__

Displays the entire addressable (64K) memory, exactly as the Z80 CPU sees it. Displays the currently selected RAM
in the `#0000`..`#3FFF` address range, Bank 5 in the `#4000`..`#7FFF` range, Bank 2 between `#8000` and `#BFFF`.
Uses the currently paged bank for the `#C000`..`#FFFF` range.

_Note_: Right now, the memory view cannot handle the special banking modes available in the Spectrum +3E model (through the `#1ffd` port).
This feature will be implemented later.
