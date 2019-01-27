using System;
using Spect.Net.Assembler.Assembler;
using Spect.Net.EvalParser.SyntaxTree;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.VsPackage.Z80Programs;

namespace Spect.Net.VsPackage.ToolWindows.Watch
{
    /// <summary>
    /// Evaluation context that uses the Spectrum VM's current state
    /// </summary>
    public class SpectrumEvaluationContext: IExpressionEvaluationContext, IDisposable
    {
        /// <summary>
        /// The ZX Spectrum virtual machine
        /// </summary>
        public ISpectrumVm SpectrumVm { get;  }

        /// <summary>
        /// Compiler output
        /// </summary>
        public AssemblerOutput CompilerOutput { get; private set; }
        
        public SpectrumEvaluationContext(ISpectrumVm spectrumVm)
        {
            SpectrumVm = spectrumVm;
            SpectNetPackage.Default.CodeManager.CompilationCompleted += OnCompilationCompleted;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            SpectNetPackage.Default.CodeManager.CompilationCompleted -= OnCompilationCompleted;
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
        public ExpressionValue GetSymbolValue(string symbol)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current value of the specified Z80 register
        /// </summary>
        /// <param name="registerName">Name of the register</param>
        /// <param name="is8Bit">Is it an 8-bit register?</param>
        /// <returns>Z80 register value</returns>
        public ExpressionValue GetZ80RegisterValue(string registerName, out bool is8Bit)
        {
            is8Bit = true;
            var regs = SpectrumVm.Cpu.Registers;
            switch (registerName.ToLower())
            {
                case "a":
                    return new ExpressionValue(regs.A);
                case "b":
                    return new ExpressionValue(regs.B);
                case "c":
                    return new ExpressionValue(regs.C);
                case "d":
                    return new ExpressionValue(regs.D);
                case "e":
                    return new ExpressionValue(regs.E);
                case "h":
                    return new ExpressionValue(regs.H);
                case "l":
                    return new ExpressionValue(regs.L);
                case "f":
                    return new ExpressionValue(regs.F);
                case "i":
                    return new ExpressionValue(regs.I);
                case "r":
                    return new ExpressionValue(regs.R);
                case "xh":
                case "ixh":
                    return new ExpressionValue(regs.XH);
                case "xl":
                case "ixl":
                    return new ExpressionValue(regs.XL);
                case "yh":
                case "iyh":
                    return new ExpressionValue(regs.YH);
                case "yl":
                case "iyl":
                    return new ExpressionValue(regs.YL);
                case "af":
                    is8Bit = false;
                    return new ExpressionValue(regs.AF);
                case "bc":
                    is8Bit = false;
                    return new ExpressionValue(regs.BC);
                case "de":
                    is8Bit = false;
                    return new ExpressionValue(regs.DE);
                case "hl":
                    is8Bit = false;
                    return new ExpressionValue(regs.HL);
                case "af'":
                    is8Bit = false;
                    return new ExpressionValue(regs._AF_);
                case "bc'":
                    is8Bit = false;
                    return new ExpressionValue(regs._BC_);
                case "de'":
                    is8Bit = false;
                    return new ExpressionValue(regs._DE_);
                case "hl'":
                    is8Bit = false;
                    return new ExpressionValue(regs._HL_);
                case "ix":
                    is8Bit = false;
                    return new ExpressionValue(regs.IX);
                case "iy":
                    is8Bit = false;
                    return new ExpressionValue(regs.IY);
                case "pc":
                    is8Bit = false;
                    return new ExpressionValue(regs.PC);
                case "sp":
                    is8Bit = false;
                    return new ExpressionValue(regs.SP);
                case "wz":
                    is8Bit = false;
                    return new ExpressionValue(regs.WZ);
                default:
                    return ExpressionValue.Error;
            }
        }

        /// <summary>
        /// Gets the current value of the specified Z80 flag
        /// </summary>
        /// <param name="flagName">Name of the flag</param>
        /// <returns>Z80 register value</returns>
        public ExpressionValue GetZ80FlagValue(string flagName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current value of the memory pointed by the specified Z80 register
        /// </summary>
        /// <param name="registerName">Name of the register</param>
        /// <returns>Z80 register value</returns>
        public ExpressionValue GetZ80RegisterIndirectValue(string registerName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current value of the memory pointed by the specified Z80 register
        /// </summary>
        /// <param name="address">Memory address</param>
        /// <returns>Z80 register value</returns>
        public ExpressionValue GetMemoryIndirectValue(ExpressionValue address)
        {
            throw new NotImplementedException();
        }
    }
}