using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Expressions;
using Spect.Net.Assembler.SyntaxTree.Pragmas;
using Z80AsmParser = Spect.Net.Assembler.Generated.Z80AsmParser;

// ReSharper disable StringLiteralTypo
// ReSharper disable UsePatternMatching
// ReSharper disable JoinNullCheckWithUsage

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class implements the Z80 assembler
    /// </summary>
    public partial class Z80Assembler
    {
        /// <summary>
        /// The file name of a direct text compilation
        /// </summary>
        public const string NO_FILE_ITEM = "#";

        /// <summary>
        /// The valid Spectrum model values
        /// </summary>
        public static string[] ValidModels = 
        {
            "SPECTRUM48",
            "SPECTRUM128",
            "SPECTRUMP3",
            "NEXT"
        };

        private AssemblerOptions _options;

        /// <summary>
        /// The output of the assembler
        /// </summary>
        public AssemblerOutput Output { get; private set; }

        /// <summary>
        /// The current module that determines the scope of labels
        /// </summary>
        public AssemblyModule CurrentModule { get; private set; }

        /// <summary>
        /// The condition symbols
        /// </summary>
        public HashSet<string> ConditionSymbols { get; private set; } = new HashSet<string>();

        /// <summary>
        /// Lines after running the preprocessor
        /// </summary>
        public List<SourceLineBase> PreprocessedLines { get; private set; }

        /// <summary>
        /// This event is raised whenever a TRACE pragma creates an output message
        /// </summary>
        public event EventHandler<AssemblerMessageArgs> AssemblerMessageCreated;

        /// <summary>
        /// COMPAREBIN pragma information
        /// </summary>
        public List<BinaryComparisonInfo> CompareBins { get; private set; }

        /// <summary>
        /// Raises a new assembler message
        /// </summary>
        /// <param name="message">Assembler message</param>
        protected virtual void OnAssemblerMessageCreated(string message)
        {
            AssemblerMessageCreated?.Invoke(this, new AssemblerMessageArgs(message));
        }

        /// <summary>
        /// This method compiles the Z80 Assembly code in the specified file into Z80
        /// binary code.
        /// </summary>
        /// <param name="filename">Z80 assembly source file</param>
        /// <param name="options">
        /// Compilation options. If null is passed, the compiler uses the
        /// default options
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>
        /// Output of the compilation
        /// </returns>
        public AssemblerOutput CompileFile(string filename, AssemblerOptions options = null)
        {
            var fi = new FileInfo(filename);
            var fullName = fi.FullName;
            var sourceText = File.ReadAllText(fullName);
            return DoCompile(new SourceFileItem(fullName), sourceText, options);
        }

        /// <summary>
        /// This method compiles the passed Z80 Assembly code into Z80
        /// binary code.
        /// </summary>
        /// <param name="sourceText">Source code text</param>
        /// <param name="options">
        /// Compilation options. If null is passed, the compiler uses the
        /// default options
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>
        /// Output of the compilation
        /// </returns>
        public AssemblerOutput Compile(string sourceText, AssemblerOptions options = null)
            => DoCompile(new SourceFileItem(NO_FILE_ITEM), sourceText, options);

        /// <summary>
        /// This method compiles the passed Z80 Assembly code into Z80
        /// binary code.
        /// </summary>
        /// <param name="sourceItem"></param>
        /// <param name="sourceText">Source code text</param>
        /// <param name="options">
        ///     Compilation options. If null is passed, the compiler uses the
        ///     default options
        /// </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>
        /// Output of the compilation
        /// </returns>
        private AssemblerOutput DoCompile(SourceFileItem sourceItem, string sourceText,
            AssemblerOptions options = null)
        {
            // --- Init the compilation process
            if (sourceText == null)
            {
                throw new ArgumentNullException(nameof(sourceText));
            }
            _options = options ?? new AssemblerOptions();
            ConditionSymbols = new HashSet<string>(_options.PredefinedSymbols);
            CurrentModule = Output = new AssemblerOutput(sourceItem);
            CompareBins = new List<BinaryComparisonInfo>();

            // --- Do the compilation phases
            if (!ExecuteParse(0, sourceItem, sourceText, out var lines)
                || !EmitCode(lines)
                || !FixupSymbols()
                || !CompareBinaries())
            {
                // --- Compilation failed, remove segments
                Output.Segments.Clear();
            }
            PreprocessedLines = lines;

            // --- Create symbol map
            Output.CreateSymbolMap();
            return Output;
        }

        #region Parsing and Directive processing

        /// <summary>
        /// Parses the source code passed to the compiler
        /// </summary>
        /// <param name="fileIndex">Source file index</param>
        /// <param name="sourceItem">Source file item</param>
        /// <param name="sourceText">Source text to parse</param>
        /// <param name="parsedLines"></param>
        /// <returns>True, if parsing was successful</returns>
        private bool ExecuteParse(int fileIndex, SourceFileItem sourceItem, string sourceText,
            out List<SourceLineBase> parsedLines)
        {
            // --- No lines has been parsed yet
            parsedLines = new List<SourceLineBase>();

            // --- Parse all source code lines
            var inputStream = new AntlrInputStream(sourceText);
            var lexer = new Z80AsmLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80AsmParser(tokenStream);
            var context = parser.compileUnit();
            var visitor = new Z80AsmVisitor();
            visitor.Visit(context);
            var visitedLines = visitor.Compilation;

            // --- Store any tasks defined by the user
            StoreTasks(sourceItem, visitedLines.Lines);

            // --- Collect syntax errors
            foreach (var error in parser.SyntaxErrors)
            {
                ReportError(sourceItem, error);
            }

            // --- Exit if there are any errors
            if (Output.ErrorCount != 0)
            {
                return false;
            }

            // --- Now, process directives and the .model pragma
            var currentLineIndex = 0;
            var ifdefStack = new Stack<bool?>();
            var processOps = true;
            parsedLines = new List<SourceLineBase>();

            // --- Traverse through parsed lines
            while (currentLineIndex < visitedLines.Lines.Count)
            {
                var line = visitedLines.Lines[currentLineIndex];
                if (line is ModelPragma modelPragma)
                {
                    ProcessModelPragma(modelPragma);
                }
                else if (line is IncludeDirective incDirective)
                {
                    // --- Parse the included file
                    if (ApplyIncludeDirective(incDirective, sourceItem,
                        out var includedLines))
                    {
                        // --- Add the parse result of the include file to the result
                        parsedLines.AddRange(includedLines);
                    }
                }
                else if (line is Directive preProc)
                {
                    ApplyDirective(preProc, ifdefStack, ref processOps);
                }
                else if (processOps)
                {
                    line.FileIndex = fileIndex;
                    line.SourceText = sourceText.Substring(line.FirstPosition,
                        line.LastPosition - line.FirstPosition + 1);
                    parsedLines.Add(line);
                }
                currentLineIndex++;
            }

            // --- Check if all #if and #ifdef has a closing #endif tag
            if (ifdefStack.Count > 0 && visitedLines.Lines.Count > 0)
            {
                ReportError(Errors.Z0062, visitedLines.Lines.Last());
            }

            return Output.ErrorCount == 0;
        }

        /// <summary>
        /// Retrieves task-related comments from the parsed lines.
        /// </summary>
        /// <param name="sourceItem">The file that has been parsed</param>
        /// <param name="lines">Parsed lines</param>
        private void StoreTasks(SourceFileItem sourceItem, IEnumerable<SourceLineBase> lines)
        {
            foreach (var line in lines)
            {
                if (line.DefinesTask)
                {
                    Output.Tasks.Add(new AssemblerTaskInfo(line.TaskDescription, 
                        sourceItem.Filename, line.SourceLine));
                }
            }
        }

        /// <summary>
        /// Loads and parses the file according the the #include directive
        /// </summary>
        /// <param name="incDirective">Directive with the file</param>
        /// <param name="sourceItem">Source file item</param>
        /// <param name="parsedLines">Collection of source code lines</param>
        private bool ApplyIncludeDirective(IncludeDirective incDirective,
            SourceFileItem sourceItem,
            out List<SourceLineBase> parsedLines)
        {
            parsedLines = new List<SourceLineBase>();

            // --- Check the #include directive
            var filename = incDirective.Filename.Trim();
            if (filename.StartsWith("<") && filename.EndsWith(">"))
            {
                filename = filename.Substring(1, filename.Length - 2);
            }

            // --- Now, we have the file name, calculate the path
            if (sourceItem.Filename != NO_FILE_ITEM)
            {
                // --- The file name is taken into account as relative
                var dirname = Path.GetDirectoryName(sourceItem.Filename) ?? string.Empty;
                filename = Path.Combine(dirname, filename);
            }

            // --- Check for file existence
            if (!File.Exists(filename))
            {
                ReportError(Errors.Z0300, incDirective, filename);
                return false;
            }

            var fi = new FileInfo(filename);
            var fullName = fi.FullName;

            // --- Check for repetition
            var childItem = new SourceFileItem(fullName);
            if (sourceItem.ContainsInIncludeList(childItem))
            {
                ReportError(Errors.Z0302, incDirective, filename);
                return false;
            }

            // --- Check for circular reference
            if (!sourceItem.Include(childItem))
            {
                ReportError(Errors.Z0303, incDirective, filename);
                return false;
            }

            // --- Now, add the included item to the output
            Output.SourceFileList.Add(childItem);

            // --- Read the include file
            string sourceText;
            try
            {
                sourceText = File.ReadAllText(filename);
            }
            catch (Exception ex)
            {
                ReportError(Errors.Z0301, incDirective, filename, ex.Message);
                return false;
            }

            // --- Parse the file
            return ExecuteParse(Output.SourceFileList.Count - 1, childItem, sourceText, out parsedLines);
        }

        /// <summary>
        /// Apply the specified preprocessor directive, and modify the
        /// current line index accordingly
        /// </summary>
        /// <param name="directive">Preprocessor directive</param>
        /// <param name="ifdefStack">Stack the holds #if/#ifdef information</param>
        /// <param name="processOps"></param>
        private void ApplyDirective(Directive directive, Stack<bool?> ifdefStack, ref bool processOps)
        {
            if (directive.Mnemonic == "#DEFINE" && processOps)
            {
                // --- Define a symbol
                ConditionSymbols.Add(directive.Identifier);
            }
            else if (directive.Mnemonic == "#UNDEF" && processOps)
            {
                // --- Remove a symbol
                ConditionSymbols.Remove(directive.Identifier);
            }
            else if (directive.Mnemonic == "#IFDEF" || directive.Mnemonic == "#IFNDEF"
                || directive.Mnemonic == "#IFMOD" || directive.Mnemonic == "#IFNMOD"
                || directive.Mnemonic == "#IF")
            {
                // --- Evaluate the condition and stop/start processing
                // --- operations accordingly
                if (processOps)
                {
                    if (directive.Mnemonic == "#IF")
                    {
                        var value = EvalImmediate(directive, directive.Expr);
                        processOps = value.IsValid && value.Value != 0;
                    }
                    else if (directive.Mnemonic == "#IFMOD" || directive.Mnemonic == "#IFNMOD")
                    {
                        // --- Check for invalid identifiers
                        if (!ValidModels.Contains(directive.Identifier))
                        {
                            ReportError(Errors.Z0090, directive);
                            processOps = false;
                        }
                        else
                        {
                            // --- OK, check the condition
                            var refModel = Output.ModelType ?? _options.CurrentModel;
                            processOps = refModel.ToString().ToUpper() == directive.Identifier ^
                                         directive.Mnemonic == "#IFNMOD";
                        }
                    }
                    else
                    {
                        processOps = ConditionSymbols.Contains(directive.Identifier) ^
                                      directive.Mnemonic == "#IFNDEF";
                    }
                    ifdefStack.Push(processOps);
                }
                else
                {
                    // --- Do not process after #else or #endif
                    ifdefStack.Push(null);
                }
            }
            else if (directive.Mnemonic == "#ELSE")
            {
                if (ifdefStack.Count == 0)
                {
                    ReportError(Errors.Z0060, directive);
                }
                else
                {
                    // --- Process operations according to the last
                    // --- condition's value
                    var peekVal = ifdefStack.Pop();
                    if (peekVal.HasValue)
                    {
                        processOps = !peekVal.Value;
                        ifdefStack.Push(processOps);
                    }
                    else
                    {
                        ifdefStack.Push(null);
                    }
                }
            }
            else if (directive.Mnemonic == "#ENDIF")
            {
                if (ifdefStack.Count == 0)
                {
                    ReportError(Errors.Z0061, directive);
                }
                else
                {
                    // --- It is the end of an #ifdef/#ifndef block
                    ifdefStack.Pop();
                    // ReSharper disable once PossibleInvalidOperationException
                    processOps = ifdefStack.Count == 0 || ifdefStack.Peek().HasValue && ifdefStack.Peek().Value;
                }
            }
        }

        #endregion Parsing and Directive processing

        #region Comparison

        /// <summary>
        /// Executes the COMPAREBIN pragmas
        /// </summary>
        /// <returns>True, if all comparison were OK; otherwise, false</returns>
        private bool CompareBinaries()
        {
            foreach (var binInfo in CompareBins)
            {
                var pragma = binInfo.ComparePragma;

                // --- Get the file name
                var fileNameValue = EvalImmediate(pragma, pragma.FileExpr);
                if (!fileNameValue.IsValid) continue;

                if (fileNameValue.Type != ExpressionValueType.String)
                {
                    ReportError(Errors.Z0306, pragma);
                    continue;
                }

                // --- Obtain optional offset
                var offset = 0;
                if (pragma.OffsetExpr != null)
                {
                    var offsValue = EvalImmediate(pragma, pragma.OffsetExpr);
                    if (offsValue.Type != ExpressionValueType.Integer)
                    {
                        ReportError(Errors.Z0308, pragma);
                        continue;
                    }
                    offset = (int)offsValue.AsLong();
                    if (offset < 0)
                    {
                        ReportError(Errors.Z0443, pragma);
                        continue;
                    }
                }

                // --- Obtain optional length
                int? length = null;
                if (pragma.LengthExpr != null)
                {
                    var lengthValue = EvalImmediate(pragma, pragma.LengthExpr);
                    if (lengthValue.Type != ExpressionValueType.Integer)
                    {
                        ReportError(Errors.Z0308, pragma);
                        continue;
                    }
                    length = (int)lengthValue.AsLong();
                    if (length < 0)
                    {
                        ReportError(Errors.Z0444, pragma);
                        continue;
                    }
                }

                // --- Read the binary file
                var currentSourceFile = Output.SourceFileList[pragma.FileIndex];
                var dirname = Path.GetDirectoryName(currentSourceFile.Filename) ?? string.Empty;
                var filename = Path.Combine(dirname, fileNameValue.AsString());

                byte[] contents;
                try
                {
                    var fileLength = new FileInfo(filename).Length;
                    using (var reader = new BinaryReader(File.OpenRead(filename)))
                    {
                        contents = reader.ReadBytes((int)fileLength);
                    }
                }
                catch (Exception e)
                {
                    ReportError(Errors.Z0446, pragma, e.Message);
                    continue;
                }

                // --- Check content segment
                if (offset >= contents.Length)
                {
                    ReportError(Errors.Z0443, pragma);
                    continue;
                }

                if (length == null)
                {
                    length = contents.Length - offset;
                }

                // --- Check length
                if (offset + length > contents.Length)
                {
                    ReportError(Errors.Z0444, pragma);
                    continue;
                }

                // --- Everything is ok, do the comparison
                var segment = binInfo.Segment;
                if (segment == null)
                {
                    ReportError(Errors.Z0445, pragma, "No output segment to compare.");
                    continue;
                }

                // --- Check current segment length
                if (binInfo.SegmentLength < length)
                {
                    ReportError(Errors.Z0445, pragma,
                        $"Current segment length is only {segment.CurrentOffset} while binary length to check is {length}");
                    continue;
                }

                for (var i = 0; i < length; i++)
                {
                    var segmData = segment.EmittedCode[i];
                    var binData = contents[i + offset];
                    if (segmData == binData) continue;
                    ReportError(Errors.Z0445, pragma,
                        $"Output segment at offset {i} (#{i:X4}) is {segmData} (#{segmData:X4}), but in binary it is {binData} (#{binData:X4})");
                    break;
                }
            }
            return Output.ErrorCount == 0;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Translates a Z80AsmParserErrorInfo instance into an error
        /// </summary>
        /// <param name="sourceItem">
        /// Source file information, to allow the error to track the filename the error occurred in
        /// </param>
        /// <param name="error">Error information</param>
        private void ReportError(SourceFileItem sourceItem, Z80AsmParserErrorInfo error)
        {
            var errInfo = new AssemblerErrorInfo(sourceItem, error);
            Output.Errors.Add(errInfo);
            ReportScopeError(errInfo.ErrorCode);
        }

        /// <summary>
        /// Reports the specified error
        /// </summary>
        /// <param name="errorCode">Code of error</param>
        /// <param name="line">Source line associated with the error</param>
        /// <param name="parameters">Optional error message parameters</param>
        public void ReportError(string errorCode, SourceLineBase line, params object[] parameters)
        {
            var sourceItem = line != null 
                ? Output.SourceFileList[line.FileIndex] 
                : null;
            Output.Errors.Add(new AssemblerErrorInfo(sourceItem, errorCode, line, parameters));
            ReportScopeError(errorCode);
        }

        /// <summary>
        /// Administers the error in the local scope
        /// </summary>
        /// <param name="errorCode"></param>
        private void ReportScopeError(string errorCode)
        {
            if (Output.LocalScopes.Count == 0) return;
            var localScope = Output.LocalScopes.Peek();
            if (localScope.OwnerScope != null)
            {
                localScope = localScope.OwnerScope;
            }
            localScope.ReportError(errorCode);
        }

        #endregion Helpers
    }
}