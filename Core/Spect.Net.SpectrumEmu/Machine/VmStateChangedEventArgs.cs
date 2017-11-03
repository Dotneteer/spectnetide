using System;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class represents the arguments of the event that signs that
    /// the state of the virtual machine changes
    /// </summary>
    public class VmStateChangedEventArgs: EventArgs
    {
        /// <summary>
        /// The previous state of the virtual machine
        /// </summary>
        public VmStatus OldState { get; }

        /// <summary>
        /// The new state of the virtual machine
        /// </summary>
        public VmStatus NewState { get; }

        /// <summary>
        /// Initializes the event arguments
        /// </summary>
        /// <param name="oldState">Old vm state</param>
        /// <param name="newState">New vm state</param>
        public VmStateChangedEventArgs(VmStatus oldState, VmStatus newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}