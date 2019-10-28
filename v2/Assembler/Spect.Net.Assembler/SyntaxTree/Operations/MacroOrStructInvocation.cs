using System.Collections.Generic;
using System.Linq;
using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents a macro or struct invocation statement
    /// </summary>
    public class MacroOrStructInvocation : StatementBase
    {
        /// <summary>
        /// Name of the macro to invoke
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Actual parameters of the macro invocation
        /// </summary>
        public List<Operand> Parameters { get; }

        public MacroOrStructInvocation(IZ80AsmVisitorContext visitorContext, Z80AsmParser.MacroOrStructInvocationContext context)
        {
            Parameters = new List<Operand>();
            if (context.macroArgument().Length > 1
                || context.macroArgument().Length > 0 && context.macroArgument()[0].operand() != null)
            {
                Parameters.AddRange(context.macroArgument()
                    .Select(arg => arg.operand() != null
                        ? visitorContext.GetOperand(arg.operand())
                        : new Operand()));
            }

            KeywordSpan = new TextSpan(context.Start.StartIndex, context.Start.StopIndex + 1);
            Name = context.IDENTIFIER().NormalizeToken();
        }
    }
}