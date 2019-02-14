using System;
using System.Collections.Generic;
using System.Linq;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Expressions;

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
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        ExpressionValue IEvaluationContext.GetSymbolValue(string symbol)
        {
            // --- Check the local scope in stack order
            foreach (var scope in _output.LocalScopes)
            {
                if (scope.Symbols.TryGetValue(symbol, out var localSymbolValue))
                {
                    return localSymbolValue.Value;
                }
            }

            // --- Check the global scope
            return _output.Symbols.TryGetValue(symbol, out var symbolValue) 
                ? symbolValue.Value 
                : null;
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

            var scope = _output.LocalScopes.Peek();
            if (!scope.IsLoopScope)
            {
                ReportError(Errors.Z0412, CurrentSourceLine);
                return ExpressionValue.Error;
            }

            return new ExpressionValue(scope.LoopCounter);
        }

        #region Symbol handler methods

        /// <summary>
        /// Tests if the current assembly instruction is in the global scope
        /// </summary>
        public bool IsInGlobalScope => _output.LocalScopes.Count == 0;

        /// <summary>
        /// Checks is the specified error should be reported in the local scope
        /// </summary>
        /// <param name="errorCode">Error code to check</param>
        /// <returns></returns>
        public bool ShouldReportErrorInCurrentScope(string errorCode)
        {
            if (IsInGlobalScope) return true;
            var localScope = _output.LocalScopes.Peek();
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
            var lookup = _output.LocalScopes.Count > 0
                ? _output.LocalScopes.Peek().Symbols
                : _output.Symbols;
            return lookup.TryGetValue(symbol, out var symbolInfo) && symbolInfo.Type == SymbolType.Label;
        }

        /// <summary>
        /// Adds a symbol to the current scope
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="value"></param>
        public void AddSymbol(string symbol, ExpressionValue value)
        {
            var lookup = _output.LocalScopes.Count > 0
                ? _output.LocalScopes.Peek().Symbols
                : _output.Symbols;
            lookup.Add(symbol, new AssemblySymbolInfo(symbol, SymbolType.Label, value));
        }

        /// <summary>
        /// Checks if the variable with the specified name exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool VariableExists(string name)
        {
            // --- Search for the variable from inside out
            foreach (var scope in _output.LocalScopes)
            {
                if (scope.Symbols.TryGetValue(name, out var symbolInfo) && symbolInfo.Type == SymbolType.Var)
                {
                    return true;
                }
            }

            // --- Check the global scope
            return _output.Symbols.TryGetValue(name, out var globalSymbol) 
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
            foreach (var scope in _output.LocalScopes)
            {
                if (scope.Symbols.TryGetValue(name, out var symbolInfo) && symbolInfo.Type == SymbolType.Var)
                {
                    symbolInfo.Value = value;
                    return;
                }
            }

            // --- Check the global scope
            if (_output.Symbols.TryGetValue(name, out var globalSymbol) 
                && globalSymbol.Type == SymbolType.Var)
            {
                globalSymbol.Value = value;
                return;
            }

            // --- The variable does not exist, create it in the current scope
            var vars = _output.LocalScopes.Count > 0
                ? _output.LocalScopes.Peek().Symbols
                : _output.Symbols;
            vars[name] = new AssemblySymbolInfo(name, SymbolType.Var, value);
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
            if (_output.Symbols.TryGetValue(symbol, out var symbolInfo))
            {
                symbolInfo.Value = value;
            }
            else
            {
                _output.Symbols.Add(symbol, new AssemblySymbolInfo(symbol, SymbolType.Label, value));
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
        private void RecordFixup(SourceLineBase opLine, FixupType type, ExpressionNode expression, string label = null)
        {
            var localScope = IsInGlobalScope ? null : _output.LocalScopes.Peek();
            var fixup = new FixupEntry(this, localScope, opLine, type, _output.Segments.Count - 1,
                CurrentSegment.CurrentOffset, expression, label);
            foreach (var scope in _output.LocalScopes)
            {
                scope.Fixups.Add(fixup);
            }
            _output.Fixups.Add(fixup);
        }

        /// <summary>
        /// Fixes the unresolved symbol in the last phase of compilation
        /// </summary>
        /// <returns></returns>
        private bool FixupSymbols()
        {
            // --- Go through all scopes from inside to outside
            foreach (var scope in _output.LocalScopes)
            {
                if (FixupSymbols(scope.Fixups, scope.Symbols, false))
                {
                    // --- Successful fixup in the local scope
                    return true;
                }
            }

            return FixupSymbols(_output.Fixups, _output.Symbols, true);
        }

        /// <summary>
        /// Tries to create fixups in the specified scope
        /// </summary>
        /// <param name="fixups">Fixup entries in the scope</param>
        /// <param name="symbols">Symbols in the scope</param>
        /// <param name="signNotEvaluable">Raise error if the symbol is not evaluable</param>
        /// <returns></returns>
        private bool FixupSymbols(List<FixupEntry> fixups, Dictionary<string, AssemblySymbolInfo> symbols, bool signNotEvaluable)
        {
            // --- First, fix the .equ values
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
                        symbols.Add(equ.Label, new AssemblySymbolInfo(equ.Label, SymbolType.Label, value));
                    }
                }
                else
                {
                    success = false;
                }
            }

            // --- Second, fix all the other values
            foreach (var fixup in fixups.Where(f => f.Type != FixupType.Equ && !f.Resolved))
            {
                if (EvaluateFixupExpression(fixup, true, signNotEvaluable, out var value))
                {
                    var segment = _output.Segments[fixup.SegmentIndex];
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
                            _output.EntryAddress = value.AsWord();
                            break;

                        case FixupType.Xent:
                            _output.ExportEntryAddress = value.AsWord();
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