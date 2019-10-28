using System;
using System.Collections.Generic;
using Spect.Net.Assembler.SyntaxTree.Operations;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class provides a description for the processing rules of
    /// a particular compound operation associated with a mnemonic
    /// </summary>
    public class CompoundOperationDescriptor
    {
        /// <summary>
        /// Allow these operand rules
        /// </summary>
        public List<OperandRule> Allow { get; }

        /// <summary>
        /// Action that processes the particular mnemonic
        /// </summary>
        public Action<Z80Assembler, CompoundOperation> ProcessAction { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public CompoundOperationDescriptor(List<OperandRule> allow, 
            Action<Z80Assembler, CompoundOperation> processAction)
        {
            Allow = allow;
            ProcessAction = processAction;
        }
    }
}