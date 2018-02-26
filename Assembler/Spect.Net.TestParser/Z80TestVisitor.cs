using System;
using System.Globalization;
using Antlr4.Runtime.Tree;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree;
using Spect.Net.TestParser.SyntaxTree.DataBlock;
using Spect.Net.TestParser.SyntaxTree.Expressions;
using Spect.Net.TestParser.SyntaxTree.TestSet;

namespace Spect.Net.TestParser
{
    /// <summary>
    /// This visitor class processes the elements of the AST parsed by ANTLR
    /// </summary>
    public class Z80TestVisitor: Z80TestBaseVisitor<object>
    {
        /// <summary>
        /// Access the compilation results through this object
        /// </summary>
        public CompilationUnit Compilation { get; } = new CompilationUnit();

        #region TestSet visitors

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.testSet"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTestSet(Z80TestParser.TestSetContext context)
        {
            if (IsInvalidContext(context)) return null;

            var node = new TestSetNode(context);
            Compilation.TestSets.Add(node);
            node.SourceContext = (SourceContextNode) VisitSourceContext(context.sourceContext());
            if (context.callstub() != null)
            {
                node.CallStub = (CallStubNode) VisitCallstub(context.callstub());
            }
            if (context.dataBlock() != null)
            {
                node.DataBlock = (DataBlockNode) VisitDataBlock(context.dataBlock());
            }
            if (context.initSettings() != null)
            {
                node.Init = (AssignmentsNode) VisitInitSettings(context.initSettings());
            }

            foreach (var tbContext in context.testBlock())
            {
                node.TestBlocks.Add((TestBlockNode)VisitTestBlock(tbContext));
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.sourceContext"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitSourceContext(Z80TestParser.SourceContextContext context)
        {
            return IsInvalidContext(context) 
                ? null 
                : new SourceContextNode(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.callstub"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitCallstub(Z80TestParser.CallstubContext context)
        {
            return IsInvalidContext(context) 
                ? null 
                : new CallStubNode(context, (ExpressionNode)VisitExpr(context.expr()));
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.testOptions"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTestOptions(Z80TestParser.TestOptionsContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new TestOptionsNode(context);
            foreach (var toContext in context.testOption())
            {
                node.Options.Add((TestOptionNodeBase)VisitTestOption(toContext));
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.testOption"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTestOption(Z80TestParser.TestOptionContext context)
        {
            if (IsInvalidContext(context)) return null;

            // --- Create the appropriate node
            var optionName = context.GetTokenText(0);
            switch (optionName)
            {
                case "di":
                    return new DiTestOptionNode(context);
                case "ei":
                    return new EiTestOptionNode(context);
                case "timeout":
                    return new TimeoutTestOptionNode(context, (ExpressionNode)VisitExpr(context.expr()));
                default:
                    return null;
            }
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.initSettings"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitInitSettings(Z80TestParser.InitSettingsContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new AssignmentsNode(context);
            foreach (var asgContext in context.assignment())
            {
                node.Assignments.Add((AssignmentNode)VisitAssignment(asgContext));
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.assignment"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAssignment(Z80TestParser.AssignmentContext context)
        {
            if (IsInvalidContext(context)) return null;
            if (context.regAssignment() != null)
            {
                return (RegisterAssignmentNode) VisitRegAssignment(context.regAssignment());
            }
            if (context.flagStatus() != null)
            {
                return (FlagAssignmentNode)VisitFlagStatus(context.flagStatus());
            }
            return (MemoryAssignmentNode) VisitMemAssignment(context.memAssignment());
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.regAssignment"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitRegAssignment(Z80TestParser.RegAssignmentContext context)
        {
            return IsInvalidContext(context) 
                ? null 
                : new RegisterAssignmentNode(context, (ExpressionNode)VisitExpr(context.expr()));
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.flagStatus"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitFlagStatus(Z80TestParser.FlagStatusContext context)
        {
            return IsInvalidContext(context) 
                ? null 
                : new FlagAssignmentNode(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.memAssignment"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMemAssignment(Z80TestParser.MemAssignmentContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new MemoryAssignmentNode(context);
            if (context.expr().Length > 0)
            {
                node.Address = (ExpressionNode) VisitExpr(context.expr()[0]);
            }
            if (context.expr().Length > 1)
            {
                node.Value = (ExpressionNode) VisitExpr(context.expr()[1]);
            }
            if (context.expr().Length > 2)
            {
                node.Length = (ExpressionNode) VisitExpr(context.expr()[2]);
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.setupCode"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitSetupCode(Z80TestParser.SetupCodeContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = (InvokeCodeNode)VisitInvokeCode(context.invokeCode());
            if (node != null)
            {
                node.KeywordSpan = new TextSpan(context.SETUP());
                node.Span = new TextSpan(
                    node.KeywordSpan.StartLine,
                    node.KeywordSpan.StartColumn,
                    node.Span.EndLine,
                    node.Span.EndColumn);
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.cleanupCode"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitCleanupCode(Z80TestParser.CleanupCodeContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = (InvokeCodeNode)VisitInvokeCode(context.invokeCode());
            if (node != null)
            {
                node.KeywordSpan = new TextSpan(context.CLEANUP());
                node.Span = new TextSpan(
                    node.KeywordSpan.StartLine,
                    node.KeywordSpan.StartColumn,
                    node.Span.EndLine,
                    node.Span.EndColumn);
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.invokeCode"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitInvokeCode(Z80TestParser.InvokeCodeContext context)
        {
            if (IsInvalidContext(context)) return null;

            var startExpr = context.expr().Length > 0 
                ? (ExpressionNode) VisitExpr(context.expr()[0]) 
                : null;
            var stopExpr = context.expr().Length > 1 
                ? (ExpressionNode)VisitExpr(context.expr()[1]) 
                : null;
            return new InvokeCodeNode(context, startExpr, stopExpr);
        }

        #endregion

        #region DataBlock visitors

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.dataBlock"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDataBlock(Z80TestParser.DataBlockContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new DataBlockNode(context);
            foreach (var body in context.dataBlockBody())
            {
                node.DataMembers.Add((DataMemberNode)VisitDataBlockBody(body));
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.dataBlockBody"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitDataBlockBody(Z80TestParser.DataBlockBodyContext context)
        {
            if (IsInvalidContext(context)) return null;
            if (context.valueDef() != null)
            {
                return (ValueMemberNode) VisitValueDef(context.valueDef());
            }
            if (context.memPattern() != null)
            {
                return (MemoryPatternMemberNode) VisitMemPattern(context.memPattern());
            }
            return (PortMockMemberNode) VisitPortMock(context.portMock());
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.valueDef"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitValueDef(Z80TestParser.ValueDefContext context)
        {
            return IsInvalidContext(context) 
                ? null 
                : new ValueMemberNode(context, (ExpressionNode)VisitExpr(context.expr()));
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.memPattern"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMemPattern(Z80TestParser.MemPatternContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new MemoryPatternMemberNode(context);
            if (context.text() != null)
            {
                node.Patterns.Add((TextPatternNode)VisitText(context.text()));
            }
            else
            {
                foreach (var body in context.memPatternBody())
                {
                    node.Patterns.Add((MemoryPatternNode)VisitMemPatternBody(body));
                }
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.memPatternBody"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMemPatternBody(Z80TestParser.MemPatternBodyContext context)
        {
            if (IsInvalidContext(context)) return null;
            if (context.byteSet() != null)
            {
                return (BytePatternNode) VisitByteSet(context.byteSet());
            }
            if (context.wordSet() != null)
            {
                return (WordPatternNode)VisitWordSet(context.wordSet());
            }
            return (TextPatternNode) VisitText(context.text());
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.byteSet"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitByteSet(Z80TestParser.ByteSetContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new BytePatternNode(context);
            foreach (var expr in context.expr())
            {
                node.Bytes.Add((ExpressionNode)VisitExpr(expr));
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.wordSet"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitWordSet(Z80TestParser.WordSetContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new WordPatternNode(context);
            foreach (var expr in context.expr())
            {
                node.Words.Add((ExpressionNode)VisitExpr(expr));
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.text"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitText(Z80TestParser.TextContext context)
        {
            return IsInvalidContext(context) 
                ? null 
                : new TextPatternNode(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.portMock"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitPortMock(Z80TestParser.PortMockContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new PortMockMemberNode(context, (ExpressionNode)VisitExpr(context.expr()));
            foreach (var pulse in context.portPulse())
            {
                node.Pulses.Add((PortPulseNode)VisitPortPulse(pulse));
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.portPulse"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitPortPulse(Z80TestParser.PortPulseContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new PortPulseNode(context);
            if (context.expr().Length > 0)
            {
                node.ValueExpr = (ExpressionNode) VisitExpr(context.expr()[0]);
            }

            if (context.expr().Length > 1)
            {
                node.Pulse1Expr = (ExpressionNode) VisitExpr(context.expr()[1]);
            }
            if (context.expr().Length > 2)
            {
                node.Pulse2Expr = (ExpressionNode)VisitExpr(context.expr()[2]);
                node.IsInterval = context.GetChild(4).GetText() == "..";
            }
            return node;
        }

        #endregion

        #region TestBlock visitors

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.testBlock"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTestBlock(Z80TestParser.TestBlockContext context)
        {
            if (IsInvalidContext(context)) return null;

            var node = new TestBlockNode(context);
            if (context.testOptions() != null)
            {
                node.TestOptions = (TestOptionsNode) VisitTestOptions(context.testOptions());
            }
            if (context.testOptions() != null)
            {
                node.TestOptions = (TestOptionsNode)VisitTestOptions(context.testOptions());
            }
            if (context.setupCode() != null)
            {
                node.Setup = (InvokeCodeNode)VisitSetupCode(context.setupCode());
            }
            if (context.testParams() != null)
            {
                node.Params = (ParamsNode) VisitTestParams(context.testParams());
            }
            foreach (var testCase in context.testCase())
            {
                node.Cases.Add((TestCaseNode)VisitTestCase(testCase));
            }
            if (context.arrange() != null)
            {
                node.Arrange = (AssignmentsNode) VisitArrange(context.arrange());
            }
            node.Act = (InvokeCodeNode) VisitAct(context.act());
            if (context.breakpoint() != null)
            {
                node.Breakpoints = (BreakpointsNode) VisitBreakpoint(context.breakpoint());
            }
            if (context.assert() != null)
            {
                node.Assert = (AssertNode) VisitAssert(context.assert());
            }
            if (context.cleanupCode() != null)
            {
                node.Cleanup = (InvokeCodeNode)VisitCleanupCode(context.cleanupCode());
            }

            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.testParams"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTestParams(Z80TestParser.TestParamsContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new ParamsNode(context);
            foreach (var id in context.IDENTIFIER())
            {
                node.Ids.Add(new IdentifierNameNode(id));
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.act"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAct(Z80TestParser.ActContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = (InvokeCodeNode)VisitInvokeCode(context.invokeCode());
            if (node != null)
            {
                node.KeywordSpan = new TextSpan(context.ACT());
                node.Span = new TextSpan(
                    node.KeywordSpan.StartLine,
                    node.KeywordSpan.StartColumn,
                    node.Span.EndLine,
                    node.Span.EndColumn);
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.testCase"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitTestCase(Z80TestParser.TestCaseContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new TestCaseNode(context);
            foreach (var expr in context.expr())
            {
                node.Expressions.Add((ExpressionNode)VisitExpr(expr));
            }
            foreach (var id in context.IDENTIFIER())
            {
                node.PortMocks.Add(new IdentifierNameNode(id));
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.arrange"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitArrange(Z80TestParser.ArrangeContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new AssignmentsNode(context);
            foreach (var asgContext in context.assignment())
            {
                node.Assignments.Add((AssignmentNode)VisitAssignment(asgContext));
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.breakpoint"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitBreakpoint(Z80TestParser.BreakpointContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new BreakpointsNode(context);
            foreach (var expr in context.expr())
            {
                node.Expressions.Add((ExpressionNode)VisitExpr(expr));
            }
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.assert"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAssert(Z80TestParser.AssertContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new AssertNode(context);
            foreach (var expr in context.expr())
            {
                node.Expressions.Add((ExpressionNode)VisitExpr(expr));
            }
            return node;
        }

        #endregion

        #region Expression visitors

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.expr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitExpr(Z80TestParser.ExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitOrExpr(context.orExpr());
            if (context.ChildCount == 1) return expr;

            return new ConditionalExpressionNode(context)
            {
                Condition = expr,
                TrueExpression = (ExpressionNode)VisitExpr(context.expr()[0]),
                FalseExpression = (ExpressionNode)VisitExpr(context.expr()[1])
            };
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.orExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitOrExpr(Z80TestParser.OrExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = VisitXorExpr(context.xorExpr()[0]);
            for (var i = 1; i < context.xorExpr().Length; i++)
            {
                var rightExpr = VisitXorExpr(context.xorExpr()[i]);
                expr = new BitwiseOrOperationNode(context)
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.xorExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitXorExpr(Z80TestParser.XorExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = VisitAndExpr(context.andExpr()[0]);
            for (var i = 1; i < context.andExpr().Length; i++)
            {
                var rightExpr = VisitAndExpr(context.andExpr()[i]);
                expr = new BitwiseXorOperationNode(context)
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.andExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAndExpr(Z80TestParser.AndExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = VisitEquExpr(context.equExpr()[0]);
            for (var i = 1; i < context.equExpr().Length; i++)
            {
                var rightExpr = VisitEquExpr(context.equExpr()[i]);
                expr = new BitwiseAndOperationNode(context)
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.equExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitEquExpr(Z80TestParser.EquExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitRelExpr(context.relExpr()[0]);
            var opIndex = 1;
            for (var i = 1; i < context.relExpr().Length; i++)
            {
                var rightExpr = VisitRelExpr(context.relExpr()[i]);
                var opToken = context.GetTokenText(opIndex);
                var equExpr = opToken == "=="
                    ? new EqualOperationNode(context)
                    : new NotEqualOperationNode(context) as BinaryOperationNode;
                equExpr.LeftOperand = expr;
                equExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = equExpr;
                opIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.relExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitRelExpr(Z80TestParser.RelExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitShiftExpr(context.shiftExpr()[0]);
            var opIndex = 1;
            for (var i = 1; i < context.shiftExpr().Length; i++)
            {
                var rightExpr = VisitShiftExpr(context.shiftExpr()[i]);
                var opToken = context.GetTokenText(opIndex);
                var relExpr = opToken == "<"
                    ? new LessThanOperationNode(context)
                    : (opToken == "<="
                        ? new LessThanOrEqualOperationNode(context)
                        : (opToken == ">"
                            ? new GreaterThanOperationNode(context)
                            : new GreaterThanOrEqualOperationNode(context) as BinaryOperationNode));

                relExpr.LeftOperand = expr;
                relExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = relExpr;
                opIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.shiftExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitShiftExpr(Z80TestParser.ShiftExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitAddExpr(context.addExpr()[0]);
            var opIndex = 1;
            for (var i = 1; i < context.addExpr().Length; i++)
            {
                var rightExpr = VisitAddExpr(context.addExpr()[i]);
                var opToken = context.GetTokenText(opIndex);
                var shiftExpr = opToken == "<<"
                    ? new ShiftLeftOperationNode(context)
                    : new ShiftRightOperationNode(context) as BinaryOperationNode;
                shiftExpr.LeftOperand = expr;
                shiftExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = shiftExpr;
                opIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.addExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAddExpr(Z80TestParser.AddExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitMultExpr(context.multExpr()[0]);
            var opIndex = 1;
            for (var i = 1; i < context.multExpr().Length; i++)
            {
                var rightExpr = VisitMultExpr(context.multExpr()[i]);
                var opToken = context.GetTokenText(opIndex);
                var addExpr = opToken == "+"
                    ? new AddOperationNode(context)
                    : new SubtractOperationNode(context) as BinaryOperationNode;
                addExpr.LeftOperand = expr;
                addExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = addExpr;
                opIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.multExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMultExpr(Z80TestParser.MultExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var expr = (ExpressionNode)VisitUnaryExpr(context.unaryExpr()[0]);
            var opIndex = 1;
            for (var i = 1; i < context.unaryExpr().Length; i++)
            {
                var rightExpr = VisitUnaryExpr(context.unaryExpr()[i]);
                var opToken = context.GetTokenText(opIndex);
                var multExpr = opToken == "*"
                    ? new MultiplyOperationNode(context)
                    : (opToken == "/"
                        ? new DivideOperationNode(context)
                        : new ModuloOperationNode(context) as BinaryOperationNode);
                multExpr.LeftOperand = expr;
                multExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = multExpr;
                opIndex += 2;
            }
            return expr;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.unaryExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitUnaryExpr(Z80TestParser.UnaryExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            if (context.unaryExpr() != null)
            {
                var unexpr = (ExpressionNode) VisitUnaryExpr(context.unaryExpr());
                var op = context.GetChild(0).GetText();
                switch (op)
                {
                    case "+":
                        return new UnaryPlusNode(context) { Operand = unexpr };
                    case "-":
                        return new UnaryMinusNode(context) { Operand = unexpr };
                    case "~":
                        return new UnaryBitwiseNotNode(context) { Operand = unexpr };
                    case "!":
                        return new UnaryLogicalNotNode(context) { Operand = unexpr };
                    case "*":
                        return new UnaryAllNode(context) { Operand = unexpr };
                    case "?":
                        return new UnaryAnyNode(context) { Operand = unexpr };
                }
            }
            if (context.literalExpr() != null)
            {
                return VisitLiteralExpr(context.literalExpr());
            }
            if (context.symbolExpr() != null)
            {
                return VisitSymbolExpr(context.symbolExpr());
            }
            if (context.registerSpec() != null)
            {
                return VisitRegisterSpec(context.registerSpec());
            }
            if (context.addrSpec() != null)
            {
                return VisitAddrSpec(context.addrSpec());
            }
            if (context.flag() != null)
            {
                return VisitFlag(context.flag());
            }
            if (context.reachSpec() != null)
            {
                return VisitReachSpec(context.reachSpec());
            }
            if (context.memReadSpec() != null)
            {
                return VisitMemReadSpec(context.memReadSpec());
            }
            if (context.memWriteSpec() != null)
            {
                return VisitMemWriteSpec(context.memWriteSpec());
            }
            return VisitExpr(context.GetChild(1) as Z80TestParser.ExprContext);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.literalExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitLiteralExpr(Z80TestParser.LiteralExprContext context)
        {
            if (IsInvalidContext(context)) return null;

            var token = context.GetText();
            ushort value;
            // --- Hexadecimal literals
            if (token.StartsWith("#"))
            {
                value = ushort.Parse(token.Substring(1), NumberStyles.HexNumber);
            }
            else if (token.StartsWith("0x"))
            {
                value = ushort.Parse(token.Substring(2), NumberStyles.HexNumber);
            }
            else if (token.EndsWith("h", StringComparison.OrdinalIgnoreCase))
            {
                value = (ushort)int.Parse(token.Substring(0, token.Length - 1),
                    NumberStyles.HexNumber);
            }
            // --- Binary literals
            else if (token.StartsWith("%"))
            {
                value = (ushort)Convert.ToInt32(token.Substring(1).Replace("_", ""), 2);
            }
            else if (token.StartsWith("0b"))
            {
                value = (ushort)Convert.ToInt32(token.Substring(2).Replace("_", ""), 2);
            }
            // --- Character literals
            else if (token.StartsWith("\""))
            {
                var charExpr = context.GetText();
                var bytes = SyntaxHelper.SpectrumStringToBytes(charExpr.Substring(1, charExpr.Length - 2));
                value = bytes.Count == 0 ? (ushort)0x00 : bytes[0];
            }
            // --- Decimal literals
            else
            {
                value = int.TryParse(context.GetText(), out var shortVal) ? (ushort)shortVal : (ushort)0;
            }
            return new LiteralNode(context)
            {
                LiteralValue = value
            };
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Generated.Z80TestParser.symbolExpr"/>.
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitSymbolExpr(Z80TestParser.SymbolExprContext context)
        {
            return IsInvalidContext(context) 
                ? null 
                : new IdentifierNode(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.registerSpec"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitRegisterSpec(Z80TestParser.RegisterSpecContext context)
        {
            return IsInvalidContext(context) 
                ? null 
                : new RegisterNode(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.addrSpec"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitAddrSpec(Z80TestParser.AddrSpecContext context)
        {
            if (IsInvalidContext(context)) return null;
            var startExpr = (ExpressionNode) VisitExpr(context.expr()[0]);
            var endExpr = context.expr().Length > 1 ? (ExpressionNode)VisitExpr(context.expr()[1]) : null;
            return new AddressRangeNode(context, startExpr, endExpr);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.flag"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitFlag(Z80TestParser.FlagContext context)
        {
            return IsInvalidContext(context) 
                ? null 
                : new FlagNode(context);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.reachSpec"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitReachSpec(Z80TestParser.ReachSpecContext context)
        {
            if (IsInvalidContext(context)) return null;
            var startExpr = (ExpressionNode)VisitExpr(context.expr()[0]);
            var endExpr = context.expr().Length > 1 ? (ExpressionNode)VisitExpr(context.expr()[1]) : null;
            return new ReachRangeNode(context, startExpr, endExpr);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.memReadSpec"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMemReadSpec(Z80TestParser.MemReadSpecContext context)
        {
            if (IsInvalidContext(context)) return null;
            var startExpr = (ExpressionNode)VisitExpr(context.expr()[0]);
            var endExpr = context.expr().Length > 1 ? (ExpressionNode)VisitExpr(context.expr()[1]) : null;
            return new MemoryReadTouchNode(context, startExpr, endExpr);
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.memWriteSpec"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMemWriteSpec(Z80TestParser.MemWriteSpecContext context)
        {
            if (IsInvalidContext(context)) return null;
            var startExpr = (ExpressionNode)VisitExpr(context.expr()[0]);
            var endExpr = context.expr().Length > 1 ? (ExpressionNode)VisitExpr(context.expr()[1]) : null;
            return new MemoryWriteTouchNode(context, startExpr, endExpr);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Checks if the current context is invalid for further visiting
        /// </summary>
        /// <param name="context"></param>
        private bool IsInvalidContext(ITree context) => context == null || context.ChildCount == 0;

        #endregion
    }
}