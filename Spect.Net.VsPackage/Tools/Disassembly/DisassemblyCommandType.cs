namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// Command types available in the Disassembly tool window
    /// </summary>
    public enum DisassemblyCommandType
    {
        None,
        Invalid,
        Goto,
        Label,
        Comment,
        PrefixComment,
        SetBreakPoint,
        ToggleBreakPoint,
        RemoveBreakPoint,
        EraseAllBreakPoint,
        Retrieve,
        AddSection,
        Literal
    }
}