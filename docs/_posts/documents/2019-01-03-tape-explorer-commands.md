---
layout: documents
categories: 
  - "Tool Commands"
title:  "Tape Explorer Commands"
alias: watch-commands
seqno: 55
selector: documents
permalink: "documents/tape-explorer-commands"
---

You can issue these commands in the __Tape File Explorer__ tool window when you display a data block:

## Go To Address Command

__*`hhhh`*__

Scrolls the memory view to the address specified in __*hhhh*__ (1-4 hexadecimal digits).

## Disassembly Command

__`#`__ *`hhhh`*

Opens the disassembly pane at displays the Z80 disassembly of the data block starting at address in __*hhhh*__ (1-4 hexadecimal digits). Do not forget that the first byte is the data block header (`$ff`), so to disassembly from the beginning of the block, use `#1`.

## Close Disassembly Pane Command

__`q`__

Closes the disassembly pane. You can reopen it agin with the __Disassembly__ command. 

## Export Disassembly Command

__`x`__ *`hhhh`* *`hhhh`*

This command exports the Z80 disassembly of the tape data block from the start address (inclusive), to the end address (inclusive) to export. When you run the command, it shows up a dialog to set a few options:

![Predefined symbols]({{ site.baseurl }}/assets/images/commands/export-disassembly-dialog.png)

## Use Sinclair BASIC output

__`s`__

Sets the BASIC listing display mode to *Sinclair*. In this mode, control characters (e.g., __INK__, __PAPER__, __BRIGHT__, etc.) are shown with their hexadecimal code between `°` characters:

![Sinclair mode]({{ site.baseurl }}/assets/images/commands/sinclair-basic-mode.png)

## Use ZX BASIC output

__`z`__

Sets the BASIC listing display mode to *ZXB*. In this mode, control characters (e.g., __INK__, __PAPER__, __BRIGHT__, etc.) are shown with their ZX BASIC escape sequences; inline control characters (out of string values wrapped in doupbe quotes) are omitted:

![Sinclair mode]({{ site.baseurl }}/assets/images/commands/zx-basic-mode.png)

## Export BASIC Program

__`p`__

This command exports the BASIC listing of the tape data block, provided, it is a program data block:

![Predefined symbols]({{ site.baseurl }}/assets/images/commands/export-basic-listing-dialog.png)

By setting the **Mimic ZX BASIC** checkbox, the output will use ZX BASIC escape sequences for control characters.
