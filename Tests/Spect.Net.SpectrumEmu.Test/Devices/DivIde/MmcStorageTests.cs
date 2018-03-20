using System;
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

        [TestMethod]
        public void WriteDataWorksWithSingleByte()
        {
            // --- Arrange
            var mmc = MmcStorage.Create(MMC_FILENAME, 32);

            // --- Act
            mmc.WriteData(0x1000, 123);

            // --- Assert
            TestFileStructure(MMC_FILENAME, 512, 1, 0x14000,
                new Dictionary<ushort, ushort> {{0, 0}}, true,
                new Dictionary<int, byte> {{0x1000, 123}});
        }

        [TestMethod]
        public void WriteDataWorksWithNormalSequence()
        {
            // --- Arrange
            var mmc = MmcStorage.Create(MMC_FILENAME, 32);

            // --- Act
            mmc.WriteData(0x1000, 123);
            mmc.WriteData(0x11000, 124);

            // --- Assert
            TestFileStructure(MMC_FILENAME, 512, 2, 0x24000,
                new Dictionary<ushort, ushort>
                {
                    { 0, 0 },
                    { 1, 1 }
                }, true,
                new Dictionary<int, byte>
                {
                    { 0x1000, 123 },
                    { 0x11000, 124 }
                });
        }

        [TestMethod]
        public void WriteDataWorksWithReverseSequence()
        {
            // --- Arrange
            var mmc = MmcStorage.Create(MMC_FILENAME, 32);

            // --- Act
            mmc.WriteData(0x11000, 124);
            mmc.WriteData(0x1000, 123);

            // --- Assert
            TestFileStructure(MMC_FILENAME, 512, 2, 0x24000,
                new Dictionary<ushort, ushort>
                {
                    { 1, 0 },
                    { 0, 1 }
                }, true,
                new Dictionary<int, byte>
                {
                    { 0x1000, 124 },
                    { 0x11000, 123 }
                });
        }

        [TestMethod]
        public void WriteDataWorksWithSingleBlockBytes()
        {
            // --- Arrange
            var mmc = MmcStorage.Create(MMC_FILENAME, 32);

            // --- Act
            mmc.WriteData(0x1000, 123);
            mmc.WriteData(0x2000, 124);
            mmc.WriteData(0x1800, 125);

            // --- Assert
            TestFileStructure(MMC_FILENAME, 512, 1, 0x14000,
                new Dictionary<ushort, ushort>
                {
                    { 0, 0 }
                }, true,
                new Dictionary<int, byte>
                {
                    { 0x1000, 123 },
                    { 0x2000, 124 },
                    { 0x1800, 125 },
                });
        }

        [TestMethod]
        public void WriteDataWorksWithSingleBlockBytesOnHighSector()
        {
            // --- Arrange
            var mmc = MmcStorage.Create(MMC_FILENAME, 32);

            // --- Act
            mmc.WriteData(0xC1000, 123);
            mmc.WriteData(0xC2000, 124);
            mmc.WriteData(0xC1800, 125);

            // --- Assert
            TestFileStructure(MMC_FILENAME, 512, 1, 0x14000,
                new Dictionary<ushort, ushort>
                {
                    { 0x0C, 0 }
                }, true,
                new Dictionary<int, byte>
                {
                    { 0x1000, 123 },
                    { 0x2000, 124 },
                    { 0x1800, 125 },
                });
        }

        [TestMethod]
        public void WriteDataWorksWithRandomSequence()
        {
            // --- Arrange
            var mmc = MmcStorage.Create(MMC_FILENAME, 32);

            // --- Act
            mmc.WriteData(0x11000, 124);
            mmc.WriteData(0x1000, 123);
            mmc.WriteData(0x42000, 125);

            // --- Assert
            TestFileStructure(MMC_FILENAME, 512, 3, 0x34000,
                new Dictionary<ushort, ushort>
                {
                    { 1, 0 },
                    { 0, 1 },
                    { 4, 2 }
                }, true,
                new Dictionary<int, byte>
                {
                    { 0x1000, 124 },
                    { 0x11000, 123 },
                    { 0x22000, 125 }
                });
        }

        [TestMethod]
        public void WriteDataWorksWithSingleBlockByteArray()
        {
            // --- Arrange
            var sequence = new byte[] {0x80, 0x90, 0xA0};
            var mmc = MmcStorage.Create(MMC_FILENAME, 32);

            // --- Act
            mmc.WriteData(0x1000, sequence);

            // --- Assert
            TestFileStructure(MMC_FILENAME, 512, 1, 0x14000,
                new Dictionary<ushort, ushort> { { 0, 0 } }, true,
                new Dictionary<int, byte>
                {
                    { 0x1000, sequence[0] },
                    { 0x1001, sequence[1] },
                    { 0x1002, sequence[2] }
                });
        }

        [TestMethod]
        public void WriteDataWorksWithMultipleBlockByteArray()
        {
            // --- Arrange
            var sequence = new byte[] { 0x80, 0x90, 0xA0, 0xB0 };
            var mmc = MmcStorage.Create(MMC_FILENAME, 32);

            // --- Act
            mmc.WriteData(0xFFFF, sequence);

            // --- Assert
            TestFileStructure(MMC_FILENAME, 512, 2, 0x24000,
                new Dictionary<ushort, ushort>
                {
                    { 0, 0 },
                    { 1, 1 }
                }, true,
                new Dictionary<int, byte>
                {
                    { 0xFFFF, sequence[0] },
                    { 0x10000, sequence[1] },
                    { 0x10001, sequence[2] },
                    { 0x10002, sequence[3] },
                });
        }

        [TestMethod]
        public void WriteDataWorksWithMultipleBlockByteArrayInHighSectors()
        {
            // --- Arrange
            var sequence = new byte[] { 0x80, 0x90, 0xA0, 0xB0 };
            var mmc = MmcStorage.Create(MMC_FILENAME, 32);

            // --- Act
            mmc.WriteData(0x23FFFF, sequence);

            // --- Assert
            TestFileStructure(MMC_FILENAME, 512, 2, 0x24000,
                new Dictionary<ushort, ushort>
                {
                    { 0x23, 0 },
                    { 0x24, 1 }
                }, true,
                new Dictionary<int, byte>
                {
                    { 0xFFFF, sequence[0] },
                    { 0x10000, sequence[1] },
                    { 0x10001, sequence[2] },
                    { 0x10002, sequence[3] },
                });
        }

        [TestMethod]
        [DataRow(32)]
        [DataRow(64)]
        [DataRow(128)]
        [DataRow(256)]
        public void WriteOverMmcRaisesException(int size)
        {
            // --- Arrange
            var mmc = MmcStorage.Create(MMC_FILENAME, size);

            // --- Act
            mmc.WriteData(size * 1024 * 1024 - 1, 124);
            try
            {
                mmc.WriteData(size * 1024 * 1024, 124);
            }
            catch (InvalidOperationException)
            {
                return;
            }
            Assert.Fail("InvalidOpearationException expected.");
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
                    br.ReadInt32().ShouldBe(mBlocks);
                    br.ReadInt32().ShouldBe(cBlocks);
                    br.ReadInt32().ShouldBe(MmcStorage.MAP_SIZE);

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
                                allocMap[i * 2].ShouldBe((byte)0xFF);
                                allocMap[i * 2 + 1].ShouldBe((byte)0xFF);
                            }
                        }
                    }

                    // --- Check data bytes
                    var data = br.ReadBytes(fileLength - 0x4000);
                    if (dataBytes != null)
                    {
                        for (var i = 0; i < data.Length; i++)
                        {
                            if (dataBytes.ContainsKey(i))
                            {
                                dataBytes[i].ShouldBe(data[i]);
                            }
                            else if (checkEmptyDataBytes)
                            {
                                data[i].ShouldBe((byte)0x00);
                            }
                        }
                    }
                }
            }
        }
    }
}
