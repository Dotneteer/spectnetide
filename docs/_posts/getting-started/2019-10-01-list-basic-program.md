---
layout: documents
categories: 
  - "Tutorials v2"
title:  "List the BASIC program"
alias: list-basic-program
seqno: 1032
selector: tutorial
permalink: "getting-started/list-basic-program"
---

With the **ZX Spectrum BASIC list** tool window, you can display the BASIC program typed or loaded into your running ZX Spectrum machine. Here you can see the program code of the **Jackpot** game while it's running:

![List BASIC program]({{ site.baseurl }}/assets/images/tutorials/list-basic-program.png)

The toolbar of the **ZX Spectrum BASIC List** window has two buttons:

![List BASIC toolbar]({{ site.baseurl }}/assets/images/tutorials/list-basic-toolbar.png)

- The first button allows you to export the BASIC listing.
- With the second button you can toggle between *Sinclair* and *ZX BASIC* modes.

## Export BASIC Listing

With this command, you can export the BASIC listing to a file:

![Predefined symbols]({{ site.baseurl }}/assets/images/commands/export-basic-listing-dialog.png)

By setting the **Mimic ZX BASIC** checkbox, the output will use ZX BASIC escape sequences for control characters.

## Set the display mode of BASIC 

In *Sinclair* mode, control characters (e.g., __INK__, __PAPER__, __BRIGHT__, etc.) are shown with their hexadecimal code between `°` characters: 

![Sinclair BASIC mode]({{ site.baseurl }}/assets/images/tutorials/sinclair-basic-mode.png)

However, in *ZX BASIC* mode, the control characters are displayed with their ZX BASIC escape sequences. Inline control characters (out of string values wrapped in double quotes) are omitted:

![ZX BASIC mode]({{ site.baseurl }}/assets/images/tutorials/zx-basic-mode.png)
