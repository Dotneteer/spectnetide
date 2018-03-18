using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Devices.DivIde;

namespace Spect.Net.SpectrumEmu.Test.Devices.DivIde
{
    [TestClass]
    public class MmcStorageTests
    {
        public const string MMC_FILENAME = @"C:\Temp\MmcFiles\mmcfile.mmc";

        [TestMethod]
        [DataRow(0, 32, 512)]
        [DataRow(32, 32, 512)]
        [DataRow(33, 32, 512)]
        [DataRow(63, 32, 512)]
        [DataRow(64, 64, 1024)]
        [DataRow(127, 64, 1024)]
        [DataRow(128, 128, 2048)]
        [DataRow(255, 128, 2048)]
        [DataRow(256, 256, 4096)]
        [DataRow(512, 256, 4096)]
        [DataRow(1024, 256, 4096)]
        public void CreateAdjustsSizeInMb(int requestedSize, int createdSize, int maxBlocks)
        {
            // --- Act
            var mmc = MmcStorage.Create(MMC_FILENAME, requestedSize);

            // --- Assert
            mmc.SizeInMb.ShouldBe(createdSize);
            mmc.MBlocks.ShouldBe((ushort)maxBlocks);
        }
    }
}
