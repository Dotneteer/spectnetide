using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Devices;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Machine
{
    [TestClass]
    public class DebuggerModeTests
    {
        [TestMethod]
        public void MachineStopsAtBreakpoint()
        {
            // --- Arrange
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            debugProvider.Breakpoints.Add(0x8003);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            debugProvider.Breakpoints.Add(0x8003);
            debugProvider.Breakpoints.Add(0x8004);
            var regs = spectrum.Cpu.Registers;

            // --- Act
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepInto);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepInto);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepInto);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepInto);
            var pc3 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepInto);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc3 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

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
            var pars = new DisplayParameters();
            var pixels = new TestPixelRenderer(pars);
            var spectrum = new SpectrumAdvancedTestMachine(pars, pixels);
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
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc1 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);
            var pc2 = regs.PC;
            spectrum.ExecuteCycle(CancellationToken.None, EmulationMode.Debugger, DebugStepMode.StepOver);

            // --- Assert
            regs.D.ShouldBe((byte)0x06);
            pc1.ShouldBe((ushort)0x8003);
            pc2.ShouldBe((ushort)0x8004);
            regs.PC.ShouldBe((ushort)0x8005);
        }
    }
}
