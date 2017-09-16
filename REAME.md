## Modifying the Z80 Assembler grammar

The __`Assembler`__ folder contains two Visual Studio Projects:
* __AntlrZ80AsmParserGenerator__: A Visual Studio 2015 project with a custom
tool that generates C# lexer, parser, and visitor classes from the formal
Z80 Assembly grammar. This project is not a part of the __Spect.Net__ solution.
* __Spect.Net.Assembler__: A Visual Studio 2017 project that references and utilizes
__AntlrZ80AsmParserGenerator__ to create the assembler.

When you need to modify the Z80 Assembly grammar, you have to do it with
the first project. It runs only with Visual Studio 2015, as the custom tool
that generates Antlr artifacts is not available for VS 2017 (at least, it was
not as of writing this memo).

When you modify the grammar, you need to build the __AntlrZ80AsmParserGenerator__
project. It applies an after build event that copies the generated artifacts right
into the __Generated__ folder of the __Spect.Net.Assembler__ project.

You can immediately rebuild the assembler with the modified grammar.

### Notes for the AnlrZ80AsmParserGenerator project

To use this project in Visual Studio 2015, you need to install the *ANTLR
Language Support* VS extension (by Sam Harwell). The project also needs the following 
NuGet packages to be able to generate the Antlr C# target files:
* __Antlr4__
* __Antlt4.CodeGenerator__
* __Antlr4.Runtime__

The grammar definition is in the __Z80Asm.g4__ file.



