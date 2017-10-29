using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.RomResources;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.SpectrumEmu.Test.Disassembler
{
    [TestClass]
    public class DisassembleToFileTest
    {
        private const string TMP_DIR = @"C:\Temp";

        [TestMethod]
        public void CreateFileTest()
        {
            // --- Arrange
            var romP = new ResourceRomProvider(typeof(ResourceRomProvider).Assembly);
            var romInfo = romP.LoadRom("ZXSpectrum48");
            romInfo.RomBytes.ShouldNotBeNull();
            var disassembler = new Z80Disassembler(romInfo.MemorySections, romInfo.RomBytes);
            var output = disassembler.Disassemble(0x0000, 0x7FF);
            if (!Directory.Exists(TMP_DIR))
            {
                Directory.CreateDirectory(TMP_DIR);
            }

            // --- Act
            using (var writer = File.CreateText(@"C:\Temp\Disassembly07ff.z80asm"))
            {
                disassembler.SaveDisassembly(writer, output, romInfo.Annotations);
            }
        }
    }
}
