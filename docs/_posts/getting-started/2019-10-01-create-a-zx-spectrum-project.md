---
layout: documents
categories: 
  - "Tutorials v2"
title:  "Create a ZX Spectrum 48K Project"
alias: create-zx-spectrum-48k-project-2
seqno: 1020
selector: tutorial
permalink: "getting-started/create-zx-spectrum-48k-project-2"
---

1. Run the __File &rarr; New &rarr; Project__ command (Ctrl+Shift+N). In the Create a new project dialog type "ZX Spectrum" in the search box. The dialog displays the ZX Spectrum Code Discovery Project. Select this type, and click Next.  

![New Zx Spectrum Project]({{ site.baseurl }}/assets/images/tutorials/new-zx-spectrum-project-2.png)

{:start="2"}
2. Type MyFirstDiscovery in the Project name field, and then click Create.

![Specify project info]({{ site.baseurl }}/assets/images/tutorials/specify-project-name.png)

{:start="3"}
3. The IDE displays a list of available Spectrum models. Select the first Spectrum 48K model
(PAL - Normal Speed) from the list, and click __Create__.

![Select Spectrum model]({{ site.baseurl }}/assets/images/tutorials/select-machine-type-2.png)

{:start="3"}
3. The IDE creates a new project with a few files and folders:

![Solution Explorer with the new project]({{ site.baseurl }}/assets/images/tutorials/solution-structure-2.png)

File/Folder | Description
----------- | -----------
__`Rom`__ | The folder that holds Spectrum ROMs
__`Rom/ZxSpectrum.spconfig`__ | This file stores the configuration information about the selected ZX Spectrum model
__`Rom/ZxSpectrum48.rom`__ | The binary ROM file for the Spectrum 48K model
__`Rom/ZxSpectrum48.disann`__ | Disassembly annotations for the Spectrum 48K ROM
__`TapeFiles`__ | Stores *.tzx*, *.tap* (and, in the future, other) tape files
__`Z80CodeFiles`__ | The folder to put your Z80 Assembly code files in
__`Z80CodeFiles/Code.z80asm`__ | A simple Z80 Assembly code file
__`Z80UnitTests`__ | The folder to put your Z80 unit tests files in
__`Z80UnitTests/UnitTest.z80test`__ | A simple Z80 unit test file
__`Z80BasicFiles`__ | The folder to put your ZX BASIC files in
__`Z80BasicFiles/UnitTest.z80test`__ | A simple ZX BASIC program file
__`Annotations.disann`__ | Your custom disassembly annotations are saved into this file

{:start="4"}
4. As soon as the IDE created the project, you can discover a new submenu, __ZX Spectrum IDE__, under __Extensions__:

![ZX Spectrum IDE menu]({{ site.baseurl }}/assets/images/tutorials/zx-spectrum-menu-2.png)

## Run the ZX Spectrum Virtual Machine

1. Run the __ZX Spectrum IDE|ZX Spectrum Emulator__ command. The IDE shows up the emulator tool window.
As the title of the tool window indicates, the virtual machine is not started yet. 

![The Emulator Tool Window]({{ site.baseurl }}/assets/images/tutorials/machine-not-started-2.png)

{:start="2"}
2. Click the little *Play* icon in the toolbar of the emulator to start the virtual machine.
The ZX Spectrum computer comes to life. As you resize the tool window, the emulator changes its screen size, 
accordingly.

![Spectrum VM started]({{ site.baseurl }}/assets/images/tutorials/machine-started-2.png)

{:start="3"}
3. Type the __`LOAD ""`__ command into the emulator. Take care that the emulator window is the active one, receiving the
keyboard focus, and press the __J__, and then twice the __Shift+P__ keys again. With pressing __Enter__, you can execute __`LOAD ""`__.
Th virtual machine starts loading the `Welcome.tzx` file.

![Loading a Game]({{ site.baseurl }}/assets/images/tutorials/machine-loading-2.png)

{:start="4"}
4. When the game has been loaded, type the __`RUN`__ command into the emulator (while the emulator window is the active one, press the __R__ key),
and then, press __Enter__. The program starts, and displays its message:

![Welcome in action]({{ site.baseurl }}/assets/images/tutorials/run-completed-2.png)

You have just scratched the surface! There are many other features you can use in the
IDE to discover the structure and internals of ZX Spectrum applications.
