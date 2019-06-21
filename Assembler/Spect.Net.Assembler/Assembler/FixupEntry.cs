using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Spect.Net.Assembler.SyntaxTree;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a fixup that recalculates and replaces
    /// unresolved symbol value at the end of the compilation
    /// </summary>
    public class FixupEntry : IEvaluationContext
    {
        /// <summary>
        /// The parent evaluation context
        /// </summary>
        public IEvaluationContext ParentContext { get; }

        /// <summary>
        /// The module of the evaluation context
        /// </summary>
        public AssemblyModule Module { get; }

        /// <summary>
        /// The source line that belongs to the fixup
        /// </summary>
        public SourceLineBase SourceLine { get; }

        /// <summary>
        /// Type of the fixup
        /// </summary>
        public FixupType Type { get; }

        /// <summary>
        /// Affected code segment
        /// </summary>
        public int SegmentIndex { get; }

        /// <summary>
        /// Offset within the code segment
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Expression to evaluate
        /// </summary>
        public ExpressionNode Expression { get; }

        /// <summary>
        /// Structure bytes to emit
        /// </summary>
        public Dictionary<ushort, byte> StructBytes { get; }

        /// <summary>
        /// Signs if the fixup is resolved
        /// </summary>
        public bool Resolved { get; set; }

        /// <summary>
        /// Gets the optional label, provided the fixup is FixupType.Equ.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public FixupEntry(IEvaluationContext parentContext, AssemblyModule module,
            SourceLineBase sourceLine, FixupType type, 
            int segmentIndex, int offset, ExpressionNode expression, string label = null,
            Dictionary<ushort, byte> structBytes = null)
        {
            ParentContext = parentContext;
            Module = module;
            SourceLine = sourceLine;
            Type = type;
            SegmentIndex = segmentIndex;
            Offset = offset;
            Expression = expression;
            Resolved = false;
            Label = label;
            StructBytes = structBytes;
        }

        /// <summary>
        /// Gets the current assembly address
        /// </summary>
        public ushort GetCurrentAddress() => ParentContext.GetCurrentAddress();

        /// <summary>
        /// Gets the value of the specified symbol
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <param name="startFromGlobal">Should resolution start from global scope?</param>
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        public (ExpressionValue ExprValue, IHasUsageInfo UsageInfo) GetSymbolValue(string symbol, 
            bool startFromGlobal = false)
        {
            (ExpressionValue ExprValue, IHasUsageInfo UsageInfo) resolved;
            if (startFromGlobal)
            {
                // --- Most be a compound symbol
                resolved = Module.ResolveCompoundSymbol(symbol, true);
            }
            else if (symbol.Contains("."))
            {
                resolved = Module.ResolveCompoundSymbol(symbol, false);
                if (resolved.ExprValue == null)
                {
                    resolved = Module.ResolveSimpleSymbol(symbol);
                }
            }
            else
            {
                resolved = Module.ResolveSimpleSymbol(symbol);
            }
            return resolved.ExprValue != null ? resolved : ParentContext.GetSymbolValue(symbol, startFromGlobal);
        }

        /// <summary>
        /// Gets the current loop counter value
        /// </summary>
        public ExpressionValue GetLoopCounterValue() => ParentContext.GetLoopCounterValue();
    }
}