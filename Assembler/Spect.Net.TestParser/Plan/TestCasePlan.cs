using System.Collections.Generic;
using Spect.Net.TestParser.SyntaxTree;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// Represents a single test case
    /// </summary>
    public class TestCasePlan : IExpressionEvaluationContext
    {
        /// <summary>
        /// Machine context for evaluation
        /// </summary>
        public IMachineContext MachineContext { get; set; }

        /// <summary>
        /// The parameter values of test expressions
        /// </summary>
        public List<ExpressionNode> ParamValues { get; }

        /// <summary>
        /// The parent test block
        /// </summary>
        public TestBlockPlan TestBlock { get; }

        /// <summary>
        /// The port mocks for the test case
        /// </summary>
        public List<PortMockPlan> PortMockPlans { get; }

        /// <summary>
        /// The title of the test case
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Span of the test case
        /// </summary>
        public TextSpan Span { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TestCasePlan(TestBlockPlan testBlock, List<ExpressionNode> paramValues,
            List<PortMockPlan> portMockPlans,
            string title, TextSpan span)
        {
            ParamValues = paramValues;
            PortMockPlans = portMockPlans;
            Title = title;
            Span = span;
            TestBlock = testBlock;
        }

        /// <summary>
        /// Gets the value of the specified symbol
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        public ExpressionValue GetSymbolValue(string symbol)
        {
            var index = TestBlock.ParameterNames.IndexOf(symbol.ToUpperInvariant());
            if (index < 0) return TestBlock.GetSymbolValue(symbol);

            return index >= ParamValues.Count 
                ? ExpressionValue.Error 
                : ParamValues[index].Evaluate(TestBlock);
        }

        /// <summary>
        /// Gets the machine context to evaluate registers, flags, and memory
        /// </summary>
        /// <returns></returns>
        public IMachineContext GetMachineContext() => MachineContext;
    }
}