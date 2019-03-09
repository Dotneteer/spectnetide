---
layout: documents
categories: 
  - "Tool Commands"
title:  "Watch Commands"
alias: watch-commands
seqno: 60
selector: documents
permalink: "documents/watch-commands"
---

With these commands, you can manage the list of items in the __Watch Memory__ tool window:

## Add a New Watch Item Command

__`+`__ *`expression`* [*`format`*]

Appends a new watch expression to the watch list.

## Remove a Watch Item command

__`-`__ *`index`*

Removes the watch item with the specified index from the list. Automatically renumbers the indexes of
remaining items.

> You can access this command from the context menu of a watch item (__Remove__)

## Modify a Watch Item command

__`*`__ *`index`* *`expression`* [*`format`*]

Modifies the watch item with the specified index to the provided `expression` and optional `format`.

> You can access this command from the context menu of a watch item (__Modify__)

## Exchange Watch Items command

__`XW`__ *`index1`* *`index2`*

Exchanges the watch items specified by the two indexes in the list.

> You can move up or down watch items with the commands available from the context menu of an item
(__Move up__, __Move down__)

## Erase the Watch Item List command

__`EW`__

Erases the entire watch item list.

## Set Watch Item Label Width command

__`LW`__ *`index`*

Sets the width of the label that displays the watch expression to the value specified by `index`. Here index is
used as a width in pixels.

> Instead of using the command line, you can use a sizing grip between the watch item's expression and its displayed
value to change the width with the mouse.
