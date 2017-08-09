namespace Spect.Net.SpectrumEmu.Abstraction.Discovery
{
    /// <summary>
    /// This class provides information about a stack pointer manipulation
    /// event
    /// </summary>
    public class StackPointerManipulationEvent
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
        public ushort OldValue { get; }

        /// <summary>
        /// New SP value
        /// </summary>
        public ushort NewValue { get; }

        /// <summary>
        /// CPU tacts after the operation
        /// </summary>
        public long Tacts { get; }

        public StackPointerManipulationEvent(ushort operationAddress, string operation, 
            ushort oldValue, ushort newValue, long tacts)
        {
            OperationAddress = operationAddress;
            Operation = operation;
            OldValue = oldValue;
            NewValue = newValue;
            Tacts = tacts;
        }
    }
}