using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Machine;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Machine
{
    [TestClass]
    public class DebuggerModeTests
    {
        [TestMethod]
        public void MachineStopsAtFirstInstructionBreakpoint()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x4F,             // LD C,A
            });
            debugProvider.Breakpoints.Add(0x8000, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.A.ShouldBe((byte)0x00);
            regs.B.ShouldBe((byte)0x00);
            regs.C.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void MachineStopsAtFirstInstructionBreakpointAndStepsFurther()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x4F,             // LD C,A
            });
            debugProvider.Breakpoints.Add(0x8000, MinimumBreakpointInfo.EmptyBreakpointInfo);
            debugProvider.Breakpoints.Add(0x8002, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.A.ShouldBe((byte)0x10);
            regs.B.ShouldBe((byte)0x00);
            regs.C.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8002);
        }


        [TestMethod]
        public void MachineStopsAtBreakpoint()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x4F,             // LD C,A
            });
            debugProvider.Breakpoints.Add(0x8003, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.A.ShouldBe((byte)0x20);
            regs.B.ShouldBe((byte)0x00);
            regs.C.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void MachineStopsAtMultipleBreakpoints()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x4F,             // LD C,A
            });
            debugProvider.Breakpoints.Add(0x8003, MinimumBreakpointInfo.EmptyBreakpointInfo);
            debugProvider.Breakpoints.Add(0x8004, MinimumBreakpointInfo.EmptyBreakpointInfo);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger));

            // --- Assert
            regs.A.ShouldBe((byte)0x20);
            regs.B.ShouldBe((byte)0x20);
            regs.C.ShouldBe((byte)0x00);
            pc1.ShouldBe((ushort)0x8003);
            regs.PC.ShouldBe((ushort)0x8004);
        }

        [TestMethod]
        public void StepIntoStopsAtNextInstruction()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x4F,             // LD C,A
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Assert
            regs.A.ShouldBe((byte)0x10);
            regs.B.ShouldBe((byte)0x00);
            regs.C.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8002);
        }

        [TestMethod]
        public void StepIntoWorksWithMultipleSteps()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x4F,             // LD C,A
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            var pc3 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Assert
            regs.A.ShouldBe((byte)0x20);
            regs.B.ShouldBe((byte)0x20);
            regs.C.ShouldBe((byte)0x20);
            pc1.ShouldBe((ushort)0x8002);
            pc2.ShouldBe((ushort)0x8003);
            pc3.ShouldBe((ushort)0x8004);
            regs.PC.ShouldBe((ushort)0x8005);
        }

        [TestMethod]
        public void StepOverStopsAtNextInstruction()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x4F,             // LD C,A
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));


            // --- Assert
            regs.A.ShouldBe((byte)0x10);
            regs.B.ShouldBe((byte)0x00);
            regs.C.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x8002);
        }

        [TestMethod]
        public void StepOverWorksWithMultipleSteps()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0x87,             // ADD A,A
                0x47,             // LD B,A
                0x4F,             // LD C,A
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc3 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x20);
            regs.B.ShouldBe((byte)0x20);
            regs.C.ShouldBe((byte)0x20);
            pc1.ShouldBe((ushort)0x8002);
            pc2.ShouldBe((ushort)0x8003);
            pc3.ShouldBe((ushort)0x8004);
            regs.PC.ShouldBe((ushort)0x8005);
        }

        [TestMethod]
        public void StepOverWorksWithCall()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0xB7,             // OR A
                0xCD, 0x09, 0x80, // CALL $8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x87,             // ADD A,A
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x20);
            regs.B.ShouldBe((byte)0x20);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x20);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallZTrue()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0xAF,             // XOR A
                0xCC, 0x09, 0x80, // CALL Z,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x20);
            regs.B.ShouldBe((byte)0x20);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x20);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallZFalse()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0xB7,             // OR A
                0xCC, 0x09, 0x80, // CALL Z,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x10);
            regs.B.ShouldBe((byte)0x10);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x00);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallNzTrue()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0xB7,             // OR A
                0xC4, 0x09, 0x80, // CALL NZ,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x20);
            regs.B.ShouldBe((byte)0x20);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x20);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallNzFalse()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0xAF,             // XOR A
                0xC4, 0x09, 0x80, // CALL NZ,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x00);
            regs.B.ShouldBe((byte)0x00);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x00);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallCTrue()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0x37,             // SCF
                0xDC, 0x09, 0x80, // CALL C,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x20);
            regs.B.ShouldBe((byte)0x20);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x20);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallCFalse()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0xA7,             // AND A
                0xDC, 0x09, 0x80, // CALL C,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x10);
            regs.B.ShouldBe((byte)0x10);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x00);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallNcTrue()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0xA7,             // AND A
                0xD4, 0x09, 0x80, // CALL NC,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x20);
            regs.B.ShouldBe((byte)0x20);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x20);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallNcFalse()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0x37,             // SCF
                0xD4, 0x09, 0x80, // CALL NC,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x10);
            regs.B.ShouldBe((byte)0x10);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x00);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallPeTrue()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x11,       // LD A,$11
                0xB7,             // OR A
                0xEC, 0x09, 0x80, // CALL PE,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x20);
            regs.B.ShouldBe((byte)0x20);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x20);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallPeFalse()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0xB7,             // OR A
                0xEC, 0x09, 0x80, // CALL PE,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x10);
            regs.B.ShouldBe((byte)0x10);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x00);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallPoTrue()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0xB7,             // OR A
                0xE4, 0x09, 0x80, // CALL PO,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x20);
            regs.B.ShouldBe((byte)0x20);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x20);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallPoFalse()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x11,       // LD A,$11
                0xB7,             // OR A
                0xE4, 0x09, 0x80, // CALL PO,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x11);
            regs.B.ShouldBe((byte)0x11);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x00);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallMTrue()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x90,       // LD A,$90
                0xB7,             // OR A
                0xFC, 0x09, 0x80, // CALL M,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x20);
            regs.B.ShouldBe((byte)0x20);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x20);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallMFalse()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0xB7,             // OR A
                0xFC, 0x09, 0x80, // CALL M,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x10);
            regs.B.ShouldBe((byte)0x10);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x00);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallPTrue()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x10,       // LD A,$10
                0xB7,             // OR A
                0xF4, 0x09, 0x80, // CALL P,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x20);
            regs.B.ShouldBe((byte)0x20);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x20);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithCallPFalse()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x90,       // LD A,$90
                0xB7,             // OR A
                0xF4, 0x09, 0x80, // CALL P,$8009
                0x47,             // LD B,A
                0x4F,             // LD C,A
                0x76,             // HALT
                0x3E, 0x20,       // LD a,$20
                0x57,             // LD D,A
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.A.ShouldBe((byte)0x90);
            regs.B.ShouldBe((byte)0x90);
            regs.C.ShouldBe((byte)0x00);
            regs.D.ShouldBe((byte)0x00);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8006);
            regs.PC.ShouldBe((ushort)0x8007);
        }

        [TestMethod]
        public void StepOverWorksWithRst()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0xF3,       // DI
                0x16, 0x06, // LD D,6
                0xFF,       // RST $38
                0x7A,       // LD A,D
                0x76        // HALT
            });
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOver));

            // --- Assert
            regs.D.ShouldBe((byte)0x06);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8004);
            regs.PC.ShouldBe((ushort)0x8005);
        }

        [TestMethod]
        public void StepOutWorksWithCall()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x00,       // LD A,00H
                0xB7,             // OR A
                0xCD, 0x07, 0x80, // CALL 8007H
                0x76,             // HALT
                0x78,             // LD A,B
                0x00,             // NOP
                0x00,             // NOP
                0x00,             // NOP
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8006);
        }

        [TestMethod]
        public void StepOutWorksWithCallNz()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x2A,       // LD A,2AH
                0xB7,             // OR A
                0xC4, 0x07, 0x80, // CALL NZ, 8007H
                0x76,             // HALT
                0x78,             // LD A,B
                0x00,             // NOP
                0x00,             // NOP
                0x00,             // NOP
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8006);
        }

        [TestMethod]
        public void StepOutWorksWithCallZ()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x00,       // LD A,00H
                0xB7,             // OR A
                0xCC, 0x07, 0x80, // CALL Z, 8007H
                0x76,             // HALT
                0x78,             // LD A,B
                0x00,             // NOP
                0x00,             // NOP
                0x00,             // NOP
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8006);
        }

        [TestMethod]
        public void StepOutWorksWithCallNc()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x2A,       // LD A,2AH
                0xB7,             // OR A
                0xD4, 0x07, 0x80, // CALL NC, 8007H
                0x76,             // HALT
                0x78,             // LD A,B
                0x00,             // NOP
                0x00,             // NOP
                0x00,             // NOP
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8006);
        }

        [TestMethod]
        public void StepOutWorksWithCallC()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x2A,       // LD A,2AH
                0x37,             // SCF
                0xDC, 0x07, 0x80, // CALL C, 8007H
                0x76,             // HALT
                0x78,             // LD A,B
                0x00,             // NOP
                0x00,             // NOP
                0x00,             // NOP
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8006);
        }

        [TestMethod]
        public void StepOutWorksWithCallPo()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x2A,       // LD A,2AH
                0x87,             // ADD A
                0xE4, 0x07, 0x80, // CALL PO, 8007H
                0x76,             // HALT
                0x78,             // LD A,B
                0x00,             // NOP
                0x00,             // NOP
                0x00,             // NOP
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8006);
        }

        [TestMethod]
        public void StepOutWorksWithCallPe()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x88,       // LD A,88H
                0x87,             // ADD A
                0xEC, 0x07, 0x80, // CALL PE, 8007H
                0x76,             // HALT
                0x78,             // LD A,B
                0x00,             // NOP
                0x00,             // NOP
                0x00,             // NOP
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8006);
        }

        [TestMethod]
        public void StepOutWorksWithCallP()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x32,       // LD A,32H
                0x87,             // ADD A
                0xF4, 0x07, 0x80, // CALL P, 8007H
                0x76,             // HALT
                0x78,             // LD A,B
                0x00,             // NOP
                0x00,             // NOP
                0x00,             // NOP
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8006);
        }

        [TestMethod]
        public void StepOutWorksWithCallM()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0xC0,       // LD A,C0H
                0x87,             // ADD A
                0xFC, 0x07, 0x80, // CALL M, 8007H
                0x76,             // HALT
                0x78,             // LD A,B
                0x00,             // NOP
                0x00,             // NOP
                0x00,             // NOP
                0xC9              // RET
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8006);
        }

        [TestMethod]
        public void StepOutWorksWithRetNz()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0xCD, 0x04, 0x80, // CALL 8004H
                0x76,             // HALT
                0x3E, 0x2A,       // LD A,2AH
                0xB7,             // OR A
                0xC0,             // RET NZ
                0x76              // HALT
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void StepOutWorksWithRetZ()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0xCD, 0x04, 0x80, // CALL 8004H
                0x76,             // HALT
                0x3E, 0x00,       // LD A,00H
                0xB7,             // OR A
                0xC8,             // RET Z
                0x76              // HALT
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void StepOutWorksWithRetNc()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0xCD, 0x04, 0x80, // CALL 8004H
                0x76,             // HALT
                0x3E, 0x16,       // LD A,16H
                0xA7,             // AND A
                0xD0,             // RET NC
                0x76              // HALT
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void StepOutWorksWithRetC()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0xCD, 0x04, 0x80, // CALL 8004H
                0x76,             // HALT
                0x3E, 0x16,       // LD A,16H
                0x37,             // SCF
                0xD8,             // RET C
                0x76              // HALT
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void StepOutWorksWithRetPo()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0xCD, 0x04, 0x80, // CALL 8004H
                0x76,             // HALT
                0x3E, 0x2A,       // LD A,2AH
                0x87,             // ADD A
                0xE0,             // RET PO
                0x76              // HALT
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void StepOutWorksWithRetPe()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0xCD, 0x04, 0x80, // CALL 8004H
                0x76,             // HALT
                0x3E, 0x88,       // LD A,88H
                0x87,             // ADD A
                0xE8,             // RET PE
                0x76              // HALT
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void StepOutWorksWithRetP()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0xCD, 0x04, 0x80, // CALL 8004H
                0x76,             // HALT
                0x3E, 0x32,       // LD A,32H
                0x87,             // ADD A
                0xF0,             // RET P
                0x76              // HALT
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void StepOutWorksWithRetM()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0xCD, 0x04, 0x80, // CALL 8004H
                0x76,             // HALT
                0x3E, 0xC0,       // LD A,C0H
                0x87,             // ADD A
                0xF8,             // RET M
                0x76              // HALT
            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8003);
        }

        [TestMethod]
        public void StepOutWorksWithNestedCall()
        {
            // --- Arrange
            var spectrum = new SpectrumAdvancedTestMachine();
            var debugProvider = new TestDebugInfoProvider();
            spectrum.SetDebugInfoProvider(debugProvider);

            // --- We render the screen while the interrupt is disabled
            spectrum.InitCode(new byte[]
            {
                0x3E, 0x00,       // LD A,00H
                0xB7,             // OR A
                0xCD, 0x07, 0x80, // CALL 8007H
                0x76,             // HALT
                0x78,             // LD A,B
                0xCD, 0x0E, 0x80, // CALL 800EH
                0x00,             // NOP
                0x00,             // NOP
                0xC9,             // RET
                0x00,             // NOP
                0xC9              // RET

            });
            var regs = spectrum.Cpu.Registers;
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepInto));

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.Debugger, DebugStepMode.StepOut));

            // --- Assert
            regs.PC.ShouldBe((ushort)0x8006);
        }
    }
}
