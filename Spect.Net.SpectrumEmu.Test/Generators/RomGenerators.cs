using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Test.Generators
{
    [TestClass]
    public class RomGenerators
    {
        [TestMethod]
        public void GenerateSpectrum48Annotations()
        {
            // --- Arrange
            var osInfo = new RomInfo
            {
                Name = "ZXSpectrum48",
                MemoryMap = new List<MemorySection>
                {
                    new MemorySection(0x0000, 0x3D00),
                    new MemorySection(0x3D00, 0x0300, MemorySectionType.ByteArray),
                    new MemorySection(0x4000, 0x1B00, MemorySectionType.Skip)
                },
                SaveBytesRoutineAddress = 0x4C2,
                LoadBytesRoutineAddress = 0x56C
            };

            // --- Act
            Console.WriteLine(RomInfo.SerializeToJson(osInfo));
        }
    }
}
