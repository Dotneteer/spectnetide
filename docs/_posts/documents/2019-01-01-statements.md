---
layout: documents
categories: 
  - "Z80 Assembler"
title:  "Statements"
alias: statements
seqno: 70
selector: documents
permalink: "documents/statements"
---

Statements are __SpectNetIDE__ specific control flow constructs &mdash; thanks again for the inspiration by
[Simon Brattel](http://www.desdes.com/index.htm) &mdash; that instruct the compiler about loop-like and
conditional compilation.

> While *directives* help you to organize your code and include code files optionally according to the
> compilation context, *statements* provide you more useful tools to shorten the way you can declare Z80
> assembly code.

Each statement can be written with a leading dot, or without it, and the compiler accepts both lowercase 
and uppercase versions. For example all of these version are valid: `.if`, `if`, `.IF`, and `IF`.

## The LOOP Block

With LOOP block, you can organize a cycle to emit code. Here is a sample that tells the gist:

```
.loop 6
  add hl,hl 
.endl
```

This is a shorter way to multiply __HL__ with 64. It is equivalent with the following code:

```
  add hl,hl
  add hl,hl
  add hl,hl
  add hl,hl
  add hl,hl
  add hl,hl
```

The `.loop` statement accepts an expression. The compiler repeats the instructions within the 
loop's body according to the value of the expression. The `.endl` statement marks the end of the loop.

> You can use many flavors for the `.endl` block closing statement. `.endl`, `endl`, `.lend`, `lend`
> are all accepted &mdash; with fully uppercase letters, too.

Look at this code:

```
counter .equ 2
; do something (code omitted)
.loop counter + 1
  .db #80, #00
.endl
```

This is as if you wrote this:

```
  .db #80, #00
  .db #80, #00
  .db #80, #00
```

## The LOOP Scope

The `.loop` statement declares a scope for all labels, symbols, and variables declared in the loop's
body. Every iteration has its separate local scope. When the assembler resolves symbols, it starts 
from the scope of the loop, and tries to resolve the value of a symbol. If it fails, steps out to 
the outer scope, and goes on with the resolution.

Check this code:

```
value .equ 2
; do something (code omitted)
.loop 2
    value .equ 5
    ld a,value
.endl
```

The compiler takes it into account as if you wrote this:

```
    ld a,5
    ld a,5
```

The `value` symbol declared within the loop, overrides `value` in the outer scope, and
thus 5 is used instead of 5.

Nonetheless, you when you utilize a different construct, it seems a bit strange at first:

```
value .equ 2
; do something (code omitted)
.loop 2
    ld a,value
    value .equ 5
    ld b,value
.endl
```

The strangeness is that the compiler creates this:

```
    ld a,2
    ld b,5
    ld a,2
    ld b,5
```

When the assembler resolves `value` in the `ld a,value` instruction, if finds `value` 
in the outer scope only, as it is not declared yet within the loop's scope. In the `ld b,value`
instruction `value` gets resolved from the inner scope, so it takes 5.

## Variables and Scopes

Unlike symbols that work as constant values, variables (declared with the `.var` pragma, or its syntactical 
equivalents, the `=` or `:=` tokens) can change their values.

Take a look at this code:

```
counter = 4
.loop 3
    innercounter = 4
    ld a,counter + innercounter
    counter = counter + 1
.endl
```

Here, the `counter` variable is defined in the global scope (out of the loop's scope), while `innercounter` in
the local scope of the loop. When evaluating the `counter = counter + 1` statement, the compiler finds `counter` in
the outer scope, so it uses that variable to increment its value. This code emits machine code for this source:

```
ld a,#08
ld a,#09
ld a,#0A
```

Now, add a single line to the loop's code:

```
counter = 4
.loop 3
    innercounter = 4
    ld a,counter + innercounter
    counter = counter + 1
.endl
ld b,innercounter
```

The compiler will not compile this code, as it cannot find the value for `innercounter` in the `ld b,innercount` 
instruction. Because `innercounter` is defined in the local scope of the loop, this scope is immediately disposed as
the loop is completed. When the compiler processes the `ld b,innercounter` instruction, the local scope is not 
available.

## Labels and Scopes

Labels behave like symbols, and they work similarly. When you create a label within a loop, that label is created in
the local scope of the loop. The following code helps you understand which labels are the part of the global scope, and
which are created in the loop's scope:

```
.org #8000
MyLoop: .loop 2
    ld bc,MyLoop
Inner: 
    ld de,MyEnd
    ld hl,Inner
    ld ix,Outer
MyEnd: .endl
Outer: nop
```

The label of the `.loop` statement is part of the outer (global) scope, just like the label that *follows* the 
`.endl` statement. However, all labels declared within the loop's body, including the label of the `.endl`
statement belongs to the local scope of the loop.

Thus, the compiler translates the code above into this one:

```
         (#8000): ld bc,#8000 (MyLoop)
Inner_1  (#8003): ld de,#800D (MyEnd_1)
         (#8006): ld hl,#8003 (Inner_1)
         (#8009): ld ix,#801A (Outer)
MyEnd_1  (#800D): ld bc,#8000 (MyLoop)
Inner_2  (#8010): ld de,#801A (MyEnd_2)
         (#8013): ld hl,#8010 (Inner_2)
         (#8016): ld ix,#801A (Outer)
MyEnd_2
Outer    (#801A): nop
```

Here, `Inner_1`, `Inner_2`, `MyEnd_1`, and `MyEnd_2` represents the labels created in the local scope of the
loop. The `_1` and `_2` suffixes indicate that each loop iteration has a separate local scope. As you can see,
the last iteration of `MyLabel` points to the first outer address (`Outer` label).

## Nesting LOOPs

Of course, you can nest loops, such as in this code:

```
.loop 3
  nop
  .loop 2
    ld a,b
  .endl
  inc b
.endl
```

This code snippet translates to this:

```
nop
ld a,b
ld a,b
inc b
nop
ld a,b
ld a,b
inc b
nop
ld a,b
ld a,b
inc b
```

When you nest loops, each loop has its separate scope.

## The $CNT value

It is very useful to use the `$cnt` value that represents the current loop counter. It starts from 
1 and increments to the maximum number of loops. This sample demonstrates how you can use it:

```
.loop 2
  outerCount = $cnt
  .loop 3
     .db #10 * outerCount + $cnt
  .endl
.endl
```

This code translates to this:

```
.db #11
.db #12
.db #13
.db #21
.db #22
.db #23
```

You can observe that each loop has its spearate `$cnt` value.

> The `$ctn` value has several syntax versions that the compiler accepts: `$CNT`, 
> `.cnt`, and `.CNT`.

## The PROC..ENDP Block

In the previous section you could understand how labels and scopes work for the `.loop` statement.
You can utilize this scoping mechanism with the help of the `.proc`..`.endp` statement.
This sample code demonstrates the concepts (just as you learned earlier):

```
.org #8000
MyLabel:
  ld de,Outer
  ld hl,Mylabel
  call MyProc
  halt

MyProc: 
  .proc
    ld bc,MyProc
  MyLabel: 
    ld de,MyEnd
    ld hl,MyLabel
    ld ix,Outer
    ret
MyEnd:
    .endp
Outer: nop
```

The first `MyLabel` label belongs to the global scope, while the second (within `MyProc`)
to the local scope of the procedure wrapped between `.proc` and `endp`. `MyProc` belongs to the
global scope too, however, `MyEnd` is the part of the `MyProc` scope, so it is visible only from
within the procedure.

The assembler emits this code:

```
MyLabel  (#8000): ld de,#8018 (Outer)
         (#8003): ld hl,#8000 (MyLabel)
         (#8006): call #800A (MyProc)
         (#8009): halt
MyProc   (#800A): ld bc,#800A (MyProc)
MyLabel_ (#800D): ld de,#8018 (MyEnd)
         (#8010): ld hl,#800D (MyLabel_)
         (#8013): ld ix,#8018 (Outer)
         (#8017): ret
MyEnd
Outer    (#8018): nop
```

You can nest `PROC` bloks just as `LOOP` blocks. Each `PROC` block has its private scope.
When the compiler sees a `PROC` block, it works just as if you wrote `.loop 1`.

> NOTE: `PROC` is different than a loop. You cannot use the `$cnt` value. Similarly, the `break` 
> and `continue` instructions are unavailable within a `PROC` block.

> The assembler accepts these aliases for `PROC` and `ENDP`: `.proc`, `proc`, `.PROC`
> , `PROC`, `.endp`, `.ENDP`, `endp`, `ENDP`, `.pend`, `.PEND`, `pend`, `PEND`.

## The REPEAT..UNTIL Block

While the `.loop` statement works with an expression that specified the loop counter,
the `.repeat`..`.until` block uses an exit condition to create more flexible loops.
Here is a sample:

```
counter = 0
.repeat 
    .db counter
    counter = counter + 3
.until counter % 7 == 0
```

Observe, the `counter % 7 == 0` condition specifies when *to exit* the loop. Because the
exit condition is examined only at the end of the loop, the `.repeat` blocks executes 
at least once.

The sample above translates to this:

```
.db 0
.db 3
.db 6
.db 9
.db 12
.db 15
.db 18
```

The `.repeat` block uses the same approach to handle its local scope, symbols, labels, and
variables as the `.loop` block. The block also provides the `$cnt` loop counter that starts 
from 1 and increments in every loop cycle. 

This sample demontrates the `.repeat` block in action:

```
.org #8000
counter = 0
.repeat 
    .db low(EndLabel), high(Endlabel), $cnt
    counter = counter + 3
EndLabel: .until counter % 7 == 0
```

The compiler translates the code to this:

```
.db #03, #80, #01
.db #06, #80, #02
.db #09, #80, #03
.db #0C, #80, #04
.db #0F, #80, #05
.db #12, #80, #06
.db #15, #80, #07
```

## The WHILE..ENDW Block

With `.while` loop, you can create another kind of block, which uses entry condition. For example,
the following code snippet generates instructions to create the sum of numbers from 1 to 9:

```
counter = 1
    ld a,0
.while counter < 10
    add a,counter
    counter = counter + 1
.endw
```

The `.while`..`.endw` block uses an entry condition declared in the `.while` statement. Provided, this
condition is true, the compiler enters into the body of the loop, and compiles all instructions and statements
until it reaches the `.endw` statement. Observe, it may happen that the body of the loop is never reached.

The compiler translates the code snippet above to the following:

```
ld a,0
add a,1
add a,2
add a,3
add a,4
add a,5
add a,6
add a,7
add a,8
add a,9
```

Just like the `.loop` and the `.repeat` blocks, `.while` uses the same approach to handle its local scope, 
symbols, labels, and variables. This block also provides the `$cnt` loop counter that starts  from 1 and increments 
in every loop cycle.

This code demonstrates the `.while` block with labels and using `$cnt` value:

```
counter = 0
.while counter < 21 
    .db low(EndLabel), high(Endlabel), $cnt
    counter = counter + 3
EndLabel: .endw
```

The compiler translates the code to this:

```
.db #03, #80, #01
.db #06, #80, #02
.db #09, #80, #03
.db #0C, #80, #04
.db #0F, #80, #05
.db #12, #80, #06
.db #15, #80, #07
```

> You can use many flavors for the `.endw` block closing statement. `.endw`, `endw`, `.wend`, `wend`
> are all accepted &mdash; with fully uppercase letters, too.

## The FOR..NEXT Loop

Tou can use the traditional `.for`..`.next` loop to create a loop:

```
.for myVar = 2 .to 5
  .db 1 << int(myVar)
.next
```

This loop uses the `myVar` variable as its *iteration variable*, which iterates from 1 to 4. As you expect, 
the compiler translates the for-loop into this:

```
.db #04
.db #08
.db #10
.db #20
```

You can specify a `.step` close to change the loop increment value:

```
.for myVar = 1 .to 7 .step 2
  .db 1 << int(myVar)
.next
```

Now, the code translates to this:

```
.db #02
.db #08
.db #20
.db #80
```

You can create a loop with decrementing iteration variable value:

```
.for myVar = 7 .to 1 .step -2
  .db 1 << int(myVar)
.next
```

As you expect, now you get this translation:

```
.db #80
.db #20
.db #08
.db #02
```

> Just as with the other statements, you can use the `.for`, `.to`, and `.step` keywords without the `.`
> prefix, so `for`, `to`, and `step` are also valid.

The for-loop can do the same stunts as the other kind of loops; it handles labels, symbols, and variables exactly 
the same way. There's only one exception, the loop iteration variable. If this variable is found in an outer scope,
instead of using that value, the compiler raises an error. You can us the for-loop only with a freshly created
variable.

So both cases in this code raise an error:

```
myVar = 0
.for myVar = 1 .to 4 ; ERROR: Variable myVar is already declared
  ; ...
.next

.for _i = 1 .to 3
  .for _i = 3 .to 8 ; ; ERROR: Variable _i is already declared
    ; ...
  .next
.next
```

> As `i` is a reserved token (it represents the `I` register), you cannot use `i` as a variable name. Nonetheless,
> `_i` is a valid variable name.

The for-loop works with both integer and float variables. If any of the initial value, the last value (the one after `.to`), 
or the increment value (the one after `.step`) is a float value, the for-loop uses float operations; otherwise it uses
integer operations.

This code snippet demonstrates the difference:

```
.for myVar = 1 .to 4 .step 1
  .db 1 << myVar
.next

.for myVar = 1 .to 4 .step 1.4
  .db 1 << myVar ; ERROR: Right operand of the shift left operator must be integral
.next
```

Nonetheless, you can solve this issue with applying the `int()` function:

```
.for myVar = 1 .to 4 .step 1.4
  .db 1 << int(myVar) ; Now, it's OK.
.next
```

> You can still use the `$cnt` value in for loops. Just like with other loop, it indicates the count of
> cycles strating from one and incremented by one in each iteration.

## Maximum Loop Count

It's pretty easy to create an infinite (or at least a very long) loop. For example, these loops are
obviously infinite ones:

```
.repeat
.until false

.while true
.wend 
```

The assembler checks the loop counter during compilation. Whenever it exceeds #FFFF (65535), it raises an error.

## The IF..ELIF..ELSE..ENDIF Statement

You can use the `.if` statement to create branches with conditions. For example, this code emits `inc b`
or `inc c` statement depending on whether the value of `branch` is even or odd:

```
.if branch % 2 == 0
  inc b
.else
  inc c
.endif
```

You do not have to specify an `.else` branch, so this statement is entirely valid:

```
.if branch % 2 == 0
  inc b
.endif
```

You can nest if statements like this to manage four different code branches according to the value of `branch`:

```
.if branch == 1
  inc b
.else
  .if branch == 2
    inc c
  .else 
    .if branch == 3
      inc d
    .else
      inc e
    .endif
  .endif
.endif
```

Nonetheless, you can use the `.elif` statement to create the code snippet above in clearer way:

```
.if branch == 1
  inc b
.elif branch == 2
  inc c
.elif branch == 3
  inc d
.else
  inc e
.endif
```

## IF and Scopes

Unlike the loop statements, `.if` does not provide its local scope. Whenever you create a symbol, a label or
a variable, those get into the current scope. This code defines a label with the same name in each branches. Because
the compiler evaluates the `.if` branches from top to down, it either compiles one of the `.elif` branches &mdash;
the first with a matching condition &mdash; or the else branch. Thus, this code does not define `MyLabel` twice:

```
branch = 4 ; Try to set up a different value
; Do something (omitted from code)
    ld hl,MyLabel
.if branch == 1
  inc b
  MyLabel ld a,20
.elif branch > 2
  MyLabel ld a,30
  inc c
.elif branch < 6
  inc d
  MyLabel ld a,40
.else
  MyLabel ld a,50
  inc e
.endif
```

Generally, you can decorate any statement with labels. The `.elif` and `.else` statements are exception. If you
do so, the compiler raises an error:

```
.if branch == 1
  inc b
  MyLabel ld a,20
.elif branch > 2
  MyLabel ld a,30
  inc c
Other .elif branch < 6 ; ERROR: ELIF section cannot have a label
  inc d
  MyLabel ld a,40
Another .else          ; ERROR: ELSE section cannot have a label
  MyLabel ld a,50
  inc e
.endif
```

## IF Nesting

When you nest `.if` statements, take care that each of them has a corresponding `.endif`. Whenever
the compiler finds an `.endif`, is associates it with the closest `.if` statement before `.endif`.
I suggest you use indentation to make the structure more straightforward, as the following code snippet shows:

```
row = 2
col = 2
; Change row and col (omitted from code)
.if row == 0
  .if col == 0
    .db #00
  .elif col == 1
    .db #01
  .else
    .db #02
  .endif
.elif row == 1
  .if col == 0
    .db #03
  .elif col == 1
    .db #04
  .else
    .db #05
  .endif
.elif row == 2
  .if col == 0
    .db #06
  .elif col == 1
    .db #07
  .else
    .db #08
  .endif
.else
  .if col == 0
    .db #09
  .elif col == 1
    .db #0A
  .else
    .db #0B
  .endif
.endif
```

## The IFUSED/IFNUSED Statements

__SpectNetIDE__ offers a similar construct to IF..ELIF..ELSE..ENDIF, using the IFUSED or IFNUSED statement instead of IF. These new statements are specialized forms of IF. You can use these statements to emit code depending on whether a symbol (label, `.EQU`, `.VAR`, structure, or structure field) exists and has already been used by the code preceding the IFUSED/IFNUSED statement.

Here are a few examples:

```
MyProc:
  ld hl,#5800
  ld (hl),a
  ret
  ; some other code

  .ifused MyProc
    MyMsg: .defn "MyProc is used"
  .else
    MyMsg: .defn "MyProc is not used"
  .endif

Main:
  ld hl,MyMsg
```

Here, the `.ifused` statement will set the string the `MyMsg` label point to according to whether the `MyProc` label is used, or not. As in this case `MyProc` is defined but not invoked before the `.ifused` statement, __HL__ will point to the "MyProc is not used" message.

Should you call `MyProc` before `.ifused`, __HL__ would point to the other message, "MyProc is used":

```
MyProc:
  ld hl,#5800
  ld (hl),a
  ret
  ; some other code
  call MyProc
  ; some other code

  .ifused MyProc
    MyMsg: .defn "MyProc is used"
  .else
    MyMsg: .defn "MyProc is not used"
  .endif

Main:
  ld hl,MyMsg
```

The `.ifnused` statement is the complement of `.ifused`. It is evaluated to a true condition value only if the symbol following `.ifnused` is not defined, or, if defined, is not used.

### IFUSED/IFNUSED Syntax

You need to specify a symbol after the `.ifused`  or `.ifnused` keywords. These symbols must follow the syntax of identifiers. They can be compound names used for modules and structures. So, all of these symbol names are correct:

```
MyLabel
MyStruct
MyStruct.FieldX
MyModule.Main
::NestedModule.Start.MyProc
```

> __Note__: You can use these aliases for `.ifused`: `.IFUSED`, `ifused`, `IFUSED`. Similarly, `.ifnused` accept alternative tokens: `.IFNUSED`, `ifnused`, `IFNUSED`.

### IFUSED/IFNUSED Semantics

The __SpectNetIDE__ compiler accepts any `.ifused` and `.ifnused` statements until they are syntactically correct. When the assembler tests their condition, it works this way:
- If the specified symbol does not exists, `.ifused` evaluates to false, while `.ifnused` evaluates to true.
- If the particular symbol exists and it is used in the code section preceding the `.ifused` or `.ifnused` statement, `.ifused` evaluates to true, `.ifnused` to false.
- If the particular symbol exists and it is _not_ used in the code section preceding the `.ifused` or `.ifnused` statement, `.ifused` evaluates to false, `.ifnused` to true.

These statements do not support look-ahead in the code. This behavior could lead to paradox situations, like in this example:

```
MyFlag = true
MyValue: .equ #1234
  ; some other code that does not use MyValue

  .ifused MyValue
    MyFlag = false;
  .endif

  ; some other code that does not change MyFlag

  .if MyFlag
    ld a,MyValue
  .endif
```

Should `.ifused` work with look-ahead, this code would make the compiler scratch its virtual head. Because `MyFlag` is set to true, the `.if` statement at the bottom of the code would emit an `ld a,MyValue` instruction. Knowing this fact, the compiler would say that `.ifused MyValue` should be evaluated to true. However, in this case, the body `.ifused` would set `MyFlag` to true, and that would prevent the bottom `.if` to emit `ld a,MyValue`, and then `MyValue` would not be used at all.

## Block Statements without a Closing Statement

The compiler automatically recognizes if a block does not have a closing statement, and provides an
error message accordingly.

## Orphan Closing Statements

When the compiler finds a closing statement (such as `.endw`, `.endl`, `.until`, `.endif`, etc.) it will
issue an error.

## The BREAK statement

You can exit the loop &mdash; independently of the loop's exit condition &mdash; with the `.break` statement:

```
; LOOP sample
.loop 5
  .if $cnt == 4
    .break
  .endif
  .db $cnt
.endl

; REPEAT sample
.repeat
  .if $cnt == 4
    .break
  .endif
  .db $cnt
.until $cnt == 5

; WHILE sample
.while $cnt < 5
  .if $cnt == 4
    .break
  .endif
  .db $cnt
.endw

; FOR-loop sample
.for value = 1 to 5
  .if value == 4
    .break
  .endif
  .db value
.next
```

Because all these loops are exited at the beginning of the 4th iteration, they produce this output:

```
.db #01
.db #02
.db #03
```

> You cannot use the `.break` statement outside of a loop construct. If you do so, the compiler 
> raises an error.

## The CONTINUE Statement

You can interrupt the current iteration of the loop and carry on the next iteration with the `.continue` statement:

```
; LOOP sample
.loop 5
  .if $cnt == 4
    .continue
  .endif
  .db $cnt
.endl

; REPEAT sample
.repeat
  .if $cnt == 4
    .continue
  .endif
  .db $cnt
.until $cnt == 5

; WHILE sample
.while $cnt <= 5 
  .if $cnt == 4
    .continue
  .endif
  .db $cnt
.endw

; FOR-loop sample
.for value = 1 to 5
  .if value == 4
    .continue
  .endif
  .db value
.next
```

Because all these loops skip the 4th iteration, they produce this output:

```
.db #01
.db #02
.db #03
; #04 is skipped
.db #05
```

> You cannot use the `.continue` statement outside of a loop construct. If you do so, the compiler 
> raises an error.
