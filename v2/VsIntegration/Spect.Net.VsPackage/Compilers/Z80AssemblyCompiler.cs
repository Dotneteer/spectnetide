using System;
using Spect.Net.Assembler.Assembler;
using Spect.Net.VsPackage.VsxLibrary.Output;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
// ReSharper disable SuspiciousTypeConversion.Global

namespace Spect.Net.VsPackage.Compilers
{
    /// <summary>
    /// This class represents the Z80 AssemblyCompiler
    /// </summary>
    public class Z80AssemblyCompiler: ICompilerService
    {
        /// <summary>
        /// The name of the service
        /// </summary>
        public string ServiceName => "SpectNetIde Z80 Assembler";

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
            compiler.AssemblerMessageCreated += OnAssemblerMessage;
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
                compiler.AssemblerMessageCreated -= OnAssemblerMessage;
            }
            return true;
        }

        /// <summary>
        /// Responds to the event when the Z80 assembler releases a message
        /// </summary>
        private void OnAssemblerMessage(object sender, AssemblerMessageArgs e)
        {
            var pane = OutputWindow.GetPane<Z80AssemblerOutputPane>();
            pane.WriteLine(e.Message);
        }
    }
}

#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
