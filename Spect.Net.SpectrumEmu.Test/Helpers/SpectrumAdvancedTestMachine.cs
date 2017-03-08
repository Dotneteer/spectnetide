using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Devices;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class SpectrumAdvancedTestMachine: Spectrum48
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public SpectrumAdvancedTestMachine(DisplayParameters pars = null, IScreenPixelRenderer renderer = null): 
            base(new ResourceRomProvider(), 
                new HighResolutionClockProvider(), 
                renderer ?? new TestPixelRenderer(pars ?? new DisplayParameters()))
        {
        }

        /// <summary>
        /// Initializes the code passed in <paramref name="programCode"/>. This code
        /// is put into the memory from <paramref name="codeAddress"/> and
        /// </summary>
        /// <param name="programCode">Program code</param>
        /// <param name="codeAddress">Address of first code byte</param>
        /// <param name="startAddress">Code start address, null if same as the first byte</param>
        public void InitCode(IEnumerable<byte> programCode = null, ushort codeAddress = 0x8000, 
            ushort? startAddress = null)
        {
            if (programCode == null) return;
            if (startAddress == null) startAddress = codeAddress;

            // --- Initialize the code
            foreach (var op in programCode)
            {
                WriteMemory(codeAddress++, op);
            }
            while (codeAddress != 0)
            {
                WriteMemory(codeAddress++, 0);
            }

            Cpu.Reset();
            Cpu.Registers.PC = startAddress.Value;
        }
    }
}