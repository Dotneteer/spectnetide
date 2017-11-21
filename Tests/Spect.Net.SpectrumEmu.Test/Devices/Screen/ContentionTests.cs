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


        /// <summary>
        /// Inits machine with instructions so the a tact can be reached
        /// </summary>
        /// <param name="tactToReach"></param>
        /// <param name="spectrum"></param>
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

            // --- Because we execute only 451 CPU tacts, rendering does not
            // --- reach the visible border area
            spectrum.InitCode(code);
        }

        /// <summary>
        /// Inits machine with instructions so the a tact can be reached
        /// </summary>
        /// <param name="tactToReach"></param>
        /// <param name="spectrum"></param>
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

            // --- Because we execute only 451 CPU tacts, rendering does not
            // --- reach the visible border area
            spectrum.InitCode(code);
        }
    }
}
