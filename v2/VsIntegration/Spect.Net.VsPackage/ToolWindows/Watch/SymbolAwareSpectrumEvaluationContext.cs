using Spect.Net.Assembler.Assembler;
using Spect.Net.EvalParser.SyntaxTree;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Compilers;
using System;

namespace Spect.Net.VsPackage.ToolWindows.Watch
{
    /// <summary>
    /// Evaluation context that uses the ZX Spectrum virtual machine and the current
    /// assembler output
    /// </summary>
    public class SymbolAwareSpectrumEvaluationContext : SpectrumEvaluationContext, IDisposable
    {
        /// <summary>
        /// Compiler output
        /// </summary>
        public AssemblerOutput CompilerOutput { get; private set; }

        public SymbolAwareSpectrumEvaluationContext(ISpectrumVm spectrumVm) : base(spectrumVm)
        {
            // TODO: Implement this
            //SpectNetPackage.Default.CodeManager.CompilationCompleted += OnCompilationCompleted;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // TODO: Implement this
            //SpectNetPackage.Default.CodeManager.CompilationCompleted -= OnCompilationCompleted;
        }

        /// <summary>
        /// Catch the event of compilation
        /// </summary>
        private void OnCompilationCompleted(object sender, CompilationCompletedEventArgs e)
        {
            CompilerOutput = e.Output;
        }

        /// <summary>
        /// Gets the value of the specified symbol
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        public override ExpressionValue GetSymbolValue(string symbol)
        {
            return CompilerOutput != null && CompilerOutput.Symbols.TryGetValue(symbol, out var symbolValue)
                ? new ExpressionValue(symbolValue.Value)
                : ExpressionValue.Error;
        }
    }
}
