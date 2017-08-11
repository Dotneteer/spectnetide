using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Cpu.Discovery
{
    [TestClass]
    public class StackDebugSupportTests
    {
        /// <summary>
        /// LD SP,NN: 0x31
        /// </summary>
        [TestMethod]
        public void LD_SP_NN_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xFF,      // LD A,FFH
                0x31, 0x26, 0xA9 // LD SP,A926H
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackPointerManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog.First();
            spManip.OperationAddress.ShouldBe((ushort)0x0002);
            spManip.Operation.ShouldBe("ld sp,A926H");
            spManip.OldValue.ShouldBe((ushort)0x0000);
            spManip.NewValue.ShouldBe((ushort)0xA926);
            spManip.Tacts.ShouldBe(17);
        }

        /// <summary>
        /// INC SP: 0x33
        /// </summary>
        [TestMethod]
        public void INC_SP_CollectsDebugInfo()
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
            var spLog = m.StackPointerManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0003);
            spManip.Operation.ShouldBe("inc sp");
            spManip.OldValue.ShouldBe((ushort)0xA926);
            spManip.NewValue.ShouldBe((ushort)0xA927);
            spManip.Tacts.ShouldBe(16);
        }

        /// <summary>
        /// DEC SP: 0x3B
        /// </summary>
        [TestMethod]
        public void DEC_SP_CollectsDebugInfo()
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
            var spLog = m.StackPointerManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0003);
            spManip.Operation.ShouldBe("dec sp");
            spManip.OldValue.ShouldBe((ushort)0xA926);
            spManip.NewValue.ShouldBe((ushort)0xA925);
            spManip.Tacts.ShouldBe(16);
        }

        /// <summary>
        /// LD SP,HL: 0xF9
        /// </summary>
        [TestMethod]
        public void LD_SP_HL_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x00, 0x10, // LD HL,1000H
                0xF9              // LD SP,HL
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackPointerManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0003);
            spManip.Operation.ShouldBe("ld sp,hl");
            spManip.OldValue.ShouldBe((ushort)0x0000);
            spManip.NewValue.ShouldBe((ushort)0x1000);
            spManip.Tacts.ShouldBe(16);
        }

        /// <summary>
        /// LD SP,(NN): 0xED 0x7B
        /// </summary>
        [TestMethod]
        public void LD_SP_NNi_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x3E, 0xFF,            // LD A,FFH
                0xED, 0x7B, 0x00, 0x10 // LD SP,(1000H)
            });
            m.Memory[0x1000] = 0x34;
            m.Memory[0x1001] = 0x12;

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackPointerManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0002);
            spManip.Operation.ShouldBe("ld sp,(1000H)");
            spManip.OldValue.ShouldBe((ushort)0x0000);
            spManip.NewValue.ShouldBe((ushort)0x1234);
            spManip.Tacts.ShouldBe(27);
        }

        /// <summary>
        /// RET NZ: 0xC0
        /// </summary>
        [TestMethod]
        public void RET_NZ_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xB7,             // OR A
                0xC0,             // RET NZ
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0007);
            spManip.Operation.ShouldBe("ret nz");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBeNull();
            spManip.Tacts.ShouldBe(39);
        }

        /// <summary>
        /// RET NZ: 0xC0
        /// </summary>
        [TestMethod]
        public void RET_NZ_DoesNotCollectDebugInfoWhenZ()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x00,       // LD A,00H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xB7,             // OR A
                0xC0,             // RET NZ
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2); // --- CALL, RET
        }

        /// <summary>
        /// POP BC: 0xC1
        /// </summary>
        [TestMethod]
        public void POP_BC_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x52, 0x23, // LD HL,2352H
                0xE5,             // PUSH HL
                0xC1              // POP BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0004);
            spManip.Operation.ShouldBe("pop bc");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBe((ushort)0x2352);
            spManip.Tacts.ShouldBe(31);
        }

        /// <summary>
        /// CALL NZ: 0xC4
        /// </summary>
        [TestMethod]
        public void CALL_NZ_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xB7,             // OR A
                0xC4, 0x07, 0x00, // CALL NZ,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0003);
            spManip.Operation.ShouldBe("call nz,0007H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0006);
            spManip.Tacts.ShouldBe(28);
        }

        /// <summary>
        /// CALL NZ: 0xC4
        /// </summary>
        [TestMethod]
        public void CALL_NZ_DoesNotCollectDebugInfoWhenZ()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x00,       // LD A,00H
                0xB7,             // OR A
                0xC4, 0x07, 0x00, // CALL NZ,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// PUSH BC: 0xC5
        /// </summary>
        [TestMethod]
        public void PUSH_BC_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x01, 0x52, 0x23, // LD BC,2352H
                0xC5,             // PUSH BC
                0xE1              // POP HL
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0003);
            spManip.Operation.ShouldBe("push bc");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x2352);
            spManip.Tacts.ShouldBe(21);
        }

        /// <summary>
        /// RST 00H: 0xC7
        /// </summary>
        [TestMethod]
        public void RST_0_CollectsDebugInfo()
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
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0002);
            spManip.Operation.ShouldBe("rst 00H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0003);
            spManip.Tacts.ShouldBe(18);
        }

        /// <summary>
        /// RET Z: 0xC8
        /// </summary>
        [TestMethod]
        public void RET_Z_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xAF,             // XOR A
                0xC8,             // RET Z
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0007);
            spManip.Operation.ShouldBe("ret z");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBeNull();
            spManip.Tacts.ShouldBe(39);
        }

        /// <summary>
        /// RET Z: 0xC8
        /// </summary>
        [TestMethod]
        public void RET_Z_DoesNotCollectDebugInfoWhenNZ()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xB7,             // OR A
                0xC8,             // RET Z
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2); // --- CALL, RET
        }

        /// <summary>
        /// RET: 0xC9
        /// </summary>
        [TestMethod]
        public void RET_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0006);
            spManip.Operation.ShouldBe("ret");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBeNull();
            spManip.Tacts.ShouldBe(34);
        }

        /// <summary>
        /// CALL Z: 0xCC
        /// </summary>
        [TestMethod]
        public void CALL_Z_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xAF,             // XOR A
                0xCC, 0x07, 0x00, // CALL Z,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0003);
            spManip.Operation.ShouldBe("call z,0007H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0006);
            spManip.Tacts.ShouldBe(28);
        }

        /// <summary>
        /// CALL Z: 0xCC
        /// </summary>
        [TestMethod]
        public void CALL_Z_DoesNotCollectDebugInfoWhenNZ()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xB7,             // OR A
                0xCC, 0x07, 0x00, // CALL Z,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// CALL NN: 0xCD
        /// </summary>
        [TestMethod]
        public void CALL_NN_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x3E, 0xA3,       // LD A,A3H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0002);
            spManip.Operation.ShouldBe("call 0006H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0005);
            spManip.Tacts.ShouldBe(24);
        }

        /// <summary>
        /// RST 08h: 0xCF
        /// </summary>
        [TestMethod]
        public void RST_8_CollectsDebugInfo()
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
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0002);
            spManip.Operation.ShouldBe("rst 08H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0003);
            spManip.Tacts.ShouldBe(18);
        }

        /// <summary>
        /// RET NC: 0xD0
        /// </summary>
        [TestMethod]
        public void RET_NC_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xA7,             // AND A
                0xD0,             // RET NC
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0007);
            spManip.Operation.ShouldBe("ret nc");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBeNull();
            spManip.Tacts.ShouldBe(39);
        }

        /// <summary>
        /// RET NC: 0xD0
        /// </summary>
        [TestMethod]
        public void RET_NC_DoesNotCollectDebugInfoWhenC()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x37,             // SCF
                0xD0,             // RET NC
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2); // --- CALL, RET
        }

        /// <summary>
        /// POP DE: 0xD1
        /// </summary>
        [TestMethod]
        public void POP_DE_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x52, 0x23, // LD HL,2352H
                0xE5,             // PUSH HL
                0xD1              // POP DE
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0004);
            spManip.Operation.ShouldBe("pop de");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBe((ushort)0x2352);
            spManip.Tacts.ShouldBe(31);
        }

        /// <summary>
        /// CALL NC: 0xD4
        /// </summary>
        [TestMethod]
        public void CALL_NC_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xA7,             // AND A
                0xD4, 0x07, 0x00, // CALL NC,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0003);
            spManip.Operation.ShouldBe("call nc,0007H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0006);
            spManip.Tacts.ShouldBe(28);
        }

        /// <summary>
        /// CALL NC: 0xD4
        /// </summary>
        [TestMethod]
        public void CALL_NC_DoesNotCollectDebugInfoWhenC()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0x37,             // SCF
                0xD4, 0x07, 0x00, // CALL NC,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// PUSH DE: 0xD5
        /// </summary>
        [TestMethod]
        public void PUSH_DE_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x11, 0x52, 0x23, // LD DE,2352H
                0xD5,             // PUSH DE
                0xE1              // POP HL
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0003);
            spManip.Operation.ShouldBe("push de");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x2352);
            spManip.Tacts.ShouldBe(21);
        }

        /// <summary>
        /// RST 10H: 0xD7
        /// </summary>
        [TestMethod]
        public void RST_10_CollectsDebugInfo()
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
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0002);
            spManip.Operation.ShouldBe("rst 10H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0003);
            spManip.Tacts.ShouldBe(18);
        }

        /// <summary>
        /// RET C: 0xD8
        /// </summary>
        [TestMethod]
        public void RET_C_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x37,             // SCF
                0xD8,             // RET C
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0007);
            spManip.Operation.ShouldBe("ret c");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBeNull();
            spManip.Tacts.ShouldBe(39);
        }

        /// <summary>
        /// RET C: 0xD8
        /// </summary>
        [TestMethod]
        public void RET_C_DoesNotCollectDebugInfoWhenNC()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xB7,             // OR A
                0xD8,             // RET C
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2); // --- CALL, RET
        }

        /// <summary>
        /// CALL C: 0xDC
        /// </summary>
        [TestMethod]
        public void CALL_C_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0x37,             // SCF
                0xDC, 0x07, 0x00, // CALL C,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0003);
            spManip.Operation.ShouldBe("call c,0007H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0006);
            spManip.Tacts.ShouldBe(28);
        }

        /// <summary>
        /// CALL C: 0xDC
        /// </summary>
        [TestMethod]
        public void CALL_C_DoesNotCollectDebugInfoWhenNC()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xB7,             // OR A
                0xDC, 0x07, 0x00, // CALL C,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(0); 
        }

        /// <summary>
        /// RST 18H: 0xDF
        /// </summary>
        [TestMethod]
        public void RST_18_CollectsDebugInfo()
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
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0002);
            spManip.Operation.ShouldBe("rst 18H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0003);
            spManip.Tacts.ShouldBe(18);
        }

        /// <summary>
        /// RET PO: 0xE0
        /// </summary>
        [TestMethod]
        public void RET_PO_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x2A,       // LD A,2AH
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x87,             // ADD A
                0xE0,             // RET PO
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0007);
            spManip.Operation.ShouldBe("ret po");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBeNull();
            spManip.Tacts.ShouldBe(39);
        }

        /// <summary>
        /// RET PO: 0xE0
        /// </summary>
        [TestMethod]
        public void RET_PO_DoesNotCollectDebugInfoWhenPE()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x88,       // LD A,88H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x87,             // ADD A
                0xE0,             // RET PO
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2); // --- CALL, RET
        }

        /// <summary>
        /// POP HL: 0xE1
        /// </summary>
        [TestMethod]
        public void POP_HL_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x01, 0x52, 0x23, // LD BC,2352H
                0xC5,             // PUSH BC
                0xE1              // POP HL
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0004);
            spManip.Operation.ShouldBe("pop hl");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBe((ushort)0x2352);
            spManip.Tacts.ShouldBe(31);
        }

        /// <summary>
        /// EX (SP),HL: 0xE3
        /// </summary>
        [TestMethod]
        public void EX_SPi_HL_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x31, 0x00, 0x10, // LD SP, 1000H
                0x21, 0x34, 0x12, // LD HL, 1234H
                0xE3              // EX (SP),HL
            });
            m.Memory[0x1000] = 0x78;
            m.Memory[0x1001] = 0x56;

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0006);
            spManip.Operation.ShouldBe("ex (sp),hl");
            spManip.SpValue.ShouldBe((ushort)0x1000);
            spManip.Content.ShouldBe((ushort)0x5678);
            spManip.Tacts.ShouldBe(39);
        }

        /// <summary>
        /// CALL PO: 0xE4
        /// </summary>
        [TestMethod]
        public void CALL_PO_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x2A,       // LD A,2AH
                0x87,             // ADD A
                0xE4, 0x07, 0x00, // CALL PO,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0003);
            spManip.Operation.ShouldBe("call po,0007H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0006);
            spManip.Tacts.ShouldBe(28);
        }

        /// <summary>
        /// CALL PO: 0xE4
        /// </summary>
        [TestMethod]
        public void CALL_PO_DoesNotCollectDebugInfoWhenPE()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x88,       // LD A,88H
                0x87,             // ADD A
                0xE4, 0x07, 0x00, // CALL PO,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// PUSH HL: 0xE5
        /// </summary>
        [TestMethod]
        public void PUSH_HL_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x21, 0x52, 0x23, // LD HL,2352H
                0xE5,             // PUSH HL
                0xC1              // POP BC
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0003);
            spManip.Operation.ShouldBe("push hl");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x2352);
            spManip.Tacts.ShouldBe(21);
        }

        /// <summary>
        /// RST 20H: 0xE7
        /// </summary>
        [TestMethod]
        public void RST_20_CollectsDebugInfo()
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
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0002);
            spManip.Operation.ShouldBe("rst 20H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0003);
            spManip.Tacts.ShouldBe(18);
        }

        /// <summary>
        /// RET PE: 0xE8
        /// </summary>
        [TestMethod]
        public void RET_PE_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x88,       // LD A,88H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x87,             // ADD A
                0xE8,             // RET PE
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0007);
            spManip.Operation.ShouldBe("ret pe");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBeNull();
            spManip.Tacts.ShouldBe(39);
        }

        /// <summary>
        /// RET PE: 0xE8
        /// </summary>
        [TestMethod]
        public void RET_PE_DoesNotCollectDebugInfoWhenPO()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x2A,       // LD A,2AH
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x87,             // ADD A
                0xE8,             // RET PE
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2); // --- CALL, RET
        }

        /// <summary>
        /// CALL PE: 0xEC
        /// </summary>
        [TestMethod]
        public void CALL_PE_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x88,       // LD A,88H
                0x87,             // ADD A
                0xEC, 0x07, 0x00, // CALL PE,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0003);
            spManip.Operation.ShouldBe("call pe,0007H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0006);
            spManip.Tacts.ShouldBe(28);
        }

        /// <summary>
        /// CALL PE: 0xEC
        /// </summary>
        [TestMethod]
        public void CALL_PE_DoesNotCollectDebugInfoWhenPO()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x2A,       // LD A,2AH
                0x87,             // ADD A
                0xEC, 0x07, 0x00, // CALL PE,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// RST 28H: 0xEF
        /// </summary>
        [TestMethod]
        public void RST_28_CollectsDebugInfo()
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
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0002);
            spManip.Operation.ShouldBe("rst 28H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0003);
            spManip.Tacts.ShouldBe(18);
        }

        /// <summary>
        /// RET P: 0xF0
        /// </summary>
        [TestMethod]
        public void RET_P_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x32,       // LD A,32H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x87,             // ADD A
                0xF0,             // RET P
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0007);
            spManip.Operation.ShouldBe("ret p");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBeNull();
            spManip.Tacts.ShouldBe(39);
        }

        /// <summary>
        /// RET P: 0xF0
        /// </summary>
        [TestMethod]
        public void RET_P_DoesNotCollectDebugInfoWhenM()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0xC0,       // LD A,C0H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x87,             // ADD A
                0xF0,             // RET P
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2); // --- CALL, RET
        }

        /// <summary>
        /// POP AF: 0xF1
        /// </summary>
        [TestMethod]
        public void POP_AF_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0x01, 0x52, 0x23, // LD BC,2352H
                0xC5,             // PUSH BC
                0xF1              // POP AF
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0004);
            spManip.Operation.ShouldBe("pop af");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBe((ushort)0x2352);
            spManip.Tacts.ShouldBe(31);
        }

        /// <summary>
        /// CALL P: 0xF4
        /// </summary>
        [TestMethod]
        public void CALL_P_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x32,       // LD A,32H
                0x87,             // ADD A
                0xF4, 0x07, 0x00, // CALL P,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0003);
            spManip.Operation.ShouldBe("call p,0007H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0006);
            spManip.Tacts.ShouldBe(28);
        }

        /// <summary>
        /// CALL P: 0xF4
        /// </summary>
        [TestMethod]
        public void CALL_P_DoesNotCollectDebugInfoWhenM()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0xC0,       // LD A,C0H
                0x87,             // ADD A
                0xF4, 0x07, 0x00, // CALL P,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// PUSH AF: 0xF5
        /// </summary>
        [TestMethod]
        public void PUSH_AF_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilEnd);
            m.InitCode(new byte[]
            {
                0xF5,             // PUSH AF
                0xC1              // POP BC
            });
            m.Cpu.Registers.AF = 0x3456;

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0000);
            spManip.Operation.ShouldBe("push af");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x3456);
            spManip.Tacts.ShouldBe(11);
        }

        /// <summary>
        /// RST 30H: 0xF7
        /// </summary>
        [TestMethod]
        public void RST_30_CollectsDebugInfo()
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
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0002);
            spManip.Operation.ShouldBe("rst 30H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0003);
            spManip.Tacts.ShouldBe(18);
        }

        /// <summary>
        /// RET M: 0xF8
        /// </summary>
        [TestMethod]
        public void RET_M_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0xC0,       // LD A,C0H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x87,             // ADD A
                0xF8,             // RET M
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0007);
            spManip.Operation.ShouldBe("ret m");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBeNull();
            spManip.Tacts.ShouldBe(39);
        }

        /// <summary>
        /// RET M: 0xF8
        /// </summary>
        [TestMethod]
        public void RET_M_DoesNotCollectDebugInfoWhenP()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x32,       // LD A,32H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0x87,             // ADD A
                0xF8,             // RET M
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2); // --- CALL, RET
        }

        /// <summary>
        /// CALL M: 0xFC
        /// </summary>
        [TestMethod]
        public void CALL_M_CollectsDebugInfo()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0xC0,       // LD A,C0H
                0x87,             // ADD A
                0xFC, 0x07, 0x00, // CALL M,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0003);
            spManip.Operation.ShouldBe("call m,0007H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0006);
            spManip.Tacts.ShouldBe(28);
        }

        /// <summary>
        /// CALL M: 0xFC
        /// </summary>
        [TestMethod]
        public void CALL_M_DoesNotCollectDebugInfoWhenP()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x32,       // LD A,32H
                0x87,             // ADD A
                0xFC, 0x07, 0x00, // CALL M,0007H
                0x76,             // HALT
                0x3E, 0x24,       // LD A,24H
                0xC9              // RET
            });

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(0);
        }

        /// <summary>
        /// RST 38H: 0xFF
        /// </summary>
        [TestMethod]
        public void RST_38_CollectsDebugInfo()
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
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0002);
            spManip.Operation.ShouldBe("rst 38H");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x0003);
            spManip.Tacts.ShouldBe(18);
        }

        /// <summary>
        /// RETN: 0xED 0x45
        /// </summary>
        [TestMethod]
        public void RETN_CollectsDebugInformation()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xED, 0x45        // RETN
            });
            m.Cpu.IFF1 = true;

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0006);
            spManip.Operation.ShouldBe("retn");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBeNull();
            spManip.Tacts.ShouldBe(38);
        }

        /// <summary>
        /// RETI: 0xED 0x4D
        /// </summary>
        [TestMethod]
        public void RETI_CollectsDebugInformation()
        {
            // --- Arrange
            var m = new Z80TestMachine(RunMode.UntilHalt);
            m.InitCode(new byte[]
            {
                0x3E, 0x16,       // LD A,16H
                0xCD, 0x06, 0x00, // CALL 0006H
                0x76,             // HALT
                0xED, 0x4D        // RETI
            });
            m.Cpu.IFF1 = false;

            // --- Act
            m.Run();

            // --- Assert
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0006);
            spManip.Operation.ShouldBe("reti");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBeNull();
            spManip.Tacts.ShouldBe(38);
        }

        /// <summary>
        /// POP IX: 0xDD 0xE1
        /// </summary>
        [TestMethod]
        public void POP_IX_CollectsDebugInfo()
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
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0004);
            spManip.Operation.ShouldBe("pop ix");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBe((ushort)0x2352);
            spManip.Tacts.ShouldBe(35);
        }

        /// <summary>
        /// POP IY: 0xFD 0xE1
        /// </summary>
        [TestMethod]
        public void POP_IY_CollectsDebugInfo()
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
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[1];
            spManip.OperationAddress.ShouldBe((ushort)0x0004);
            spManip.Operation.ShouldBe("pop iy");
            spManip.SpValue.ShouldBe((ushort)0xFFFE);
            spManip.Content.ShouldBe((ushort)0x2352);
            spManip.Tacts.ShouldBe(35);
        }

        /// <summary>
        /// PUSH IX: 0xDD 0xE5
        /// </summary>
        [TestMethod]
        public void PUSH_IX_CollectsDebugInfo()
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
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0004);
            spManip.Operation.ShouldBe("push ix");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x2352);
            spManip.Tacts.ShouldBe(29);
        }

        /// <summary>
        /// PUSH IY: 0xFD 0xE5
        /// </summary>
        [TestMethod]
        public void PUSH_IY_CollectsDebugInfo()
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
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(2);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0004);
            spManip.Operation.ShouldBe("push iy");
            spManip.SpValue.ShouldBe((ushort)0x0000);
            spManip.Content.ShouldBe((ushort)0x2352);
            spManip.Tacts.ShouldBe(29);
        }

        /// <summary>
        /// EX (SP),IX: 0xDD 0xE3
        /// </summary>
        [TestMethod]
        public void EX_SPi_IX_CollectsDebugInfo()
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
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0007);
            spManip.Operation.ShouldBe("ex (sp),ix");
            spManip.SpValue.ShouldBe((ushort)0x1000);
            spManip.Content.ShouldBe((ushort)0x5678);
            spManip.Tacts.ShouldBe(47);
        }

        /// <summary>
        /// EX (SP),IY: 0xDD 0xE3
        /// </summary>
        [TestMethod]
        public void EX_SPi_IY_CollectsDebugInfo()
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
            var spLog = m.StackContentManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0007);
            spManip.Operation.ShouldBe("ex (sp),iy");
            spManip.SpValue.ShouldBe((ushort)0x1000);
            spManip.Content.ShouldBe((ushort)0x5678);
            spManip.Tacts.ShouldBe(47);
        }

        /// <summary>
        /// LD SP,IX: 0xDD 0xF9
        /// </summary>
        [TestMethod]
        public void LD_SP_IX_CollectsDebugInfo()
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
            var spLog = m.StackPointerManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0004);
            spManip.Operation.ShouldBe("ld sp,ix");
            spManip.OldValue.ShouldBe((ushort)0x0000);
            spManip.NewValue.ShouldBe((ushort)0x1000);
            spManip.Tacts.ShouldBe(24);
        }

        /// <summary>
        /// LD SP,IY: 0xFD 0xF9
        /// </summary>
        [TestMethod]
        public void LD_SP_IY_CollectsDebugInfo()
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
            var spLog = m.StackPointerManipulations;
            spLog.Count.ShouldBe(1);
            var spManip = spLog[0];
            spManip.OperationAddress.ShouldBe((ushort)0x0004);
            spManip.Operation.ShouldBe("ld sp,iy");
            spManip.OldValue.ShouldBe((ushort)0x0000);
            spManip.NewValue.ShouldBe((ushort)0x1000);
            spManip.Tacts.ShouldBe(24);
        }
    }
}
