---
layout: post
title:  "ZX Spectrum IDE — Part #3: A Brief Overview of the Z80 CPU"
date:   2018-01-23 13:20:00 +0100
categories: "ZX-Spectrum"
abstract: >- 
  The soul of the ZX Spectrum 48K microcomputer—what a surprise—is the Z80 CPU. Obviously, you need to emulate the CPU to get closer to a full Spectrum emulator. Believe it or not, CPU emulation is not a big challenge compared to other devices of the machine (video display generation, tape emulation, etc.), but it is laborious due to the richness of Z80’s instruction set.
---

As I mentioned in the previous post, the soul of the ZX Spectrum 48K microcomputer—what a surprise—is the Z80 CPU. Obviously, you need to emulate the CPU to get closer to a full Spectrum emulator. Believe it or not, CPU emulation is not a big challenge compared to other devices of the machine (video display generation, tape emulation, etc.), but it is laborious due to the richness of Z80’s instruction set.

Before going into the implementation details I used in [SpectNetIde](https://github.com/Dotneteer/spectnetide), I give you a brief overview of Z80A. Please, do not expect this article as a tutorial to learn about the CPU. I will focus on the aspects that are the most essential from the emulator design and development point of view.

The manufacturer of Z80A, Zilog, is an American manufacturer of microcontrollers. Its most known product is the Z80 family of chips. According to its simplicity from hardware interfacing point of view, it became popular right after its initial issue in 1976.

The Z80A version—the one used in ZX Spectrum 48K—is an improved model that increased the maximum speed of the original Z80 from 2.5 MHz to 4 MHz. The chip is an 8-bit CPU with 8-bit and 16-bit registers and provides over 1000 instructions.

Registers
As Figure 1 depicts, it has 8 main registers (__A__, __B__, __C__, __D__, __E__, __H__, __L__, and __F__) with their corresponding alternate registers (__A’__, __B’__, __C’__, __D’__, __E’__, __H’__, __L’__, and __F’__); two index registers (__IX__, __IY__); a stack pointer (__SP__); a program counter (__PC__), and two special registers, __I__ and __R__.

![f0201](/assets/images/zx-spectrum/f0201.png)

*__Figure 1__: The registers of the Z80A CPU*

The __A__ register is called _Accumulator_. From the programming point of view, this register is the most flexible, as it can be the operand of more instructions than any others. The __F__ register is a set of 1-bit flags that are individually set by Z80 instructions. The CPU can use the state of flags as conditions for instructions that change the program flow.

The main 8-bit registers can be used in 16-bit pairs. Thus __B__ and __C__ compose __BC__ (the 8 bits of __B__ are the upper, the bits of __C__ the lower parts of the resulting 16-bit register). Similarly, you have __DE__ and __HL__.

The alternate register set can serve as a backup for the main registers. A Z80 instruction, __EXX__, swaps the main and alternate pairs.

The __PC__ (_Program Counter_) points to the location the CPU should read the next byte of the operation code to execute. __SP__ stands for _Stack Pointer_—as its name suggests, it points to the top of the stack used for subroutine calls and stack-saved values.

__IX__ and __IY__ are index registers. You can use their contents as a memory pointer in tandem with an 8-bit displacement and read or write the composed address. For example, if __IX__ contains __$4000__, an instruction reading the __(IX+$2A)__ address—here, __$2A__ is the displacement—will obtain the contents of address __$402A__.

There are two special registers, __I__ and __R__. __I__ (_Interrupt Page Address_ register) is used in a particular interrupt mode as the high-order eight bits of the memory location that serves as the interrupt routine address—the device requesting the interrupt provides the low-order eight bits. __R__ (_Memory Refresh_ register) contains a memory refresh counter enabling dynamic memories to be used with the same ease as static memories.

### Control Signals

The CPU uses control signals to communicate with external devices. Vice-versa, it receives signals when the external world wants to notify the CPU about events. The Z80 has thirteen control pins, 8 of them send, five of them gets signals. From the emulator points of view, these three signals arriving at the CPU are essential:

* __INT__: interrupt request from devices. This request can be disabled (enabled) by with CPU instructions.
* __NMI__: Non-maskable interrupt request (cannot be enabled or disabled)
* __RESET__: Resets the CPU immediately just as if we turned the power off and then on again.
### Instructions

The Z80 CPU has more than 1000 instructions. The most often used ones have a single-byte operation code. There are less frequently used instructions that use two or three bytes of operation codes. Besides these, instructions may have arguments. Each instruction starts with the operation code. These codes explicitly tell the CPU whether the instruction has arguments, or its code has multiple bytes.

* The __$ED__ operation code is a prefix that means a second byte specifies the extended operation.
* __$CB__ is another prefix code. In this case the second byte names a bit manipulation instruction.
* __$DD__ and __$FD__ are prefixes for indexed operations. A second byte describes the instruction. __$DD__ means that the __IX__, __$FD__ implies that the __IY__ index register should be used in the instruction.
* When a __$CB__ prefix follows the __$DD__ and __$FD__ prefixes, a third byte names the bit manipulation instruction.
* When writing Z80 code, we use the Z80 Assembler language to define the instructions to execute. The compiler translates these very instructions to their machine code equivalent, to operation codes and arguments, respectively.

Let’s see a few examples:

* The __INC BC__ instruction has a single operation code, __$03__. (This instruction increments the 16-value of the __BC__ register pair with one.)
* The __LD A,$4C__ instruction is a standard operation (code __$3C__) and has an argument, __$4C__. The entire operation is these two bytes, in this order: __$3E__, __$4C__. (This instruction loads the 8-bit value __$4C__ into the Accumulator.)
* The __LD ($4238),A__ instruction is a standard operation (code __$32__) and has a 16-bit argument of __$4238__. The argument follows the operation code in LSB/MSB order (the least significant byte then the most significant one), so the entire operation contains these three bytes: __$32__, __$38__, __$42__. (Stores the value of __A__ in the __$4238__ memory address.)
* The __NEG__ instruction is an extended operation with the __$ED__ prefix followed by the __$44__ operation code, so it consists these two bytes: __$ED__, __$44__. (Calculates the two’s complement of __A__.)
* The __BIT 0,E__ instruction tests the leftmost bit of the __E__ register and sets status flags accordingly. It’s a bit manipulation operation (__$CB__ prefix) with the operation code of __$43__. So, the entire operation is composed of these bytes: __$CB__, __$43__.
* The __LD (IX+$3C),$87__ operation stores __$87__ in the memory address calculated from the current value of __IX__ plus the __$3C__ displacement. It starts with the __$DD__ prefix and the __$36__ operation code. The entire operation contains the __$3C__ displacement, and then __$87__ argument. So, altogether it has four bytes: __$DD__, __$36__, __$3C__, and __$87__.
* The __RLC (IY+$2F),C__ instruction starts with the __$FD__ prefix (it is __IY__-indexed), then goes on with the __$CB__ prefix (bit manipulation). There are two more bytes, __$01__, the operation code, and __$2F__, the displacement, respectively. The entire operation has these four bytes: __$FD__, __$CB__, __$01__, __$2F__.

{: class="note"}
__Note__: In this article, I do not intend to teach you Z80 instructions in details. If you’re interested in Z80 Assembler programming, you’ll find enough information. Here are two pages to start:
http://sgate.emt.bme.hu/patai/publications/z80guide/
http://z80-heaven.wikidot.com/system:tutorials

### Undocumented Instructions and Registers

The official Z80 documentation—I do not know why—omits hundreds of operations the Z80 can execute. Many of these are related to the higher and lower eight bits of the __IX__ and __IY__ index registers (named __XL__, __XH__, __YL__, and __YH__; or sometimes __IXL__, __IXH__, __IYL__, and __IYH__).

{: class="note"}
__Note__: Fortunately, you can find reliable documents on the internet, which give you those missing details. You need to know that many ZX Spectrum games utilize these undocumented instructions, so a high-fidelity emulator must implement them—this it what SpectNetIde does, too.

Figure 2, 3, 4, 5, and 6 show the entire instruction set of Z80. The reddish cells are the initially undocumented instructions of the CPU. Please note, Figure 5 and Figure 6 display the __IX__-indexed instructions. You can use the same instructions with the __$FD__ prefix for the __IY__ register.

![f0202](/assets/images/zx-spectrum/f0202.png)

*__Figure 2__: Z80 standard instruction set (source: clrhome.org)*

![f0203](/assets/images/zx-spectrum/f0203.png)

*__Figure 3__: Z80 extended instruction set (source: clrhome.org)*

![f0204](/assets/images/zx-spectrum/f0204.png)

*__Figure 4__: Z80 bit manipulation instruction (source: clrhome.org)*

![f0205](/assets/images/zx-spectrum/f0205.png)

*__Figure 5__: Z80 indexed (IX) instructions (source: clrhome.org)*

![f0206](/assets/images/zx-spectrum/f0206.png)

*__Figure 6__: Z80 indexed (IX) bit manipulation operations (source: clrhome.org)*

### Timings

If you are about to write a ZX Spectrum emulator—or any computer emulator—you soon learn that taking care of timing is probably the most important thing. Without this, you won’t be able to create a high-fidelity emulation of real hardware.

{: class="note"}
__Note__: In the future articles in this series I will treat particular aspects of timings in almost every post.

The Z80 CPU executes instructions as a series of subsequent machine cycles. To understand how it works, Figure 7 gives you the detailed timing of the __INC (HL)__ instruction. __INC (HL)__ increments the value stored at the memory address pointed by the __HL__ register pair.

As the figure shows, the CPU executes this instruction in four machine cycles:

* __M1__: The CPU reads the opcode from the memory address pointed by __PC__ (_Program Counter_). The execution logic understands what this instruction means, and how to process it.
* __M2__: The CPU reads the contents of the memory address pointed by __HL__ into some internal ALU register to be ready to process it.
* __M3__: The CPU increments the value of the internal ALU register
* __M4__: The CPU writes back the incremented value to the memory address pointed by __HL__.

![f0207](/assets/images/zx-spectrum/f0207.png)

*__Figure 7__: Timing diagram of the INC (HL) instruction*

Well, this concise description of the __M1...M4__ cycles did not mention many subtle operation details. The real execution is more complicated. The diagram shows that the machine cycles utilize more than a single clock pulse (_T-cycle_) to carry out their tasks. The longest is __M1__ with four T-cycles. Reading and writing memory takes three T-cycles, respectively. The fastest step is the increment, it consumes a single clock cycle.

The __M1__ cycle is a special one, so I’d like to add some more details on it:

Every instruction starts with fetching the operation code. If the code is prefixed (such as in case of extended, indexed, and bit manipulation instructions), every opcode fetch is similar. The __M1__ cycle takes for T-cycles, __T1__, __T2__, __T3__, and __T4__. This is what happens during M1:

* As __T1__ begins, the CPU puts the contents of __PC__ to the address bus. About a half T-cycle later, the CPU sign the __MREQ__ (_Memory Request_) control signal in tandem with __RD__ (_Read_).
* In __T2__, the memory responds by placing the content of the memory addressed by __PC__ to the data bus. (By this time, the memory address stabilizes on the address bus.)
* Just as __T3__ begins, with the rising edge of the clock signal, the CPU reads the contents of the data bus—the opcode gets into one of the internal registers. The CPU revokes the __RD__, and __MREQ__ signals and puts lower seven bit of __R__ (_Refresh Page Register_) to the lower seven lines of the address bus. At the same time, the CPU places the contents of __I__ (_Interrupt Register_), to the highest eight lines of the address bus, and signs __RFSH__ (_Refresh_ signal).
* During __T3__ and __T4__, when the refresh page address is stabilized on the address bus, the CPU raises the __MREQ__ signal again. The combination of __MREQ__ and __RFSH__ allows the DRAM chips to refresh the memory contents of the addressed page.
* By the end of __T3__, the CPU analyzes the opcode read and prepares for the subsequent machine cycles. Many operations, are simple and do not need any further memory or I/O access. The CPU executes these operations during __T4__.
* By the end of __T4__, __MREQ__, __RD__ (and __RFSH__, a little bit later) go back to their inactive state. The CPU increments the last seven bits of R while keeping its most significant bit.

As you can see, the __M1__ machine cycle is significant, it is responsible for refreshing the DRAM memory. Without this periodic refresh cycle, the memory would forget its content. Just to have an idea about this time, every page should be refreshed in every 64 milliseconds or less.

To let peripherals and other devices know that the CPU executes __M1__, Z80 has a system control signal, __M1__, that goes active during __T1__ and __T2__.

It may happen that during memory and I/O operations the CPU needs to wait while the memory or a device gets ready for a data transfer. The CPU has a __WAIT__ input signal; devices may use it to sign that they are not prepared to let the CPU carry on the read or write operation.

{: class="note"}
__Note__: Later, in another post, you will learn that the __$4000–$7FFF__ range of memory in ZX Spectrum 48K is contended. Sometimes, when the CPU wants to read or write the memory, it’s forced to __WAIT__, as the ULA has priority to keep the electron ray in the cathode tub uninterrupted.

Memory read and write, I/O read and write machine cycles have their detailed timings—similarly to __M1__. Here I won’t detail them. If you are interested, check the official Zilog Z80 documentation here.

### Interrupts

The CPU cannot continuously poll devices whether they have something to tell. Devices can notify the CPU by generating an interrupt signal. Z80 receives that signal and suspends its normal execution. In response, it executes a routine, the interrupt routine. As that routine is completed, the CPU goes back and continues executing instructions right from the point it was before receiving the signal.

Z80 can handle two kinds of interrupts through the __INT__ and __NMI__ signals. __NMI__ stands for Non-Maskable Interrupt. When the CPU receives an __NMI__ request, it executes the interrupt routine starting at address __$0066__.

__INT__ raises a maskable interrupt. Maskable means that you can disable (and re-enable) it from software—with the __DI__ (_Disable interrupt_), and __EI__ (_Enable interrupt_) Z80 instructions, respectively.

Z80 offers three interrupt handling modes. You can activate these with the __IM 0__, __IM 1__, and __IM 2__ instructions:

* __IM 0__: In this mode the interrupting device can force the CPU to execute a single machine operation. The device places the opcode to the address bus to let the CPU read it in. If the operation contains more bytes, the device should take care to provide those bytes according to the normal memory read timing sequence. No ZX Spectrum models use this interrupt mode.
* __IM 1__: This is the simplest interrupt mode. When the CPU receives the request, it starts the interrupt routine at address __$0038__. By default, all ZX Spectrum models—I mean, their operating system—uses this mode.
* __IM 2__: This mode is the most complex, often called vectored interrupt, for it allows an indirect call to any memory location by an 8-bit vector supplied from the peripheral device. This vector then becomes the least significant eight bits of the indirect pointer, while __I__ (_Interrupt Register_) in the CPU provides the most significant eight bits. This address points to an address in a vector table that is the starting address for the interrupt service routine. ZX Spectrum games often use this mode to change the original interrupt handler routine entirely.
Of course, the CPU cannot stash its current operation at the very moment an interrupt request arrives. Z80 defines an interrupt request/acknowledge timing cycle:

The CPU samples the __INT__ signal with the rising edge of the final clock at the end of any instruction. The signal is not accepted unless the maskable interrupt is enabled. When the signal is accepted, the CPU generates a special __M1__ cycle. During this cycle, the __IORQ__ signal becomes active (instead of the normal __MREQ__) to indicate that the interrupting device can place an 8-bit vector on the data bus. Two wait states are automatically added to this cycle. These states are added so that a ripple priority interrupt scheme can be easily implemented. The two wait states allow sufficient time for the ripple signals to stabilize and identify which I/O device must insert the response vector.

### Next: Emulating the Z80A CPU

By now, you know enough information about the Z80 CPU. You are prepared to understand the operation of the Z80 emulator. In the next posts of this series, you will learn about the concepts, design, implementation, and testing details.
