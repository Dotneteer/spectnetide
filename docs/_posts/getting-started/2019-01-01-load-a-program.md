---
layout: documents
categories: 
  - "Tutorials"
title:  "LOAD a Program"
alias: load-a-program
seqno: 60
selector: tutorial
permalink: "getting-started/load-a-program"
---

You can load a program from the tape into the memory with the `LOAD ""` statement. Hey, we do not have a tape, so how does SpectNetIDE know what program to load?

As a part of your project, you can add tape files with standard `.TAP` or `.TZX` format. When the IDE creates a new ZX Spectrum project, it adds a sample tape file, `Welcome.tzx`; it puts it into the `TapeFiles` folder. With the Add Existing Item command of Visual Studio, you can attach more tape files to the project. The virtual machine -- that runs the ZX Spectrum Emulator -- can work only with a single tape file at a time. To load that one -- called the default tape --, follow these steps:

1. In Solution Explorer, select the tape file you want to attach to the virtual machine.

2. Right-click the __Set as default tape file__ command:

![Set default tape]({{ site.baseurl }}/assets/images/tutorials/set-default-tape-file.png)

The IDE marks the default tape file with bold typeface.

![Default tape file set]({{ site.baseurl }}/assets/images/tutorials/default-tape-file-set.png)

{:start="3"}
3. Next time you enter the __`LOAD ""`__ statement, the virtual machine will play the newly selected tape file.

![Pacman loads]({{ site.baseurl }}/assets/images/tutorials/pacman-loads.png)

> __Note__: You can change the default tape file between loads while the machine runs -- without stopping and restarting it.
