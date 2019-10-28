using System;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// Thsi class signs that the VM state is invalid
    /// </summary>
    public class InvalidVmStateException: Exception
    {
        /// <summary>
        /// Original exception message
        /// </summary>
        public string OriginalMessage { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message.</summary>
        /// <param name="message">The message that describes the error. </param>
        public InvalidVmStateException(string message) : base(message)
        {
            OriginalMessage = message;
        }
    }
}