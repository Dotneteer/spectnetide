using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Devices.Screen
{
    [TestClass]
    public class ContentionTests
    {
        [TestMethod]
        public void NoWriteContentionValueIsAppliedWhileFirstDisplayLineIsReached()
        {
            // --- Arrange
            const int EXPECTED_CONTENTION = 0;
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime;
            InitMachineWriteWithNops(tactToReach, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(EXPECTED_CONTENTION + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        public void NoWriteContentionValueIsAppliedWhileLeftDisplayEdgeIsReached()
        {
            // --- Arrange
            const int EXPECTED_CONTENTION = 0;
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                + screenConfig.FirstPixelTactInLine;
            InitMachineWriteWithNops(tactToReach, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(EXPECTED_CONTENTION + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        [DataRow(1, 6)]
        [DataRow(2, 5)]
        [DataRow(3, 4)]
        [DataRow(4, 3)]
        [DataRow(5, 2)]
        [DataRow(6, 1)]
        [DataRow(7, 0)]
        [DataRow(8, 0)]
        public void WriteContentionValueIsAppliedWhenLeftDisplayEdgeIsReached(int deviation, int expected)
        {
            // --- Arrange
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine + deviation;
            InitMachineWriteWithNops(tactToReach, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(expected + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        public void NoReadContentionValueIsAppliedWhileFirstDisplayLineIsReached()
        {
            // --- Arrange
            const int EXPECTED_CONTENTION = 0;
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime;
            InitMachineReadWithNops(tactToReach, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(EXPECTED_CONTENTION + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        public void NoReadContentionValueIsAppliedWhileLeftDisplayEdgeIsReached()
        {
            // --- Arrange
            const int EXPECTED_CONTENTION = 0;
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine;
            InitMachineReadWithNops(tactToReach, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(EXPECTED_CONTENTION + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        [DataRow(1, 6)]
        [DataRow(2, 5)]
        [DataRow(3, 4)]
        [DataRow(4, 3)]
        [DataRow(5, 2)]
        [DataRow(6, 1)]
        [DataRow(7, 0)]
        [DataRow(8, 0)]
        public void ReadContentionValueIsAppliedWhenLeftDisplayEdgeIsReached(int deviation, int expected)
        {
            // --- Arrange
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine + deviation;
            InitMachineReadWithNops(tactToReach, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(expected + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        public void NoOutAContentionValueOnPort0XfeIsAppliedWhileFirstDisplayLineIsReached()
        {
            // --- Arrange
            const int EXPECTED_CONTENTION = 0;
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime;
            InitMachineOutAWithNops(tactToReach, 0xFE, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(EXPECTED_CONTENTION + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        public void NoOutAContentionValueOnPort0XfeIsAppliedWhileLeftDisplayEdgeIsReached()
        {
            // --- Arrange
            const int EXPECTED_CONTENTION = 0;
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine;
            InitMachineOutAWithNops(tactToReach, 0xFE, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(EXPECTED_CONTENTION + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        [DataRow(1, 6)]
        [DataRow(2, 5)]
        [DataRow(3, 4)]
        [DataRow(4, 3)]
        [DataRow(5, 2)]
        [DataRow(6, 1)]
        [DataRow(7, 0)]
        [DataRow(8, 0)]
        public void OutAContentionValueOnPort0XfeIsAppliedWhenLeftDisplayEdgeIsReached(int deviation, int expected)
        {
            // --- Arrange
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine + deviation;
            InitMachineOutAWithNops(tactToReach, 0xfe, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(expected + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        [DataRow(1, 0)]
        [DataRow(2, 6)]
        [DataRow(3, 5)]
        [DataRow(4, 4)]
        [DataRow(5, 3)]
        [DataRow(6, 2)]
        [DataRow(7, 1)]
        [DataRow(8, 0)]
        public void OutAContentionValueOnPort0X40IsAppliedWhenLeftDisplayEdgeIsReached(int deviation, int expected)
        {
            // --- Arrange
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine + deviation;
            InitMachineOutAWithNops(tactToReach, 0x40, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(expected + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        [DataRow(1, 0)]
        [DataRow(2, 6)]
        [DataRow(3, 5)]
        [DataRow(4, 4)]
        [DataRow(5, 3)]
        [DataRow(6, 2)]
        [DataRow(7, 1)]
        [DataRow(8, 0)]
        public void OutAContentionValueOnPort0X41IsAppliedWhenLeftDisplayEdgeIsReached(int deviation, int expected)
        {
            // --- Arrange
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine + deviation;
            InitMachineOutAWithNops(tactToReach, 0x41, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(expected + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        [DataRow(1, 0)]
        [DataRow(2, 0)]
        [DataRow(3, 0)]
        [DataRow(4, 0)]
        [DataRow(5, 0)]
        [DataRow(6, 0)]
        [DataRow(7, 0)]
        [DataRow(8, 0)]
        public void NoOutAContentionValueOnPort0XffIsAppliedWhenLeftDisplayEdgeIsReached(int deviation, int expected)
        {
            // --- Arrange
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine + deviation;
            InitMachineOutAWithNops(tactToReach, 0xff, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(expected + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        public void NoInAContentionValueOnPort0XfeIsAppliedWhileFirstDisplayLineIsReached()
        {
            // --- Arrange
            const int EXPECTED_CONTENTION = 0;
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime;
            InitMachineInAWithNops(tactToReach, 0xFE, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(EXPECTED_CONTENTION + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        public void NoInAContentionValueOnPort0XfeIsAppliedWhileLeftDisplayEdgeIsReached()
        {
            // --- Arrange
            const int EXPECTED_CONTENTION = 0;
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine;
            InitMachineInAWithNops(tactToReach, 0xFE, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(EXPECTED_CONTENTION + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        [DataRow(1, 6)]
        [DataRow(2, 5)]
        [DataRow(3, 4)]
        [DataRow(4, 3)]
        [DataRow(5, 2)]
        [DataRow(6, 1)]
        [DataRow(7, 0)]
        [DataRow(8, 0)]
        public void InAContentionValueOnPort0XfeIsAppliedWhenLeftDisplayEdgeIsReached(int deviation, int expected)
        {
            // --- Arrange
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine + deviation;
            InitMachineInAWithNops(tactToReach, 0xfe, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(expected + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        [DataRow(1, 0)]
        [DataRow(2, 6)]
        [DataRow(3, 5)]
        [DataRow(4, 4)]
        [DataRow(5, 3)]
        [DataRow(6, 2)]
        [DataRow(7, 1)]
        [DataRow(8, 0)]
        public void InAContentionValueOnPort0X40IsAppliedWhenLeftDisplayEdgeIsReached(int deviation, int expected)
        {
            // --- Arrange
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine + deviation;
            InitMachineInAWithNops(tactToReach, 0x40, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(expected + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        [DataRow(1, 0)]
        [DataRow(2, 6)]
        [DataRow(3, 5)]
        [DataRow(4, 4)]
        [DataRow(5, 3)]
        [DataRow(6, 2)]
        [DataRow(7, 1)]
        [DataRow(8, 0)]
        public void InAContentionValueOnPort0X41IsAppliedWhenLeftDisplayEdgeIsReached(int deviation, int expected)
        {
            // --- Arrange
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine + deviation;
            InitMachineInAWithNops(tactToReach, 0x41, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(expected + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        [DataRow(1, 0)]
        [DataRow(2, 0)]
        [DataRow(3, 0)]
        [DataRow(4, 0)]
        [DataRow(5, 0)]
        [DataRow(6, 0)]
        [DataRow(7, 0)]
        [DataRow(8, 0)]
        public void NoInAContentionValueOnPort0XffIsAppliedWhenLeftDisplayEdgeIsReached(int deviation, int expected)
        {
            // --- Arrange
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine + deviation;
            InitMachineInAWithNops(tactToReach, 0xff, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(expected + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        public void NoOutCContentionValueOnPort0XfeIsAppliedWhileFirstDisplayLineIsReached()
        {
            // --- Arrange
            const int EXPECTED_CONTENTION = 0;
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime;
            InitMachineOutCWithNops(tactToReach, 0xFE, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(EXPECTED_CONTENTION + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        public void NoOutCContentionValueOnPort0XfeIsAppliedWhileLeftDisplayEdgeIsReached()
        {
            // --- Arrange
            const int EXPECTED_CONTENTION = 0;
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine - 4;
            InitMachineOutCWithNops(tactToReach, 0xFE, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(EXPECTED_CONTENTION + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        [DataRow(1, 3)]
        [DataRow(2, 2)]
        [DataRow(3, 1)]
        [DataRow(4, 0)]
        [DataRow(5, 0)]
        [DataRow(6, 6)]
        [DataRow(7, 5)]
        [DataRow(8, 4)]
        public void OutCContentionValueOnPort0XfeIsAppliedWhenLeftDisplayEdgeIsReached(int deviation, int expected)
        {
            // --- Arrange
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine + deviation;
            InitMachineOutCWithNops(tactToReach, 0xfe, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(expected + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        [DataRow(1, 4)]
        [DataRow(2, 3)]
        [DataRow(3, 2)]
        [DataRow(4, 1)]
        [DataRow(5, 0)]
        [DataRow(6, 0)]
        [DataRow(7, 6)]
        [DataRow(8, 5)]
        public void OutCContentionValueOnPort0X40IsAppliedWhenLeftDisplayEdgeIsReached(int deviation, int expected)
        {
            // --- Arrange
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine + deviation;
            InitMachineOutCWithNops(tactToReach, 0x40, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(expected + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        [DataRow(1, 4)]
        [DataRow(2, 3)]
        [DataRow(3, 2)]
        [DataRow(4, 1)]
        [DataRow(5, 0)]
        [DataRow(6, 0)]
        [DataRow(7, 6)]
        [DataRow(8, 5)]
        public void OutCContentionValueOnPort0X41IsAppliedWhenLeftDisplayEdgeIsReached(int deviation, int expected)
        {
            // --- Arrange
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine + deviation;
            InitMachineOutCWithNops(tactToReach, 0x41, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(expected + tactToReach + 4); // +4 for HALT
        }

        [TestMethod]
        [DataRow(1, 0)]
        [DataRow(2, 0)]
        [DataRow(3, 0)]
        [DataRow(4, 0)]
        [DataRow(5, 0)]
        [DataRow(6, 0)]
        [DataRow(7, 0)]
        [DataRow(8, 0)]
        public void NoOutCContentionValueOnPort0XffIsAppliedWhenLeftDisplayEdgeIsReached(int deviation, int expected)
        {
            // --- Arrange
            var screenConfig = new ScreenConfiguration(SpectrumModels.ZxSpectrum48Pal.Screen);
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.Screen);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            var tactToReach = screenConfig.FirstDisplayLine * screenConfig.ScreenLineTime
                              + screenConfig.FirstPixelTactInLine - 12 + deviation;
            InitMachineOutCWithNops(tactToReach, 0xff, spectrum);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            spectrum.Cpu.Tacts.ShouldBe(expected + tactToReach + 4); // +4 for HALT
        }




        private static void InitMachineWriteWithNops(int tactToReach, SpectrumAdvancedTestMachine spectrum)
        {
            var code = new List<byte>
            {
                0xF3 // DI ; (4) 
            };
            var usedTacts = 4;
            var remainder = tactToReach % 4;
            switch (remainder)
            {
                case 0:
                    break;
                case 1:
                    code.Add(0xED); // LD A,I (9)
                    code.Add(0x57);
                    usedTacts += 9;
                    break;
                case 2:
                    code.Add(0x03); // INC BC (6)
                    usedTacts += 6;
                    break;
                default:
                    code.Add(0x3E); // LD A,0 (7)
                    code.Add(0x00);
                    usedTacts += 7;
                    break;
            }

            usedTacts += 20;
            var extraNops = (tactToReach - usedTacts) / 4;
            for (var i = 0; i < extraNops; i++)
            {
                code.Add(0x00); // NOP
            }

            // --- Now we add the instruction for contention
            code.Add(0x3E); // LD A,#FF ; (7)
            code.Add(0xFF);
            code.Add(0x32); // LD (#4100),A ; (13)
            code.Add(0x00);
            code.Add(0x41);
            code.Add(0x76); // HALT

            spectrum.InitCode(code);
        }

        private static void InitMachineReadWithNops(int tactToReach, SpectrumAdvancedTestMachine spectrum)
        {
            var code = new List<byte>
            {
                0xF3 // DI ; (4) 
            };
            var usedTacts = 4;
            var remainder = tactToReach % 4;
            switch (remainder)
            {
                case 0:
                    break;
                case 1:
                    code.Add(0xED); // LD A,I (9)
                    code.Add(0x57);
                    usedTacts += 9;
                    break;
                case 2:
                    code.Add(0x03); // INC BC (6)
                    usedTacts += 6;
                    break;
                default:
                    code.Add(0x3E); // LD A,0 (7)
                    code.Add(0x00);
                    usedTacts += 7;
                    break;
            }

            usedTacts += 20;
            var extraNops = (tactToReach - usedTacts) / 4;
            for (var i = 0; i < extraNops; i++)
            {
                code.Add(0x00); // NOP
            }

            // --- Now we add the instruction for contention
            code.Add(0x3E); // LD A,#FF ; (7)
            code.Add(0xFF);
            code.Add(0x3A); // LD A,(#4100) ; (13)
            code.Add(0x00);
            code.Add(0x41);
            code.Add(0x76); // HALT

            spectrum.InitCode(code);
        }

        private static void InitMachineOutAWithNops(int tactToReach, byte port, SpectrumAdvancedTestMachine spectrum)
        {
            var code = new List<byte>
            {
                0xF3 // DI ; (4) 
            };
            var usedTacts = 4;
            var remainder = tactToReach % 4;
            switch (remainder)
            {
                case 0:
                    code.Add(0x03); // INC BC (6)
                    usedTacts += 6;
                    break;
                case 1:
                    code.Add(0x3E); // LD A,0 (7)
                    code.Add(0x00);
                    usedTacts += 7;
                    break;
                case 2:
                    break;
                case 3:
                    code.Add(0xED); // LD A,I (9)
                    code.Add(0x57);
                    usedTacts += 9;
                    break;
            }

            usedTacts += 18;
            var extraNops = (tactToReach - usedTacts) / 4;
            for (var i = 0; i < extraNops; i++)
            {
                code.Add(0x00); // NOP
            }

            // --- Now we add the instruction for contention
            code.Add(0x3E); // LD A,port ; (7)
            code.Add(port);
            code.Add(0xD3); // OUT (port),A ; (11)
            code.Add(port);
            code.Add(0x76); // HALT

            spectrum.InitCode(code);
        }

        private static void InitMachineInAWithNops(int tactToReach, byte port, SpectrumAdvancedTestMachine spectrum)
        {
            var code = new List<byte>
            {
                0xF3 // DI ; (4) 
            };
            var usedTacts = 4;
            var remainder = tactToReach % 4;
            switch (remainder)
            {
                case 0:
                    code.Add(0x03); // INC BC (6)
                    usedTacts += 6;
                    break;
                case 1:
                    code.Add(0x3E); // LD A,0 (7)
                    code.Add(0x00);
                    usedTacts += 7;
                    break;
                case 2:
                    break;
                case 3:
                    code.Add(0xED); // LD A,I (9)
                    code.Add(0x57);
                    usedTacts += 9;
                    break;
            }

            usedTacts += 18;
            var extraNops = (tactToReach - usedTacts) / 4;
            for (var i = 0; i < extraNops; i++)
            {
                code.Add(0x00); // NOP
            }

            // --- Now we add the instruction for contention
            code.Add(0x3E); // LD A,port ; (7)
            code.Add(port);
            code.Add(0xDB); // IN (port),A ; (11)
            code.Add(port);
            code.Add(0x76); // HALT

            spectrum.InitCode(code);
        }

        private static void InitMachineOutCWithNops(int tactToReach, byte port, SpectrumAdvancedTestMachine spectrum)
        {
            var code = new List<byte>
            {
                0xF3 // DI ; (4) 
            };
            var usedTacts = 4;
            var remainder = tactToReach % 4;
            switch (remainder)
            {
                case 0:
                    code.Add(0x03); // INC BC (6)
                    usedTacts += 6;
                    break;
                case 1:
                    code.Add(0x3E); // LD A,0 (7)
                    code.Add(0x00);
                    usedTacts += 7;
                    break;
                case 2:
                    break;
                case 3:
                    code.Add(0xED); // LD A,I (9)
                    code.Add(0x57);
                    usedTacts += 9;
                    break;
            }

            usedTacts += 26;
            var extraNops = (tactToReach - usedTacts) / 4;
            for (var i = 0; i < extraNops; i++)
            {
                code.Add(0x00); // NOP
            }

            // --- Now we add the instruction for contention
            code.Add(0x06); // LD B,port ; (7)
            code.Add(port);
            code.Add(0x0E); // LD C,port ; (7)
            code.Add(port);
            code.Add(0xED); // OUT (C),A ; (12)
            code.Add(0x79); 
            code.Add(0x76); // HALT

            spectrum.InitCode(code);
        }
    }
}
