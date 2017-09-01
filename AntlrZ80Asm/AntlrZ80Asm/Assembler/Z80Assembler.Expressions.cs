using System;
using System.Collections.Generic;
using AntlrZ80Asm.SyntaxTree.Expressions;
// ReSharper disable InlineOutVariableDeclaration

namespace AntlrZ80Asm.Assembler
{
    /// <summary>
    /// This class implements the Z80 assembler
    /// </summary>
    public partial class Z80Assembler: IEvaluationContext
    {
        /// <summary>
        /// The symbol table with properly defined symbols
        /// </summary>
        public Dictionary<string, ushort> Symbols { get; } = 
            new Dictionary<string, ushort>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// The list of fixups to carry out as the last phase of the compilation
        /// </summary>
        public List<FixupEntry> Fixups { get; } = new List<FixupEntry>();

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
            return Symbols.TryGetValue(symbol, out symbolValue)
                ? symbolValue
                : (ushort?) null;
        }

        /// <summary>
        /// Sets the current value of the symbol to the specified van
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="value">Symbol value</param>
        public void SetSymbolValue(string symbol, ushort value)
            => Symbols[symbol] = value;

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
        /// Records fixup information
        /// </summary>
        /// <param name="type">Fixup type</param>
        /// <param name="expression">Fixup expression</param>
        private void RecordFixup(FixupType type, ExpressionNode expression)
        {
            Fixups.Add(new FixupEntry(type, _output.Segments.Count - 1, CurrentSegment.CurrentOffset, expression));
        }
    }
}