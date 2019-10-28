using System;
using System.Threading.Tasks;
using Spect.Net.Assembler.Assembler;
using Spect.Net.VsPackage.VsxLibrary.Output;

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
// ReSharper disable SuspiciousTypeConversion.Global

namespace Spect.Net.VsPackage.Compilers
{
    /// <summary>
    /// This class represents the Z80 Assembly compiler
    /// </summary>
    public class Z80AssemblyCompiler: ICompilerService
    {
        /// <summary>
        /// Tests if the compiler is available.
        /// </summary>
        /// <returns>True, if the compiler is installed, and so available.</returns>
        public Task<bool> IsAvailable()
        {
            return Task.FromResult(true);
        }

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
        public Task<AssemblerOutput> CompileDocument(string itemPath, 
            AssemblerOptions options)
        {
            var tcs = new TaskCompletionSource<AssemblerOutput>();
            _ = Task.Factory.StartNew(() =>
            {
                var compiler = new Z80Assembler();
                if (_traceMessageHandler != null)
                {
                    compiler.AssemblerMessageCreated += _traceMessageHandler;
                }
                compiler.AssemblerMessageCreated += OnAssemblerMessage;
                try
                {
                    var output = compiler.CompileFile(itemPath, options);
                    tcs.SetResult(output);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
                finally
                {
                    if (_traceMessageHandler != null)
                    {
                        compiler.AssemblerMessageCreated -= _traceMessageHandler;
                    }
                    compiler.AssemblerMessageCreated -= OnAssemblerMessage;
                }
            });
            return tcs.Task;
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
