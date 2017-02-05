using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core.ExtendedOps
{
    [TestClass]
    public class ExtendedOpTests
    {
        /// <summary>
        /// IN B,(C): 0xED 0x40
        /// </summary>
        [TestMethod]
        public void IN_B_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x40 // IN B,(C)
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;

            // --- Act
            m.Run();

            // --- Assert

            regs.B.ShouldBe((byte)0xD5);
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0xD5);
            m.IoAccessLog[0].IsOutput.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// OUT (C),B: 0xED 0x41
        /// </summary>
        [TestMethod]
        public void OUT_C_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x41        // OUT (C),B
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;

            // --- Act
            m.Run();

            // --- Assert
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0x12);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// IN C,(C): 0xED 0x48
        /// </summary>
        [TestMethod]
        public void IN_C_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x48 // IN C,(C)
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;

            // --- Act
            m.Run();

            // --- Assert

            regs.C.ShouldBe((byte)0xD5);
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0xD5);
            m.IoAccessLog[0].IsOutput.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// OUT (C),C: 0xED 0x49
        /// </summary>
        [TestMethod]
        public void OUT_C_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x49        // OUT (C),C
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;

            // --- Act
            m.Run();

            // --- Assert
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0x34);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// IN D,(C): 0xED 0x50
        /// </summary>
        [TestMethod]
        public void IN_D_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x50 // IN D,(C)
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;

            // --- Act
            m.Run();

            // --- Assert

            regs.D.ShouldBe((byte)0xD5);
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0xD5);
            m.IoAccessLog[0].IsOutput.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// OUT (C),D: 0xED 0x51
        /// </summary>
        [TestMethod]
        public void OUT_C_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x51        // OUT (C),D
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;
            regs.DE = 0xBA98;

            // --- Act
            m.Run();

            // --- Assert
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0xBA);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// IN E,(C): 0xED 0x58
        /// </summary>
        [TestMethod]
        public void IN_E_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x58 // IN E,(C)
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;

            // --- Act
            m.Run();

            // --- Assert

            regs.E.ShouldBe((byte)0xD5);
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0xD5);
            m.IoAccessLog[0].IsOutput.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// OUT (C),E: 0xED 0x59
        /// </summary>
        [TestMethod]
        public void OUT_C_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x59        // OUT (C),E
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;
            regs.DE = 0xBA98;

            // --- Act
            m.Run();

            // --- Assert
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0x98);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// IN H,(C): 0xED 0x60
        /// </summary>
        [TestMethod]
        public void IN_H_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x60 // IN H,(C)
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;

            // --- Act
            m.Run();

            // --- Assert

            regs.H.ShouldBe((byte)0xD5);
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0xD5);
            m.IoAccessLog[0].IsOutput.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, H");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// OUT (C),H: 0xED 0x61
        /// </summary>
        [TestMethod]
        public void OUT_C_H_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x61        // OUT (C),H
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;
            regs.HL = 0xBA98;

            // --- Act
            m.Run();

            // --- Assert
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0xBA);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// IN L,(C): 0xED 0x68
        /// </summary>
        [TestMethod]
        public void IN_L_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x68 // IN L,(C)
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;

            // --- Act
            m.Run();

            // --- Assert

            regs.L.ShouldBe((byte)0xD5);
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0xD5);
            m.IoAccessLog[0].IsOutput.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, L");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// OUT (C),L: 0xED 0x69
        /// </summary>
        [TestMethod]
        public void OUT_C_L_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x69        // OUT (C),L
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;
            regs.HL = 0xBA98;

            // --- Act
            m.Run();

            // --- Assert
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0x98);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// IN (C): 0xED 0x70
        /// </summary>
        [TestMethod]
        public void IN_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x70 // IN (C)
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;

            // --- Act
            m.Run();

            // --- Assert

            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0xD5);
            m.IoAccessLog[0].IsOutput.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// OUT (C),0: 0xED 0x71
        /// </summary>
        [TestMethod]
        public void OUT_C_0_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x71        // OUT (C),0
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;

            // --- Act
            m.Run();

            // --- Assert
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0x00);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// IN A,(C): 0xED 0x78
        /// </summary>
        [TestMethod]
        public void IN_A_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x78 // IN A,(C)
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0xD5);
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0xD5);
            m.IoAccessLog[0].IsOutput.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "F, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

        /// <summary>
        /// OUT (C),A: 0xED 0x79
        /// </summary>
        [TestMethod]
        public void OUT_C_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x79        // OUT (C),A
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;
            regs.A = 0x98;

            // --- Act
            m.Run();

            // --- Assert
            m.IoAccessLog.Count.ShouldBe(1);
            m.IoAccessLog[0].Address.ShouldBe((ushort)0x1234);
            m.IoAccessLog[0].Value.ShouldBe((byte)0x98);
            m.IoAccessLog[0].IsOutput.ShouldBeTrue();

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(12ul);
        }

    }
}
