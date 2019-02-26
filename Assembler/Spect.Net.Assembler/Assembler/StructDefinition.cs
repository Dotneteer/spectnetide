using System;
using System.Collections.Generic;

namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This class represents a structure
    /// </summary>
    public class StructDefinition
    {
        /// <summary>
        /// The name of the macro
        /// </summary>
        public string StructName { get; }

        /// <summary>
        /// Macro definition section
        /// </summary>
        public DefinitionSection Section { get; }

        /// <summary>
        /// The fields of the structure
        /// </summary>
        public Dictionary<string, ushort> Fields { get; }

        /// <summary>
        /// The default contents of the structure
        /// </summary>
        public List<byte> DefaultContents { get; }

        /// <summary>
        /// Get the structure size
        /// </summary>
        public int Size => DefaultContents.Count;

        /// <summary>
        /// Fixups to resolve
        /// </summary>
        public List<StructFixup> Fixups { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public StructDefinition(string structName, int macroDefLine, int macroEndLine)
        {
            StructName = structName;
            Section = new DefinitionSection(macroDefLine, macroEndLine);
            Fields = new Dictionary<string, ushort>(StringComparer.InvariantCultureIgnoreCase);
            DefaultContents = new List<byte>();
            Fixups = new List<StructFixup>();
        }
    }
}