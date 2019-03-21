namespace Spect.Net.VsPackage.ToolWindows.CompilerOutput
{
    /// <summary>
    /// Respresents a symbol in the Z80 Assembler Output tool window
    /// </summary>
    public class AssemblySymbol
    {
        public string Name { get; }
        public ushort Value { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public AssemblySymbol(string name, ushort value)
        {
            Name = name;
            Value = value;
        }
    }
}