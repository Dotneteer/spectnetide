namespace Spect.Net.SpectrumEmu.Abstraction.Discovery
{
    /// <summary>
    /// This class provides information about a branching event
    /// </summary>
    public class BranchEvent
    {
        /// <summary>
        /// Address of the operation that modified SP
        /// </summary>
        public ushort OperationAddress { get; }

        /// <summary>
        /// The operation that modified SP
        /// </summary>
        public string Operation { get; }

        /// <summary>
        /// Old SP value
        /// </summary>
        public ushort JumpAddress { get; }

        /// <summary>
        /// CPU tacts after the operation
        /// </summary>
        public long Tacts { get; }

        public BranchEvent(ushort operationAddress, string operation, ushort jumpAddress, long tacts)
        {
            OperationAddress = operationAddress;
            Operation = operation;
            JumpAddress = jumpAddress;
            Tacts = tacts;
        }
    }
}