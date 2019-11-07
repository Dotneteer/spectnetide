### Version 2.0.2 (Preview 1)

__FIX__: ZX BASIC setup instructions added. In case of error, the IDE opens the help page for you.

### Version 1.20.0
__FEATURE__: The virtual machine now supports Kempston joystick emulation.

### Version 1.19.5
__FIX__: Bug in code generation for IX, IY bit instructions fixed.

### Version 1.19.4
__FIX__: Virtual machine injection startup phase bug fixed.

### Version 1.19.3
__FIX__: In a few cases the emulator did not remove the breakpoint when you erased
that in the code window. Now, it works correctly.  
__FIX__: The Code.z80asm code file in a newly created project missed the `.model Spectrum48` pragma, 
and so it did not run on other architectures than ZX Spectrum 48K.

### Version 1.19.2
__FIX__: The resource issues that prevented SpectNetIDE to load in VS 2019 16.1.2 resolved.

### Version 1.19.1
__FIX__: Typing binary literals starting with "0b" in the code editor does not raise an exception.

### Version 1.19.0
__FEATURE__: Now, you can create assembly output listing file when comiling th code.
__CHANGE__: SpectNetIDE uses the new VS SDK metapackages.

### Version 1.18.5
__CHANGE__: The standalone emulator client has been deleted from the solution.

### Version 1.18.4
__FEATURE__: The ZX Spectrum memory window allows exporting the memory contents.  
__FEATURE__: Z80 Assembler: new `.comparebin` pragma.  
__CHANGE__: The Z80 Disassembly window uses `X` commands (instead of `XD`) for exporting disassembly  
__FIX__: Character literals use syntax highlighting (just like strings).    
__CHANGE__: The SpectNetIDE package now uses the `PackageAutoLoadFlags.BackgroundLoad` flag.

### Version 1.18.3
__FEATURE__: You can toggle the the breakpoint in the Z80 Disassembly tool window by clicking on an item.  
__FEATURE__: The Z80 Disassembly window allows exporting the disassembly.

### Version 1.18.2
__FEATURE__: __SpectNetIDE__ now can be installed on Visual Studio 2019 Preview 4.1.  
__CHANGE__: Old reference documentation has been moved to the new documentation hub at https://dotneteer.github.io/spectnetide/

### Version 1.18.1
__FEATURE__: The Z80 Assembler has a two new statements, `.ifused`, and `.ifnused`, respectively.


### Version 1.18.0
__FEATURE__: Z80 Assembler supports the `.xorg` pragma to set the segment start address for Intel HEX export.  
__FEATURE__: The __Export Z80 Program__ supports the Intel HEX format.  
__FEATURE__: The Z80 Disassembly tool window now can disassembly to `.DEFG` pragma with the `MG`, `MG1`, 
`MG2`, `MG3`, and `MG4` commands.  
__FEATURE__: The Z80 Assembler handles structure definitions and initializations.  
__FIX__: Retrieve annotation commands are documented.  
__FIX__: The command parser now understands extra identifier characters, such as `!`, `?`, `@`, and `#`.  
__CHANGE__: Tool window command documentation has been moved to the project's GitHub pages, links has been
updated accordingly.  


### Version 1.17.0
__FEATURE__: Z80 Assembler &mdash; Now you can define binary literals with the __`b`__ or __`B`__ suffix.  
__FEATURE__: Z80 Assembler &mdash; Now you can define octal literals with the __`q`__, __`Q`__, __`o`__,
or __`O`__ (letter, and not zero) suffix.  
__FEATURE__: Z80 Assembler &mdash; Now, block comments (__`/*`__ ... __`*/`__) are supported.  
__FEATURE__: Z80 Assembler &mdash; The set of characters that can be used in identifiers extended.  
__FEATURE__: Z80 Assembler &mdash; New pragma, __`DEFC`__.
__FEATURE__: Z80 Assembler now supports modules/scopes.

### Version 1.16.0
__FIX__: Wrong ZX Spectrum keyboard labels fixed.  
__CHANGE__: The default `Code.z80asm` template now displays "ZX Spectrum IDE" text.  
__FIX__: The __`G`__ disassembly view command finds symbols pointing to the RAM.
__FEATURE__: Now all commands (ZX Spectrum Memory, Z80 Disassembly, and Watch Memory) 
accept decimal numbers and symbols (wherever reasonable).  
__FEATURE__: ZX Spectrum Memory view now marks symbol and annotations links; displays decimal 
value and symbol information.  
__BREAK__: The command syntax of the Z80 Disassembly and Watch Memory tool windows has been
slightly changed.


### Version 1.15.0
__FEATURE__: You can add conditional breakpoints in the Disassembly tool window and in the source code.  
__FIX__: New colors of the set and current breakpoint highlights provide better visibility and distinction.

### Version 1.14.0
__FEATURE__: Now, you can use the brand new Watch Memory tool window to display values
while you run or debug your program.  
__FIX__: When you click reference pages in Z80 Disassembly, ZX Spectrum Memory, or Watch 
Memory tool windows, the help pages are displayed in the default browser.  
__FEATURE__: The tool windows display the "(Debugging)" tag in the caption if the ZX Spectrum
VM runs is debug mode.  
__FEATURE__: The ZX Spectrum Memory window provides a tooltip when you move the mouse over a
memory byte. It highlights memory addresses pointed by `BC`, `DE`, `HL`, `IX`, `IY`, `SP`, and `PC`.
In the __Tools | Options__ dialog (*Spect.Net IDE* tab) you can define highlight colors.  
__FEATURE__: The debug engine now supports step-out. While you are debugging, step-out completes 
the current subroutine and steps back to the next instruction right after the subroutine call.

### Version 1.13.1
__FIX__: ZX Spectrum Code Discovery project commands does not show up in Solution Explorer anymore
if you work with a non-ZX-Spectrum project.  
__FIX__: When you cancel creating a ZX Spectrun Code Discovery project, it does not create an empty
project folder.  
__FEATURE__: When you run or debug a Z80 program, the ZX Spectrum Memory and Z80 Disassembly 
tool windows automatically position to the entry address of the code.  
__FIX__: Now, you can use the F5, F10, and F11 function keys seamlessly to debug your Z80 code.
Whenever a breakpoint is reached, the focus shifts back to the ZX Spectrum emulator window.
__FIX__: To provide a beeter UX for running and debugging Z80 programs, by default the
"Confirm code injection", "Confirm code start", and "Confirm machine options" are turned off.
You can use these double-key shortcuts:
* __Run Z80 Program__: `Ctrl+M`, `Ctrl+R`
* __Debug Z80 Program__: `Ctrl+M`, `R`
* __Stop the virtual machine__: `Ctrl+M`, `Ctrl+S`


### Version 1.13.0
__BREAK__: The `DEFG` pragma uses a string pattern and not a string expression any more.  
__FEATURE__: The new `DEFGX` pragma overtakes the role of the former `DEFG` pragma. This change
was done to be compatible with other Z80 Assemblers.  
__FEATURE__: Now, you can use the `*` token just like `$` and `.` to obtain the current
assembly address.  
__FEATURE__: New built-in functions support screen and color handling: `scraddr()`, `attraddr()`, 
ink(), `paper()`, `bright()`, `flash()`, and `attr()`.  
__FIX__: Documentation of macros now contains explicit statement that macros cannot be used as user-defined functions.  

### Version 1.12.0
__FEATURE__: The Z80 assembler supports the `PROC`..`ENDP` statement to create local labels.  
__FEATURE__: You can use new pragmas: `DEFH` (emit bytes from a string with hexadecimal digits),
`DEFN` (emits zero-terminated strings).  
__FEATURE__: The `DEFS` pragma now supports new aliases: `.ds`, `ds`, `.DS`, and  `DS`.  
__FEATURE__: The `Z0201` error message displays the name of symbols that cannot be resolved.  
__FIX__: When you close the current project, the IDE clears the Error List.

### Version 1.11.0
__FEATURE__: Now, yo can specify a LOAD screen when exporting Z80 programs, and
you can set an option for "PAUSE 0" statement before running tho code. You can also
specify a BORDER color.  
__FIX__: ZX Spectrum Emulator id displayed to fit into the window.

### Version 1.10.3
__FIX__: Rebuild with the latest source to eliminate issue #76, and #77
(Windows 7 with non-English display language)

### Version 1.10.2
__FIX__: Issue #75 fixed: The virtual machine now handles Bit 6 of 
port `$FE` properly for ULA Issue 2/3 configurations.

### Version 1.10.1

__FIX__: Issue #73 fixed: Now, no files are installed outside of the
extension path.

### Version 1.10.0

__FEATURE__: The SpectNetIde VS Package now loads asynchronously.  
__FEATURE__: You can raise a custom error with the `.error` pragma.  
__FEATURE__: Now, you can use the `str()`, `lowercase()` and `uppercase()` functions.  
__FEATURE__: You can include binary files with the `.includebin` pragma.  
__FEATURE__: New parse-time functions are available: `isreg8()`, `isreg8std()`, 
`isreg8spec()`, `isreg8idx()`, `isreg16()`, `isreg16std()`,`isreg16idx()`, 
`isregindirect()`, `iscport()`, `isindexedaddr()`, `isexpr()`, and `iscondition()`.

### Version 1.9.0

__FEATURE:__ Floppy drive emulation added to Spectrum +3e.
* Drive A: and drive B: supported (0, 1, or 2 floppy configurations)
* You can choose the configuration when creating a new project
* All four floppy disk types supported by Spectrum +3 (Spectrum +3, CPC Data, CPC System, Amstrad PCW)
can be created, inserted, or ejected.
* Disks can be marked as write protected
* The contents of the disks are stored in `.vfdd` files that are now part of
the ZX Spectrum project
* `.vfdd` files have an editor that displays the format information of the virtual disk

### Version 1.8.0

__FEATURE__: Macros are now supported.  
__FEATURE__: The Z80 assembler now supports the `FOR`..`NEXT` loop.  
__FEATURE__: Unary logical NOT operator (`!`) added to the language.  
__FEATURE__: The `&` operator can be used with two string values. The result is the concatenation
of the two strings with a `\r\n` (next line) character pair inserted between them.  
__FEATURE__: You can exit loops with `.break`, or carry on to the next iteration with `.continue`.  
__FEATURE__: You can define syntax hightlighting for instruction operands (registers, conditions, etc.),
and semi-variable literals ($, $cnt, etc.)  
__FEATURE__: The assembler now supports *dynamic macros*, a non-preprocessor way of declaring and applying
macros in the code.  
__FEATURE__: The `lreg()` and `hreg()` parse-time functions can be applied an 16-bit standard registers and 16-bit
index registers.  
__FEATURE__: The `texof()` operator retrieves the compile-time text of a mnemonic, register, or condition.  
__FEATURE__: New comparison operators, `===` and `!==` now support case-insensitive string comparison.  
__FIX__: The `.defb` and `.defw` pragma accepted string values. Now, they do not.  


### Version 1.7.0

__FEATURE__: The Z80 assembler supports the `IF`..`ELIF`..`ELSE`..`ENDIF` statement.  
__FEATURE__: The Z80 assembler now supports loop statements: `LOOP`, `REPEAT`..`UNTIL`
`WHILE`..`WEND`.  
__FIX__: The Z80 assembler accepted the invalid `sbc ix,de` and `sbc iy,de` operations without an error
message &mdash; and compiled `add ix,de` and `add iy,de` operations. Now, it raises an error message.  
__FIX__: The Z80 assembler did not accepted the `ld (ix+MySymbol+2),h` expression, you had to write 
`ld (ix+[MySymbol+2]),h`. Now, the assembler does not have this restriction, so the first instruction 
is also correct.  
__FEATURE__: The Z80 assembler now accepts `(` and `)` as expression delimiters, not only `[` and `]`.  
__FIX__: The Z80 assembler took the `ld sp,de` and `ld sp,bc` instructions into account as if they were 
`ld sp,hl`. Now, the assembler recognizes that they are invalid, and raises an error message.  
__FIX__: The lists in the *Symbols* and *Fixups* tabs of the Z80 Assembler Output window  

### Version 1.6.0

__FEATURE__: The Z80 Assembler now allows using floating point numbers and functions.  
__FEATURE__: New pragmas are supported: `ALIGN`, `TRACE`, `TRACEHEX`, `DEFG`  
__FEATURE__: `VAR` pragma supports two new alternate token: `=` and `:=`  
__FEATURE__: The current address is now accessible not only with `$` but with `.`, too  

### Version 1.5.0

__FEATURE__: The ZX Spectrum emulator now supports the +2A/3 "floating bus" feature (http://sky.relative-path.com/zx/floating_bus.html)  
__FEATURE__: The Z80 Registers tool window contains new counters:
* `STP`: The number of T-cycles spent since the last pause
* `DEL`: The accumulated contention delays since the machine started
* `LCO`: The accumulated contention delays since the machine was last paused
* `CON`: Contention delay value of the current screen rendering frame

__FEATURE__: ZX Spectrum I/O port logging can be turned on/off through a SpectNetIde option
 
### Version 1.4.0

__FEATURE__: Now, the assembler recognises hexadecimal-like identifiers as identifiers.
When using the `h` or `H` suffix, hexedecimal literals should start with a decimal digit.  
__FEATURE__: The ZX Spectrum emulator now supports the ULA "floating bus" feature (http://ramsoft.bbk.org.omegahg.com/floatingbus.html)

### Version 1.3.0

__FEATURE__: The first version of ZX Spectrum scripting object model is added to the project.  
__CHANGE__: When saving the virtual machine state of a ZX Spectrum 48K model, now, the memory
image is compressed.  
__WARNING__: When you work with a ZX Spectrum 48K state file from an older __SpectNetIde__ version,
that may cause issues.  
__WORKAROUND__: Delete the entire `.SpectNetIde` folder under you solution
folder.

### Version 1.2.1

__FIX__: Now, Newtonsoft.Json assembly is embedded into the VSOX package. Without this, SpectNetIde
caused any Visual Studio solution to crash the IDE during load time.
### Version 1.2.0

__This is the first version with tracked changes__

__FEATURE__: Code injection logic has been changed:
  * When the Spectrum VM is stopped, the IDE gives a warning to pause it.
  * When the VM is paused, the IDE injects the code to the VM.
  * When the VM is stopped, the IDE starts the VM, pauses it, and then injects the code.

__FEATURE__: The Disassembly tool window has a new command, JUMP (__`J`__ _`hhhh`_). 
This command works only when the Spectrum VM is paused. It sets the Program Counter (PC) to the
specified _`hhhh`_ address. When the Spectum VM continues running &mdash; after the Start command &mdash;
it will carry on from the specified address.
 
__FEATURE__: Now, you can use the __Add VM State to project__ command in the Spectrum emulator tool window
toolbar to save the Spectrum VM state directly into the project hierarchy.

__FEATURE__: You can execute the __Load VM State file__ command from the context menu of a `.vmstate` file in Solution Explorer.
