using System;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// Event arguments for sending assembler message to an output
    /// </summary>
    public class AssemblerMessageArgs: EventArgs
    {
        /// <summary>
        /// The message sent to output
        /// </summary>
        public string Message { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.EventArgs" /> class.</summary>
        public AssemblerMessageArgs(string message)
        {
            Message = message;
        }
    }
}