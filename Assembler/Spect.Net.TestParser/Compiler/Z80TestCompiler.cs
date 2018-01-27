using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;
using Spect.Net.Assembler.Assembler;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.Plan;
using Spect.Net.TestParser.SyntaxTree;
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
            var testSetPlan = new TestSetPlan();
            VisitMachineContext(plan, testSetPlan, node.MachineContext);
            VisitSourceContext(plan, testSetPlan, node.SourceContext);
            VisitTestOptions(plan, testSetPlan, node.TestOptions);
            return testSetPlan;
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
            testSetPlan.DisableInterrupt = nonmi;
            testSetPlan.TimeoutValue = timeout;
        }

        /// <summary>
        /// Vistis test set and test node options
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testSetPlan">TestSetPlan to visit</param>
        /// <param name="testOptions">TestOptions syntax node</param>
        /// <param name="nonmi">NONMI value</param>
        /// <param name="timeout">TIMEOUT value</param>
        private void VisitTestOptions(TestFilePlan plan, TestSetPlan testSetPlan, TestOptionsNode testOptions, out bool nonmi, out int timeout)
        {
            // --- Set default values
            nonmi = false;
            timeout = 100;
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
                    var value = EvalImmediate(plan, testSetPlan, timeoutNode.Expr);
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

        /// <summary>
        /// Visits the machine context of a test set
        /// </summary>
        /// <param name="plan">Test file plan</param>
        /// <param name="testSetPlan">TestSetPlan to visit</param>
        /// <param name="machineContext">machine context</param>
        private void VisitMachineContext(TestFilePlan plan, TestSetPlan testSetPlan, MachineContextNode machineContext)
        {
            if (machineContext == null) return;
            switch (machineContext.Id.ToUpper())
            {
                case "SPECTRUM48":
                    testSetPlan.MachineType = MachineType.Spectrum48;
                    return;
                case "SPECTRUM128":
                    testSetPlan.MachineType = MachineType.Spectrum128;
                    return;
                case "SPECTRUMP3":
                    testSetPlan.MachineType = MachineType.SpectrumP3;
                    return;
                case "NEXT":
                    testSetPlan.MachineType = MachineType.Next;
                    return;
            }
            ReportError(Errors.T0002, plan, machineContext.IdSpan);
        }

        #region Helpers

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="sourceItem">Source item of the expression</param>
        /// <param name="testSetPlan">TestSetPlan that holds the expression</param>
        /// <param name="expr">Expression to evaluate</param>
        /// <returns>
        /// Null, if the expression cannot be evaluated, or evaluation 
        /// results an error (e.g. divide by zero)
        /// </returns>
        public ExpressionValue EvalImmediate(TestFilePlan sourceItem, TestSetPlan testSetPlan, ExpressionNode expr)
        {
            if (expr == null)
            {
                throw new ArgumentNullException(nameof(expr));
            }
            if (!expr.ReadyToEvaluate(testSetPlan))
            {
                ReportError(Errors.T0201, sourceItem, expr.Span);
                return null;
            }
            var result = expr.Evaluate(testSetPlan);
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