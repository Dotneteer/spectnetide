using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.TestParser.Compiler;
using Spect.Net.TestParser.Plan;

namespace Spect.Net.TestParser.Test.Compiler
{
    [TestClass]
    public class TestSetEmitTests: CompilerTestBed
    {
        [TestMethod]
        public void SingleTestSetEmitWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                    }
                    ";

            // --- Act/Assert
            CompileWorks(SOURCE);
        }

        [TestMethod]
        public void Sp48ModeEmitWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        sp48mode;
                        source ""Simple.z80asm"";
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(0);
            var testSet = plan.TestSetPlans[0];
            testSet.Sp48Mode.ShouldBeTrue();
        }

        [TestMethod]
        public void CallStubEmitWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        callstub CallStubAddr;
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(0);
            var testSet = plan.TestSetPlans[0];
            testSet.CallStubAddress.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void SourceContextRaisesErrorWithNonExistingFile()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""DoesNotExist.z80asm"";
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0003);
        }

        [TestMethod]
        public void SourceContextRaisesErrorWithFailedZ80Compilation()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Failed.z80asm"";
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0004);
        }

        [TestMethod]
        public void TestSetEmitWithSingleValueDataMemberWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1: #1000+#2345;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var value1 = plan.TestSetPlans[0].GetDataMember("value1");
            value1.AsNumber().ShouldBe(0x1000 + 0x2345);
        }

        [TestMethod]
        public void TestSetEmitWithMultipleValueDataMemberWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1: #1000+#2345;
                            value2: 1000*#1000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var value1 = plan.TestSetPlans[0].GetDataMember("value1");
            value1.AsNumber().ShouldBe(0x1000 + 0x2345);
            var value2 = plan.TestSetPlans[0].GetDataMember("value2");
            value2.AsNumber().ShouldBe(1000 * 0x1000);
        }

        [TestMethod]
        public void TestSetEmitWithDuplicatedValueDataMemberFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1: #1000+#2345;
                            value1: 100;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0006);
        }

        [TestMethod]
        public void TestSetEmitWithSingleByteMemberWorks1()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 { 0x45; }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var value1 = plan.TestSetPlans[0].GetDataMember("value1").AsByteArray();
            value1.SequenceEqual(new byte[] {0x45}).ShouldBeTrue();
        }

        [TestMethod]
        public void TestSetEmitWithSingleByteMemberWorks2()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 { byte 0x45; }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var value1 = plan.TestSetPlans[0].GetDataMember("value1").AsByteArray();
            value1.SequenceEqual(new byte[] { 0x45 }).ShouldBeTrue();
        }

        [TestMethod]
        public void TestSetEmitWithMultipleByteMemberWorks1()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 { 0x45, #1234; }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var value1 = plan.TestSetPlans[0].GetDataMember("value1").AsByteArray();
            value1.SequenceEqual(new byte[] { 0x45, 0x34 }).ShouldBeTrue();
        }

        [TestMethod]
        public void TestSetEmitWithMultipleByteMemberWorks2()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 { byte 0x45, #1234; }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var value1 = plan.TestSetPlans[0].GetDataMember("value1").AsByteArray();
            value1.SequenceEqual(new byte[] { 0x45, 0x34 }).ShouldBeTrue();
        }

        [TestMethod]
        public void TestSetEmitWithSingleWordMemberWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 { word #AC45; }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var value1 = plan.TestSetPlans[0].GetDataMember("value1").AsByteArray();
            value1.SequenceEqual(new byte[] { 0x45, 0xAC }).ShouldBeTrue();
        }

        [TestMethod]
        public void TestSetEmitWithMultipleWordMemberWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 { word #AC45, #1000+#2000; }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var value1 = plan.TestSetPlans[0].GetDataMember("value1").AsByteArray();
            value1.SequenceEqual(new byte[] { 0x45, 0xAC, 0x00, 0x30 }).ShouldBeTrue();
        }

        [TestMethod]
        public void TestSetEmitWithSingleTextMemberWorks1()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 { ""ABCD01\a""; }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var value1 = plan.TestSetPlans[0].GetDataMember("value1").AsByteArray();
            value1.SequenceEqual(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x30, 0x31, 0x16 }).ShouldBeTrue();
        }

        [TestMethod]
        public void TestSetEmitWithSingleTextMemberWorks2()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 { text ""ABCD01\a""; }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var value1 = plan.TestSetPlans[0].GetDataMember("value1").AsByteArray();
            value1.SequenceEqual(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x30, 0x31, 0x16 }).ShouldBeTrue();
        }

        [TestMethod]
        public void TestSetEmitWithMixedMembersWorks1()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 { 
                                0x01, 0x02;
                                ""ABCD01\a""; 
                            }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var value1 = plan.TestSetPlans[0].GetDataMember("value1").AsByteArray();
            value1.SequenceEqual(new byte[] { 0x01, 0x02, 0x41, 0x42, 0x43, 0x44, 0x30, 0x31, 0x16 }).ShouldBeTrue();
        }

        [TestMethod]
        public void TestSetEmitWithMixedMembersWorks2()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 { 
                                ""ABCD01\a""; 
                                0x01, 0x02;
                            }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var value1 = plan.TestSetPlans[0].GetDataMember("value1").AsByteArray();
            value1.SequenceEqual(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x30, 0x31, 0x16, 0x01, 0x02 }).ShouldBeTrue();
        }

        [TestMethod]
        public void TestSetEmitWithMixedMembersWorks3()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 { 
                                word #1234, #4567;
                                ""ABCD01\a""; 
                                0x01, 0x02;
                            }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var value1 = plan.TestSetPlans[0].GetDataMember("value1").AsByteArray();
            value1.SequenceEqual(new byte[] { 0x34, 0x12, 0x67, 0x45, 0x41, 0x42, 0x43, 0x44, 0x30, 0x31, 0x16, 0x01, 0x02 }).ShouldBeTrue();
        }

        [TestMethod]
        public void TestSetEmitWithMixedMembersWorks4()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 { 
                                word #1234, #4567;
                                text ""ABCD01\a""; 
                                byte 0x01, 0x02;
                            }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var value1 = plan.TestSetPlans[0].GetDataMember("value1").AsByteArray();
            value1.SequenceEqual(new byte[] { 0x34, 0x12, 0x67, 0x45, 0x41, 0x42, 0x43, 0x44, 0x30, 0x31, 0x16, 0x01, 0x02 }).ShouldBeTrue();
        }

        [TestMethod]
        public void TestSetEmitWithDuplicatedDataMemberNameFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1: 0x1234;
                            value1 { byte 0x45; }
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0006);
        }


        [TestMethod]
        public void TestSetEmitWithSinglePortMockWorks1()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 <#FE>: {#38: 0..120};
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var mock1 = plan.TestSetPlans[0].GetPortMock("value1");
            mock1.ShouldNotBeNull();
            mock1.PortAddress.ShouldBe((ushort)0xFE);
            mock1.Pulses.Count.ShouldBe(1);
            mock1.Pulses[0].Value.ShouldBe((byte)0x38);
            mock1.Pulses[0].StartTact.ShouldBe(0);
            mock1.Pulses[0].EndTact.ShouldBe(120);
        }

        [TestMethod]
        public void TestSetEmitWithSinglePortMockWorks2()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 <#FE>: {#38: 0:120};
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var mock1 = plan.TestSetPlans[0].GetPortMock("value1");
            mock1.ShouldNotBeNull();
            mock1.PortAddress.ShouldBe((ushort)0xFE);
            mock1.Pulses.Count.ShouldBe(1);
            mock1.Pulses[0].Value.ShouldBe((byte)0x38);
            mock1.Pulses[0].StartTact.ShouldBe(0);
            mock1.Pulses[0].EndTact.ShouldBe(119);
        }

        [TestMethod]
        public void TestSetEmitWithSinglePortMockWorks3()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 <#FE>: {#38:115};
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var mock1 = plan.TestSetPlans[0].GetPortMock("value1");
            mock1.ShouldNotBeNull();
            mock1.PortAddress.ShouldBe((ushort)0xFE);
            mock1.Pulses.Count.ShouldBe(1);
            mock1.Pulses[0].Value.ShouldBe((byte)0x38);
            mock1.Pulses[0].StartTact.ShouldBe(0);
            mock1.Pulses[0].EndTact.ShouldBe(114);
        }

        [TestMethod]
        public void TestSetEmitWithMultiplePortMockPulsesWorks1()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 <#FE>: {#38: 0..120}, {#48: 130..140};
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var mock1 = plan.TestSetPlans[0].GetPortMock("value1");
            mock1.ShouldNotBeNull();
            mock1.PortAddress.ShouldBe((ushort)0xFE);
            mock1.Pulses.Count.ShouldBe(2);
            mock1.Pulses[0].Value.ShouldBe((byte)0x38);
            mock1.Pulses[0].StartTact.ShouldBe(0);
            mock1.Pulses[0].EndTact.ShouldBe(120);
            mock1.Pulses[1].Value.ShouldBe((byte)0x48);
            mock1.Pulses[1].StartTact.ShouldBe(130);
            mock1.Pulses[1].EndTact.ShouldBe(140);
        }

        [TestMethod]
        public void TestSetEmitWithMultiplePortMockPulsesWorks2()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 <#FE>: {#38: 0..120}, {#48: 130:140};
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var mock1 = plan.TestSetPlans[0].GetPortMock("value1");
            mock1.ShouldNotBeNull();
            mock1.PortAddress.ShouldBe((ushort)0xFE);
            mock1.Pulses.Count.ShouldBe(2);
            mock1.Pulses[0].Value.ShouldBe((byte)0x38);
            mock1.Pulses[0].StartTact.ShouldBe(0);
            mock1.Pulses[0].EndTact.ShouldBe(120);
            mock1.Pulses[1].Value.ShouldBe((byte)0x48);
            mock1.Pulses[1].StartTact.ShouldBe(130);
            mock1.Pulses[1].EndTact.ShouldBe(269);
        }

        [TestMethod]
        public void TestSetEmitWithMultiplePortMockPulsesWorks3()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            value1 <#FE>: {#38: 0..120}, {#48:140};
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var mock1 = plan.TestSetPlans[0].GetPortMock("value1");
            mock1.ShouldNotBeNull();
            mock1.PortAddress.ShouldBe((ushort)0xFE);
            mock1.Pulses.Count.ShouldBe(2);
            mock1.Pulses[0].Value.ShouldBe((byte)0x38);
            mock1.Pulses[0].StartTact.ShouldBe(0);
            mock1.Pulses[0].EndTact.ShouldBe(120);
            mock1.Pulses[1].Value.ShouldBe((byte)0x48);
            mock1.Pulses[1].StartTact.ShouldBe(121);
            mock1.Pulses[1].EndTact.ShouldBe(260);
        }

        [TestMethod]
        public void AssignmentWithSingleRegisterWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        init {
                            bc: #1234;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].InitAssignments.Count.ShouldBe(1);
            var asgn1 = plan.TestSetPlans[0].InitAssignments[0] as RegisterAssignmentPlan;
            asgn1.ShouldNotBeNull();
            asgn1.RegisterName.ShouldBe("bc");
            asgn1.Value.ShouldBe((ushort)0x1234);
        }

        [TestMethod]
        public void AssignmentWithMultipleRegisterWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        init {
                            bc: #1234;
                            IXl: #6B;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].InitAssignments.Count.ShouldBe(2);
            var asgn1 = plan.TestSetPlans[0].InitAssignments[0] as RegisterAssignmentPlan;
            asgn1.ShouldNotBeNull();
            asgn1.RegisterName.ShouldBe("bc");
            asgn1.Value.ShouldBe((ushort)0x1234);
            var asgn2 = plan.TestSetPlans[0].InitAssignments[1] as RegisterAssignmentPlan;
            asgn2.ShouldNotBeNull();
            asgn2.RegisterName.ShouldBe("ixl");
            asgn2.Value.ShouldBe((ushort)0x6B);
        }

        [TestMethod]
        public void AssignmentWithSingleFlagWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        init {
                            .c;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].InitAssignments.Count.ShouldBe(1);
            var asgn1 = plan.TestSetPlans[0].InitAssignments[0] as FlagAssignmentPlan;
            asgn1.ShouldNotBeNull();
            asgn1.FlagName.ShouldBe("c");
        }

        [TestMethod]
        public void AssignmentWithMultipleFlagsWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        init 
                            { .c; .NZ; }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].InitAssignments.Count.ShouldBe(2);
            var asgn1 = plan.TestSetPlans[0].InitAssignments[0] as FlagAssignmentPlan;
            asgn1.ShouldNotBeNull();
            asgn1.FlagName.ShouldBe("c");
            var asgn2 = plan.TestSetPlans[0].InitAssignments[1] as FlagAssignmentPlan;
            asgn2.ShouldNotBeNull();
            asgn2.FlagName.ShouldBe("nz");
        }

        [TestMethod]
        public void AssignmentWithSingleMemoryWorks1()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        init {
                            [#4000]: mymem;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].InitAssignments.Count.ShouldBe(1);
            var asgn1 = plan.TestSetPlans[0].InitAssignments[0] as MemoryAssignmentPlan;
            asgn1.ShouldNotBeNull();
            asgn1.Address.ShouldBe((ushort)0x4000);
            asgn1.Value.SequenceEqual(new byte[] {0x11, 0x12, 0x13}).ShouldBeTrue();
        }

        [TestMethod]
        public void AssignmentWithSingleMemoryWorks2()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        init {
                            [#4000]: mymem : 2;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].InitAssignments.Count.ShouldBe(1);
            var asgn1 = plan.TestSetPlans[0].InitAssignments[0] as MemoryAssignmentPlan;
            asgn1.ShouldNotBeNull();
            asgn1.Address.ShouldBe((ushort)0x4000);
            asgn1.Value.SequenceEqual(new byte[] { 0x11, 0x12 }).ShouldBeTrue();
        }

        [TestMethod]
        public void AssignmentWithSingleMemoryWorks3()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        init {
                            [#4000]: mymem : 4;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].InitAssignments.Count.ShouldBe(1);
            var asgn1 = plan.TestSetPlans[0].InitAssignments[0] as MemoryAssignmentPlan;
            asgn1.ShouldNotBeNull();
            asgn1.Address.ShouldBe((ushort)0x4000);
            asgn1.Value.SequenceEqual(new byte[] { 0x11, 0x12, 0x13 }).ShouldBeTrue();
        }

        [TestMethod]
        public void AssignmentWithSingleMemoryWorks4()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        init {
                            [#4000]: 0x18;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].InitAssignments.Count.ShouldBe(1);
            var asgn1 = plan.TestSetPlans[0].InitAssignments[0] as MemoryAssignmentPlan;
            asgn1.ShouldNotBeNull();
            asgn1.Address.ShouldBe((ushort)0x4000);
            asgn1.Value.SequenceEqual(new byte[] { 0x18 }).ShouldBeTrue();
        }

        [TestMethod]
        public void AssignmentWithSingleMemoryWorks5()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        init {
                            [#4000]: 0x18 : 2;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].InitAssignments.Count.ShouldBe(1);
            var asgn1 = plan.TestSetPlans[0].InitAssignments[0] as MemoryAssignmentPlan;
            asgn1.ShouldNotBeNull();
            asgn1.Address.ShouldBe((ushort)0x4000);
            asgn1.Value.SequenceEqual(new byte[] { 0x18 }).ShouldBeTrue();
        }

        [TestMethod]
        public void AssignmentWithSingleMemoryWorks6()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        init {
                            [#4000]: mymem : -4;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].InitAssignments.Count.ShouldBe(1);
            var asgn1 = plan.TestSetPlans[0].InitAssignments[0] as MemoryAssignmentPlan;
            asgn1.ShouldNotBeNull();
            asgn1.Address.ShouldBe((ushort)0x4000);
            asgn1.Value.SequenceEqual(new byte[] { 0x11, 0x12, 0x13 }).ShouldBeTrue();
        }
    }
}
