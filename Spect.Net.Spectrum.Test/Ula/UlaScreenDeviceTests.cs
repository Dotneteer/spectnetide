using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Spectrum.Machine;
using Spect.Net.Spectrum.Test.Helpers;

namespace Spect.Net.Spectrum.Test.Ula
{
    [TestClass]
    public class UlaScreenDeviceTests
    {
        [TestMethod]
        public void SettingBorderValueDoesNotChangeInvisibleScreenArea()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();

            // --- Because we execute only 443 CPU tacts, rendering does not
            // --- reach the visible border area
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x10, 0x00, // LD BC,$0010
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });
            var pixels = new TestPixelRenderer(spectrum.DisplayPars);
            spectrum.ScreenDevice.SetScreenPixelRenderer(pixels);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.UntilHalt);
            
            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800D);
            spectrum.Cpu.Ticks.ShouldBe(443ul);

            for (var row = 0; row < spectrum.DisplayPars.ScreenLines; row++)
            {
                for (var column = 0; column < spectrum.DisplayPars.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0xFF);
                }
            }
        }

        [TestMethod]
        public void SettingBorderValueChangesBorderArea1()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();

            // --- Because we execute 3667 CPU tacts, rendering reaches
            // --- the first 88 pixels of the first border row
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x8C, 0x00, // LD BC,$008C
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });
            var pixels = new TestPixelRenderer(spectrum.DisplayPars);
            spectrum.ScreenDevice.SetScreenPixelRenderer(pixels);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.UntilHalt);

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800D);
            spectrum.Cpu.Ticks.ShouldBe(3667ul);

            // --- The left 88 pixels of the first border row should be set to 0x05
            for (var column = 0; column < 88; column++)
            {
                pixels[0, column].ShouldBe((byte)0x05);
            }

            // --- The remaining pixels of the first border row should be intact (0xFF)
            for (var column = 88; column < spectrum.DisplayPars.ScreenWidth; column++)
            {
                pixels[0, column].ShouldBe((byte)0xFF);
            }

            // --- All the other screen bytes should be intact (0xFF)
            for (var row = 1; row < spectrum.DisplayPars.ScreenLines; row++)
            {
                for (var column = 0; column < spectrum.DisplayPars.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0xFF);
                }
            }
        }

        [TestMethod]
        public void SettingBorderValueChangesBorderArea2()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();

            // --- Because we execute 14327 CPU tacts, rendering reaches
            // --- all top border rows, save the last invisible pixels of the last top border row
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x26, 0x02, // LD BC,$0226
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });
            var pixels = new TestPixelRenderer(spectrum.DisplayPars);
            spectrum.ScreenDevice.SetScreenPixelRenderer(pixels);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.UntilHalt);

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800D);
            spectrum.Cpu.Ticks.ShouldBe(14327ul);

            // --- The top 48 border rows should be set to 0x05
            for (var row = 0; row < 48 ; row++)
            {
                for (var column = 0; column < spectrum.DisplayPars.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- All the other screen bytes should be intact (0xFF)
            for (var row = 48; row < spectrum.DisplayPars.ScreenLines; row++)
            {
                for (var column = 0; column < spectrum.DisplayPars.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0xFF);
                }
            }
        }

        [TestMethod]
        public void SettingBorderValueChangesBorderArea3()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();

            // --- Because we execute 14405 CPU tacts, rendering reaches
            // --- all top border rows + the left border are of the first
            // --- display row, and sets the 
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x29, 0x02, // LD BC,$0229
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });
            var pixels = new TestPixelRenderer(spectrum.DisplayPars);
            spectrum.ScreenDevice.SetScreenPixelRenderer(pixels);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.UntilHalt);

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800D);
            spectrum.Cpu.Ticks.ShouldBe(14405ul);

            // --- The top 48 border rows should be set to 0x05
            for (var row = 0; row < 48; row++)
            {
                for (var column = 0; column < spectrum.DisplayPars.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- The left border of row 48 should be set to 0x05
            for (var column = 0; column < 48; column++)
            {
                pixels[48, column].ShouldBe((byte)0x05);
            }

            // --- The first 12 pixels of the first display row (48) should be set to 0
            for (var column = 48; column < 60; column++)
            {
                pixels[48, column].ShouldBe((byte)0x00);
            }

            // --- The other pixels of the first display row (48) should be intact
            for (var column = 60; column < spectrum.DisplayPars.ScreenWidth; column++)
            {
                pixels[48, column].ShouldBe((byte)0xFF);
            }

            // --- All the other screen bytes should be intact (0xFF)
            for (var row = 49; row < spectrum.DisplayPars.ScreenLines; row++)
            {
                for (var column = 0; column < spectrum.DisplayPars.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0xFF);
                }
            }
        }

        [TestMethod]
        public void RenderingScreenWithEmptyPixelsWorks()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();

            // --- Because we execute 14405 CPU tacts, rendering reaches
            // --- all top border rows + the left border are of the first
            // --- display row, and sets the 
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x75, 0x0A, // LD BC,$0A75
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });
            var pixels = new TestPixelRenderer(spectrum.DisplayPars);
            spectrum.ScreenDevice.SetScreenPixelRenderer(pixels);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.UntilHalt);

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800D);
            spectrum.Cpu.Ticks.ShouldBe(69629ul);

            // --- The top 48 border rows should be set to 0x05
            for (var row = 0; row < 48; row++)
            {
                for (var column = 0; column < spectrum.DisplayPars.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- Display rows should have a border value of 0x05 and a pixel value of 0x00
            for (var row = 48; row < 48 + 192; row++)
            {
                for (var column = 0; column < spectrum.DisplayPars.BorderLeftPixels; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
                for (var column = spectrum.DisplayPars.BorderLeftPixels; 
                    column < spectrum.DisplayPars.ScreenWidth - spectrum.DisplayPars.BorderRightPixels; 
                    column++)
                {
                    pixels[row, column].ShouldBe((byte)0x00);
                }
                for (var column = spectrum.DisplayPars.ScreenWidth - spectrum.DisplayPars.BorderRightPixels;
                    column < spectrum.DisplayPars.ScreenWidth;
                    column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- The bottom 48 border rows should be set to 0x05
            for (var row = 48 + 192; row < spectrum.DisplayPars.ScreenLines; row++)
            {
                for (var column = 0; column < spectrum.DisplayPars.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }
        }
    }
}
