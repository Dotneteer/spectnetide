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
    public class Register8BitConditionTest: ConditionalBreakpointTestBed
    {
        [TestMethod]
        public void RegisterAWorksWithTrueCondition()
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

            var bp = CreateBreakpoint(null, "A == 4");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.A.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x7C);
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void RegisterAWorksWithFalseCondition()
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

            var bp = CreateBreakpoint(null, "A == 0xFF");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.A.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void RegisterBWorksWithTrueCondition()
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

            var bp = CreateBreakpoint(null, "B == 4");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.A.ShouldBe((byte)0x7C);
            regs.B.ShouldBe((byte)0x04);
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void RegisterBWorksWithFalseCondition()
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

            var bp = CreateBreakpoint(null, "B == 0xFF");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.A.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void RegisterCWorksWithTrueCondition()
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

            var bp = CreateBreakpoint(null, "C == 4");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.C.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x7C);
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void RegisterCWorksWithFalseCondition()
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

            var bp = CreateBreakpoint(null, "C == 0xFF");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.C.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void RegisterDWorksWithTrueCondition()
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
                0x16, 0x00,       // LD D,$00
                0x14,             // INC D
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "D == 4");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.D.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x7C);
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void RegisterDWorksWithFalseCondition()
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
                0x16, 0x00,       // LD D,$00
                0x14,             // INC D
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "D == 0xFF");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.D.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void RegisterEWorksWithTrueCondition()
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
                0x1E, 0x00,       // LD E,$00
                0x1C,             // INC E
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "E == 4");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.E.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x7C);
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void RegisterEWorksWithFalseCondition()
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
                0x1E, 0x00,       // LD E,$00
                0x1C,             // INC E
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "E == 0xFF");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.E.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void RegisterHWorksWithTrueCondition()
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
                0x26, 0x00,       // LD H,$00
                0x24,             // INC H
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "H == 4");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.H.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x7C);
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void RegisterHWorksWithFalseCondition()
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
                0x26, 0x00,       // LD H,$00
                0x24,             // INC H
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "H == 0xFF");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.H.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void RegisterLWorksWithTrueCondition()
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
                0x2E, 0x00,       // LD L,$00
                0x2C,             // INC L
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "L == 4");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.L.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x7C);
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void RegisterLWorksWithFalseCondition()
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
                0x2E, 0x00,       // LD L,$00
                0x2C,             // INC L
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "L == 0xFF");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.L.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void RegisterFWorksWithTrueCondition()
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
                0x26, 0x00,       // LD H,$00
                0x24,             // INC H
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "F == 0");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.F.ShouldBe((byte)0x00);
            regs.H.ShouldBe((byte)0x00);
            regs.B.ShouldBe((byte)0x80);
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void RegisterFWorksWithFalseCondition()
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
                0x26, 0x00,       // LD H,$00
                0x24,             // INC H
                0x10, 0xFD,       // DJNZ -3
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "F == 0xFF");
            debugProvider.Breakpoints.Add(0x8004, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.H.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void RegisterXhWorksWithTrueCondition()
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
                0xDD, 0x26, 0x00, // LD XH,$00
                0xDD, 0x24,       // INC XH
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "XH == 4");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.XH.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x7C);
            regs.PC.ShouldBe((ushort)0x8005);
        }

        [TestMethod]
        public void RegisterXhWorksWithFalseCondition()
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
                0xDD, 0x26, 0x00, // LD XH,$00
                0xDD, 0x24,       // INC XH
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "XH == 0xFF");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.XH.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8009);
        }

        [TestMethod]
        public void RegisterXlWorksWithTrueCondition()
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
                0xDD, 0x2E, 0x00, // LD XL,$00
                0xDD, 0x2C,       // INC XL
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "XL == 4");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.XL.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x7C);
            regs.PC.ShouldBe((ushort)0x8005);
        }

        [TestMethod]
        public void RegisterXlWorksWithFalseCondition()
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
                0xDD, 0x2E, 0x00, // LD XL,$00
                0xDD, 0x2C,       // INC XL
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "XL == 0xFF");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.XL.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8009);
        }

        [TestMethod]
        public void RegisterYhWorksWithTrueCondition()
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
                0xFD, 0x26, 0x00, // LD YH,$00
                0xFD, 0x24,       // INC YH
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "YH == 4");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.YH.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x7C);
            regs.PC.ShouldBe((ushort)0x8005);
        }

        [TestMethod]
        public void RegisterYhWorksWithFalseCondition()
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
                0xFD, 0x26, 0x00, // LD YH,$00
                0xFD, 0x24,       // INC YH
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "YH == 0xFF");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.YH.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8009);
        }

        [TestMethod]
        public void RegisterYlWorksWithTrueCondition()
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
                0xFD, 0x2E, 0x00, // LD YL,$00
                0xFD, 0x2C,       // INC YL
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "YL == 4");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.YL.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x7C);
            regs.PC.ShouldBe((ushort)0x8005);
        }

        [TestMethod]
        public void RegisterYlWorksWithFalseCondition()
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
                0xFD, 0x2E, 0x00, // LD YL,$00
                0xFD, 0x2C,       // INC YL
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "YL == 0xFF");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.YL.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8009);
        }

        [TestMethod]
        public void RegisterIxhWorksWithTrueCondition()
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
                0xDD, 0x26, 0x00, // LD XH,$00
                0xDD, 0x24,       // INC XH
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "IXH == 4");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.XH.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x7C);
            regs.PC.ShouldBe((ushort)0x8005);
        }

        [TestMethod]
        public void RegisterIxhWorksWithFalseCondition()
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
                0xDD, 0x26, 0x00, // LD XH,$00
                0xDD, 0x24,       // INC XH
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "IXH == 0xFF");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.XH.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8009);
        }

        [TestMethod]
        public void RegisterIxlWorksWithTrueCondition()
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
                0xDD, 0x2E, 0x00, // LD XL,$00
                0xDD, 0x2C,       // INC XL
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "IXL == 4");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.XL.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x7C);
            regs.PC.ShouldBe((ushort)0x8005);
        }

        [TestMethod]
        public void RegisterIxlWorksWithFalseCondition()
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
                0xDD, 0x2E, 0x00, // LD XL,$00
                0xDD, 0x2C,       // INC XL
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "IXL == 0xFF");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.XL.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8009);
        }

        [TestMethod]
        public void RegisterIyhWorksWithTrueCondition()
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
                0xFD, 0x26, 0x00, // LD YH,$00
                0xFD, 0x24,       // INC YH
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "IYH == 4");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.YH.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x7C);
            regs.PC.ShouldBe((ushort)0x8005);
        }

        [TestMethod]
        public void RegisterIyhWorksWithFalseCondition()
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
                0xFD, 0x26, 0x00, // LD YH,$00
                0xFD, 0x24,       // INC YH
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "IYH == 0xFF");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.YH.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8009);
        }

        [TestMethod]
        public void RegisterIylWorksWithTrueCondition()
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
                0xFD, 0x2E, 0x00, // LD YL,$00
                0xFD, 0x2C,       // INC YL
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "IYL == 4");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.YL.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x7C);
            regs.PC.ShouldBe((ushort)0x8005);
        }

        [TestMethod]
        public void RegisterIylWorksWithFalseCondition()
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
                0xFD, 0x2E, 0x00, // LD YL,$00
                0xFD, 0x2C,       // INC YL
                0x10, 0xFC,       // DJNZ -4
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "IYL == 0xFF");
            debugProvider.Breakpoints.Add(0x8005, bp);
            debugProvider.Breakpoints.Add(0x8009, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.YL.ShouldBe((byte)0x80);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8009);
        }

        [TestMethod]
        public void RegisterIWorksWithTrueCondition()
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
                0x78,             // LD A,B
                0xED, 0x47,       // LD I,A
                0x10, 0xFB,       // DJNZ -5
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "I == 4");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.I.ShouldBe((byte)0x04);
            regs.B.ShouldBe((byte)0x03);
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void RegisterIWorksWithFalseCondition()
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
                0x78,             // LD A,B
                0xED, 0x47,       // LD I,A
                0x10, 0xFB,       // DJNZ -5
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "I == 0xFF");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.I.ShouldBe((byte)0x01);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void RegisterRWorksWithTrueCondition()
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
                0x78,             // LD A,B
                0xED, 0x47,       // LD I,A
                0x10, 0xFB,       // DJNZ -5
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "R == 14");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.R.ShouldBe((byte)0x0E);
            regs.I.ShouldBe((byte)0x7E);
            regs.B.ShouldBe((byte)0x7D);
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void RegisterRWorksWithFalseCondition()
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
                0x78,             // LD A,B
                0xED, 0x47,       // LD I,A
                0x10, 0xFB,       // DJNZ -5
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "R == 13");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8007, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.I.ShouldBe((byte)0x01);
            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8007);
        }
    }
}
