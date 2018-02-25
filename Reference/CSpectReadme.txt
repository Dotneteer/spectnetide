CSpect V1.10 ZXSpectrum emulator by Mike Dailly
(c)1998-2018 All rights reserved

Be aware...emulator is far from well tested, might crash for any reason!


Whats new
======================================================================================
V1.10
-----
-cur to map cursor keys to 6789 (l/r/d/u)
Fixed copper instruction order
Copper fixed - and confirmed working. (changes are only visible on next line)
Copper increased to 2K - 1024 instructions
Fixed a bug in the AY emulation (updated source included)
Fixed Lowres colour palette selection
Added new "Beast" demo+source to the package to show off the copper


V1.09
-----
Layer 2 is now disabled if "shadow" screen is active  (bit 3 of port $7FFD)
Timex mode second ULA screen added (via port $FF bit 0 = 0 or 1). Second screen at $6000.
Fixed bit ?,(ix+*) type instructions in the disassembler. All $DD,$CB and $FD,$CB prefix instructions.


V1.08
-----
$123b is now readable
Fixed cursor disappearing


V1.07
-----
"Trace" text was still being drawn when window is too small.
"POKE"  debugger command added
New timings added to enhanced instructions - see below.


V1.06
-----
Border colour should now work again.... (palette paper offsets shifted)


V1.05
-----
Minor update to fix port $303b. If value is >63, it now writes 0 instead (as per the hardware)


V1.04
-----
Palettes now tested and working


V1.03
-----
Missing ay.dll from archive


V1.02
-----
Quick fix for ULA clip window


V1.01
----
Fixed a bug in delete key in debugger not working
Fixed Raster line IRQ so they now match the hardware
Fixed a bug in 128K SNA file loading
Fixed a bug in moving the memory window around using SHIFT key(s)
Removed SID support  :(
Added auto speed throttling.  While rendering the screen, if the speed is over 7Mhz, it will drop to 7Mhz. (*approximated)
Removed extra CPU instructions which are now defuct. (final list below)
MUL is now D*E=DE (8x8 bit = 16bit)
Current Next reg (via port $243b is now shown) in debugger
Current 8K MMU banks are now shown in debugger
CPU TRACE added to debugger view (see keys below) ( only when in 4x screen size mode)
Copper support added. Note: unlike hardware, changes will happen NEXT scanline.
Sprite Clip Window support added (Reg 25)
Layer 2 Clip Window support added (Reg 24)
ULA Clip Window support added (Reg 24)
LOWRES Clip Window support added (Reg 24)
Layer 2 2x palettes added
Sprite 2x palettes added
ULA 2x palettes added
Regiuster $243b is now readable

NOTE: SNASM now requires "opt ZXNEXTREG" to enable the "NextReg" opcode




V1.00
----
Startup and shut down crash should be fixed
You can now use register names in the debugger evaluation  ("M HL" instead of "M $1234", "BC HL" etc.)
G <val/reg/sym> to disassemble from address
-sound to disable audio
Timing fixed when no sound active.
-resid to enable loading and using of the reSID DLL. Note: not working yet - feel free to try and fix it! :)
-exit  to enable "EXIT" opcode of "DD 00"
-brk   to enable "BREAK" opcode of "DD 01"
-esc   to disable ESCAPE exit key (use exit opcode, close button or ALT+F4 to exit)
Fixed the Kempston Mouse interface, now works like the hardware.
Next registers are now readable (as per hardware)
local labels beginning with ! or . will now be stripped properly
Pressing CONTROL will release the mouse
Right shift is now also "Symbol shift"
3xAY audio added - many thanks to Matt Westcott (gasman)  https://github.com/gasman/libayemu
Timex Hicolour added
Timex Hires added
Lowest bit og BLUE can now be set
SHIFT+ENTER will set the PC to the current "user bar" address
Raster interrupts via Next registers $22 and $23
MMU memory mapping via NextReg $50 to $57
Added 3xAY demo by Purple Motion.
Source for ay.dll and resid.dll included (feel free to fix reSID.DLL!)
You can now specify the window size with -w1, -w2, -w3 and -w4(default). If winow is less than 3x then the debugger is not available
Cursor keys are now mapped to 5678 (ZX spectrum cursor)
Backspace now maps to LeftShift+0 (delete)
DMA now available! Simple block transfer (memory to memory, memory to port, port to memory)
SpecDrum sample interface included. Port $ffdf takes an 8 bit signed value and is output to audio. (not really tested)
Added the lowres demo (press 1+2 to switch demo)
Updated Mouse demo and added Raster Interrupts
Added DMA demo
Added 3xAY music demo


V0.9
----
register 20 ($14) Global transparancy colour added (defaults to $e3)
regisrer 7 Turbo mode selection implemented. 
Fixed 28Mhz to 14,7,3.5 transition. 
ULANext mode updated to new spec  (see https://www.specnext.com/tbblue-io-port-system/)
LDIRSCALE added
LDPIRX added


V0.8
----
New Lowres layer support added (details below)
ULANext palette support added (details below)
TEST $XX opcode fixed



V0.7
----
port $0057 and  $005b are now $xx57 and $xx5b, as per the hardware
Fixed some sprite rotation and flipping
Can now save via RST $08  (see layer 2 demo)
Some additions to SNasm (savebin, savesna, can now use : to seperate asm lines etc)


V0.6
----
Swapped HLDE to DEHL to aid C compilers
ACC32 changed to A32
TEST $00 added


V0.5
----
Added in lots more new Z80 opcodes (see below)
128 SNA snapshots can now be loaded properly
Shadow screen (held in bank 7) can now be used


V0.4
----
New debugger!!!  F1 will enter/exit the debugger
Sprite shape port has changed (as per spec) from  $55 to $5B
Loading files from RST $08 require the drive to be set (or gotten) properly - as per actual machines. 
  RST $08/$89 will return the drive needed. 
  Please see the example filesystem.asm for this.
  You can also pass in '*' or '$' to set the current drive to be system or default
Basic Audio now added (beeper)
New sprite priorities using register 21. Bits 4,3,2
New Layer 2 double buffering via Reg 18,Reg 19 and bit 3 of port $123b



Final new Z80 opcodes on the NEXT (V1.10.06 core)
======================================================================================
   swapnib           ED 23           8Ts   A bits 7-4 swap with A bits 3-0
   mul               ED 30           8Ts   multiply D*E = DE (no flags set)
   add  hl,a         ED 31           8Ts   Add A to HL (no flags set)
   add  de,a         ED 32           8Ts   Add A to DE (no flags set)
   add  bc,a         ED 33           8Ts   Add A to BC (no flags set)
   add  hl,$0000     ED 34 LO HI     16Ts  Add $0000 to HL (no flags set)
   add  de,$0000     ED 35 LO HI     16Ts  Add $0000 to DE (no flags set)
   add  bc,$0000     ED 36 LO HI     16Ts  Add $0000 to BC (no flags set)
   outinb            ED 90           16Ts  out (c),(hl), hl++
   ldix              ED A4           16Ts  As LDI,  but if byte==A does not copy
   ldirx             ED B4           21Ts  As LDIR, but if byte==A does not copy
   lddx              ED AC           16Ts  As LDD,  but if byte==A does not copy, and DE is incremented
   lddrx             ED BC           21Ts  As LDDR,  but if byte==A does not copy
   ldpirx            ED B7           16Ts  (de) = ( (hl&$fff8)+(E&7) ) when != A
   ldirscale         ED B6           21Ts  As LDIRX,  if(hl)!=A then (de)=(hl); HL_E'+=BC'; DE+=DE'; dec BC; Loop.
   mirror a          ED 24           8Ts   mirror the bits in A     
   mirror de         ED 26           8Ts   mirror the bits in DE     
   push $0000        ED 8A LO HI     19Ts  push 16bit immidiate value
   nextreg reg,val   ED 91 reg,val   16Ts  Set a NEXT register (like doing out($243b),reg then out($253b),val )
   nextreg reg,a     ED 92 reg       12Ts  Set a NEXT register using A (like doing out($243b),reg then out($253b),A )
   pixeldn           ED 93           8Ts   Move down a line on the ULA screen
   pixelad           ED 94           8Ts   using D,E (as Y,X) calculate the ULA screen address and store in HL
   setae             ED 95           8Ts   Using the lower 3 bits of E (X coordinate), set the correct bit value in A
   test $00          ED 27           11Ts  And A with $XX and set all flags. A is not affected.



Command line
======================================================================================
-zxnext            =  enable Next hardware registers
-zx128             =  enable ZX Spectrum 128 mode
-s7                =  enable 7Mhz mode
-s14               =  enable 14Mhz mode
-s28               =  enable 28Mhz mode
-exit              =  to enable "EXIT" opcode of "DD 00"
-brk               =  to enable "BREAK" opcode of "DD 01"
-esc               =  to disable ESCAPE exit key (use exit opcode, close button or ALT+F4 to exit)
-cur               =  to map cursor keys to 6789 (l/r/d/u)
-mmc=<dir>\        =  enable RST $08 usage, must provide path to "root" dir of emulated SD card (eg  "-mmc=.\" or "-mmc="c:\test\")
-map=<path\file>   =  SNASM format map file for use in the debugger. Local labels in the format "<primary>@<local>".





General Emulator Keys
======================================================================================
Escape - quit
F1 - Enter/Exit debugger
F2 - load SNA
F3 - reset
F5 - 3.5Mhz mode  	(when not in debugger)
F6 - 7Mhz mode		(when not in debugger)
F7 - 14Mhz mode		(when not in debugger)
F8 - 28Mhz mode		(when not in debugger)





Debugger Keys
======================================================================================
F1                  - Exit debugger
F2                  - load SNA
F3                  - reset
F7                  - single step
F8                  - Step over (for loops calls etc)
F9                  - toggle breakpoint on current line
Up                  - move user bar up
Down                - move user bar down
PageUp              - Page disassembly window up
PageDown            - Page disassembly window down
SHIFT+Up            - move memory window up 16 bytes
SHIFT+Down          - move memory window down 16 bytes
SHIFT+PageUp        - Page memory window up
SHIFT+PageDown      - Page memory window down
CTRL+SHIFT+Up       - move trace window up 16 bytes
CTRL+SHIFT+Down     - move trace window down 16 bytes
CTRL+SHIFT+PageUp   - Page trace window up
CTRL+SHIFT+PageDown - Page trace window down

Mouse is used to toggle "switches"
HEX/DEC mode can be toggled via "switches"




Debugger Commands
======================================================================================
M <address>     Set memory window base address
G <address>     Goto address in disassembly window
BR <address>    Toggle Breakpoint
WRITE <address> Toggle a WRITE access break point
READ  <address> Toggle a READ access break point (also when EXECUTED)
PUSH <value>    push a 16 bit value onto the stack
POP				pop the top of the stack
POKE <add>,<val>Poke a value into memory
Registers:
   A  <value>    Set the A register
   A' <value>    Set alternate A register
   F  <value>    Set the Flags register
   F' <value>    Set alternate Flags register
   AF <value>    Set 16bit register pair value
   AF'<value>    Set 16bit register pair value
   |
   | same for all others
   |
   SP <value>    Set the stack register
   PC <value>    Set alternate program counter register



LowRes mode
======================================================================================
Register 21 ($15) bit 7 enables the new mode. Layer priorities work as normal.
Register 50 ($32) Lowres X scroll (0-255) auto wraps
Register 51 ($32) Lowres Y scroll (0-191) auto wraps

Can not use shadow screen. Setting the shadow screen bit will switch to the standard ULA screen.
Screen is 128x96 byte-per-pixels in size. 
Top half  : 128x48 located at $4000 to $5800
Lower half: 128x48 located at $6000 to $6800


XScroll: 0-255
Being only 128 pixels in resolution, this allows the display to scroll in half pixels, at the same resolution and smoothness as Layer 2.

YScroll: 0-191
Being only 96 pixels in resolution, this allows the display to scroll in half pixels, at the same resolution and smoothness as Layer 2.




ULANext mode
======================================================================================
(W) 0x40 (64) => Palette Index
  bits 7-0 = Select the palette index to change the default colour. 
  0 to 127 indexes are to ink colours and 128 to 255 index are to papers.
  (Except full ink colour mode, that all values 0 to 255 are inks)
  Border colours are the same as paper 0 to 7, positions 128 to 135,
  even at full ink mode. 

(W) 0x41 (65) => Palette Value
  bits 7-0 = Colour for the palette index selected by the register 0x40. Format is RRRGGGBB
  After the write, the palette index is auto-incremented to the next index. 
  The changed palette remains until a Hard Reset.

(W) 0x42 (66) => Palette Format
  bits 7-0 = Number of the last ink colour entry on palette. (Reset to 15 after a Reset)
  This number can be 1, 3, 7, 15, 31, 63, 127 or 255.
  The 255 value enables the full ink colour mode and 
  all the the palette entries are inks with black paper.

(W) 0x43 (67) => Palette Control
  bits 7-2 = Reserved, must be 0
  bit 1 = Select the second palette
  bit 0 = Disable the standard Spectrum flash feature to enable the extra colours

(W) 0x44 (68) => Palette Lower bit
  bits 7-1 = Reserved, must be 0
  bit 0 = Set the lower blue bit colour for the current palette value


Without Palette Control bit 0 set.
Palette[0-15] = INK colours  					(0-7 normal, 8-15 =BRIGHT)
Palette[128-143] = Paper + Border colours		(0-7 normal, 8-15 =BRIGHT)


WITH Palette Control bit 0 set
Attribute byte swaps to paper/ink selection only. 
Palette Format specifies the number of colours INK will use. default is 15, so attribute if PPPPIIII
1  = PPPPPPPI
3  = PPPPPPII
7  = PPPPPIII
15 = PPPPIIII
31 = PPPIIIII
63 = PPIIIIII
127= PIIIIIII
255= IIIIIIII
Note if mode is 255, then Paper colour is 0 (in paper range, palette index 128)
Border colours always come from paper banks, palette index 128-135



Layer 2 access
======================================================================================
Register 18 = 	bank of Layer 2 front buffer
Register 19 = 	bank of Layer 2 back buffer 
Register 21 = 	sprite system.  
		bits 4,3,2 = layer order  
		000   S L U		 (sprites on top)
		001   L S U
		010   S U L
		011   L U S
		100   U S L
		101   U L S
Register $20	; Layer 2 transparency color working

port $123b
bit 0 = WRITE paging on. $0000-$3fff write access goes to selected Layer 2 page 
bit 1 = Layer 2 ON (visible)
bit 3 = Page in back buffer (reg 19)
bit 6/7= VRAM Banking selection (layer 2 uses 3 banks) (0,$40 and $c0 only)


Layer 2 xscroll
===================
ld      bc, $243B		; select the X scroll register
ld      a,22
out     (c),a
ld	a,<scrollvalue>		; 0 to 255
ld      bc, $253B
out     (c),a

Layer 2 yscroll
===================
ld      bc, $243B		; select the Y scroll register
ld      a,23
out     (c),a
ld	a,<scrollvalue>		; 0 to 191
ld      bc, $253B
out     (c),a

Layer 2: $E3	; bright magenta acts as transparent


Layer 2 window mode
====================
(W) 0x18 (24) => Clip Window Layer 2
  bits 7-0 = Cood. of the clip window
  1st write - X1 position
  2nd write - X2 position
  3rd write - Y1 position
  4rd write - Y2 position
  The values are 0,255,0,191 after a Reset

Sprite window mode
===================
(W) 0x19 (25) => Clip Window Sprites
  bits 7-0 = Cood. of the clip window
  1st write - X1 position
  2nd write - X2 position
  3rd write - Y1 position
  4rd write - Y2 position
  The values are 0,255,0,191 after a Reset
  Clip window on Sprites only work when the "over border bit" is disabled


Clip window resets
===================
(W) 0x1C (28) => Clip Window control
  bits 7-3 = Reserved, must be 0
  bit 2 - reset the ULA/LoRes clip index.
  bit 1 - reset the sprite clip index.
  bit 0 - reset the Layer 2 clip index.

Kempston mouse 
==============
Buttons  $fadf
Mouse X  $fddf    (0 to 255 continuous clocking)
Mouse Y  $ffdf    (0 to 255 continuous clocking) 


esxDOS simulation
===================
M_GETSETDRV	-	simulated
F_OPEN		-	simulated
F_READ		-	simulated
F_WRITE		-	simulated
F_CLOSE		-	simulated
F_SEEK      -   simulated
F_FSTAT     -   simulated
F_STAT      -   simulated


