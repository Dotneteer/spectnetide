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

    }
}
