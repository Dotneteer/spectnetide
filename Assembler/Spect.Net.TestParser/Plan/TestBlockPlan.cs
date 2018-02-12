using System.Collections.Generic;
using Spect.Net.TestParser.SyntaxTree;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// Respresent a test block plan
    /// </summary>
    public class TestBlockPlan: IExpressionEvaluationContext
    {
        private bool _machineAvailable;

        public List<string> ParameterNames { get; } = new List<string>();

        /// <summary>
        /// Parent test set
        /// </summary>
        public TestSetPlan TestSet { get; }

        /// <summary>
        /// ID of the test block
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The span of the test block
        /// </summary>
        public TextSpan Span { get; }

        /// <summary>
        /// ID of the test category
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Disable the interrupt when running test code
        /// </summary>
        public bool? DisableInterrupt { get; set; }

        /// <summary>
        /// Test timeout in milliseconds
        /// </summary>
        public int? TimeoutValue { get; set; }

        /// <summary>
        /// Test case plans 
        /// </summary>
        public List<TestCasePlan> TestCases { get; } = new List<TestCasePlan>();

        /// <summary>
        /// The Arrange assignments of this test block
        /// </summary>
        public List<RunTimeAssignmentPlanBase> ArrangAssignments { get; } = new List<RunTimeAssignmentPlanBase>();

        /// <summary>
        /// Act of the test block
        /// </summary>
        public InvokePlanBase Act { get; set; }

        /// <summary>
        /// List of breakpoints
        /// </summary>
        public List<ExpressionNode> Breakpoints { get; } = new List<ExpressionNode>();

        /// <summary>
        /// List of assertions
        /// </summary>
        public List<ExpressionNode> Assertions { get; } = new List<ExpressionNode>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public TestBlockPlan(TestSetPlan testSet, string id, string category, TextSpan span)
        {
            Id = id;
            Category = category;
            Span = span;
            TestSet = testSet;
            _machineAvailable = false;
        }

        public void SignMachineAvailable()
        {
            _machineAvailable = true;
        }

        /// <summary>
        /// Checks if this testblock contains the specified parameter
        /// </summary>
        /// <param name="param">Parameter name</param>
        /// <returns>Trui, if the test block contains the parameter; otherwise, false</returns>
        public bool ContainsParameter(string param) => ParameterNames.Contains(param.ToUpperInvariant());

        /// <summary>
        /// Adds the specified parameter to the test block parameters
        /// </summary>
        /// <param name="param"></param>
        public void AddParameter(string param) => ParameterNames.Add(param.ToUpperInvariant());

        /// <summary>
        /// Gets the value of the specified symbol
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        public ExpressionValue GetSymbolValue(string symbol)
        {
            if (ParameterNames.Contains(symbol.ToUpperInvariant()))
            {
                // TODO: Update to get the real symbol value during run time
                return ExpressionValue.NonEvaluated;
            }

            return TestSet.GetSymbolValue(symbol);
        }

        /// <summary>
        /// Gets the flag that indicates if machine is available
        /// </summary>
        /// <returns></returns>
        public bool IsMachineAvailable() => _machineAvailable;

        /// <summary>
        /// Gets the value of the specified Z80 register
        /// </summary>
        /// <param name="regName">Register name</param>
        /// <returns>
        /// The register's current value
        /// </returns>
        public ushort GetRegisterValue(string regName) => TestSet.GetRegisterValue(regName);

        /// <summary>
        /// Gets the value of the specified Z80 flag
        /// </summary>
        /// <param name="flagName">Register name</param>
        /// <returns>
        /// The flags's current value
        /// </returns>
        public bool GetFlagValue(string flagName) => TestSet.GetFlagValue(flagName);

        /// <summary>
        /// Gets the range of the machines memory from start to end
        /// </summary>
        /// <param name="start">Start address (inclusive)</param>
        /// <param name="end">End address (inclusive)</param>
        /// <returns>The memory section</returns>
        public byte[] GetMemorySection(ushort start, ushort end) 
            => TestSet.GetMemorySection(start, end);

        /// <summary>
        /// Gets the range of memory reach values
        /// </summary>
        /// <param name="start">Start address (inclusive)</param>
        /// <param name="end">End address (inclusive)</param>
        /// <returns>The memory section</returns>
        public byte[] GetReachSection(ushort start, ushort end)
            => TestSet.GetReachSection(start, end);
    }
}