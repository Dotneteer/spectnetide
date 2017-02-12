using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Z80Emu.Test.Helpers;
using Spect.Net.Z80TestHelpers;

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.Z80Emu.Test.Core.BitOps
{
    [TestClass]
    public class BitOpTestsResN
    {
        /// <summary>
        /// RES N,B: 0xCB 0x80/0x88/0x90/0x98/0xA0/0xA8/0xB0/0xB8
        /// </summary>
        [TestMethod]
        public void RES_N_B_WorksWithBitsAlreadySet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x80 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,B
                });

                var regs = m.Cpu.Registers;
                regs.B = 0xFF;

                // --- Act
                m.Run();

                // --- Assert
                regs.B.ShouldBe((byte)(0xFF & ~(1 << n)));

                m.ShouldKeepRegisters(except: "B");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// RES N,B: 0xCB 0x80/0x88/0x90/0x98/0xA0/0xA8/0xB0/0xB8
        /// </summary>
        [TestMethod]
        public void RES_N_B_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x80 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,B
                });

                var regs = m.Cpu.Registers;
                regs.B = 0xAA;

                // --- Act
                m.Run();

                // --- Assert
                regs.B.ShouldBe((byte)(0xAA & ~(1 << n)));

                m.ShouldKeepRegisters(except: "B");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// RES N,C: 0xCB 0x81/0x89/0x91/0x99/0xA1/0xA9/0xB1/0xB9
        /// </summary>
        [TestMethod]
        public void RES_N_C_WorksWithBitsAlreadySet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x81 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,C
                });

                var regs = m.Cpu.Registers;
                regs.C = 0xFF;

                // --- Act
                m.Run();

                // --- Assert
                regs.C.ShouldBe((byte)(0xFF & ~(1 << n)));

                m.ShouldKeepRegisters(except: "C");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// RES N,C: 0xCB 0x81/0x89/0x91/0x99/0xA1/0xA9/0xB1/0xB9
        /// </summary>
        [TestMethod]
        public void RES_N_C_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x81 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,C
                });

                var regs = m.Cpu.Registers;
                regs.C = 0xAA;

                // --- Act
                m.Run();

                // --- Assert
                regs.C.ShouldBe((byte)(0xAA & ~(1 << n)));

                m.ShouldKeepRegisters(except: "C");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// RES N,D: 0xCB 0x82/0x8A/0x92/0x9A/0xA2/0xAA/0xB2/0xBA
        /// </summary>
        [TestMethod]
        public void RES_N_D_WorksWithBitsAlreadySet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x82 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,D
                });

                var regs = m.Cpu.Registers;
                regs.D = 0xFF;

                // --- Act
                m.Run();

                // --- Assert
                regs.D.ShouldBe((byte)(0xFF & ~(1 << n)));

                m.ShouldKeepRegisters(except: "D");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// RES N,D: 0xCB 0x82/0x8A/0x92/0x9A/0xA2/0xAA/0xB2/0xBA
        /// </summary>
        [TestMethod]
        public void RES_N_D_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x82 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,D
                });

                var regs = m.Cpu.Registers;
                regs.D = 0xAA;

                // --- Act
                m.Run();

                // --- Assert
                regs.D.ShouldBe((byte)(0xAA & ~(1 << n)));

                m.ShouldKeepRegisters(except: "D");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// RES N,E: 0xCB 0x83/0x8B/0x93/0x9B/0xA3/0xAB/0xB3/0xBB
        /// </summary>
        [TestMethod]
        public void RES_N_E_WorksWithBitsAlreadySet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x83 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,E
                });

                var regs = m.Cpu.Registers;
                regs.E = 0xFF;

                // --- Act
                m.Run();

                // --- Assert
                regs.E.ShouldBe((byte)(0xFF & ~(1 << n)));

                m.ShouldKeepRegisters(except: "E");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// RES N,E: 0xCB 0x83/0x8B/0x93/0x9B/0xA3/0xAB/0xB3/0xBB
        /// </summary>
        [TestMethod]
        public void RES_N_E_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x83 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,E
                });

                var regs = m.Cpu.Registers;
                regs.E = 0xAA;

                // --- Act
                m.Run();

                // --- Assert
                regs.E.ShouldBe((byte)(0xAA & ~(1 << n)));

                m.ShouldKeepRegisters(except: "E");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// RES N,H: 0xCB 0x84/0x8C/0x94/0x9C/0xA4/0xAC/0xB4/0xBC
        /// </summary>
        [TestMethod]
        public void RES_N_H_WorksWithBitsAlreadySet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x84 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,H
                });

                var regs = m.Cpu.Registers;
                regs.H = 0xFF;

                // --- Act
                m.Run();

                // --- Assert
                regs.H.ShouldBe((byte)(0xFF & ~(1 << n)));

                m.ShouldKeepRegisters(except: "H");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// RES N,H: 0xCB 0x84/0x8C/0x94/0x9C/0xA4/0xAC/0xB4/0xBC
        /// </summary>
        [TestMethod]
        public void RES_N_H_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x84 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,H
                });

                var regs = m.Cpu.Registers;
                regs.H = 0xAA;

                // --- Act
                m.Run();

                // --- Assert
                regs.H.ShouldBe((byte)(0xAA & ~(1 << n)));

                m.ShouldKeepRegisters(except: "H");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// RES N,L: 0xCB 0x85/0x8D/0x95/0x9D/0xA5/0xAD/0xB5/0xBD
        /// </summary>
        [TestMethod]
        public void RES_N_L_WorksWithBitsAlreadySet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x85 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,L
                });

                var regs = m.Cpu.Registers;
                regs.L = 0xFF;

                // --- Act
                m.Run();

                // --- Assert
                regs.L.ShouldBe((byte)(0xFF & ~(1 << n)));

                m.ShouldKeepRegisters(except: "L");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// RES N,L: 0xCB 0x85/0x8D/0x95/0x9D/0xA5/0xAD/0xB5/0xBD
        /// </summary>
        [TestMethod]
        public void RES_N_L_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x85 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,L
                });

                var regs = m.Cpu.Registers;
                regs.L = 0xAA;

                // --- Act
                m.Run();

                // --- Assert
                regs.L.ShouldBe((byte)(0xAA & ~(1 << n)));

                m.ShouldKeepRegisters(except: "L");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// RES N,(HL): 0xCB 0x86/0x8E/0x96/0x9E/0xA6/0xAE/0xB6/0xBE
        /// </summary>
        [TestMethod]
        public void RES_N_HLi_WorksWithBitsAlreadySet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x86 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,(HL)
                });

                var regs = m.Cpu.Registers;
                regs.HL = 0x1000;
                m.Memory[regs.HL] = 0xFF;

                // --- Act
                m.Run();

                // --- Assert
                m.Memory[regs.HL].ShouldBe((byte)(0xFF & ~(1 << n)));

                m.ShouldKeepRegisters();
                m.ShouldKeepMemory(except: "1000");

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(15ul);
            }
        }

        /// <summary>
        /// RES N,(HL): 0xCB 0x86/0x8E/0x96/0x9E/0xA6/0xAE/0xB6/0xBE
        /// </summary>
        [TestMethod]
        public void RES_N_HLi_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte) (0x86 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,(HL)
                });

                var regs = m.Cpu.Registers;
                regs.HL = 0x1000;
                m.Memory[regs.HL] = 0xAA;

                // --- Act
                m.Run();

                // --- Assert
                m.Memory[regs.HL].ShouldBe((byte) (0xAA & ~(1 << n)));

                m.ShouldKeepRegisters();
                m.ShouldKeepMemory(except: "1000");

                regs.PC.ShouldBe((ushort) 0x0002);
                m.Cpu.Ticks.ShouldBe(15ul);
            }
        }

        /// <summary>
        /// RES N,A: 0xCB 0x87/0x8F/0x97/0x9F/0xA7/0xAF/0xB7/0xBF
        /// </summary>
        [TestMethod]
        public void RES_N_A_WorksWithBitsAlreadySet()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x87 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,A
                });

                var regs = m.Cpu.Registers;
                regs.A = 0xFF;

                // --- Act
                m.Run();

                // --- Assert
                regs.A.ShouldBe((byte)(0xFF & ~(1 << n)));

                m.ShouldKeepRegisters(except: "A");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(8ul);
            }
        }

        /// <summary>
        /// RES N,A: 0xCB 0x87/0x8F/0x97/0x9F/0xA7/0xAF/0xB7/0xBF
        /// </summary>
        [TestMethod]
        public void RES_N_A_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0x87 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // RES N,A
                });

                var regs = m.Cpu.Registers;
                regs.A = 0xAA;

                // --- Act
                m.Run();

                // --- Assert
                regs.A.ShouldBe((byte)(0xAA & ~(1 << n)));

                m.ShouldKeepRegisters(except: "A");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Ticks.ShouldBe(8ul);
            }
        }
    }
}
