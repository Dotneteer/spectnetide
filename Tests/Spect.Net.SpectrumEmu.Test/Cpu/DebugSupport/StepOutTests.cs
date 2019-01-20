using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Cpu.DebugSupport
{
    [TestClass]
    public class StepOutTests
    {
        /// <summary>
        /// RET NZ: 0xC0
        /// </summary>
        [TestMethod]
        public void RET_NZ_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x3E, 0x16,       // LD A,16H
                0xB7,             // OR A
                0xC0,             // RET NZ
                0xC4, 0x01, 0x00, // CALL NZ,0001H
            }, 0x0000, 0x0005);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0008);

            m.RetStepOutEvents.Count.ShouldBe(1);
            m.StepOutPopEvents.Count.ShouldBe(1);
            m.StepOutPopEvents[0].ShouldBe((ushort)0x0008);
            m.StepOutAddress.ShouldBe((ushort)0x0008);
        }

        /// <summary>
        /// CALL NZ: 0xC4
        /// </summary>
        [TestMethod]
        public void CALL_NZ_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x78,             // LD A,B
                0xC9,             // RET
                0x3E, 0x16,       // LD A,16H
                0xB7,             // OR A
                0xC4, 0x01, 0x00, // CALL NZ,0001H
            }, 0x0000, 0x0003);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0009);
        }

        /// <summary>
        /// RST 00H: 0xC7
        /// </summary>
        [TestMethod]
        public void RST_0_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xC7        // RST 0
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0000);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0003);
        }

        /// <summary>
        /// RET Z: 0xC8
        /// </summary>
        [TestMethod]
        public void RET_Z_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x3E, 0x00,       // LD A,00H
                0xB7,             // OR A
                0xC8,             // RET Z
                0xC4, 0x01, 0x00, // CALL NZ,0001H
            }, 0x0000, 0x0005);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0008);

            m.RetStepOutEvents.Count.ShouldBe(1);
            m.StepOutPopEvents.Count.ShouldBe(1);
            m.StepOutPopEvents[0].ShouldBe((ushort)0x0008);
            m.StepOutAddress.ShouldBe((ushort)0x0008);
        }

        /// <summary>
        /// RET: 0xC9
        /// </summary>
        [TestMethod]
        public void RET_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x3E, 0x16,       // LD A,16H
                0xA7,             // AND A
                0xC9,             // RET
                0xC4, 0x01, 0x00, // CALL NZ,0001H
            }, 0x0000, 0x0005);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0008);

            m.RetStepOutEvents.Count.ShouldBe(1);
            m.StepOutPopEvents.Count.ShouldBe(1);
            m.StepOutPopEvents[0].ShouldBe((ushort)0x0008);
            m.StepOutAddress.ShouldBe((ushort)0x0008);
        }

        /// <summary>
        /// CALL Z: 0xCC
        /// </summary>
        [TestMethod]
        public void CALL_Z_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x78,             // LD A,B
                0xC9,             // RET
                0x3E, 0x00,       // LD A,00H
                0xB7,             // OR A
                0xCC, 0x01, 0x00, // CALL Z,0001H
            }, 0x0000, 0x0003);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0009);
        }

        /// <summary>
        /// CALL: 0xCD
        /// </summary>
        [TestMethod]
        public void CALL_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x78,             // LD A,B
                0xC9,             // RET
                0x3E, 0x16,       // LD A,16H
                0xB7,             // OR A
                0xCD, 0x01, 0x00, // CALL 0001H
            }, 0x0000, 0x0003);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0009);
        }

        /// <summary>
        /// RST 08H: 0xCF
        /// </summary>
        [TestMethod]
        public void RST_8_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xCF        // RST 8
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0008);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0003);
        }

        /// <summary>
        /// RET NC: 0xD0
        /// </summary>
        [TestMethod]
        public void RET_NC_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x3E, 0x16,       // LD A,16H
                0xA7,             // AND A
                0xD0,             // RET NC
                0xC4, 0x01, 0x00, // CALL NZ,0001H
            }, 0x0000, 0x0005);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0008);

            m.RetStepOutEvents.Count.ShouldBe(1);
            m.StepOutPopEvents.Count.ShouldBe(1);
            m.StepOutPopEvents[0].ShouldBe((ushort)0x0008);
            m.StepOutAddress.ShouldBe((ushort)0x0008);
        }

        /// <summary>
        /// CALL NC: 0xD4
        /// </summary>
        [TestMethod]
        public void CALL_NC_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x78,             // LD A,B
                0xC9,             // RET
                0x3E, 0x16,       // LD A,16H
                0xB7,             // OR A
                0xD4, 0x01, 0x00, // CALL NC,0001H
            }, 0x0000, 0x0003);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0009);
        }

        /// <summary>
        /// RST 10H: 0xD7
        /// </summary>
        [TestMethod]
        public void RST_10_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xD7        // RST 10H
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0010);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0003);
        }

        /// <summary>
        /// RET C: 0xD8
        /// </summary>
        [TestMethod]
        public void RET_C_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x3E, 0x16,       // LD A,16H
                0x37,             // SCF
                0xD8,             // RET C
                0xC4, 0x01, 0x00, // CALL NZ,0001H
            }, 0x0000, 0x0005);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0008);

            m.RetStepOutEvents.Count.ShouldBe(1);
            m.StepOutPopEvents.Count.ShouldBe(1);
            m.StepOutPopEvents[0].ShouldBe((ushort)0x0008);
            m.StepOutAddress.ShouldBe((ushort)0x0008);
        }

        /// <summary>
        /// CALL C: 0xDC
        /// </summary>
        [TestMethod]
        public void CALL_C_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x78,             // LD A,B
                0xC9,             // RET
                0x3E, 0x16,       // LD A,16H
                0x37,             // SCF
                0xDC, 0x01, 0x00, // CALL C,0001H
            }, 0x0000, 0x0003);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0009);
        }

        /// <summary>
        /// RST 18H: 0xDF
        /// </summary>
        [TestMethod]
        public void RST_18_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xDF        // RST 18H
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0018);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0003);
        }

        /// <summary>
        /// RET PO: 0xE0
        /// </summary>
        [TestMethod]
        public void RET_PO_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x3E, 0x2A,       // LD A,2AH
                0x87,             // ADD A
                0xE0,             // RET PO
                0xC4, 0x01, 0x00, // CALL NZ,0001H
            }, 0x0000, 0x0005);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0008);

            m.RetStepOutEvents.Count.ShouldBe(1);
            m.StepOutPopEvents.Count.ShouldBe(1);
            m.StepOutPopEvents[0].ShouldBe((ushort)0x0008);
            m.StepOutAddress.ShouldBe((ushort)0x0008);
        }

        /// <summary>
        /// CALL PO: 0xE4
        /// </summary>
        [TestMethod]
        public void CALL_PO_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x78,             // LD A,B
                0xC9,             // RET
                0x3E, 0x2A,       // LD A,2AH
                0x87,             // ADD A
                0xE4, 0x01, 0x00, // CALL PO,0001H
            }, 0x0000, 0x0003);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0009);
        }

        /// <summary>
        /// RST 20H: 0xE7
        /// </summary>
        [TestMethod]
        public void RST_20_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xE7        // RST 20H
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0020);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0003);
        }

        /// <summary>
        /// RET PE: 0xE8
        /// </summary>
        [TestMethod]
        public void RET_PE_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x3E, 0x88,       // LD A,88H
                0x87,             // ADD A
                0xE8,             // RET PE
                0xC4, 0x01, 0x00, // CALL NZ,0001H
            }, 0x0000, 0x0005);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0008);

            m.RetStepOutEvents.Count.ShouldBe(1);
            m.StepOutPopEvents.Count.ShouldBe(1);
            m.StepOutPopEvents[0].ShouldBe((ushort)0x0008);
            m.StepOutAddress.ShouldBe((ushort)0x0008);
        }

        /// <summary>
        /// CALL PE: 0xEC
        /// </summary>
        [TestMethod]
        public void CALL_PE_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x78,             // LD A,B
                0xC9,             // RET
                0x3E, 0x88,       // LD A,88H
                0x87,             // ADD A
                0xEC, 0x01, 0x00, // CALL PE,0001H
            }, 0x0000, 0x0003);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0009);
        }

        /// <summary>
        /// RST 28H: 0xEF
        /// </summary>
        [TestMethod]
        public void RST_28_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xEF        // RST 28H
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0028);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0003);
        }

        /// <summary>
        /// RET P: 0xF0
        /// </summary>
        [TestMethod]
        public void RET_P_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x3E, 0x32,       // LD A,32H
                0x87,             // ADD A
                0xF0,             // RET P
                0xC4, 0x01, 0x00, // CALL NZ,0001H
            }, 0x0000, 0x0005);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0008);

            m.RetStepOutEvents.Count.ShouldBe(1);
            m.StepOutPopEvents.Count.ShouldBe(1);
            m.StepOutPopEvents[0].ShouldBe((ushort)0x0008);
            m.StepOutAddress.ShouldBe((ushort)0x0008);
        }

        /// <summary>
        /// CALL P: 0xF4
        /// </summary>
        [TestMethod]
        public void CALL_P_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x78,             // LD A,B
                0xC9,             // RET
                0x3E, 0x32,       // LD A,32H
                0x87,             // ADD A
                0xF4, 0x01, 0x00, // CALL P,0001H
            }, 0x0000, 0x0003);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0009);
        }

        /// <summary>
        /// RST 30H: 0xF7
        /// </summary>
        [TestMethod]
        public void RST_30_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xF7        // RST 30H
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0030);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0003);
        }

        /// <summary>
        /// RET M: 0xF8
        /// </summary>
        [TestMethod]
        public void RET_M_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x3E, 0xC0,       // LD A,C0H
                0x87,             // ADD A
                0xF8,             // RET M
                0xC4, 0x01, 0x00, // CALL NZ,0001H
            }, 0x0000, 0x0005);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0008);

            m.RetStepOutEvents.Count.ShouldBe(1);
            m.StepOutPopEvents.Count.ShouldBe(1);
            m.StepOutPopEvents[0].ShouldBe((ushort)0x0008);
            m.StepOutAddress.ShouldBe((ushort)0x0008);
        }

        /// <summary>
        /// CALL M: 0xFC
        /// </summary>
        [TestMethod]
        public void CALL_M_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x00,             // NOP
                0x78,             // LD A,B
                0xC9,             // RET
                0x3E, 0xC0,       // LD A,C0H
                0x87,             // ADD A
                0xFC, 0x01, 0x00, // CALL M,0001H
            }, 0x0000, 0x0003);

            // --- Act
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0001);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0009);
        }

        /// <summary>
        /// RST 38H: 0xFF
        /// </summary>
        [TestMethod]
        public void RST_38_SetsStepOutValues()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.OneInstruction);
            m.InitCode(new byte[]
            {
                0x3E, 0x12, // LD A,12H
                0xFF        // RST 38H
            });

            // --- Act
            m.Run();
            m.Run();

            // --- Assert
            m.CallStepOutEvents.Count.ShouldBe(1);
            m.CallStepOutEvents[0].ShouldBe((ushort)0x0038);
            m.StepOutPushEvents.Count.ShouldBe(1);
            m.StepOutPushEvents[0].ShouldBe((ushort)0x0003);
        }
    }
}
