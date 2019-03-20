---
layout: documents
categories: 
  - "Tool Commands"
title:  "Common Commands"
alias: memory-commands
seqno: 20
selector: documents
permalink: "documents/common-commands"
---

This section describes the navigation and view commands you can use in the __ZX Spectrum Memory__, and __Z80 Disassembly__ tool windows.

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

> __Note__: Right now, these views cannot handle the special banking modes available in the Spectrum +3E model (through the `#1ffd` port).
This feature will be implemented later.

## Export Command

You can export the contents of the memory or the disassembly with the export command:

__`X`__ *`literal`* *`literal`*

The first literal defines the start (inclusive), the second the end of the address range (inclusive) to export. When you run the command, it shows up a dialog to set a few export parameters.

ZX Spectrum Memory:

![Predefined symbols]({{ site.baseurl }}/assets/images/commands/export-memory-dialog.png)

Z80 Disassembly:

![Predefined symbols]({{ site.baseurl }}/assets/images/commands/export-disassembly-dialog.png)
