using System.Collections.Generic;

namespace Spect.Net.SpectrumEmu.Abstraction.Machine
{
    /// <summary>
    /// This interface defines the operations that support 
    /// running injected Z80 code on the Spectrum virtual machine.
    /// </summary>
    public interface ISpectrumVmRunCodeSupport
    {
        /// <summary>
        /// Injects code into the memory.
        /// </summary>
        /// <param name="addr">Start address.</param>
        /// <param name="code">Code to inject.</param>
        /// <remarks>The code leaves the ROM area untouched.</remarks>
        void InjectCodeToMemory(ushort addr, IReadOnlyCollection<byte> code);

        /// <summary>
        /// Injects code to the specified bank, starting at address 0
        /// </summary>
        /// <param name="bankIndex">Index of the bank</param>
        /// <param name="bankOffset">Offset within the bank</param>
        /// <param name="code">Code to insert</param>
        void InjectCodeToBank(int bankIndex, int bankOffset, IReadOnlyCollection<byte> code);

        /// <summary>
        /// Prepares the custom code for running, as if it were started
        /// with the RUN command.
        /// </summary>
        void PrepareRunMode(HashSet<string> options);

        /// <summary>
        /// Clears the screen
        /// </summary>
        void ClearScreen();
    }
}