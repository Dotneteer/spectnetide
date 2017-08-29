using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using AntlrZ80Asm.SyntaxTree;
// ReSharper disable UsePatternMatching

// ReSharper disable JoinNullCheckWithUsage

namespace AntlrZ80Asm.Compiler
{
    /// <summary>
    /// This class implements the Z80 assembler
    /// </summary>
    public class Z80Assembler
    {
        private string _sourceText;
        private CompilerOptions _options;
        private CompilerOutput _output;

        private CompilationUnit _parsedLines;
        private Stack<bool?> _ifdefStack;
        private bool _processOps;

        /// <summary>
        /// The condition symbols
        /// </summary>
        public HashSet<string> ConditionSymbols { get; private set; } = new HashSet<string>();

        /// <summary>
        /// Lines after running the preprocessor
        /// </summary>
        public List<SourceLineBase> PreprocessedLines { get; private set; }

        /// <summary>
        /// This method compiles the passed Z80 Assembly code into Z80
        /// binary code.
        /// </summary>
        /// <param name="sourceText">Source code text</param>
        /// <param name="options">
        /// Compilation options. If null is passed, the compiler uses the
        /// default options
        /// </param>
        /// <returns>
        /// Output of the compilation
        /// </returns>
        public CompilerOutput Compile(string sourceText, CompilerOptions options = null)
        {
            if (sourceText == null)
            {
                throw new ArgumentNullException(nameof(sourceText));
            }

            // --- Init the compilation process
            _sourceText = sourceText;
            _options = options ?? new CompilerOptions();
            ConditionSymbols = new HashSet<string>(_options.PredefinedSymbols);
            _output = new CompilerOutput();

            // --- Do the compilation phases
            if (ExecuteParse()
                && ExecutePreprocessing()
                && EmitCode()
                && FixupSymbols())
            {
                // --- Successful compilation
            }
            else
            {
                // --- Sign the issues
                _output.Segments.Clear();
            }
            return _output;
        }

        /// <summary>
        /// Parses the source code passed to the compiler
        /// </summary>
        /// <returns>True, if parsing was successful</returns>
        private bool ExecuteParse()
        {
            var inputStream = new AntlrInputStream(_sourceText);
            var lexer = new Z80AsmLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80AsmParser(tokenStream);
            var context = parser.compileUnit();
            var visitor = new Z80AsmVisitor();
            visitor.Visit(context);
            _parsedLines = visitor.Compilation;
            foreach (var error in parser.SyntaxErrors)
            {
                _output.Errors.Add(new SyntaxError(error));
            }
            return _output.Errors.Count == 0;
        }

        /// <summary>
        /// This method processes the parsed lines and creates a list of
        /// lines that should be used for code emitting according to
        /// the preprocessor directives
        /// </summary>
        /// <returns></returns>
        private bool ExecutePreprocessing()
        {
            // --- Init the preprocessor
            var currentLineIndex = 0;
            PreprocessedLines = new List<SourceLineBase>();
            _ifdefStack = new Stack<bool?>();
            _processOps = true;

            // --- Traverse through parsed lines
            while (currentLineIndex < _parsedLines.Lines.Count)
            {
                var line = _parsedLines.Lines[currentLineIndex];
                var preProc = line as PreprocessorDirective;
                if (preProc != null)
                {
                    ApplyPreprocessorDirective(preProc);
                }
                else if (_processOps)
                {
                    PreprocessedLines.Add(line);
                }
                currentLineIndex++;
            }
            return _output.Errors.Count == 0;
        }

        /// <summary>
        /// Apply the specified prprocessor directive, and modify the
        /// current line index accordingly
        /// </summary>
        /// <param name="preProc">Preprocessor directive</param>
        private void ApplyPreprocessorDirective(PreprocessorDirective preProc)
        {
            if (preProc.Mnemonic == "#DEFINE" && _processOps)
            {
                // --- Define a symbol
                ConditionSymbols.Add(preProc.Identifier);
            }
            else if (preProc.Mnemonic == "#UNDEF" && _processOps)
            {
                // --- Remove a symbol
                if (_processOps) ConditionSymbols.Remove(preProc.Identifier);
            }
            else if (preProc.Mnemonic == "#IFDEF" || preProc.Mnemonic == "#IFNDEF")
            {
                // --- Evaluate the condition and stop/start processing
                // --- operations accordingly
                if (_processOps)
                {
                    _processOps = ConditionSymbols.Contains(preProc.Identifier) ^
                                  preProc.Mnemonic == "#IFNDEF";
                    _ifdefStack.Push(_processOps);
                }
                else
                {
                    // --- Do not process after #else or #endif
                    _ifdefStack.Push(null);
                }
            }
            else if (preProc.Mnemonic == "#ELSE")
            {
                if (_ifdefStack.Count == 0)
                {
                    _output.Errors.Add(new PreprocessorError(preProc, "Unexpected #else directive"));
                }
                else
                {
                    // --- Process operations according to the last
                    // --- condition's value
                    var peekVal = _ifdefStack.Pop();
                    if (peekVal.HasValue)
                    {
                        _processOps = !peekVal.Value;
                        _ifdefStack.Push(_processOps);
                    }
                    else
                    {
                        _ifdefStack.Push(null);
                    }
                }
            }
            else if (preProc.Mnemonic == "#ENDIF")
            {
                if (_ifdefStack.Count == 0)
                {
                    _output.Errors.Add(new PreprocessorError(preProc, "Unexpected #endif directive"));
                }
                else
                {
                    // --- It is the end of an #ifden/#ifndef block
                    _ifdefStack.Pop();
                    // ReSharper disable once PossibleInvalidOperationException
                    _processOps = _ifdefStack.Count == 0 || _ifdefStack.Peek().HasValue && _ifdefStack.Peek().Value;
                }
            }
        }

        private bool EmitCode()
        {
            return true;
        }

        private bool FixupSymbols()
        {
            return true;
        }
    }
}