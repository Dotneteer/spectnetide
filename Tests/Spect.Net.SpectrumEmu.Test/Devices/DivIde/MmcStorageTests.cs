using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Devices.DivIde;

namespace Spect.Net.SpectrumEmu.Test.Devices.DivIde
{
    [TestClass]
    public class MmcStorageTests
    {
        public const string MMC_FILENAME = @"C:\Temp\MmcFiles\mmcfile.mmc";

        [TestInitialize]
        public void InitializeTest()
        {
            if (File.Exists(MMC_FILENAME))
            {
                File.Delete(MMC_FILENAME);
            }
        }

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
            mmc.CBlocks.ShouldBe((ushort)0);
            mmc.MapSize.ShouldBe((ushort)MmcStorage.MAP_SIZE);
            mmc.CachedBlock.ShouldBe(-1);
        }

        [TestMethod]
        public void CreateInitializesStorageFile()
        {
            // --- Precondition
            var before = File.Exists(MMC_FILENAME);

            // --- Act
            MmcStorage.Create(MMC_FILENAME, 32);

            // --- Assert
            var after = File.Exists(MMC_FILENAME);
            before.ShouldBeFalse();
            after.ShouldBeTrue();
            TestFileStructure(MMC_FILENAME, 512, 0, 0x4000);
        }

        private void TestFileStructure(string filename, ushort mBlocks, ushort cBlocks,
            int fileLength, 
            Dictionary<ushort,ushort> mapEntries = null, bool checkEmptyMapEntries = true,
            Dictionary<int, byte> dataBytes = null, bool checkEmptyDataBytes = true)
        {
            using (var file = File.OpenRead(filename))
            {
                file.Length.ShouldBe(fileLength);
                using (var br = new BinaryReader(File.OpenRead(filename)))
                {
                    // --- Check file header
                    br.ReadByte().ShouldBe((byte)'M');
                    br.ReadByte().ShouldBe((byte)'M');
                    br.ReadByte().ShouldBe((byte)'C');
                    br.ReadByte().ShouldBe((byte)'_');
                    br.ReadUInt16().ShouldBe(mBlocks);
                    br.ReadUInt16().ShouldBe(cBlocks);
                    br.ReadUInt16().ShouldBe((ushort)MmcStorage.MAP_SIZE);
                    br.ReadUInt16().ShouldBe((ushort)0);
                    br.ReadInt32().ShouldBe(0);

                    // --- Check allocation map
                    var allocMap = br.ReadBytes(16368);
                    if (mapEntries != null)
                    {
                        for (ushort i = 0; i < allocMap.Length / 2; i++)
                        {
                            var blockEntry = allocMap[i * 2] + (allocMap[i * 2 + 1] << 8);
                            if (mapEntries.ContainsKey(i))
                            {
                                mapEntries[i].ShouldBe((ushort) blockEntry);
                            }
                            else if (checkEmptyMapEntries)
                            {
                                mapEntries[i].ShouldBe((ushort) 0xFFFF);
                            }
                        }
                    }

                    // --- Check data bytes
                    var data = br.ReadBytes(fileLength - 0x4000);
                    if (dataBytes != null)
                    {

                    }
                }
            }
        }
    }
}
