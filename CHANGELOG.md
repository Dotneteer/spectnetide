### Version under development

__FIX__: The Z80 assembler did not accepted the `ld (ix+MySymbol+2),h` expression, you had to write 
`ld (ix+[MySymbol+2]),h`. Now, the assembler does not have this restriction, so the first instruction also correct.  
__FEATURE__: The Z80 assembler now accepts `(` and `)` as expression delimiters, not only `[` and `]`.  
__FIX__: The Z80 assembler took the `ld sp,de` and `ld sp,bc` instructions into account as if they were 
`ld sp,hl`. Now, the assembler recognizes that they are invalid, and raises an error message.  

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
