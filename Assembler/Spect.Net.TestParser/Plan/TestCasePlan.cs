using System.Collections.Generic;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// Represents a single test case
    /// </summary>
    public class TestCasePlan
    {
        /// <summary>
        /// The parameter values of test expressions
        /// </summary>
        public List<ExpressionNode> ParamValues { get; }

        /// <summary>
        /// The port mocks for the test case
        /// </summary>
        public List<PortMockPlan> PortMockPlans { get; }

        /// <summary>
        /// The title of the test case
        /// </summary>
        public string Title { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TestCasePlan(List<ExpressionNode> paramValues, List<PortMockPlan> portMockPlans, string title)
        {
            ParamValues = paramValues;
            PortMockPlans = portMockPlans;
            Title = title;
        }
    }
}