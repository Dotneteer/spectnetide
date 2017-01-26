using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core
{
    [TestClass]
    public class StandardOpTests0X00
    {
        /// <summary>
        /// NOP: 0x00
        /// </summary>
        [TestMethod]
        public void NopWorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x00, // NOP
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0001);
            m.Cpu.Ticks.ShouldBe(4ul);
        }

        /// <summary>
        /// LD BC,NN: 0x01
        /// </summary>
        [TestMethod]
        public void LD_BC_NN_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x01, 0x26, 0xA9 // LD BC,A926H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "BC");
            m.ShouldKeepMemory();

            regs.BC.ShouldBe((ushort)0xA926);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(10ul);
        }

        /// <summary>
        /// LD (BC),A: 0x02
        /// </summary>
        [TestMethod]
        public void LD_BCi_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x01, 0x26, 0xA9, // LD BC,A926H
                0x3E, 0x94,       // LD A,94H
                0x02,             // LD (BC),A
                0x76              // HALT
            });

            // --- Act
            var valueBefore = m.Memory[0xA926];
            m.Run();
            var valueAfter = m.Memory[0xA926];

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "BC, A");
            m.ShouldKeepMemory(except: "A926");

            regs.BC.ShouldBe((ushort)0xA926);
            regs.A.ShouldBe((byte)0x94);
            valueBefore.ShouldBe((byte)0);
            valueAfter.ShouldBe((byte)0x94);
            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Ticks.ShouldBe(28ul);
        }

        /// <summary>
        /// INC BC: 0x03
        /// </summary>
        [TestMethod]
        public void INC_BC_WorksAsExpected1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x01, 0x26, 0xA9, // LD BC,A926H
                0x03              // INC BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "BC");
            m.ShouldKeepMemory();

            regs.BC.ShouldBe((ushort)0xA927);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(16ul);
        }

        /// <summary>
        /// INC BC: 0x03
        /// </summary>
        [TestMethod]
        public void INC_BC_WorksAsExpected2()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x01, 0xFF, 0xFF, // LD BC,FFFFH
                0x03              // INC BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "BC");
            m.ShouldKeepMemory();

            regs.BC.ShouldBe((ushort)0x0000);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(16ul);
        }

        /// <summary>
        /// INC B: 0x04
        /// </summary>
        [TestMethod]
        [Ignore]
        public void INC_B_WorksAsExpected1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x43, // LD B,43H
                0x04        // INC B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B");
            m.ShouldKeepMemory();

            regs.B.ShouldBe((byte)0x44);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }

        /// <summary>
        /// INC B: 0x04
        /// </summary>
        [TestMethod]
        [Ignore]
        public void INC_B_WorksAsExpected2()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0xFF, // LD B,FFH
                0x04        // INC B
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "B");
            m.ShouldKeepMemory();

            regs.B.ShouldBe((byte)0x00);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(11ul);
        }
    }
}
