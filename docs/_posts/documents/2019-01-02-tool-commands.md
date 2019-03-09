---
layout: documents
categories: 
  - "Tool Commands"
title:  "Overview"
alias: tool-overview
seqno: 10
selector: documents
permalink: "documents/tool-overview"
---

__SpectNetIDE__ has several tool windows that provide a prompt to enter commands. Such windows are __ZX Spectrum Memory__, __Z80 Disassembly__, and __Watch Memory__.
Though each of them has different command sets that slightly overlap, they share a common syntax.
Here, you can find an overview of that syntax.

## Syntax Basics

Each _command_ has a name, and zero, one or more _arguments_. Command names are one or two characters, you can use both lowercase and uppercase letters. Commands may use literals, which can be hexadecimal numbers, decimal numbers or identifiers.

_Hexadecimal numbers_ must use the 0 to 9 digits, or letters from `A` to `F` or `a` to `f`. If a hexadecimal number would start with a letter, you should add a `0` prefix so that the parser consider it as a number and not as an identifier.

Decimal numbers should start with a colon (`:`) and followed by digits.

Identifiers should start with one a letter or an underscore (`_`) and may continue with digits, letters, or underscore characters.

Here are a few examples of literals:

```
1234      (hexadecimal number!)
0FA12     (hexadecimal number)
FA12      (identifier: it starts with a letter!)
:123      (decimal number)
MySymbol  (identifier)
```

In the sections providing detailed command reference, you will meet these elements:

* _number_: hexadecimal or decimal number.
* _identifier_: an identifier, as specified earlier.
* _literal_: a hexadecimal number, a decimal number, or an identifier.
* _text_: a text that contains arbitrary characters, including spaces, punctuations, and so on.
* _[optional]_: the argument is optional.

## Identifier Resolution

When executing a command, identifiers are translated into addresses. During the resolution process, the command parsing engine resolves identifiers in these steps:

1. Checks the output of the last compilation. If the identifier is found, its value is taken from the Assembler's symbol table.
2. Checks the labels and symbols in the user annotations (by default stored in the Annotations.disann file). If the identifier is found, its value is taken from the symbol table of the annotation.
3. Checks the labels and symbols in the current ROM's annotations. If the identifier is found, its value is taken from the symbol table of the ROM annotation.
4. The identifier cannot be resolved, the command parsing engine signs an error.


