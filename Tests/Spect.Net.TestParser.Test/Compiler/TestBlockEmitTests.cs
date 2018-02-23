using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.TestParser.Compiler;
using Spect.Net.TestParser.Plan;

namespace Spect.Net.TestParser.Test.Compiler
{
    [TestClass]
    public class TestBlockEmitTests : CompilerTestBed
    {
        [TestMethod]
        public void SingleTestBlockWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.Id.ShouldBe("First");
            tb.Category.ShouldBeNull();
            var act = tb.Act as CallPlan;
            act.ShouldNotBeNull();
            act.Address.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void SingleTestBlockWithCategoryWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            category Misc;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.Id.ShouldBe("First");
            tb.Category.ShouldBe("Misc");
            var act = tb.Act as CallPlan;
            act.ShouldNotBeNull();
            act.Address.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void MultipleTestBlockWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            act call #8000;
                        }
                        test Second
                        {
                            act call #8010;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(2);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.Id.ShouldBe("First");
            tb.Category.ShouldBeNull();
            tb = plan.TestSetPlans[0].TestBlocks[1];
            tb.Id.ShouldBe("Second");
            tb.Category.ShouldBeNull();
        }

        [TestMethod]
        public void MultipleTestBlockWithDuplicatedIdFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            act call #8000;
                        }
                        test First
                        {
                            act call #8010;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0007);
        }

        [TestMethod]
        public void ActWithCallWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var act = plan.TestSetPlans[0].TestBlocks[0].Act as CallPlan;
            act.ShouldNotBeNull();
            act.Address.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void ActWithStartWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            act start #8000 halt;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var act = plan.TestSetPlans[0].TestBlocks[0].Act as StartPlan;
            act.ShouldNotBeNull();
            act.Address.ShouldBe((ushort)0x8000);
            act.StopAddress.ShouldBeNull();
        }

        [TestMethod]
        public void ActWithStartStopWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            act start #8000 stop #8010;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var act = plan.TestSetPlans[0].TestBlocks[0].Act as StartPlan;
            act.ShouldNotBeNull();
            act.Address.ShouldBe((ushort)0x8000);
            act.StopAddress.ShouldBe((ushort)0x8010);
        }

        [TestMethod]
        public void TestBlockEmitWithDiWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            with di;
                            act start #8000 stop #8010;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks[0].DisableInterrupt.ShouldBeTrue();
        }

        [TestMethod]
        public void TestBlockEmitWithMultipleInterruptOptionFails1()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            with di, ei;
                            act start #8000 stop #8010;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0005);
        }

        [TestMethod]
        public void TestBlockEmitWithMultipleInterruptOptionFails2()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            with di, di;
                            act start #8000 stop #8010;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0005);
        }

        [TestMethod]
        public void TestBlockEmitWithMultipleInterruptOptionFails3()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            with ei, ei;
                            act start #8000 stop #8010;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0005);
        }

        [TestMethod]
        public void TestBlockEmitWithTimeoutWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            with timeout #1234;
                            act start #8000 stop #8010;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks[0].DisableInterrupt.ShouldBeFalse();
            plan.TestSetPlans[0].TestBlocks[0].TimeoutValue.ShouldBe(0x1234);
        }

        [TestMethod]
        public void TestBlockTimeoutDefaultsTo100()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            act start #8000 stop #8010;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks[0].DisableInterrupt.ShouldBeFalse();
            plan.TestSetPlans[0].TestBlocks[0].TimeoutValue.ShouldBe(100);
        }

        [TestMethod]
        public void TestBlockEmitWithSourceCodeSymbolWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                        with timeout MySymbol;
                            act start #8000 stop #8010;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks[0].DisableInterrupt.ShouldBeFalse();
            plan.TestSetPlans[0].TestBlocks[0].TimeoutValue.ShouldBe(0x2345);
        }

        [TestMethod]
        public void TestBlockEmitWithUnknownSymbolFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            with timeout unknown;
                            act start #8000 stop #8010;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0201);
        }

        [TestMethod]
        public void TestBlockEmitWithEvaluationErrorFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            with timeout 113/0;
                            act start #8000 stop #8010;
                        }
                    }
                    ";
    
            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0200);
        }

        [TestMethod]
        public void SetupWithCallWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            setup call #8000;
                            act start #8000 stop #8010;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var invoke = plan.TestSetPlans[0].TestBlocks[0].Setup as CallPlan;
            invoke.ShouldNotBeNull();
            invoke.Address.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void SetupWithStartWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            setup start #8000 halt;
                            act start #8000 stop #8010;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var invoke = plan.TestSetPlans[0].TestBlocks[0].Setup as StartPlan;
            invoke.ShouldNotBeNull();
            invoke.Address.ShouldBe((ushort)0x8000);
            invoke.StopAddress.ShouldBeNull();
        }

        [TestMethod]
        public void SetupWithStartStopWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            setup start #8000 stop #8010;
                            act start #8000 stop #8010;
                        }
                    }
                    ";
    
            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var invoke = plan.TestSetPlans[0].TestBlocks[0].Setup as StartPlan;
            invoke.ShouldNotBeNull();
            invoke.Address.ShouldBe((ushort)0x8000);
            invoke.StopAddress.ShouldBe((ushort)0x8010);
        }

        [TestMethod]
        public void SingleTestParameterWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            params SingleParam;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.ParameterNames.Count.ShouldBe(1);
            tb.ContainsParameter("SingleParam").ShouldBeTrue();
        }

        [TestMethod]
        public void MultipleTestParameterWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            params Param1, Param2, Param3;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.ParameterNames.Count.ShouldBe(3);
            tb.ContainsParameter("Param1").ShouldBeTrue();
            tb.ContainsParameter("Param2").ShouldBeTrue();
            tb.ContainsParameter("Param3").ShouldBeTrue();
        }

        [TestMethod]
        public void DuplicatedParameterFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            params Param1, Param2, Param1;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0008);
        }

        [TestMethod]
        public void SingleTestCaseWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            params SingleParam;
                            case #1000;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.TestCases.Count.ShouldBe(1);
            tb.TestCases[0].ParamValues.Count.ShouldBe(1);
        }

        [TestMethod]
        public void SingleTestCaseWithDataValueWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data 
                        {
                            value1: #1000;
                        }
                        test First
                        {
                            params SingleParam;
                            case value1;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.TestCases.Count.ShouldBe(1);
            tb.TestCases[0].ParamValues.Count.ShouldBe(1);
        }

        [TestMethod]
        public void SingleTestCaseWithZ80SymbolWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data 
                        {
                            value1: #1000;
                        }
                        test First
                        {
                            params SingleParam;
                            case MySymbol;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.TestCases.Count.ShouldBe(1);
            tb.TestCases[0].ParamValues.Count.ShouldBe(1);
        }

        [TestMethod]
        public void SingleTestCaseWithUnknownSymbolFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data 
                        {
                            value1: #1000;
                        }
                        test First
                        {
                            params SingleParam;
                            case unknown;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0201);
        }

        [TestMethod]
        public void SingleTestCaseWithParameterNameReferenceFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data 
                        {
                            value1: #1000;
                        }
                        test First
                        {
                            params SingleParam;
                            case SingleParam;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0201);
        }

        [TestMethod]
        public void SingleTestCaseWithMultipleParametersWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            params Param1, Param2;
                            case #1000, #2000;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.TestCases.Count.ShouldBe(1);
            tb.TestCases[0].ParamValues.Count.ShouldBe(2);
        }

        [TestMethod]
        public void MultipleTestCaseWithMultipleParametersWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            params Param1, Param2;
                            case #1000, #2000;
                            case #1010, #2020;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.TestCases.Count.ShouldBe(2);
            tb.TestCases[0].ParamValues.Count.ShouldBe(2);
            tb.TestCases[1].ParamValues.Count.ShouldBe(2);
        }

        [TestMethod]
        public void SingleTestCaseWithPortMockWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data 
                        {
                            value1: #1000;
                            mock1 <#FE>: {#38: 0..120};
                        }
                        test First
                        {
                            params SingleParam;
                            case MySymbol portmock mock1;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.TestCases.Count.ShouldBe(1);
            tb.TestCases[0].ParamValues.Count.ShouldBe(1);
            tb.TestCases[0].PortMockPlans.Count.ShouldBe(1);
        }

        [TestMethod]
        public void SingleTestCaseWithMultiplePortMocksWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data 
                        {
                            value1: #1000;
                            mock1 <#FE>: {#38: 0..120};
                            mock2 <#FE>: {#38: 0..120};
                        }
                        test First
                        {
                            params SingleParam;
                            case MySymbol portmock mock1, mock2;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.TestCases.Count.ShouldBe(1);
            tb.TestCases[0].ParamValues.Count.ShouldBe(1);
            tb.TestCases[0].PortMockPlans.Count.ShouldBe(2);
        }

        [TestMethod]
        public void SingleTestCaseWithMissingMockFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data 
                        {
                            value1: #1000;
                            mock1 <#FE>: {#38: 0..120};
                            mock2 <#FE>: {#38: 0..120};
                        }
                        test First
                        {
                            params SingleParam;
                            case MySymbol portmock mock1, unknownmock;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0010);
        }

        [TestMethod]
        public void MultipleTestCasesWithMultiplePortMocksWork()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data 
                        {
                            value1: #1000;
                            mock1 <#FE>: {#38: 0..120};
                            mock2 <#FE>: {#38: 0..120};
                        }
                        test First
                        {
                            params param1, param2;
                            case #1000, #2000 portmock mock1;
                            case #1010, #2020 portmock mock1, mock2;
                            case #1020, #2040 portmock mock1;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.TestCases.Count.ShouldBe(3);
            tb.TestCases[0].ParamValues.Count.ShouldBe(2);
            tb.TestCases[0].PortMockPlans.Count.ShouldBe(1);
            tb.TestCases[1].ParamValues.Count.ShouldBe(2);
            tb.TestCases[1].PortMockPlans.Count.ShouldBe(2);
            tb.TestCases[2].ParamValues.Count.ShouldBe(2);
            tb.TestCases[2].PortMockPlans.Count.ShouldBe(1);
        }

        [TestMethod]
        public void DifferentParamAndCaseArgumentNumbersFail1()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            params SingleParam;
                            case MySymbol, #1000;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0009);
        }

        [TestMethod]
        public void DifferentParamAndCaseArgumentNumbersFail2()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            params Param1, Param2;
                            case MySymbol;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0009);
        }

        [TestMethod]
        public void DifferentParamAndCaseArgumentNumbersFail3()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            params Param1, Param2;
                            case MySymbol;
                            case #1000, #2000;
                            case #1000, MySymbol, #2000;
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(2);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0009);
            plan.Errors[1].ErrorCode.ShouldBe(Errors.T0009);
        }

        [TestMethod]
        public void SingleBreakpointWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            act call #8000;
                            breakpoint #8002;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.Breakpoints.Count.ShouldBe(1);
        }

        [TestMethod]
        public void SingleBreakpointWithParameterReferenceWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            params Param1, Param2;
                            case #8002, #1000;
                            case #8004, #1002;
                            act call #8000;
                            breakpoint Param1;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.Breakpoints.Count.ShouldBe(1);
        }

        [TestMethod]
        public void MultipleBreakpointsWithParameterReferenceWork()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            params Param1, Param2;
                            case #8002, #1000;
                            case #8004, #1002;
                            act call #8000;
                            breakpoint Param2, Param1;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.Breakpoints.Count.ShouldBe(2);
        }

        [TestMethod]
        public void SingleBreakpointWithUnknownIdFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            act call #8000;
                            breakpoint unknown;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0201);
        }

        [TestMethod]
        public void ArrangeWithSingleRegisterWorks1()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            arrange {
                                bc: #1234;
                            }
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.ArrangeAssignments.Count.ShouldBe(1);
            var asgn1 = tb.ArrangeAssignments[0] as RunTimeRegisterAssignmentPlan;
            asgn1.ShouldNotBeNull();
        }

        [TestMethod]
        public void ArrangeWithSingleRegisterWorks2()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            params Param1;
                            case #8000;
                            arrange {
                                bc: Param1;
                            }
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.ArrangeAssignments.Count.ShouldBe(1);
            var asgn1 = tb.ArrangeAssignments[0] as RunTimeRegisterAssignmentPlan;
            asgn1.ShouldNotBeNull();
        }

        [TestMethod]
        public void ArrangeWithSingleRegisterFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            params Param1;
                            case #8000;
                            arrange {
                                bc: unknown;
                            }
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0201);
        }

        [TestMethod]
        public void ArrangeWithSingleFlagWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            arrange {
                                .nz;
                            }
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.ArrangeAssignments.Count.ShouldBe(1);
            var asgn1 = tb.ArrangeAssignments[0] as RunTimeFlagAssignmentPlan;
            asgn1.ShouldNotBeNull();
        }

        [TestMethod]
        public void ArrangeWithSingleMemAssignmentWorks1()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        test First
                        {
                            arrange {
                                [#4000]: mymem;
                            }
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.ArrangeAssignments.Count.ShouldBe(1);
            var asgn1 = tb.ArrangeAssignments[0] as RunTimeMemoryAssignmentPlan;
            asgn1.ShouldNotBeNull();
        }

        [TestMethod]
        public void ArrangeWithSingleMemAssignmentWorks2()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        test First
                        {
                            arrange {
                                [#4000]: mymem : 2;
                            }
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.ArrangeAssignments.Count.ShouldBe(1);
            var asgn1 = tb.ArrangeAssignments[0] as RunTimeMemoryAssignmentPlan;
            asgn1.ShouldNotBeNull();
        }

        [TestMethod]
        public void ArrangeWithSingleMemAssignmentWorks3()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        test First
                        {
                            params Param1;
                            case #4000;
                            arrange {
                                [Param1]: mymem : 2;
                            }
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.ArrangeAssignments.Count.ShouldBe(1);
            var asgn1 = tb.ArrangeAssignments[0] as RunTimeMemoryAssignmentPlan;
            asgn1.ShouldNotBeNull();
        }

        [TestMethod]
        public void ArrangeWithSingleMemAssignmentFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        test First
                        {
                            params Param1;
                            case #4000;
                            arrange {
                                [Param1]: unknown : 2;
                            }
                            act call #8000;
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0201);
        }

        [TestMethod]
        public void AssertWithSingleExpressionWorks1()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        test First
                        {
                            params Param1, Param2;
                            case #8000, #2000;
                            act call #8000;
                            assert 
                            {
                                bc == Param1;
                            }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.Assertions.Count.ShouldBe(1);
        }

        [TestMethod]
        public void AssertWithSingleExpressionWorks2()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        test First
                        {
                            params Param1, Param2;
                            case #8000, #2000;
                            act call #8000;
                            assert 
                            {
                                .nz;
                            }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.Assertions.Count.ShouldBe(1);
        }

        [TestMethod]
        public void AssertWithSingleExpressionWorks3()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        test First
                        {
                            params Param1, Param2;
                            case #8000, #2000;
                            act call #8000;
                            assert 
                            {
                                [Param1..Param2] == mymem;
                            }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.Assertions.Count.ShouldBe(1);
        }

        [TestMethod]
        public void AssertWithSingleExpressionWorks4()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        test First
                        {
                            params Param1, Param2;
                            case #8000, #2000;
                            act call #8000;
                            assert 
                            {
                                [#8000] == mymem;
                            }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.Assertions.Count.ShouldBe(1);
        }

        [TestMethod]
        public void AssertWithSingleExpressionWorks5()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        test First
                        {
                            params Param1, Param2;
                            case #8000, #2000;
                            act call #8000;
                            assert 
                            {
                                <. Param1..Param2 .>;
                            }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.Assertions.Count.ShouldBe(1);
        }

        [TestMethod]
        public void AssertWithSingleExpressionWorks6()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        test First
                        {
                            params Param1, Param2;
                            case #8000, #2000;
                            act call #8000;
                            assert 
                            {
                                <. Param1 .>;
                            }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.Assertions.Count.ShouldBe(1);
        }

        [TestMethod]
        public void AssertWithSingleExpressionFails()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        test First
                        {
                            params Param1, Param2;
                            case #8000, #2000;
                            act call #8000;
                            assert 
                            {
                                bc == unknown;
                            }
                        }
                    }
                    ";

            // --- Act
            var plan = Compile(SOURCE);

            // --- Assert
            plan.Errors.Count.ShouldBe(1);
            plan.Errors[0].ErrorCode.ShouldBe(Errors.T0201);
        }

        [TestMethod]
        public void AssertWithMultipleExpressionWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        data {
                            mymem { 0x11, 0x12, 0x13; }
                        }
                        test First
                        {
                            params Param1, Param2;
                            case #8000, #2000;
                            act call #8000;
                            assert 
                            {
                                bc == #8000;
                                .nc;
                                <. Param1 .>;
                                [#4000] != mymem;
                            }
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            plan.TestSetPlans[0].TestBlocks.Count.ShouldBe(1);
            var tb = plan.TestSetPlans[0].TestBlocks[0];
            tb.Assertions.Count.ShouldBe(4);
        }

        [TestMethod]
        public void CleanupWithCallWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            act call #8000;
                            cleanup call #8000;
                        }
                    }
                    ";
    
            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var invoke = plan.TestSetPlans[0].TestBlocks[0].Cleanup as CallPlan;
            invoke.ShouldNotBeNull();
            invoke.Address.ShouldBe((ushort)0x8000);
        }

        [TestMethod]
        public void CleanupWithStartWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            act call #8000;
                            cleanup start #8000 halt;
                        }
                    }
                    ";
    
            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var invoke = plan.TestSetPlans[0].TestBlocks[0].Cleanup as StartPlan;
            invoke.ShouldNotBeNull();
            invoke.Address.ShouldBe((ushort)0x8000);
            invoke.StopAddress.ShouldBeNull();
        }

        [TestMethod]
        public void CleanupWithStartStopWorks()
        {
            const string SOURCE = @"
                    testset FIRST
                    {
                        source ""Simple.z80asm"";
                        test First
                        {
                            act call #8000;
                            cleanup start #8000 stop #8010;
                        }
                    }
                    ";

            // --- Act
            var plan = CompileWorks(SOURCE);

            // --- Assert
            var invoke = plan.TestSetPlans[0].TestBlocks[0].Cleanup as StartPlan;
            invoke.ShouldNotBeNull();
            invoke.Address.ShouldBe((ushort)0x8000);
            invoke.StopAddress.ShouldBe((ushort)0x8010);
        }
    }
}
