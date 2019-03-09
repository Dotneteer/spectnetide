---
layout: documents
categories: 
  - "Tool Commands"
title:  "Disassembly: Annotations"
alias: annotation-commands
seqno: 40
selector: documents
permalink: "documents/annotation-commands"
---

Managing annotations is a unique feature of __SpectNetIDE__. It is very useful when you re-engineer existing code, including a ZX Spectrum ROM. You can define labels, add end-of-instruction comments, prefix comments to disassembly instructions. Moreover, you can define identifiers for literal values and tell the disassembler to use them for particular instructions instead of the less meaningful numbers.

These commands allow you to edit the code annotation (both for the ROM and the code in the RAM):

Command | Description
--------|------------
__`L`__ *`number`* *[`identifier`]* | Adds a label with the given identifier for the specified address. If the identifier is omitted, removes the label from the address.
__`C`__ *`number`* *[`text`]* | Adds a tail comment with the given text to the instruction at the specified addres. If no text is specified, the comment is removed.
__`P`__ *`number`* *[`text`]* | Adds a prefix comment with the given text to the instruction at the specified addres. If no text is specified, the comment is removed.

With these commands you can create identifiers for literals, and replace literals with their corresponding identifiers.

Command | Description
--------|------------
__`D`__ *`literal`* *`identifier`* | Replaces the literal value within the instruction that starts at the specified address with the given *identifier*.
__`D`__ *`literal`*  | Removes the literal replacements from instruction that starts at the specified address.
__`D`__ *`literal`* __`#`__  | Replaces the literal value within the instruction that starts at the specified address with the first available identifier in the symbol table that has the value of the literal.

The IDE saves the annotations you add with these commands into the annotations file of your project. The ZX Spectrum ROMs within SpectNetIDE ship with their predefined annotations, but you can change them, too.

If your change regards to ROM addresses, the `.disann` file of the specific ROM file is changed; otherwise, modifications go to your default annotation file (`Annotations.disann` in a new project).

With the following commands, you can retrieve the already placed annotations of a particular address:

Command | Description
--------|------------
__`RL`__ *`literal`* | Retrieves the label of the specified address.
__`RC`__ *`literal`* | Retrieves the comment of the specified address.
__`RP`__ *`literal`* | Retrieves the prefix comment of the specified address.

When the specified address has an annotation, the Disassembly commands above prepare a new command line you can modify and change the label, comment, or prefix comment, respectively. However, if there is no annotation associated with the particular address, the command line remains empty.
