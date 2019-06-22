using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Cpu;
using Spect.Net.SpectrumEmu.Test.Helpers;

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.SpectrumEmu.Test.Cpu.IndexedOps
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
            m.Cpu.Tacts.ShouldBe(39L);
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
            m.Cpu.Tacts.ShouldBe(39L);
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
            m.Cpu.Tacts.ShouldBe(39L);
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
            m.Cpu.Tacts.ShouldBe(39L);
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
            m.Cpu.Tacts.ShouldBe(14L);
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
            m.Cpu.Tacts.ShouldBe(34L);
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
            m.Cpu.Tacts.ShouldBe(24L);
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

            m.ShouldKeepRegisters(except: "F, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(25L);
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
            m.Cpu.Tacts.ShouldBe(29L);
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
            m.Cpu.Tacts.ShouldBe(20L);
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
            m.Cpu.Tacts.ShouldBe(24L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(25L);
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
            m.Cpu.Tacts.ShouldBe(23L);
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
            m.Cpu.Tacts.ShouldBe(23L);
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
                0xDD, 0x36, 0x52, 0xD2  // LD (IX+52H),D2H
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
            m.Cpu.Tacts.ShouldBe(19L);
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
            m.Cpu.Tacts.ShouldBe(39L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(19L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(19L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(19L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// LD XH,B: 0xDD 0x60
        /// </summary>
        [TestMethod]
        public void LD_XH_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0x60 // LD XH,B
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAAAA;
            regs.B = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.XH.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// LD XH,C: 0xDD 0x61
        /// </summary>
        [TestMethod]
        public void LD_XH_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0x61 // LD XH,C
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAAAA;
            regs.C = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.XH.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// LD XH,D: 0xDD 0x62
        /// </summary>
        [TestMethod]
        public void LD_XH_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0x62 // LD XH,D
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAAAA;
            regs.D = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.XH.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// LD XH,E: 0xDD 0x63
        /// </summary>
        [TestMethod]
        public void LD_XH_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0x63 // LD XH,E
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAAAA;
            regs.E = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.XH.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// LD XH,XL: 0xDD 0x65
        /// </summary>
        [TestMethod]
        public void LD_XH_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0x65 // LD XH,XL
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAABB;

            // --- Act
            m.Run();

            // --- Assert
            regs.IX.ShouldBe((ushort)0xBBBB);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
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
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// LD XH,A: 0xDD 0x67
        /// </summary>
        [TestMethod]
        public void LD_XH_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0x67 // LD XH,A
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAAAA;
            regs.A = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.XH.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// LD XL_B: 0xDD 0x68
        /// </summary>
        [TestMethod]
        public void LD_XL_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0x68 // LD XL,B
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAAAA;
            regs.B = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.XL.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// LD XL_C: 0xDD 0x69
        /// </summary>
        [TestMethod]
        public void LD_XL_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0x69 // LD XL,C
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAAAA;
            regs.C = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.XL.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// LD XL_D: 0xDD 0x6A
        /// </summary>
        [TestMethod]
        public void LD_XL_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0x6A // LD XL,D
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAAAA;
            regs.D = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.XL.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// LD XL_E: 0xDD 0x6B
        /// </summary>
        [TestMethod]
        public void LD_XL_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0x6B // LD XL,E
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAAAA;
            regs.E = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.XL.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// LD XL_XH: 0xDD 0x6C
        /// </summary>
        [TestMethod]
        public void LD_XL_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0x6C // LD XL,XH
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAABB;

            // --- Act
            m.Run();

            // --- Assert
            regs.IX.ShouldBe((ushort)0xAAAA);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
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
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// LD XL_A: 0xDD 0x6F
        /// </summary>
        [TestMethod]
        public void LD_XL_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0x6F // LD XL,A
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAAAA;
            regs.A = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.XL.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// LD (IX+D),B: 0xDD 0x70
        /// </summary>
        [TestMethod]
        public void LD_IXi_B_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x70, 0x52  // LD (IX+52H),B
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.B = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IX + OFFS].ShouldBe((byte)0xA5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// LD (IX+D),C: 0xDD 0x71
        /// </summary>
        [TestMethod]
        public void LD_IXi_C_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x71, 0x52  // LD (IX+52H),C
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.C = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IX + OFFS].ShouldBe((byte)0xA5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// LD (IX+D),D: 0xDD 0x72
        /// </summary>
        [TestMethod]
        public void LD_IXi_D_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x72, 0x52  // LD (IX+52H),D
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.D = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IX + OFFS].ShouldBe((byte)0xA5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// LD (IX+D),E: 0xDD 0x73
        /// </summary>
        [TestMethod]
        public void LD_IXi_E_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x73, 0x52  // LD (IX+52H),E
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.E = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IX + OFFS].ShouldBe((byte)0xA5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// LD (IX+D),H: 0xDD 0x74
        /// </summary>
        [TestMethod]
        public void LD_IXi_H_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x74, 0x52  // LD (IX+52H),H
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.H = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IX + OFFS].ShouldBe((byte)0xA5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// LD (IX+D),L: 0xDD 0x75
        /// </summary>
        [TestMethod]
        public void LD_IXi_L_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x75, 0x52  // LD (IX+52H),L
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.L = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IX + OFFS].ShouldBe((byte)0xA5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// LD (IX+D),A: 0xDD 0x77
        /// </summary>
        [TestMethod]
        public void LD_IXi_A_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x77, 0x52  // LD (IX+52H),A
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            regs.A = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IX + OFFS].ShouldBe((byte)0xA5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(22L);
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
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// ADD A,XH: 0xDD 0x84
        /// </summary>
        [TestMethod]
        public void ADD_A_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12,             // LD A,12H
                0xDD, 0x21, 0x24, 0x3D, // LD IX,3D24H
                0xDD, 0x84              // ADD A,XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x4F);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(29L);
        }

        /// <summary>
        /// ADD A,XL: 0xDD 0x85
        /// </summary>
        [TestMethod]
        public void ADD_A_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12,             // LD A,12H
                0xDD, 0x21, 0x24, 0x3D, // LD IX,3D24H
                0xDD, 0x85              // ADD A,XL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x36);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(29L);
        }

        /// <summary>
        /// ADD A,(IX+D): 0xDD 0x86
        /// </summary>
        [TestMethod]
        public void ADD_A_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12,      // LD A,12H
                0xDD, 0x86, 0x54 // ADD A,(IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0x24;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x36);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// ADC A,XH: 0xDD 0x8C
        /// </summary>
        [TestMethod]
        public void ADC_A_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xF0, // LD A,F0H
                0x37,       // SCF
                0xDD, 0x8C  // ADC A,XH
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xF0AA;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0xE1);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// ADC A,XL: 0xDD 0x8D
        /// </summary>
        [TestMethod]
        public void ADC_A_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xF0, // LD A,F0H
                0x37,       // SCF
                0xDD, 0x8D  // ADC A,XL
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAAF0;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0xE1);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// ADC A,(IX+D): 0xDD 0x8E
        /// </summary>
        [TestMethod]
        public void ADC_A_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xF0,       // LD A,F0H
                0x37,             // SCF
                0xDD, 0x8E, 0x54  // ADC A,(IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0xF0;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0xE1);
            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(30L);
        }

        /// <summary>
        /// SUB XH: 0xDD 0x94
        /// </summary>
        [TestMethod]
        public void SUB_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36,             // LD A,36H
                0xDD, 0x21, 0x3D, 0x24, // LD IX,243DH
                0xDD, 0x94              // SUB XH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x12);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(29L);
        }

        /// <summary>
        /// SUB XL: 0xDD 0x95
        /// </summary>
        [TestMethod]
        public void SUB_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36,             // LD A,36H
                0xDD, 0x21, 0x24, 0x3D, // LD IX,3D24H
                0xDD, 0x95              // SUB XL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.A.ShouldBe((byte)0x12);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(29L);
        }

        /// <summary>
        /// SUB (IX+D): 0xDD 0x96
        /// </summary>
        [TestMethod]
        public void SUB_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36,       // LD A,36H
                0x37,             // SCF
                0xDD, 0x96, 0x54  // SUB (IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0x24;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x12);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(30L);
        }

        /// <summary>
        /// SBC XH: 0xDD 0x9C
        /// </summary>
        [TestMethod]
        public void SBC_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36,             // LD A,36H
                0xDD, 0x21, 0x3D, 0x24, // LD IX,243DH
                0xDD, 0x9C              // SBC XH
            });
            var regs = m.Cpu.Registers;
            regs.F |= FlagsSetMask.C;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x11);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(29L);
        }

        /// <summary>
        /// SBC XL: 0xDD 0x9D
        /// </summary>
        [TestMethod]
        public void SBC_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36,             // LD A,36H
                0xDD, 0x21, 0x24, 0x3D, // LD IX,3D24H
                0xDD, 0x9D              // SBC XL
            });
            var regs = m.Cpu.Registers;
            regs.F |= FlagsSetMask.C;

            // --- Act
            m.Run();

            // --- Assert
            regs.A.ShouldBe((byte)0x11);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF, IX");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(29L);
        }

        /// <summary>
        /// SBC (IX+D): 0xDD 0x9E
        /// </summary>
        [TestMethod]
        public void SBC_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36,       // LD A,36H
                0x37,             // SCF
                0xDD, 0x9E, 0x54  // SBC (IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.F |= FlagsSetMask.C;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0x24;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x11);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(30L);
        }

        /// <summary>
        /// AND XH: 0xDD 0xA4
        /// </summary>
        [TestMethod]
        public void AND_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xDD, 0xA4  // AND XH
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x23AA;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x02);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// AND XL: 0xDD 0xA5
        /// </summary>
        [TestMethod]
        public void AND_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xDD, 0xA5  // AND XL
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAA23;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x02);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// AND (IX+D): 0xDD 0xA6
        /// </summary>
        [TestMethod]
        public void AND_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12,       // LD A,12H
                0xDD, 0xA6, 0x54  // AND (IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0x23;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x02);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// XOR XH: 0xDD 0xAC
        /// </summary>
        [TestMethod]
        public void XOR_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xDD, 0xAC  // XOR XH
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x23AA;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x31);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// XOR XL: 0xDD 0xAD
        /// </summary>
        [TestMethod]
        public void XOR_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xDD, 0xAD  // XOR XL
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAA23;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x31);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// XOR (IX+D): 0xDD 0xAE
        /// </summary>
        [TestMethod]
        public void XOR_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12,       // LD A,12H
                0xDD, 0xAE, 0x54  // XOR (IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0x23;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x31);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// OR XH: 0xDD 0xB4
        /// </summary>
        [TestMethod]
        public void OR_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xDD, 0xB4  // OR XH
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x23AA;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x33);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// OR XL: 0xDD 0xB5
        /// </summary>
        [TestMethod]
        public void OR_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xDD, 0xB5  // OR XL
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0xAA23;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x33);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(15L);
        }

        /// <summary>
        /// OR (IX+D): 0xDD 0xB6
        /// </summary>
        [TestMethod]
        public void OR_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12,       // LD A,12H
                0xDD, 0xB6, 0x54  // OR (IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0x23;

            // --- Act
            m.Run();

            // --- Assert

            regs.A.ShouldBe((byte)0x33);
            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeTrue();

            regs.HFlag.ShouldBeFalse();
            regs.NFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// CP XH: 0xDD 0xBC
        /// </summary>
        [TestMethod]
        public void CP_XH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xBC  // CP XH
            });
            var regs = m.Cpu.Registers;
            regs.A = 0x36;
            regs.IX = 0x24AA;

            // --- Act
            m.Run();

            // --- Assert

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// CP XL: 0xDD 0xBD
        /// </summary>
        [TestMethod]
        public void CP_XL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xBD  // CP XL
            });
            var regs = m.Cpu.Registers;
            regs.A = 0x36;
            regs.IX = 0xAA24;

            // --- Act
            m.Run();

            // --- Assert

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8L);
        }

        /// <summary>
        /// CP (IX+D): 0xDD 0xBE
        /// </summary>
        [TestMethod]
        public void CP_IXi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xDD, 0xBE, 0x54 // CP (IX+54H)
            });
            var regs = m.Cpu.Registers;
            regs.A = 0x36;
            regs.IX = 0x1000;
            m.Memory[regs.IX + OFFS] = 0x24;

            // --- Act
            m.Run();

            // --- Assert

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeTrue();
            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19L);
        }

        /// <summary>
        /// POP IX: 0xDD 0xE1
        /// </summary>
        [TestMethod]
        public void POP_IX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x52, 0x23, // LD HL,2352H
                0xE5,             // PUSH HL
                0xDD, 0xE1        // POP IX
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.IX.ShouldBe((ushort)0x2352);
            m.ShouldKeepRegisters(except: "HL, IX");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(35L);
        }

        /// <summary>
        /// EX (SP),IX: 0xDD 0xE3
        /// </summary>
        [TestMethod]
        public void EX_SPi_IX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x31, 0x00, 0x10,       // LD SP, 1000H
                0xDD, 0x21, 0x34, 0x12, // LD IX,1234H
                0xDD, 0xE3              // EX (SP),IX
            });
            m.Memory[0x1000] = 0x78;
            m.Memory[0x1001] = 0x56;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.IX.ShouldBe((ushort)0x5678);
            m.Memory[0x1000].ShouldBe((byte)0x34);
            m.Memory[0x1001].ShouldBe((byte)0x12);

            m.ShouldKeepRegisters(except: "SP, IX");
            m.ShouldKeepMemory(except: "1000-1001");

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Tacts.ShouldBe(47L);
        }

        /// <summary>
        /// PUSH IX: 0xDD 0xE5
        /// </summary>
        [TestMethod]
        public void PUSH_IX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x52, 0x23, // LD IX,2352H
                0xDD, 0xE5,             // PUSH IX
                0xC1                    // POP BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.BC.ShouldBe((ushort)0x2352);
            m.ShouldKeepRegisters(except: "IX, BC");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(39L);
        }

        /// <summary>
        /// JP (IX): 0xDD 0xE9
        /// </summary>
        [TestMethod]
        public void JP_IXi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x00, 0x10, // LD IX, 1000H
                0xDD, 0xE9              // JP (IX)
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.PC.ShouldBe((ushort)0x1000);

            m.ShouldKeepRegisters(except: "IX");
            m.ShouldKeepMemory();

            m.Cpu.Tacts.ShouldBe(22L);
        }

        /// <summary>
        /// LD SP,IX: 0xDD 0xF9
        /// </summary>
        [TestMethod]
        public void LD_SP_IX_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xDD, 0x21, 0x00, 0x10, // LD IX,1000H
                0xDD, 0xF9              // LD SP,IX
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SP.ShouldBe((ushort)0x1000);
            m.ShouldKeepRegisters(except: "IX, SP");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(24L);
        }

    }
}
