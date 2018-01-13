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
        private TestSetNode _lastTestSet;
        private TestOptionsNode _lastTestOptions;
        private DataBlockNode _lastDataBlock;
        private MemoryPatternMemberNode _lastMemoryPattern;
        private PortMockMemberNode _lastPortMock;
        private AssignmentsNode _lastAssignmentsNode;

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
            var node = _lastTestSet = new TestSetNode(context);
            Compilation.TestSets.Add(node);
            base.VisitTestSet(context);
            return node;
        }

        /// <summary>
        /// Visit a parse tree produced by <see cref="Z80TestParser.machineContext"/>.
        /// <para>
        /// The default implementation returns the result of calling <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)"/>
        /// on <paramref name="context"/>.
        /// </para>
        /// </summary>
        /// <param name="context">The parse tree.</param>
        /// <return>The visitor result.</return>
        public override object VisitMachineContext(Z80TestParser.MachineContextContext context)
        {
            if (IsInvalidContext(context)) return null;
            var node = new MachineContextNode(context);
            if (_lastTestSet != null)
            {
                _lastTestSet.MachineContext = node;
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
            if (IsInvalidContext(context)) return null;
            var node = new SourceContextNode(context);
            if (_lastTestSet != null)
            {
                _lastTestSet.SourceContext = node;
            }
            return node;
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

            // --- Create the node
            var node = _lastTestOptions = new TestOptionsNode(context);
            if (_lastTestSet != null)
            {
                _lastTestSet.TestOptions = node;
            }

            // --- Set up node information
            base.VisitTestOptions(context);
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
            TestOptionNode option = null;
            if (optionName == "nonmi")
            {
                option = new NoNmiTestOptionNode(context);
            }
            else if (optionName == "timeout")
            {
                var toption = new TimeoutTestOptionNode(context, (ExpressionNode) VisitExpr(context.expr()));
                option = toption;
            }

            // --- Add test option
            if (option != null)
            {
                _lastTestOptions.Options.Add(option);
            }
            return option;
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
            var node = _lastAssignmentsNode = new AssignmentsNode(context);
            if (_lastTestSet != null)
            {
                _lastTestSet.Init = node;
            }
            base.VisitInitSettings(context);
            return node;
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
            if (IsInvalidContext(context)) return null;
            var node = new RegisterAssignmentNode(context, (ExpressionNode)VisitExpr(context.expr()));
            _lastAssignmentsNode?.Assignments.Add(node);
            return node;
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
            if (IsInvalidContext(context)) return null;
            var node = new FlagAssignmentNode(context);
            _lastAssignmentsNode?.Assignments.Add(node);
            return node;
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
            var node = new MemoryAssignmentNode(context)
            {
                Address = (ExpressionNode) VisitExpr(context.expr()[0]),
                Value = (ExpressionNode) VisitExpr(context.expr()[1])
            };
            if (context.expr().Length > 2)
            {
                node.Lenght = (ExpressionNode) VisitExpr(context.expr()[2]);
            }
            _lastAssignmentsNode?.Assignments.Add(node);
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
            node.KeywordSpan = new TextSpan(context.SETUP());
            if (_lastTestSet != null)
            {
                _lastTestSet.Setup = node;
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

            var startExpr = (ExpressionNode) VisitExpr(context.expr()[0]);
            var stopExpr = context.expr().Length > 1 
                ? (ExpressionNode)VisitExpr(context.expr()[1]) 
                : null;
            return new InvokeCodeNode(context, startExpr, stopExpr);
        }

        #endregion

        #region Data block visitors

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

            // --- Create the node
            var node = _lastDataBlock = new DataBlockNode(context);
            if (_lastTestSet != null)
            {
                _lastTestSet.DataBlock = node;
            }

            base.VisitDataBlock(context);
            return node;
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
            if (IsInvalidContext(context)) return null;

            var node = new ValueMemberNode(context, (ExpressionNode)VisitExpr(context.expr()));
            _lastDataBlock?.DataMembers.Add(node);
            return node;
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

            var node = _lastMemoryPattern = new MemoryPatternMemberNode(context);
            _lastDataBlock?.DataMembers.Add(node);
            base.VisitMemPattern(context);
            return node;
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
            _lastMemoryPattern?.Patterns.Add(node);
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
            _lastMemoryPattern?.Patterns.Add(node);
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
            if (IsInvalidContext(context)) return null;
            var node = new TextPatternNode(context);
            _lastMemoryPattern?.Patterns.Add(node);
            return node;
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
            var node = _lastPortMock = new PortMockMemberNode(context, (ExpressionNode)VisitExpr(context.expr()));
            _lastDataBlock?.DataMembers.Add(node);
            base.VisitPortMock(context);
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
            var node = new PortPulseNode(context)
            {
                ValueExpr = (ExpressionNode) VisitExpr(context.expr()[0]),
                Pulse1Expr = (ExpressionNode) VisitExpr(context.expr()[1])
            };
            if (context.expr().Length > 2)
            {
                node.Pulse2Expr = (ExpressionNode)VisitExpr(context.expr()[2]);
                node.IsInterval = context.GetChild(4).GetText() == "..";
            }
            _lastPortMock?.Pulses.Add(node);
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

            var expr = (ExpressionNode)VisitOrExpr(context.GetChild(0) as Z80TestParser.OrExprContext);
            if (context.ChildCount == 1) return expr;

            return new ConditionalExpressionNode(context)
            {
                Condition = expr,
                TrueExpression = (ExpressionNode)VisitExpr(context.GetChild(2) as Z80TestParser.ExprContext),
                FalseExpression = (ExpressionNode)VisitExpr(context.GetChild(4) as Z80TestParser.ExprContext)
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

            var expr = VisitXorExpr(context.GetChild(0) as Z80TestParser.XorExprContext);

            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitXorExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.XorExprContext);
                expr = new BitwiseOrOperationNode(context)
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
                nextChildIndex += 2;
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

            var expr = VisitAndExpr(context.GetChild(0) as Z80TestParser.AndExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitAndExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.AndExprContext);
                expr = new BitwiseXorOperationNode(context)
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
                nextChildIndex += 2;
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

            var expr = VisitEquExpr(context.GetChild(0)
                as Z80TestParser.EquExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitEquExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.EquExprContext);
                expr = new BitwiseAndOperationNode(context)
                {
                    LeftOperand = (ExpressionNode)expr,
                    RightOperand = (ExpressionNode)rightExpr
                };
                nextChildIndex += 2;
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

            var expr = (ExpressionNode)VisitRelExpr(context.GetChild(0) as Z80TestParser.RelExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitRelExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.RelExprContext);
                var opToken = context.GetTokenText(nextChildIndex - 1);
                var equExpr = opToken == "=="
                    ? new EqualOperationNode(context)
                    : new NotEqualOperationNode(context) as BinaryOperationNode;
                equExpr.LeftOperand = expr;
                equExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = equExpr;
                nextChildIndex += 2;
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

            var expr = (ExpressionNode)VisitShiftExpr(context.GetChild(0) as Z80TestParser.ShiftExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitShiftExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.ShiftExprContext);
                var opToken = context.GetTokenText(nextChildIndex - 1);
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
                nextChildIndex += 2;
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

            var expr = (ExpressionNode)VisitAddExpr(context.GetChild(0) as Z80TestParser.AddExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitAddExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.AddExprContext);
                var opToken = context.GetTokenText(nextChildIndex - 1);
                var shiftExpr = opToken == "<<"
                    ? new ShiftLeftOperationNode(context)
                    : new ShiftRightOperationNode(context) as BinaryOperationNode;

                shiftExpr.LeftOperand = expr;
                shiftExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = shiftExpr;
                nextChildIndex += 2;
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

            var expr = (ExpressionNode)VisitMultExpr(context.GetChild(0) as Z80TestParser.MultExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitMultExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.MultExprContext);
                var opToken = context.GetTokenText(nextChildIndex - 1);
                var addExpr = opToken == "+"
                    ? new AddOperationNode(context)
                    : new SubtractOperationNode(context) as BinaryOperationNode;

                addExpr.LeftOperand = expr;
                addExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = addExpr;
                nextChildIndex += 2;
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

            var expr = (ExpressionNode)VisitUnaryExpr(context.GetChild(0) as Z80TestParser.UnaryExprContext);
            var nextChildIndex = 2;
            while (nextChildIndex < context.ChildCount)
            {
                var rightExpr = VisitUnaryExpr(context.GetChild(nextChildIndex)
                    as Z80TestParser.UnaryExprContext);
                var opToken = context.GetTokenText(nextChildIndex - 1);
                var multExpr = opToken == "*"
                    ? new MultiplyOperationNode(context)
                    : (opToken == "/"
                        ? new DivideOperationNode(context)
                        : new ModuloOperationNode(context) as BinaryOperationNode);

                multExpr.LeftOperand = expr;
                multExpr.RightOperand = (ExpressionNode)rightExpr;
                expr = multExpr;
                nextChildIndex += 2;
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

            var child0 = context.GetChild(0);
            if (child0 == null) return null;

            if (child0 is Z80TestParser.LiteralExprContext)
            {
                return VisitLiteralExpr(child0 as Z80TestParser.LiteralExprContext);
            }
            if (child0 is Z80TestParser.SymbolExprContext)
            {
                return VisitSymbolExpr(child0 as Z80TestParser.SymbolExprContext);
            }
            if (child0.GetText() == "+")
            {
                return new UnaryPlusNode(context)
                {
                    Operand = (ExpressionNode)VisitUnaryExpr(context.GetChild(1) as Z80TestParser.UnaryExprContext)
                };
            }
            if (child0.GetText() == "-")
            {
                return new UnaryMinusNode(context)
                {
                    Operand = (ExpressionNode)VisitUnaryExpr(context.GetChild(1) as Z80TestParser.UnaryExprContext)
                };
            }
            if (child0.GetText() == "~")
            {
                return new UnaryBitwiseNotNode(context)
                {
                    Operand = (ExpressionNode)VisitUnaryExpr(context.GetChild(1) as Z80TestParser.UnaryExprContext)
                };
            }
            if (child0.GetText() == "!")
            {
                return new UnaryLogicalNotNode(context)
                {
                    Operand = (ExpressionNode)VisitUnaryExpr(context.GetChild(1) as Z80TestParser.UnaryExprContext)
                };
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
            else if (token.StartsWith("0X"))
            {
                value = ushort.Parse(token.Substring(2), NumberStyles.HexNumber);
            }
            else if (token.EndsWith("H", StringComparison.OrdinalIgnoreCase))
            {
                value = (ushort)int.Parse(token.Substring(0, token.Length - 1),
                    NumberStyles.HexNumber);
            }
            // --- Binary literals
            else if (token.StartsWith("%"))
            {
                value = (ushort)Convert.ToInt32(token.Substring(1).Replace("_", ""), 2);
            }
            else if (token.StartsWith("0B"))
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
                value = (ushort)int.Parse(context.GetText());
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
            if (IsInvalidContext(context)) return null;

            return new IdentifierNode(context)
            {
                SymbolName = context.GetChild(0).GetText()
            };
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
            if (IsInvalidContext(context)) return null;

            return base.VisitRegisterSpec(context);
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