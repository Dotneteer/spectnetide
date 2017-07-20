using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Test.Helpers;
using Z80TestMachine = Spect.Net.SpectrumEmu.Test.Helpers.Z80TestMachine;

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.SpectrumEmu.Test.Cpu.StandardOps
{
    [TestClass]
    public class StandardOpTests0X30
    {
        /// <summary>
        /// JR NC,E: 0x30
        /// </summary>
        [TestMethod]
        public void JR_NC_E_WorksWithNoJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x37,       // SCF 
                0x30, 0x02  // JR NC,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11ul);
        }

        /// <summary>
        /// JR NC,E: 0x30
        /// </summary>
        [TestMethod]
        public void JR_NC_E_WorksWithJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x37,       // SCF
                0x3F,       // CCF 
                0x30, 0x02  // JR NC,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(20ul);
        }

        /// <summary>
        /// LD SP,NN: 0x31
        /// </summary>
        [TestMethod]
        public void LD_SP_NN_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x31, 0x26, 0xA9 // LD SP,A926H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory();

            regs.SP.ShouldBe((ushort)0xA926);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(10ul);
        }

        /// <summary>
        /// LD (NN),A: 0x32
        /// </summary>
        [TestMethod]
        public void LD_NNi_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xA9,       // LD A,A9H
                0x32, 0x00, 0x10  // LD (1000H),A
            });

            // --- Act
            var before = m.Memory[0x1000];
            m.Run();
            var after = m.Memory[0x1000];
            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A");
            m.ShouldKeepMemory(except: "1000");

            before.ShouldBe((byte)0x00);
            after.ShouldBe((byte)0xA9);

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(20ul);
        }

        /// <summary>
        /// INC SP: 0x33
        /// </summary>
        [TestMethod]
        public void INC_SP_WorksAsExpected1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x31, 0x26, 0xA9, // LD SP,A926H
                0x33              // INC SP
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory();

            regs.SP.ShouldBe((ushort)0xA927);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(16ul);
        }

        /// <summary>
        /// INC (HL): 0x34
        /// </summary>
        [TestMethod]
        public void INC_HLi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x00, 0x10, // LD HL,1000H
                0x34              // INC (HL)
            });
            m.Memory[0x1000] = 0x23;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "HL, F");
            m.ShouldKeepMemory(except: "1000");
            m.Memory[0x1000].ShouldBe((byte)0x24);

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(21ul);
        }

        /// <summary>
        /// DEC (HL): 0x35
        /// </summary>
        [TestMethod]
        public void DEC_HLi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x00, 0x10, // LD HL,1000H
                0x35              // DEC (HL)
            });
            m.Memory[0x1000] = 0x23;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "HL, F");
            m.ShouldKeepMemory(except: "1000");
            m.Memory[0x1000].ShouldBe((byte)0x22);

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(21ul);
        }

        /// <summary>
        /// LD (HL),N: 0x36
        /// </summary>
        [TestMethod]
        public void LD_HLi_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x00, 0x10, // LD HL,1000H
                0x36, 0x56        // LD (HL),56H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "HL, F");
            m.ShouldKeepMemory(except: "1000");
            m.Memory[0x1000].ShouldBe((byte)0x56);

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(20ul);
        }

        /// <summary>
        /// SCF: 0x37
        /// </summary>
        [TestMethod]
        public void SCF_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x37 // SCF
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F");
            regs.CFlag.ShouldBeTrue();
            regs.PC.ShouldBe((ushort)0x0001);
            m.Cpu.Tacts.ShouldBe(4ul);
        }

        /// <summary>
        /// JR C,E: 0x38
        /// </summary>
        [TestMethod]
        public void JR_C_E_WorksWithNoJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x37,       // SCF
                0x3F,       // CCF 
                0x38, 0x02  // JR C,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15ul);
        }

        /// <summary>
        /// JR C,E: 0x38
        /// </summary>
        [TestMethod]
        public void JR_C_E_WorksWithJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x37,       // SCF 
                0x38, 0x02  // JR C,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(16ul);
        }

        /// <summary>
        /// ADD HL,SP: 0x39
        /// </summary>
        [TestMethod]
        public void ADD_HL_SP_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x34, 0x12, // LD HL,1234H
                0x31, 0x45, 0x23, // LD SP,2345H
                0x39              // ADD HL,SP
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F, HL, SP");
            m.ShouldKeepMemory();

            regs.HL.ShouldBe((ushort)0x3579);
            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(31ul);
        }

        /// <summary>
        /// LD A,(NN): 0x3A
        /// </summary>
        [TestMethod]
        public void LD_A_NNi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3A, 0x00, 0x10 // LD A,(1000H)
            });
            m.Memory[0x1000] = 0x34;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A");
            m.ShouldKeepMemory();

            regs.A.ShouldBe((byte)0x34);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(13ul);
        }

        /// <summary>
        /// DEC SP: 0x3B
        /// </summary>
        [TestMethod]
        public void DEC_SP_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x31, 0x26, 0xA9, // LD SP,A926H
                0x3B              // DEC SP
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "SP");
            m.ShouldKeepMemory();

            regs.SP.ShouldBe((ushort)0xA925);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(16ul);
        }

        /// <summary>
        /// INC A: 0x3C
        /// </summary>
        [TestMethod]
        public void INC_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x43, // LD L,43H
                0x3C        // INC A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeFalse();

            regs.A.ShouldBe((byte)0x44);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11ul);
        }

        /// <summary>
        /// DEC A: 0x3D
        /// </summary>
        [TestMethod]
        public void DEC_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x43, // LD A,43H
                0x3D        // DEC A
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeTrue();

            regs.A.ShouldBe((byte)0x42);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11ul);
        }

        /// <summary>
        /// LD A,N: 0x3E
        /// </summary>
        [TestMethod]
        public void LD_L_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x26 // LD A,26H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A");
            m.ShouldKeepMemory();

            regs.A.ShouldBe((byte)0x26);
            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(7ul);
        }

        /// <summary>
        /// CCF: 0x3F
        /// </summary>
        [TestMethod]
        public void CPL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x37, // SCF
                0x3F  // CCF
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();
            regs.CFlag.ShouldBeFalse();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8ul);
        }
    }
}