using System;

namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// Event arguments for the VmStoppedWithException events
    /// </summary>
    public class VmStoppedWithExceptionEventArgs: EventArgs
    {
        /// <summary>
        /// Exception that stopped the virtual machine
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
        /// </summary>
        public VmStoppedWithExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}