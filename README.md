# Introduction

This project implements a ZX Spectrum integrated development environment (IDE) that is integrated into Visual Studio 2017.
Originally, I intended this project to be just a demo project so that I can use it as illustration in my agile 
software design and testing courses, but it became a fun project.

At the moment it is entirely written in C#, but I plan to implement certain parts in C++ (somewhen in the future, for 
the sake of performance), and maybe in JavaScript, too.

Right now, the project implements a ZX Spectrum 48K Emulator (other Spectrum emulators will be added in the future), 
and probably is less mature than most of the ZX Spectrum emulators with longer past.

Nonetheless, this project has some unique features:

1. It is very well commented and harnessed with unit tests. I plan to add a lot of documentation that help you
understand how to design and develop such an emulator.
2. Although the project is only in alpha phase, it adds a number of useful features integrated into the VS 2017 IDE that 
you can develop for understanding ZX Spectrum applications and games, discover their code, and develop new ZX Spectrum apps. 
Let me list a you a few of them:
    * __ZX Spectrum Emulator tool window__ in the IDE
    * __ZX Spectrum Code Discovery project type__ to keep together the ZX Spectrum ROM, your annotations, .TZX and .TAP files that
    you use to analyze apps and games.
    * __Disassembly view__
    * __Register and Memory views__
    * __BASIC List view__ to display the BASIC program list loaded into the memory
    * __TZX Explorer__ to analyze TZX files, even peek into the BASIC programs they contain, or disassembly their code.
    * __Debugging the code_ with *breakpoints, Run/Pause/Step-Into/Step/Over* commands
3. You can add __annotations to the ROM and the code in the RAM__ you're analyzing. The disassembler is aware of Spectrum-specific features such as __RST 08__ and __RST 28__ calls, and understands their special byte code.
4. You can define detailed memory map for an app to designate parts that should be dispaled az Z80 disassembly, __.defb__ or __defw__
directives. 

# Screenshots

Here are a few screenshots with brief explanations to let you get a bit more rundown about this project.

# Future plans

I do not want to stop here, and plan a number of exciting features:

* More ZX Spectrum emulators: ZX Spectrum 128, ZX Spectrum Next
* ZX Spectrum Application project type to develop ZX Spectrum apps in Z80 assembly
* Compiler for a higher level language
* Integrated development tools for Zx Spectrum Next features (for example sprites with editors, etc).

# Contribution

You can contribute in the project. Please contact me here: dotneteer@hotmail.com




