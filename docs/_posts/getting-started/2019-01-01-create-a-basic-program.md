---
layout: documents
categories: 
  - "Tutorials"
title:  "Create a BASIC Program"
alias: create-a-basic-program
seqno: 30
selector: tutorial
permalink: "getting-started/create-a-basic-program"
---

In a previous tutorial, you could see how easy is to create a new ZX Spectrum program from scratch.

Here, you will create and run a simple BASIC program. Although you can use the keyboard of the PC to enter a program — provided, the ZX Spectrum Emulator window has the focus — if you're not familiar with the Spectrum keys and BASIC editor, it may frustrate you while entering the code. To avoid such chaffing, use the ZX Spectrum Keyboard tool window to enter the program.

First, add this line:

```
10 BORDER 2
```

To enter this BASIC line, press these keys, in this order: "`1`", "`0`", "`B`", "`2`", "`Enter`" (of course, you do not need to click comma between the keys wrapped in double quotes)

As soon as you've added this program line, it appears in the screen listing:

![Basic line 1]({{ site.baseurl }}/assets/images/tutorials/basic-line-1.png)

Now, add these lines to the code:

```
20 BORDER 5
30 BORDER 6
40 PAUSE 1
50 GOTO 10
```

![Basic line 2]({{ site.baseurl }}/assets/images/tutorials/basic-line-1.png)

Your first BASIC program is ready to run. Invoke the `RUN` command (press
`R`, and then `Enter`) to start this code. As you can see from the listing, 
it implements an infinite loop while changing the background color:

![Basic code runs]({{ site.baseurl }}/assets/images/tutorials/basic-program-runs.png)

To stop the program, press the `SPACE` and `SYM SHIFT` keys simultaneously
or click the `BREAK` button in the ZX Spectrum Keyboard window with the right
mouse button:

![Basic program stopped]({{ site.baseurl }}/assets/images/tutorials/basic-program-stopped.png)

Congratulations! It is time to move on and create a Z80 assembly program!