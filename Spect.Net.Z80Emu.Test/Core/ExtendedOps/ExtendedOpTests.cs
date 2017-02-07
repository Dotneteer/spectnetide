using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Core;
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
        /// SBC HL,BC: 0xED 0x42
        /// </summary>
        [TestMethod]
        public void SBC_HL_BC_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x42        // SBC HL,BC
            });
            var regs = m.Cpu.Registers;
            regs.HL = 0x3456;
            regs.BC = 0x1234;

            // --- Act
            m.Run();

            // --- Assert
            regs.HL.ShouldBe((ushort)0x2222);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "HL, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(15ul);
        }

        /// <summary>
        /// SBC HL,BC: 0xED 0x42
        /// </summary>
        [TestMethod]
        public void SBC_HL_BC_SetsCarry()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x42        // SBC HL,BC
            });
            var regs = m.Cpu.Registers;
            regs.HL = 0x1234;
            regs.BC = 0x3456;

            // --- Act
            m.Run();

            // --- Assert
            regs.HL.ShouldBe((ushort)0xDDDE);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "HL, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(15ul);
        }

        /// <summary>
        /// SBC HL,BC: 0xED 0x42
        /// </summary>
        [TestMethod]
        public void SBC_HL_BC_SetsZeroFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x42        // SBC HL,BC
            });
            var regs = m.Cpu.Registers;
            regs.HL = 0x1234;
            regs.BC = 0x1234;

            // --- Act
            m.Run();

            // --- Assert
            regs.HL.ShouldBe((ushort)0x0000);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "HL, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(15ul);
        }

        /// <summary>
        /// SBC HL,BC: 0xED 0x42
        /// </summary>
        [TestMethod]
        public void SBC_HL_BC_UsesCarry()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x42        // SBC HL,BC
            });
            var regs = m.Cpu.Registers;
            regs.HL = 0x3456;
            regs.BC = 0x1234;
            regs.F |= FlagsSetMask.C;

            // --- Act
            m.Run();

            // --- Assert
            regs.HL.ShouldBe((ushort)0x2221);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "HL, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(15ul);
        }

        /// <summary>
        /// LD (NN),BC: 0xED 0x43
        /// </summary>
        [TestMethod]
        public void LD_NNi_BC_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x43, 0x00, 0x10 // LD (1000H),BC
            });
            var regs = m.Cpu.Registers;
            regs.BC = 0x1234;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[0x1000].ShouldBe((byte)0x34);
            m.Memory[0x1001].ShouldBe((byte)0x12);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1000-1001");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(20ul);
        }

        /// <summary>
        /// NEG: 0xED 0x44
        /// </summary>
        [TestMethod]
        public void NEG_WorksAsExpected1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x44 // NEG
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.A = 0x03;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0xFD);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
        }

        /// <summary>
        /// NEG: 0xED 0x44
        /// </summary>
        [TestMethod]
        public void NEG_SetsZero()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x44 // NEG
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.A = 0x00;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x00);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
        }

        /// <summary>
        /// NEG: 0xED 0x44
        /// </summary>
        [TestMethod]
        public void NEG_SetsPFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x44 // NEG
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.A = 0xFE;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x02);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeTrue();

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
        }

        /// <summary>
        /// NEG: 0xED 0x44
        /// </summary>
        [TestMethod]
        public void NEG_ResetsHFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x44 // NEG
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.A = 0xD0;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x30);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeTrue();

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
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
        /// RETN: 0xED 0x45
        /// </summary>
        [TestMethod]
        public void RETN_WorksWithDI1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xED, 0x45        // RETN
            });
            m.Cpu.IFF1 = false;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IFF1.ShouldBe(m.Cpu.IFF2);

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(42ul);
        }

        /// <summary>
        /// RETN: 0xED 0x45
        /// </summary>
        [TestMethod]
        public void RETN_WorksWithEI()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xED, 0x45        // RETN
            });
            m.Cpu.IFF1 = true;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IFF1.ShouldBe(m.Cpu.IFF2);

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(42ul);
        }

        /// <summary>
        /// IM 0: 0xED 0x46
        /// </summary>
        [TestMethod]
        public void IM_0_WorksAsExpected1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x46 // IM 0
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IM.ShouldBe((byte)0);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
        }

        /// <summary>
        /// LD I,A: 0xED 0x47
        /// </summary>
        [TestMethod]
        public void LD_I_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x47 // LD I,A
            });
            var regs = m.Cpu.Registers;
            regs.A = 0xD5;

            // --- Act
            m.Run();

            // --- Assert
            regs.I.ShouldBe((byte)0xD5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(9ul);
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
        /// NEG: 0xED 0x4C
        /// </summary>
        [TestMethod]
        public void NEG_WorksAsExpected2()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x4C // NEG
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.A = 0x03;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0xFD);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
        }

        /// <summary>
        /// RETI: 0xED 0x4D
        /// </summary>
        [TestMethod]
        public void RETI_WorksWithAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xED, 0x4D        // RETI
            });
            m.Cpu.IFF1 = false;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IFF1.ShouldBe(m.Cpu.IFF2);

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(42ul);
        }

        /// <summary>
        /// IM 0: 0xED 0x4E
        /// </summary>
        [TestMethod]
        public void IM_0_WorksAsExpected2()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x4E // IM 0
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IM.ShouldBe((byte)0);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
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
        /// SBC HL,DE: 0xED 0x52
        /// </summary>
        [TestMethod]
        public void SBC_HL_DE_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x52        // SBC HL,DE
            });
            var regs = m.Cpu.Registers;
            regs.HL = 0x3456;
            regs.DE = 0x1234;
            regs.F |= FlagsSetMask.C;

            // --- Act
            m.Run();

            // --- Assert
            regs.HL.ShouldBe((ushort)0x2221);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "HL, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(15ul);
        }

        /// <summary>
        /// LD (NN),BC: 0xED 0x53
        /// </summary>
        [TestMethod]
        public void LD_NNi_DE_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x53, 0x00, 0x10 // LD (1000H),DE
            });
            var regs = m.Cpu.Registers;
            regs.DE = 0x1234;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[0x1000].ShouldBe((byte)0x34);
            m.Memory[0x1001].ShouldBe((byte)0x12);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1000-1001");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(20ul);
        }

        /// <summary>
        /// NEG: 0xED 0x54
        /// </summary>
        [TestMethod]
        public void NEG_WorksAsExpected3()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x54 // NEG
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.A = 0x03;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0xFD);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
        }

        /// <summary>
        /// RETN: 0xED 0x55
        /// </summary>
        [TestMethod]
        public void RETN_WorksWithDI2()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xED, 0x55        // RETN
            });
            m.Cpu.IFF1 = false;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IFF1.ShouldBe(m.Cpu.IFF2);

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(42ul);
        }

        /// <summary>
        /// IM 1: 0xED 0x56
        /// </summary>
        [TestMethod]
        public void IM_1_WorksAsExpected1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x56 // IM 1
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IM.ShouldBe((byte)1);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
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
        /// NEG: 0xED 0x5C
        /// </summary>
        [TestMethod]
        public void NEG_WorksAsExpected4()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x5C // NEG
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.A = 0x03;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0xFD);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
        }

        /// <summary>
        /// RETN: 0xED 0x5D
        /// </summary>
        [TestMethod]
        public void RETN_WorksWithDI3()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xED, 0x5D        // RETN
            });
            m.Cpu.IFF1 = false;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IFF1.ShouldBe(m.Cpu.IFF2);

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(42ul);
        }

        /// <summary>
        /// IM 2: 0xED 0x5E
        /// </summary>
        [TestMethod]
        public void IM_2_WorksAsExpected1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x5E // IM 2
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IM.ShouldBe((byte)2);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
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
        /// SBC HL,DE: 0xED 0x62
        /// </summary>
        [TestMethod]
        public void SBC_HL_HL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x62        // SBC HL,HL
            });
            var regs = m.Cpu.Registers;
            regs.HL = 0x3456;
            regs.F |= FlagsSetMask.C;

            // --- Act
            m.Run();

            // --- Assert
            regs.HL.ShouldBe((ushort)0xFFFF);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "HL, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(15ul);
        }

        /// <summary>
        /// LD (NN),BC: 0xED 0x63
        /// </summary>
        [TestMethod]
        public void LD_NNi_HL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x63, 0x00, 0x10 // LD (1000H),HL
            });
            var regs = m.Cpu.Registers;
            regs.HL = 0x1234;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[0x1000].ShouldBe((byte)0x34);
            m.Memory[0x1001].ShouldBe((byte)0x12);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1000-1001");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(20ul);
        }

        /// <summary>
        /// NEG: 0xED 0x64
        /// </summary>
        [TestMethod]
        public void NEG_WorksAsExpected5()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x64 // NEG
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.A = 0x03;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0xFD);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
        }

        /// <summary>
        /// RETN: 0xED 0x65
        /// </summary>
        [TestMethod]
        public void RETN_WorksWithDI4()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xED, 0x65        // RETN
            });
            m.Cpu.IFF1 = false;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IFF1.ShouldBe(m.Cpu.IFF2);

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(42ul);
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
        /// NEG: 0xED 0x6C
        /// </summary>
        [TestMethod]
        public void NEG_WorksAsExpected6()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x6C // NEG
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.A = 0x03;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0xFD);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
        }

        /// <summary>
        /// RETN: 0xED 0x6D
        /// </summary>
        [TestMethod]
        public void RETN_WorksWithDI5()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xED, 0x6D        // RETN
            });
            m.Cpu.IFF1 = false;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IFF1.ShouldBe(m.Cpu.IFF2);

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(42ul);
        }

        /// <summary>
        /// IM 0: 0xED 0x6E
        /// </summary>
        [TestMethod]
        public void IM_0_WorksAsExpected3()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x6E // IM 0
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IM.ShouldBe((byte)0);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
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
        /// SBC HL,SP: 0xED 0x72
        /// </summary>
        [TestMethod]
        public void SBC_HL_SP_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x72        // SBC HL,SP
            });
            var regs = m.Cpu.Registers;
            regs.HL = 0x3456;
            regs.SP = 0x1234;
            regs.F |= FlagsSetMask.C;

            // --- Act
            m.Run();

            // --- Assert
            regs.HL.ShouldBe((ushort)0x2221);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "HL, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(15ul);
        }

        /// <summary>
        /// LD (NN),BC: 0xED 0x73
        /// </summary>
        [TestMethod]
        public void LD_NNi_SP_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x73, 0x00, 0x10 // LD (1000H),SP
            });
            var regs = m.Cpu.Registers;
            regs.SP = 0x1234;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[0x1000].ShouldBe((byte)0x34);
            m.Memory[0x1001].ShouldBe((byte)0x12);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1000-1001");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(20ul);
        }

        /// <summary>
        /// NEG: 0xED 0x74
        /// </summary>
        [TestMethod]
        public void NEG_WorksAsExpected7()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x74 // NEG
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.A = 0x03;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0xFD);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
        }

        /// <summary>
        /// RETN: 0xED 0x75
        /// </summary>
        [TestMethod]
        public void RETN_WorksWithDI6()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xED, 0x75        // RETN
            });
            m.Cpu.IFF1 = false;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IFF1.ShouldBe(m.Cpu.IFF2);

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(42ul);
        }

        /// <summary>
        /// IM 1: 0xED 0x76
        /// </summary>
        [TestMethod]
        public void IM_1_WorksAsExpected2()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x76 // IM 1
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IM.ShouldBe((byte)1);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
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

        /// <summary>
        /// NEG: 0xED 0x7C
        /// </summary>
        [TestMethod]
        public void NEG_WorksAsExpected8()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x7C // NEG
            });
            m.IoInputSequence.Add(0xD5);
            var regs = m.Cpu.Registers;
            regs.A = 0x03;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0xFD);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
        }

        /// <summary>
        /// RETN: 0xED 0x7D
        /// </summary>
        [TestMethod]
        public void RETN_WorksWithDI7()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xED, 0x7D        // RETN
            });
            m.Cpu.IFF1 = false;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IFF1.ShouldBe(m.Cpu.IFF2);

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(42ul);
        }

        /// <summary>
        /// IM 2: 0xED 0x7E
        /// </summary>
        [TestMethod]
        public void IM_2_WorksAsExpected2()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xED, 0x7E // IM 2
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            m.Cpu.IM.ShouldBe((byte)2);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Ticks.ShouldBe(8ul);
        }


    }
}
