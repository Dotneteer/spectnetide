using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;

namespace Spect.Net.SpectrumEmu.Cpu
{
    /// <summary>
    /// This class provides stack debug support
    /// </summary>
    public class DefaultStackDebugSupport : IStackDebugSupport
    {
        // --- The Step-Out stack
        private readonly Stack<ushort> _stepOutStack = new Stack<ushort>();

        /// <summary>
        /// Stack Pointer events
        /// </summary>
        public LruList<StackPointerManipulationEvent> StackPointerEvents { get; private set; }
            = new LruList<StackPointerManipulationEvent>();

        /// <summary>
        /// Stack content events
        /// </summary>
        public Dictionary<ushort, StackContentManipulationEvent> StackContentEvents { get; private set; }
            = new Dictionary<ushort, StackContentManipulationEvent>();

        /// <summary>
        /// Resets the debug support
        /// </summary>
        public void Reset()
        {
            StackPointerEvents = new LruList<StackPointerManipulationEvent>(16);
            StackContentEvents = new Dictionary<ushort, StackContentManipulationEvent>();
            ClearStepOutStack();
        }

        /// <summary>
        /// Records a stack pointer manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        public void RecordStackPointerManipulationEvent(StackPointerManipulationEvent ev)
        {
            StackPointerEvents.Add(ev);
        }

        /// <summary>
        /// Records a stack content manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        public void RecordStackContentManipulationEvent(StackContentManipulationEvent ev)
        {
            // --- Do not trace POP events
            if (ev.Content != null)
            {
                StackContentEvents[ev.SpValue] = ev;
            }
        }

        /// <summary>
        /// Checks if the Step-Out stack contains any information
        /// </summary>
        /// <returns></returns>
        public bool HasStepOutInfo()
        {
            return _stepOutStack.Count > 0;
        }

        /// <summary>
        /// The depth of the Step-Out stack
        /// </summary>
        public int StepOutStackDepth => _stepOutStack.Count;

        /// <summary>
        /// Clears the content of the Step-Out stack
        /// </summary>
        public void ClearStepOutStack()
        {
            _stepOutStack.Clear();
        }

        /// <summary>
        /// Pushes the specified return address to the Step-Out stack
        /// </summary>
        /// <param name="address"></param>
        public void PushStepOutAddress(ushort address)
        {
            _stepOutStack.Push(address);
        }

        /// <summary>
        /// Pops a Step-Out return point address from the stack
        /// </summary>
        /// <returns>Address popped from the stack</returns>
        /// <returns>Zero, if the Step-Out stack is empty</returns>
        public ushort PopStepOutAddress()
        {
            if (_stepOutStack.Count > 0)
            {
                StepOutAddress = _stepOutStack.Pop();
                return StepOutAddress.Value;
            }
            StepOutAddress = null;
            return 0;
        }

        /// <summary>
        /// Indicates that the last instruction executed by the CPU was a CALL
        /// </summary>
        public bool CallExecuted { get; set; }

        /// <summary>
        /// Indicates that the last instruction executed by the CPU was a RET
        /// </summary>
        public bool RetExecuted { get; set; }

        /// <summary>
        /// Gets the last popped Step-Out address
        /// </summary>
        public ushort? StepOutAddress { get; set; }
    }
}