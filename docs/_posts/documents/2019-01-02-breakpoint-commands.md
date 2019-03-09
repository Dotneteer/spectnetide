---
layout: documents
categories: 
  - "Tool Commands"
title:  "Disassembly: Breakpoints"
alias: breakpoint-commands
seqno: 50
selector: documents
permalink: "documents/breakpoint-commands"
---

With these commands, you can set up breakpoint according to their addresses. These breakpoints are not persisted,
they are removed when you close the solution.

Command | Description
--------|------------
__`SB`__ *`literal`* [ __`H`__ *`hit-condition`*] [ __`C`__ *`filter-condition`*] | Sets a breakpoint at the specified address with the optional *hit condition* and/or *filter condition*.
__`TB`__ *`literal`* | Toggles a breakpoint at the specified address.
__`RB`__ *`literal`* | Removes the breakpoint from the specified address.
__`UB`__ *`literal`* | Retrieves the breakpoint at the specified address so that you can update it.
__`EB`__ | Erases all breakpoints.

### Hit Condition

The __`SB`__ command allows you to specify a hit condition to define when the program should stop at the specified breakpoint.
The debugger counts how many times the program code reaches the breakpoint and stops when the hit condition meets:

__`H`__ *`condition-type`* *`condition-value`*

The *condition-value* is an integer number. You can apply one of these *condition-type* tokens:

Type | Description
-----|------------
__`<`__ | Execution stops when the current hit counter is less than *condition-value*
__`<=`__ | Execution stops when the current hit counter is less than or equal to *condition-value*
__`=`__ | Execution stops when the current hit counter is equal to *condition-value*
__`>`__ | Execution stops when the current hit counter is greater than *condition-value*
__`>=`__ | Execution stops when the current hit counter is greater than or equal to *condition-value*
__`*`__ | Execution stops when the current hit counter is a multiple of *condition-value*

The following example defines a hit condition that stops at the `$8010` address when the 
hit counter s greater than 10:

```
SB 8010 H>10
```

These command sets a breakpoint at `$6100` to stop at every fifth hit:

```
SB 6000 H*5*
```

### Filter Condition

You can apply not only hit conditions, but also *filter conditions* to a breakpoint. When
the execution reaches the breakpoint, the debugger evaluates the expression. If it is a `true`
value (non-zero integer), the execution flow pauses; otherwise it goes on without stoping.

You can use the same syntax for defining a filter condition as for watch items in the 
__Watch Memory tool window__.

> When the watch expression results an evaluation error, the debug engine pauses as if there
> were no filter condition.

Let's see a few examples. The following command defines a breakpoint at `$6800` that stops when
the contents of the __HL__ register is `$4020`:

```
SB 6800 C HL==#4020
```

This condition breakpoint tests if the value of the memory address `$4100` equals to `$FF`:

```
SB 7A00 C [#4100]==#FF
```

You can use the condition to check if there's a `$20` value at the __IX+12__ address:

```
SB 6500 C [IX+12]==#20
```

The following condition results in a "Divide by zero" error, so it stops every time the execution
flow reaches the `$6200` address:

```
SB 6200 C HL/0==2
```

### Combining Hit Conditions and Filter Conditions

You can apply both hit and filter conditions for the same breakpoint. You have to define the hit
condition first, filter condition next:

```
SB 6400 H>5 C B<=10
```

If you exchange the condition order, the command prompt will indicate syntax error:

```
SB 6400 C B<=10 H>5
```

When you apply both conditions, they must be both satisfied to pause at that particular breakpoint.
