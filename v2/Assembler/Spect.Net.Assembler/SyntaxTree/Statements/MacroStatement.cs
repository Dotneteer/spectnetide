using System;
using System.Collections.Generic;
using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// This class represents a MACRO statement
    /// </summary>
    public sealed class MacroStatement: BlockStatementBase, ILabelSetter
    {
        /// <summary>
        /// Macro argument names
        /// </summary>
        public List<string> Arguments { get; }

        /// <summary>
        /// Type of end statement
        /// </summary>
        public override Type EndType => typeof(MacroEndStatement);

        /// <summary>
        /// Gets the name of the end statement
        /// </summary>
        public override string EndStatementName => "MEND";

        public MacroStatement(IZ80AsmVisitorContext visitorContext, Z80AsmParser.MacroStatementContext context)
        {
            Arguments = new List<string>();
            var paramIds = context.IDENTIFIER();
            foreach (var id in paramIds)
            {
                visitorContext.AddIdentifier(id);
                Arguments.Add(id.NormalizeToken());
            }
        }
    }
}