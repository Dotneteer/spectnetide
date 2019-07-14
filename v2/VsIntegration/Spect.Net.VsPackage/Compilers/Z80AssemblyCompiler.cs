using System;
using Spect.Net.Assembler.Assembler;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
// ReSharper disable SuspiciousTypeConversion.Global

namespace Spect.Net.VsPackage.Compilers
{
    /// <summary>
    /// This class represents the Z80 AssemblyCompiler
    /// </summary>
    public class Z80AssemblyCompiler: ICompilerService
    {
        private EventHandler<AssemblerMessageArgs> _traceMessageHandler;

        /// <summary>
        /// Gets the handler that displays trace messages
        /// </summary>
        /// <returns>Trace message handler</returns>
        public EventHandler<AssemblerMessageArgs> GetTraceMessageHandler()
        {
            return _traceMessageHandler;
        }

        /// <summary>
        /// Sets the handler that displays trace messages
        /// </summary>
        /// <param name="messageHandler">Message handler to use</param>
        public void SetTraceMessageHandler(EventHandler<AssemblerMessageArgs> messageHandler)
        {
            _traceMessageHandler = messageHandler;
        }

        /// <summary>
        /// Compiles the specified Visua Studio document.
        /// </summary>
        /// <param name="itemPath">VS document item path</param>
        /// <param name="options">Assembler options to use</param>
        /// <param name="output">Assembler output</param>
        /// <returns>True, if compilation is successful; otherwise, false</returns>
        public bool CompileDocument(string itemPath, 
            AssemblerOptions options, 
            out AssemblerOutput output)
        {
            output = null;
            var compiler = new Z80Assembler();
            if (_traceMessageHandler != null)
            {
                compiler.AssemblerMessageCreated += _traceMessageHandler;
            }
            try
            {
                output = compiler.CompileFile(itemPath, options);
            }
            finally
            {
                if (_traceMessageHandler != null)
                {
                    compiler.AssemblerMessageCreated -= _traceMessageHandler;
                }
            }
            return true;
        }
    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
