using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Cpu;
using Spect.Net.SpectrumEmu.Test.Helpers;
// ReSharper disable InconsistentNaming

// ReSharper disable ArgumentsStyleStringLiteral

namespace Spect.Net.SpectrumEmu.Test.Cpu.StandardOps
{
    [TestClass]
    public class StandardOpTests0X20
    {
        /// <summary>
        /// JR NZ,E: 0x20
        /// </summary>
        [TestMethod]
        public void JR_NZ_E_WorksWithNoJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x01, // LD A,01H
                0x3D,       // DEC A 
                0x20, 0x02  // JR NZ,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18L);
        }

        /// <summary>
        /// JR NZ,E: 0x20
        /// </summary>
        [TestMethod]
        public void JR_NZ_E_WorksWithJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x02, // LD A,02H
                0x3D,       // DEC A 
                0x20, 0x02  // JR NZ,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// LD HL,NN: 0x21
        /// </summary>
        [TestMethod]
        public void LD_HL_NN_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x21, 0x26, 0xA9 // LD HL,A926H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory();

            regs.HL.ShouldBe((ushort)0xA926);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(10L);
        }

        /// <summary>
        /// LD (NN),HL: 0x22
        /// </summary>
        [TestMethod]
        public void LD_NNi_HL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x26, 0xA9, // LD HL,A926H
                0x22, 0x00, 0x10  // LD (1000H),HL
            });

            // --- Act
            var lBefore = m.Memory[0x1000];
            var hBefore = m.Memory[0x1001];
            m.Run();
            var lAfter = m.Memory[0x1000];
            var hAfter = m.Memory[0x1001];
            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory(except: "1000-1001");

            regs.HL.ShouldBe((ushort)0xA926);
            lBefore.ShouldBe((byte)0x00);
            hBefore.ShouldBe((byte)0x00);
            lAfter.ShouldBe((byte)0x26);
            hAfter.ShouldBe((byte)0xA9);

            regs.PC.ShouldBe((ushort)0x0006);
            m.Cpu.Tacts.ShouldBe(26L);
        }

        /// <summary>
        /// INC HL: 0x23
        /// </summary>
        [TestMethod]
        public void INC_HL_WorksAsExpected1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x26, 0xA9, // LD HL,A926H
                0x23              // INC HL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory();

            regs.HL.ShouldBe((ushort)0xA927);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// INC H: 0x24
        /// </summary>
        [TestMethod]
        public void INC_H_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x26, 0x43, // LD H,43H
                0x24        // INC H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "H, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeFalse();

            regs.H.ShouldBe((byte)0x44);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11L);
        }

        /// <summary>
        /// DEC H: 0x25
        /// </summary>
        [TestMethod]
        public void DEC_H_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x26, 0x43, // LD H,43H
                0x25        // DEC H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "H, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeTrue();

            regs.H.ShouldBe((byte)0x42);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11L);
        }

        /// <summary>
        /// LD H,N: 0x26
        /// </summary>
        [TestMethod]
        public void LD_H_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x26, 0x36 // LD B,36H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "H");
            m.ShouldKeepMemory();

            regs.H.ShouldBe((byte)0x36);
            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(7L);
        }

        /// <summary>
        /// DAA: 0x27
        /// </summary>
        [TestMethod]
        public void DAA_WorksAsExpected()
        {
            // --- Arrange
            var samples = new[]
            {
                //new DaaSample(0x99, false, false, false, 0x998C),
                //new DaaSample(0x99, true, false, false, 0x9F8C),
                //new DaaSample(0x7A, false, false, false, 0x8090),
                //new DaaSample(0x7A, true, false, false, 0x8090),
                //new DaaSample(0xA9, false, false, false, 0x090D),
                //new DaaSample(0x87, false, false, true, 0xE7A5),
                new DaaSample(0x87, true, false, true, 0xEDAD),
                //new DaaSample(0x1B, false, false, true, 0x8195),
                //new DaaSample(0x1B, true, false, true, 0x8195),
                //new DaaSample(0xAA, false, false, false, 0x1011),
                //new DaaSample(0xAA, true, false, false, 0x1011),
                //new DaaSample(0xC6, true, false, false, 0x2C29)
            };

            // --- Act
            foreach (var sample in samples)
            {
                var m = new Z80TestMachine(RunMode.UntilEnd);
                m.InitCode(new byte[]
                {
                    0x27  // DAA
                });
                m.Cpu.Registers.A = sample.A;
                m.Cpu.Registers.F = (byte) ((sample.H ? FlagsSetMask.H : 0) 
                    | (sample.N ? FlagsSetMask.N : 0)
                    | (sample.C ? FlagsSetMask.C : 0));

                // --- Act
                m.Run();

                // --- Assert
                var regs = m.Cpu.Registers;

                m.ShouldKeepRegisters(except: "AF");
                m.ShouldKeepMemory();

                regs.AF.ShouldBe(sample.AF);
                regs.PC.ShouldBe((ushort)0x0001);
                m.Cpu.Tacts.ShouldBe(4L);
            }
        }

        /// <summary>
        /// JR Z,E: 0x28
        /// </summary>
        [TestMethod]
        public void JR_Z_E_WorksWithNoJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x02, // LD A,02H
                0x3D,       // DEC A 
                0x28, 0x02  // JR Z,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0005);
            m.Cpu.Tacts.ShouldBe(18L);
        }

        /// <summary>
        /// JR Z,E: 0x28
        /// </summary>
        [TestMethod]
        public void JR_Z_E_WorksWithJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x01, // LD A,01H
                0x3D,       // DEC A 
                0x28, 0x02  // JR Z,02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "AF");
            m.ShouldKeepMemory();

            regs.PC.ShouldBe((ushort)0x0007);
            m.Cpu.Tacts.ShouldBe(23L);
        }

        /// <summary>
        /// ADD HL,HL: 0x29
        /// </summary>
        [TestMethod]
        public void ADD_HL_HL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x34, 0x12, // LD HL,1234H
                0x29              // ADD HL,HL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "F, HL");
            m.ShouldKeepMemory();
            m.ShouldKeepSFlag();
            m.ShouldKeepZFlag();
            m.ShouldKeepPVFlag();
            regs.NFlag.ShouldBeFalse();

            regs.CFlag.ShouldBeFalse();
            regs.HFlag.ShouldBeFalse();

            regs.HL.ShouldBe((ushort)0x2468);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(21L);
        }

        /// <summary>
        /// LD HL,(NN): 0x2A
        /// </summary>
        [TestMethod]
        public void LD_HL_NNi_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x2A, 0x00, 0x10 // LD HL,(1000H)
            });
            m.Memory[0x1000] = 0x34;
            m.Memory[0x1001] = 0x12;

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory();

            regs.HL.ShouldBe((ushort)0x1234);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// DEC HL: 0x2B
        /// </summary>
        [TestMethod]
        public void DEC_HL_WorksAsExpected1()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x26, 0xA9, // LD HL,A926H
                0x2B              // DEC HL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "HL");
            m.ShouldKeepMemory();

            regs.HL.ShouldBe((ushort)0xA925);
            regs.PC.ShouldBe((ushort)0x0004);
            m.Cpu.Tacts.ShouldBe(16L);
        }

        /// <summary>
        /// INC L: 0x2C
        /// </summary>
        [TestMethod]
        public void INC_L_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x2E, 0x43, // LD L,43H
                0x2C        // INC L
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "L, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeFalse();

            regs.L.ShouldBe((byte)0x44);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11L);
        }

        /// <summary>
        /// DEC L: 0x2D
        /// </summary>
        [TestMethod]
        public void DEC_L_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x2E, 0x43, // LD L,43H
                0x2D        // DEC L
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "L, F");
            m.ShouldKeepMemory();
            m.ShouldKeepCFlag();
            regs.NFlag.ShouldBeTrue();

            regs.L.ShouldBe((byte)0x42);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11L);
        }

        /// <summary>
        /// LD L,N: 0x2E
        /// </summary>
        [TestMethod]
        public void LD_L_N_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x2E, 0x26 // LD L,26H
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "L");
            m.ShouldKeepMemory();

            regs.L.ShouldBe((byte)0x26);
            regs.PC.ShouldBe((ushort)0x0002);
            m.Cpu.Tacts.ShouldBe(7L);
        }

        /// <summary>
        /// CPL: 0x2F
        /// </summary>
        [TestMethod]
        public void CPL_WorksAsExpected()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x81, // LD A,81H
                0x2F        // CPL
            });

            // --- Act
            m.Run();

            // --- Assert
            var regs = m.Cpu.Registers;

            m.ShouldKeepRegisters(except: "A, F");
            m.ShouldKeepMemory();
            m.ShouldKeepSFlag();
            m.ShouldKeepZFlag();
            m.ShouldKeepPVFlag();
            m.ShouldKeepCFlag();
            regs.HFlag.ShouldBeTrue();
            regs.NFlag.ShouldBeTrue();

            regs.A.ShouldBe((byte)0x7E);
            regs.PC.ShouldBe((ushort)0x0003);
            m.Cpu.Tacts.ShouldBe(11L);
        }


        private class DaaSample
        {
            public readonly byte A;
            public readonly bool H;
            public readonly bool N;
            public readonly bool C;
            public readonly ushort AF;

            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public DaaSample(byte a, bool h, bool n, bool c, ushort af)
            {
                A = a;
                H = h;
                N = n;
                C = c;
                AF = af;
            }
        }
    }
}