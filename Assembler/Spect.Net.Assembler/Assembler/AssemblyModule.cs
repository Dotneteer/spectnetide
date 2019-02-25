using System;
using System.Collections.Generic;
using Spect.Net.Assembler.SyntaxTree.Expressions;
// ReSharper disable IdentifierTypo

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents an assembly module that my contain child modules and symbols.
    /// </summary>
    public class AssemblyModule
    {
        /// <summary>
        /// The parent of this module
        /// </summary>
        public AssemblyModule ParentModule { get; }

        /// <summary>
        /// Gets the root (Global) module
        /// </summary>
        public AssemblyModule RootModule
            => ParentModule == null ? this : ParentModule.RootModule;

        /// <summary>
        /// Child modules within this module
        /// </summary>
        public Dictionary<string, AssemblyModule> NestedModules { get;  } = 
            new Dictionary<string, AssemblyModule>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// The symbol table with properly defined symbols
        /// </summary>
        public Dictionary<string, AssemblySymbolInfo> Symbols { get; } =
            new Dictionary<string, AssemblySymbolInfo>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// The table of structures
        /// </summary>
        public Dictionary<string, StructDefinition> Structs { get; } =
            new Dictionary<string, StructDefinition>(StringComparer.InvariantCultureIgnoreCase);


        /// <summary>
        /// The list of fixups to carry out as the last phase of the compilation
        /// </summary>
        public List<FixupEntry> Fixups { get; } = new List<FixupEntry>();

        /// <summary>
        /// Local symbol scopes
        /// </summary>
        public Stack<SymbolScope> LocalScopes = new Stack<SymbolScope>();

        /// <summary>
        /// The macro table
        /// </summary>
        public Dictionary<string, MacroDefinition> Macros { get; } =
            new Dictionary<string, MacroDefinition>(StringComparer.InvariantCultureIgnoreCase);

        public AssemblyModule(AssemblyModule parentModule = null)
        {
            ParentModule = parentModule;
        }

        /// <summary>
        /// Resolves a simple symbol (symbol with simple name)
        /// </summary>
        /// <param name="symbol">Symbol to resolve</param>
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        public ExpressionValue ResolveSimpleSymbol(string symbol)
        {
            // --- Checks the specified module for a symbol
            ExpressionValue ResolveInModule(AssemblyModule module, string symb)
            {
                // --- Check the local scope in stack order
                foreach (var scope in module.LocalScopes)
                {
                    if (scope.Symbols.TryGetValue(symb, out var localSymbolValue))
                    {
                        return localSymbolValue.Value;
                    }
                }

                // --- Check the global scope
                return module.Symbols.TryGetValue(symb, out var symbolValue)
                    ? symbolValue.Value
                    : null;
            }

            // --- Iterate through all modules from the innermost to the outermost
            var currentModule = this;
            while (currentModule != null)
            {
                var valueFound = ResolveInModule(currentModule, symbol);
                if (valueFound != null) return valueFound;
                currentModule = currentModule.ParentModule;
            }

            // --- The symbol has not been found
            return null;
        }

        /// <summary>
        /// Resolves a compound symbol through modules
        /// </summary>
        /// <param name="symbol">First segment of the symbol</param>
        /// <param name="scopeSymbolNames">Last segment of the symbol</param>
        /// <param name="startFromGlobal">Should resolution start from global scope?</param>
        /// <returns></returns>
        public ExpressionValue ResolveCompoundSymbol(string symbol, IEnumerable<string> scopeSymbolNames,
            bool startFromGlobal)
        {
            var symbolSegments = new List<string> { symbol };
            symbolSegments.AddRange(scopeSymbolNames);

            // --- Determine the module to start from
            var module = startFromGlobal ? RootModule : this;

            // --- Iterate through segments
            for (var i = 0; i < symbolSegments.Count; i++)
            {
                var segment = symbolSegments[i];

                // --- Do not search for module-local variables
                if (segment.StartsWith("@") && i > 0)
                {
                    return null;
                }

                if (i == symbolSegments.Count - 1)
                {
                    // --- This is the last segment, it should be a symbol
                    // --- in the currently reached module.
                    return module.Symbols.TryGetValue(segment, out var symbolInfo)
                        ? symbolInfo.Value
                        : null;
                }

                // --- This is a module name within the currently reached module.
                if (!module.NestedModules.TryGetValue(segment, out var subModule))
                {
                    // --- Module does not exist
                    return null;
                }
                module = subModule;
            }
            return null;
        }
    }
}