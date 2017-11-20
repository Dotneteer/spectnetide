using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Devices.Memory;

namespace Spect.Net.SpectrumEmu.Test.Devices.Memory
{
    [TestClass]
    public class Spectrum128MemoryDeviceTests
    {
        [TestMethod]
        public void ConstructionWorksAsExpected()
        {
            // --- Act
            var dev = new Spectrum128MemoryDevice();

            // -- Assert
            dev.GetSelectedRomIndex().ShouldBe(0);
            dev.GetSelectedBankIndex(3).ShouldBe(0);
        }

        [TestMethod]
        public void ResetWorksAsExpected()
        {
            // --- Arrange
            var dev = new Spectrum128MemoryDevice();
            dev.OnAttachedToVm(null);

            // --- Act
            dev.Reset();

            // -- Assert
            dev.GetSelectedRomIndex().ShouldBe(0);
            dev.GetSelectedBankIndex(3).ShouldBe(0);

            for (var i = 0; i <= 0xFFFF; i++)
            {
                dev.Read((ushort)i).ShouldBe((byte)0xFF);
            }

            for (var b = 0; b < 8; b++)
            {
                dev.PageIn(3, b);
                for (var i = 0xC000; i <= 0xFFFF; i++)
                {
                    dev.Read((ushort)i).ShouldBe((byte)0xFF);
                }
            }
        }

        [TestMethod]
        public void ReadWorksAsExpected()
        {
            // --- Arrange
            var dev = new Spectrum128MemoryDevice();
            dev.OnAttachedToVm(null);
            for (var i = 0; i < 0x4000; i++)
            {
                dev.CurrentRom[i] = 0xFF;
                for (var b = 0; b < 8; b++)
                {
                    dev.RamBanks[b][i] = (byte)b;
                }
            }

            // -- Act/Assert
            for (var i = 0; i <= 0x3FFF; i++)
            {
                dev.Read((ushort)i).ShouldBe((byte)0xFF);
            }
            for (var i = 0x4000; i <= 0x7FFF; i++)
            {
                dev.Read((ushort)i).ShouldBe((byte)0x05);
            }
            for (var i = 0x8000; i <= 0xBFFF; i++)
            {
                dev.Read((ushort)i).ShouldBe((byte)0x02);
            }
            for (var i = 0xC000; i <= 0xFFFF; i++)
            {
                dev.Read((ushort)i).ShouldBe((byte)0x00);
            }
        }

        [TestMethod]
        public void WriteWorksAsExpected()
        {
            // --- Arrange
            var dev = new Spectrum128MemoryDevice();
            dev.OnAttachedToVm(null);
            dev.Reset();

            for (var i = 0; i < 0x4000; i++)
            {
                dev.CurrentRom[i] = 0xFC;
            }

            for (var i = 0; i <= 0xFFFF; i++)
            {
                dev.Write((ushort)i, (byte)(i & 0xFF));
            }

            // -- Assert
            for (var i = 0; i <= 0x3FFF; i++)
            {
                dev.Read((ushort)i).ShouldBe((byte)0xFC);
            }
            for (var i = 0x4000; i <= 0x7FFF; i++)
            {
                dev.Read((ushort)i).ShouldBe((byte)(i & 0xFF));
            }
            for (var i = 0x8000; i <= 0xBFFF; i++)
            {
                dev.Read((ushort)i).ShouldBe((byte)(i & 0xFF));
            }
            for (var i = 0xC000; i <= 0xFFFF; i++)
            {
                dev.Read((ushort)i).ShouldBe((byte)(i & 0xFF));
            }
            for (var b = 0; b < 8; b++)
            {
                for (var i = 0; i <= 0x3FFF; i++)
                {
                    if (b == 0 || b == 2 || b == 5)
                    {
                        dev.RamBanks[b][i].ShouldBe((byte)(i & 0xFF));
                    }
                    else
                    {
                        dev.RamBanks[b][i].ShouldBe((byte)0xFF);
                    }
                }
            }
        }

        [TestMethod]
        public void CloneWorksAsExpectedWithDefaultBanks()
        {
            // --- Arrange
            var dev = new Spectrum128MemoryDevice();
            dev.OnAttachedToVm(null);
            dev.Reset();

            for (var i = 0; i < 0x4000; i++)
            {
                dev.CurrentRom[i] = 0xFC;
                for (var b = 0; b < 8; b++)
                {
                    dev.RamBanks[b][i] = (byte)b;
                }
            }

            // --- Act
            var cloned = dev.CloneMemory();

            // -- Assert
            for (var i = 0; i <= 0x3FFF; i++)
            {
                cloned[i].ShouldBe((byte)0xFC);
            }
            for (var i = 0x4000; i <= 0x7FFF; i++)
            {
                cloned[i].ShouldBe((byte)0x05);
            }
            for (var i = 0x8000; i <= 0xBFFF; i++)
            {
                cloned[i].ShouldBe((byte)0x02);
            }
            for (var i = 0xC000; i <= 0xFFFF; i++)
            {
                cloned[i].ShouldBe((byte)0x00);
            }
        }

        [TestMethod]
        public void CloneWorksAsExpectedWithChangedBanks()
        {
            // --- Arrange
            var dev = new Spectrum128MemoryDevice();
            dev.OnAttachedToVm(null);
            dev.Reset();

            for (var i = 0; i < 0x4000; i++)
            {
                dev.CurrentRom[i] = 0xFC;
                for (var b = 0; b < 8; b++)
                {
                    dev.RamBanks[b][i] = (byte)b;
                }
            }
            dev.PageIn(3, 4);

            // --- Act
            var cloned = dev.CloneMemory();

            // -- Assert
            for (var i = 0; i <= 0x3FFF; i++)
            {
                cloned[i].ShouldBe((byte)0xFC);
            }
            for (var i = 0x4000; i <= 0x7FFF; i++)
            {
                cloned[i].ShouldBe((byte)0x05);
            }
            for (var i = 0x8000; i <= 0xBFFF; i++)
            {
                cloned[i].ShouldBe((byte)0x02);
            }
            for (var i = 0xC000; i <= 0xFFFF; i++)
            {
                cloned[i].ShouldBe((byte)0x04);
            }
        }

    }
}
