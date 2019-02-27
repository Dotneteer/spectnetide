using System;
using System.Collections.Generic;
using System.Linq;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Expressions;
// ReSharper disable IdentifierTypo

// ReSharper disable InlineOutVariableDeclaration

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class implements the Z80 assembler
    /// </summary>
    public partial class Z80Assembler: IEvaluationContext
    {
        /// <summary>
        /// Gets the current assembly address
        /// </summary>
        ushort IEvaluationContext.GetCurrentAddress() => GetCurrentInstructionAddress();

        /// <summary>
        /// Gets the value of the specified symbol
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="scopeSymbolNames">Additional symbol name segments</param>
        /// <param name="startFromGlobal">Should resolution start from global scope?</param>
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        public ExpressionValue GetSymbolValue(string symbol, List<string> scopeSymbolNames = null, bool startFromGlobal = false)
        {
            return (scopeSymbolNames == null || scopeSymbolNames.Count == 0) && !startFromGlobal 
                ? CurrentModule.ResolveSimpleSymbol(symbol) 
                : CurrentModule.ResolveCompoundSymbol(symbol, scopeSymbolNames, startFromGlobal);
        }

        /// <summary>
        /// Gets the current loop counter value
        /// </summary>
        public ExpressionValue GetLoopCounterValue()
        {
            if (IsInGlobalScope)
            {
                ReportError(Errors.Z0412, CurrentSourceLine);
                return ExpressionValue.Error;
            }

            var scope = CurrentModule.LocalScopes.Peek();
            if (!scope.IsLoopScope)
            {
                ReportError(Errors.Z0412, CurrentSourceLine);
                return ExpressionValue.Error;
            }

            return new ExpressionValue(scope.LoopCounter);
        }

        #region Symbol handler methods

        /// <summary>
        /// Tests if the current assembly instruction is in the global scope of the current module
        /// </summary>
        public bool IsInGlobalScope => CurrentModule.LocalScopes.Count == 0;

        /// <summary>
        /// Checks is the specified error should be reported in the local scope
        /// </summary>
        /// <param name="errorCode">Error code to check</param>
        /// <returns></returns>
        public bool ShouldReportErrorInCurrentScope(string errorCode)
        {
            if (IsInGlobalScope) return true;
            var localScope = CurrentModule.LocalScopes.Peek();
            if (localScope.OwnerScope != null)
            {
                localScope = localScope.OwnerScope;
            }
            return !localScope.IsErrorReported(errorCode);
        }

        /// <summary>
        /// Checks if the specified symbol exists
        /// </summary>
        /// <param name="symbol">Symbol to check</param>
        /// <returns></returns>
        public bool SymbolExists(string symbol)
        {
            var lookup = CurrentModule.LocalScopes.Count > 0
                ? CurrentModule.LocalScopes.Peek().Symbols
                : CurrentModule.Symbols;
            return lookup.TryGetValue(symbol, out var symbolInfo) && symbolInfo.Type == SymbolType.Label;
        }

        /// <summary>
        /// Adds a symbol to the current scope
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="value"></param>
        public void AddSymbol(string symbol, ExpressionValue value)
        {
            Dictionary<string, AssemblySymbolInfo> GetSymbols() =>
                CurrentModule.LocalScopes.Count > 0
                    ? CurrentModule.LocalScopes.Peek().Symbols
                    : CurrentModule.Symbols;

            var currentScopeIsTemporary = CurrentModule.LocalScopes.Count != 0 
                && CurrentModule.LocalScopes.Peek().IsTemporaryScope;
            var symbolIsTemporary = symbol.StartsWith("`");

            var lookup = GetSymbols();
            if (currentScopeIsTemporary)
            {
                if (!symbolIsTemporary)
                {
                    // --- Remove the previous temporary scope
                    var tempScope = CurrentModule.LocalScopes.Peek();
                    FixupSymbols(tempScope.Fixups, tempScope.Symbols, false);
                    CurrentModule.LocalScopes.Pop();

                    lookup = GetSymbols();
                }
            }
            else
            {
                // --- Create a new temporary scope
                CurrentModule.LocalScopes.Push(new SymbolScope() { IsTemporaryScope = true });
                if (symbolIsTemporary)
                {
                    // --- Temporary symbol should go into the new temporary scope
                    lookup = GetSymbols();
                }
            }
            lookup.Add(symbol, AssemblySymbolInfo.CreateLabel(symbol, value));
        }

        /// <summary>
        /// Checks if the variable with the specified name exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool VariableExists(string name)
        {
            // --- Search for the variable from inside out
            foreach (var scope in CurrentModule.LocalScopes)
            {
                if (scope.Symbols.TryGetValue(name, out var symbolInfo) && symbolInfo.Type == SymbolType.Var)
                {
                    return true;
                }
            }

            // --- Check the global scope
            return CurrentModule.Symbols.TryGetValue(name, out var globalSymbol) 
                && globalSymbol.Type == SymbolType.Var;
        }

        /// <summary>
        /// Sets the value of a variable
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="value">Variable value</param>
        public void SetVariable(string name, ExpressionValue value)
        {
            // --- Search for the variable from inside out
            foreach (var scope in CurrentModule.LocalScopes)
            {
                if (scope.Symbols.TryGetValue(name, out var symbolInfo) && symbolInfo.Type == SymbolType.Var)
                {
                    symbolInfo.Value = value;
                    return;
                }
            }

            // --- Check the global scope
            if (CurrentModule.Symbols.TryGetValue(name, out var globalSymbol) 
                && globalSymbol.Type == SymbolType.Var)
            {
                globalSymbol.Value = value;
                return;
            }

            // --- The variable does not exist, create it in the current scope
            var vars = CurrentModule.LocalScopes.Count > 0
                ? CurrentModule.LocalScopes.Peek().Symbols
                : CurrentModule.Symbols;
            vars[name] = AssemblySymbolInfo.CreateVar(name, value);
        }

        #endregion

        #region Evaluation methods

        /// <summary>
        /// Sets the current value of the symbol to the specified value
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="value">Symbol value</param>
        public void SetSymbolValue(string symbol, ExpressionValue value)
        {
            if (CurrentModule.Symbols.TryGetValue(symbol, out var symbolInfo))
            {
                symbolInfo.Value = value;
            }
            else
            {
                CurrentModule.Symbols.Add(symbol, AssemblySymbolInfo.CreateLabel(symbol, value));
            }
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="opLine">Assembly line with operation</param>
        /// <param name="expr">Expression to evaluate</param>
        /// <returns>
        /// Null, if the expression cannot be evaluated, or evaluation 
        /// results an error (e.g. divide by zero)
        /// </returns>
        public ExpressionValue Eval(SourceLineBase opLine, ExpressionNode expr)
        {
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }
            if (!expr.ReadyToEvaluate(this)) return ExpressionValue.NonEvaluated;
            ExpressionNode.ClearErrors();
            var result = expr.Evaluate(this);

            // --- Certain symbols may not bee be evaluated
            if (result == ExpressionValue.Error)
            {
                ReportError(Errors.Z0200, opLine, expr.EvaluationError);
            }
            return result;
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="opLine">Assembly line with operation</param>
        /// <param name="expr">Expression to evaluate</param>
        /// <returns>
        /// Null, if the expression cannot be evaluated, or evaluation 
        /// results an error (e.g. divide by zero)
        /// </returns>
        public ExpressionValue EvalImmediate(SourceLineBase opLine, ExpressionNode expr)
        {
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }
            ExpressionNode.ClearErrors();
            if (!expr.ReadyToEvaluate(this))
            {
                ReportError(Errors.Z0201, opLine, ExpressionNode.SymbolErrors);
                return ExpressionValue.NonEvaluated;
            }
            var result = expr.Evaluate(this);
            if (result.IsValid)
            {
                return result;
            }

            ReportError(Errors.Z0200, opLine, expr.EvaluationError);
            return ExpressionValue.Error;
        }

        #endregion

        #region Fixup methods

        /// <summary>
        /// Records fixup information
        /// </summary>
        /// <param name="opLine">The operation line</param>
        /// <param name="type">Fixup type</param>
        /// <param name="expression">Fixup expression</param>
        /// <param name="label">Optional EQU label</param>
        /// <param name="structeBytes">Optional structure bytes</param>
        private void RecordFixup(SourceLineBase opLine, FixupType type, ExpressionNode expression, 
            string label = null, Dictionary<ushort, byte> structeBytes = null)
        {
            var fixup = new FixupEntry(this, CurrentModule, opLine, type, Output.Segments.Count - 1,
                CurrentSegment.CurrentOffset, expression, label, structeBytes);

            // --- Record fixups in every local scope up to the root
            foreach (var scope in CurrentModule.LocalScopes)
            {
                scope.Fixups.Add(fixup);
            }

            // --- Record fixup in every module up to the root
            var currentModule = CurrentModule;
            while (currentModule != null)
            {
                currentModule.Fixups.Add(fixup);
                currentModule = currentModule.ParentModule;
            }
        }

        /// <summary>
        /// Fixes the unresolved symbol in the last phase of compilation
        /// </summary>
        /// <returns></returns>
        private bool FixupSymbols()
        {
            // --- Go through all scopes from inside to outside
            foreach (var scope in CurrentModule.LocalScopes)
            {
                if (scope.Fixups.Count == 0) continue;
                if (FixupSymbols(scope.Fixups, scope.Symbols, false))
                {
                    // --- Successful fixup in the local scope
                    return true;
                }
            }

            return FixupSymbols(CurrentModule.Fixups, CurrentModule.Symbols, true);
        }

        /// <summary>
        /// Tries to create fixups in the specified scope
        /// </summary>
        /// <param name="fixups">Fixup entries in the scope</param>
        /// <param name="symbols">Symbols in the scope</param>
        /// <param name="signNotEvaluable">Raise error if the symbol is not evaluable</param>
        /// <returns></returns>
        private bool FixupSymbols(IReadOnlyCollection<FixupEntry> fixups, 
            IDictionary<string, AssemblySymbolInfo> symbols, 
            bool signNotEvaluable)
        {
            // --- #1: fix the .equ values
            var success = true;
            foreach (var equ in fixups.Where(f => f.Type == FixupType.Equ && !f.Resolved))
            {
                if (EvaluateFixupExpression(equ, false, signNotEvaluable, out var value))
                {
                    if (symbols.TryGetValue(equ.Label, out var symbolInfo))
                    {
                        symbolInfo.Value = value;
                    }
                    else
                    {
                        symbols.Add(equ.Label, AssemblySymbolInfo.CreateLabel(equ.Label, value));
                    }
                }
                else
                {
                    success = false;
                }
            }

            // --- #2: fix Bit8, Bit16, Jr, Ent, Xent
            foreach (var fixup in fixups.Where(f => !f.Resolved && 
                (f.Type == FixupType.Bit8 || f.Type == FixupType.Bit16 || f.Type == FixupType.Jr
                 || f.Type == FixupType.Ent || f.Type == FixupType.Xent)))
            {
                if (EvaluateFixupExpression(fixup, true, signNotEvaluable, out var value))
                {
                    var segment = Output.Segments[fixup.SegmentIndex];
                    var emittedCode = segment.EmittedCode;
                    switch (fixup.Type)
                    {
                        case FixupType.Bit8:
                            emittedCode[fixup.Offset] = value.AsByte();
                            break;

                        case FixupType.Bit16:
                            emittedCode[fixup.Offset] = value.AsByte();
                            emittedCode[fixup.Offset + 1] = (byte)(value.AsWord() >> 8);
                            break;

                        case FixupType.Jr:
                            // --- Check for Relative address
                            var currentAssemblyAddress = segment.StartAddress
                                + (segment.Displacement ?? 0)
                                + fixup.Offset;
                            var dist = value.AsWord() - (currentAssemblyAddress + 2);
                            if (dist < -128 || dist > 127)
                            {
                                ReportError(Errors.Z0022, fixup.SourceLine, dist);
                                success = false;
                                break;
                            }
                            emittedCode[fixup.Offset + 1] = (byte)dist;
                            break;

                        case FixupType.Ent:
                            Output.EntryAddress = value.AsWord();
                            break;

                        case FixupType.Xent:
                            Output.ExportEntryAddress = value.AsWord();
                            break;
                    }
                }
                else
                {
                    success = false;
                }
            }

            // --- #3: fix Struct
            foreach (var fixup in fixups.Where(f => !f.Resolved && f.Type == FixupType.Struct))
            {
                // TODO: Fix Structs
            }

            // --- #4: fix FieldBit8, and FieldBit16
            foreach (var fixup in fixups.Where(f => !f.Resolved 
                && (f.Type == FixupType.FieldBit8 || f.Type == FixupType.FieldBit16)))
            {
                if (EvaluateFixupExpression(fixup, true, signNotEvaluable, out var value))
                {
                    var segment = Output.Segments[fixup.SegmentIndex];
                    var emittedCode = segment.EmittedCode;
                    switch (fixup.Type)
                    {
                        case FixupType.FieldBit8:
                            emittedCode[fixup.Offset] = value.AsByte();
                            break;

                        case FixupType.FieldBit16:
                            emittedCode[fixup.Offset] = value.AsByte();
                            emittedCode[fixup.Offset + 1] = (byte)(value.AsWord() >> 8);
                            break;
                    }
                }
                else
                {
                    success = false;
                }
            }
            return success;
        }

        /// <summary>
        /// Evaluates the fixup entry
        /// </summary>
        /// <param name="fixup"></param>
        /// <param name="numericOnly">Signs if only numeric expressions are accepted</param>
        /// <param name="signNotEvaluable">Raise error if the symbol is not evaluable</param>
        /// <param name="exprValue">The value of the expression</param>
        /// <returns>True, if evaluation successful; otherwise, false</returns>
        private bool EvaluateFixupExpression(FixupEntry fixup, bool numericOnly, bool signNotEvaluable, 
            out ExpressionValue exprValue)
        {
            exprValue = new ExpressionValue(0L);
            ExpressionNode.ClearErrors();
            if (!fixup.Expression.ReadyToEvaluate(fixup))
            {
                if (signNotEvaluable)
                {
                    ReportError(Errors.Z0201, fixup.SourceLine, ExpressionNode.SymbolErrors);
                }
                return false;
            }

            // --- Now resolve the fixup
            exprValue = fixup.Expression.Evaluate(fixup);
            fixup.Resolved = true;

            // --- Check, if resolution was successful
            if (fixup.Expression.EvaluationError != null)
            {
                ReportError(Errors.Z0200, fixup.SourceLine, fixup.Expression.EvaluationError);
                return false;
            }
            if (numericOnly && exprValue.Type == ExpressionValueType.String)
            {
                ReportError(Errors.Z0305, fixup.SourceLine);
                return false;
            }

            // --- Ok, no error
            return true;
        }

        #endregion
    }
}