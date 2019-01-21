namespace Spect.Net.SpectrumEmu.Abstraction.Discovery
{
    /// <summary>
    /// This interface defines the operations that support debugging the call stack
    /// </summary>
    public interface IStackDebugSupport
    {
        /// <summary>
        /// Resets the debug support
        /// </summary>
        void Reset();

        /// <summary>
        /// Records a stack pointer manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        void RecordStackPointerManipulationEvent(StackPointerManipulationEvent ev);

        /// <summary>
        /// Records a stack content manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        void RecordStackContentManipulationEvent(StackContentManipulationEvent ev);

        /// <summary>
        /// Checks if the Step-Out stack contains any information
        /// </summary>
        /// <returns></returns>
        bool HasStepOutInfo();

        /// <summary>
        /// The depth of the Step-Out stack
        /// </summary>
        int StepOutStackDepth { get; }

        /// <summary>
        /// Clears the content of the Step-Out stack
        /// </summary>
        void ClearStepOutStack();

        /// <summary>
        /// Pushes the specified return address to the Step-Out stack
        /// </summary>
        /// <param name="address"></param>
        void PushStepOutAddress(ushort address);

        /// <summary>
        /// Pops a Step-Out return point address from the stack
        /// </summary>
        /// <returns>Address popped from the stack</returns>
        /// <returns>Zeor, if the Step-Out stack is empty</returns>
        ushort PopStepOutAddress();

        /// <summary>
        /// Indicates that the last instruction executed by the CPU was a CALL
        /// </summary>
        bool CallExecuted { get; set; }

        /// <summary>
        /// Indicates that the last instruction executed by the CPU was a RET
        /// </summary>
        bool RetExecuted { get; set; }

        /// <summary>
        /// Gets the last popped Step-Out address
        /// </summary>
        ushort? StepOutAddress { get; set; }
    }
}