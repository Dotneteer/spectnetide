---
layout: documents
categories: 
  - "Z80 Assembler"
title:  "Assembler Output"
alias: assembler-output
seqno: 120
selector: documents
permalink: "documents/assembler-output"
---

## Assembler listing output file

When you need to take a look at the output the __SpectNetIDE__ Z80 Assembler generates, you can turn on the generation of assembler listing output file. Use the __Spect.Net IDE__ tab in the __Tools\|Options__ dialog:

![Listing Output]({{ site.baseurl }}/assets/images/z80-assembler/listing-output.png)

Set the _Generate listing output file_ to __True__ to create the listing file when compiling a Z80 code file. The Assembler will generate this file only when your code does not contain any error.

By default, only the Compile command generates the listing output. However, setting the _Generate listing file only for Compile_ to __False__ turns on listing file generation for the Run, Debug, Inject, and Export commands, too.

### Listing file name and location

The listing output file goes to the folder you specify with the _Listing file save folder_ option. Should you leave it empty, the list files are saved into the `.SpectNetIde/Listing` folder within your project folder. The listing files get the name of the source file you compile with the extension set in the _Listing file extension_ option. However, you can add a date-based suffix to the file name with specifying a name suffix pattern. For example, the pattern in the figure above will add minute-precision date information to the file name. If your source file is named `Code.z80asm`, this setting will result in a file name like this: `Code_2019-05-19-08-29.out`.

The IDE will add the listing output file to your current project structure if you turn on the _Add listing file to project_ option. By default, the IDE uses the `Listings` subfolder, however, you can specify a different one in with the _Listing file project folder_ option.

### Listing file format

The listing file contains a separate line for each source code line that contains a pragma, or an instruction that emits machine code. The file does not contain any information about directives, macro or struct definitions, and statements.

To influence the format of an output line, you can specify a template with the _Listing file line template_ option. This option is a string that contains placeholders. The placeholders are replaced with their current values.
- `{A}`: the instruction address, formatted as four hexadecimal digits.
- `{C}`: the operation codes emitted for the source code line, formatted as two hexadecimal digits for each byte, separated by a space character.
- `{CX}`: like `{C}`, but padded with spaces if there are less than four bytes emitted.
- `{F}`: Zero-based file index. The source files (the root source file and files included during the compilation) are counted. The root file has an index of zero, the include file loaded first has an index of one, and so on.
- `{F2}`: Like `{F}`, but the field width is two characters, aligned to the right, padded with spaces from left.
- `{F3}`: Like `{F2}`, but the field width is three characters.
- `{L}`: Source code line number, starting from one.
- `{L3}`: Like `{L}`, but the field width is three characters, aligned to the right, padded with spaces from left.
- `{L4}`: Like `{L3}`, but the field width is four characters.
- `{L5}`: Like `{L3}`, but the field width is five characters.
- `{S}`: The source code text of the emitted line, spaces trimmed from the left.

> If the template contains a placeholder multiple times, the engine generates output only the first occurrence and ignores the others.
 
The file indexes do not tell too much about the names of the source files. With the _Source file name output_ mode option, you can specify how the compiler puts file name information into the listing file:
- `None`: No source file information gets into the listing file.
- `Header`: Whenever the file index information changes, the compiler puts a section header with the name of the source code file.
- `FileMap`: The list of source files is displayed once at the top of the listing file.


