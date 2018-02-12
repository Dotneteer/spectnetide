using System;
using System.Collections.Generic;
using Spect.Net.Assembler.Assembler;
using Spect.Net.TestParser.SyntaxTree.Expressions;
using TextSpan = Spect.Net.TestParser.SyntaxTree.TextSpan;

namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// This class represents the execution plan of a TestSet
    /// </summary>
    public class TestSetPlan: IExpressionEvaluationContext
    {
        private readonly Dictionary<string, ExpressionValue> _dataEntries = 
            new Dictionary<string, ExpressionValue>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, PortMockPlan> _portMocks = 
            new Dictionary<string, PortMockPlan>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public TestSetPlan(string id, TextSpan span)
        {
            Id = id;
            Span = span;
        }

        /// <summary>
        /// Test set ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The span of the test set
        /// </summary>
        public TextSpan Span { get; }

        /// <summary>
        /// Type of the machine
        /// </summary>
        public MachineType? MachineType { get; set; }

        /// <summary>
        /// The compiled Z80 Assembler output
        /// </summary>
        public AssemblerOutput CodeOutput { get; set; }

        /// <summary>
        /// Disable the interrupt when running test code
        /// </summary>
        public bool DisableInterrupt { get; set; } = false;

        /// <summary>
        /// Test timeout in milliseconds
        /// </summary>
        public int TimeoutValue { get; set; } = 100;

        /// <summary>
        /// The Init assignments of this test set
        /// </summary>
        public List<AssignmentPlanBase> InitAssignments { get; } = new List<AssignmentPlanBase>();

        /// <summary>
        /// Setup plan
        /// </summary>
        public InvokePlanBase Setup { get; set; }

        /// <summary>
        /// Cleanup plan
        /// </summary>
        public InvokePlanBase Cleanup { get; set; }

        /// <summary>
        /// Test blocks nested into this test set
        /// </summary>
        public List<TestBlockPlan> TestBlocks { get; } = new List<TestBlockPlan>();

        /// <summary>
        /// Checks if this test set contains the specified symbol
        /// </summary>
        /// <param name="symbol">Symbol to check</param>
        /// <returns>Tru, if the test set contains the symbol; otherwise, false</returns>
        public bool ContainsSymbol(string symbol) 
            => _dataEntries.ContainsKey(symbol) 
               || CodeOutput.Symbols.ContainsKey(symbol);

        /// <summary>
        /// Sets the data member with the specified id and given value
        /// </summary>
        /// <param name="id">Data member ID</param>
        /// <param name="value">Data memer value</param>
        public void SetDataMember(string id, ExpressionValue value)
        {
            _dataEntries[id] = value;
        }

        /// <summary>
        /// Gets the value of the specified data member
        /// </summary>
        /// <param name="id">Data member ID</param>
        /// <returns>The named data merber, if found; otherwise, null</returns>
        public ExpressionValue GetDataMember(string id) => 
            _dataEntries.TryGetValue(id, out var value) 
                ? value 
                : null;

        /// <summary>
        /// Sets the port mock with the specified id and given value
        /// </summary>
        /// <param name="id">Data member ID</param>
        /// <param name="value">Data memer value</param>
        public void SetPortMock(string id, PortMockPlan value)
        {
            _portMocks[id] = value;
        }

        /// <summary>
        /// Gets the value of the specified port mock
        /// </summary>
        /// <param name="id">Data member ID</param>
        /// <returns>The named data merber, if found; otherwise, null</returns>
        public PortMockPlan GetPortMock(string id) =>
            _portMocks.TryGetValue(id, out var value)
                ? value
                : null;

        /// <summary>
        /// Gets the value of the specified symbol
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        public ExpressionValue GetSymbolValue(string symbol)
        {
            // --- First, find in the data section of the test set
            if (_dataEntries.TryGetValue(symbol, out var value))
            {
                return value;
            }

            // --- Second, find within the assembly output symbols
            if (CodeOutput?.Symbols == null) return null;
            return CodeOutput.Symbols.TryGetValue(symbol, out var ushortValue) 
                ? new ExpressionValue(ushortValue) 
                : null;
        }

        /// <summary>
        /// Gets the flag that indicates if machine is available
        /// </summary>
        /// <returns></returns>
        public bool IsMachineAvailable() => false;

        /// <summary>
        /// Gets the value of the specified Z80 register
        /// </summary>
        /// <param name="regName">Register name</param>
        /// <returns>
        /// The register's current value
        /// </returns>
        public ushort GetRegisterValue(string regName) => 0;

        /// <summary>
        /// Gets the value of the specified Z80 flag
        /// </summary>
        /// <param name="flagName">Register name</param>
        /// <returns>
        /// The flags's current value
        /// </returns>
        public bool GetFlagValue(string flagName) => false;

        /// <summary>
        /// Gets the range of the machines memory from start to end
        /// </summary>
        /// <param name="start">Start address (inclusive)</param>
        /// <param name="end">End address (inclusive)</param>
        /// <returns>The memory section</returns>
        public byte[] GetMemorySection(ushort start, ushort end) => null;

        /// <summary>
        /// Gets the range of memory reach values
        /// </summary>
        /// <param name="start">Start address (inclusive)</param>
        /// <param name="end">End address (inclusive)</param>
        /// <returns>The memory section</returns>
        public byte[] GetReachSection(ushort start, ushort end) => null;
    }
}