using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Devices.Next;

namespace Spect.Net.SpectrumEmu.Test.Devices.Next.Palettes
{
    [TestClass]
    public class PaletteSettingTests
    {
        [TestMethod]
        public void PaletteRegistersAreInitializedProperly()
        {
            // --- Act
            var nfs = new NextFeatureControlSet();

            // --- Assert
            nfs.PaletteIndexRegister.LastValue.ShouldBe((byte)0);
            nfs.PaletteValueRegister.LastValue.ShouldBe((byte)0);
            nfs.UlaNextControlRegister.LastValue.ShouldBe((byte)0);
            nfs.UlaNextPaletteExtensionRegister.LastValue.ShouldBe((byte)0);
            nfs.ActivePalette.ShouldBeSameAs(nfs.UlaNextFirstPalette);
        }

        [TestMethod]
        public void UlaNextControlRegisterSetsActivePaletteForUlaFirst1()
        {
            // --- Arrange
            var nfs = new NextFeatureControlSet();

            // --- Act
            nfs.UlaNextControlRegister.Write(0x00);

            // --- Assert
            nfs.ActivePalette.ShouldBeSameAs(nfs.UlaNextFirstPalette);
        }

        [TestMethod]
        public void UlaNextControlRegisterSetsActivePaletteForUlaFirst2()
        {
            // --- Arrange
            var nfs = new NextFeatureControlSet();

            // --- Act
            nfs.UlaNextControlRegister.Write(0x30);

            // --- Assert
            nfs.ActivePalette.ShouldBeSameAs(nfs.UlaNextFirstPalette);
        }

        [TestMethod]
        public void UlaNextControlRegisterSetsActivePaletteForUlaFirst3()
        {
            // --- Arrange
            var nfs = new NextFeatureControlSet();

            // --- Act
            nfs.UlaNextControlRegister.Write(0x70);

            // --- Assert
            nfs.ActivePalette.ShouldBeSameAs(nfs.UlaNextFirstPalette);
        }

        [TestMethod]
        public void UlaNextControlRegisterSetsActivePaletteForLayer2First()
        {
            // --- Arrange
            var nfs = new NextFeatureControlSet();

            // --- Act
            nfs.UlaNextControlRegister.Write(0x10);

            // --- Assert
            nfs.ActivePalette.ShouldBeSameAs(nfs.Layer2FirstPalette);
        }

        [TestMethod]
        public void UlaNextControlRegisterSetsActivePaletteForSpritesFirst()
        {
            // --- Arrange
            var nfs = new NextFeatureControlSet();

            // --- Act
            nfs.UlaNextControlRegister.Write(0x20);

            // --- Assert
            nfs.ActivePalette.ShouldBeSameAs(nfs.SpritesFirstPalette);
        }

        [TestMethod]
        public void UlaNextControlRegisterSetsActivePaletteForUlaNextSecond()
        {
            // --- Arrange
            var nfs = new NextFeatureControlSet();

            // --- Act
            nfs.UlaNextControlRegister.Write(0x40);

            // --- Assert
            nfs.ActivePalette.ShouldBeSameAs(nfs.UlaNextSecondPalette);
        }

        [TestMethod]
        public void UlaNextControlRegisterSetsActivePaletteForLayer2Second()
        {
            // --- Arrange
            var nfs = new NextFeatureControlSet();

            // --- Act
            nfs.UlaNextControlRegister.Write(0x50);

            // --- Assert
            nfs.ActivePalette.ShouldBeSameAs(nfs.Layer2SecondPalette);
        }

        [TestMethod]
        public void UlaNextControlRegisterSetsActivePaletteForSpritesSecond()
        {
            // --- Arrange
            var nfs = new NextFeatureControlSet();

            // --- Act
            nfs.UlaNextControlRegister.Write(0x60);

            // --- Assert
            nfs.ActivePalette.ShouldBeSameAs(nfs.SpritesSecondPalette);
        }

        [TestMethod]
        [DataRow(0, 0b111_111_00, 1, 0b111_111_000)]
        [DataRow(0, 0b111_111_10, 1, 0b111_111_101)]
        [DataRow(0, 0b000_000_01, 1, 0b000_000_011)]
        [DataRow(0, 0b111_000_11, 1, 0b111_000_111)]
        [DataRow(2, 0b000_000_01, 3, 0b000_000_011)]
        [DataRow(0x80, 0b000_000_01, 0x81, 0b000_000_011)]
        [DataRow(0xFE, 0b000_000_01, 0xFF, 0b000_000_011)]
        [DataRow(0xFF, 0b000_000_01, 0x00, 0b000_000_011)]
        public void SinglePaletteValueSettingWith8BitWorks(int index, int rgb, int expIndex, int expRgb)
        {
            // --- Arrange
            var nfs = new NextFeatureControlSet();
            nfs.UlaNextControlRegister.Write(0x00);

            // --- Act
            nfs.PaletteIndexRegister.Write((byte)index);
            nfs.PaletteValueRegister.Write((byte)rgb);

            // --- Assert
            nfs.ActivePalette[(byte)index].ShouldBe(expRgb);
            nfs.PaletteIndexRegister.LastValue.ShouldBe((byte)expIndex);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(11)]
        [DataRow(127)]
        [DataRow(128)]
        [DataRow(254)]
        [DataRow(255)]
        public void FullPaletteValueSettingWith8BitWorks(int shift)
        {
            // --- Arrange
            var nfs = new NextFeatureControlSet();
            nfs.UlaNextControlRegister.Write(0x00);

            // --- Act
            nfs.PaletteIndexRegister.Write((byte)shift);
            for (var i = 0x00; i <= 0xFF; i++)
            {
                nfs.PaletteValueRegister.Write((byte)i);
            }

            // --- Assert
            for (var i = 0x00; i <= 0xFF; i++)
            {
                var expColor = (byte)(i-shift) * 2 | (((byte)(i - shift) & 0x03) == 0 ? 0 : 1);
                nfs.ActivePalette[(byte)i].ShouldBe(expColor);
            }
        }

    }
}
