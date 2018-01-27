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
        public void MachineWithSpectrum48Works()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        machine Spectrum48;
                        source ""Simple.z80asm"";
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].MachineType.ShouldBe(MachineType.Spectrum48);
        }

        [TestMethod]
        public void MachineWithSpectrum128Works()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        machine Spectrum128;
                        source ""Simple.z80asm"";
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].MachineType.ShouldBe(MachineType.Spectrum128);
        }

        [TestMethod]
        public void MachineWithSpectrumP3Works()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        machine SpectrumP3;
                        source ""Simple.z80asm"";
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].MachineType.ShouldBe(MachineType.SpectrumP3);
        }

        [TestMethod]
        public void MachineWithSpectrumNextWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        machine Next;
                        source ""Simple.z80asm"";
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].MachineType.ShouldBe(MachineType.Next);
        }

        [TestMethod]
        public void SourceContextRaisesErrorWithNonExistingFile()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        machine Next;
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
                        machine Next;
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
        public void TestSetEmitWithNoNmiWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        with nonmi;
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].DisableInterrupt.ShouldBeTrue();
        }

        [TestMethod]
        public void TestSetEmitWithMultipleNoNmiFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        with nonmi, nonmi;
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0005);
        }

        [TestMethod]
        public void TestSetEmitWithTimeoutWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        with timeout #1234;
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].DisableInterrupt.ShouldBeFalse();
            plan.TestSetPlans[0].TimeoutValue.ShouldBe(0x1234);
        }

        [TestMethod]
        public void TestSetEmitWithSourceCodeSymbolWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        with timeout MySymbol;
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].DisableInterrupt.ShouldBeFalse();
            plan.TestSetPlans[0].TimeoutValue.ShouldBe(0x2345);
        }

        [TestMethod]
        public void TestSetEmitWithUnknownSymbolFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        with timeout unknown;
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0201);
        }

        [TestMethod]
        public void TestSetEmitWithEvaluationErrorFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        with timeout 113/0;
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
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

    }
}
