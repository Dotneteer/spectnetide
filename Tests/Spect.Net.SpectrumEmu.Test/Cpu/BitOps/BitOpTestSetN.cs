using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Test.Helpers;
using Z80TestMachine = Spect.Net.SpectrumEmu.Test.Helpers.Z80TestMachine;

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.SpectrumEmu.Test.Cpu.BitOps
{
    [TestClass]
    public class BitOpTestSetN
    {
        /// <summary>
        /// SET N,B: 0xCB 0xC0/0xC8/0xD0/0xD8/0xE0/0xE8/0xF0/0xF8
        /// </summary>
        [TestMethod]
        public void SET_N_B_WorksWithBitsReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC0 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,B
                });

                var regs = m.Cpu.Registers;
                regs.B = 0x00;

                // --- Act
                m.Run();

                // --- Assert
                regs.B.ShouldBe((byte)(1 << n));

                m.ShouldKeepRegisters(except: "B");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }

        /// <summary>
        /// SET N,B: 0xCB 0xC0/0xC8/0xD0/0xD8/0xE0/0xE8/0xF0/0xF8
        /// </summary>
        [TestMethod]
        public void SET_N_B_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC0 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,B
                });

                var regs = m.Cpu.Registers;
                regs.B = 0x55;

                // --- Act
                m.Run();

                // --- Assert
                regs.B.ShouldBe((byte)(0x55 | (1 << n)));

                m.ShouldKeepRegisters(except: "B");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }

        /// <summary>
        /// SET N,C: 0xCB 0xC1/0xC9/0xD1/0xD9/0xE1/0xE9/0xF1/0xF9
        /// </summary>
        [TestMethod]
        public void SET_N_C_WorksWithBitsReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC1 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,C
                });

                var regs = m.Cpu.Registers;
                regs.C = 0x00;

                // --- Act
                m.Run();

                // --- Assert
                regs.C.ShouldBe((byte)(1 << n));

                m.ShouldKeepRegisters(except: "C");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }

        /// <summary>
        /// SET N,C: 0xCB 0xC1/0xC9/0xD1/0xD9/0xE1/0xE9/0xF1/0xF9
        /// </summary>
        [TestMethod]
        public void SET_N_C_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC1 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,C
                });

                var regs = m.Cpu.Registers;
                regs.C = 0x55;

                // --- Act
                m.Run();

                // --- Assert
                regs.C.ShouldBe((byte)(0x55 | (1 << n)));

                m.ShouldKeepRegisters(except: "C");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }

        /// <summary>
        /// SET N,D: 0xCB 0xC2/0xCA/0xD2/0xDA/0xE2/0xEA/0xF2/0xFA
        /// </summary>
        [TestMethod]
        public void SET_N_D_WorksWithBitsReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC2 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,D
                });

                var regs = m.Cpu.Registers;
                regs.D = 0x00;

                // --- Act
                m.Run();

                // --- Assert
                regs.D.ShouldBe((byte)(1 << n));

                m.ShouldKeepRegisters(except: "D");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }

        /// <summary>
        /// SET N,D: 0xCB 0xC2/0xCA/0xD2/0xDA/0xE2/0xEA/0xF2/0xFA
        /// </summary>
        [TestMethod]
        public void SET_N_D_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC2 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,D
                });

                var regs = m.Cpu.Registers;
                regs.D = 0x55;

                // --- Act
                m.Run();

                // --- Assert
                regs.D.ShouldBe((byte)(0x55 | (1 << n)));

                m.ShouldKeepRegisters(except: "D");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }

        /// <summary>
        /// SET N,E: 0xCB 0xC3/0xCB/0xD3/0xDB/0xE3/0xEB/0xF3/0xFB
        /// </summary>
        [TestMethod]
        public void SET_N_E_WorksWithBitsReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC3 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,E
                });

                var regs = m.Cpu.Registers;
                regs.E = 0x00;

                // --- Act
                m.Run();

                // --- Assert
                regs.E.ShouldBe((byte)(1 << n));

                m.ShouldKeepRegisters(except: "E");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }

        /// <summary>
        /// SET N,E: 0xCB 0xC3/0xCB/0xD3/0xDB/0xE3/0xEB/0xF3/0xFB
        /// </summary>
        [TestMethod]
        public void SET_N_E_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC3 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,E
                });

                var regs = m.Cpu.Registers;
                regs.E = 0x55;

                // --- Act
                m.Run();

                // --- Assert
                regs.E.ShouldBe((byte)(0x55 | (1 << n)));

                m.ShouldKeepRegisters(except: "E");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }

        /// <summary>
        /// SET N,H: 0xCB 0xC4/0xCC/0xD4/0xDC/0xE4/0xEC/0xF4/0xFC
        /// </summary>
        [TestMethod]
        public void SET_N_H_WorksWithBitsReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC4 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,H
                });

                var regs = m.Cpu.Registers;
                regs.H = 0x00;

                // --- Act
                m.Run();

                // --- Assert
                regs.H.ShouldBe((byte)(1 << n));

                m.ShouldKeepRegisters(except: "H");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }

        /// <summary>
        /// SET N,H: 0xCB 0xC4/0xCC/0xD4/0xDC/0xE4/0xEC/0xF4/0xFC
        /// </summary>
        [TestMethod]
        public void SET_N_H_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC4 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,H
                });

                var regs = m.Cpu.Registers;
                regs.H = 0x55;

                // --- Act
                m.Run();

                // --- Assert
                regs.H.ShouldBe((byte)(0x55 | (1 << n)));

                m.ShouldKeepRegisters(except: "H");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }

        /// <summary>
        /// SET N,L: 0xCB 0xC5/0xCD/0xD5/0xDD/0xE5/0xED/0xF5/0xFD
        /// </summary>
        [TestMethod]
        public void SET_N_L_WorksWithBitsReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC5 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,L
                });

                var regs = m.Cpu.Registers;
                regs.L = 0x00;

                // --- Act
                m.Run();

                // --- Assert
                regs.L.ShouldBe((byte)(1 << n));

                m.ShouldKeepRegisters(except: "L");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }

        /// <summary>
        /// SET N,L: 0xCB 0xC5/0xCD/0xD5/0xDD/0xE5/0xED/0xF5/0xFD
        /// </summary>
        [TestMethod]
        public void SET_N_L_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC5 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,L
                });

                var regs = m.Cpu.Registers;
                regs.L = 0x55;

                // --- Act
                m.Run();

                // --- Assert
                regs.L.ShouldBe((byte)(0x55 | (1 << n)));

                m.ShouldKeepRegisters(except: "L");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }

        /// <summary>
        /// SET N,(HL): 0xCB 0xC6/0xCE/0xD6/0xDE/0xE6/0xEE/0xF6/0xFE
        /// </summary>
        [TestMethod]
        public void SET_N_HLi_WorksWithBitsReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC6 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,(HL)
                });

                var regs = m.Cpu.Registers;
                regs.HL = 0x1000;
                m.Memory[regs.HL] = 0x00;

                // --- Act
                m.Run();

                // --- Assert
                m.Memory[regs.HL].ShouldBe((byte)(1 << n));

                m.ShouldKeepRegisters();
                m.ShouldKeepMemory(except: "1000");

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(15L);
            }
        }

        /// <summary>
        /// SET N,(HL): 0xCB 0xC6/0xCE/0xD6/0xDE/0xE6/0xEE/0xF6/0xFE
        /// </summary>
        [TestMethod]
        public void SET_N_HLi_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC6 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,(HL)
                });

                var regs = m.Cpu.Registers;
                regs.HL = 0x1000;
                m.Memory[regs.HL] = 0x55;

                // --- Act
                m.Run();

                // --- Assert
                m.Memory[regs.HL].ShouldBe((byte)(0x55 | (1 << n)));

                m.ShouldKeepRegisters();
                m.ShouldKeepMemory(except: "1000");

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(15L);
            }
        }

        /// <summary>
        /// SET N,A: 0xCB 0xC7/0xCF/0xD7/0xDF/0xE7/0xEF/0xF7/0xFF
        /// </summary>
        [TestMethod]
        public void SET_N_A_WorksWithBitsReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC7 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,A
                });

                var regs = m.Cpu.Registers;
                regs.A = 0x00;

                // --- Act
                m.Run();

                // --- Assert
                regs.A.ShouldBe((byte)(1 << n));

                m.ShouldKeepRegisters(except: "A");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }

        /// <summary>
        /// SET N,A: 0xCB 0xC7/0xCF/0xD7/0xDF/0xE7/0xEF/0xF7/0xFF
        /// </summary>
        [TestMethod]
        public void SET_N_A_WorksWithBitsPartlyReset()
        {
            for (var n = 0; n < 8; n++)
            {
                // --- Arrange
                var m = new Z80TestMachine(RunMode.OneInstruction);
                var opcn = (byte)(0xC7 | (n << 3));

                m.InitCode(new byte[]
                {
                    0xCB, opcn // SET N,A
                });

                var regs = m.Cpu.Registers;
                regs.A = 0x55;

                // --- Act
                m.Run();

                // --- Assert
                regs.A.ShouldBe((byte)(0x55 | (1 << n)));

                m.ShouldKeepRegisters(except: "A");
                m.ShouldKeepMemory();

                regs.PC.ShouldBe((ushort)0x0002);
                m.Cpu.Tacts.ShouldBe(8L);
            }
        }
    }
}
