using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Machine;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Test.Helpers;
// ReSharper disable CommentTypo

namespace Spect.Net.SpectrumEmu.Test.Machine
{
    [TestClass]
    public class Register16BitConditionTest: ConditionalBreakpointTestBed
    {
        [TestMethod]
        public void RegisterAfWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x3E, 0x00,       // LD A,$00
                0x3C,             // INC A
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "AF == 0");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.AF.ShouldBe((ushort)0x00);
            regs.B.ShouldBe((byte)0x80);
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void RegisterAfWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x3E, 0x00,       // LD A,$00
                0x3C,             // INC A
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "AF == 0x10");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.AF.ShouldBe((ushort)0x8094);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void RegisterAf2WorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x3E, 0x00,       // LD A,$00
                0x3C,             // INC A
                0x08,             // EX AF,AF'
                0x08,             // EX AF,AF'
                0x10, 0xFB,       // DJNZ -5
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "AF' == 0x0100");
            debugProvider.Breakpoints.Add(0x8006, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8006);
            regs._AF_.ShouldBe((ushort)0x0100);
            regs.B.ShouldBe((byte)0x80);
        }

        [TestMethod]
        public void RegisterAf2WorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x3E, 0x00,       // LD A,$00
                0x3C,             // INC A
                0x08,             // EX AF,AF'
                0x08,             // EX AF,AF'
                0x10, 0xFB,       // DJNZ -5
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "AF' == 0xFF00");
            debugProvider.Breakpoints.Add(0x8006, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8009);
            regs.B.ShouldBe((byte)0x00);
        }

        [TestMethod]
        public void RegisterBcWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x0E, 0x00,       // LD C,$00
                0x0C,             // INC C
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "BC == 0x7E02");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.BC.ShouldBe((ushort)0x7E02);
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void RegisterBcWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x0E, 0x00,       // LD C,$00
                0x0C,             // INC C
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "BC == 0x0000");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.BC.ShouldBe((ushort)0x0080);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void RegisterBc2WorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x0E, 0x00,       // LD C,$00
                0x0C,             // INC C
                0xD9,             // EXX
                0xD9,             // EXX
                0x10, 0xFb,       // DJNZ -5
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "BC' == 0x7E03");
            debugProvider.Breakpoints.Add(0x8006, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8006);
            regs.BC.ShouldBe((ushort)0x0000);
        }

        [TestMethod]
        public void RegisterBc2WorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x0E, 0x00,       // LD C,$00
                0x0C,             // INC C
                0xD9,             // EXX
                0xD9,             // EXX
                0x10, 0xFb,       // DJNZ -5
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "BC' == 0x0123");
            debugProvider.Breakpoints.Add(0x8006, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8009);
            regs.BC.ShouldBe((ushort)0x0080);
        }

        [TestMethod]
        public void RegisterDeWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x11, 0x00, 0x40, // LD DE,$4000
                0x13,             // INC DE
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "DE == 0x4003");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8008, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8005);
            regs.DE.ShouldBe((ushort)0x4003);
            regs.B.ShouldBe((byte)0x7D);
        }

        [TestMethod]
        public void RegisterDeWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x11, 0x00, 0x40, // LD DE,$4000
                0x13,             // INC DE
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "DE == 0x6003");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8008, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8008);
            regs.B.ShouldBe((byte)0x00);
        }

        [TestMethod]
        public void RegisterDe2WorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x11, 0x00, 0x40, // LD DE,$4000
                0x13,             // INC DE
                0xD9,             // EXX
                0xD9,             // EXX
                0x10, 0xFB,       // DJNZ -5
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "DE' == 0x4003");
            debugProvider.Breakpoints.Add(0x8007, bp);
            debugProvider.Breakpoints.Add(0x800A, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8007);
            regs.DE.ShouldBe((ushort)0x0000);
            regs.B.ShouldBe((byte)0x00);
        }

        [TestMethod]
        public void RegisterDe2WorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x11, 0x00, 0x40, // LD DE,$4000
                0x13,             // INC DE
                0xD9,             // EXX
                0xD9,             // EXX
                0x10, 0xFB,       // DJNZ -5
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "DE' == 0x6003");
            debugProvider.Breakpoints.Add(0x8007, bp);
            debugProvider.Breakpoints.Add(0x800A, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x800A);
            regs.B.ShouldBe((byte)0x00);
        }

        [TestMethod]
        public void RegisterHlWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x21, 0x00, 0x40, // LD HL,$4000
                0x23,             // INC HL
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "HL == 0x4003");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8008, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8005);
            regs.HL.ShouldBe((ushort)0x4003);
            regs.B.ShouldBe((byte)0x7D);
        }

        [TestMethod]
        public void RegisterHlWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x21, 0x00, 0x40, // LD HL,$4000
                0x23,             // INC HL
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "HL == 0x6003");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8008, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8008);
            regs.B.ShouldBe((byte)0x00);
        }

        [TestMethod]
        public void RegisterHl2WorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x21, 0x00, 0x40, // LD HL,$4000
                0x23,             // INC HL
                0xD9,             // EXX
                0xD9,             // EXX
                0x10, 0xFB,       // DJNZ -5
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "HL' == 0x4003");
            debugProvider.Breakpoints.Add(0x8007, bp);
            debugProvider.Breakpoints.Add(0x800A, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8007);
            regs.HL.ShouldBe((ushort)0x0000);
            regs.B.ShouldBe((byte)0x00);
        }

        [TestMethod]
        public void RegisterHl2WorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x21, 0x00, 0x40, // LD HL,$4000
                0x23,             // INC HL
                0xD9,             // EXX
                0xD9,             // EXX
                0x10, 0xFB,       // DJNZ -5
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "HL' == 0x6003");
            debugProvider.Breakpoints.Add(0x8007, bp);
            debugProvider.Breakpoints.Add(0x800A, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x800A);
            regs.B.ShouldBe((byte)0x00);
        }

        [TestMethod]
        public void RegisterIxWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,             // LD B,$80
                0xDD, 0x21, 0x00, 0x40, // LD IX,$4000
                0xDD, 0x23,             // INC IX
                0x10, 0xFC,             // DJNZ -4
                0x76,                   // HALT
            });

            var bp = CreateBreakpoint(null, "IX == 0x4003");
            debugProvider.Breakpoints.Add(0x8006, bp);
            debugProvider.Breakpoints.Add(0x800A, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8006);
            regs.IX.ShouldBe((ushort)0x4003);
            regs.B.ShouldBe((byte)0x7D);
        }

        [TestMethod]
        public void RegisterIxWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,             // LD B,$80
                0xDD, 0x21, 0x00, 0x40, // LD IX,$4000
                0xDD, 0x23,             // INC IX
                0x10, 0xFC,             // DJNZ -4
                0x76,                   // HALT
            });

            var bp = CreateBreakpoint(null, "IX == 0x6003");
            debugProvider.Breakpoints.Add(0x8006, bp);
            debugProvider.Breakpoints.Add(0x800A, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x800A);
            regs.B.ShouldBe((byte)0x00);
        }

        [TestMethod]
        public void RegisterIyWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,             // LD B,$80
                0xFD, 0x21, 0x00, 0x40, // LD IY,$4000
                0xFD, 0x23,             // INC IY
                0x10, 0xFC,             // DJNZ -4
                0x76,                   // HALT
            });

            var bp = CreateBreakpoint(null, "IY == 0x4003");
            debugProvider.Breakpoints.Add(0x8006, bp);
            debugProvider.Breakpoints.Add(0x800A, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8006);
            regs.IY.ShouldBe((ushort)0x4003);
            regs.B.ShouldBe((byte)0x7D);
        }

        [TestMethod]
        public void RegisterIyWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,             // LD B,$80
                0xFD, 0x21, 0x00, 0x40, // LD IY,$4000
                0xFD, 0x23,             // INC IY
                0x10, 0xFC,             // DJNZ -4
                0x76,                   // HALT
            });

            var bp = CreateBreakpoint(null, "IY == 0x6003");
            debugProvider.Breakpoints.Add(0x8006, bp);
            debugProvider.Breakpoints.Add(0x800A, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x800A);
            regs.B.ShouldBe((byte)0x00);
        }

        [TestMethod]
        public void RegisterSpWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x31, 0x00, 0x40, // LD SP,$4000
                0x33,             // INC SP
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "SP == 0x4003");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8008, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8005);
            regs.SP.ShouldBe((ushort)0x4003);
            regs.B.ShouldBe((byte)0x7D);
        }

        [TestMethod]
        public void RegisterSpWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x31, 0x00, 0x40, // LD SP,$4000
                0x33,             // INC SP
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "SP == 0x6003");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8008, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8008);
            regs.B.ShouldBe((byte)0x00);
        }

        [TestMethod]
        public void RegisterPcWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x31, 0x00, 0x40, // LD SP,$4000
                0x33,             // INC SP
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "PC == 0x8005");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8008, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8005);
            regs.B.ShouldBe((byte)0x80);
        }

        [TestMethod]
        public void RegisterPcWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x06, 0x80,       // LD B,$80
                0x31, 0x00, 0x40, // LD SP,$4000
                0x33,             // INC SP
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "PC == 0x6003");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8008, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8008);
            regs.B.ShouldBe((byte)0x00);
        }

    }
}