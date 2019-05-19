---
layout: documents
categories: 
  - "Z80 Assembler"
title:  "How the assembler works"
alias: how-assembler-works
seqno: 20
selector: documents
permalink: "documents/how-assembler-works"
---

The assembler compiles the code in these phases:

1. It takes the source code and runs a preprocessor that parses the entire code, and applies the
*directives* in the code. You can easily recognize directives, as they start with `#`, such as
`#ifdef`, `#endif`, `#define`, `#include` and others. During the preprocessing phase,
the assembler detects the syntax errors, loads and processes the included files. The result is 
a *digested syntax tree* that does not contain directives anymore, only *instructions*, *pragmas*,
and *statements*.

1. The assembler collects macro definitions and stores their syntax tree so that later it can use them when macros are invoked with their actual parameters.
  
2. The assembler goes through the digested syntax tree and emits code. During this operation, it needs
to evaluate expressions, resolve symbols and identifiers to their actual values. Because the assembler 
progresses from the first line to the last, it may happen that it cannot get the value of an identifier
which is defined somewhere later in the code. When the assembler detects such a situation, it makes 
a note of it &mdash; it creates a *fixup* entry.

1. The assembler goes through all fixup entries and resolves symbols that were not defined in
the previous phase. Of course, it might find unknows symbols. If this happens, the assembler reports
an error.

> Several pragmas and statements intend to evaluate an expression in phase 3. If they find an
> unresolved symbol during that phase, they do not create a fixup entry but immediately report an error.
