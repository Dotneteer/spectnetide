using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents and assembly symbol
    /// </summary>
    public class AssemblySymbolInfo
    {
        /// <summary>
        /// Name of the symbol
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Type of the symbol
        /// </summary>
        public SymbolType Type { get; }

        /// <summary>
        /// Current value of the Symbol
        /// </summary>
        public ExpressionValue Value { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public AssemblySymbolInfo(string name, SymbolType type, ExpressionValue value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}