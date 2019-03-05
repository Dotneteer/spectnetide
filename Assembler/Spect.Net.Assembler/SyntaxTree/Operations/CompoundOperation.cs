using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Operations
{
    /// <summary>
    /// This class represents a compound instruction that contains 
    /// additional arguments additionally to the mnemonic
    /// </summary>
    public sealed class CompoundOperation : EmittingOperationBase
    {
        /// <summary>
        /// First operands
        /// </summary>
        public Operand Operand { get; set; }

        /// <summary>
        /// Second operands
        /// </summary>
        public Operand Operand2 { get; set; }

        /// <summary>
        /// First operands
        /// </summary>
        public Operand Operand3 { get; set; }

        public CompoundOperation(IZ80AsmVisitorContext visitorContext, Z80AsmParser.CompoundOperationContext context) : base(context)
        {
            var operands = context.operand();
            if (operands == null) return;
            for (var i = 0; i < operands.Length; i++)
            {
                // --- Collect operands
                var operandCtx = operands[i];
                switch (i)
                {
                    case 0:
                        Operand = visitorContext.GetOperand(operandCtx);
                        break;
                    case 1:
                        Operand2 = visitorContext.GetOperand(operandCtx);
                        break;
                    default:
                        Operand3 = visitorContext.GetOperand(operandCtx);
                        break;
                }
            }
        }
    }
}