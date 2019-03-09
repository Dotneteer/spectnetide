---
layout: documents
categories: 
  - "Z80 Assembler"
title:  "Labels and Symbols"
alias: labels
seqno: 32
selector: documents
permalink: "documents/labels-and-symbols"
---

In Z80 assembly, you can define labels and symbols. Both constructs are syntactically the same, but there is some difference in their semantics. While we define labels to mark addresses (code points) in the program so that we can jump to those addresses, read or write their contents, symbols are not as specific, they just store values we intend to use.
From now on, I will mention "label" for both constructs and will do otherwise only when the context requires.
When you write a __SpectNetIDE__ assembly instruction, you can start the line with a label:

```
MyStart: ld hl,0
```

Here, in this sample, `MyStart` is a label. The assembler allows you to omit the colon after the label name, so this line is valid:

```
MyStart ld hl,0
```

Some developers like to put a label in a separate line from the instruction it belongs. You can use the same hanging label style within __SpectNetIDE__. In this case, the label should go _before_ its instruction. Take a look at this code snippet:

```
MyStart:
  ld hl,0
MyNext
  ; Use B as a counter
  ld b,32
```

This code is entirely correct. Note, the `ld b,32` instruction belongs to the `MyNext` label. As you see from the sample, the colon character is optional for hanging labels, too. You can have multiple line breaks between a label and its instruction, and the space can include comments.

## Label and Symbol Declarations

As you will learn later, you can define symbols with the `.EQU` or `.VAR` pragmas. While `.EQU` allows you to assign a constant value to a symbol &mdash; and so it cannot change its value after the declaration, `.VAR` let's you re-assign the initial value.

__SpectNetIDE__ supports the idea of lexical scopes. When you create the program, it starts with a global (outermost) lexical scope. Particular language elements, such a _statements_ create their own nested lexical scope. Labels and symbols are always created within the current lexical scope. Nonetheless, when resolving them, the assembler starts with the innermost scope and goes through all outers scopes until it manages to find the label declaration.
This mechanism means that you can declare labels within a nested scope so that those hide labels and symbols in outer scopes.
You can learn more about this topic in the __Statements__ section.

__SpectNetIDE__ also supports modules, which allow you to use namespace-like constructs (see the Modules section for details).

## Temporary Labels

The assembler considers labels that start with a backtick (`) character as temporary labels. Their scope is the area between the last persistent label preceding the temporary and the first persistent label following the temporary one.

This code snippet demonstrates this concept:

```
SetPixels:        ; Persistent label
  ld hl, #4000
  ld a,#AA
  ld b,#20
`loop:            ; Temporary label (scope #1)
  ld (hl),a
  inc hl
  djnz `loop
SetAttr:          ; Persistent label, scope #1 disposed here
  ld hl,#5800
  ld a,#32
  ld b,#20
`loop:            ; Temporary label (scope #2)
  ld (hl),a
  inc hl
  djnz `loop
  ret

; scope #2 still lives here
; ...
Another: ; Persistent label, scope #2 disposed here
  ld a,b
```

As you see, the two occurrences of __\`loop__  belong to two separate temporary scopes. The first scope is between from `SetPixels` to `SetAttr`, the second one between `SetAttr` and `Another`.


