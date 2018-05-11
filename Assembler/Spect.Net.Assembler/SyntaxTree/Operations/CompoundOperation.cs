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
    }
}