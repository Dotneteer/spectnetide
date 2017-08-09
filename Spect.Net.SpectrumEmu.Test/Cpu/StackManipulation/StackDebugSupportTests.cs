using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Cpu.StackManipulation
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


    }
}
