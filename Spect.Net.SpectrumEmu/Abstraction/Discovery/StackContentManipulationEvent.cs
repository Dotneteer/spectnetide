namespace Spect.Net.SpectrumEmu.Abstraction.Discovery
{
    /// <summary>
    /// This class provides information about the manipulation of the stack's contents
    /// event
    /// </summary>
    public class StackContentManipulationEvent
    {
        /// <summary>
        /// Address of the operation that modified the stack
        /// </summary>
        public ushort OperationAddress { get; }

        /// <summary>
        /// The operation that modified the stack
        /// </summary>
        public string Operation { get; }

        /// <summary>
        /// SP value before th operation
        /// </summary>
        public ushort SpValue { get; }

        /// <summary>
        /// Value put on the stack
        /// </summary>
        public ushort Content { get; }

        /// <summary>
        /// CPU tact after the operation
        /// </summary>
        public long Tacts { get; }

        public StackContentManipulationEvent(ushort operationAddress, string operation, ushort spValue, ushort content, long tacts)
        {
            OperationAddress = operationAddress;
            Operation = operation;
            SpValue = spValue;
            Content = content;
            Tacts = tacts;
        }
    }
}