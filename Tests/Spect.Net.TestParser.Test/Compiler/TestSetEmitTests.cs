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
    }
}
