using System.Collections.Generic;
using Spect.Net.Assembler.Generated;
using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// Represents the LOCAL statement
    /// </summary>
    /// <remarks>
    /// This instruction is silent, it has been implemented to support ZXBASIC's LOCAL
    /// statement
    /// </remarks>
    public sealed class LocalStatement : StatementBase
    {
        /// <summary>
        /// Loop expression
        /// </summary>
        public List<string> Locals { get; }

        public LocalStatement(IZ80AsmVisitorContext visitorContext, Z80AsmParser.LocalStatementContext context)
        {
            if (context.IDENTIFIER() == null) return;

            Locals = new List<string>();
            foreach (var expr in context.IDENTIFIER())
            {
                Locals.Add(expr.GetText());
                visitorContext.AddIdentifier(expr);
            }
        }

    }
}