using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Machine
{
    [TestClass]
    public class FlagConditionTest: ConditionalBreakpointTestBed
    {
        [TestMethod]
        public void FlagZWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x00,       // LD A,$00
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`Z");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.ZFlag.ShouldBeTrue();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void FlagZWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x01,       // LD A,$01
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`Z");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void FlagNzWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x01,       // LD A,$01
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`NZ");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.ZFlag.ShouldBeFalse();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void FlagNzWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x00,       // LD A,$00
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`NZ");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void FlagCWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x88,       // LD A,$88
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`C");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.CFlag.ShouldBeTrue();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void FlagCWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x36,       // LD A,$36
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`C");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void FlagNcWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x36,       // LD A,$36
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`NC");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.CFlag.ShouldBeFalse();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void FlagNcWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x88,       // LD A,$88
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`NC");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void FlagMWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x44,       // LD A,$44
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`M");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.SFlag.ShouldBeTrue();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void FlagMWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x36,       // LD A,$36
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`M");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void FlagPWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x36,       // LD A,$36
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`P");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.SFlag.ShouldBeFalse();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void FlagPWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x44,       // LD A,$44
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`P");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void FlagHWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x08,       // LD A,$08
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`H");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.HFlag.ShouldBeTrue();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void FlagHWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x02,       // LD A,$02
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`H");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void FlagNhWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x02,       // LD A,$02
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`NH");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.HFlag.ShouldBeFalse();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void FlagNhWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x08,       // LD A,$08
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`NH");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void FlagPeWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x03,       // LD A,$03
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`PE");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PFlag.ShouldBeTrue();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void FlagPeWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x02,       // LD A,$02
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`PE");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void FlagPoWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x02,       // LD A,$02
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`PO");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PFlag.ShouldBeFalse();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void FlagPoWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x03,       // LD A,$03
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`PO");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void FlagNWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x08,       // LD A,$08
                0x97,             // SUB A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`N");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.NFlag.ShouldBeTrue();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void FlagNWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x02,       // LD A,$02
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`N");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void FlagNnWorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x08,       // LD A,$08
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`NN");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.NFlag.ShouldBeFalse();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void FlagNnWorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x02,       // LD A,$02
                0x97,             // SUB A,A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`NN");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void Flag3WorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x08,       // LD A,$08
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`3");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.R3Flag.ShouldBeTrue();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void Flag3WorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x04,       // LD A,$04
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`3");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void FlagN3WorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x04,       // LD A,$04
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`N3");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.R3Flag.ShouldBeFalse();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void FlagN3WorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x08,       // LD A,$08
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`N3");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void Flag5WorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x20,       // LD A,$20
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`5");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.R5Flag.ShouldBeTrue();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void Flag5WorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x04,       // LD A,$04
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`5");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void FlagN5WorksWithTrueCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x04,       // LD A,$04
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`N5");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.R5Flag.ShouldBeFalse();
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void FlagN5WorksWithFalseCondition()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            spectrum.DebugExpressionContext = new SpectrumEvaluationContext(spectrum);
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x20,       // LD A,$20
                0xB7,             // OR A
                0x47,             // LD B,A
                0x76,             // HALT
            });

            var bp = CreateBreakpoint(null, "`N5");
            debugProvider.Breakpoints.Add(0x8003, bp);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8004);
        }

    }
}