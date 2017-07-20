using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Cpu;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Cpu.IndexedOps
{
    [TestClass]
    public class IyIndexedOpsTests
    {
        /// <summary>
        /// ADD IY,BC: 0xFD 0x09
        /// </summary>
        [TestMethod]
        public void ADD_IY_BC_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x01, 0x11, // LD IY,1101H
                0x01, 0x34, 0x12,       // LD BC,1234H
                0xFD, 0x09              // ADD IY,BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x2335);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IY, BC, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Tacts.ShouldBe(39ul);
        }

        /// <summary>
        /// ADD IY,BC: 0xFD 0x09
        /// </summary>
        [TestMethod]
        public void ADD_IY_BC_SetsCarry()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x01, 0xF0, // LD IY,F001H
                0x01, 0x34, 0x12,       // LD BC,1234H
                0xFD, 0x09              // ADD IY,BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x0235);

            regs.CFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IY, BC, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Tacts.ShouldBe(39ul);
        }

        /// <summary>
        /// ADD IY,BC: 0xFD 0x09
        /// </summary>
        [TestMethod]
        public void ADD_IY_BC_SetsHFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x01, 0x0F, // LD IY,0F01H
                0x01, 0x34, 0x12,       // LD BC,1234H
                0xFD, 0x09              // ADD IY,BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x2135);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IY, BC, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Tacts.ShouldBe(39ul);
        }

        /// <summary>
        /// ADD IY,DE: 0xFD 0x19
        /// </summary>
        [TestMethod]
        public void ADD_IY_DE_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x01, 0x11, // LD IY,1101H
                0x11, 0x34, 0x12,       // LD DE,1234H
                0xFD, 0x19              // ADD IY,DE
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x2335);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IY, DE, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Tacts.ShouldBe(39ul);
        }

        /// <summary>
        /// LD IY,NN: 0xFD 0x21
        /// </summary>
        [TestMethod]
        public void LD_IY_NN_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x01, 0x11 // LD IY,1101H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x1101);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(14ul);
        }

        /// <summary>
        /// LD (NN),IY: 0xFD 0x22
        /// </summary>
        [TestMethod]
        public void LD_NNi_IY_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x01, 0x11, // LD IY,1101H
                0xFD, 0x22, 0x00, 0x10  // LD (1000H),IY
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x1101);
            m.Memory[0x1000].ShouldBe(regs.YL);
            m.Memory[0x1001].ShouldBe(regs.YH);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory(except: "1000-1001");

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(34ul);
        }

        /// <summary>
        /// INC IY: 0xFD 0x23
        /// </summary>
        [TestMethod]
        public void INC_IY_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0x12, // LD IY,1234H
                0xFD, 0x23              // INC IY
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x1235);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(24ul);
        }

        /// <summary>
        /// INC YH: 0xFD 0x24
        /// </summary>
        [TestMethod]
        public void INC_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0x12, // LD IY,1234H
                0xFD, 0x24              // INC YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x1334);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// INC YH: 0xFD 0x24
        /// </summary>
        [TestMethod]
        public void INC_YH_SetsSFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0xFE, // LD IY,FE34H
                0xFD, 0x24              // INC YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0xFF34);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// INC YH: 0xFD 0x24
        /// </summary>
        [TestMethod]
        public void INC_YH_SetsHFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0x4F, // LD IY,4F34H
                0xFD, 0x24              // INC YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x5034);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// INC YH: 0xFD 0x24
        /// </summary>
        [TestMethod]
        public void INC_YH_SetsPFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0x7F, // LD IY,7F34H
                0xFD, 0x24              // INC YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x8034);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// INC YH: 0xFD 0x24
        /// </summary>
        [TestMethod]
        public void INC_YH_SetsZFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0xFF, // LD IY,FF34H
                0xFD, 0x24              // INC YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x0034);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// DEC YH: 0xFD 0x25
        /// </summary>
        [TestMethod]
        public void DEC_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0x12, // LD IY,1234H
                0xFD, 0x25              // DEC YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x1134);

            m.ShouldKeepRegisters(except: "F, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// DEC YH: 0xFD 0x25
        /// </summary>
        [TestMethod]
        public void DEC_YH_SetsSFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0x85, // LD IY,8534H
                0xFD, 0x25              // DEC YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x8434);

            regs.SFlag.ShouldBeTrue();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// DEC YH: 0xFD 0x25
        /// </summary>
        [TestMethod]
        public void DEC_YH_SetsHFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0x40, // LD IY,4034H
                0xFD, 0x25              // DEC YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x3F34);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// DEC YH: 0xFD 0x25
        /// </summary>
        [TestMethod]
        public void DEC_YH_SetsPFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0x80, // LD IY,8034H
                0xFD, 0x25              // INC YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x7F34);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeTrue();
            regs.PFlag.ShouldBeTrue();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// DEC YH: 0xFD 0x25
        /// </summary>
        [TestMethod]
        public void DEC_YH_SetsZFlag()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0x01, // LD IY,0134H
                0xFD, 0x25              // DEC YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x0034);

            regs.SFlag.ShouldBeFalse();
            regs.ZFlag.ShouldBeTrue();
            regs.HFlag.ShouldBeFalse();
            regs.PFlag.ShouldBeFalse();
            regs.CFlag.ShouldBeFalse();

            m.ShouldKeepRegisters(except: "F, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// LD YH,N: 0xFD 0x26
        /// </summary>
        [TestMethod]
        public void LD_YH_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0x12, // LD IY,1234H
                0xFD, 0x26, 0x2D        // LD YH,2DH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x2D34);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(25ul);
        }

        /// <summary>
        /// ADD IY,IY: 0xFD 0x29
        /// </summary>
        [TestMethod]
        public void ADD_IY_IY_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x01, 0x11, // LD IY,1101H
                0xFD, 0x29              // ADD IY,IY
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x2202);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IY, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(29ul);
        }

        /// <summary>
        /// LD IY,(NN): 0xFD 0x2A
        /// </summary>
        [TestMethod]
        public void LD_IY_NNi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x2A, 0x00, 0x10  // LD IY,(1000H)
            });
            m.Memory[0x1000] = 0x34;
            m.Memory[0x1001] = 0x12;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x1234);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory(except: "1000-1001");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(20ul);
        }

        /// <summary>
        /// DEC IY: 0xFD 0x2B
        /// </summary>
        [TestMethod]
        public void DEC_IY_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0x12, // LD IY,1234H
                0xFD, 0x2B             // DEC IY
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x1233);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(24ul);
        }

        /// <summary>
        /// INC YL: 0xFD 0x2C
        /// </summary>
        [TestMethod]
        public void INC_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0x12, // LD IY,1234H
                0xFD, 0x2C              // INC YL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x1235);

            m.ShouldKeepRegisters(except: "IY, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// DEC YL: 0xFD 0x2D
        /// </summary>
        [TestMethod]
        public void DEC_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0x12, // LD IY,1234H
                0xFD, 0x2D              // DEC YL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x1233);

            m.ShouldKeepRegisters(except: "IY, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// LD YL,N: 0xFD 0x2E
        /// </summary>
        [TestMethod]
        public void LD_YL_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x34, 0x12, // LD IY,1234H
                0xFD, 0x2E, 0x2D        // LD YL,2DH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x122D);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(25ul);
        }

        /// <summary>
        /// INC (IY+D): 0xFD 0x34
        /// </summary>
        [TestMethod]
        public void INC_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x34, 0x52  // INC (IY+52H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0xA6);

            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(23ul);
        }

        /// <summary>
        /// INC (IY+D): 0xFD 0x35
        /// </summary>
        [TestMethod]
        public void DEC_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x35, 0x52  // DEC (IY+52H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0xA4);

            m.ShouldKeepRegisters(except: "F");
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(23ul);
        }

        /// <summary>
        /// LD (IY+D),N: 0xFD 0x36
        /// </summary>
        [TestMethod]
        public void LD_IYi_N_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x36, 0x52, 0xD2  // DEC (IY+52H),D2H
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0xD2);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// ADD IY,SP: 0xFD 0x39
        /// </summary>
        [TestMethod]
        public void ADD_IY_SP_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x01, 0x11, // LD IY,1101H
                0x31, 0x34, 0x12,       // LD SP,1234H
                0xFD, 0x39              // ADD IY,SP
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.IY.ShouldBe((ushort)0x2335);

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.NFlag.ShouldBeFalse();
            m.ShouldKeepRegisters(except: "IY, SP, F");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Tacts.ShouldBe(39ul);
        }

        /// <summary>
        /// LD B,YH: 0xFD 0x44
        /// </summary>
        [TestMethod]
        public void LD_B_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x9A, 0x78, // LD IY,789AH
                0xFD, 0x44              // LD B,YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.B.ShouldBe((byte)0x78);

            m.ShouldKeepRegisters(except: "IY, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// LD B,YL: 0xFD 0x45
        /// </summary>
        [TestMethod]
        public void LD_B_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x9A, 0x78, // LD IY,789AH
                0xFD, 0x45              // LD B,YL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.B.ShouldBe((byte)0x9A);

            m.ShouldKeepRegisters(except: "IY, B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// LD B,(IY+D): 0xFD 0x46
        /// </summary>
        [TestMethod]
        public void LD_B_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x46, 0x54  // LD B,(IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x7C;

            // --- Act
            m.Run();

            // --- Assert
            regs.B.ShouldBe((byte)0x7C);

            m.ShouldKeepRegisters(except: "B");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// LD C,YH: 0xFD 0x4C
        /// </summary>
        [TestMethod]
        public void LD_C_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x9A, 0x78, // LD IY,789AH
                0xFD, 0x4C              // LD C,YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.C.ShouldBe((byte)0x78);

            m.ShouldKeepRegisters(except: "IY, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// LD C,YL: 0xFD 0x4D
        /// </summary>
        [TestMethod]
        public void LD_C_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x9A, 0x78, // LD IY,789AH
                0xFD, 0x4D              // LD C,YL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.C.ShouldBe((byte)0x9A);

            m.ShouldKeepRegisters(except: "IY, C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// LD C,(IY+D): 0xFD 0x4E
        /// </summary>
        [TestMethod]
        public void LD_C_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x4E, 0x54  // LD C,(IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x7C;

            // --- Act
            m.Run();

            // --- Assert
            regs.C.ShouldBe((byte)0x7C);

            m.ShouldKeepRegisters(except: "C");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// LD D,YH: 0xFD 0x54
        /// </summary>
        [TestMethod]
        public void LD_D_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x9A, 0x78, // LD IY,789AH
                0xFD, 0x54              // LD D,YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.D.ShouldBe((byte)0x78);

            m.ShouldKeepRegisters(except: "IY, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// LD D,YL: 0xFD 0x55
        /// </summary>
        [TestMethod]
        public void LD_D_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x9A, 0x78, // LD IY,789AH
                0xFD, 0x55              // LD D,YL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.D.ShouldBe((byte)0x9A);

            m.ShouldKeepRegisters(except: "IY, D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// LD D,(IY+D): 0xFD 0x56
        /// </summary>
        [TestMethod]
        public void LD_D_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x56, 0x54  // LD D,(IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x7C;

            // --- Act
            m.Run();

            // --- Assert
            regs.D.ShouldBe((byte)0x7C);

            m.ShouldKeepRegisters(except: "D");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// LD E,YH: 0xFD 0x5C
        /// </summary>
        [TestMethod]
        public void LD_E_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x9A, 0x78, // LD IY,789AH
                0xFD, 0x5C              // LD E,YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.E.ShouldBe((byte)0x78);

            m.ShouldKeepRegisters(except: "IY, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// LD E,YL: 0xFD 0x5D
        /// </summary>
        [TestMethod]
        public void LD_E_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x9A, 0x78, // LD IY,789AH
                0xFD, 0x5D              // LD E,YL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.E.ShouldBe((byte)0x9A);

            m.ShouldKeepRegisters(except: "IY, E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// LD E,(IY+D): 0xFD 0x5E
        /// </summary>
        [TestMethod]
        public void LD_E_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x5E, 0x54  // LD E,(IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x7C;

            // --- Act
            m.Run();

            // --- Assert
            regs.E.ShouldBe((byte)0x7C);

            m.ShouldKeepRegisters(except: "E");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// LD YH,B: 0xFD 0x60
        /// </summary>
        [TestMethod]
        public void LD_YH_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0x60 // LD YH,B
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAAAA;
            regs.B = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.YH.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8ul);
        }

        /// <summary>
        /// LD YH,C: 0xFD 0x61
        /// </summary>
        [TestMethod]
        public void LD_YH_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0x61 // LD YH,C
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAAAA;
            regs.C = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.YH.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8ul);
        }

        /// <summary>
        /// LD YH,D: 0xFD 0x62
        /// </summary>
        [TestMethod]
        public void LD_YH_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0x62 // LD YH,D
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAAAA;
            regs.D = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.YH.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8ul);
        }

        /// <summary>
        /// LD YH,E: 0xFD 0x63
        /// </summary>
        [TestMethod]
        public void LD_YH_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0x63 // LD YH,E
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAAAA;
            regs.E = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.YH.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8ul);
        }

        /// <summary>
        /// LD YH,YL: 0xFD 0x65
        /// </summary>
        [TestMethod]
        public void LD_YH_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0x65 // LD YH,YL
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAABB;

            // --- Act
            m.Run();

            // --- Assert
            regs.IY.ShouldBe((ushort)0xBBBB);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8ul);
        }

        /// <summary>
        /// LD H,(IY+D): 0xFD 0x66
        /// </summary>
        [TestMethod]
        public void LD_H_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x66, 0x54  // LD H,(IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x7C;

            // --- Act
            m.Run();

            // --- Assert
            regs.H.ShouldBe((byte)0x7C);

            m.ShouldKeepRegisters(except: "H");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// LD YH,A: 0xFD 0x67
        /// </summary>
        [TestMethod]
        public void LD_YH_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0x67 // LD YH,A
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAAAA;
            regs.A = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.YH.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8ul);
        }

        /// <summary>
        /// LD YL_B: 0xFD 0x68
        /// </summary>
        [TestMethod]
        public void LD_YL_B_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0x68 // LD YL,B
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAAAA;
            regs.B = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.YL.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8ul);
        }

        /// <summary>
        /// LD YL_C: 0xFD 0x69
        /// </summary>
        [TestMethod]
        public void LD_YL_C_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0x69 // LD YL,C
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAAAA;
            regs.C = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.YL.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8ul);
        }

        /// <summary>
        /// LD YL_D: 0xFD 0x6A
        /// </summary>
        [TestMethod]
        public void LD_YL_D_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0x6A // LD YL,D
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAAAA;
            regs.D = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.YL.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8ul);
        }

        /// <summary>
        /// LD YL_E: 0xFD 0x6B
        /// </summary>
        [TestMethod]
        public void LD_YL_E_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0x6B // LD YL,E
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAAAA;
            regs.E = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.YL.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8ul);
        }

        /// <summary>
        /// LD YL_YH: 0xFD 0x6C
        /// </summary>
        [TestMethod]
        public void LD_YL_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0x6C // LD YL,YH
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAABB;

            // --- Act
            m.Run();

            // --- Assert
            regs.IY.ShouldBe((ushort)0xAAAA);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8ul);
        }

        /// <summary>
        /// LD L,(IY+D): 0xFD 0x6E
        /// </summary>
        [TestMethod]
        public void LD_L_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x6E, 0x54  // LD L,(IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x7C;

            // --- Act
            m.Run();

            // --- Assert
            regs.L.ShouldBe((byte)0x7C);

            m.ShouldKeepRegisters(except: "L");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// LD YL_A: 0xFD 0x6F
        /// </summary>
        [TestMethod]
        public void LD_YL_A_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0x6F // LD YL,A
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAAAA;
            regs.A = 0x55;

            // --- Act
            m.Run();

            // --- Assert
            regs.YL.ShouldBe((byte)0x55);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(8ul);
        }

        /// <summary>
        /// LD (IY+D),B: 0xFD 0x70
        /// </summary>
        [TestMethod]
        public void LD_IYi_B_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x70, 0x52  // LD (IY+52H),B
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.B = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0xA5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// LD (IY+D),C: 0xFD 0x71
        /// </summary>
        [TestMethod]
        public void LD_IYi_C_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x71, 0x52  // LD (IY+52H),C
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.C = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0xA5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// LD (IY+D),D: 0xFD 0x72
        /// </summary>
        [TestMethod]
        public void LD_IYi_D_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x72, 0x52  // LD (IY+52H),D
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.D = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0xA5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// LD (IY+D),E: 0xFD 0x73
        /// </summary>
        [TestMethod]
        public void LD_IYi_E_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x73, 0x52  // LD (IY+52H),E
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.E = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0xA5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// LD (IY+D),H: 0xFD 0x74
        /// </summary>
        [TestMethod]
        public void LD_IYi_H_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x74, 0x52  // LD (IY+52H),H
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.H = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0xA5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// LD (IY+D),L: 0xFD 0x75
        /// </summary>
        [TestMethod]
        public void LD_IYi_L_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x75, 0x52  // LD (IY+52H),L
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.L = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0xA5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// LD (IY+D),A: 0xFD 0x77
        /// </summary>
        [TestMethod]
        public void LD_IYi_A_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x52;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x77, 0x52  // LD (IY+52H),A
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            regs.A = 0xA5;

            // --- Act
            m.Run();

            // --- Assert
            m.Memory[regs.IY + OFFS].ShouldBe((byte)0xA5);

            m.ShouldKeepRegisters();
            m.ShouldKeepMemory(except: "1052");

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// LD A,YH: 0xFD 0x7C
        /// </summary>
        [TestMethod]
        public void LD_A_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x9A, 0x78, // LD IY,789AH
                0xFD, 0x7C              // LD A,YH
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.A.ShouldBe((byte)0x78);

            m.ShouldKeepRegisters(except: "IY, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// LD A,YL: 0xFD 0x7D
        /// </summary>
        [TestMethod]
        public void LD_A_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x9A, 0x78, // LD IY,789AH
                0xFD, 0x7D              // LD A,YL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;
            regs.A.ShouldBe((byte)0x9A);

            m.ShouldKeepRegisters(except: "IY, A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// LD A,(IY+D): 0xFD 0x7E
        /// </summary>
        [TestMethod]
        public void LD_A_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x7E, 0x54  // LD A,(IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x7C;

            // --- Act
            m.Run();

            // --- Assert
            regs.A.ShouldBe((byte)0x7C);

            m.ShouldKeepRegisters(except: "A");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// ADD A,YH: 0xFD 0x84
        /// </summary>
        [TestMethod]
        public void ADD_A_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12,             // LD A,12H
                0xFD, 0x21, 0x24, 0x3D, // LD IY,3D24H
                0xFD, 0x84              // ADD A,YH
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
            m.ShouldKeepRegisters(except: "AF, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(29ul);
        }

        /// <summary>
        /// ADD A,YL: 0xFD 0x85
        /// </summary>
        [TestMethod]
        public void ADD_A_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12,             // LD A,12H
                0xFD, 0x21, 0x24, 0x3D, // LD IY,3D24H
                0xFD, 0x85              // ADD A,YL
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
            m.ShouldKeepRegisters(except: "AF, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(29ul);
        }

        /// <summary>
        /// ADD A,(IY+D): 0xFD 0x86
        /// </summary>
        [TestMethod]
        public void ADD_A_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12,      // LD A,12H
                0xFD, 0x86, 0x54 // ADD A,(IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x24;

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
            m.ShouldKeepRegisters(except: "AF, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(26ul);
        }

        /// <summary>
        /// ADC A,YH: 0xFD 0x8C
        /// </summary>
        [TestMethod]
        public void ADC_A_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xF0, // LD A,F0H
                0x37,       // SCF
                0xFD, 0x8C  // ADC A,YH
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xF0AA;

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
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// ADC A,YL: 0xFD 0x8D
        /// </summary>
        [TestMethod]
        public void ADC_A_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xF0, // LD A,F0H
                0x37,       // SCF
                0xFD, 0x8D  // ADC A,YL
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAAF0;

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
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// ADC A,(IY+D): 0xFD 0x8E
        /// </summary>
        [TestMethod]
        public void ADC_A_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xF0,       // LD A,F0H
                0x37,             // SCF
                0xFD, 0x8E, 0x54  // ADC A,(IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0xF0;

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
            m.Cpu.Tacts.ShouldBe(30ul);
        }

        /// <summary>
        /// SUB YH: 0xFD 0x94
        /// </summary>
        [TestMethod]
        public void SUB_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36,             // LD A,36H
                0xFD, 0x21, 0x3D, 0x24, // LD IY,243DH
                0xFD, 0x94              // SUB YH
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
            m.ShouldKeepRegisters(except: "AF, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(29ul);
        }

        /// <summary>
        /// SUB YL: 0xFD 0x95
        /// </summary>
        [TestMethod]
        public void SUB_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36,             // LD A,36H
                0xFD, 0x21, 0x24, 0x3D, // LD IY,3D24H
                0xFD, 0x95              // SUB YL
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
            m.ShouldKeepRegisters(except: "AF, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(29ul);
        }

        /// <summary>
        /// SUB (IY+D): 0xFD 0x96
        /// </summary>
        [TestMethod]
        public void SUB_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36,       // LD A,36H
                0x37,             // SCF
                0xFD, 0x96, 0x54  // SUB (IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x24;

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
            m.Cpu.Tacts.ShouldBe(30ul);
        }

        /// <summary>
        /// SBC YH: 0xFD 0x9C
        /// </summary>
        [TestMethod]
        public void SBC_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36,             // LD A,36H
                0xFD, 0x21, 0x3D, 0x24, // LD IY,243DH
                0xFD, 0x9C              // SBC YH
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
            m.ShouldKeepRegisters(except: "AF, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(29ul);
        }

        /// <summary>
        /// SBC YL: 0xFD 0x9D
        /// </summary>
        [TestMethod]
        public void SBC_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36,             // LD A,36H
                0xFD, 0x21, 0x24, 0x3D, // LD IY,3D24H
                0xFD, 0x9D              // SBC YL
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
            m.ShouldKeepRegisters(except: "AF, IY");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0008);
            m.Cpu.Tacts.ShouldBe(29ul);
        }

        /// <summary>
        /// SBC (IY+D): 0xFD 0x9E
        /// </summary>
        [TestMethod]
        public void SBC_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x36,       // LD A,36H
                0x37,             // SCF
                0xFD, 0x9E, 0x54  // SBC (IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.F |= FlagsSetMask.C;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x24;

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
            m.Cpu.Tacts.ShouldBe(30ul);
        }

        /// <summary>
        /// AND YH: 0xFD 0xA4
        /// </summary>
        [TestMethod]
        public void AND_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xFD, 0xA4  // AND YH
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x23AA;

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
            m.Cpu.Tacts.ShouldBe(15ul);
        }

        /// <summary>
        /// AND YL: 0xFD 0xA5
        /// </summary>
        [TestMethod]
        public void AND_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xFD, 0xA5  // AND YL
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAA23;

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
            m.Cpu.Tacts.ShouldBe(15ul);
        }

        /// <summary>
        /// AND (IY+D): 0xFD 0xA6
        /// </summary>
        [TestMethod]
        public void AND_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12,       // LD A,12H
                0xFD, 0xA6, 0x54  // AND (IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x23;

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
            m.Cpu.Tacts.ShouldBe(26ul);
        }

        /// <summary>
        /// XOR YH: 0xFD 0xAC
        /// </summary>
        [TestMethod]
        public void XOR_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xFD, 0xAC  // XOR YH
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x23AA;

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
            m.Cpu.Tacts.ShouldBe(15ul);
        }

        /// <summary>
        /// XOR YL: 0xFD 0xAD
        /// </summary>
        [TestMethod]
        public void XOR_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xFD, 0xAD  // XOR YL
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAA23;

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
            m.Cpu.Tacts.ShouldBe(15ul);
        }

        /// <summary>
        /// XOR (IY+D): 0xFD 0xAE
        /// </summary>
        [TestMethod]
        public void XOR_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12,       // LD A,12H
                0xFD, 0xAE, 0x54  // XOR (IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x23;

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
            m.Cpu.Tacts.ShouldBe(26ul);
        }

        /// <summary>
        /// OR YH: 0xFD 0xB4
        /// </summary>
        [TestMethod]
        public void OR_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xFD, 0xB4  // OR YH
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x23AA;

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
            m.Cpu.Tacts.ShouldBe(15ul);
        }

        /// <summary>
        /// OR YL: 0xFD 0xB5
        /// </summary>
        [TestMethod]
        public void OR_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xFD, 0xB5  // OR YL
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0xAA23;

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
            m.Cpu.Tacts.ShouldBe(15ul);
        }

        /// <summary>
        /// OR (IY+D): 0xFD 0xB6
        /// </summary>
        [TestMethod]
        public void OR_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x12,       // LD A,12H
                0xFD, 0xB6, 0x54  // OR (IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x23;

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
            m.Cpu.Tacts.ShouldBe(26ul);
        }

        /// <summary>
        /// CP YH: 0xFD 0xBC
        /// </summary>
        [TestMethod]
        public void CP_YH_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xB8  // CP YH
            });
            var regs = m.Cpu.Registers;
            regs.A = 0x36;
            regs.IY = 0x24AA;

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
            m.Cpu.Tacts.ShouldBe(8ul);
        }

        /// <summary>
        /// CP YL: 0xFD 0xBD
        /// </summary>
        [TestMethod]
        public void CP_YL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xBD  // CP YL
            });
            var regs = m.Cpu.Registers;
            regs.A = 0x36;
            regs.IY = 0xAA24;

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
            m.Cpu.Tacts.ShouldBe(8ul);
        }

        /// <summary>
        /// CP (IY+D): 0xFD 0xBE
        /// </summary>
        [TestMethod]
        public void CP_IYi_WorksAsExpected()
        {
            // --- Arrange
            const byte OFFS = 0x54;
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0xFD, 0xBE, 0x54 // CP (IY+54H)
            });
            var regs = m.Cpu.Registers;
            regs.A = 0x36;
            regs.IY = 0x1000;
            m.Memory[regs.IY + OFFS] = 0x24;

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
            m.Cpu.Tacts.ShouldBe(19ul);
        }

        /// <summary>
        /// POP IY: 0xFD 0xE1
        /// </summary>
        [TestMethod]
        public void POP_IY_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x52, 0x23, // LD HL,2352H
                0xE5,             // PUSH HL
                0xFD, 0xE1        // POP IY
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.IY.ShouldBe((ushort)0x2352);
            m.ShouldKeepRegisters(except: "HL, IY");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(35ul);
        }

        /// <summary>
        /// EX (SP),IY: 0xFD 0xE3
        /// </summary>
        [TestMethod]
        public void EX_SPi_IY_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x31, 0x00, 0x10,       // LD SP, 1000H
                0xFD, 0x21, 0x34, 0x12, // LD IY,1234H
                0xFD, 0xE3              // EX (SP),IY
            });
            m.Memory[0x1000] = 0x78;
            m.Memory[0x1001] = 0x56;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.IY.ShouldBe((ushort)0x5678);
            m.Memory[0x1000].ShouldBe((byte)0x34);
            m.Memory[0x1001].ShouldBe((byte)0x12);

            m.ShouldKeepRegisters(except: "SP, IY");
            m.ShouldKeepMemory(except: "1000-1001");

            regs.PC.ShouldBe((ushort)0x0009);
            m.Cpu.Tacts.ShouldBe(47ul);
        }

        /// <summary>
        /// PUSH IY: 0xFD 0xE5
        /// </summary>
        [TestMethod]
        public void PUSH_IY_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x52, 0x23, // LD IY,2352H
                0xFD, 0xE5,             // PUSH IY
                0xC1                    // POP BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.BC.ShouldBe((ushort)0x2352);
            m.ShouldKeepRegisters(except: "IY, BC");
            m.ShouldKeepMemory(except: "FFFE-FFFF");

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(39ul);
        }

        /// <summary>
        /// JP (IY): 0xFD 0xE9
        /// </summary>
        [TestMethod]
        public void JP_IYi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x00, 0x10, // LD IY, 1000H
                0xFD, 0xE9              // JP (IY)
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.PC.ShouldBe((ushort)0x1000);

            m.ShouldKeepRegisters(except: "IY");
            m.ShouldKeepMemory();

            m.Cpu.Tacts.ShouldBe(22ul);
        }

        /// <summary>
        /// LD SP,IY: 0xFD 0xF9
        /// </summary>
        [TestMethod]
        public void LD_SP_IY_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xFD, 0x21, 0x00, 0x10, // LD IY,1000H
                0xFD, 0xF9              // LD SP,IY
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            regs.SP.ShouldBe((ushort)0x1000);
            m.ShouldKeepRegisters(except: "IY, SP");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(24ul);
        }

    }
}