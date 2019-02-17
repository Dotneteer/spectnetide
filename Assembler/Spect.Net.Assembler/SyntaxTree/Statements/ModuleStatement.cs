using System;
using Spect.Net.Assembler.Generated;

// ReSharper disable StringLiteralTypo

namespace Spect.Net.Assembler.SyntaxTree.Statements
{
    /// <summary>
    /// This class represents a MACRO statement
    /// </summary>
    public sealed class ModuleStatement: BlockStatementBase
    {
        /// <summary>
        /// Module name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of end statement
        /// </summary>
        public override Type EndType => typeof(ModuleEndStatement);

        /// <summary>
        /// Gets the name of the end statement
        /// </summary>
        public override string EndStatementName => "MODULEEND";

        public ModuleStatement(Z80AsmParser.ModuleStatementContext context)
        {
            if (context.IDENTIFIER() != null)
            {
                Name = context.IDENTIFIER().GetText().ToUpper();
            }
        }
    }
}