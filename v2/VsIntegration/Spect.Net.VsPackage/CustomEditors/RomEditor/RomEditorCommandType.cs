namespace Spect.Net.VsPackage.CustomEditors.RomEditor
{
    /// <summary>
    /// Command types available in the ROM editor
    /// </summary>
    public enum RomEditorCommandType
    {
        None,
        Invalid,
        Goto,
        Disassemble,
        ExportDisass,
        ExportProgram,
        ExitDisass,
        SinclairMode,
        ZxBasicMode
    }
}
