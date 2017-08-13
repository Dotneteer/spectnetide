using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.SpectrumEmu.Test.Generators
{
    [TestClass]
    public class RomGenerators
    {
        [TestMethod]
        public void GenerateSpectrum48Annotations()
        {
            // --- Arrange
            var dc = new DisassemblyAnnotation();
            dc.MemoryMap.Add(new MemorySection(0x0000, 0x3BFF));
            dc.MemoryMap.Add(new MemorySection(0x3C00, 0x3FFF));
            dc.AddLiteral(0x04C2, "$SaveBytesRoutineAddress");
            dc.AddLiteral(0x0000, "$SaveBytesResumeAddress");
            dc.AddLiteral(0x056C, "$LoadBytesRoutineAddress");
            dc.AddLiteral(0x05E2, "$LoadBytesResumeAddress");
            dc.AddLiteral(0x05B6, "$LoadBytesInvalidHeaderAddress");

            // --- Act
            Console.WriteLine(dc.Serialize());
        }
    }
}
