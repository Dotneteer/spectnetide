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
        public TestCasePlan(TestBlockPlan testBlock, List<ExpressionNode> paramValues, List<PortMockPlan> portMockPlans,
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
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the flag that indicates if machine is available
        /// </summary>
        /// <returns></returns>
        public bool IsMachineAvailable()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the value of the specified Z80 register
        /// </summary>
        /// <param name="regName">Register name</param>
        /// <returns>
        /// The register's current value
        /// </returns>
        public ushort GetRegisterValue(string regName)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the range of the machines memory from start to end
        /// </summary>
        /// <param name="start">Start address (inclusive)</param>
        /// <param name="end">End address (inclusive)</param>
        /// <returns>The memory section</returns>
        public byte[] GetMemorySection(ushort start, ushort end)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the range of memory reach values
        /// </summary>
        /// <param name="start">Start address (inclusive)</param>
        /// <param name="end">End address (inclusive)</param>
        /// <returns>The memory section</returns>
        public byte[] GetReachSection(ushort start, ushort end)
        {
            throw new System.NotImplementedException();
        }
    }
}