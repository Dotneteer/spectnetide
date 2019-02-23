---
layout: documents
categories: 
  - "Tool Commands"
title:  "Disassembly: Memory Sections"
id: memory-section-commands
seqno: 30
selector: documents
permalink: "documents/memory-section-commands"
---

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

The IDE saves the memory map you set up with these commands into the annotations file of your project. The ZX Spectrum ROMs within SpectNetIDE ship with their memory map set, but you can change them, too.

If your change regards to ROM addresses, the `.disann` file of the specific ROM file is changed; otherwise, memory map modifications go to your default annotation file (`Annotations.disann` in a new project).

> __Hint__: When you define memory maps, avoid those that overlap the ROM/RAM boundary.



