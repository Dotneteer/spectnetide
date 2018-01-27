using System;
using System.Collections.Generic;
using Spect.Net.Assembler.Assembler;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// This class represents the execution plan of a TestSet
    /// </summary>
    public class TestSetPlan: IExpressionEvaluationContext
    {
        private Dictionary<string, ExpressionValue> _dataEntries = 
            new Dictionary<string, ExpressionValue>(StringComparer.InvariantCultureIgnoreCase);

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
        public bool DisableInterrupt { get; set; }

        /// <summary>
        /// Test timeout in milliseconds
        /// </summary>
        public int TimeoutValue { get; set; }

        /// <summary>
        /// Memory patterns used in this test set
        /// </summary>
        public List<byte[]> MemPatterns { get; } = new List<byte[]>();

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
            if (CodeOutput.Symbols.TryGetValue(symbol, out var ushortValue))
            {
                return new ExpressionValue(ushortValue);
            }

            // --- Value not found
            return null;
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