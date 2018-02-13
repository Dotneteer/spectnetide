using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using Spect.Net.Assembler.Assembler;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.Plan;
using Spect.Net.TestParser.SyntaxTree;
using Spect.Net.TestParser.SyntaxTree.DataBlock;
using Spect.Net.TestParser.SyntaxTree.Expressions;
using Spect.Net.TestParser.SyntaxTree.TestSet;
using TextSpan = Spect.Net.TestParser.SyntaxTree.TextSpan;

namespace Spect.Net.TestParser.Compiler
{
    /// <summary>
    /// This class implements the Z80 Test Compiler
    /// </summary>
    public class Z80TestCompiler
    {
        /// <summary>
        /// The file name of a direct text compilation
        /// </summary>
        public const string NOFILE_ITEM = "#";

        /// <summary>
        /// The default timeout value
        /// </summary>
        public const int DEFAULT_TIMEOUT = 100;

        /// <summary>
        /// The default folder for Z80 Assembler source files
        /// </summary>
        public string DefaultSourceFolder { get; set; }

        /// <summary>
        /// Compiles the test specified test file
        /// </summary>
        /// <param name="filename">Test file name</param>
        /// <returns></returns>
        public TestFilePlan CompileFile(string filename)
        {
            var fi = new FileInfo(filename);
            var fullName = fi.FullName;
            var sourceText = File.ReadAllText(fullName);
            var testFilePlan = new TestFilePlan(fullName);
            DoCompile(testFilePlan, sourceText);
            return testFilePlan;
        }

        /// <summary>
        /// Compiles the test specified test file
        /// </summary>
        /// <returns></returns>
        public TestFilePlan Compile(string sourceText)
        {
            var testFilePlan = new TestFilePlan(NOFILE_ITEM);
            DoCompile(testFilePlan, sourceText);
            return testFilePlan;
        }

        /// <summary>
        /// Carries out the compilation of the specified source text into the given plan
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="sourceText">Test language source text</param>
        private void DoCompile(TestFilePlan plan, string sourceText)
        {
            // --- Init the compilation process
            if (sourceText == null)
            {
                throw new ArgumentNullException(nameof(sourceText));
            }

            if (ExecuteParse(plan, sourceText, out var testSetNodes))
            {
                EmitPlan(plan, testSetNodes);
            }
        }

        /// <summary>
        /// Executes the parse phase of the compilation
        /// </summary>
        /// <param name="plan">Test plan</param>
        /// <param name="sourcetext">Source text</param>
        /// <param name="testSetNodes">TestSetNode as the result of compilation</param>
        /// <returns>True, if compilation successful; otherwise, false</returns>
        private bool ExecuteParse(TestFilePlan plan, string sourcetext, out List<TestSetNode> testSetNodes)
        {
            // --- Parse the source text
            var inputStream = new AntlrInputStream(sourcetext);
            var lexer = new Z80TestLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new Z80TestParser(tokenStream);
            var context = parser.compileUnit();
            var visitor = new Z80TestVisitor();
            visitor.Visit(context);
            testSetNodes = visitor.Compilation.TestSets;

            // --- Collect syntax errors
            foreach (var error in parser.SyntaxErrors)
            {
                ReportError(plan, error);
            }

            return parser.SyntaxErrors.Count == 0;
        }

        /// <summary>
        /// Emits the test plan from the passed syntax nodes
        /// </summary>
        /// <param name="plan">Test plan</param>
        /// <param name="testSetNodes">TesSet syntax nodes</param>
        private void EmitPlan(TestFilePlan plan, IEnumerable<TestSetNode> testSetNodes)
        {
            foreach (var testSetNode in testSetNodes)
            {
                plan.TestSetPlans.Add(VisitTestSet(plan, testSetNode));
            }
        }

        /// <summary>
        /// Visits a single test plan
        /// </summary>
        /// <param name="plan">Test plan to emit</param>
        /// <param name="node">TestSetNode to use</param>
        private TestSetPlan VisitTestSet(TestFilePlan plan, TestSetNode node)
        {
            var testSetPlan = new TestSetPlan(node.TestSetId, node.Span);
            VisitSourceContext(plan, testSetPlan, node.SourceContext);
            VisitTestOptions(plan, testSetPlan, node.TestOptions);
            VisitDataBlock(plan, testSetPlan, node.DataBlock);
            if (node.Init != null)
            {
                foreach (var asgn in node.Init.Assignments)
                {
                    var asgnPlan = VisitAssignment(plan, testSetPlan, asgn);
                    if (asgnPlan != null)
                    {
                        testSetPlan.InitAssignments.Add(asgnPlan);
                    }
                }
            }
            if (node.Setup != null)
            {
                testSetPlan.Setup = VisitInvoke(plan, testSetPlan, node.Setup);
            }
            if (node.Cleanup != null)
            {
                testSetPlan.Cleanup = VisitInvoke(plan, testSetPlan, node.Cleanup);
            }

            foreach (var block in node.TestBlocks)
            {
                var blockPlan = VisitTestBlock(plan, testSetPlan, block);
                if (blockPlan != null)
                {
                    if (testSetPlan.TestBlocks.Any(tb => 
                        string.Compare(tb.Id, blockPlan.Id, StringComparison.InvariantCultureIgnoreCase) == 0))
                    {
                        ReportError(Errors.T0007, plan, block.TestIdSpan, block.TestId);
                        continue;
                    }
                    testSetPlan.TestBlocks.Add(blockPlan);
                }
            }
            return testSetPlan;
        }

        /// <summary>
        /// Visits a test block
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="testSetPlan"></param>
        /// <param name="block"></param>
        /// <returns>Test block plan</returns>
        private TestBlockPlan VisitTestBlock(TestFilePlan plan, TestSetPlan testSetPlan, TestBlockNode block)
        {
            var testBlock = new TestBlockPlan(testSetPlan, block.TestId, block.Category,block.Span);
            if (block.TestOptions != null)
            {
                VisitTestOptions(plan, testSetPlan, block.TestOptions, out var nonmi, out var timeout);
                testBlock.DisableInterrupt = nonmi;
                testBlock.TimeoutValue = timeout;
            }
            VisitTestParameters(plan, testBlock, block.Params);
            VisitTestCases(plan, testBlock, block.Cases);
            var invoke = VisitInvoke(plan, testSetPlan, block.Act);
            if (invoke != null)
            {
                testBlock.Act = invoke;
            }
            VisitArrange(plan, testBlock, block.Arrange);
            if (block.Breakpoints != null)
            {
                VisitBreakPoints(plan, testBlock, block.Breakpoints);
            }
            testBlock.MachineContext = new CompileTimeMachineContext();
            VisitAssert(plan, testBlock, block.Assert);
            return testBlock;
        }

        /// <summary>
        /// Visit the assert section of the block
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testBlock">TestBlockPlan to visit</param>
        /// <param name="assert">Asser syntax node</param>
        private void VisitAssert(TestFilePlan plan, TestBlockPlan testBlock, AssertNode assert)
        {
            if (assert == null) return;
            foreach (var expr in assert.Expressions)
            {
                var value = Eval(plan, testBlock, expr, true);
                if (value == null) continue;
                testBlock.Assertions.Add(expr);
            }
        }

        /// <summary>
        /// Visit the arrange section of the block
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testBlock">TestBlockPlan to visit</param>
        /// <param name="arrange">Arrange syntax node</param>
        private void VisitArrange(TestFilePlan plan, TestBlockPlan testBlock, AssignmentsNode arrange)
        {
            if (arrange == null) return;
            foreach (var asgn in arrange.Assignments)
            {
                var asgnPlan = VisitAssignment(plan, testBlock, asgn);
                if (asgnPlan != null)
                {
                    testBlock.ArrangAssignments.Add(asgnPlan);
                }
            }
        }

        /// <summary>
        /// Visits the test cases of the block
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testBlock">TestBlockPlan to visit</param>
        /// <param name="cases">Test cases syntax node</param>
        private void VisitTestCases(TestFilePlan plan, TestBlockPlan testBlock, List<TestCaseNode> cases)
        {
            if (cases == null) return;
            var testIndex = 0;
            foreach (var blockCase in cases)
            {
                testIndex++;
                var exprs = new List<ExpressionNode>();
                foreach (var expr in blockCase.Expressions)
                {
                    // --- We intentionally use the evaluation context of the test set, because 
                    // --- test case expressions must not contain parameter identifiers
                    var value = Eval(plan, testBlock.TestSet, expr, true);
                    if (value == null) continue;
                    exprs.Add(expr);
                }
                if (blockCase.Expressions.Count != testBlock.ParameterNames.Count)
                {
                    ReportError(Errors.T0009, plan, blockCase.CaseKeywordSpan, testIndex, 
                        blockCase.Expressions.Count, testBlock.ParameterNames.Count);
                }

                var portMocks = new List<PortMockPlan>();

                foreach (var portMockId in blockCase.PortMocks)
                {
                    var portMockPlan = testBlock.TestSet.GetPortMock(portMockId.Id);
                    if (portMockPlan == null)
                    {
                        ReportError(Errors.T0010, plan, portMockId.Span, portMockId.Id);
                        continue;
                    }
                    portMocks.Add(portMockPlan);
                }

                testBlock.TestCases.Add(new TestCasePlan(testBlock, exprs, portMocks, blockCase.TestCaseText, blockCase.Span));
            }
        }

        /// <summary>
        /// Visits the parameters of a test block
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testBlock">TestBlockPlan to visit</param>
        /// <param name="paramsNode">Parameters syntax node</param>
        private void VisitTestParameters(TestFilePlan plan, TestBlockPlan testBlock, ParamsNode paramsNode)
        {
            if (paramsNode == null) return;
            foreach (var param in paramsNode.Ids)
            {
                if (testBlock.ContainsParameter(param.Id))
                {
                    ReportError(Errors.T0008, plan, param.Span, param.Id);
                    continue;
                }

                testBlock.AddParameter(param.Id);
            }
        }

        /// <summary>
        /// Visits breakpoint of the test block
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testBlock">TestBlockPlan to visit</param>
        /// <param name="breakpoints">Breakpoints syntax node</param>
        private void VisitBreakPoints(TestFilePlan plan, TestBlockPlan testBlock, BreakpointsNode breakpoints)
        {
            if (breakpoints == null) return;
            foreach (var expr in breakpoints.Expressions)
            {
                var value = Eval(plan, testBlock, expr, true);
                if (value == null) continue;

                testBlock.Breakpoints.Add(expr);
            }
        }

        /// <summary>
        /// Visit an invoke node
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testSetPlan">TestSetPlan to visit</param>
        /// <param name="invokeNode">Invoke syntax node</param>
        /// <returns>Invoke plan</returns>
        private InvokePlanBase VisitInvoke(TestFilePlan plan, TestSetPlan testSetPlan, InvokeCodeNode invokeNode)
        {
            // --- Get start address
            var start = Eval(plan, testSetPlan, invokeNode.StartExpr);
            if (start == null) return null;

            if (invokeNode.IsCall)
            {
                return new CallPlan(start.AsWord());
            }

            if (invokeNode.IsHalt)
            {
                return new StartPlan(start.AsWord(), null);
            }

            // --- Get Stop address
            var stop = Eval(plan, testSetPlan, invokeNode.StopExpr);
            return stop == null 
                ? null 
                : new StartPlan(start.AsWord(), stop.AsWord());
        }

        /// <summary>
        /// Visits an assignment of a TestSet
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testSetPlan">TestSetPlan to visit</param>
        /// <param name="asgn">Assignment syntax node</param>
        /// <returns>Assignment plan</returns>
        private AssignmentPlanBase VisitAssignment(TestFilePlan plan, TestSetPlan testSetPlan, AssignmentNode asgn)
        {
            if (asgn is RegisterAssignmentNode regAsgn)
            {
                var value = Eval(plan, testSetPlan, regAsgn.Expr);
                return value != null 
                    ? new RegisterAssignmentPlan(regAsgn.RegisterName, value.AsWord())
                    : null;
            }

            if (asgn is FlagAssignmentNode flagAsgn)
            {
                return new FlagAssignmentPlan(flagAsgn.FlagName);
            }

            if (asgn is MemoryAssignmentNode memAsgn)
            {
                var address = Eval(plan, testSetPlan, memAsgn.Address);
                var value = Eval(plan, testSetPlan, memAsgn.Value);
                if (address == null || value == null) return null;
                ExpressionValue length = null;
                if (memAsgn.Length != null)
                {
                    length = Eval(plan, testSetPlan, memAsgn.Length);
                    if (length == null) return null;
                }

                return length == null 
                    ? new MemoryAssignmentPlan(address.AsWord(), value.AsByteArray()) 
                    : new MemoryAssignmentPlan(address.AsWord(), value.AsByteArray().Take(length.AsWord()).ToArray());
            }

            return null;
        }

        /// <summary>
        /// Visits an assignment of a TestBlock
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testBlockPlan">TestSetPlan to visit</param>
        /// <param name="asgn">Assignment syntax node</param>
        /// <returns>Assignment plan</returns>
        private RunTimeAssignmentPlanBase VisitAssignment(TestFilePlan plan, TestBlockPlan testBlockPlan, AssignmentNode asgn)
        {
            if (asgn is RegisterAssignmentNode regAsgn)
            {
                var value = Eval(plan, testBlockPlan, regAsgn.Expr, true);
                return value != null
                    ? new RunTimeRegisterAssignmentPlan(regAsgn.RegisterName, regAsgn.Expr)
                    : null;
            }

            if (asgn is FlagAssignmentNode flagAsgn)
            {
                return new RunTimeFlagAssignmentPlan(flagAsgn.FlagName);
            }

            if (asgn is MemoryAssignmentNode memAsgn)
            {
                var address = Eval(plan, testBlockPlan, memAsgn.Address, true);
                var value = Eval(plan, testBlockPlan, memAsgn.Value, true);
                if (address == null || value == null) return null;
                if (memAsgn.Length != null)
                {
                    if (Eval(plan, testBlockPlan, memAsgn.Length, true) == null) return null;
                }
                return new RunTimeMemoryAssignmentPlan(memAsgn.Address, memAsgn.Value, memAsgn.Length);
            }

            return null;
        }

        /// <summary>
        /// Visit a data block
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testSetPlan">TestSetPlan to visit</param>
        /// <param name="dataBlock">DataBlock syntax node</param>
        private void VisitDataBlock(TestFilePlan plan, TestSetPlan testSetPlan, DataBlockNode dataBlock)
        {
            if (dataBlock == null) return;
            foreach (var dataMember in dataBlock.DataMembers)
            {
                if (dataMember is ValueMemberNode valueMember)
                {
                    VisitValueMember(plan, testSetPlan, valueMember);
                }
                else if (dataMember is MemoryPatternMemberNode mempatMember)
                {
                    VisitMemoryPatternMember(plan, testSetPlan, mempatMember);
                }
                else if (dataMember is PortMockMemberNode portMockMember)
                {
                    VisitPortMockMember(plan, testSetPlan, portMockMember);
                }
            }
        }

        /// <summary>
        /// Visit a port mock member
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testSetPlan">TestSetPlan to visit</param>
        /// <param name="portMockMember">Port mock syntax node</param>
        private void VisitPortMockMember(TestFilePlan plan, TestSetPlan testSetPlan, PortMockMemberNode portMockMember)
        {
            // --- Check ID duplication
            var id = portMockMember.Id;
            if (testSetPlan.ContainsSymbol(id))
            {
                ReportError(Errors.T0006, plan, portMockMember.IdSpan, id);
                return;
            }

            // --- Get port address
            var portAddress = Eval(plan, testSetPlan, portMockMember.Expr);
            var portMock = portAddress != null ? new PortMockPlan(portAddress.AsWord()) : null;

            // --- Get pulses
            var pulsesOk = true;
            var nextPulseStart = 0L;
            foreach (var pulse in portMockMember.Pulses)
            {
                // --- Get pulse expression values
                var value = Eval(plan, testSetPlan, pulse.ValueExpr);
                var pulse1 = Eval(plan, testSetPlan, pulse.Pulse1Expr);
                if (value == null || pulse1 == null)
                {
                    pulsesOk = false;
                    continue;
                }

                ExpressionValue pulse2 = null;
                if (pulse.Pulse2Expr != null)
                {
                    pulse2 = Eval(plan, testSetPlan, pulse.Pulse2Expr);
                    if (pulse2 == null)
                    {
                        pulsesOk = false;
                        continue;
                    }
                }

                // --- Create a new pulse
                PortPulsePlan pulsePlan;
                if (pulse2 == null)
                {
                    // --- We have only value and length
                    pulsePlan = new PortPulsePlan(value.AsByte(), nextPulseStart,
                        nextPulseStart + pulse1.AsNumber() - 1);

                }
                else
                {
                    // --- We have pulse value, start, and end
                    pulsePlan = new PortPulsePlan(value.AsByte(), pulse1.AsNumber(),
                        pulse.IsInterval ? pulse2.AsNumber() : pulse1.AsNumber() + pulse2.AsNumber() - 1);
                }
                nextPulseStart = pulsePlan.EndTact + 1;
                portMock?.AddPulse(pulsePlan);
            }

            // --- Create the entire port mock
            if (portMock != null && pulsesOk)
            {
                testSetPlan.SetPortMock(id, portMock);
            }
        }

        /// <summary>
        /// Visits a memory pattern member
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testSetPlan">TestSetPlan to visit</param>
        /// <param name="mempatMember">Memory pattern member syntax node</param>
        private void VisitMemoryPatternMember(TestFilePlan plan, TestSetPlan testSetPlan, MemoryPatternMemberNode mempatMember)
        {
            // --- Check ID duplication
            var id = mempatMember.Id;
            if (testSetPlan.ContainsSymbol(id))
            {
                ReportError(Errors.T0006, plan, mempatMember.IdSpan, id);
                return;
            }

            // --- Evaluate byte array
            var bytes = new List<byte>();
            var errorFound = false;
            foreach (var mempat in mempatMember.Patterns)
            {
                if (mempat is BytePatternNode bytePattern)
                {
                    foreach (var byteExpr in bytePattern.Bytes)
                    {
                        var value = Eval(plan, testSetPlan, byteExpr);
                        if (value != null)
                        {
                            bytes.Add((byte)value.AsNumber());
                        }
                        else
                        {
                            errorFound = true;
                        }
                    }
                }
                else if (mempat is WordPatternNode wordPattern)
                {
                    foreach (var byteExpr in wordPattern.Words)
                    {
                        var value = Eval(plan, testSetPlan, byteExpr);
                        if (value != null)
                        {
                            var word = value.AsWord();
                            bytes.Add((byte)word);
                            bytes.Add((byte)(word >> 8));
                        }
                        else
                        {
                            errorFound = true;
                        }
                    }
                }
                else if (mempat is TextPatternNode textPattern)
                {
                    foreach (var val in SyntaxHelper.SpectrumStringToBytes(textPattern.String))
                    {
                        bytes.Add(val);
                    }
                }
            }

            if (errorFound) return;
            testSetPlan.SetDataMember(id, new ExpressionValue(bytes.ToArray())); 
        }

        /// <summary>
        /// Visits a value data member
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testSetPlan">TestSetPlan to visit</param>
        /// <param name="valueMember">Value member syntax node</param>
        private void VisitValueMember(TestFilePlan plan, TestSetPlan testSetPlan, ValueMemberNode valueMember)
        {
            // --- Check ID duplication
            var id = valueMember.Id;
            if (testSetPlan.ContainsSymbol(id))
            {
                ReportError(Errors.T0006, plan, valueMember.IdSpan, id);
                return;
            }

            // --- Evaluate the expression
            var value = Eval(plan, testSetPlan, valueMember.Expr);
            if (value != null)
            {
                testSetPlan.SetDataMember(id, value);
            }
        }

        /// <summary>
        /// Visits test set options
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testSetPlan">TestSetPlan to visit</param>
        /// <param name="testOptions">TestOptions syntax node</param>
        private void VisitTestOptions(TestFilePlan plan, TestSetPlan testSetPlan, TestOptionsNode testOptions)
        {
            if (testOptions == null) return;
            VisitTestOptions(plan, testSetPlan, testOptions, out var nonmi, out var timeout);
            testSetPlan.DisableInterrupt = nonmi ?? false;
            testSetPlan.TimeoutValue = timeout ?? DEFAULT_TIMEOUT;
        }

        /// <summary>
        /// Vistis test set and test node options
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testSetPlan">TestSetPlan to visit</param>
        /// <param name="testOptions">TestOptions syntax node</param>
        /// <param name="nonmi">NONMI value</param>
        /// <param name="timeout">TIMEOUT value</param>
        private void VisitTestOptions(TestFilePlan plan, TestSetPlan testSetPlan, TestOptionsNode testOptions, out bool? nonmi, out int? timeout)
        {
            // --- Set default values
            nonmi = null;
            timeout = null;
            if (testOptions?.Options == null) return;

            // --- Process options
            var nonmiFound = false;
            var timeoutFound = false;
            foreach (var option in testOptions.Options)
            {
                if (option is NoNmiTestOptionNode nonmiNode)
                {
                    if (nonmiFound)
                    {
                        ReportError(Errors.T0005, plan, nonmiNode.Span, "NONMI");
                        return;
                    }
                    nonmiFound = true;
                    nonmi = true;
                }
                else if (option is TimeoutTestOptionNode timeoutNode)
                {
                    if (timeoutFound)
                    {
                        ReportError(Errors.T0005, plan, timeoutNode.Span, "TIMEOUT");
                        return;
                    }
                    timeoutFound = true;
                    var value = Eval(plan, testSetPlan, timeoutNode.Expr);
                    if (value != null)
                    {
                        timeout = (int)value.AsNumber();
                    }
                }
            }
        }

        /// <summary>
        /// Visits the source context of a test set
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testSetPlan">TestSetPlan to visit</param>
        /// <param name="sourceContext">Machine context</param>
        private void VisitSourceContext(TestFilePlan plan, TestSetPlan testSetPlan, SourceContextNode sourceContext)
        {
            if (sourceContext == null) return;

            // --- Prepare predefined symbols for Z80 compilation
            var options = new AssemblerOptions();
            foreach (var symbol in sourceContext.Symbols)
            {
                options.PredefinedSymbols.Add(symbol.Id);
            }
            var assembler = new Z80Assembler();

            // --- Check filename existence
            var filename = sourceContext.SourceFile;
            if (!Path.IsPathRooted(filename))
            {
                filename = Path.Combine(DefaultSourceFolder ?? "", filename);
            }
            if (!File.Exists(filename))
            {
                ReportError(Errors.T0003, plan, sourceContext.SourceFileSpan, filename);
                return;
            }

            // --- Compile the Z80 source file
            var output = assembler.CompileFile(filename, options);
            if (output.ErrorCount == 0)
            {
                testSetPlan.CodeOutput = output;
                return;
            }

            // --- Issue cZ80 ompilation error
            ReportError(Errors.T0004, plan, sourceContext.SourceFileSpan, filename, output.ErrorCount);
        }

        #region Compile time context

        /// <summary>
        /// We use this class to emulate that machine context is available during arrange and act operations
        /// </summary>
        private class CompileTimeMachineContext : IMachineContext
        {
            /// <summary>
            /// Signs if this is a compile time context
            /// </summary>
            public bool IsCompileTimeContext => true;

            /// <summary>
            /// Gets the value of the specified Z80 register
            /// </summary>
            /// <param name="regName">Register name</param>
            /// <returns>
            /// The register's current value
            /// </returns>
            public ushort GetRegisterValue(string regName)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets the value of the specified Z80 flag
            /// </summary>
            /// <param name="flagName">Register name</param>
            /// <returns>
            /// The flags's current value
            /// </returns>
            public bool GetFlagValue(string flagName)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets the range of the machines memory from start to end
            /// </summary>
            /// <param name="start">Start address (inclusive)</param>
            /// <param name="end">End address (inclusive)</param>
            /// <returns>The memory section</returns>
            public byte[] GetMemorySection(ushort start, ushort end)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets the range of memory reach values
            /// </summary>
            /// <param name="start">Start address (inclusive)</param>
            /// <param name="end">End address (inclusive)</param>
            /// <returns>The memory section</returns>
            public byte[] GetReachSection(ushort start, ushort end)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="sourceItem">Source item of the expression</param>
        /// <param name="evalContext">TestSetPlan that holds the expression</param>
        /// <param name="expr">Expression to evaluate</param>
        /// <param name="checkOnly">Check only if the expression could be evaluated</param>
        /// <returns>
        /// Null, if the expression cannot be evaluated, or evaluation 
        /// results an error (e.g. divide by zero)
        /// </returns>
        public ExpressionValue Eval(TestFilePlan sourceItem, IExpressionEvaluationContext evalContext, ExpressionNode expr, bool checkOnly = false)
        {
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }
            if (!expr.ReadyToEvaluate(evalContext))
            {
                ReportError(Errors.T0201, sourceItem, expr.Span);
                return null;
            }
            var result = expr.Evaluate(evalContext, checkOnly);
            if (expr.EvaluationError == null) return result;

            ReportError(Errors.T0200, sourceItem, expr.Span, expr.EvaluationError);
            return null;
        }

        /// <summary>
        /// Translates a Z80AsmParserErrorInfo instance into an error
        /// </summary>
        /// <param name="sourceItem">
        /// Source file information, to allow the error to track the filename the error ocurred in
        /// </param>
        /// <param name="error">Error information</param>
        private static void ReportError(TestFilePlan sourceItem, Z80TestParserErrorInfo error)
        {
            sourceItem.Errors.Add(new TestCompilerErrorInfo(sourceItem.Filename, error));
        }

        /// <summary>
        /// Reports the specified error
        /// </summary>
        /// <param name="errorCode">Code of error</param>
        /// <param name="sourceItem"></param>
        /// <param name="span">Span associated with the error</param>
        /// <param name="parameters">Optiona error message parameters</param>
        private static void ReportError(string errorCode, TestFilePlan sourceItem, TextSpan span, params object[] parameters)
        {
            sourceItem.Errors.Add(new TestCompilerErrorInfo(sourceItem.Filename, errorCode, 
                span.StartLine, span.StartColumn, parameters));
        }

        #endregion Helpers
    }
}