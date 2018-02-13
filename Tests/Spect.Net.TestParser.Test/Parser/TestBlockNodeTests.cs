using Antlr4.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.Expressions;
using Spect.Net.TestParser.SyntaxTree.TestSet;

namespace Spect.Net.TestParser.Test.Parser
{
    [TestClass]
    public class TestBlockNodeTests
    {
        [TestMethod]
        public void EmptyTestBlockWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { act call #1234; }");

            // --- Assert
            visitor.TestKeywordSpan.StartLine.ShouldBe(1);
            visitor.TestKeywordSpan.StartColumn.ShouldBe(0);
            visitor.TestKeywordSpan.EndLine.ShouldBe(1);
            visitor.TestKeywordSpan.EndColumn.ShouldBe(3);

            visitor.TestIdSpan.StartLine.ShouldBe(1);
            visitor.TestIdSpan.StartColumn.ShouldBe(5);
            visitor.TestIdSpan.EndLine.ShouldBe(1);
            visitor.TestIdSpan.EndColumn.ShouldBe(10);

            visitor.TestId.ShouldBe("sample");
        }

        [TestMethod]
        public void TestBlockWithCategoryWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { category hard; act call #1234; }");

            // --- Assert
            visitor.TestKeywordSpan.StartLine.ShouldBe(1);
            visitor.TestKeywordSpan.StartColumn.ShouldBe(0);
            visitor.TestKeywordSpan.EndLine.ShouldBe(1);
            visitor.TestKeywordSpan.EndColumn.ShouldBe(3);

            visitor.TestIdSpan.StartLine.ShouldBe(1);
            visitor.TestIdSpan.StartColumn.ShouldBe(5);
            visitor.TestIdSpan.EndLine.ShouldBe(1);
            visitor.TestIdSpan.EndColumn.ShouldBe(10);

            visitor.TestId.ShouldBe("sample");

            visitor.CategoryKeywordSpan.ShouldNotBeNull();
            var cs = visitor.CategoryKeywordSpan.Value;
            cs.StartLine.ShouldBe(1);
            cs.StartColumn.ShouldBe(14);
            cs.EndLine.ShouldBe(1);
            cs.EndColumn.ShouldBe(21);

            visitor.CategoryIdSpan.ShouldNotBeNull();
            cs = visitor.CategoryIdSpan.Value;
            cs.StartLine.ShouldBe(1);
            cs.StartColumn.ShouldBe(23);
            cs.EndLine.ShouldBe(1);
            cs.EndColumn.ShouldBe(26);

            visitor.Category.ShouldBe("hard");
        }

        [TestMethod]
        public void TestBlockWithActCallWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { act call #1234; }");

            // --- Assert
            var act = visitor.Act;
            act.ShouldNotBeNull();
            act.IsCall.ShouldBeTrue();
            act.StartExpr.ShouldBeOfType<LiteralNode>();
            var kw = act.CallOrStartSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(18);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(21);
        }

        [TestMethod]
        public void TestBlockWithStartWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { act start #1234 stop #1235; }");

            // --- Assert
            var act = visitor.Act;
            act.ShouldNotBeNull();
            act.IsCall.ShouldBeFalse();
            act.StartExpr.ShouldBeOfType<LiteralNode>();
            var kw = act.CallOrStartSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(18);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(22);

            act.StopExpr.ShouldBeOfType<LiteralNode>();
            kw = act.StopOrHaltSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(30);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(33);
        }

        [TestMethod]
        public void TestBlockWithHaltWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { act start #1234 halt; }");

            // --- Assert
            var act = visitor.Act;
            act.ShouldNotBeNull();
            act.IsCall.ShouldBeFalse();
            act.StartExpr.ShouldBeOfType<LiteralNode>();
            var kw = act.CallOrStartSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(18);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(22);

            act.IsHalt.ShouldBeTrue();
            act.StopExpr.ShouldBeNull();
            kw = act.StopOrHaltSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(30);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(33);
        }

        [TestMethod]
        public void TestBlockWithDiWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { with di; act call #1234; }");

            // --- Assert
            visitor.TestOptions.Options.Count.ShouldBe(1);
            visitor.TestOptions.Options[0].ShouldBeOfType<DiTestOptionNode>();
        }

        [TestMethod]
        public void TestBlockWithEiWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { with ei; act call #1234; }");

            // --- Assert
            visitor.TestOptions.Options.Count.ShouldBe(1);
            visitor.TestOptions.Options[0].ShouldBeOfType<EiTestOptionNode>();
        }

        [TestMethod]
        public void TestBlockWithTimeoutWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { with timeout 1000; act call #1234; }");

            // --- Assert
            visitor.TestOptions.Options.Count.ShouldBe(1);
            var timeout = visitor.TestOptions.Options[0] as TimeoutTestOptionNode;
            timeout.ShouldNotBeNull();
            timeout.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void TestBlockWithTimeoutAndDiWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { with timeout 1000, di; act call #1234; }");

            // --- Assert
            visitor.TestOptions.Options.Count.ShouldBe(2);
            var timeout = visitor.TestOptions.Options[0] as TimeoutTestOptionNode;
            timeout.ShouldNotBeNull();
            timeout.Expr.ShouldBeOfType<LiteralNode>();
            visitor.TestOptions.Options[1].ShouldBeOfType<DiTestOptionNode>();
        }

        [TestMethod]
        public void TestBlockWithTimeoutAndEiWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { with timeout 1000, ei; act call #1234; }");

            // --- Assert
            visitor.TestOptions.Options.Count.ShouldBe(2);
            var timeout = visitor.TestOptions.Options[0] as TimeoutTestOptionNode;
            timeout.ShouldNotBeNull();
            timeout.Expr.ShouldBeOfType<LiteralNode>();
            visitor.TestOptions.Options[1].ShouldBeOfType<EiTestOptionNode>();
        }

        [TestMethod]
        public void TestBlockWithSingleParamWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { params ab; act call #1234; }");

            // --- Assert
            visitor.Params.Ids.Count.ShouldBe(1);
            visitor.Params.ParamsKeywordSpan.ShouldNotBeNull();
            var kw = visitor.Params.ParamsKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(14);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(19);

            var param = visitor.Params.Ids[0];
            param.Span.StartLine.ShouldBe(1);
            param.Span.StartColumn.ShouldBe(21);
            param.Span.EndLine.ShouldBe(1);
            param.Span.EndColumn.ShouldBe(22);
            param.Id.ShouldBe("ab");
        }

        [TestMethod]
        public void TestBlockWithMultipleParamWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { params par1, par2, par3; act call #1234; }");

            // --- Assert
            visitor.Params.Ids.Count.ShouldBe(3);
            visitor.Params.ParamsKeywordSpan.ShouldNotBeNull();

            visitor.Params.Ids[0].Id.ShouldBe("par1");
            visitor.Params.Ids[1].Id.ShouldBe("par2");
            visitor.Params.Ids[2].Id.ShouldBe("par3");
        }

        [TestMethod]
        public void TestBlockWithSimpleTestCaseWorks1()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { case #1; act call #1234; }");

            // --- Assert
            visitor.Cases.Count.ShouldBe(1);
            var cs = visitor.Cases[0];
            cs.Span.StartLine.ShouldBe(1);
            cs.Span.StartColumn.ShouldBe(14);
            cs.Span.EndLine.ShouldBe(1);
            cs.Span.EndColumn.ShouldBe(21);

            cs.CaseKeywordSpan.StartLine.ShouldBe(1);
            cs.CaseKeywordSpan.StartColumn.ShouldBe(14);
            cs.CaseKeywordSpan.EndLine.ShouldBe(1);
            cs.CaseKeywordSpan.EndColumn.ShouldBe(17);

            cs.Expressions.Count.ShouldBe(1);
            cs.Expressions[0].ShouldBeOfType<LiteralNode>();

            cs.PortMockKeywordSpan.ShouldBeNull();
            cs.PortMocks.Count.ShouldBe(0);
        }

        [TestMethod]
        public void TestBlockWithSimpleTestCaseWorks2()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { case #1, 123; act call #1234; }");

            // --- Assert
            visitor.Cases.Count.ShouldBe(1);
            var cs = visitor.Cases[0];
            cs.Expressions.Count.ShouldBe(2);
            cs.Expressions[0].ShouldBeOfType<LiteralNode>();
            cs.Expressions[1].ShouldBeOfType<LiteralNode>();

            cs.PortMockKeywordSpan.ShouldBeNull();
            cs.PortMocks.Count.ShouldBe(0);
        }

        [TestMethod]
        public void TestBlockWithSimpleTestCaseWorks3()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { case #1, 123 portmock abc; act call #1234; }");

            // --- Assert
            visitor.Cases.Count.ShouldBe(1);
            var cs = visitor.Cases[0];
            cs.Expressions.Count.ShouldBe(2);
            cs.Expressions[0].ShouldBeOfType<LiteralNode>();
            cs.Expressions[1].ShouldBeOfType<LiteralNode>();

            cs.PortMockKeywordSpan.ShouldNotBeNull();
            var kw = cs.PortMockKeywordSpan.Value;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(27);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(34);

            cs.PortMocks.Count.ShouldBe(1);
            cs.PortMocks[0].Id.ShouldBe("abc");
        }

        [TestMethod]
        public void TestBlockWithSimpleTestCaseWorks4()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { case #1, 123 portmock abc, bcd; act call #1234; }");

            // --- Assert
            visitor.Cases.Count.ShouldBe(1);
            var cs = visitor.Cases[0];
            cs.Expressions.Count.ShouldBe(2);
            cs.Expressions[0].ShouldBeOfType<LiteralNode>();
            cs.Expressions[1].ShouldBeOfType<LiteralNode>();

            cs.PortMockKeywordSpan.ShouldNotBeNull();
            cs.PortMocks.Count.ShouldBe(2);
            cs.PortMocks[0].Id.ShouldBe("abc");
            cs.PortMocks[1].Id.ShouldBe("bcd");
        }

        [TestMethod]
        public void TestBlockWithMultipleTestsCaseWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { case #1, 123 portmock abc, bcd; case #2, 234 portmock aaa, bbb; act call #1234; }");

            // --- Assert
            visitor.Cases.Count.ShouldBe(2);
            var cs = visitor.Cases[0];
            cs.Expressions.Count.ShouldBe(2);
            cs.Expressions[0].ShouldBeOfType<LiteralNode>();
            cs.Expressions[1].ShouldBeOfType<LiteralNode>();

            cs.PortMockKeywordSpan.ShouldNotBeNull();
            cs.PortMocks.Count.ShouldBe(2);
            cs.PortMocks[0].Id.ShouldBe("abc");
            cs.PortMocks[1].Id.ShouldBe("bcd");

            cs = visitor.Cases[1];
            cs.Expressions.Count.ShouldBe(2);
            cs.Expressions[0].ShouldBeOfType<LiteralNode>();
            cs.Expressions[1].ShouldBeOfType<LiteralNode>();

            cs.PortMockKeywordSpan.ShouldNotBeNull();
            cs.PortMocks.Count.ShouldBe(2);
            cs.PortMocks[0].Id.ShouldBe("aaa");
            cs.PortMocks[1].Id.ShouldBe("bbb");
        }

        [TestMethod]
        public void TestBlockWithArrangeRegSpecWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { arrange { bc: #bd; } act call #1234; }");

            // --- Assert
            visitor.Arrange.ShouldNotBeNull();
            var ar = visitor.Arrange;

            ar.Span.StartLine.ShouldBe(1);
            ar.Span.StartColumn.ShouldBe(14);
            ar.Span.EndLine.ShouldBe(1);
            ar.Span.EndColumn.ShouldBe(33);

            var kw = ar.KeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(14);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(20);

            ar.Assignments.Count.ShouldBe(1);
            var asg = ar.Assignments[0] as RegisterAssignmentNode;
            asg.ShouldNotBeNull();
            asg.RegisterName.ShouldBe("bc");
            asg.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void TestBlockWithArrangeFlagStatusWorks1()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { arrange {.z; } act call #1234; }");

            // --- Assert
            visitor.Arrange.ShouldNotBeNull();
            var ar = visitor.Arrange;

            ar.Span.StartLine.ShouldBe(1);
            ar.Span.StartColumn.ShouldBe(14);
            ar.Span.EndLine.ShouldBe(1);
            ar.Span.EndColumn.ShouldBe(27);

            var kw = ar.KeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(14);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(20);

            ar.Assignments.Count.ShouldBe(1);
            var asg = ar.Assignments[0] as FlagAssignmentNode;
            asg.ShouldNotBeNull();
            asg.FlagName.ShouldBe("z");
        }

        [TestMethod]
        public void TestBlockWithArrangeFlagStatusWorks2()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { arrange { .h; } act call #1234; }");

            // --- Assert
            visitor.Arrange.ShouldNotBeNull();
            var ar = visitor.Arrange;

            ar.Span.StartLine.ShouldBe(1);
            ar.Span.StartColumn.ShouldBe(14);
            ar.Span.EndLine.ShouldBe(1);
            ar.Span.EndColumn.ShouldBe(28);

            var kw = ar.KeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(14);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(20);

            ar.Assignments.Count.ShouldBe(1);
            var asg = ar.Assignments[0] as FlagAssignmentNode;
            asg.ShouldNotBeNull();
            asg.FlagName.ShouldBe("h");
        }

        [TestMethod]
        public void TestBlockWithArrangeMemAssignmentWorks1()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { arrange { [#1000]: 123; } act call #1234; }");

            // --- Assert
            visitor.Arrange.ShouldNotBeNull();
            var ar = visitor.Arrange;

            ar.Span.StartLine.ShouldBe(1);
            ar.Span.StartColumn.ShouldBe(14);
            ar.Span.EndLine.ShouldBe(1);
            ar.Span.EndColumn.ShouldBe(38);

            var kw = ar.KeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(14);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(20);

            ar.Assignments.Count.ShouldBe(1);
            var asg = ar.Assignments[0] as MemoryAssignmentNode;
            asg.ShouldNotBeNull();
            asg.Address.ShouldBeOfType<LiteralNode>();
            asg.Length.ShouldBeNull();
            asg.Value.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void TestBlockWithArrangeMemAssignmentWorks2()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { arrange { [#1000]: 123:#01; } act call #1234; }");

            // --- Assert
            visitor.Arrange.ShouldNotBeNull();
            var ar = visitor.Arrange;

            ar.Span.StartLine.ShouldBe(1);
            ar.Span.StartColumn.ShouldBe(14);
            ar.Span.EndLine.ShouldBe(1);
            ar.Span.EndColumn.ShouldBe(42);

            var kw = ar.KeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(14);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(20);

            ar.Assignments.Count.ShouldBe(1);
            var asg = ar.Assignments[0] as MemoryAssignmentNode;
            asg.ShouldNotBeNull();
            asg.Address.ShouldBeOfType<LiteralNode>();
            asg.Length.ShouldBeOfType<LiteralNode>();
            asg.Value.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void TestBlockWithMultipleAssignmentsWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { arrange { [#1000]: 123:#01; .nz; de: #1234; } act call #1234; }");

            // --- Assert
            visitor.Arrange.ShouldNotBeNull();
            var ar = visitor.Arrange;

            ar.Span.StartLine.ShouldBe(1);
            ar.Span.StartColumn.ShouldBe(14);
            ar.Span.EndLine.ShouldBe(1);
            ar.Span.EndColumn.ShouldBe(58);

            var kw = ar.KeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(14);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(20);

            ar.Assignments.Count.ShouldBe(3);
            var asg = ar.Assignments[0] as MemoryAssignmentNode;
            asg.ShouldNotBeNull();
            asg.Address.ShouldBeOfType<LiteralNode>();
            asg.Length.ShouldBeOfType<LiteralNode>();
            asg.Value.ShouldBeOfType<LiteralNode>();

            var flag = ar.Assignments[1] as FlagAssignmentNode;
            flag.ShouldNotBeNull();
            flag.FlagName.ShouldBe("nz");

            var reg = ar.Assignments[2] as RegisterAssignmentNode;
            reg.ShouldNotBeNull();
            reg.RegisterName.ShouldBe("de");
            reg.Expr.ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void TestBlockWithAssertSingleExpressionWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { act call #1234; assert { #01; } }");

            // --- Assert
            visitor.Assert.ShouldNotBeNull();
            var asr = visitor.Assert;

            asr.Span.StartLine.ShouldBe(1);
            asr.Span.StartColumn.ShouldBe(30);
            asr.Span.EndLine.ShouldBe(1);
            asr.Span.EndColumn.ShouldBe(44);

            var kw = asr.AssertKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(30);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(35);

            asr.Expressions.Count.ShouldBe(1);
            asr.Expressions[0].ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void TestBlockWithAssertMultipleExpressionWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { act call #1234; assert { #01; #02; 123; } }");

            // --- Assert
            visitor.Assert.ShouldNotBeNull();
            var asr = visitor.Assert;

            asr.Span.StartLine.ShouldBe(1);
            asr.Span.StartColumn.ShouldBe(30);
            asr.Span.EndLine.ShouldBe(1);
            asr.Span.EndColumn.ShouldBe(54);

            var kw = asr.AssertKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(30);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(35);

            asr.Expressions.Count.ShouldBe(3);
            asr.Expressions[0].ShouldBeOfType<LiteralNode>();
            asr.Expressions[1].ShouldBeOfType<LiteralNode>();
            asr.Expressions[2].ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void TestBlockWithSingleBreakpointWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { act call #1234; breakpoint #1000; }");

            // --- Assert
            var bp = visitor.Breakpoints;
            bp.ShouldNotBeNull();
            bp.Span.StartLine.ShouldBe(1);
            bp.Span.StartColumn.ShouldBe(30);
            bp.Span.EndLine.ShouldBe(1);
            bp.Span.EndColumn.ShouldBe(46);

            var kw = bp.BreakpointKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(30);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(39);

            bp.Expressions.Count.ShouldBe(1);
            bp.Expressions[0].ShouldBeOfType<LiteralNode>();
        }

        [TestMethod]
        public void TestBlockWithMultipleBreakpointWorks()
        {
            // --- Act
            var visitor = ParseTestBlock("test sample { act call #1234; breakpoint #1000, #1010, #1020; }");

            // --- Assert
            var bp = visitor.Breakpoints;
            bp.ShouldNotBeNull();
            bp.Span.StartLine.ShouldBe(1);
            bp.Span.StartColumn.ShouldBe(30);
            bp.Span.EndLine.ShouldBe(1);
            bp.Span.EndColumn.ShouldBe(60);

            var kw = bp.BreakpointKeywordSpan;
            kw.StartLine.ShouldBe(1);
            kw.StartColumn.ShouldBe(30);
            kw.EndLine.ShouldBe(1);
            kw.EndColumn.ShouldBe(39);

            bp.Expressions.Count.ShouldBe(3);
            bp.Expressions[0].ShouldBeOfType<LiteralNode>();
            bp.Expressions[1].ShouldBeOfType<LiteralNode>();
            bp.Expressions[2].ShouldBeOfType<LiteralNode>();
        }

        /// <summary>
        /// Returns a visitor with the results of a single parsing pass
        /// </summary>
        /// <param name="textToParse">Z80 assembly code to parse</param>
        /// <param name="expectedErrors">Number of errors expected</param>
        /// <returns>
        /// Visitor with the syntax tree
        /// </returns>
        private static TestBlockNode ParseTestBlock(string textToParse, int expectedErrors = 0)
        {
            var inputStream = new AntlrInputStream(textToParse);
            var lexer = new Z80TestLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80TestParser(tokenStream);
            var context = parser.testBlock();
            var visitor = new Z80TestVisitor();
            var result = (TestBlockNode)visitor.VisitTestBlock(context);
            parser.SyntaxErrors.Count.ShouldBe(expectedErrors);
            return result;
        }

    }
}
