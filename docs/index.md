---
layout: default
selector: home
---

<div class="ui vertical stripe segment with-content-back as-first">
  <div class="ui container">
    <div class="ui grid">
      <div class="six wide computer column five wide tablet sixteen wide mobile column">
        <h2>Essential Tools</h2>
        <p>
          SpectNet IDE encapsulates the tools you need to develop Z80 Programs
          for the ZX Spectrum 48K, 128+, and +3E. It integrates with Visual
          Studio 2017 and 2019 <em>(soon)</em> IDE, including the free Community editions.
        </p>
        <br />
        <p><a target="_blank" href='https://marketplace.visualstudio.com/items?itemName=Dotneteer.SpectNetIde'>Download SpectNet IDE</a></p>
        <p><a target="_blank" href='https://visualstudio.microsoft.com/free-developer-offers/'>Get VS 2017 Community</a></p>
      </div>
      <div class="ten wide computer column eleven wide tablet sixteen wide mobile column">
        <img src='./assets/images/features/feature1.png'/>
      </div>
    </div>
  </div>
</div>

<div class="ui vertical stripe segment with-content-back even">
  <div class="ui container">
    <div class="ui grid">
      <div class="six wide computer column five wide tablet sixteen wide mobile column">
        <h2>ZX Spectrum Project Types</h2>
        <p>
          In Visual Studio, you can create ZX Spectrum projects. SpectNetIde provides two
          project types out of the box.
        </p>
        <p>
          You can use the ZX Spectrum Code Discovery Project to analyze, examine, and debug
          the Z80 code of the ROM, games, and other applications. Also, you can create your
          own Z80 assembly programs with the built-in assembler.
        </p>
        <p>
          The ZX Scripting Console App allows you to write C# applications to create scripts
          that control the ZX Spectrum virtual machine. This project type can be used to
          understand how games run (e.g. equip them with "eternal life"), and automate tasks
          that help you peep into the code that could not be solved otherwise.
        </p>
      </div>
      <div class="ten wide computer column eleven wide tablet sixteen wide mobile column">
        <img src='./assets/images/features/feature5.png'/>
        <img src='./assets/images/features/feature5a.png'/>
      </div>
    </div>
  </div>
</div>

<div class="ui vertical stripe segment with-content-back">
  <div class="ui container">
    <div class="ui grid">
      <div class="six wide computer column five wide tablet sixteen wide mobile column">
        <h2>ZX Spectrum Emulator</h2>
        <p>
          The built-in ZX Spectrum Emulator allows you to run a virtual machine
          just like the original hardware. It not only ensures high-fidelity
          operation of the selected ZX Spectrum model; you can pause the machine
          any time to start debugging.
        </p>
        <p>
          The emulator allows you to save and restore the paused state of the
          virtual machine, moreover, you can store these files as a part of your project.
        </p>
      </div>
      <div class="ten wide computer column eleven wide tablet sixteen wide mobile column">
        <img src='./assets/images/features/feature2.png'/>
      </div>
    </div>
  </div>
</div>

<div class="ui vertical stripe segment with-content-back even">
  <div class="ui container">
    <div class="ui grid">
      <div class="six wide computer column five wide tablet sixteen wide mobile column">
        <h2>Z80 Disassembly</h2>
        <p>
          The built-in Z80 Disassembler has many unique features that only SpectNetIDE provides.
          It not only turns binary code into Z80 disassembly, but it allows you to annotate the code.
        </p>
        <p>
          You can define labels and symbols as well as tail or prefix comments. To separate code and data,
          you can mark segments of the binary as a byte or a word array or a memory area to ignore.
        </p>
        <p>
          The SpectNetIDE disassembler understands ZX Spectrum specific code, such as the byte code
          following the RST $08 and RST $28 instructions, and generates the output accordingly.
        </p>
        <p>
          The disassembler understands the extended Z80 instruction set of ZX Spectrum Next, and &mdash; of
          course &mdash; handles all undocumented Z80 operations. The IDE comes with annotated ROMs for each
          model, including ZX Spectrum 48K, +128, and +3E.
        </p>
      </div>
      <div class="ten wide computer column eleven wide tablet sixteen wide mobile column">
        <img src='./assets/images/features/feature3.png'/>
        <img src='./assets/images/features/feature4.png'/>
      </div>
    </div>
  </div>
</div>

<div class="ui vertical stripe segment with-content-back">
  <div class="ui container">
    <div class="ui grid">
      <div class="six wide computer column five wide tablet sixteen wide mobile column">
        <h2>Debugging tools</h2>
        <p>
          Creating and analyzing code is in the heart of SpectNetIDE. The environment provides many
          tools to examine the running code, the state of the CPU, the ULA, and the memory.
        </p>
        <p>
          When stepping through code, you can immediately get information about the number of T-cycles
          spent between to stops, as well as the clock cycles wasted for contended memory access.
        </p>
        <p>
          With the help of the Disassembly tool window, you can set up simple breakpoints, and
          even define conditions to pause the execution flow.
        </p>
        <p>
          The Memory tool window allows you to have a real-time view of the memory, highlighting
          the addresses pointed by one of the 16-bit Z80 register pairs.
        </p>
        <p>
          To examine the running code, you can define and show watch expressions that combine the
          state of registers, flags, and memory locations into display values that tell you what
          the program does.
        </p>
      </div>
      <div class="ten wide computer column eleven wide tablet sixteen wide mobile column">
        <img src='./assets/images/features/feature8.png'/>
        <img src='./assets/images/features/feature6.png'/>
        <img src='./assets/images/features/feature7.png'/>
      </div>
    </div>
  </div>
</div>

<div class="ui vertical stripe segment with-content-back even">
  <div class="ui container">
    <div class="ui grid">
      <div class="six wide computer column five wide tablet sixteen wide mobile column">
        <h2>Full-blown Z80 assembly programming</h2>
        <p>
          SpectNetIde provides you a robust Z80 assembler with many useful features.
        </p>
        <ul>
          <li>Syntax-highlighted editor</li>
          <li>Flexible syntax</li>
          <li>String escape sequences for ZX Spectrum-specific characters</li>
          <li>Loops and conditional statements</li>
          <li>Powerful dynamic macros</li>
        </ul>
        <p>
            You can set up breakpoint in the source code. When the execution flow
            reaches one of them, the corresponding source code is displayed. The Debug,
            Pause, Step-Into, Step-Over, and Step-Out commands are available with source
            code, too.
        </p>
        <p>
            You can export the Z80 assembly code into .TZX and .TAP code files that can
            be immediately LOADed into ZX Spectrum — with optional auto start support.
        </p>
      </div>
      <div class="ten wide computer column eleven wide tablet sixteen wide mobile column">
        <img src='./assets/images/features/feature10.png'/>
        <img src='./assets/images/features/feature10a.png'/>
      </div>
    </div>
  </div>
</div>

<div class="ui vertical stripe segment with-content-back">
  <div class="ui container">
    <div class="ui grid">
      <div class="sixteen wide column">
        <h2>... And many others</h2>
        <ul>
          <li>Two types of ZX Spectrum keyboards</li>
          <li>Tape File Explorer</li>
          <li>Call Stack Tool Window</li>
          <li>Unit Tests for Z80 Code</li>
          <li>ZX Spectrum BASIC List</li>
          <li>Virtual floppy devices and disks for ZX Spectrum +3E</li>
        </ul>
      </div>
    </div>
  </div>
</div>
