using System;
using System.Threading.Tasks;
using Spect.Net.Assembler.Assembler;

namespace Spect.Net.VsPackage.Compilers
{
    /// <summary>
    /// This interface represents a compiler service that can create Z80 machine
    /// code from the specified document.
    /// </summary>
    public interface ICompilerService
    {
        /// <summary>
        /// Gets the name of the compiler service
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Tests if the compiler is available.
        /// </summary>
        /// <returns>True, if the compiler is installed, and so available.</returns>
        Task<bool> IsAvailable();

        /// <summary>
        /// Gets the handler that displays trace messages
        /// </summary>
        /// <returns>Trace message handler</returns>
        EventHandler<AssemblerMessageArgs> GetTraceMessageHandler();

        /// <summary>
        /// Sets the handler that displays trace messages
        /// </summary>
        /// <param name="messageHandler">Message handler to use</param>
        void SetTraceMessageHandler(EventHandler<AssemblerMessageArgs> messageHandler);

        /// <summary>
        /// Compiles the specified Visua Studio document.
        /// </summary>
        /// <param name="itemPath">VS document item path</param>
        /// <param name="options">Assembler options to use</param>
        /// <param name="output">Assembler output</param>
        /// <returns>True, if compilation is successful; otherwise, false</returns>
        Task<AssemblerOutput> CompileDocument(string itemPath,
            AssemblerOptions options);
    }
}