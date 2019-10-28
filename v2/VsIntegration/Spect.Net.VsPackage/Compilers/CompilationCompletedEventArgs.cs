using Spect.Net.Assembler.Assembler;
using System;

namespace Spect.Net.VsPackage.Compilers
{
    /// <summary>
    /// This class represents the argument of a compilation completed event
    /// </summary>
    public class CompilationCompletedEventArgs : EventArgs
    {
        public AssemblerOutput Output { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.EventArgs" /> class.</summary>
        public CompilationCompletedEventArgs(AssemblerOutput output)
        {
            Output = output;
        }
    }
}
