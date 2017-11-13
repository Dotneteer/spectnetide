using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Devices.Screen
{
    [TestClass]
    public class ScreenDeviceTests
    {
        [TestMethod]
        public void SettingBorderValueDoesNotChangeInvisibleScreenArea()
        {
            // --- Arrange
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.ScreenConfiguration);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            // --- Because we execute only 451 CPU tacts, rendering does not
            // --- reach the visible border area
            spectrum.InitCode(new byte[]
            {
                0xF3,             // DI
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x10, 0x00, // LD BC,$0010
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0xFB,             // EI
                0x76              // HALT
            });
            ((IScreenDeviceTestSupport)spectrum.ScreenDevice).FillScreenBuffer(0xFF);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800E);
            spectrum.Cpu.Tacts.ShouldBe(451L);
            pixels.IsFrameReady.ShouldBeFalse();

            pixels.SetPixelMemory(spectrum.ScreenDevice.GetPixelBuffer());
            for (var row = 0; row < spectrum.ScreenDevice.ScreenConfiguration.ScreenLines; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0xFF);
                }
            }
        }

        [TestMethod]
        public void SettingBorderValueChangesBorderArea1()
        {
            // --- Arrange
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.ScreenConfiguration);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            // --- Because we execute 3675 CPU tacts, rendering reaches
            // --- the first 104 pixels of the first border row
            spectrum.InitCode(new byte[]
            {
                0xF3,             // DI
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x8C, 0x00, // LD BC,$008C
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800D);
            spectrum.Cpu.Tacts.ShouldBe(3671L);
            pixels.IsFrameReady.ShouldBeFalse();

            // --- The left 104 pixels of the first border row should be set to 0x05
            pixels.SetPixelMemory(spectrum.ScreenDevice.GetPixelBuffer());
            for (var column = 0; column < 96; column++)
            {
                pixels[0, column].ShouldBe((byte)0x05);
            }

            // --- The remaining pixels of the first border row should be intact (0xFF)
            for (var column = 96; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
            {
                pixels[0, column].ShouldBe((byte)0x00);
            }

            // --- All the other screen bytes should be intact (0xFF)
            for (var row = 1; row < spectrum.ScreenDevice.ScreenConfiguration.ScreenLines; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x00);
                }
            }
        }

        [TestMethod]
        public void SettingBorderValueChangesBorderArea2()
        {
            // --- Arrange
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.ScreenConfiguration);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            // --- Because we execute 14335 CPU tacts, rendering reaches
            // --- all top border rows, save the last invisible pixels of the last top border row
            spectrum.InitCode(new byte[]
            {
                0xF3,             // DI
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x26, 0x02, // LD BC,$0226
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800D);
            spectrum.Cpu.Tacts.ShouldBe(14331L);
            pixels.IsFrameReady.ShouldBeFalse();

            // --- The top 48 border rows should be set to 0x05
            pixels.SetPixelMemory(spectrum.ScreenDevice.GetPixelBuffer());
            for (var row = 0; row < 48 ; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- All the other screen bytes should be intact (0xFF)
            for (var row = 48; row < spectrum.ScreenDevice.ScreenConfiguration.ScreenLines; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x00);
                }
            }
        }

        [TestMethod]
        public void SettingBorderValueChangesBorderArea3()
        {
            // --- Arrange
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.ScreenConfiguration);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            // --- Because we execute 14413 CPU tacts, rendering reaches
            // --- all top border rows + the left border of the first
            // --- display row, and sets the first 28 pixels to 0.

            spectrum.InitCode(new byte[]
            {
                0xF3,             // DI
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x29, 0x02, // LD BC,$0229
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0xFB,             // EI
                0x76              // HALT
            });
            ((IScreenDeviceTestSupport)spectrum.ScreenDevice).FillScreenBuffer(0xDC);

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800E);
            spectrum.Cpu.Tacts.ShouldBe(14413L);
            pixels.IsFrameReady.ShouldBeFalse();

            // --- The top 48 border rows should be set to 0x05
            pixels.SetPixelMemory(spectrum.ScreenDevice.GetPixelBuffer());
            for (var row = 0; row < 48; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
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
            for (var column = 48; column < 76; column++)
            {
                pixels[48, column].ShouldBe((byte)0x00);
            }

            // --- The other pixels of the first display row (48) should be intact
            for (var column = 76; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
            {
                pixels[48, column].ShouldBe((byte)0xDC);
            }

            // --- All the other screen bytes should be intact (0xDC)
            for (var row = 49; row < spectrum.ScreenDevice.ScreenConfiguration.ScreenLines; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0xDC);
                }
            }
        }

        [TestMethod]
        public void RenderingScreenWithEmptyPixelsWorks()
        {
            // --- Arrange
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.ScreenConfiguration);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            // --- Because we execute 69637 CPU tacts, rendering set all border
            // --- pixels to 5 + screen pixels to 0 
            spectrum.InitCode(new byte[]
            {
                0xF3,             // DI
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x75, 0x0A, // LD BC,$0A75
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800D);
            spectrum.Cpu.Tacts.ShouldBe(69633L);
            pixels.IsFrameReady.ShouldBeFalse();

            // --- The top 48 border rows should be set to 0x05
            pixels.SetPixelMemory(spectrum.ScreenDevice.GetPixelBuffer());
            for (var row = 0; row < 48; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- Display rows should have a border value of 0x05 and a pixel value of 0x00
            for (var row = 48; row < 48 + 192; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.BorderLeftPixels; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
                for (var column = spectrum.ScreenDevice.ScreenConfiguration.BorderLeftPixels; 
                    column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth - spectrum.ScreenDevice.ScreenConfiguration.BorderRightPixels; 
                    column++)
                {
                    pixels[row, column].ShouldBe((byte)0x00);
                }
                for (var column = spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth - spectrum.ScreenDevice.ScreenConfiguration.BorderRightPixels;
                    column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth;
                    column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- The bottom 48 border rows should be set to 0x05
            for (var row = 48 + 192; row < spectrum.ScreenDevice.ScreenConfiguration.ScreenLines; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }
        }

        [TestMethod]
        public void RenderingScreenWithPatternWorks1()
        {
            // --- Arrange
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.ScreenConfiguration);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            // --- We render the screen with pixels with color index
            // --- of 0x09 and 0x0A in a chequered pattern
            spectrum.InitCode(new byte[]
            {
                0xF3,             // DI
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x75, 0x0A, // LD BC,$0A75
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });

            for (var addr = 0x4000; addr < 0x5800; addr++)
            {
                spectrum.WriteSpectrumMemory((ushort)addr, (byte)((addr & 0x0100) == 0 ? 0x55 : 0xAA));
            }
            for (var addr = 0x5800; addr < 0x5B00; addr++)
            {
                spectrum.WriteSpectrumMemory((ushort)addr, 0x51);
            }

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800D);
            spectrum.Cpu.Tacts.ShouldBe(69633L);
            pixels.IsFrameReady.ShouldBeFalse();

            // --- The top 48 border rows should be set to 0x05
            pixels.SetPixelMemory(spectrum.ScreenDevice.GetPixelBuffer());
            for (var row = 0; row < 48; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- Display rows should have a border value of 0x05 and a pixel value of 0x00
            for (var row = 48; row < 48 + 192; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.BorderLeftPixels; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
                for (var column = spectrum.ScreenDevice.ScreenConfiguration.BorderLeftPixels;
                    column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth - spectrum.ScreenDevice.ScreenConfiguration.BorderRightPixels;
                    column++)
                {
                    var expectedColor = (row + column) % 2 == 1  ? 0x09 : 0x0A;
                    pixels[row, column].ShouldBe((byte)expectedColor);
                }
                for (var column = spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth - spectrum.ScreenDevice.ScreenConfiguration.BorderRightPixels;
                    column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth;
                    column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- The bottom 48 border rows should be set to 0x05
            for (var row = 48 + 192; row < spectrum.ScreenDevice.ScreenConfiguration.ScreenLines; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }
        }

        [TestMethod]
        public void RenderingScreenWithUntilFrameEnds()
        {
            // --- Arrange
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.ScreenConfiguration);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            // === The same as RenderingScreenWithPatternWorks1 test case, but waits
            // === while the full frame is rendered.

            // --- We render the screen with pixels with color index
            // --- of 0x09 and 0x0A in a chequered pattern
            spectrum.InitCode(new byte[]
            {
                0xF3,             // DI
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x75, 0x0A, // LD BC,$0A75
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });

            for (var addr = 0x4000; addr < 0x5800; addr++)
            {
                spectrum.WriteSpectrumMemory((ushort)addr, (byte)((addr & 0x0100) == 0 ? 0x55 : 0xAA));
            }
            for (var addr = 0x5800; addr < 0x5B00; addr++)
            {
                spectrum.WriteSpectrumMemory((ushort)addr, 0x51);
            }
            var startTime = spectrum.Clock.GetCounter();

            // --- Act
            // === Be aware of EmulationMode.UntilFrameEnds
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));

            // === Display some extra information about the duration of the frame execution
            var duration = (spectrum.Clock.GetCounter() - startTime)
                /(double)spectrum.Clock.GetFrequency();
            Console.WriteLine("Frame execution time: {0} second", duration);

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800D);
            pixels.IsFrameReady.ShouldBeTrue();

            // === The full frame's tact time is used
            spectrum.Cpu.Tacts.ShouldBeGreaterThanOrEqualTo(spectrum.ScreenDevice.ScreenConfiguration.UlaFrameTactCount);

            // === The full time should not exceed the frame time + the longest Z80 instruction length,
            // === which is 23
            spectrum.Cpu.Tacts.ShouldBeLessThanOrEqualTo(spectrum.ScreenDevice.ScreenConfiguration.UlaFrameTactCount + 23);

            // --- The top 48 border rows should be set to 0x05
            pixels.SetPixelMemory(spectrum.ScreenDevice.GetPixelBuffer());
            for (var row = 0; row < 48; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- Display rows should have a border value of 0x05 and a pixel value of 0x00
            for (var row = 48; row < 48 + 192; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.BorderLeftPixels; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
                for (var column = spectrum.ScreenDevice.ScreenConfiguration.BorderLeftPixels;
                    column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth - spectrum.ScreenDevice.ScreenConfiguration.BorderRightPixels;
                    column++)
                {
                    var expectedColor = (row + column) % 2 == 1 ? 0x09 : 0x0A;
                    pixels[row, column].ShouldBe((byte)expectedColor);
                }
                for (var column = spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth - spectrum.ScreenDevice.ScreenConfiguration.BorderRightPixels;
                    column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth;
                    column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- The bottom 48 border rows should be set to 0x05
            for (var row = 48 + 192; row < spectrum.ScreenDevice.ScreenConfiguration.ScreenLines; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }
        }

        [TestMethod]
        public void RenderingScreenWithUntilNewFrameStarts()
        {
            // --- Arrange
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.ScreenConfiguration);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            // === The same as RenderingScreenWithPatternWorks1 test case, but waits
            // === while the full frame is rendered and a new frame is started.

            // --- We render the screen with pixels with color index
            // --- of 0x09 and 0x0A in a chequered pattern
            spectrum.InitCode(new byte[]
            {
                0xF3,             // DI
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x75, 0x0A, // LD BC,$0A75
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });

            for (var addr = 0x4000; addr < 0x5800; addr++)
            {
                spectrum.WriteSpectrumMemory((ushort)addr, (byte)((addr & 0x0100) == 0 ? 0x55 : 0xAA));
            }
            for (var addr = 0x5800; addr < 0x5B00; addr++)
            {
                spectrum.WriteSpectrumMemory((ushort)addr, 0x51);
            }
            var startTime = spectrum.Clock.GetCounter();

            // --- Act
            // === Be aware of EmulationMode.UntilNextFrame
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));

            // === Display some extra information about the duration of the frame execution
            var duration = (spectrum.Clock.GetCounter() - startTime)
                / (double)spectrum.Clock.GetFrequency();
            Console.WriteLine("Frame execution time: {0} second", duration);

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800D);
            pixels.IsFrameReady.ShouldBeTrue();

            // === The full frame's tact time is used
            spectrum.Cpu.Tacts.ShouldBeGreaterThanOrEqualTo(spectrum.ScreenDevice.ScreenConfiguration.UlaFrameTactCount);

            // === The full time should not exceed the frame time + the longest Z80 instruction length,
            // === which is 23
            spectrum.Cpu.Tacts.ShouldBeLessThanOrEqualTo(spectrum.ScreenDevice.ScreenConfiguration.UlaFrameTactCount + 23);

            // --- The top 48 border rows should be set to 0x05
            pixels.SetPixelMemory(spectrum.ScreenDevice.GetPixelBuffer());
            for (var row = 0; row < 48; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- Display rows should have a border value of 0x05 and a pixel value of 0x00
            for (var row = 48; row < 48 + 192; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.BorderLeftPixels; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
                for (var column = spectrum.ScreenDevice.ScreenConfiguration.BorderLeftPixels;
                    column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth - spectrum.ScreenDevice.ScreenConfiguration.BorderRightPixels;
                    column++)
                {
                    var expectedColor = (row + column) % 2 == 1 ? 0x09 : 0x0A;
                    pixels[row, column].ShouldBe((byte)expectedColor);
                }
                for (var column = spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth - spectrum.ScreenDevice.ScreenConfiguration.BorderRightPixels;
                    column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth;
                    column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- The bottom 48 border rows should be set to 0x05
            for (var row = 48 + 192; row < spectrum.ScreenDevice.ScreenConfiguration.ScreenLines; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }
        }

        [TestMethod]
        public void RenderingTenScreenFramesWorksAsExpected()
        {
            // --- Arrange
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.ScreenConfiguration);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            // === The same as RenderingScreenWithPatternWorks1 test case, but waits
            // === while the full frame is rendered and a new frame is started.

            // --- We render the screen with pixels with color index
            // --- of 0x09 and 0x0A in a chequered pattern
            spectrum.InitCode(new byte[]
            {
                0xF3,             // DI
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x75, 0x0A, // LD BC,$0A75
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0x76              // HALT
            });

            for (var addr = 0x4000; addr < 0x5800; addr++)
            {
                spectrum.WriteSpectrumMemory((ushort)addr, (byte)((addr & 0x0100) == 0 ? 0x55 : 0xAA));
            }
            for (var addr = 0x5800; addr < 0x5B00; addr++)
            {
                spectrum.WriteSpectrumMemory((ushort)addr, 0x51);
            }
            var startTime = spectrum.Clock.GetCounter();

            // --- Act
            // === Be aware of EmulationMode.UntilNextFrame
            for (var i = 0; i < 10; i++)
            {
                spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            }

            // === Display some extra information about the duration of the frame execution
            var duration = (spectrum.Clock.GetCounter() - startTime)
                / (double)spectrum.Clock.GetFrequency();
            Console.WriteLine("Frame execution time: {0} second", duration);

            // --- Assert
            var regs = spectrum.Cpu.Registers;
            regs.PC.ShouldBe((ushort)0x800D);
            pixels.IsFrameReady.ShouldBeTrue();

            // === The full frame's tact time is used
            spectrum.Cpu.Tacts.ShouldBeGreaterThanOrEqualTo(spectrum.ScreenDevice.ScreenConfiguration.UlaFrameTactCount*10);

            // === The full time should not exceed the 10*frame time + the longest Z80 instruction length,
            // === which is 23
            spectrum.Cpu.Tacts.ShouldBeLessThanOrEqualTo(spectrum.ScreenDevice.ScreenConfiguration.UlaFrameTactCount*10 + 23);

            // --- The top 48 border rows should be set to 0x05
            pixels.SetPixelMemory(spectrum.ScreenDevice.GetPixelBuffer());
            for (var row = 0; row < 48; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- Display rows should have a border value of 0x05 and a pixel value of 0x00
            for (var row = 48; row < 48 + 192; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.BorderLeftPixels; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
                for (var column = spectrum.ScreenDevice.ScreenConfiguration.BorderLeftPixels;
                    column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth - spectrum.ScreenDevice.ScreenConfiguration.BorderRightPixels;
                    column++)
                {
                    var expectedColor = (row + column) % 2 == 1 ? 0x09 : 0x0A;
                    pixels[row, column].ShouldBe((byte)expectedColor);
                }
                for (var column = spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth - spectrum.ScreenDevice.ScreenConfiguration.BorderRightPixels;
                    column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth;
                    column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }

            // --- The bottom 48 border rows should be set to 0x05
            for (var row = 48 + 192; row < spectrum.ScreenDevice.ScreenConfiguration.ScreenLines; row++)
            {
                for (var column = 0; column < spectrum.ScreenDevice.ScreenConfiguration.ScreenWidth; column++)
                {
                    pixels[row, column].ShouldBe((byte)0x05);
                }
            }
        }

        [TestMethod]
        public void ExecutionCyleWorksWithCancellation()
        {
            // --- Arrange
            var pixels = new TestPixelRenderer(SpectrumModels.ZxSpectrum48Pal.ScreenConfiguration);
            var spectrum = new SpectrumAdvancedTestMachine(pixels);

            // === The same as RenderingScreenWithPatternWorks1 test case, but waits
            // === while the full frame is rendered and a new frame is started.

            // --- We render the screen with pixels with color index
            // --- of 0x09 and 0x0A in a chequered pattern
            spectrum.InitCode(new byte[]
            {
                0xF3,             // DI
                0x3E, 0x05,       // LD A,$05
                0xD3, 0xFE,       // OUT ($FE),A
                0x01, 0x75, 0x0A, // LD BC,$0A75
                0x0B,             // DECLB: DEC BC
                0x78,             // LD A,B
                0xB1,             // OR C
                0x20, 0xFB,       // JR NZ,DECLB
                0xFB,             // EI
                0x76              // HALT
            });

            for (var addr = 0x4000; addr < 0x5800; addr++)
            {
                spectrum.WriteSpectrumMemory((ushort)addr, (byte)((addr & 0x0100) == 0 ? 0x55 : 0xAA));
            }
            for (var addr = 0x5800; addr < 0x5B00; addr++)
            {
                spectrum.WriteSpectrumMemory((ushort)addr, 0x51);
            }
            var counter = spectrum.Clock.GetCounter();
            var cancellationTime = counter + spectrum.Clock.GetFrequency()/100000; // 0.01ms

            var startTime = spectrum.Clock.GetCounter();
            var cancellationSource = new CancellationTokenSource();

            // --- Act
            // === We wait up to two frames
            Task.WaitAll(
                Task.Run(() => spectrum.ExecuteCycle(cancellationSource.Token, new ExecuteCycleOptions(EmulationMode.UntilFrameEnds)), 
                    cancellationSource.Token),
                Task.Run(() =>
                {
                    spectrum.Clock.WaitUntil(cancellationTime, cancellationSource.Token);
                    cancellationSource.Cancel();
                }, cancellationSource.Token));

            // === Display some extra information about the duration of the frame execution
            var duration = (spectrum.Clock.GetCounter() - startTime)
                / (double)spectrum.Clock.GetFrequency();
            Console.WriteLine("Frame execution time: {0} second", duration);

            // --- Assert
            // === Only a part of the frame's tact time is used
            spectrum.Cpu.Tacts.ShouldBeLessThan(spectrum.ScreenDevice.ScreenConfiguration.UlaFrameTactCount);
        }
    }
}
