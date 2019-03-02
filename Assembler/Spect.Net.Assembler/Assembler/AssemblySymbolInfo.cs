using Spect.Net.Assembler.SyntaxTree.Expressions;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents and assembly symbol
    /// </summary>
    public class AssemblySymbolInfo: IHasUsageInfo
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

        /// <summary>
        /// Tests if this symbol is a local symbol within a module.
        /// </summary>
        public bool IsModuleLocal { get; }

        /// <summary>
        /// Tests if this symbol is a short-term symbol
        /// </summary>
        public bool IsShortTerm { get; }

        /// <summary>
        /// Signs if the symbol has been used
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        private AssemblySymbolInfo(string name, SymbolType type, ExpressionValue value)
        {
            Name = name;
            Type = type;
            Value = value;
            IsModuleLocal = name != null && name.StartsWith("@");
            IsShortTerm = name != null && name.StartsWith("`");
            IsUsed = false;
        }

        /// <summary>
        /// Factory method that creates a label
        /// </summary>
        /// <param name="name">Label name</param>
        /// <param name="value">Label value</param>
        /// <returns></returns>
        public static AssemblySymbolInfo CreateLabel(string name, ExpressionValue value)
            => new AssemblySymbolInfo(name, SymbolType.Label, value);

        /// <summary>
        /// Factory method that creates a variable
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="value">Variable initial value</param>
        /// <returns></returns>
        public static AssemblySymbolInfo CreateVar(string name, ExpressionValue value)
            => new AssemblySymbolInfo(name, SymbolType.Var, value);
    }
}