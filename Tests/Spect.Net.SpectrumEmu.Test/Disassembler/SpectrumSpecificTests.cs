using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Disassembler
{
    [TestClass]
    public class SpectrumSpecificTests
    {
        [TestMethod]
        public void Rst08WorksAsExpected()
        {
            // --- Act
            Z80Tester.Test(SpectrumSpecificDisassemblyFlags.Spectrum48All,
                new []{"rst #08", ".defb #0a"}, 0xCF, 0x0A);
        }

        [TestMethod]
        public void Rst08GoesOnAsExpected()
        {
            // --- Act
            Z80Tester.Test(SpectrumSpecificDisassemblyFlags.Spectrum48All, 
                new[] { "rst #08", ".defb #0a", "nop" }, 0xCF, 0x0A, 0x00);
        }
    }
}