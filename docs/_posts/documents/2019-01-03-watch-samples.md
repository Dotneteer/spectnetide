---
layout: documents
categories: 
  - "Tool Commands"
title:  "Watch Samples"
alias: watch-samples
seqno: 80
selector: documents
permalink: "documents/watch-samples"
---

These samples help you to understand how powerful the watch expressions are:

```
[XPOS:W]
```

Displays the two bytes stored in the memory at the address pointed by the `XPOS` symbol.

```
[`Z ? XPOS : YPOS :W]
```

If the Z flag is set, reads the two bytes stored at the `XPOS` address; otherwise retrieves
the two bytes from `YPOS` address.

```
DE*HL
```

Displays the value of DE and HL registers multiplied. The result is a double word (32 bits).

```
[#4000:DW] & [MASK:DW] :%32
```

Reads the four bytes stored at the `#4000` address. Uses the four bytes store at the 
address pointed by `MASK` and executes a bitwise AND operation on these two 32-bit values.
Displays the result as a 32-bit binary vector.
