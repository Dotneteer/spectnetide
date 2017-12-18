namespace Spect.Net.VsPackage.ToolWindows.Disassembly
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
        Literal,
        SetRomPage,
        SetRamBank,
        MemoryMode,
        DisassemblyType,
        ReDisassembly
    }
}