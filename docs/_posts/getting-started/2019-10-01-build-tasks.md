---
layout: documents
categories: 
  - "Tutorials v2"
title:  "Build Tasks"
alias: build-tasks
seqno: 1090
selector: tutorial
permalink: "getting-started/build-tasks"
---

__SpectNetIDE__ allows you to extend the build chain with these kinds of tasks:

- *Pre-build*: command to run before the Z80/BASIC compilation.
- *Post-build*: command to run after the Z80/BASIC compilation.
- *Pre-export*: command to run after the successful Z80/BASIC compilation before the export operation.
- *Post-export*: command to run after the successful Z80/BASIC compilation and the export operation.
- *Build-error*: command to run when any of the pre-build, post-build, or compilation events fails. You can use this build action for cleanup activities.

The *Pre-build* and *Post-build* events run with the **Compile code**, **Inject program**, **Run program**, and **Debug program** commands. The *Pre-export* and *Post-export* events work with the **Export program** command only.

You can define these tasks in the `Spectrum.projconf` file, which you find in the root folder of your ZX Spectrum project.

## Migration Notes

If you created your ZX Spectrum project before v2 Preview 5, you need to add this file to your project manually. Use Windows Explorer (or command line) to place the `Spectrum.projconf` file into the ZX Spectrum project's root folder (and not into the solution root folder). Set the contents of the file to represent an empty JSON object:

```
{}
```

In Solution Explorer, right-click the project node and use the **Add \| Existing item** command to add the `Spectrum.projconf` file to the project.

## Creating Build and Export Tasks

Build tasks use instructions that you can run from a Windows command-line prompt. You can define them in `Spectrum.projconf` (JSON format, case-sensitive):

```
{
  "preBuild": "<Place pre-build command line here>",
  "postBuild": "<Place post-build command line here>",
  "errorBuild": "<Place post-build command line here>",
  "preExport": "<Place pre-export command line here>",
  "postExport": "<Place post-export command line here>"
}
```
Each of these tasks is optional. By default, a ZX Spectrum project leaves all of them empty.

Here is a sample:

```
{
  "preBuild": "echo 'It is just a pre-build message'",
  "postBuild": "myExtraBuildStep.bat",
  "errorBuild": "echo 'Something wrong happened...'",
}
```

In this sample, the pre-build and build-error tasks display a message, while the post-build task carries out extra compilation tasks by running a Windows batch file.

> **Note**: When running the commands, __SpectNetIDE__ uses the active project's root folder as the working directory.

## Macros

You can place macros into the build commands using these case-sensitive placeholders:
- `$(SolutionPath)`: Full path of the .sln file
- `$(SolutionDir)`: Solution folder
- `$(ProjectFile)`: Name of the project file (without path information)
- `$(ProjectDir)`: Name of the project directory
- `$(ProjectName)`: Name of the active project
- `$(SourcePath)`: Full path of the main file being compiled
- `$(SourceDir)`: Name of the directory in which the main file being compiled is stored

Export commands provide additional macros:

- `$(ExportPath)`: Full path of the target export file
- `$(ExportDir)`: Name of the directory of the target export file

Here is an example that displays the directory of the source file to compile and uses the `output` folder within the current directory to put some build artifacts, and the `bin` directory to store extra exported information:

```
{
  "preBuild": "echo $(SourceDir)",
  "postBuild": "myExtraBuildStep.bat $(ProjectDir)\output",
  "postExport": "myExtraExportStep.bat $(ProjectDir)\bin $(ExportPath)",
}
```

When developing, you can use the **Z80 Assembler** pane of the Output tool window in the IDE to check the commands issued. In this window, SpectNetIDE logs the exact command string it executes. Thus, here, you can check macro replacements.

## Command Output

Should any command behind the task fail, __SpectNetIDE__ would put the corresponding message into the __Error list__ tool window.
