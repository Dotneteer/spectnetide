using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Expressions;
using Spect.Net.Assembler.SyntaxTree.Operations;
using Spect.Net.Assembler.SyntaxTree.Pragmas;
using Spect.Net.Assembler.SyntaxTree.Statements;

// ReSharper disable InlineOutVariableDeclaration
// ReSharper disable UsePatternMatching

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class implements the Z80 assembler
    /// </summary>
    public partial class Z80Assembler
    {
        public static readonly Regex MacroParamRegex = new Regex(@"{{\s*([_a-zA-Z][_a-zA-Z0-9]*)\s*}}");

        /// <summary>
        /// The current output segment of the emitted code
        /// </summary>
        public BinarySegment CurrentSegment { get; private set; }

        /// <summary>
        /// The current source line being processed
        /// </summary>
        public SourceLineBase CurrentSourceLine { get; private set; }

        /// <summary>
        /// A lable that overflew from a label-only line
        /// </summary>
        public NoInstructionLine OverflowLabelLine { get; private set; }

        /// <summary>
        /// Gets the current assembly address (represented by the "$" sign
        /// in the assembly language)
        /// </summary>
        /// <returns></returns>
        public ushort GetCurrentAssemblyAddress()
        {
            EnsureCodeSegment();
            return (ushort)(CurrentSegment.StartAddress 
                + (CurrentSegment?.Displacement ?? 0)
                + CurrentSegment.EmittedCode.Count);
        }

        /// <summary>
        /// Emits the code after processing the directives
        /// </summary>
        /// <returns></returns>
        private bool EmitCode(List<SourceLineBase> lines)
        {
            // --- Initialize code emission
            _output.Segments.Clear();
            EnsureCodeSegment();

            // --- Iterate through all parsed lines
            var currentLineIndex = 0;
            while (currentLineIndex < lines.Count)
            {
                var asmLine = lines[currentLineIndex];
                EmitSingleLine(lines, lines, asmLine, ref currentLineIndex);

                // --- Next line
                currentLineIndex++;
            }

            // --- Handle the orphan hanging label
            if (OverflowLabelLine != null)
            {
                CreateCurrentPointLabel(OverflowLabelLine);
                OverflowLabelLine = null;
            }

            // --- Ok, it's time to return with the result
            return _output.ErrorCount == 0;
        }

        /// <summary>
        /// Emits a single line
        /// </summary>
        /// <param name="allLines">All parsed lines</param>
        /// <param name="scopeLines">Lines to process in the current scope</param>
        /// <param name="asmLine">The line to emit</param>
        /// <param name="currentLineIndex">The index of the line to emit</param>
        /// <param name="fromMacroEmit">Is the method called during macro emit?</param>
        /// <returns></returns>
        private void EmitSingleLine(List<SourceLineBase> allLines, List<SourceLineBase> scopeLines, SourceLineBase asmLine, ref int currentLineIndex, 
            bool fromMacroEmit = false)
        {
            CurrentSourceLine = asmLine;

            // --- Report any parse-time function issue
            if (asmLine.EmitIssue != null)
            {
                ReportError(asmLine.EmitIssue, asmLine);
                return;
            }

            // --- No parse-time issue, process the line
            if (asmLine is NoInstructionLine noInstrLine)
            {
                if (noInstrLine.Label != null)
                {
                    // --- This is a label-only line
                    if (OverflowLabelLine != null)
                    {
                        // --- Create a label point for the previous label
                        CreateCurrentPointLabel(OverflowLabelLine);
                    }

                    OverflowLabelLine = noInstrLine;
                }
            }
            else
            {
                string currentLabel;
                if (OverflowLabelLine == null)
                {
                    // --- No hanging label, use the one in the current line
                    currentLabel = asmLine.Label;
                }
                else
                {
                    if (asmLine.Label == null)
                    {
                        // --- No current label, use the hanging label
                        currentLabel = OverflowLabelLine.Label;
                    }
                    else
                    {
                        // --- Create a point for the hanging label, and use the current label
                        CreateCurrentPointLabel(OverflowLabelLine);
                        currentLabel = asmLine.Label;
                        OverflowLabelLine = null;
                    }
                }

                // --- Check if there's a label to create
                if (currentLabel != null)
                {
                    // --- There's a label, we clear the previous hanging label
                    OverflowLabelLine = null;
                    if (!(asmLine is ILabelSetter))
                    {
                        // --- Create the label unless the current pragma does it
                        if (SymbolExists(currentLabel))
                        {
                            ReportError(Errors.Z0040, asmLine, currentLabel);
                        }
                        else
                        {
                            AddSymbol(currentLabel, new ExpressionValue(GetCurrentAssemblyAddress()));
                        }
                    }
                }

                // --- Let's handle assembly lines with macro parameters
                if (asmLine.MacroParams != null && asmLine.MacroParams.Count > 0)
                {
                    if (fromMacroEmit)
                    {
                        // --- Macro parameters cannot be passed in macro parameters
                        ReportError(Errors.Z0422, asmLine);
                    }
                    if (IsInGlobalScope)
                    {
                        // --- Macro parameters cannot be used in the global scope
                        ReportError(Errors.Z0420, asmLine);
                    }
                    else
                    {
                        // --- Macro argument used outside of a macro definition
                        var scope = _output.LocalScopes.Peek();
                        if (scope.IsMacroContext) return;

                        if (ShouldReportErrorInCurrentScope(Errors.Z0420))
                        {
                            ReportError(Errors.Z0420, asmLine);
                        }
                    }
                    return;
                }

                if (asmLine is PragmaBase pragmaLine)
                {
                    // --- Process a pragma
                    ApplyPragma(pragmaLine, currentLabel);
                }
                else if (asmLine is StatementBase statement)
                {
                    ProcessStatement(allLines, scopeLines, statement, currentLabel, ref currentLineIndex);
                }
                else if (asmLine is OperationBase opLine)
                {
                    // --- Emit the code output
                    var addr = GetCurrentAssemblyAddress();
                    EmitAssemblyOperationCode(opLine);

                    // --- Generate source map information
                    var sourceInfo = (opLine.FileIndex, opLine.SourceLine);
                    _output.SourceMap[addr] = sourceInfo;
                    _output.AddToAddressMap(opLine.FileIndex, opLine.SourceLine, addr);
                }
            }
        }

        /// <summary>
        /// Creates a label at the current point
        /// </summary>
        /// <param name="asmLine">Assembly line with a label</param>
        private void CreateCurrentPointLabel(SourceLineBase asmLine)
        {
            if (SymbolExists(asmLine.Label))
            {
                ReportError(Errors.Z0040, asmLine, asmLine.Label);
            }
            else
            {
               AddSymbol(asmLine.Label, new ExpressionValue(GetCurrentAssemblyAddress()));
            }
        }

        #region Statement processing

        /// <summary>
        /// Processes a compiler statement
        /// </summary>
        /// <param name="allLines">All parsed lines</param>
        /// <param name="scopeLines">Lines to process in the current scope</param>
        /// <param name="stmt">Statement to process</param>
        /// <param name="label">Label to process</param>
        /// <param name="currentLineIndex">Current line index</param>
        private void ProcessStatement(List<SourceLineBase> allLines, List<SourceLineBase> scopeLines, 
            StatementBase stmt, string label, ref int currentLineIndex)
        {
            switch (stmt)
            {
                case MacroStatement macroStmt:
                    CollectMacroDefinition(macroStmt, label, allLines, ref currentLineIndex);
                    break;

                case MacroEndStatement macroEndStmt:
                    ReportError(Errors.Z0405, macroEndStmt, "ENDM/MEND", "MACRO");
                    break;

                case ProcStatement procStatement:
                    ProcessProcStatement(procStatement, allLines, scopeLines, ref currentLineIndex);
                    break;

                case ProcEndStatement procEndStmt:
                    ReportError(Errors.Z0405, procEndStmt, "ENDP/PEND", "PROC");
                    break;

                case LoopStatement loopStatement:
                    ProcessLoopStatement(loopStatement, allLines, scopeLines, ref currentLineIndex);
                    break;

                case LoopEndStatement loopEndStmt:
                    ReportError(Errors.Z0405, loopEndStmt, "ENDL/LEND", "LOOP");
                    break;

                case RepeatStatement repeatStatement:
                    ProcessRepeatStatement(repeatStatement, allLines, scopeLines, ref currentLineIndex);
                    break;

                case UntilStatement untilStmt:
                    ReportError(Errors.Z0405, untilStmt, "UNTIL", "REPEAT");
                    break;

                case WhileStatement whileStatement:
                    ProcessWhileStatement(whileStatement, allLines, scopeLines, ref currentLineIndex);
                    break;

                case WhileEndStatement whileEndStmt:
                    ReportError(Errors.Z0405, whileEndStmt, "ENDW/WEND", "WHILE");
                    break;

                case ForStatement forStatement:
                    ProcessForStatement(forStatement, allLines, scopeLines, ref currentLineIndex);
                    break;

                case NextStatement ifEndStmt:
                    ReportError(Errors.Z0405, ifEndStmt, "NEXT", "FOR");
                    break;

                case ElifStatement elifStmt:
                    ReportError(Errors.Z0405, elifStmt, "ELIF", "IF");
                    break;

                case IfStatement ifStatement:
                    ProcessIfStatement(ifStatement, allLines, scopeLines, ref currentLineIndex);
                    break;

                case ElseStatement elseStmt:
                    ReportError(Errors.Z0405, elseStmt, "ELSE", "IF");
                    break;

                case IfEndStatement ifEndStmt:
                    ReportError(Errors.Z0405, ifEndStmt, "ENDIF", "IF");
                    break;

                case BreakStatement breakStmt:
                    ProcessBreakStatement(breakStmt);
                    break;

                case ContinueStatement continueStmt:
                    ProcessContinueStatement(continueStmt);
                    break;

                case MacroInvocation macroInvokeStmt:
                    ProcessMacroInvocation(macroInvokeStmt, allLines);
                    break;
            }
        }

        /// <summary>
        /// Checks and collects the current macro definition
        /// </summary>
        /// <param name="macro">Statement for the macro</param>
        /// <param name="label">Label of the macro</param>
        /// <param name="allLines">All parsed lines</param>
        /// <param name="currentLineIndex">Index of the macro definition line</param>
        private void CollectMacroDefinition(MacroStatement macro, string label,
            List<SourceLineBase> allLines, ref int currentLineIndex)
        {
            var errorFound = false;
            // --- Check for parameter uniqueness
            var args = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var macroArg in macro.Arguments)
            {
                if (args.Contains(macroArg))
                {
                    ReportError(Errors.Z0417, macro, macroArg);
                    errorFound = true;
                }
                args.Add(macroArg);
            }

            if (label == null)
            {
                errorFound = true;
                ReportError(Errors.Z0400, macro);
            }
            else if (_output.Macros.ContainsKey(label))
            {
                errorFound = true;
                ReportError(Errors.Z0402, macro, label);
            }

            // --- Search for the end of the macro
            var firstLine = currentLineIndex;
            if (!macro.SearchForEnd(this, allLines, ref currentLineIndex, out var endLabel)) return;

            // --- Create macro definition
            var macroDef = new MacroDefinition(label, firstLine, currentLineIndex, 
                macro.Arguments, endLabel);

            // --- Check each macro line for invalid macro parameter names
            // --- or nested macro
            for (var i = firstLine + 1; i < currentLineIndex; i++)
            {
                var macroLine = allLines[i];
                if (macroLine.EmitIssue != null)
                {
                    ReportError(macroLine.EmitIssue, macroLine);
                    errorFound = true;
                    continue;
                }

                if (macroLine is MacroStatement)
                {
                    ReportError(Errors.Z0404, macroLine);
                    errorFound = true;
                    continue;
                }

                if (macroLine.MacroParamNames != null)
                {
                    foreach (var paramName in macroLine.MacroParamNames)
                    {
                        if (macro.Arguments.Contains(paramName)) continue;

                        errorFound = true;
                        ReportError(Errors.Z0403, macroLine, paramName);
                    }
                }
            }

            // --- If macro is OK, store it
            if (!errorFound)
            {
                _output.Macros[label] = macroDef;
            }
        }

        /// <summary>
        /// Processes the LOOP statement
        /// </summary>
        /// <param name="loop">LOOP statement</param>
        /// <param name="allLines">All parsed lines</param>
        /// <param name="scopeLines">Lines to process in the current scope</param>
        /// <param name="currentLineIndex">Index of the LOOP definition line</param>
        private void ProcessLoopStatement(LoopStatement loop, List<SourceLineBase> allLines, 
            List<SourceLineBase> scopeLines, ref int currentLineIndex)
        {
            // --- Search for the end of the loop
            var firstLine = currentLineIndex;
            if (!loop.SearchForEnd(this, scopeLines, ref currentLineIndex, out var endLabel)) return;

            // --- End found
            var lastLine = currentLineIndex;

            // --- Now, we can process the loop
            var loopCounter = EvalImmediate(loop, loop.Expr);
            if (!loopCounter.IsValid) return;
            if (loopCounter.Type == ExpressionValueType.String)
            {
                ReportError(Errors.Z0305, loop);
                return;
            }

            // --- Check the loop counter
            var counter = loopCounter.AsLong();
            if (counter >= 0x10000)
            {
                ReportError(Errors.Z0406, loop);
                counter = 1;
            }

            // --- Create a scope for the loop
            var loopScope = new SymbolScope();
            _output.LocalScopes.Push(loopScope);
            var errorsBefore = _output.ErrorCount;

            for (var i = 0; i < counter; i++)
            {
                // --- Create a local scope for the loop body
                var iterationScope = new SymbolScope(loopScope);
                _output.LocalScopes.Push(iterationScope);
                iterationScope.LoopCounter = i + 1;

                var loopLineIndex = firstLine + 1;
                while (loopLineIndex < lastLine)
                {
                    var curLine = scopeLines[loopLineIndex];
                    EmitSingleLine(allLines, scopeLines, curLine, ref loopLineIndex);
                    if (iterationScope.BreakReached || iterationScope.ContinueReached)
                    {
                        break;
                    }
                    loopLineIndex++;
                }

                // --- Add the end label to the local scope
                if (endLabel != null)
                {
                    // --- Add the end label to the loop scope
                    var endLine = scopeLines[currentLineIndex];
                    if (SymbolExists(endLabel))
                    {
                        ReportError(Errors.Z0040, endLine, endLabel);
                    }
                    else
                    {
                        AddSymbol(endLabel, new ExpressionValue(GetCurrentAssemblyAddress()));
                    }
                }

                // --- Clean up the hanging label
                OverflowLabelLine = null;

                // --- Fixup the symbols locally
                FixupSymbols(iterationScope.Fixups, iterationScope.Symbols, false);

                // --- Remove the local scope
                _output.LocalScopes.Pop();

                // --- Check for the maximum number of error
                if (_output.ErrorCount - errorsBefore >= _options.MaxLoopErrorsToReport)
                {
                    ReportError(Errors.Z0408, loop);
                    break;
                }

                // --- BREAK reached, exit the loop
                if (iterationScope.BreakReached)
                {
                    break;
                }
            }

            // --- Clean up the loop's scope
            _output.LocalScopes.Pop();
        }

        /// <summary>
        /// Processes the PROC statement
        /// </summary>
        /// <param name="proc">LOOP statement</param>
        /// <param name="allLines">All parsed lines</param>
        /// <param name="scopeLines">Lines to process in the current scope</param>
        /// <param name="currentLineIndex">Index of the LOOP definition line</param>
        private void ProcessProcStatement(ProcStatement proc, List<SourceLineBase> allLines,
            List<SourceLineBase> scopeLines, ref int currentLineIndex)
        {
            // --- Search for the end of the loop
            var firstLine = currentLineIndex;
            if (!proc.SearchForEnd(this, scopeLines, ref currentLineIndex, out var endLabel)) return;

            // --- End found
            var lastLine = currentLineIndex;

            // --- Create a scope for the proc
            var procScope = new SymbolScope {IsLoopScope = false};
            _output.LocalScopes.Push(procScope);

            // --- Create a local scope for the loop body
            var loopLineIndex = firstLine + 1;
            while (loopLineIndex < lastLine)
            {
                var curLine = scopeLines[loopLineIndex];
                EmitSingleLine(allLines, scopeLines, curLine, ref loopLineIndex);
                loopLineIndex++;
            }

            // --- Add the end label to the local scope
            if (endLabel != null)
            {
                // --- Add the end label to the loop scope
                var endLine = scopeLines[currentLineIndex];
                if (SymbolExists(endLabel))
                {
                    ReportError(Errors.Z0040, endLine, endLabel);
                }
                else
                {
                    AddSymbol(endLabel, new ExpressionValue(GetCurrentAssemblyAddress()));
                }
            }

            // --- Clean up the hanging label
            OverflowLabelLine = null;

            // --- Fixup the symbols locally
            FixupSymbols(procScope.Fixups, procScope.Symbols, false);

            // --- Clean up the loop's scope
            _output.LocalScopes.Pop();
        }

        /// <summary>
        /// Processes the REPEAT statement
        /// </summary>
        /// <param name="repeat">REPEAT statement</param>
        /// <param name="allLines">All parsed lines</param>
        /// <param name="scopeLines">Lines to process in the current scope</param>
        /// <param name="currentLineIndex">Index of the REPEAT definition line</param>
        private void ProcessRepeatStatement(RepeatStatement repeat,
            List<SourceLineBase> allLines,
            List<SourceLineBase> scopeLines, ref int currentLineIndex)
        {
            // --- Search for the end of the loop
            var firstLine = currentLineIndex;
            if (!repeat.SearchForEnd(this, scopeLines, ref currentLineIndex, out var endLabel)) return;

            // --- End found
            var lastLine = currentLineIndex;
            var untilStmt = scopeLines[lastLine] as UntilStatement;

            // --- Create a scope for the repeat loop
            var loopScope = new SymbolScope();
            _output.LocalScopes.Push(loopScope);
            var errorsBefore = _output.ErrorCount;

            // --- Execute the REPEAT body
            var loopCount = 1;
            bool condition;
            do
            {
                // --- Create a local scope for the repeat body
                var iterationScope = new SymbolScope(loopScope);
                _output.LocalScopes.Push(iterationScope);
                iterationScope.LoopCounter = loopCount;

                var loopLineIndex = firstLine + 1;
                while (loopLineIndex < lastLine)
                {
                    var curLine = scopeLines[loopLineIndex];
                    EmitSingleLine(allLines, scopeLines, curLine, ref loopLineIndex);
                    if (iterationScope.BreakReached || iterationScope.ContinueReached)
                    {
                        break;
                    }
                    loopLineIndex++;
                }

                // --- Add the end label to the local scope
                if (endLabel != null)
                {
                    // --- Add the end label to the loop scope
                    var endLine = scopeLines[currentLineIndex];
                    if (SymbolExists(endLabel))
                    {
                        ReportError(Errors.Z0040, endLine, endLabel);
                    }
                    else
                    {
                        AddSymbol(endLabel, new ExpressionValue(GetCurrentAssemblyAddress()));
                    }
                }

                // --- Clean up the hanging label
                OverflowLabelLine = null;

                // --- Fixup the symbols locally
                FixupSymbols(iterationScope.Fixups, iterationScope.Symbols, false);

                // --- Check for the maximum number of error
                if (_output.ErrorCount - errorsBefore >= _options.MaxLoopErrorsToReport)
                {
                    ReportError(Errors.Z0408, repeat);
                    break;
                }

                // --- Evaluate the loop expression
                var loopExitCondition = EvalImmediate(untilStmt, untilStmt?.Expr);
                if (!loopExitCondition.IsValid) return;
                if (loopExitCondition.Type == ExpressionValueType.String)
                {
                    ReportError(Errors.Z0305, untilStmt);
                    break;
                }
                condition = loopExitCondition.AsBool();

                // --- Increment counter, check loop safety
                loopCount++;
                iterationScope.LoopCounter = loopCount;
                if (loopCount >= 0x10000)
                {
                    ReportError(Errors.Z0409, repeat);
                    break;
                }

                // --- Remove the local scope
                _output.LocalScopes.Pop();

                // --- BREAK reached, exit the loop
                if (iterationScope.BreakReached)
                {
                    break;
                }
            } while (!condition);

            // --- Clean up the loop's scope
            _output.LocalScopes.Pop();
        }

        /// <summary>
        /// Processes the WHILE statement
        /// </summary>
        /// <param name="whileStmt">WHILE statement</param>
        /// <param name="allLines">All parsed lines</param>
        /// <param name="scopeLines">Lines to process in the current scope</param>
        /// <param name="currentLineIndex">Index of the WHILE definition line</param>
        private void ProcessWhileStatement(WhileStatement whileStmt,
            List<SourceLineBase> allLines,
            List<SourceLineBase> scopeLines, ref int currentLineIndex)
        {
            // --- Search for the end of the loop
            var firstLine = currentLineIndex;
            if (!whileStmt.SearchForEnd(this, scopeLines, ref currentLineIndex, out var endLabel)) return;

            // --- End found
            var lastLine = currentLineIndex;
            
            // --- Create a scope for the while loop
            var loopScope = new SymbolScope();
            _output.LocalScopes.Push(loopScope);
            var errorsBefore = _output.ErrorCount;

            // --- Execute the WHILE body
            var loopCount = 1;
            while (true)
            {
                // --- Create a local scope for the repeat body
                var iterationScope = new SymbolScope(loopScope);
                _output.LocalScopes.Push(iterationScope);
                iterationScope.LoopCounter = loopCount;

                // --- Evaluate the loop expression
                var loopCondition = EvalImmediate(whileStmt, whileStmt.Expr);
                if (!loopCondition.IsValid) return;
                if (loopCondition.Type == ExpressionValueType.String)
                {
                    ReportError(Errors.Z0305, whileStmt);
                    break;
                }

                // --- Exit if while condition fails
                if (!loopCondition.AsBool()) break;

                var loopLineIndex = firstLine + 1;
                while (loopLineIndex < lastLine)
                {
                    var curLine = scopeLines[loopLineIndex];
                    EmitSingleLine(allLines, scopeLines, curLine, ref loopLineIndex);
                    if (iterationScope.BreakReached || iterationScope.ContinueReached)
                    {
                        break;
                    }
                    loopLineIndex++;
                }

                // --- Add the end label to the local scope
                if (endLabel != null)
                {
                    // --- Add the end label to the loop scope
                    var endLine = scopeLines[currentLineIndex];
                    if (SymbolExists(endLabel))
                    {
                        ReportError(Errors.Z0040, endLine, endLabel);
                    }
                    else
                    {
                        AddSymbol(endLabel, new ExpressionValue(GetCurrentAssemblyAddress()));
                    }
                }

                // --- Clean up the hanging label
                OverflowLabelLine = null;

                // --- Fixup the symbols locally
                FixupSymbols(iterationScope.Fixups, iterationScope.Symbols, false);

                // --- Remove the local scope
                _output.LocalScopes.Pop();

                // --- Check for the maximum number of error
                if (_output.ErrorCount - errorsBefore >= _options.MaxLoopErrorsToReport)
                {
                    ReportError(Errors.Z0408, whileStmt);
                    break;
                }

                // --- Increment counter, check loop safety
                loopCount++;
                if (loopCount >= 0xFFFF)
                {
                    ReportError(Errors.Z0409, whileStmt);
                    break;
                }

                // --- BREAK reached, exit the loop
                if (iterationScope.BreakReached)
                {
                    break;
                }
            }

            // --- Clean up the loop's scope
            _output.LocalScopes.Pop();
        }

        /// <summary>
        /// Processes the FOR statement
        /// </summary>
        /// <param name="forStmt">FOR statement</param>
        /// <param name="allLines">All parsed lines</param>
        /// <param name="scopeLines">Lines to process in the current scope</param>
        /// <param name="currentLineIndex">Index of the FOR definition line</param>
        private void ProcessForStatement(ForStatement forStmt,
            List<SourceLineBase> allLines,
            List<SourceLineBase> scopeLines, ref int currentLineIndex)
        {
            // --- Search for the end of the loop
            var firstLine = currentLineIndex;
            if (!forStmt.SearchForEnd(this, scopeLines, ref currentLineIndex, out var endLabel)) return;

            // --- End found
            var lastLine = currentLineIndex;

            // --- Evaluate FROM, TO, and STEP expressions
            var fromValue = EvalImmediate(forStmt, forStmt.From);
            if (!fromValue.IsValid) return;
            if (fromValue.Type == ExpressionValueType.String)
            {
                ReportError(Errors.Z0305, forStmt);
                return;
            }

            var toValue = EvalImmediate(forStmt, forStmt.To);
            if (!toValue.IsValid) return;
            if (toValue.Type == ExpressionValueType.String)
            {
                ReportError(Errors.Z0305, forStmt);
                return;
            }

            var stepValue = new ExpressionValue(1);
            if (forStmt.Step != null)
            {
                stepValue = EvalImmediate(forStmt, forStmt.Step);
                if (!stepValue.IsValid) return;
                if (stepValue.Type == ExpressionValueType.String)
                {
                    ReportError(Errors.Z0305, forStmt);
                    return;
                }
                if (Math.Abs(stepValue.AsReal()) < double.Epsilon)
                {
                    ReportError(Errors.Z0413, forStmt);
                    return;
                }
            }

            // --- Check the FOR variable
            if (VariableExists(forStmt.ForVariable))
            {
                ReportError(Errors.Z0414, forStmt, forStmt.ForVariable);
                return;
            }

            // --- Create a scope for the FOR loop
            var loopScope = new SymbolScope();
            _output.LocalScopes.Push(loopScope);

            // --- Init the FOR variable
            loopScope.Vars[forStmt.ForVariable] = fromValue;
            var errorsBefore = _output.ErrorCount;

            var isIntLoop =
                (fromValue.Type == ExpressionValueType.Bool || fromValue.Type == ExpressionValueType.Integer)
                && (toValue.Type == ExpressionValueType.Bool || toValue.Type == ExpressionValueType.Integer)
                && (stepValue.Type == ExpressionValueType.Bool || stepValue.Type == ExpressionValueType.Integer);

            var loopIntValue = fromValue.AsLong();
            var endIntValue = toValue.AsLong();
            var incIntValue = stepValue.AsLong();
            var loopRealValue = fromValue.AsReal();
            var endRealValue = toValue.AsReal();
            var incRealValue = stepValue.AsReal();

            // --- Execute the WHILE body
            var loopCount = 0;
            while (true)
            {
                // --- Check the loop's exit condition
                if (isIntLoop)
                {
                    if (incIntValue > 0 && loopIntValue > endIntValue) break;
                    if (incIntValue < 0 && loopIntValue < endIntValue) break;
                }
                else
                {
                    if (incRealValue > 0 && loopRealValue > endRealValue) break;
                    if (incRealValue < 0 && loopRealValue < endRealValue) break;
                }

                // --- Increment counter, check loop safety
                loopCount++;
                if (loopCount >= 0xFFFF)
                {
                    ReportError(Errors.Z0409, forStmt);
                    break;
                }

                // --- Create a local scope for the FOR body
                var iterationScope = new SymbolScope(loopScope);
                _output.LocalScopes.Push(iterationScope);
                iterationScope.LoopCounter = loopCount;

                var loopLineIndex = firstLine + 1;
                while (loopLineIndex < lastLine)
                {
                    var curLine = scopeLines[loopLineIndex];
                    EmitSingleLine(allLines, scopeLines, curLine, ref loopLineIndex);
                    if (iterationScope.BreakReached || iterationScope.ContinueReached)
                    {
                        break;
                    }
                    loopLineIndex++;
                }

                // --- Add the end label to the local scope
                if (endLabel != null)
                {
                    // --- Add the end label to the loop scope
                    var endLine = scopeLines[currentLineIndex];
                    if (SymbolExists(endLabel))
                    {
                        ReportError(Errors.Z0040, endLine, endLabel);
                    }
                    else
                    {
                        AddSymbol(endLabel, new ExpressionValue(GetCurrentAssemblyAddress()));
                    }
                }

                // --- Clean up the hanging label
                OverflowLabelLine = null;

                // --- Fixup the symbols locally
                FixupSymbols(iterationScope.Fixups, iterationScope.Symbols, false);

                // --- Remove the local scope
                _output.LocalScopes.Pop();

                // --- Check for the maximum number of error
                if (_output.ErrorCount - errorsBefore >= _options.MaxLoopErrorsToReport)
                {
                    ReportError(Errors.Z0408, forStmt);
                    break;
                }

                // --- BREAK reached, exit the loop
                if (iterationScope.BreakReached)
                {
                    break;
                }

                // --- Increment cycle variable
                if (isIntLoop)
                {
                    loopIntValue += incIntValue;
                    loopScope.Vars[forStmt.ForVariable] = new ExpressionValue(loopIntValue);
                }
                else
                {
                    loopRealValue += incRealValue;
                    loopScope.Vars[forStmt.ForVariable] = new ExpressionValue(loopRealValue);
                }
            }

            // --- Clean up the loop's scope
            _output.LocalScopes.Pop();
        }

        /// <summary>
        /// Processes the BREAK statement
        /// </summary>
        /// <param name="breakStmt">BREAK statement</param>
        private void ProcessBreakStatement(SourceLineBase breakStmt)
        {
            if (IsInGlobalScope || !_output.LocalScopes.Peek().IsLoopScope)
            {
                ReportError(Errors.Z0415, breakStmt);
                return;
            }
            _output.LocalScopes.Peek().BreakReached = true;
        }

        /// <summary>
        /// Processes the CONTINUE statement
        /// </summary>
        /// <param name="continueStmt">CONTINUE statement</param>
        private void ProcessContinueStatement(SourceLineBase continueStmt)
        {
            if (IsInGlobalScope || !_output.LocalScopes.Peek().IsLoopScope)
            {
                ReportError(Errors.Z0416, continueStmt);
                return;
            }
            _output.LocalScopes.Peek().ContinueReached = true;
        }

        /// <summary>
        /// Processes the if statement
        /// </summary>
        /// <param name="ifStmt">IF statement</param>
        /// <param name="allLines">All parsed lines</param>
        /// <param name="scopeLines">Lines to process in the current scope</param>
        /// <param name="currentLineIndex">Index of the IF definition line</param>
        private void ProcessIfStatement(IfStatement ifStmt,
            List<SourceLineBase> allLines,
            List<SourceLineBase> scopeLines, ref int currentLineIndex)
        {
            // --- Search for the end of the loop
            var ifDef = GetIfSections(ifStmt, scopeLines, ref currentLineIndex, out var endLabel);
            if (ifDef == null) return;

            // --- Process the IF definition
            IfSection sectionToCompile = null;
            foreach (var ifSection in ifDef.IfSections)
            {
                // --- Evaluate the condition
                var expr = ifSection.IfStatement is ElifStatement elifStmt 
                    ? elifStmt.Expr 
                    : ifStmt.Expr;
                var exprValue = EvalImmediate(ifSection.IfStatement, expr);

                // --- Handle evaluation errors
                if (!exprValue.IsValid) continue;
                if (exprValue.Type == ExpressionValueType.String)
                {
                    ReportError(Errors.Z0305, ifSection.IfStatement);
                    continue;
                }

                // --- Check the condition
                if (exprValue.AsBool())
                {
                    sectionToCompile = ifSection;
                }
            }

            // --- Check if there is any section to compile
            sectionToCompile = sectionToCompile ?? ifDef.ElseSection;
            if (sectionToCompile == null)
            {
                // --- No matching IF, ELIF, and no ELSE, so there's nothing to emit
                return;
            }

            // --- Emit the matching section
            var loopLineIndex = sectionToCompile.Section.FirstLine + 1;
            while (loopLineIndex < sectionToCompile.Section.LastLine)
            {
                var curLine = scopeLines[loopLineIndex];
                EmitSingleLine(allLines, scopeLines, curLine, ref loopLineIndex);
                loopLineIndex++;
            }

            // --- Add the end label to the local scope
            if (endLabel != null)
            {
                // --- Add the end label to the loop scope
                var endLine = scopeLines[currentLineIndex];
                if (SymbolExists(endLabel))
                {
                    ReportError(Errors.Z0040, endLine, endLabel);
                }
                else
                {
                    AddSymbol(endLabel, new ExpressionValue(GetCurrentAssemblyAddress()));
                }
            }

            // --- Clean up the hanging label
            OverflowLabelLine = null;
        }

        /// <summary>
        /// Collects the strcuture information of an IF statement and makes fundamental syntax checks
        /// </summary>
        /// <param name="ifStmt">IF statement</param>
        /// <param name="lines">Parsed assembly lines</param>
        /// <param name="currentLineIndex">Index of the LOOP definition line</param>
        /// <param name="endLabel">Optional label of the ENDIF statement</param>
        /// <returns></returns>
        private IfDefinition GetIfSections(IfStatement ifStmt, List<SourceLineBase> lines, ref int currentLineIndex, out string endLabel)
        {
            endLabel = null;
            if (currentLineIndex >= lines.Count)
            {
                return null;
            }

            var ifDef = new IfDefinition();
            var firstLine = currentLineIndex;
            var sectionStart = firstLine;
            var sectionStmt = lines[sectionStart] as StatementBase;
            var elseDetected = false;
            var errorDetected = false;
            currentLineIndex++;

            // --- Iterate through lines
            while (currentLineIndex < lines.Count)
            {
                var curLine = lines[currentLineIndex];

                // --- Check for ENDIF
                if (curLine is IfEndStatement)
                {
                    // --- We have found the end line, get its label
                    endLabel = curLine.Label ?? endLabel;
                    if (elseDetected)
                    {
                        // --- Store the ELSE section
                        ifDef.ElseSection = new IfSection(null, sectionStart, currentLineIndex);
                    }
                    else
                    {
                        // --- Store the IF/ELIF section
                        ifDef.IfSections.Add(new IfSection(sectionStmt, sectionStart, currentLineIndex));
                    }

                    // --- Calculate the entire IF section and return with it
                    ifDef.FullSection = new DefinitionSection(firstLine, currentLineIndex);
                    return errorDetected ? null : ifDef;
                }

                // --- Check for ELIF section
                if (curLine is ElifStatement elifStmt)
                {
                    endLabel = curLine.Label ?? endLabel;
                    if (endLabel != null)
                    {
                        ReportError(Errors.Z0411, sectionStmt, "ELIF");
                    }
                    if (elseDetected)
                    {
                        errorDetected = true;
                        ReportError(Errors.Z0410, curLine, "ELIF");
                    }
                    else
                    {
                        // --- Store the previous section
                        ifDef.IfSections.Add(new IfSection(sectionStmt, sectionStart, currentLineIndex));
                        sectionStmt = elifStmt;
                        sectionStart = currentLineIndex;
                    }
                }

                // --- Check for ELSE section
                else if (curLine is ElseStatement)
                {
                    endLabel = curLine.Label ?? endLabel;
                    if (endLabel != null)
                    {
                        ReportError(Errors.Z0411, sectionStmt, "ELSE");
                    }
                    if (elseDetected)
                    {
                        errorDetected = true;
                        ReportError(Errors.Z0410, curLine, "ELSE");
                    }
                    else
                    {
                        // --- Store the previous section
                        ifDef.IfSections.Add(new IfSection(sectionStmt, sectionStart, currentLineIndex));
                        sectionStart = currentLineIndex;
                    }
                    elseDetected = true;
                }

                if (curLine is NoInstructionLine noinstrLine)
                {
                    // --- Record the last hanging label
                    endLabel = noinstrLine.Label;
                }
                else
                {
                    endLabel = null;
                    if (curLine is BlockStatementBase blockStmt)
                    {
                        // --- Search for the end of an embedded block statement
                        var success = blockStmt.SearchForEnd(this, lines, ref currentLineIndex, out endLabel);
                        if (!success)
                        {
                            ReportError(Errors.Z0401, lines[firstLine], blockStmt.EndStatementName);
                            return null;
                        }
                    }
                }

                currentLineIndex++;
            }
            ReportError(Errors.Z0401, lines[firstLine], ifStmt.EndStatementName);
            return null;
        }

        /// <summary>
        /// Handles the invocation of a MACRO
        /// </summary>
        /// <param name="macroStmt">MACRO invocation statement</param>
        /// <param name="allLines">All parsed lines</param>
        private void ProcessMacroInvocation(MacroInvocation macroStmt,
            List<SourceLineBase> allLines)
        {
            // --- Check if macro definition exists
            if (!_output.Macros.TryGetValue(macroStmt.Name, out var macroDef))
            {
                ReportError(Errors.Z0418, macroStmt, macroStmt.Name);
                return;
            }

            // --- Match parameters
            if (macroDef.ArgumentNames.Count < macroStmt.Parameters.Count)
            {
                ReportError(Errors.Z0419, macroStmt, macroDef.MacroName, macroDef.ArgumentNames.Count, macroStmt.Parameters.Count);
                return;
            }

            // --- Evaluate arguments
            var arguments = new Dictionary<string, ExpressionValue>(StringComparer.InvariantCultureIgnoreCase);
            var errorFound = false;
            var emptyArgValue = new ExpressionValue("$<none>$");
            for (var i = 0; i < macroDef.ArgumentNames.Count; i++)
            {
                if (i >= macroStmt.Parameters.Count)
                {
                    arguments.Add(macroDef.ArgumentNames[i], emptyArgValue);
                    continue;
                }
                var op = macroStmt.Parameters[i];
                ExpressionValue argValue;
                switch (op.Type)
                {
                    case OperandType.Reg8:
                    case OperandType.Reg8Idx:
                    case OperandType.Reg8Spec:
                    case OperandType.Reg16:
                    case OperandType.Reg16Idx:
                    case OperandType.Reg16Spec:
                    case OperandType.RegIndirect:
                        argValue = new ExpressionValue(op.Register);
                        break;
                    case OperandType.Expr:
                        argValue = EvalImmediate(macroStmt, op.Expression);
                        if (!argValue.IsValid) errorFound = true;
                        break;
                    case OperandType.MemIndirect:
                        argValue = EvalImmediate(macroStmt, op.Expression);
                        if (!argValue.IsValid) errorFound = true;
                        else
                        {
                            argValue = new ExpressionValue($"({argValue.AsString()})");
                        } 
                        break;
                    case OperandType.CPort:
                        argValue = new ExpressionValue("(C)");
                        break;
                    case OperandType.IndexedAddress:
                        if (op.Expression == null)
                        {
                            argValue = new ExpressionValue($"({op.Register})");
                        }
                        else
                        {
                            argValue = EvalImmediate(macroStmt, op.Expression);
                            if (!argValue.IsValid) errorFound = true;
                            else
                            {
                                argValue = new ExpressionValue($"({op.Register}{op.Sign}{argValue.AsString()})");
                            }
                        }
                        break;
                    case OperandType.Condition:
                        argValue = new ExpressionValue(op.Condition);
                        break;
                    default:
                        argValue = emptyArgValue;
                        break;
                }
                if (errorFound) continue;

                arguments.Add(macroDef.ArgumentNames[i], argValue);
            }
            if (errorFound) return;

            // --- Create a scope for the macro
            var macroScope = new SymbolScope
            {
                MacroArguments = arguments
            };
            _output.LocalScopes.Push(macroScope);

            // --- The macro name will serve as its starting label
            macroScope.Symbols.Add(macroDef.MacroName, new ExpressionValue(GetCurrentAssemblyAddress()));

            var lineIndex = macroDef.Section.FirstLine + 1;
            var lastLine = macroDef.Section.LastLine;

            // --- Create source info for the macro invocation
            var currentAddress = GetCurrentAssemblyAddress();
            _output.AddToAddressMap(macroStmt.FileIndex, macroStmt.SourceLine, currentAddress);
            _output.SourceMap[currentAddress] = (macroStmt.FileIndex, macroStmt.SourceLine);

            // --- We store the original source file information to
            // --- assign it later with the re-parsed macro code
            var sourceInfo = new List<(int fileIndex, int line)>();

            // --- Setup the macro source
            var macroSource = new StringBuilder(4096);
            while (lineIndex < lastLine)
            {
                // --- Replace all macro arguments by their actual value
                var curLine = allLines[lineIndex];
                var lineText = curLine.SourceText;
                var matches = MacroParamRegex.Matches(lineText);
                foreach (Match match in matches)
                {
                    var toReplace = match.Groups[0].Value;
                    var argName = match.Groups[1].Value;
                    if (!arguments.TryGetValue(argName, out var argValue))
                    {
                        continue;
                    }
                    lineText = lineText.Replace(toReplace, argValue.AsString());
                }

                // --- Store the source information for the currently processed macro line
                var newLines = lineText.Split(new[] {"\r\n"}, StringSplitOptions.None).Length;
                for (var i = 0; i < newLines; i++)
                {
                    sourceInfo.Add((curLine.FileIndex, curLine.SourceLine));
                }
                macroSource.AppendLine(lineText);
                lineIndex++;
            }

            // --- Now we have the source text to compile
            var inputStream = new AntlrInputStream(macroSource.ToString());
            var lexer = new Z80AsmLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80AsmParser(tokenStream);
            var context = parser.compileUnit();
            var visitor = new Z80AsmVisitor
            {
                MacroParsingPhase = true
            };
            visitor.Visit(context);
            var visitedLines = visitor.Compilation;

            // --- Store any tasks defined by the user
            StoreTasks(_output.SourceItem, visitedLines.Lines);

            // --- Collect syntax errors
            foreach (var error in parser.SyntaxErrors)
            {
                // --- Translate the syntax error location
                if (error.SourceLine > 0 && error.SourceLine < sourceInfo.Count)
                {
                    var errorLocation = sourceInfo[error.SourceLine - 1];
                    error.SourceLine = errorLocation.line;
                    ReportError(_output.SourceFileList[errorLocation.fileIndex], error);
                }
                else
                {
                    ReportError(_output.SourceItem, error);
                }
                errorFound = true;
            }

            if (errorFound)
            {
                // --- Stop compilation, if macro contains error
                return;
            }

            // --- Set the source line information
            for (var i = 0; i < sourceInfo.Count; i++)
            {
                if (i < visitedLines.Lines.Count)
                {
                    var lineInfo = sourceInfo[i];
                    var line = visitedLines.Lines[i];
                    line.FileIndex = lineInfo.fileIndex;
                    line.SourceLine = lineInfo.line;
                }
            }

            // --- Now, emit the compiled lines
            lineIndex = 0;
            while (lineIndex < visitedLines.Lines.Count)
            {
                var macroLine = visitedLines.Lines[lineIndex];
                EmitSingleLine(allLines, visitedLines.Lines, macroLine, ref lineIndex, true);

                // --- Next line
                lineIndex++;
            }

            // --- Add the end label to the local scope
            var endLabel = macroDef.EndLabel;
            if (endLabel != null)
            {
                // --- Add the end label to the macro scope
                var endLine = allLines[lastLine];
                if (SymbolExists(endLabel))
                {
                    ReportError(Errors.Z0040, endLine, endLabel);
                }
                else
                {
                    AddSymbol(endLabel, new ExpressionValue(GetCurrentAssemblyAddress()));
                }
            }

            // --- Clean up the hanging label
            OverflowLabelLine = null;

            // --- Fixup the symbols locally
            FixupSymbols(macroScope.Fixups, macroScope.Symbols, false);

            // --- Remove the macro's scope
            _output.LocalScopes.Pop();
        }

        #endregion

        #region Pragma processing

        /// <summary>
        /// Applies a pragma in the assembly source code
        /// </summary>
        /// <param name="pragmaLine">Assembly line that represents a pragma</param>
        /// <param name="label">Label to use with the pragme</param>
        private void ApplyPragma(PragmaBase pragmaLine, string label)
        {
            switch (pragmaLine)
            {
                case OrgPragma orgPragma:
                    ProcessOrgPragma(orgPragma, label);
                    return;
                case EntPragma entPragma:
                    ProcessEntPragma(entPragma);
                    return;
                case XentPragma xentPragma:
                    ProcessXentPragma(xentPragma);
                    return;
                case DispPragma dispPragma:
                    ProcessDispPragma(dispPragma);
                    return;
                case EquPragma equPragma:
                    ProcessEquPragma(equPragma, label);
                    return;
                case VarPragma varPragma:
                    ProcessVarPragma(varPragma, label);
                    return;
                case SkipPragma skipPragma:
                    ProcessSkipPragma(skipPragma);
                    return;
                case DefbPragma defbPragma:
                    ProcessDefbPragma(defbPragma);
                    return;
                case DefwPragma defwPragma:
                    ProcessDefwPragma(defwPragma);
                    return;
                case DefmnPragma defmnPragma:
                    ProcessDefmnPragma(defmnPragma);
                    break;
                case DefhPragma defhPragma:
                    ProcessDefhPragma(defhPragma);
                    break;
                case DefsPragma defsPragma:
                    ProcessDefsPragma(defsPragma);
                    break;
                case FillbPragma fillbPragma:
                    ProcessFillbPragma(fillbPragma);
                    break;
                case FillwPragma fillwPragma:
                    ProcessFillwPragma(fillwPragma);
                    break;
                case AlignPragma alignPragma:
                    ProcessAlignPragma(alignPragma);
                    break;
                case TracePragma tracePragma:
                    ProcessTracePragma(tracePragma);
                    break;
                case RndSeedPragma rndSeedPragma:
                    ProcessRndSeedPragma(rndSeedPragma);
                    break;
                case DefgPragma defgPragma:
                    ProcessDefgPragma(defgPragma);
                    break;
                case ErrorPragma errorPragma:
                    ProcessErrorPragma(errorPragma);
                    break;
                case IncludeBinPragma incBinPragma:
                    ProcessIncBinPragma(incBinPragma);
                    break;
            }
        }

        /// <summary>
        /// Processes the ORG pragma
        /// </summary>
        /// <param name="pragma">Assembly line of ORG pragma</param>
        /// <param name="label">Label to use</param>
        private void ProcessOrgPragma(OrgPragma pragma, string label)
        {
            var value = EvalImmediate(pragma, pragma.Expr);
            if (!value.IsValid) return;

            EnsureCodeSegment();
            if (CurrentSegment.CurrentOffset != 0)
            {
                // --- There is already code emitted for the current segment
                CurrentSegment = new BinarySegment
                {
                    StartAddress = value.Value
                };
                _output.Segments.Add(CurrentSegment);
            }
            else
            {
                CurrentSegment.StartAddress = value.Value;
            }

            if (label == null)
            {
                return;
            }

            // --- There is a label, set its value
            if (SymbolExists(label))
            {
                ReportError(Errors.Z0040, pragma, label);
                return;
            }
            AddSymbol(label, value);
        }

        /// <summary>
        /// Processes the ENT pragma
        /// </summary>
        /// <param name="pragma">Assembly line of ENT pragma</param>
        private void ProcessEntPragma(EntPragma pragma)
        {
            if (!IsInGlobalScope && ShouldReportErrorInCurrentScope(Errors.Z0407))
            {
                ReportError(Errors.Z0407, pragma, "ENT");
            }
            var value = Eval(pragma, pragma.Expr);
            if (value.IsNonEvaluated)
            {
                RecordFixup(pragma, FixupType.Ent, pragma.Expr);
                return;
            }
            _output.EntryAddress = value.Value;
        }

        /// <summary>
        /// Processes the XENT pragma
        /// </summary>
        /// <param name="pragma">Assembly line of XENT pragma</param>
        private void ProcessXentPragma(XentPragma pragma)
        {
            if (!IsInGlobalScope && ShouldReportErrorInCurrentScope(Errors.Z0407))
            {
                ReportError(Errors.Z0407, pragma, "XENT");
            }
            var value = Eval(pragma, pragma.Expr);
            if (value.IsNonEvaluated)
            {
                RecordFixup(pragma, FixupType.Xent, pragma.Expr);
                return;
            }
            _output.ExportEntryAddress = value.Value;
        }

        /// <summary>
        /// Processes the DISP pragma
        /// </summary>
        /// <param name="pragma">Assembly line of DISP pragma</param>
        private void ProcessDispPragma(DispPragma pragma)
        {
            var value = EvalImmediate(pragma, pragma.Expr);
            if (!value.IsValid) return;

            EnsureCodeSegment();
            CurrentSegment.Displacement = value.Value;
        }

        /// <summary>
        /// Processes the EQU pragma
        /// </summary>
        /// <param name="pragma">Assembly line of EQU pragma</param>
        /// <param name="label">Label to use</param>
        private void ProcessEquPragma(EquPragma pragma, string label)
        {
            if (label == null)
            {
                ReportError(Errors.Z0082, pragma);
                return;
            }
            if (SymbolExists(label))
            {
                ReportError(Errors.Z0040, pragma, label);
                return;
            }

            var value = Eval(pragma, pragma.Expr);
            if (value.IsNonEvaluated)
            {
                RecordFixup(pragma, FixupType.Equ, pragma.Expr, label);
            }
            else
            {
                AddSymbol(label, value);
            }
        }

        /// <summary>
        /// Processes the VAR pragma
        /// </summary>
        /// <param name="pragma">Assembly line of VAR pragma</param>
        /// <param name="label">Label to use</param>
        private void ProcessVarPragma(VarPragma pragma, string label)
        {
            if (label == null)
            {
                ReportError(Errors.Z0086, pragma);
                return;
            }

            var value = EvalImmediate(pragma, pragma.Expr);
            if (!value.IsValid) return;

            // --- Allow reusing a symbol already declared
            if (SymbolExists(label))
            {
                ReportError(Errors.Z0087, pragma);
                return;
            }
            SetVariable(label, value);
        }

        /// <summary>
        /// Processes the SKIP pragma
        /// </summary>
        /// <param name="pragma">Assembly line of SKIP pragma</param>
        private void ProcessSkipPragma(SkipPragma pragma)
        {
            var skipAddr = EvalImmediate(pragma, pragma.Expr);
            if (!skipAddr.IsValid) return;

            var currentAddr = GetCurrentAssemblyAddress();
            if (skipAddr.Value < currentAddr)
            {
                ReportError(Errors.Z0081, pragma, $"{skipAddr:X4}", $"{currentAddr:X4}");
                return;
            }
            var fillByte = 0xff;
            if (pragma.Fill != null)
            {
                var fillValue = EvalImmediate(pragma, pragma.Fill);
                if (fillValue == null) return;
                fillByte = fillValue.Value;
            }

            while (currentAddr < skipAddr.Value)
            {
                EmitByte((byte)fillByte);
                currentAddr++;
            }
        }

        /// <summary>
        /// Processes the DEFB pragma
        /// </summary>
        /// <param name="pragma">Assembly line of DEFB pragma</param>
        private void ProcessDefbPragma(DefbPragma pragma)
        {
            foreach (var expr in pragma.Exprs)
            {
                var value = Eval(pragma, expr);
                if (value.IsValid)
                {
                    if (value.Type == ExpressionValueType.String)
                    {
                        ReportError(Errors.Z0305, pragma);
                        return;
                    }
                    EmitByte((byte) value.Value);
                }
                else if (value.IsNonEvaluated)
                {
                    RecordFixup(pragma, FixupType.Bit8, expr);
                    EmitByte(0x00);
                }
            }
        }

        /// <summary>
        /// Processes the DEFW pragma
        /// </summary>
        /// <param name="pragma">Assembly line of DEFW pragma</param>
        private void ProcessDefwPragma(DefwPragma pragma)
        {
            foreach (var expr in pragma.Exprs)
            {
                var value = Eval(pragma, expr);
                if (value.IsValid)
                {
                    if (value.Type == ExpressionValueType.String)
                    {
                        ReportError(Errors.Z0305, pragma);
                        return;
                    }
                    EmitWord(value.Value);
                }
                else if (value.IsNonEvaluated)
                {
                    RecordFixup(pragma, FixupType.Bit16, expr);
                    EmitWord(0x0000);
                }
            }
        }

        /// <summary>
        /// Processes the DEFN pragma
        /// </summary>
        /// <param name="pragma">Assembly line of DEFN pragma</param>
        // ReSharper disable once UnusedParameter.Local
        private void ProcessDefmnPragma(DefmnPragma pragma)
        {
            var message = Eval(pragma, pragma.Message);
            if (message.IsValid && message.Type != ExpressionValueType.String)
            {
                ReportError(Errors.Z0091, pragma);
            }
            var bytes = SpectrumStringToBytes(message.AsString());
            foreach (var msgByte in bytes)
            {
                EmitByte(msgByte);
            }
            if (pragma.NullTerminator)
            {
                EmitByte(0x00);
            }
        }

        /// <summary>
        /// Processes the DEFH pragma
        /// </summary>
        /// <param name="pragma">Assembly line of DEFH pragma</param>
        // ReSharper disable once UnusedParameter.Local
        private void ProcessDefhPragma(DefhPragma pragma)
        {
            var byteVector = Eval(pragma, pragma.ByteVector);
            if (byteVector.IsValid && byteVector.Type != ExpressionValueType.String)
            {
                ReportError(Errors.Z0093, pragma);
                return;
            }

            // --- Check if the byte vector is valid
            var bytesString = byteVector.AsString();
            if (bytesString.Length % 2 != 0)
            {
                ReportError(Errors.Z0094, pragma);
                return;
            }

            // --- Convert the byte vector
            try
            {
                var bytes = Enumerable.Range(0, bytesString.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(bytesString.Substring(x, 2), 16))
                    .ToArray();
                foreach (var msgByte in bytes)
                {
                    EmitByte(msgByte);
                }
            }
            catch (Exception)
            {
                ReportError(Errors.Z0094, pragma);
            }
        }

        /// <summary>
        /// Processes the DEFS pragma
        /// </summary>
        /// <param name="pragma">Assembly line of DEFS pragma</param>
        private void ProcessDefsPragma(DefsPragma pragma)
        {
            var count = Eval(pragma, pragma.Expression);
            if (!count.IsValid)
            {
                ReportError(Errors.Z0201, pragma, ExpressionNode.SymbolErrors);
                return;
            }
            for (var i = 0; i < count.Value; i++)
            {
                EmitByte(0x00);
            }
        }

        /// <summary>
        /// Processes the FILLB pragma
        /// </summary>
        /// <param name="pragma">Assembly line of FILLB pragma</param>
        private void ProcessFillbPragma(FillbPragma pragma)
        {
            var count = Eval(pragma, pragma.Count);
            if (!count.IsValid)
            {
                ReportError(Errors.Z0201, pragma, ExpressionNode.SymbolErrors);
            }
            var value = Eval(pragma, pragma.Expression);
            if (!value.IsValid)
            {
                ReportError(Errors.Z0201, pragma, ExpressionNode.SymbolErrors);
            }
            if (!count.IsValid || !value.IsValid)
            {
                return;
            }

            for (var i = 0; i < count.Value; i++)
            {
                EmitByte(value.AsByte());
            }
        }

        /// <summary>
        /// Processes the FILLW pragma
        /// </summary>
        /// <param name="pragma">Assembly line of FILLW pragma</param>
        private void ProcessFillwPragma(FillwPragma pragma)
        {
            var count = Eval(pragma, pragma.Count);
            var value = Eval(pragma, pragma.Expression);
            if (!count.IsValid || !value.IsValid)
            {
                ReportError(Errors.Z0201, pragma, ExpressionNode.SymbolErrors);
                return;
            }

            for (var i = 0; i < count.Value; i++)
            {
                EmitWord(value.AsWord());
            }
        }

        /// <summary>
        /// Processes the MODEL pragma
        /// </summary>
        /// <param name="pragma">Assembly line of MODEL pragma</param>
        private void ProcessModelPragma(ModelPragma pragma)
        {
            if (_output.ModelType != null)
            {
                ReportError(Errors.Z0088, pragma);
                return;
            }

            SpectrumModelType modelType;
            switch (pragma.Model.ToUpper())
            {
                case "SPECTRUM48":
                    modelType = SpectrumModelType.Spectrum48;
                    break;
                case "SPECTRUM128":
                    modelType = SpectrumModelType.Spectrum128;
                    break;
                case "SPECTRUMP3":
                    modelType = SpectrumModelType.SpectrumP3;
                    break;
                case "NEXT":
                    modelType = SpectrumModelType.Next;
                    break;
                default:
                    ReportError(Errors.Z0089, pragma);
                    return;
            }

            _output.ModelType = modelType;
        }

        /// <summary>
        /// Processes the ALIGN pragma
        /// </summary>
        /// <param name="pragma">Assembly line of ALIGN pragma</param>
        private void ProcessAlignPragma(AlignPragma pragma)
        {
            var alignment = 0x0100;
            if (pragma.Expr != null)
            {
                var alignValue = EvalImmediate(pragma, pragma.Expr);
                if (!alignValue.IsValid) return;
                alignment = alignValue.Value;
                if (alignment < 1 || alignment > 0x4000)
                {
                    ReportError(Errors.Z0092, pragma, alignment);
                    return;
                }
            }

            var currentAddress = GetCurrentAssemblyAddress();
            var newAddress = currentAddress % alignment == 0
                                 ? currentAddress
                                 : (currentAddress / alignment + 1) * alignment;
            for (var i = currentAddress; i < newAddress; i++)
            {
                EmitByte(0x00);
            }
        }

        /// <summary>
        /// Processes the TRACE pragma
        /// </summary>
        /// <param name="pragma">Assembly line of TRACE pragma</param>
        private void ProcessTracePragma(TracePragma pragma)
        {
            var message = new StringBuilder(1024);
            foreach (var expr in pragma.Exprs)
            {
                var exprValue = EvalImmediate(pragma, expr);
                if (!exprValue.IsValid) return;

                switch (exprValue.Type)
                {
                    case ExpressionValueType.Bool:
                        message.Append(exprValue.AsBool());
                        break;
                    case ExpressionValueType.Integer:
                        var intValue = exprValue.AsLong();
                        if (pragma.IsHex)
                        {
                            var valueStr = intValue > 0x10000
                                ? $"{intValue:X8}"
                                : $"{intValue:X4}";
                            message.Append(valueStr);
                        }
                        else
                        {
                            message.Append(intValue);
                        }
                        break;
                    case ExpressionValueType.Real:
                        message.Append(exprValue.AsReal());
                        break;
                    case ExpressionValueType.String:
                        if (pragma.IsHex)
                        {
                            var bytes = SpectrumStringToBytes(exprValue.AsString());
                            foreach (var msgByte in bytes)
                            {
                                message.Append($"{msgByte:X2}");
                            }
                        }
                        else
                        {
                            message.Append(exprValue.AsString());
                        }
                        break;
                }
            }
            OnAssemblerMessageCreated(message.ToString());
        }

        /// <summary>
        /// Processes the RNDSEED pragma
        /// </summary>
        /// <param name="pragma">Assembly line of RNDSEED pragma</param>
        private void ProcessRndSeedPragma(RndSeedPragma pragma)
        {
            if (pragma.Expr == null)
            {
                FunctionInvocationNode.SetRandomSeed(null);
                return;
            }

            var seedValue = EvalImmediate(pragma, pragma.Expr);
            FunctionInvocationNode.SetRandomSeed(seedValue);
        }

        /// <summary>
        /// Processes the DEFG pragma
        /// </summary>
        /// <param name="pragma">Assembly line of DEFG pragma</param>
        private void ProcessDefgPragma(DefgPragma pragma)
        {
            // --- Obtain and check the DEFG pattern expression
            var value = EvalImmediate(pragma, pragma.Expr);
            if (!value.IsValid) return;

            if (value.Type != ExpressionValueType.String)
            {
                ReportError(Errors.Z0306, pragma);
                return;
            }

            var pattern = value.AsString().Trim();
            if (string.IsNullOrEmpty(pattern))
            {
                ReportError(Errors.Z0307, pragma);
                return;
            }

            // --- Go through all values
            var alignToLeft = true;
            if (pattern[0] == '<')
            {
                pattern = pattern.Substring(1);
            }
            else if (pattern[0] == '>')
            {
                alignToLeft = false;
                pattern = pattern.Substring(1);
            }
            pattern = pattern.Replace(" ", "");
            if (pattern.Length == 0) return;

            var remainingBits = pattern.Length % 8;
            if (remainingBits > 0)
            {
                pattern = alignToLeft 
                    ? pattern.PadRight(pattern.Length + 8 - remainingBits, '_') 
                    : pattern.PadLeft(pattern.Length + 8 - remainingBits, '_');
            }

            var bitPattern = 0x00;
            for (var i = 0; i < pattern.Length; i++)
            {
                // --- Calculate the bit pattern
                switch (pattern[i])
                {
                    case '-':
                    case '.':
                    case '_':
                        bitPattern <<= 1;
                        break;
                    default:
                        bitPattern = (bitPattern << 1) | 1;
                        break;
                }
                if ((i + 1) % 8 != 0) continue;

                // --- Emit a full byte
                EmitByte((byte)bitPattern);
                bitPattern = 0x00;
            }
        }

        /// <summary>
        /// Processes the ERROR pragma
        /// </summary>
        /// <param name="pragma">Assembly line of DEFG pragma</param>
        private void ProcessErrorPragma(ErrorPragma pragma)
        {
            var errorValue = EvalImmediate(pragma, pragma.Expr);
            if (!errorValue.IsValid) return;
            ReportError(Errors.Z0500, pragma, errorValue.AsString());
        }

        /// <summary>
        /// Processes the INCLUDEBIN pragma
        /// </summary>
        /// <param name="pragma">Assembly line of DEFG pragma</param>
        private void ProcessIncBinPragma(IncludeBinPragma pragma)
        {
            // --- Obtain the file name
            var fileNameValue = EvalImmediate(pragma, pragma.FileExpr);
            if (!fileNameValue.IsValid) return;

            if (fileNameValue.Type != ExpressionValueType.String)
            {
                ReportError(Errors.Z0306, pragma);
                return;
            }

            // --- Obtain optional offset
            var offset = 0;
            if (pragma.OffsetExpr != null)
            {
                var offsValue = EvalImmediate(pragma, pragma.OffsetExpr);
                if (offsValue.Type != ExpressionValueType.Integer)
                {
                    ReportError(Errors.Z0308, pragma);
                    return;
                }
                offset = (int)offsValue.AsLong();
                if (offset < 0)
                {
                    ReportError(Errors.Z0424, pragma);
                    return;
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
                    return;
                }
                length = (int)lengthValue.AsLong();
                if (length < 0)
                {
                    ReportError(Errors.Z0425, pragma);
                    return;
                }
            }

            // --- Read the binary file
            var currentSourceFile = _output.SourceFileList[pragma.FileIndex];
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
                ReportError(Errors.Z0423, pragma, e.Message);
                return;
            }

            // --- Check content segment
            if (offset >= contents.Length)
            {
                ReportError(Errors.Z0424, pragma);
                return;
            }

            if (length == null)
            {
                length = contents.Length - offset;
            }

            // --- Check length
            if (offset + length > contents.Length)
            {
                ReportError(Errors.Z0425, pragma);
                return;
            }

            // --- Check for too long binary segment
            if (GetCurrentAssemblyAddress() + length >= 0x10000)
            {
                ReportError(Errors.Z0426, pragma);
                return;
            }

            // --- Everything is ok, emit the binary data
            for (var i = offset; i < offset + length; i++)
            {
                EmitByte(contents[i]);
            }
        }

        #endregion

        #region Operations code emitting

        /// <summary>
        /// Emits code for the specified operation
        /// </summary>
        /// <param name="opLine">Operation to emit the code for</param>
        private void EmitAssemblyOperationCode(SourceLineBase opLine)
        {
            // --- This line might be a single label
            if (opLine is NoInstructionLine)
            {
                return;
            }

            // --- Handle the trivial operations (with simple mnemonics, like
            // --- nop, ldir, scf, etc.
            var trivOpLine = opLine as TrivialOperation;
            if (trivOpLine != null)
            {
                if (trivOpLine is TrivialNextOperation && _output.ModelType != SpectrumModelType.Next)
                {
                    ReportError(Errors.Z0102, opLine);
                    return;
                }
                EmitTrivialOperation(trivOpLine);
                return;
            }

            // --- Handle compound operations
            var compoundOpLine = opLine as CompoundOperation;
            if (compoundOpLine != null)
            {
                EmitCompoundOperation(compoundOpLine);
                return;
            }

            // --- Any other case means an internal error
            ReportError(Errors.Z0083, opLine, opLine.GetType().FullName);
        }

        /// <summary>
        /// Emits code for trivial operations
        /// </summary>
        /// <param name="opLine">
        /// Assembly line that denotes a trivial Z80 operation.
        /// </param>
        private void EmitTrivialOperation(OperationBase opLine)
        {
            EmitOperationWithLookup(s_TrivialOpBytes, opLine.Mnemonic, opLine);
        }

        /// <summary>
        /// Emits a compound operation
        /// </summary>
        /// <param name="compoundOpLine">
        /// Assembly line that denotes a compound Z80 operation.
        /// </param>
        private void EmitCompoundOperation(CompoundOperation compoundOpLine)
        {
            CompoundOperationDescriptor rules;
            if (!_compoundOpTable.TryGetValue(compoundOpLine.Mnemonic, out rules))
            {
                ReportError(Errors.Z0084, compoundOpLine, compoundOpLine.Mnemonic);
                return;
            }

            // --- Get the operand types
            var op1Type = compoundOpLine.Operand?.Type ?? OperandType.None;
            var op2Type = compoundOpLine.Operand2?.Type ?? OperandType.None;

            var isProcessable = true;

            // --- Check inclusive rules
            if (rules.Allow != null)
            {
                isProcessable = rules.Allow.Any(r => r.FirstOp == op1Type && r.SecondOp == op2Type);
            }

            // --- We applied operands according to rules
            if (isProcessable)
            {
                rules.ProcessAction(this, compoundOpLine);
                return;
            }

            // --- This operations is invalid. Report it with the proper message.
            ReportError(Errors.Z0001, compoundOpLine, compoundOpLine.Mnemonic);
        }

        /// <summary>
        /// LD operations
        /// </summary>
        private static void ProcessLd(Z80Assembler asm, CompoundOperation op)
        {
            // --- Destination is an 8-bit register
            if (op.Operand.Type == OperandType.Reg8)
            {
                var destReg = op.Operand.Register;
                var destRegIdx = s_Reg8Order.IndexOf(destReg);
                var sourceReg = op.Operand2.Register;

                if (op.Operand2.Type == OperandType.Reg8)
                {
                    // ld '8bitreg','8bitReg'
                    asm.EmitByte((byte) (0x40 + (destRegIdx << 3) + s_Reg8Order.IndexOf(sourceReg)));
                    return;
                }

                if (op.Operand2.Type == OperandType.RegIndirect)
                {
                    if (sourceReg == "(BC)")
                    {
                        if (destReg == "A")
                        {
                            // ld a,(bc)
                            asm.EmitByte(0x0A);
                            return;
                        }
                    }
                    else if (sourceReg == "(DE)")
                    {
                        if (destReg == "A")
                        {
                            // ld a,(de)
                            asm.EmitByte(0x1A);
                            return;
                        }
                    }
                    else if (sourceReg == "(HL)")
                    {
                        // ld '8bitreg',(hl)
                        asm.EmitByte((byte) (0x46 + (destRegIdx << 3)));
                        return;
                    }
                    asm.ReportInvalidLoadOp(op, destReg, sourceReg);
                    return;
                }

                if (op.Operand2.Type == OperandType.Reg8Spec)
                {
                    // ld a,i and ld a,r
                    if (op.Operand.Register != "A")
                    {
                        asm.ReportInvalidLoadOp(op, destReg, sourceReg);
                        return;
                    }
                    asm.EmitDoubleByte(sourceReg == "R" ? 0xED5F : 0xED57);
                    return;
                }

                if (op.Operand2.Type == OperandType.Reg8Idx)
                {
                    // ld reg,'xh|xl|yh|yl'
                    // --- Destination must be one of the indexed 8-bit registers
                    if (destRegIdx >= 4 && destRegIdx <= 6)
                    {
                        // --- Deny invalid destination: h, l, (hl)
                        asm.ReportInvalidLoadOp(op, destReg, sourceReg);
                        return;
                    }
                    var opCode = sourceReg.Contains("X") ? 0xDD44 : 0xFD44;
                    asm.EmitDoubleByte(opCode + (destRegIdx << 3) + (sourceReg.EndsWith("H") ? 0 : 1));
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    // ld reg,expr
                    asm.EmitByte((byte)(0x06 + (destRegIdx << 3)));
                    asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit8);
                    return;
                }

                if (op.Operand2.Type == OperandType.MemIndirect)
                {
                    // ld a,(expr)
                    if (destReg != "A")
                    {
                        asm.ReportInvalidLoadOp(op, destReg, sourceReg);
                        return;
                    }
                    asm.EmitByte(0x3A);
                    asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit16);
                    return;
                }

                if (op.Operand2.Type == OperandType.IndexedAddress)
                {
                    // --- ld '8-bit-reg', '(idxreg+disp)' operation
                    var opCode = (byte)(0x46 + (destRegIdx << 3));
                    asm.EmitIndexedOperation(op, op.Operand2, opCode);
                    return;
                }
            }

            // --- Destination is an 8-bit index register
            if (op.Operand.Type == OperandType.Reg8Idx)
            {
                var destReg = op.Operand.Register;
                var sourceReg = op.Operand2.Register;
                if (op.Operand2.Type == OperandType.Reg8)
                {
                    // ld 'xh|xl|yh|yl', reg
                    var sourceRegIdx = s_Reg8Order.IndexOf(sourceReg);

                    // --- Destination must be one of the indexed 8-bit registers
                    if (sourceRegIdx >= 4 && sourceRegIdx <= 6)
                    {
                        // --- Deny invalid destination: h, l, (hl)
                        asm.ReportInvalidLoadOp(op, destReg, sourceReg);
                        return;
                    }
                    var opBytes = destReg.Contains("X") ? 0xDD60 : 0xFD60;
                    asm.EmitDoubleByte(opBytes + (destReg.EndsWith("H") ? 0 : 8) + sourceRegIdx);
                    return;
                }

                if (op.Operand2.Type == OperandType.Reg8Idx)
                {
                    // ld 'xh|xl|yh|yl', 'xh|xl|yh|yl'
                    if (sourceReg[0] != destReg[0])
                    {
                        asm.ReportInvalidLoadOp(op, destReg, sourceReg);
                        return;
                    }

                    var xopBytes = destReg.Contains("X") ? 0xDD64 : 0xFD64;
                    asm.EmitDoubleByte(xopBytes + (destReg.EndsWith("H") ? 0 : 8)
                                       + (sourceReg.EndsWith("H") ? 0 : 1));
                    return;
                }

                // ld 'xh|xl|yh|yl',expr
                var opCode = destReg.Contains("X") ? 0xDD26 : 0xFD26;
                opCode += destReg.EndsWith("H") ? 0 : 8;
                asm.EmitDoubleByte(opCode);
                asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit8);
                return;
            }

            // --- Destination is I or A
            if (op.Operand.Type == OperandType.Reg8Spec)
            {
                // ld i,a and ld r,a
                if (op.Operand2.Register != "A")
                {
                    asm.ReportInvalidLoadOp(op, op.Operand.Register, op.Operand2.Register);
                    return;
                }
                asm.EmitDoubleByte(op.Operand.Register == "R" ? 0xED4F : 0xED47);
                return;
            }

            // --- Destination is memory through a 16-bit register
            if (op.Operand.Type == OperandType.RegIndirect)
            {
                var destReg = op.Operand.Register;
                if (op.Operand2.Type == OperandType.Reg8)
                {
                    var sourceReg = op.Operand2.Register;
                    if (destReg == "(BC)")
                    {
                        if (sourceReg == "A")
                        {
                            // ld (bc),a
                            asm.EmitByte(0x02);
                            return;
                        }
                    }
                    else if (destReg == "(DE)")
                    {
                        if (sourceReg == "A")
                        {
                            // ld (de),a
                            asm.EmitByte(0x12);
                            return;
                        }
                    }
                    else if (destReg == "(HL)")
                    {
                        // ld (hl),'8BitReg'
                        asm.EmitByte((byte)(0x70 + s_Reg8Order.IndexOf(sourceReg)));
                        return;
                    }
                    asm.ReportInvalidLoadOp(op, destReg, sourceReg);
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    if (op.Operand.Register != "(HL)")
                    {
                        asm.ReportInvalidLoadOp(op, destReg, "<expression>");
                        return;
                    }
                    // ld (hl),expr
                    asm.EmitByte(0x36);
                    asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit8);
                    return;
                }

                return;
            }

            // --- Destination is a memory address
            if (op.Operand.Type == OperandType.MemIndirect)
            {
                if (op.Operand2.Type == OperandType.Reg8)
                {
                    if (op.Operand2.Register != "A")
                    {
                        asm.ReportInvalidLoadOp(op, "(<expression>)", op.Operand2.Register);
                        return;
                    }
                    asm.EmitByte(0x32);
                }
                else if (op.Operand2.Type == OperandType.Reg16)
                {
                    // ld (expr),reg16
                    var sourceReg = op.Operand2.Register;
                    var opCode = 0x22;
                    if (sourceReg == "BC")
                    {
                        opCode = 0xED43;
                    }
                    else if (sourceReg == "DE")
                    {
                        opCode = 0xED53;
                    }
                    else if (sourceReg == "SP")
                    {
                        opCode = 0xED73;
                    }
                    asm.EmitDoubleByte(opCode);
                }
                else if (op.Operand2.Type == OperandType.Reg16Idx)
                {
                    asm.EmitDoubleByte(op.Operand2.Register == "IX" ? 0xDD22 : 0xFD22);
                }
                asm.EmitumNumericExpression(op, op.Operand.Expression, FixupType.Bit16);
                return;
            }

            // --- Destination is a 16-bit register
            if (op.Operand.Type == OperandType.Reg16)
            {
                var destReg = op.Operand.Register;
                if (op.Operand2.Type == OperandType.MemIndirect)
                {
                    // ld reg16,(expr)
                    var opCode = 0x2A;
                    if (destReg == "BC")
                    {
                        opCode = 0xED4B;
                    }
                    else if (destReg == "DE")
                    {
                        opCode = 0xED5B;
                    }
                    else if (destReg == "SP")
                    {
                        opCode = 0xED7B;
                    }
                    asm.EmitDoubleByte(opCode);
                    asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit16);
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    // ld reg16,expr
                    var sourceRegIdx = s_Reg16Order.IndexOf(op.Operand.Register);
                    asm.EmitByte((byte)(0x01 + (sourceRegIdx << 4)));
                    asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit16);
                    return;
                }

                // --- From now on, the destination can be only SP

                if (op.Operand.Register != "SP")
                {
                    asm.ReportInvalidLoadOp(op, op.Operand.Register, op.Operand2.Register);
                    return;
                }

                var spCode = 0xF9;
                if (op.Operand2.Register == "IX")
                {
                    spCode = 0xDDF9;
                }
                else if (op.Operand2.Register == "IY")
                {
                    spCode = 0xFDF9;
                }
                else if (op.Operand2.Register != "HL")
                {
                    asm.ReportInvalidLoadOp(op, op.Operand.Register, op.Operand2.Register);
                    return;
                }
                asm.EmitDoubleByte(spCode);
                return;
            }

            // --- Destination is a 16-bit index register
            if (op.Operand.Type == OperandType.Reg16Idx)
            {
                var sourceReg = op.Operand.Register;
                if (op.Operand2.Type == OperandType.MemIndirect)
                {
                    // ld 'ix|iy',(expr)
                    asm.EmitDoubleByte(sourceReg == "IX" ? 0xDD2A : 0xFD2A);
                    asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit16);
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    // ld 'ix|iy',expr
                    asm.EmitDoubleByte(op.Operand.Register == "IX" ? 0xDD21 : 0xFD21);
                    asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit16);
                    return;
                }
                return;
            }

            // --- Destination is an indexed memory address
            if (op.Operand.Type == OperandType.IndexedAddress)
            {

                if (op.Operand2.Type == OperandType.Reg8)
                {
                    // --- ld '(idxreg+disp)','8bitReg'
                    var opCode = (byte)(0x70 + s_Reg8Order.IndexOf(op.Operand2.Register));
                    asm.EmitIndexedOperation(op, op.Operand, opCode);
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    // --- ld '(idxreg+disp)','expr'
                    asm.EmitIndexedOperation(op, op.Operand, 0x36);
                    asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit8);
                }
            }
        }

        /// <summary>
        /// BIT, SET, RES operations
        /// </summary>
        private static void ProcessBit(Z80Assembler asm, CompoundOperation op)
        {
            byte opByte;
            switch (op.Mnemonic)
            {
                case "BIT":
                    opByte = 0x40;
                    break;
                case "RES":
                    opByte = 0x80;
                    break;
                default:
                    opByte = 0xC0;
                    break;
            }

            // --- Check the bit index
            var bitIndex = asm.EvalImmediate(op, op.Operand.Expression);
            if (!bitIndex.IsValid)
            {
                return;
            }
            if (bitIndex.AsLong() < 0 || bitIndex.AsLong() > 7)
            {
                asm.ReportError(Errors.Z0002, op, bitIndex);
                return;
            }

            if (op.Operand2.Type == OperandType.IndexedAddress)
            {
                if (op.Operand3 == null)
                {
                    opByte |= 0x06;
                }
                else if (op.Operand3.Type == OperandType.Reg8)
                {
                    opByte |= (byte)s_Reg8Order.IndexOf(op.Operand3.Register);
                }
                else
                {
                    asm.ReportError(Errors.Z0001, op, op.Mnemonic);
                }
                asm.EmitIndexedBitOperation(op, op.Operand2.Register, op.Operand2.Sign, op.Operand2.Expression, 
                    (byte)(opByte + (bitIndex.Value << 3)));
                return;
            }

            if (op.Operand2.Type == OperandType.Reg8)
            {
                opByte |= (byte)s_Reg8Order.IndexOf(op.Operand2.Register);
            }
            else if (op.Operand2.Type == OperandType.RegIndirect)
            {
                if (op.Operand2.Register != "(HL)")
                {
                    asm.ReportError(Errors.Z0004, op, op.Mnemonic, op.Operand2.Register);
                    return;
                }
                opByte |= 0x06;
            }
            asm.EmitBytes(0xCB, (byte)(opByte + (bitIndex.Value << 3)));
        }

        /// <summary>
        /// Shift and rotate operations
        /// </summary>
        private static void ProcessShiftRotate(Z80Assembler asm, CompoundOperation op)
        {
            var sOpByte = (byte)(8 * s_ShiftOpOrder.IndexOf(op.Mnemonic));
            if (op.Operand.Type == OperandType.IndexedAddress)
            {
                if (op.Operand2 == null)
                {
                    sOpByte |= 0x06;
                }
                else if (op.Operand2.Type == OperandType.Reg8)
                {
                    sOpByte |= (byte)s_Reg8Order.IndexOf(op.Operand2.Register);
                }
                asm.EmitIndexedBitOperation(op, op.Operand.Register, op.Operand.Sign, op.Operand.Expression, sOpByte);
                return;
            }

            if (op.Operand.Type == OperandType.Reg8)
            {
                sOpByte |= (byte) s_Reg8Order.IndexOf(op.Operand.Register);
            }
            else if (op.Operand.Type == OperandType.RegIndirect)
            {
                if (op.Operand.Register != "(HL)")
                {
                    asm.ReportError(Errors.Z0004, op, op.Mnemonic, op.Operand.Register);
                    return;
                }
                sOpByte |= 0x06;
            }
            asm.EmitBytes(0xCB, sOpByte);
        }

        /// <summary>
        /// OUT operations
        /// </summary>
        private static void ProcessOut(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand.Type == OperandType.MemIndirect)
            {
                if (op.Operand2.Register != "A")
                {
                    asm.ReportError(Errors.Z0005, op);
                    return;
                }

                asm.EmitByte(0xD3);
                asm.EmitumNumericExpression(op, op.Operand.Expression, FixupType.Bit8);
                return;
            }

            if (op.Operand.Type == OperandType.CPort)
            {
                if (op.Operand2.Type == OperandType.Reg8)
                {
                    asm.EmitOperationWithLookup(s_OutOpBytes, op.Operand2.Register, op);
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    var value = asm.EvalImmediate(op, op.Operand2.Expression);
                    if (!value.IsValid || value.Value != 0)
                    {
                        asm.ReportError(Errors.Z0006, op);
                        return;
                    }

                    // --- out (c),0
                    asm.EmitDoubleByte(0xED71);
                }
            }
        }

        /// <summary>
        /// IN operations
        /// </summary>
        private static void ProcessIn(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand.Type == OperandType.Reg8)
            {
                if (op.Operand2.Type == OperandType.MemIndirect)
                {
                    if (op.Operand.Register != "A")
                    {
                        asm.ReportError(Errors.Z0005, op);
                        return;
                    }

                    // --- in a,(port)
                    asm.EmitByte(0xDB);
                    asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit8);
                    return;
                }
            }

            if (op.Operand.Type == OperandType.CPort)
            {
                // --- in (c)
                asm.EmitDoubleByte(0xED70);
                return;
            }

            // --- in reg,(c)
            asm.EmitOperationWithLookup(s_InOpBytes, op.Operand.Register, op);
        }

        /// <summary>
        /// ALU operations: SUB, AND, XOR, OR, CP
        /// </summary>
        private static void ProcessAlu2(Z80Assembler asm, CompoundOperation op)
        {
            var operand = op.Operand;
            var opType = op.Operand.Type;
            var opReg = op.Operand.Register;

            // --- Check for alternative syntax (A register as the first operand)
            if (op.Operand2 != null)
            {
                if (opType != OperandType.Reg8 || opReg != "A")
                {
                    asm.ReportError(Errors.Z0023, op, op.Mnemonic);
                    return;
                }
                operand = op.Operand2;
                opType = op.Operand2.Type;
                opReg = op.Operand2.Register;
            }

            var aluIdx = (byte)s_AluOpOrder.IndexOf(op.Mnemonic);
            if (opType == OperandType.Reg8)
            {
                var regIdx = s_Reg8Order.IndexOf(opReg);
                asm.EmitByte((byte)(0x80 + (aluIdx << 3) + regIdx));
                return;
            }

            if (opType == OperandType.RegIndirect)
            {
                if (opReg != "(HL)")
                {
                    asm.ReportError(Errors.Z0004, op, op.Mnemonic, opReg);
                    return;
                }
                asm.EmitByte((byte)(0x86 + (aluIdx << 3)));
                return;
            }

            if (opType == OperandType.Reg8Idx)
            {
                asm.EmitByte((byte)(opReg.Contains("X") ? 0xDD : 0xFD));
                asm.EmitByte((byte)(0x80 + (aluIdx << 3) + (opReg.EndsWith("H") ? 4 : 5)));
                return;
            }

            if (opType == OperandType.Expr)
            {
                asm.EmitByte((byte)(0xC6 + (aluIdx << 3)));
                asm.EmitumNumericExpression(op, operand.Expression, FixupType.Bit8);
                return;
            }

            if (opType == OperandType.IndexedAddress)
            {
                var opByte = (byte)(0x86 + (aluIdx << 3));
                asm.EmitIndexedOperation(op, operand, opByte);
            }
        }

        /// <summary>
        /// ALU operations: ADD, ADC, SBC
        /// </summary>
        private static void ProcessAlu1(Z80Assembler asm, CompoundOperation op)
        {
            var aluIdx = (byte)s_AluOpOrder.IndexOf(op.Mnemonic);
            if (op.Operand.Type == OperandType.Reg8)
            {
                if (op.Operand.Register != "A")
                {
                    asm.ReportError(Errors.Z0007, op, op.Mnemonic);
                    return;
                }

                if (op.Operand2.Type == OperandType.Reg8)
                {
                    var regIdx = s_Reg8Order.IndexOf(op.Operand2.Register);
                    asm.EmitByte((byte) (0x80 + (aluIdx << 3) + regIdx));
                    return;
                }

                if (op.Operand2.Type == OperandType.RegIndirect)
                {
                    if (op.Operand2.Register != "(HL)")
                    {
                        asm.ReportError(Errors.Z0008, op, op.Mnemonic, op.Operand2.Register);
                        return;
                    }
                    asm.EmitByte((byte)(0x86 + (aluIdx << 3)));
                    return;
                }

                if (op.Operand2.Type == OperandType.Reg8Idx)
                {
                    asm.EmitByte((byte)(op.Operand2.Register.Contains("X") ? 0xDD : 0xFD));
                    asm.EmitByte((byte)(0x80 + (aluIdx << 3) + (op.Operand2.Register.EndsWith("H") ? 4 : 5)));
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    asm.EmitByte((byte)(0xC6 + (aluIdx << 3)));
                    asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit8);
                    return;
                }

                if (op.Operand2.Type == OperandType.IndexedAddress)
                {
                    var opByte = (byte)(0x86 + (aluIdx << 3));
                    asm.EmitIndexedOperation(op, op.Operand2, opByte);
                    return;
                }
            }

            if (op.Operand.Type == OperandType.Reg16)
            {
                if (op.Operand2.Type == OperandType.Reg16)
                {
                    if (op.Operand.Register != "HL")
                    {
                        asm.ReportError(Errors.Z0009, op, op.Mnemonic, op.Operand.Register);
                        return;
                    }

                    // --- 16-bit register ALU operations
                    int opCodeBase;
                    switch (op.Mnemonic)
                    {
                        case "ADD":
                            opCodeBase = 0x09;
                            break;
                        case "ADC":
                            opCodeBase = 0xED4A;
                            break;
                        default:
                            opCodeBase = 0xED42;
                            break;
                    }
                    asm.EmitDoubleByte(opCodeBase + (s_Reg16Order.IndexOf(op.Operand2.Register) << 4));
                    return;
                }

                // --- Spectrum Next extended opcodes
                if (op.Operand2.Type == OperandType.Reg8)
                {
                    if (asm._output.ModelType != SpectrumModelType.Next)
                    {
                        asm.ReportError(Errors.Z0102, op);
                        return;
                    }

                    // --- ADD HL,A; ADD DE,A; ADD BC,A
                    if (op.Operand.Register == "SP")
                    {
                        asm.ReportError(Errors.Z0009, op, op.Mnemonic, op.Operand.Register);
                        return;
                    }
                    if (op.Operand2.Register != "A")
                    {
                        asm.ReportError(Errors.Z0010, op, op.Mnemonic, op.Operand.Register, op.Operand2.Register);
                        return;
                    }
                    int opCodeBase;
                    switch (op.Operand.Register)
                    {
                        case "HL":
                            opCodeBase = 0xED31;
                            break;
                        case "DE":
                            opCodeBase = 0xED32;
                            break;
                        default: // BC
                            opCodeBase = 0xED33;
                            break;
                    }
                    asm.EmitDoubleByte(opCodeBase);
                    return;
                }

                if (op.Operand2.Type == OperandType.Expr)
                {
                    if (asm._output.ModelType != SpectrumModelType.Next)
                    {
                        asm.ReportError(Errors.Z0102, op);
                        return;
                    }

                    // --- ADD HL,NNNN; ADD DE,NNNN; ADD BC,NNNN
                    if (op.Operand.Register == "SP")
                    {
                        asm.ReportError(Errors.Z0009, op, op.Mnemonic, op.Operand.Register);
                        return;
                    }

                    int opCodeBase;
                    switch (op.Operand.Register)
                    {
                        case "HL":
                            opCodeBase = 0xED34;
                            break;
                        case "DE":
                            opCodeBase = 0xED35;
                            break;
                        default: // BC
                            opCodeBase = 0xED36;
                            break;
                    }
                    asm.EmitDoubleByte(opCodeBase);
                    asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit16);
                }
            }

            if (op.Operand.Type == OperandType.Reg16Idx)
            {
                if (op.Mnemonic != "ADD")
                {
                    asm.ReportError(Errors.Z0001, op, op.Mnemonic);
                    return;
                }

                var opCode = op.Operand.Register == "IX" ? 0xDD09 : 0xFD09;
                if (op.Operand2.Type == OperandType.Reg16)
                {
                    if (op.Operand2.Register == "HL")
                    {
                        asm.ReportError(Errors.Z0010, op, op.Mnemonic, 
                            op.Operand.Register, op.Operand2.Register);
                        return;
                    }
                    asm.EmitDoubleByte(opCode + (s_Reg16Order.IndexOf(op.Operand2.Register) << 4));
                    return;
                }

                if (op.Operand2.Type == OperandType.Reg16Idx)
                {
                    if (op.Operand.Register != op.Operand2.Register)
                    {
                        asm.ReportError(Errors.Z0010, op, op.Mnemonic,
                            op.Operand.Register, op.Operand2.Register);
                        return;
                    }
                    asm.EmitDoubleByte(opCode + 0x20);
                }
            }
        }

        /// <summary>
        /// INC/DEC operation
        /// </summary>
        private static void ProcessIncDec(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand.Type == OperandType.RegIndirect && op.Operand.Register != "(HL)")
            {
                asm.ReportError(Errors.Z0011, op, op.Mnemonic, op.Operand.Register);
                return;
            }
            if (op.Operand.Type == OperandType.IndexedAddress)
            {
                var opByte = op.Mnemonic == "INC" ? (byte)0x34 : (byte)0x35;
                asm.EmitIndexedOperation(op, op.Operand, opByte);
            }
            else
            {
                asm.EmitOperationWithLookup(
                    op.Mnemonic == "INC" ? s_IncOpBytes : s_DecOpBytes,
                    op.Operand.Register, op);
            }
        }

        /// <summary>
        /// EX operation
        /// </summary>
        private static void ProcessEx(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand.Register == "AF")
            {
                if (op.Operand2.Register != "AF'")
                {
                    asm.ReportError(Errors.Z0012, op);
                    return;
                }
                asm.EmitByte(0x08);

            }
            else if (op.Operand.Register == "DE")
            {
                if (op.Operand2.Register != "HL")
                {
                    asm.ReportError(Errors.Z0013, op);
                    return;
                }
                asm.EmitByte(0xEB);
            }
            else if (op.Operand.Register != "(SP)")
            {
                asm.ReportError(Errors.Z0014, op);
            }
            else
            {
                if (op.Operand2.Register == "HL")
                {
                    asm.EmitByte(0xE3);
                }
                else if (op.Operand2.Register == "IX")
                {
                    asm.EmitBytes(0xDD, 0xE3);
                }
                else if (op.Operand2.Register == "IY")
                {
                    asm.EmitBytes(0xFD, 0xE3); // ex (sp),iy
                }
                else
                {
                    asm.ReportError(Errors.Z0015, op);
                }
            }

        }

        /// <summary>
        /// RET operation
        /// </summary>
        private static void ProcessRet(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand == null)
            {
                asm.EmitByte(0xC9);
                return;
            }

            if (op.Operand.Type == OperandType.Reg8)
            {
                if (op.Operand.Register == "C")
                {
                    asm.EmitByte(0xD8);
                    return;
                }
                asm.ReportError(Errors.Z0001, op, op.Mnemonic);
                return;
            }
            if (op.Operand.Type == OperandType.Condition)
            {
                var condIndex = s_ConditionOrder.IndexOf(op.Operand.Condition);
                    var opCode = 0xC0 + condIndex * 8;
                asm.EmitByte((byte)opCode);
            }
            else
            {
                asm.ReportError(Errors.Z0001, op, op.Mnemonic);
            }
        }

        /// <summary>
        /// CALL operation
        /// </summary>
        private static void ProcessCall(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand.Type == OperandType.Reg8)
            {
                if (op.Operand.Register == "C")
                {
                    asm.EmitByte(0xDC);
                    asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit16);
                    return;
                }
                asm.ReportError(Errors.Z0001, op, op.Mnemonic);
                return;
            }

            if (op.Operand.Type == OperandType.Condition)
            {
                var condIndex = s_ConditionOrder.IndexOf(op.Operand.Condition);
                var opCode = 0xC4 + condIndex * 8;
                asm.EmitByte((byte) opCode);
                asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit16);
            }
            else
            {
                asm.EmitByte(0xCD);
                asm.EmitumNumericExpression(op, op.Operand.Expression, FixupType.Bit16);
            }
        }

        /// <summary>
        /// JP operation
        /// </summary>
        private static void ProcessJp(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand.Type == OperandType.Reg8)
            {
                if (op.Operand.Register == "C")
                {
                    asm.EmitByte(0xDA);
                    asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit16);
                    return;
                }
                asm.ReportError(Errors.Z0001, op, op.Mnemonic);
                return;
            }

            if (op.Operand.Type == OperandType.Condition)
            {
                var condIndex = s_ConditionOrder.IndexOf(op.Operand.Condition);
                var opCode = 0xC2 + condIndex * 8;
                asm.EmitByte((byte)opCode);
                asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit16);
                return;
            }

            if (op.Operand.Type == OperandType.Expr)
            {
                // --- Jump to a direct address
                asm.EmitByte(0xC3);
                asm.EmitumNumericExpression(op, op.Operand.Expression, FixupType.Bit16);
                return;
            }

            if (op.Operand.Type == OperandType.RegIndirect && op.Operand.Register != "(HL)"
                || op.Operand.Type == OperandType.IndexedAddress && op.Operand.Sign != null)
            {
                asm.ReportError(Errors.Z0016, op);
                return;
            }

            // --- Jump to a register address
            if (op.Operand.Type == OperandType.Reg16)
            {
                if (op.Operand.Register != "HL")
                {
                    asm.ReportError(Errors.Z0016, op);
                    return;
                }
            }
            else if (op.Operand.Type == OperandType.IndexedAddress
                || op.Operand.Type == OperandType.Reg16Idx)
            {
                if (op.Operand.Register == "IX") asm.EmitByte(0xDD);
                else if (op.Operand.Register == "IY") asm.EmitByte(0xFD);
            }
            asm.EmitByte(0xE9);
        }

        /// <summary>
        /// JR operation
        /// </summary>
        private static void ProcessJr(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand.Type == OperandType.Reg8)
            {
                if (op.Operand.Register == "C")
                {
                    asm.EmitJumpRelativeOp(op, op.Operand2.Expression, 0x38);
                    return;
                }
                asm.ReportError(Errors.Z0001, op, op.Mnemonic);
                return;
            }

            if (op.Operand.Type == OperandType.Condition)
            {
                var condIndex = s_ConditionOrder.IndexOf(op.Operand.Condition);
                var opCode = 0x20 + condIndex * 8;
                asm.EmitJumpRelativeOp(op, op.Operand2.Expression, opCode);
                return;
            }

            if (op.Operand.Type != OperandType.Expr) return;

            // --- Jump to a direct address
            asm.EmitJumpRelativeOp(op, op.Operand.Expression, 0x18);
        }

        /// <summary>
        /// DJNZ operation
        /// </summary>
        private static void ProcessDjnz(Z80Assembler asm, CompoundOperation op)
        {
            asm.EmitJumpRelativeOp(op, op.Operand.Expression, 0x10);
        }

        /// <summary>
        /// RST operation
        /// </summary>
        private static void ProcessRst(Z80Assembler asm, CompoundOperation op)
        {
            var value = asm.EvalImmediate(op, op.Operand.Expression);
            if (!value.IsValid) return;
            if (value.Value > 0x38 || value.Value % 8 != 0)
            {
                asm.ReportError(Errors.Z0018, op, $"{value:X}");
                return;
            }
            asm.EmitByte((byte)(0xC7 + value.Value));
        }

        /// <summary>
        /// Process PUSH and POP operations
        /// </summary>
        private static void ProcessStackOp(Z80Assembler asm, CompoundOperation op)
        {
            if (op.Operand.Type == OperandType.Expr)
            {
                // --- PUSH NNNN Spectrum Next operation
                if (op.Mnemonic == "POP")
                {
                    asm.ReportError(Errors.Z0024, op);
                    return;
                }
                if (asm._output.ModelType != SpectrumModelType.Next)
                {
                    asm.ReportError(Errors.Z0102, op);
                    return;
                }
                asm.EmitDoubleByte(0xED8A);
                asm.EmitumNumericExpression(op, op.Operand.Expression, FixupType.Bit16);
                return;
            }

            if (op.Operand.Register == "AF'")
            {
                asm.ReportError(Errors.Z0019, op, op.Mnemonic);
                return;
            }
            asm.EmitOperationWithLookup(
                op.Mnemonic == "PUSH" ? s_PushOpBytes : s_PopOpBytes,
                op.Operand.Register, op);
        }

        /// <summary>
        /// Process the IM operation
        /// </summary>
        private static void ProcessImOp(Z80Assembler asm, CompoundOperation op)
        {
            var mode = asm.EvalImmediate(op, op.Operand.Expression);
            if (!mode.IsValid) return;

            if (mode.AsLong() < 0 || mode.AsLong() > 2)
            {
                asm.ReportError(Errors.Z0020, op, mode);
                return;
            }

            var opCodes = new[] { 0xED46, 0xED56, 0xED5E };
            asm.EmitDoubleByte(opCodes[mode.Value]);
        }

        /// <summary>
        /// Process the MIRROR operation
        /// </summary>
        private static void ProcessMirrorOp(Z80Assembler asm, CompoundOperation op)
        {
            if (asm._output.ModelType != SpectrumModelType.Next)
            {
                asm.ReportError(Errors.Z0102, op);
                return;
            }
            if (op.Operand.Register != "A" && op.Operand.Register != "DE")
            {
                asm.ReportError(Errors.Z0009, op, op.Mnemonic, op.Operand.Register);
            }
            var opCode = op.Operand.Register == "A"
                ? 0xED24
                : 0xED26;
            asm.EmitDoubleByte(opCode);
        }

        /// <summary>
        /// Process the NEXTREG operation
        /// </summary>
        private static void ProcessNextRegOp(Z80Assembler asm, CompoundOperation op)
        {
            if (asm._output.ModelType != SpectrumModelType.Next)
            {
                asm.ReportError(Errors.Z0102, op);
                return;
            }

            if (op.Operand2.Type == OperandType.Reg8)
            {
                if (op.Operand2.Register != "A")
                {
                    asm.ReportError(Errors.Z0010, op, op.Mnemonic, "<expr>", op.Operand2.Register);
                    return;
                }

                asm.EmitDoubleByte(0xED92);
                asm.EmitumNumericExpression(op, op.Operand.Expression, FixupType.Bit8);
                return;
            }

            asm.EmitDoubleByte(0xED91);
            asm.EmitumNumericExpression(op, op.Operand.Expression, FixupType.Bit8);
            asm.EmitumNumericExpression(op, op.Operand2.Expression, FixupType.Bit8);
        }

        /// <summary>
        /// Process the TEST operation
        /// </summary>
        private static void ProcessTestOp(Z80Assembler asm, CompoundOperation op)
        {
            if (asm._output.ModelType != SpectrumModelType.Next)
            {
                asm.ReportError(Errors.Z0102, op);
                return;
            }

            asm.EmitDoubleByte(0xED27);
            asm.EmitumNumericExpression(op, op.Operand.Expression, FixupType.Bit8);
        }

        /// <summary>
        /// Reports that the specified source and destination means invalid LD operation
        /// </summary>
        /// <param name="opLine">Assembly line for the load operation</param>
        /// <param name="dest">Load destination</param>
        /// <param name="source">Load source</param>
        private void ReportInvalidLoadOp(SourceLineBase opLine, string dest, string source)
        {
            ReportError(Errors.Z0021, opLine, dest, source);
        }

        /// <summary>
        /// Emits a jump relative operation
        /// </summary>
        /// <param name="opLine">Control flow operation line</param>
        /// <param name="target">Target expression</param>
        /// <param name="opCode">Operation code</param>
        private void EmitJumpRelativeOp(SourceLineBase opLine, ExpressionNode target, int opCode)
        {
            var value = Eval(opLine, target);
            if (target.EvaluationError != null) return;
            var dist = 0;
            if (value.IsNonEvaluated)
            {
                RecordFixup(opLine, FixupType.Jr, target);
            }
            else
            {
                // --- Check for Relative address
                dist = value.Value - (GetCurrentAssemblyAddress() + 2);
                if (dist < -128 || dist > 127)
                {
                    ReportError(Errors.Z0022, opLine, dist);
                    return;
                }
            }
            EmitBytes((byte)opCode, (byte)dist);
        }

        /// <summary>
        /// Emits an indexed operation with the specified operand and operation code
        /// </summary>
        /// <param name="opLine">Operation source line</param>
        /// <param name="operand">Operand with indexed address</param>
        /// <param name="opCode">Operation code</param>
        private void EmitIndexedOperation(SourceLineBase opLine, Operand operand, byte opCode)
        {
            byte idxByte, disp;
            var done = GetIndexBytes(opLine, operand, out idxByte, out disp);
            EmitBytes(idxByte, opCode);
            if (!done)
            {
                RecordFixup(opLine, FixupType.Bit8, operand.Expression);
            }
            EmitByte(disp);
        }

        /// <summary>
        /// Gets the index byte and displacement byte from an indexed address
        /// </summary>
        /// <param name="opLine">Control flow operation line</param>
        /// <param name="operand">Operand with indexed address type</param>
        /// <param name="idxByte">Index byte (0xDD for IX, 0xFD for IY)</param>
        /// <param name="disp">Displacement byte</param>
        /// <returns>
        /// True, if displacement has been resolved; 
        /// false if it can be resolved only during fixup phase
        /// </returns>
        private bool GetIndexBytes(SourceLineBase opLine, Operand operand, out byte idxByte, out byte disp)
        {
            idxByte = operand.Register == "IX" ? (byte)0xDD : (byte)0xFD;
            disp = 0x00;
            if (operand.Sign == null) return true;

            var dispValue = Eval(opLine, operand.Expression);
            if (!dispValue.IsValid) return false;
            disp = operand.Sign == "-" 
                ? (byte) -dispValue.Value 
                : dispValue.AsByte();
            return true;
        }

        /// <summary>
        /// Emits an indexed operation with the specified operand and operation code
        /// </summary>
        /// <param name="opLine">Operation source line</param>
        /// <param name="register">Index register</param>
        /// <param name="sign">Displacement sign</param>
        /// <param name="expr">Displacement expression</param>
        /// <param name="opCode">Operation code</param>
        private void EmitIndexedBitOperation(SourceLineBase opLine, string register, string sign, ExpressionNode expr, byte opCode)
        {
            byte idxByte, disp;
            var done = GetIndexBytes(opLine, register, sign, expr, out idxByte, out disp);
            EmitBytes(idxByte, 0xCB, opCode);
            if (!done)
            {
                RecordFixup(opLine, FixupType.Bit8, expr);
            }
            EmitByte(disp);
        }

        /// <summary>
        /// Gets the index byte and displacement byte from an indexxed address
        /// </summary>
        /// <param name="opLine">Control flow operation line</param>
        /// <param name="register">Index register</param>
        /// <param name="sign">Displacement sign</param>
        /// <param name="expr">Displacement expression</param>
        /// <param name="idxByte">Index byte (0xDD for IX, 0xFD for IY)</param>
        /// <param name="disp">Displacement byte</param>
        /// <returns>
        /// True, if displacement has been resolved; 
        /// false if it can be resolved only during fixup phase
        /// </returns>
        private bool GetIndexBytes(SourceLineBase opLine, string register, string sign, ExpressionNode expr, out byte idxByte, out byte disp)
        {
            idxByte = register == "IX" ? (byte)0xDD : (byte)0xFD;
            disp = 0x00;
            if (sign == null) return true;

            var dispValue = Eval(opLine, expr);
            if (!dispValue.IsValid) return false;
            disp = sign == "-"
                ? (byte)-dispValue.Value
                : dispValue.AsByte();
            return true;
        }

        /// <summary>
        /// Evaluates the expression and emits bytes accordingly. If the expression
        /// cannot be resolved, creates a fixup.
        /// </summary>
        /// <param name="opLine">Assembly line</param>
        /// <param name="expr">Expression to evaluate</param>
        /// <param name="type">Expression/Fixup type</param>
        /// <returns></returns>
        private void EmitumNumericExpression(SourceLineBase opLine, ExpressionNode expr, FixupType type)
        {
            var value = Eval(opLine, expr);
            if (value.IsNonEvaluated)
            {
                RecordFixup(opLine, type, expr);
            }
            if (value.IsValid && value.Type == ExpressionValueType.String)
            {
                ReportError(Errors.Z0305, opLine);
            }
            var fixupValue = value.Value;
            EmitByte((byte)fixupValue);
            if (type == FixupType.Bit16)
            {
                EmitByte((byte)(fixupValue >> 8));
            }
        }

        #endregion

        #region Emit helper methods

        /// <summary>
        /// Emits a new byte to the current code segment
        /// </summary>
        /// <param name="data">Data byte to emit</param>
        /// <returns>Current code offset</returns>
        public void EmitByte(byte data)
        {
            EnsureCodeSegment();
            var overflow = CurrentSegment.EmitByte(data);
            if (overflow)
            {
                ReportError(Errors.Z0304, CurrentSourceLine);
            }
        }

        /// <summary>
        /// Emits a new word to the current code segment
        /// </summary>
        /// <param name="data">Data byte to emit</param>
        /// <returns>Current code offset</returns>
        public void EmitWord(ushort data)
        {
            EmitByte((byte)data);
            EmitByte((byte)(data >> 8));
        }

        /// <summary>
        /// Emits an operation using a lookup table
        /// </summary>
        /// <param name="table">Lookup table</param>
        /// <param name="key">Operation key</param>
        /// <param name="operation">Assembly line that represents the operation</param>
        private void EmitOperationWithLookup(IReadOnlyDictionary<string, int> table, string key,
            OperationBase operation)
        {
            if (table.TryGetValue(key, out var code))
            {
                EmitDoubleByte(code);
                return;
            }
            ReportError(Errors.Z0085, operation, key, operation.Mnemonic);
        }

        /// <summary>
        /// Ensures that there's a code segment by the time the code is emitted
        /// </summary>
        private void EnsureCodeSegment()
        {
            if (CurrentSegment == null)
            {
                CurrentSegment = new BinarySegment
                {
                    StartAddress = _options?.DefaultStartAddress ?? 0x8000,
                };
                _output.Segments.Add(CurrentSegment);
            }
        }

        /// <summary>
        /// Emits a double byte passed as an integer
        /// </summary>
        private void EmitDoubleByte(int doubleByte)
        {
            var low = (byte) (doubleByte & 0xFF);
            var high = (byte) ((doubleByte >> 8) & 0xFF);
            if (high != 0)
            {
                EmitByte(high);
            }
            EmitByte(low);
        }

        /// <summary>
        /// Emits a series of bytes
        /// </summary>
        private void EmitBytes(params byte[] bytes)
        {
            foreach (var data in bytes) EmitByte(data);
        }

        /// <summary>
        /// Converts a ZX Spectrum string into a byte lisy
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Bytes representing the string</returns>
        public static List<byte> SpectrumStringToBytes(string input)
        {
            var bytes = new List<byte>(input.Length);
            var state = StrParseState.Normal;
            var collect = 0;
            foreach (var ch in input)
            {
                switch (state)
                {
                    case StrParseState.Normal:
                        if (ch == '\\')
                        {
                            state = StrParseState.Backslash;
                        }
                        else
                        {
                            bytes.Add((byte)ch);
                        }
                        break;

                    case StrParseState.Backslash:
                        state = StrParseState.Normal;
                        switch (ch)
                        {
                            case 'i': // INK
                                bytes.Add(0x10);
                                break;
                            case 'p': // PAPER
                                bytes.Add(0x11);
                                break;
                            case 'f': // FLASH
                                bytes.Add(0x12);
                                break;
                            case 'b': // BRIGHT
                                bytes.Add(0x13);
                                break;
                            case 'I': // INVERSE
                                bytes.Add(0x14);
                                break;
                            case 'o': // OVER
                                bytes.Add(0x15);
                                break;
                            case 'a': // AT
                                bytes.Add(0x16);
                                break;
                            case 't': // TAB
                                bytes.Add(0x17);
                                break;
                            case 'P': // Pound sign
                                bytes.Add(0x60);
                                break;
                            case 'C': // Copyright sign
                                bytes.Add(0x7F);
                                break;
                            case '"':
                                bytes.Add((byte)'"');
                                break;
                            case '\'':
                                bytes.Add((byte)'\'');
                                break;
                            case '\\':
                                bytes.Add((byte)'\\');
                                break;
                            case '0':
                                bytes.Add(0);
                                break;
                            case 'x':
                                state = StrParseState.X;
                                break;
                            default:
                                bytes.Add((byte)ch);
                                break;
                        }
                        break;

                    case StrParseState.X:
                        if (ch >= '0' && ch <= '9'
                            || ch >= 'a' && ch <= 'f'
                            || ch >= 'A' && ch <= 'F')
                        {
                            collect = int.Parse(new string(ch, 1), NumberStyles.HexNumber);
                            state = StrParseState.Xh;
                        }
                        else
                        {
                            bytes.Add((byte)'x');
                            state = StrParseState.Normal;
                        }
                        break;

                    case StrParseState.Xh:
                        if (ch >= '0' && ch <= '9'
                            || ch >= 'a' && ch <= 'f'
                            || ch >= 'A' && ch <= 'F')
                        {
                            collect = collect * 0x10 + int.Parse(new string(ch, 1), NumberStyles.HexNumber);
                            bytes.Add((byte)collect);
                            state = StrParseState.Normal;
                        }
                        else
                        {
                            bytes.Add((byte)collect);
                            bytes.Add((byte)ch);
                            state = StrParseState.Normal;
                        }
                        break;
                }
            }

            // --- Handle the final machine state
            switch (state)
            {
                case StrParseState.Backslash:
                    bytes.Add((byte)'\\');
                    break;
                case StrParseState.X:
                    bytes.Add((byte)'x');
                    break;
                case StrParseState.Xh:
                    bytes.Add((byte)collect);
                    break;
            }
            return bytes;
        }

        /// <summary>
        /// We use this enumeration to represent the state
        /// of the machine parsing Spectrum string
        /// </summary>
        private enum StrParseState
        {
            Normal,
            Backslash,
            X,
            Xh
        }

        #endregion

        #region Operation rules

        /// <summary>
        /// Represents the rules for the LD operations
        /// </summary>
        private static readonly List<OperandRule> s_LoadRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8, OperandType.Reg8),
                new OperandRule(OperandType.Reg8, OperandType.RegIndirect),
                new OperandRule(OperandType.Reg8, OperandType.Reg8Spec),
                new OperandRule(OperandType.Reg8, OperandType.Reg8Idx),
                new OperandRule(OperandType.Reg8, OperandType.Expr),
                new OperandRule(OperandType.Reg8, OperandType.MemIndirect),
                new OperandRule(OperandType.Reg8, OperandType.IndexedAddress),
                new OperandRule(OperandType.Reg8Idx, OperandType.Reg8),
                new OperandRule(OperandType.Reg8Idx, OperandType.Reg8Idx),
                new OperandRule(OperandType.Reg8Idx, OperandType.Expr),
                new OperandRule(OperandType.Reg8Spec, OperandType.Reg8),
                new OperandRule(OperandType.RegIndirect, OperandType.Reg8),
                new OperandRule(OperandType.RegIndirect, OperandType.Expr),
                new OperandRule(OperandType.MemIndirect, OperandType.Reg8),
                new OperandRule(OperandType.MemIndirect, OperandType.Reg16),
                new OperandRule(OperandType.MemIndirect, OperandType.Reg16Idx),
                new OperandRule(OperandType.Reg16, OperandType.Expr),
                new OperandRule(OperandType.Reg16, OperandType.MemIndirect),
                new OperandRule(OperandType.Reg16, OperandType.Reg16),
                new OperandRule(OperandType.Reg16, OperandType.Reg16Idx),
                new OperandRule(OperandType.Reg16Idx, OperandType.Expr),
                new OperandRule(OperandType.Reg16Idx, OperandType.MemIndirect),
                new OperandRule(OperandType.IndexedAddress, OperandType.Reg8),
                new OperandRule(OperandType.IndexedAddress, OperandType.Expr)
            };

        /// <summary>
        /// Represents a single expression rule
        /// </summary>
        private static readonly List<OperandRule> s_SingleExprRule =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Expr)
            };

        /// <summary>
        /// Represents a single expression rule
        /// </summary>
        private static readonly List<OperandRule> s_JumpRule =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8, OperandType.Expr),
                new OperandRule(OperandType.Condition, OperandType.Expr),
                new OperandRule(OperandType.Expr)
            };

        /// <summary>
        /// Stack operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_StackOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg16),
                new OperandRule(OperandType.Reg16Idx),
                new OperandRule(OperandType.Reg16Spec),
                new OperandRule(OperandType.Expr),
            };

        /// <summary>
        /// Represents a rule set for increment and decrement operations
        /// </summary>
        private static readonly List<OperandRule> s_IncDecOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8),
                new OperandRule(OperandType.Reg8Idx),
                new OperandRule(OperandType.Reg16),
                new OperandRule(OperandType.Reg16Idx),
                new OperandRule(OperandType.RegIndirect),
                new OperandRule(OperandType.IndexedAddress)
            };

        /// <summary>
        /// Alu operation rule set (ADD, ADC, SBC)
        /// </summary>
        private static readonly List<OperandRule> s_RsDoubleArgAluRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8, OperandType.Reg8),
                new OperandRule(OperandType.Reg8, OperandType.Reg8Idx),
                new OperandRule(OperandType.Reg8, OperandType.RegIndirect),
                new OperandRule(OperandType.Reg8, OperandType.IndexedAddress),
                new OperandRule(OperandType.Reg8, OperandType.Expr),
                new OperandRule(OperandType.Reg16, OperandType.Reg16),
                new OperandRule(OperandType.Reg16Idx, OperandType.Reg16),
                new OperandRule(OperandType.Reg16Idx, OperandType.Reg16Idx),
            };

        /// <summary>
        /// ADD operation rule set (created because of Next opcodes)
        /// </summary>
        private static readonly List<OperandRule> s_ExtAddRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8, OperandType.Reg8),
                new OperandRule(OperandType.Reg8, OperandType.Reg8Idx),
                new OperandRule(OperandType.Reg8, OperandType.RegIndirect),
                new OperandRule(OperandType.Reg8, OperandType.IndexedAddress),
                new OperandRule(OperandType.Reg8, OperandType.Expr),
                new OperandRule(OperandType.Reg16, OperandType.Reg16),
                new OperandRule(OperandType.Reg16, OperandType.Reg8),
                new OperandRule(OperandType.Reg16, OperandType.Expr),
                new OperandRule(OperandType.Reg16Idx, OperandType.Reg16),
                new OperandRule(OperandType.Reg16Idx, OperandType.Reg16Idx),
            };

        /// <summary>
        /// Alu operation rule set (SUB, AND, XOR, OR, CP)
        /// </summary>
        private static readonly List<OperandRule> s_SingleArgAluRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8),
                new OperandRule(OperandType.Reg8Idx),
                new OperandRule(OperandType.RegIndirect),
                new OperandRule(OperandType.IndexedAddress),
                new OperandRule(OperandType.Expr),
                new OperandRule(OperandType.Reg8, OperandType.Reg8),
                new OperandRule(OperandType.Reg8, OperandType.Reg8Idx),
                new OperandRule(OperandType.Reg8, OperandType.RegIndirect),
                new OperandRule(OperandType.Reg8, OperandType.IndexedAddress),
                new OperandRule(OperandType.Reg8, OperandType.Expr),
            };

        /// <summary>
        /// Shift and rotate operations rule set
        /// </summary>
        private static readonly List<OperandRule> s_ShiftRotateRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8),
                new OperandRule(OperandType.RegIndirect),
                new OperandRule(OperandType.IndexedAddress),
                new OperandRule(OperandType.IndexedAddress, OperandType.Reg8)
            };

        /// <summary>
        /// BIT, SET, RES rules
        /// </summary>
        private static readonly List<OperandRule> s_BitManipRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Expr, OperandType.Reg8),
                new OperandRule(OperandType.Expr, OperandType.RegIndirect),
                new OperandRule(OperandType.Expr, OperandType.IndexedAddress),
            };

        /// <summary>
        /// JP operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_JpOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8, OperandType.Expr),
                new OperandRule(OperandType.Condition, OperandType.Expr),
                new OperandRule(OperandType.Expr),
                new OperandRule(OperandType.Reg16),
                new OperandRule(OperandType.Reg16Idx),
                new OperandRule(OperandType.RegIndirect),
                new OperandRule(OperandType.IndexedAddress),
            };

        /// <summary>
        /// RET operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_RetOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8),
                new OperandRule(OperandType.Condition),
                new OperandRule(OperandType.None)
            };

        /// <summary>
        /// EX operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_ExOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg16Spec, OperandType.Reg16Spec),
                new OperandRule(OperandType.Reg16, OperandType.Reg16),
                new OperandRule(OperandType.RegIndirect, OperandType.Reg16),
                new OperandRule(OperandType.RegIndirect, OperandType.Reg16Idx)
            };

        /// <summary>
        /// IN operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_InOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8, OperandType.MemIndirect),
                new OperandRule(OperandType.Reg8, OperandType.CPort),
                new OperandRule(OperandType.CPort)
            };

        /// <summary>
        /// OUT operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_OutOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.MemIndirect, OperandType.Reg8),
                new OperandRule(OperandType.CPort, OperandType.Reg8),
                new OperandRule(OperandType.CPort, OperandType.Expr)
            };

        /// <summary>
        /// MIRROR operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_MirrorOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Reg8),
                new OperandRule(OperandType.Reg16)
            };

        /// <summary>
        /// NEXTREG operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_NextRegOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Expr, OperandType.Expr),
                new OperandRule(OperandType.Expr, OperandType.Reg8)
            };

        /// <summary>
        /// TEST operation rule set
        /// </summary>
        private static readonly List<OperandRule> s_TestOpRules =
            new List<OperandRule>
            {
                new OperandRule(OperandType.Expr)
            };

        /// <summary>
        /// The table that contains the first level processing rules
        /// </summary>
        private readonly Dictionary<string, CompoundOperationDescriptor> _compoundOpTable =
            new Dictionary<string, CompoundOperationDescriptor>(StringComparer.OrdinalIgnoreCase)
            {
                { "ADC", new CompoundOperationDescriptor(s_RsDoubleArgAluRules, ProcessAlu1) },
                { "ADD", new CompoundOperationDescriptor(s_ExtAddRules, ProcessAlu1) },
                { "AND", new CompoundOperationDescriptor(s_SingleArgAluRules, ProcessAlu2) },
                { "BIT", new CompoundOperationDescriptor(s_BitManipRules, ProcessBit) },
                { "CALL", new CompoundOperationDescriptor(s_JumpRule, ProcessCall) },
                { "CP", new CompoundOperationDescriptor(s_SingleArgAluRules, ProcessAlu2) },
                { "DEC", new CompoundOperationDescriptor(s_IncDecOpRules, ProcessIncDec) },
                { "DJNZ", new CompoundOperationDescriptor(s_SingleExprRule, ProcessDjnz) },
                { "EX", new CompoundOperationDescriptor(s_ExOpRules, ProcessEx) },
                { "IM", new CompoundOperationDescriptor(s_SingleExprRule, ProcessImOp) },
                { "IN", new CompoundOperationDescriptor(s_InOpRules, ProcessIn) },
                { "INC", new CompoundOperationDescriptor(s_IncDecOpRules, ProcessIncDec) },
                { "JP", new CompoundOperationDescriptor(s_JpOpRules, ProcessJp) },
                { "JR", new CompoundOperationDescriptor(s_JumpRule, ProcessJr) },
                { "LD", new CompoundOperationDescriptor(s_LoadRules, ProcessLd) },
                { "MIRROR", new CompoundOperationDescriptor(s_MirrorOpRules, ProcessMirrorOp) },
                { "NEXTREG", new CompoundOperationDescriptor(s_NextRegOpRules, ProcessNextRegOp) },
                { "OR", new CompoundOperationDescriptor(s_SingleArgAluRules, ProcessAlu2) },
                { "OUT", new CompoundOperationDescriptor(s_OutOpRules, ProcessOut) },
                { "POP", new CompoundOperationDescriptor(s_StackOpRules, ProcessStackOp) },
                { "PUSH", new CompoundOperationDescriptor(s_StackOpRules, ProcessStackOp) },
                { "RES", new CompoundOperationDescriptor(s_BitManipRules, ProcessBit) },
                { "RET", new CompoundOperationDescriptor(s_RetOpRules, ProcessRet) },
                { "RL", new CompoundOperationDescriptor(s_ShiftRotateRules, ProcessShiftRotate) },
                { "RLC", new CompoundOperationDescriptor(s_ShiftRotateRules, ProcessShiftRotate) },
                { "RR", new CompoundOperationDescriptor(s_ShiftRotateRules, ProcessShiftRotate) },
                { "RRC", new CompoundOperationDescriptor(s_ShiftRotateRules, ProcessShiftRotate) },
                { "RST", new CompoundOperationDescriptor(s_SingleExprRule, ProcessRst) },
                { "SBC", new CompoundOperationDescriptor(s_RsDoubleArgAluRules, ProcessAlu1) },
                { "SET", new CompoundOperationDescriptor(s_BitManipRules, ProcessBit) },
                { "SLA", new CompoundOperationDescriptor(s_ShiftRotateRules, ProcessShiftRotate) },
                { "SLL", new CompoundOperationDescriptor(s_ShiftRotateRules, ProcessShiftRotate) },
                { "SRA", new CompoundOperationDescriptor(s_ShiftRotateRules, ProcessShiftRotate) },
                { "SRL", new CompoundOperationDescriptor(s_ShiftRotateRules, ProcessShiftRotate) },
                { "SUB", new CompoundOperationDescriptor(s_SingleArgAluRules, ProcessAlu2) },
                { "TEST", new CompoundOperationDescriptor(s_TestOpRules, ProcessTestOp) },
                { "XOR", new CompoundOperationDescriptor(s_SingleArgAluRules, ProcessAlu2) },
            };

        #endregion

        #region Operation lookup tables

        /// <summary>
        /// The order of Z80 shift and rotation operations
        /// </summary>
        private static readonly List<string> s_ShiftOpOrder = new List<string>
        {
            "RLC",
            "RRC",
            "RL",
            "RR",
            "SLA",
            "SRA",
            "SLL",
            "SRL"
        };

        /// <summary>
        /// The order of Z80 ALU operations
        /// </summary>
        private static readonly List<string> s_AluOpOrder = new List<string>
        {
            "ADD",
            "ADC",
            "SUB",
            "SBC",
            "AND",
            "XOR",
            "OR",
            "CP"
        };

        /// <summary>
        /// The index order of 8-bit registers in Z80 operations
        /// </summary>
        private static readonly List<string> s_Reg8Order = new List<string>
        {
            "B",
            "C",
            "D",
            "E",
            "H",
            "L",
            "(HL)",
            "A"
        };

        /// <summary>
        /// The index order of 16-bit registers in Z80 operations
        /// </summary>
        private static readonly List<string> s_Reg16Order = new List<string>
        {
            "BC",
            "DE",
            "HL",
            "SP"
        };

        /// <summary>
        /// The order of Z80 conditions operations
        /// </summary>
        private static readonly List<string> s_ConditionOrder = new List<string>
        {
            "NZ",
            "Z",
            "NC",
            "C",
            "PO",
            "PE",
            "P",
            "M"
        };

        /// <summary>
        /// Z80 binary operation codes for trivial operations
        /// </summary>
        private static readonly Dictionary<string, int> s_TrivialOpBytes =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"NOP", 0x00},
                {"RLCA", 0x07},
                {"RRCA", 0x0F},
                {"RLA", 0x17},
                {"RRA", 0x1F},
                {"DAA", 0x27},
                {"CPL", 0x2F},
                {"SCF", 0x37},
                {"CCF", 0x3F},
                {"HALT", 0x76},
                {"EXX", 0xD9},
                {"DI", 0xF3},
                {"EI", 0xFB},
                {"NEG", 0xED44},
                {"RETN", 0xED45},
                {"RETI", 0xED4D},
                {"RRD", 0xED67},
                {"RLD", 0xED6F},
                {"LDI", 0xEDA0},
                {"CPI", 0xEDA1},
                {"INI", 0xEDA2},
                {"OUTI", 0xEDA3},
                {"LDD", 0xEDA8},
                {"CPD", 0xEDA9},
                {"IND", 0xEDAA},
                {"OUTD", 0xEDAB},
                {"LDIR", 0xEDB0},
                {"CPIR", 0xEDB1},
                {"INIR", 0xEDB2},
                {"OTIR", 0xEDB3},
                {"LDDR", 0xEDB8},
                {"CPDR", 0xEDB9},
                {"INDR", 0xEDBA},
                {"OTDR", 0xEDBB},
                {"SWAPNIB", 0xED23},
                {"MUL", 0xED30},
                {"OUTINB", 0xED90},
                {"LDIX", 0xEDA4},
                {"LDIRX", 0xEDB4},
                {"LDDX", 0xEDAC},
                {"LDDRX", 0xEDBC},
                {"PIXELDN", 0xED93},
                {"PIXELAD", 0xED94},
                {"SETAE", 0xED95},
                {"LDPIRX", 0xEDB7},
                {"LDIRSCALE", 0xEDB6},
            };

        /// <summary>
        /// Z80 PUSH operation binary codes
        /// </summary>
        private static readonly Dictionary<string, int> s_PushOpBytes =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"AF", 0xF5},
                {"BC", 0xC5},
                {"DE", 0xD5},
                {"HL", 0xE5},
                {"IX", 0xDDE5},
                {"IY", 0xFDE5}
            };

        /// <summary>
        /// Z80 POP operation binary codes
        /// </summary>
        private static readonly Dictionary<string, int> s_PopOpBytes =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"AF", 0xF1},
                {"BC", 0xC1},
                {"DE", 0xD1},
                {"HL", 0xE1},
                {"IX", 0xDDE1},
                {"IY", 0xFDE1}
            };

        /// <summary>
        /// Z80 INC operation binary codes
        /// </summary>
        private static readonly Dictionary<string, int> s_IncOpBytes =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"A", 0x3C},
                {"B", 0x04},
                {"C", 0x0C},
                {"D", 0x14},
                {"E", 0x1C},
                {"H", 0x24},
                {"L", 0x2C},
                {"(HL)", 0x34},
                {"XL", 0xDD2C},
                {"XH", 0xDD24},
                {"YL", 0xFD2C},
                {"YH", 0xFD24},
                {"IXL", 0xDD2C},
                {"IXH", 0xDD24},
                {"IYL", 0xFD2C},
                {"IYH", 0xFD24},
                {"BC", 0x03},
                {"DE", 0x13},
                {"HL", 0x23},
                {"SP", 0x33},
                {"IX", 0xDD23},
                {"IY", 0xFD23},
            };

        /// <summary>
        /// Z80 DEC operation binary codes
        /// </summary>
        private static readonly Dictionary<string, int> s_DecOpBytes =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"A", 0x3D},
                {"B", 0x05},
                {"C", 0x0D},
                {"D", 0x15},
                {"E", 0x1D},
                {"H", 0x25},
                {"L", 0x2D},
                {"(HL)", 0x35},
                {"XL", 0xDD2D},
                {"XH", 0xDD25},
                {"YL", 0xFD2D},
                {"YH", 0xFD25},
                {"IXL", 0xDD2D},
                {"IXH", 0xDD25},
                {"IYL", 0xFD2D},
                {"IYH", 0xFD25},
                {"BC", 0x0B},
                {"DE", 0x1B},
                {"HL", 0x2B},
                {"SP", 0x3B},
                {"IX", 0xDD2B},
                {"IY", 0xFD2B},
            };

        /// <summary>
        /// Z80 IN operation binary codes
        /// </summary>
        private static readonly Dictionary<string, int> s_InOpBytes =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"A", 0xED78},
                {"B", 0xED40},
                {"C", 0xED48},
                {"D", 0xED50},
                {"E", 0xED58},
                {"H", 0xED60},
                {"L", 0xED68},
            };

        /// <summary>
        /// Z80 OUT operation binary codes
        /// </summary>
        private static readonly Dictionary<string, int> s_OutOpBytes =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                {"A", 0xED79},
                {"B", 0xED41},
                {"C", 0xED49},
                {"D", 0xED51},
                {"E", 0xED59},
                {"H", 0xED61},
                {"L", 0xED69},
            };

        #endregion
    }
}
