---
layout: documents
categories: 
  - "Tutorial"
title:  "Use the Keyboard Tool"
alias: use-keyboard-tool
seqno: 21
selector: tutorial
permalink: "getting-started/use-keyboard-tool"
---

SpectNetIDE provides a Keyboard Tool window to help you using the ZX Spectrum Keyboard. Of course, you can use your computer's keyboard to type, but sometimes the Keyboard Tool makes your work easier.
The IDE allows you using two keyboard layouts, ZX Spectrum 48K, and ZX Spectrum 128K (+2, +2A, +3), respectively.

![ZX Spectrum 48K keyboard]({{ site.baseurl }}/assets/images/tutorials/spectrum-48-keyboard.png)

![ZX Spectrum 128K keyboard]({{ site.baseurl }}/assets/images/tutorials/spectrum-128-keyboard.png)

## Selecting the Keyboard Layout

SpectNetIDE provides a Keyboard Tool window to help you using the ZX Spectrum Keyboard. Of course, you can use your computer's keyboard to type, but sometimes the Keyboard Tool makes your work easier.
The IDE allows you to use two keyboard layouts, ZX Spectrum 48K, and ZX Spectrum 128K (+2, +2A, +3), respectively.

By default, SpectNetIDE uses the layout according to your project's machine type. Nonetheless, you can change these settings in the Tools|Options dialog within the Spect.Net IDE tab. The Keyboard Tool section contains two configuration properties.
The Keyboard layout allows you to select from the Default, Spectrum48, and Spectrum128 values. Depending on your monitor type, the original layout size may not display very well on the screen. With the Keyboard display mode property, you can change the OriginalSize value to Fit. This setting automatically resizes the keyboard as you change the size of the Keyboard tool window.

![ZX Spectrum keyboard options]({{ site.baseurl }}/assets/images/tutorials/keyboard-options.png)

## Keystrokes

To press a key in the tool window, move the mouse over the part of a particular key you want to enter and click the mouse. The tool window senses which part of the key you clicked and emulates that keystroke, applying `CAPS SHIFT`, `SYMBOL SHIFT`, `EXTENDED MODE`, as required.

### Normal keystroke

![Normal keystroke 48K]({{ site.baseurl }}/assets/images/tutorials/normal-keystroke-48.png)
![Normal keystroke 128K]({{ site.baseurl }}/assets/images/tutorials/normal-keystroke-128.png)

The left and right mouse buttons emulate different keystroke. The left button enters the key normally, the right button imitates entering the key with `CAPS SHIFT`.

### SYM SHIFT keystroke

![SYM SHIFT keystroke 48K]({{ site.baseurl }}/assets/images/tutorials/sym-shift-keystroke-48.png)
![SYM SHIFT keystroke 128K]({{ site.baseurl }}/assets/images/tutorials/sym-shift-keystroke-128.png)

### EXTENDED MODE keystroke

![EXTENDED MODE keystroke 48K]({{ site.baseurl }}/assets/images/tutorials/ext-mode-keystroke-48.png)
![EXTENDED MODE keystroke 128K]({{ site.baseurl }}/assets/images/tutorials/ext-mode-keystroke-128.png)

### EXTENDED MODE + SHIFT keystroke

![EXTENDED MODE shift keystroke 48K]({{ site.baseurl }}/assets/images/tutorials/ext-shift-keystroke-48.png)
![EXTENDED MODE shift keystroke 128K]({{ site.baseurl }}/assets/images/tutorials/ext-shift-keystroke-128.png)

### Colors

The 0...7 keys have a function to change the color of ink and/or paper. For example, key 5 sets the color to `CYAN`.

![Color keystroke 48K]({{ site.baseurl }}/assets/images/tutorials/color-keystroke-48.png)
![Color keystroke 128K]({{ site.baseurl }}/assets/images/tutorials/color-keystroke-128.png)

The left mouse button sets the paper color, while the right mouse button modifies the ink color.

### Graphic symbols

The 1...8 keys allow you to enter graphic symbols. Click the particular symbol to emulate the keystroke -- left and right buttons work the same way.

![Graphic keystroke 48K]({{ site.baseurl }}/assets/images/tutorials/graphic-keystroke-48.png)
![Graphic keystroke 128K]({{ site.baseurl }}/assets/images/tutorials/graphic-keystroke-128.png)

> __Note__: On specific computers -- depending on the amount of free RAM and CPU load -- you may experience that entering graphic symbols sometimes fails, or the keyboard does not return the letter mode. In such a situation, click the GRAPHICS function (above key 9) and then to the digit beside the appropriate graphic symbol.

## Using the Physical and Virtual Keyboards Together

You can use the physical keyboard to type into the ZX Spectrum emulator only when the emulator tool window has the keyboard focus. When you apply keystrokes in the Keyboard tool, that window receives the focus. So to be able to use the physical keyboard again, click the ZX Spectrum emulator to shift the focus.
