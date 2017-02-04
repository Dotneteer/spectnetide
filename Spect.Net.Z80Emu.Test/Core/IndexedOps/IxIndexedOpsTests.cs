using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core.IndexedOps
{
    [TestClass]
    public class IxIndexedOpsTests
    {
        /// <summary>
        /// ADD IX,BC: 0xDD 0x09
        /// </summary>
        [TestMethod]
        public void ADD_IX_BC_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x01, 0x11, // LD IX,1101H
                0x01, 0x34, 0x12,       // LD BC,1234H
                0xDD, 0x09              // ADD IX,BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x2335);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IX, BC, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Ticks.ShouldBe(39ul);
        }

        /// <summary>
        /// ADD IX,BC: 0xDD 0x09
        /// </summary>
        [TestMethod]
        public void ADD_IX_BC_SetsCarry()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x01, 0xF0, // LD IX,F001H
                0x01, 0x34, 0x12,       // LD BC,1234H
                0xDD, 0x09              // ADD IX,BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x0235);

            regs.CFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IX, BC, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Ticks.ShouldBe(39ul);
        }

        /// <summary>
        /// ADD IX,BC: 0xDD 0x09
        /// </summary>
        [TestMethod]
        public void ADD_IX_BC_SetsHFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x01, 0x0F, // LD IX,0F01H
                0x01, 0x34, 0x12,       // LD BC,1234H
                0xDD, 0x09              // ADD IX,BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x2135);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IX, BC, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Ticks.ShouldBe(39ul);
        }

        /// <summary>
        /// ADD IX,DE: 0xDD 0x19
        /// </summary>
        [TestMethod]
        public void ADD_IX_DE_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x01, 0x11, // LD IX,1101H
                0x11, 0x34, 0x12,       // LD DE,1234H
                0xDD, 0x19              // ADD IX,DE
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x2335);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IX, DE, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Ticks.ShouldBe(39ul);
        }

        /// <summary>
        /// LD IX,NN: 0xDD 0x21
        /// </summary>
        [TestMethod]
        public void LD_IX_NN_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x01, 0x11 // LD IX,1101H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x1101);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(14ul);
        }

        /// <summary>
        /// LD (NN),IX: 0xDD 0x22
        /// </summary>
        [TestMethod]
        public void LD_NNi_IX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x01, 0x11, // LD IX,1101H
                0xDD, 0x22, 0x00, 0x10  // LD (1000H),IX
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x1101);
            m.Memory[0x1000].ShouldBe(regs.XL);
            m.Memory[0x1001].ShouldBe(regs.XH);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory(except: "1000-1001");

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Ticks.ShouldBe(34ul);
        }

        /// <summary>
        /// INC IX: 0xDD 0x23
        /// </summary>
        [TestMethod]
        public void INC_IX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0x12, // LD IX,1234H
                0xDD, 0x23              // INC IX
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x1235);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(24ul);
        }

        /// <summary>
        /// INC XH: 0xDD 0x24
        /// </summary>
        [TestMethod]
        public void INC_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0x12, // LD IX,1234H
                0xDD, 0x24              // INC XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x1334);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// INC XH: 0xDD 0x24
        /// </summary>
        [TestMethod]
        public void INC_XH_SetsSFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0xFE, // LD IX,FE34H
                0xDD, 0x24              // INC XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0xFF34);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// INC XH: 0xDD 0x24
        /// </summary>
        [TestMethod]
        public void INC_XH_SetsHFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0x4F, // LD IX,4F34H
                0xDD, 0x24              // INC XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x5034);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// INC XH: 0xDD 0x24
        /// </summary>
        [TestMethod]
        public void INC_XH_SetsPFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0x7F, // LD IX,7F34H
                0xDD, 0x24              // INC XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x8034);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// INC XH: 0xDD 0x24
        /// </summary>
        [TestMethod]
        public void INC_XH_SetsZFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0xFF, // LD IX,FF34H
                0xDD, 0x24              // INC XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x0034);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// DEC XH: 0xDD 0x25
        /// </summary>
        [TestMethod]
        public void DEC_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0x12, // LD IX,1234H
                0xDD, 0x25              // DEC XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x1134);

            m.ShouldKeepRegisters(except: "F, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// DEC XH: 0xDD 0x25
        /// </summary>
        [TestMethod]
        public void DEC_XH_SetsSFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0x85, // LD IX,8534H
                0xDD, 0x25              // DEC XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x8434);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// DEC XH: 0xDD 0x25
        /// </summary>
        [TestMethod]
        public void DEC_XH_SetsHFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0x40, // LD IX,4034H
                0xDD, 0x25              // DEC XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x3F34);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// DEC XH: 0xDD 0x25
        /// </summary>
        [TestMethod]
        public void DEC_XH_SetsPFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0x80, // LD IX,8034H
                0xDD, 0x25              // INC XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x7F34);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// DEC XH: 0xDD 0x25
        /// </summary>
        [TestMethod]
        public void DEC_XH_SetsZFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0x01, // LD IX,0134H
                0xDD, 0x25              // DEC XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x0034);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// LD XH,N: 0xDD 0x26
        /// </summary>
        [TestMethod]
        public void LD_XH_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0x12, // LD IX,1234H
                0xDD, 0x26, 0x2D        // LD XH,2DH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x2D34);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Ticks.ShouldBe(25ul);
        }

        /// <summary>
        /// ADD IX,IX: 0xDD 0x29
        /// </summary>
        [TestMethod]
        public void ADD_IX_IX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x01, 0x11, // LD IX,1101H
                0xDD, 0x29              // ADD IX,IX
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x2202);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IX, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(29ul);
        }

        /// <summary>
        /// LD IX,(NN): 0xDD 0x2A
        /// </summary>
        [TestMethod]
        public void LD_IX_NNi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x2A, 0x00, 0x10  // LD IX,(1000H)
            });
            m.Memory[0x1000] = 0x34;
            m.Memory[0x1001] = 0x12;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x1234);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory(except: "1000-1001");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(20ul);
        }

        /// <summary>
        /// DEC IX: 0xDD 0x2B
        /// </summary>
        [TestMethod]
        public void DEC_IX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0x12, // LD IX,1234H
                0xDD, 0x2B             // DEC IX
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x1233);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(24ul);
        }

        /// <summary>
        /// INC XL: 0xDD 0x2C
        /// </summary>
        [TestMethod]
        public void INC_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0x12, // LD IX,1234H
                0xDD, 0x2C              // INC XL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x1235);

            m.ShouldKeepRegisters(except: "IX, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// DEC XL: 0xDD 0x2D
        /// </summary>
        [TestMethod]
        public void DEC_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0x12, // LD IX,1234H
                0xDD, 0x2D              // DEC XL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x1233);

            m.ShouldKeepRegisters(except: "IX, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// LD XL,N: 0xDD 0x2E
        /// </summary>
        [TestMethod]
        public void LD_XL_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x34, 0x12, // LD IX,1234H
                0xDD, 0x2E, 0x2D        // LD XH,2DH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x122D);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Ticks.ShouldBe(25ul);
        }

        /// <summary>
        /// INC (IX+D): 0xDD 0x34
        /// </summary>
        [TestMethod]
        public void INC_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x34, 0x52  // INC (IX+52H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IX + OFFS].ShouldBe((byte)0xA6);

            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// INC (IX+D): 0xDD 0x35
        /// </summary>
        [TestMethod]
        public void DEC_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x35, 0x52  // DEC (IX+52H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IX + OFFS].ShouldBe((byte)0xA4);

            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(23ul);
        }

        /// <summary>
        /// LD (IX+D),N: 0xDD 0x36
        /// </summary>
        [TestMethod]
        public void LD_IXi_N_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x36, 0x52, 0xD2  // DEC (IX+52H),D2H
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IX + OFFS].ShouldBe((byte)0xD2);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Ticks.ShouldBe(19ul);
        }

        /// <summary>
        /// ADD IX,SP: 0xDD 0x39
        /// </summary>
        [TestMethod]
        public void ADD_IX_SP_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x01, 0x11, // LD IX,1101H
                0x31, 0x34, 0x12,       // LD SP,1234H
                0xDD, 0x39              // ADD IX,SP
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IX.ShouldBe((ushort)0x2335);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IX, SP, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Ticks.ShouldBe(39ul);
        }

        /// <summary>
        /// LD B,XH: 0xDD 0x44
        /// </summary>
        [TestMethod]
        public void LD_B_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x9A, 0x78, // LD IX,789AH
                0xDD, 0x44              // LD B,XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.B.ShouldBe((byte)0x78);

            m.ShouldKeepRegisters(except: "IX, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// LD B,XL: 0xDD 0x45
        /// </summary>
        [TestMethod]
        public void LD_B_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x9A, 0x78, // LD IX,789AH
                0xDD, 0x45              // LD B,XL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.B.ShouldBe((byte)0x9A);

            m.ShouldKeepRegisters(except: "IX, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// LD B,(IX+D): 0xDD 0x46
        /// </summary>
        [TestMethod]
        public void LD_B_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x46, 0x54  // LD B,(IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0x7C;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe((byte)0x7C);

            m.ShouldKeepRegisters(except: "B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(19ul);
        }

        /// <summary>
        /// LD C,XH: 0xDD 0x4C
        /// </summary>
        [TestMethod]
        public void LD_C_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x9A, 0x78, // LD IX,789AH
                0xDD, 0x4C              // LD C,XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.C.ShouldBe((byte)0x78);

            m.ShouldKeepRegisters(except: "IX, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// LD C,XL: 0xDD 0x4D
        /// </summary>
        [TestMethod]
        public void LD_C_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x9A, 0x78, // LD IX,789AH
                0xDD, 0x4D              // LD C,XL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.C.ShouldBe((byte)0x9A);

            m.ShouldKeepRegisters(except: "IX, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// LD C,(IX+D): 0xDD 0x4E
        /// </summary>
        [TestMethod]
        public void LD_C_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x4E, 0x54  // LD C,(IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0x7C;

            // --- Act
            m.Run();

            // --- Assert
            regs.C.ShouldBe((byte)0x7C);

            m.ShouldKeepRegisters(except: "C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(19ul);
        }

        /// <summary>
        /// LD D,XH: 0xDD 0x54
        /// </summary>
        [TestMethod]
        public void LD_D_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x9A, 0x78, // LD IX,789AH
                0xDD, 0x54              // LD D,XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.D.ShouldBe((byte)0x78);

            m.ShouldKeepRegisters(except: "IX, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// LD D,XL: 0xDD 0x55
        /// </summary>
        [TestMethod]
        public void LD_D_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x9A, 0x78, // LD IX,789AH
                0xDD, 0x55              // LD D,XL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.D.ShouldBe((byte)0x9A);

            m.ShouldKeepRegisters(except: "IX, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// LD D,(IX+D): 0xDD 0x56
        /// </summary>
        [TestMethod]
        public void LD_D_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x56, 0x54  // LD D,(IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0x7C;

            // --- Act
            m.Run();

            // --- Assert
            regs.D.ShouldBe((byte)0x7C);

            m.ShouldKeepRegisters(except: "D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(19ul);
        }

        /// <summary>
        /// LD E,XH: 0xDD 0x5C
        /// </summary>
        [TestMethod]
        public void LD_E_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x9A, 0x78, // LD IX,789AH
                0xDD, 0x5C              // LD E,XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.E.ShouldBe((byte)0x78);

            m.ShouldKeepRegisters(except: "IX, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// LD E,XL: 0xDD 0x5D
        /// </summary>
        [TestMethod]
        public void LD_E_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x9A, 0x78, // LD IX,789AH
                0xDD, 0x5D              // LD E,XL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.E.ShouldBe((byte)0x9A);

            m.ShouldKeepRegisters(except: "IX, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// LD E,(IX+D): 0xDD 0x5E
        /// </summary>
        [TestMethod]
        public void LD_E_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x5E, 0x54  // LD E,(IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0x7C;

            // --- Act
            m.Run();

            // --- Assert
            regs.E.ShouldBe((byte)0x7C);

            m.ShouldKeepRegisters(except: "E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(19ul);
        }

        /// <summary>
        /// LD H,(IX+D): 0xDD 0x66
        /// </summary>
        [TestMethod]
        public void LD_H_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x66, 0x54  // LD H,(IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0x7C;

            // --- Act
            m.Run();

            // --- Assert
            regs.H.ShouldBe((byte)0x7C);

            m.ShouldKeepRegisters(except: "H");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(19ul);
        }

        /// <summary>
        /// LD L,(IX+D): 0xDD 0x6E
        /// </summary>
        [TestMethod]
        public void LD_L_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x6E, 0x54  // LD L,(IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0x7C;

            // --- Act
            m.Run();

            // --- Assert
            regs.L.ShouldBe((byte)0x7C);

            m.ShouldKeepRegisters(except: "L");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(19ul);
        }

        /// <summary>
        /// LD A,XH: 0xDD 0x7C
        /// </summary>
        [TestMethod]
        public void LD_A_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x9A, 0x78, // LD IX,789AH
                0xDD, 0x7C              // LD A,XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.A.ShouldBe((byte)0x78);

            m.ShouldKeepRegisters(except: "IX, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// LD A,XL: 0xDD 0x7D
        /// </summary>
        [TestMethod]
        public void LD_A_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x9A, 0x78, // LD IX,789AH
                0xDD, 0x7D              // LD A,XL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.A.ShouldBe((byte)0x9A);

            m.ShouldKeepRegisters(except: "IX, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Ticks.ShouldBe(22ul);
        }

        /// <summary>
        /// LD A,(IX+D): 0xDD 0x7E
        /// </summary>
        [TestMethod]
        public void LD_A_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x7E, 0x54  // LD A,(IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0x7C;

            // --- Act
            m.Run();

            // --- Assert
            regs.A.ShouldBe((byte)0x7C);

            m.ShouldKeepRegisters(except: "A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Ticks.ShouldBe(19ul);
        }

    }
}
