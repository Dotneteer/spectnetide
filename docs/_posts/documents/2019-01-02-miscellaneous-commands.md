---
layout: documents
categories: 
  - "Tool Commands"
title:  "Disassembly: Miscellaneous"
alias: miscellaneous-commands
seqno: 50
selector: documents
permalink: "documents/miscellaneous-commands"
---

## Disassembly Type Command

__`T`__ (`48`\|`128`\|`P3`\|`NEXT`)

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