namespace Spect.Net.Assembler.Assembler
{
    /// <summary>
    /// This enum defines the types of assembly symbols
    /// </summary>
    public enum SymbolType
    {
        /// <summary>Type not defined</summary>
        None,

        /// <summary>Label, defined once, cannot be changed later</summary>
        Label,

        /// <summary>Variable, can be changed after definition</summary>
        Var
    }
}