using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Cpu.Discovery
{
    [TestClass]
    public class BranchDebugSupportTest
    {
        /// <summary>
        /// DJNZ E: 0x10
        /// </summary>
        [TestMethod]
        public void DJNX_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x02, // LD B,02H
                0x10, 0x02  // DJNZ 02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0002);
            brManip.Operation.ShouldBe("djnz 0006H");
            brManip.JumpAddress.ShouldBe((ushort)0x0006);
            brManip.Tacts.ShouldBe(20);
        }

        /// <summary>
        /// DJNZ E: 0x10
        /// </summary>
        [TestMethod]
        public void DJNZ_DoesNotCollectDebugInfoWhenNoJump()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x06, 0x01, // LD B,01H
                0x10, 0x02  // DJNZ 02H
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// JR E: 0x18
        /// </summary>
        [TestMethod]
        public void JR_E_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0x20, // LD A,20H
                0x18, 0x20  // JR 20H
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0002);
            brManip.Operation.ShouldBe("jr 0024H");
            brManip.JumpAddress.ShouldBe((ushort)0x0024);
            brManip.Tacts.ShouldBe(19);
        }

        /// <summary>
        /// JR NZ,E: 0x20
        /// </summary>
        [TestMethod]
        public void JR_NZ_CollectsDebugInfo()
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
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0003);
            brManip.Operation.ShouldBe("jr nz,0007H");
            brManip.JumpAddress.ShouldBe((ushort)0x0007);
            brManip.Tacts.ShouldBe(23);
        }

        /// <summary>
        /// JR NZ,E: 0x20
        /// </summary>
        [TestMethod]
        public void JR_NZ_E_DoesNotCollectDebugInfoWhenZ()
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
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// JR Z,E: 0x28
        /// </summary>
        [TestMethod]
        public void JR_Z_CollectsDebugInfo()
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
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0003);
            brManip.Operation.ShouldBe("jr z,0007H");
            brManip.JumpAddress.ShouldBe((ushort)0x0007);
            brManip.Tacts.ShouldBe(23);
        }

        /// <summary>
        /// JR Z,E: 0x28
        /// </summary>
        [TestMethod]
        public void JR_Z_DoesNotCollectDebugInfoWhenNZ()
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
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// JR NC,E: 0x30
        /// </summary>
        [TestMethod]
        public void JR_NC_CollectsDebugInfo()
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
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0002);
            brManip.Operation.ShouldBe("jr nc,0006H");
            brManip.JumpAddress.ShouldBe((ushort)0x0006);
            brManip.Tacts.ShouldBe(20);
        }

        /// <summary>
        /// JR NC,E: 0x30
        /// </summary>
        [TestMethod]
        public void JR_NC_DoesNotCollectDebugInfoWhenC()
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
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// JR C,E: 0x38
        /// </summary>
        [TestMethod]
        public void JR_C_CollectsDebugInfo()
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
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0001);
            brManip.Operation.ShouldBe("jr c,0005H");
            brManip.JumpAddress.ShouldBe((ushort)0x0005);
            brManip.Tacts.ShouldBe(16);
        }

        /// <summary>
        /// JR C,E: 0x38
        /// </summary>
        [TestMethod]
        public void JR_C_DoesNotCollectDebugInfoWhenNC()
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
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// JP NZ,NN: 0xC2
        /// </summary>
        [TestMethod]
        public void JP_NZ_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xB7,             // OR A
                0xC2, 0x07, 0x00, // JP NZ,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0003);
            brManip.Operation.ShouldBe("jp nz,0007H");
            brManip.JumpAddress.ShouldBe((ushort)0x0007);
            brManip.Tacts.ShouldBe(21);
        }

        /// <summary>
        /// JP NZ,NN: 0xC2
        /// </summary>
        [TestMethod]
        public void JP_NZ_DoesNotCollectDebugInfoWhenZ()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x00,       // LD A,00H
                0xB7,             // OR A
                0xC2, 0x07, 0x00, // JP NZ,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// JP NN: 0xC3
        /// </summary>
        [TestMethod]
        public void JP_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xC3, 0x06, 0x00, // JP 0006H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0002);
            brManip.Operation.ShouldBe("jp 0006H");
            brManip.JumpAddress.ShouldBe((ushort)0x0006);
            brManip.Tacts.ShouldBe(17);
        }

        /// <summary>
        /// JP Z,NN: 0xCA
        /// </summary>
        [TestMethod]
        public void JP_Z_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xAF,             // XOR A
                0xCA, 0x07, 0x00, // JP Z,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0003);
            brManip.Operation.ShouldBe("jp z,0007H");
            brManip.JumpAddress.ShouldBe((ushort)0x0007);
            brManip.Tacts.ShouldBe(21);
        }

        /// <summary>
        /// JP Z,NN: 0xCA
        /// </summary>
        [TestMethod]
        public void JP_Z_DoesNotCollectDebugInfoWhenNZ()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xB7,             // OR A
                0xCA, 0x07, 0x00, // JP Z,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// JP NC,NN: 0xD2
        /// </summary>
        [TestMethod]
        public void JP_NC_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xA7,             // AND A
                0xD2, 0x07, 0x00, // JP NC,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0003);
            brManip.Operation.ShouldBe("jp nc,0007H");
            brManip.JumpAddress.ShouldBe((ushort)0x0007);
            brManip.Tacts.ShouldBe(21);
        }

        /// <summary>
        /// JP NC,NN: 0xD2
        /// </summary>
        [TestMethod]
        public void JP_NC_DoesNotCollectDebugInfoWhenC()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0x37,             // SCF
                0xD2, 0x07, 0x00, // JP NC,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// JP C,NN: 0xDA
        /// </summary>
        [TestMethod]
        public void JP_C_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0x37,             // SCF
                0xDA, 0x07, 0x00, // JP C,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0003);
            brManip.Operation.ShouldBe("jp c,0007H");
            brManip.JumpAddress.ShouldBe((ushort)0x0007);
            brManip.Tacts.ShouldBe(21);
        }

        /// <summary>
        /// JP C,NN: 0xDA
        /// </summary>
        [TestMethod]
        public void JP_C_DoesNotCollectDebugInfoWhenNC()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xB7,             // OR A
                0xDA, 0x07, 0x00, // JP C,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// JP PO,NN: 0xE2
        /// </summary>
        [TestMethod]
        public void JP_PO_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x2A,       // LD A,2AH
                0x87,             // ADD A
                0xE2, 0x07, 0x00, // JP PO,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0003);
            brManip.Operation.ShouldBe("jp po,0007H");
            brManip.JumpAddress.ShouldBe((ushort)0x0007);
            brManip.Tacts.ShouldBe(21);
        }

        /// <summary>
        /// JP PO,NN: 0xE2
        /// </summary>
        [TestMethod]
        public void JP_PO_DoesNotCollectDebugInfoWhenPE()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x88,       // LD A,88H
                0x87,             // ADD A
                0xE2, 0x07, 0x00, // JP PO,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// JP (HL): 0xE9
        /// </summary>
        [TestMethod]
        public void JP_HLi_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x00, 0x10, // LD HL, 1000H
                0xE9              // JP (HL)
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0003);
            brManip.Operation.ShouldBe("jp (hl)");
            brManip.JumpAddress.ShouldBe((ushort)0x1000);
            brManip.Tacts.ShouldBe(14);
        }

        /// <summary>
        /// JP PE,NN: 0xEA
        /// </summary>
        [TestMethod]
        public void JP_PE_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x88,       // LD A,88H
                0x87,             // ADD A
                0xEA, 0x07, 0x00, // JP PE,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0003);
            brManip.Operation.ShouldBe("jp pe,0007H");
            brManip.JumpAddress.ShouldBe((ushort)0x0007);
            brManip.Tacts.ShouldBe(21);
        }

        /// <summary>
        /// JP PE,NN: 0xEA
        /// </summary>
        [TestMethod]
        public void JP_PE_DoesNotCollectDebugInfoWhenPO()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x2A,       // LD A,2AH
                0x87,             // ADD A
                0xEA, 0x07, 0x00, // JP PE,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// JP P,NN: 0xF2
        /// </summary>
        [TestMethod]
        public void JP_P_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x32,       // LD A,32H
                0x87,             // ADD A
                0xF2, 0x07, 0x00, // JP P,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0003);
            brManip.Operation.ShouldBe("jp p,0007H");
            brManip.JumpAddress.ShouldBe((ushort)0x0007);
            brManip.Tacts.ShouldBe(21);
        }

        /// <summary>
        /// JP P,NN: 0xF2
        /// </summary>
        [TestMethod]
        public void JP_P_DoesNotCollectDebugInfoWhenM()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0xC0,       // LD A,C0H
                0x87,             // ADD A
                0xF2, 0x07, 0x00, // JP P,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// JP M,NN: 0xFA
        /// </summary>
        [TestMethod]
        public void JP_M_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0xC0,       // LD A,C0H
                0x87,             // ADD A
                0xFA, 0x07, 0x00, // JP M,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0003);
            brManip.Operation.ShouldBe("jp m,0007H");
            brManip.JumpAddress.ShouldBe((ushort)0x0007);
            brManip.Tacts.ShouldBe(21);
        }

        /// <summary>
        /// JP M,NN: 0xFA
        /// </summary>
        [TestMethod]
        public void JP_M_DoesNotCollectDebugInfoWhenP()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x32,       // LD A,32H
                0x87,             // ADD A
                0xFA, 0x07, 0x00, // JP M,0007H
                0x76,             // HALT
                0x3E, 0xAA,       // LD A,AAH
                0x76              // HALT
            });

            // --- Act
            m.Run();

            // --- Assert
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// JP (IX): 0xDD 0xE9
        /// </summary>
        [TestMethod]
        public void JP_IXi_CollectsDebugInfo()
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
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0004);
            brManip.Operation.ShouldBe("jp (ix)");
            brManip.JumpAddress.ShouldBe((ushort)0x1000);
            brManip.Tacts.ShouldBe(22);
        }

        /// <summary>
        /// JP (IY): 0xFD 0xE9
        /// </summary>
        [TestMethod]
        public void JP_IYi_CollectsDebugInfo()
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
            var brLog = m.BranchEvents;
            brLog.Count.ShouldBe(1);
            var brManip = brLog[0];
            brManip.OperationAddress.ShouldBe((ushort)0x0004);
            brManip.Operation.ShouldBe("jp (iy)");
            brManip.JumpAddress.ShouldBe((ushort)0x1000);
            brManip.Tacts.ShouldBe(22);
        }
    }
}
