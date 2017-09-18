using System;
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
        ushort IEvaluationContext.GetCurrentAddress() => GetCurrentAssemblyAddress();

        /// <summary>
        /// Gets the value of the specified symbol
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        ushort? IEvaluationContext.GetSymbolValue(string symbol)
        {
            ushort symbolValue;
            return _output.Symbols.TryGetValue(symbol, out symbolValue)
                ? symbolValue
                : (ushort?) null;
        }

        #region Evaluation methods

        /// <summary>
        /// Sets the current value of the symbol to the specified van
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="value">Symbol value</param>
        public void SetSymbolValue(string symbol, ushort value)
            => _output.Symbols[symbol] = value;

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expr">Expression to evaluate</param>
        /// <returns>
        /// Null, if the expression cannot be evaluated, or evaluation 
        /// results an error (e.g. divide by zero)
        /// </returns>
        public ushort? Eval(ExpressionNode expr)
        {
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }
            if (!expr.ReadyToEvaluate(this)) return null;
            var result = expr.Evaluate(this);
            return expr.EvaluationError != null 
                ? (ushort?) null 
                : result;
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
        public ushort? EvalImmediate(SourceLineBase opLine, ExpressionNode expr)
        {
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }
            if (!expr.ReadyToEvaluate(this))
            {
                _output.Errors.Add(new ExpressionEvaluationError("Z0201", opLine));
                return null;
            }
            var result = expr.Evaluate(this);
            if (expr.EvaluationError == null) return result;

            _output.Errors.Add(new ExpressionEvaluationError("Z0200", opLine, expr.EvaluationError));
            return null;
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
            _output.Fixups.Add(new FixupEntry(opLine, type, _output.Segments.Count - 1,
                CurrentSegment.CurrentOffset, expression, label));
        }

        /// <summary>
        /// Fixes the unresolved symbol in the last phase of compilation
        /// </summary>
        /// <returns></returns>
        private bool FixupSymbols()
        {
            // --- First, fix the .equ values
            var success = true;
            foreach (var equ in _output.Fixups.Where(f => f.Type == FixupType.Equ))
            {
                ushort value;
                if (EvaluateFixupExpression(equ, out value))
                {
                    _output.Symbols[equ.Label] = value;
                }
                else
                {
                    success = false;
                }
            }

            // --- Second, fix all the other values
            foreach (var fixup in _output.Fixups.Where(f => f.Type != FixupType.Equ))
            {
                ushort value;
                if (EvaluateFixupExpression(fixup, out value))
                {
                    var segment = _output.Segments[fixup.SegmentIndex];
                    var emittedCode = segment.EmittedCode;
                    switch (fixup.Type)
                    {
                        case FixupType.Bit8:
                            emittedCode[fixup.Offset] = (byte) value;
                            break;

                        case FixupType.Bit16:
                            emittedCode[fixup.Offset] = (byte)value;
                            emittedCode[fixup.Offset + 1] = (byte)(value >> 8);
                            break;

                        case FixupType.Jr:
                            // --- Check for Relative address
                            var currentAssemblyAddress = segment.StartAddress 
                                + (segment.Displacement ?? 0)
                                + fixup.Offset;
                            var dist = value - (currentAssemblyAddress + 2);
                            if (dist < -128 || dist > 127)
                            {
                                _output.Errors.Add(new RelativeAddressError(fixup.SourceLine, dist));
                                success = false;
                                break;
                            }
                            emittedCode[fixup.Offset + 1] = (byte) dist;
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
        /// <param name="exprValue">The value of the expression</param>
        /// <returns>True, if evaluation successful; otherwise, false</returns>
        private bool EvaluateFixupExpression(FixupEntry fixup, out ushort exprValue)
        {
            exprValue = 0;
            if (!fixup.Expression.ReadyToEvaluate(this))
            {
                _output.Errors.Add(new FixupError("Z0201", fixup.SourceLine));
                return false;
            }
            exprValue = fixup.Expression.Evaluate(this);
            if (fixup.Expression.EvaluationError != null)
            {
                _output.Errors.Add(new FixupError("Z0200", fixup.SourceLine,
                    fixup.Expression.EvaluationError));
                return false;
            }
            return true;
        }

        #endregion
    }
}