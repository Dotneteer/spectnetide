using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
using Z80TestMachine = Spect.Net.Z80Emu.Test.Helpers.Z80TestMachine;

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core.BitOps
{
    [TestClass]
    public class BitOpTestsBitN
    {
        /// <summary>
        /// BIT N,B: 0xCB 0x40/0x48/0x50/0x58/0x60/0x68/0x70/0x78
        /// </summary>
        [TestMethod]
        public void BIT_N_B_WorksWithBitReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x40 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,B
                });

                var regs = m.Cpu.Registers;
                regs.B = (byte)~(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBeFalse();
                regs.ZFlag.ShouldBeTrue();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeTrue();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// BIT N,B: 0xCB 0x40/0x48/0x50/0x58/0x60/0x68/0x70/0x78
        /// </summary>
        [TestMethod]
        public void BIT_N_B_WorksWithBitSet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x40 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,B
                });

                var regs = m.Cpu.Registers;
                regs.B = (byte)(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBe(n == 0x07);
                regs.ZFlag.ShouldBeFalse();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeFalse();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// BIT N,C: 0xCB 0x41/0x49/0x51/0x59/0x61/0x69/0x71/0x79
        /// </summary>
        [TestMethod]
        public void BIT_N_C_WorksWithBitReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x41 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,C
                });

                var regs = m.Cpu.Registers;
                regs.C = (byte)~(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBeFalse();
                regs.ZFlag.ShouldBeTrue();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeTrue();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// BIT N,C: 0xCB 0x41/0x49/0x51/0x59/0x61/0x69/0x71/0x79
        /// </summary>
        [TestMethod]
        public void BIT_N_C_WorksWithBitSet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x41 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,C
                });

                var regs = m.Cpu.Registers;
                regs.C = (byte)(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBe(n == 0x07);
                regs.ZFlag.ShouldBeFalse();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeFalse();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// BIT N,D: 0xCB 0x42/0x4A/0x52/0x5A/0x62/0x6A/0x72/0x7A
        /// </summary>
        [TestMethod]
        public void BIT_N_D_WorksWithBitReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x42 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,D
                });

                var regs = m.Cpu.Registers;
                regs.D = (byte)~(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBeFalse();
                regs.ZFlag.ShouldBeTrue();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeTrue();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// BIT N,D: 0xCB 0x42/0x4A/0x52/0x5A/0x62/0x6A/0x72/0x7A
        /// </summary>
        [TestMethod]
        public void BIT_N_D_WorksWithBitSet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x42 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,D
                });

                var regs = m.Cpu.Registers;
                regs.D = (byte)(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBe(n == 0x07);
                regs.ZFlag.ShouldBeFalse();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeFalse();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// BIT N,E: 0xCB 0x43/0x4B/0x53/0x5B/0x63/0x6B/0x73/0x7B
        /// </summary>
        [TestMethod]
        public void BIT_N_E_WorksWithBitReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x43 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,E
                });

                var regs = m.Cpu.Registers;
                regs.E = (byte)~(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBeFalse();
                regs.ZFlag.ShouldBeTrue();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeTrue();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// BIT N,E: 0xCB 0x43/0x4B/0x53/0x5B/0x63/0x6B/0x73/0x7B
        /// </summary>
        [TestMethod]
        public void BIT_N_E_WorksWithBitSet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x43 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,E
                });

                var regs = m.Cpu.Registers;
                regs.E = (byte)(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBe(n == 0x07);
                regs.ZFlag.ShouldBeFalse();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeFalse();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// BIT N,H: 0xCB 0x44/0x4C/0x54/0x5C/0x64/0x6C/0x74/0x7C
        /// </summary>
        [TestMethod]
        public void BIT_N_H_WorksWithBitReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x44 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,H
                });

                var regs = m.Cpu.Registers;
                regs.H = (byte)~(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBeFalse();
                regs.ZFlag.ShouldBeTrue();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeTrue();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// BIT N,H: 0xCB 0x44/0x4C/0x54/0x5C/0x64/0x6C/0x74/0x7C
        /// </summary>
        [TestMethod]
        public void BIT_N_H_WorksWithBitSet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x44 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,H
                });

                var regs = m.Cpu.Registers;
                regs.H = (byte)(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBe(n == 0x07);
                regs.ZFlag.ShouldBeFalse();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeFalse();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// BIT N,L: 0xCB 0x45/0x4D/0x55/0x5D/0x65/0x6D/0x75/0x7D
        /// </summary>
        [TestMethod]
        public void BIT_N_L_WorksWithBitReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x45 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,L
                });

                var regs = m.Cpu.Registers;
                regs.L = (byte)~(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBeFalse();
                regs.ZFlag.ShouldBeTrue();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeTrue();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// BIT N,L: 0xCB 0x45/0x4D/0x55/0x5D/0x65/0x6D/0x75/0x7D
        /// </summary>
        [TestMethod]
        public void BIT_N_L_WorksWithBitSet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x45 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,L
                });

                var regs = m.Cpu.Registers;
                regs.L = (byte)(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBe(n == 0x07);
                regs.ZFlag.ShouldBeFalse();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeFalse();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// BIT N,(HL): 0xCB 0x46/0x4E/0x56/0x5E/0x66/0x6E/0x76/0x7E
        /// </summary>
        [TestMethod]
        public void BIT_N_HLi_WorksWithBitReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x46 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,(HL)
                });

                var regs = m.Cpu.Registers;
                regs.HL = 0x1000;
                m.Memory[regs.HL] = (byte)~(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBeFalse();
                regs.ZFlag.ShouldBeTrue();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeTrue();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(12ul);
            }
        }

        /// <summary>
        /// BIT N,(HL): 0xCB 0x46/0x4E/0x56/0x5E/0x66/0x6E/0x76/0x7E
        /// </summary>
        [TestMethod]
        public void BIT_N_HLi_WorksWithBitSet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x46 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,(HL)
                });

                var regs = m.Cpu.Registers;
                regs.HL = 0x1000;
                m.Memory[regs.HL] = (byte)(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBe(n == 0x07);
                regs.ZFlag.ShouldBeFalse();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeFalse();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(12ul);
            }
        }

        /// <summary>
        /// BIT N,A: 0xCB 0x47/0x4F/0x57/0x5F/0x67/0x6F/0x77/0x7F
        /// </summary>
        [TestMethod]
        public void BIT_N_A_WorksWithBitReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x47 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,A
                });

                var regs = m.Cpu.Registers;
                regs.A = (byte)~(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBeFalse();
                regs.ZFlag.ShouldBeTrue();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeTrue();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// BIT N,A: 0xCB 0x47/0x4F/0x57/0x5F/0x67/0x6F/0x77/0x7F
        /// </summary>
        [TestMethod]
        public void BIT_N_A_WorksWithBitSet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x47 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // BIT N,A
                });

                var regs = m.Cpu.Registers;
                regs.A = (byte)(0x01 << n);

                // --- Act
                m.Run();

                // --- Assert
                regs.SFlag.ShouldBe(n == 0x07);
                regs.ZFlag.ShouldBeFalse();
                regs.CFlag.ShouldBeFalse();
                regs.PFlag.ShouldBeFalse();

                regs.HFlag.ShouldBeTrue();
                regs.NFlag.ShouldBeFalse();
                m.ShouldKeepRegisters(except: "F");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8ul);
            }
        }
    }
}
