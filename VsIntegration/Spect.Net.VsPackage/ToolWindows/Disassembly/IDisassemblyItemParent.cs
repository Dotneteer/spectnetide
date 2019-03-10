namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    /// <summary>
    /// This interface represents the parent of a disassemblyItem
    /// </summary>
    public interface IDisassemblyItemParent
    {
        /// <summary>
        /// Gets the label for the specified address
        /// </summary>
        /// <param name="address">Address to get the annotation for</param>
        /// <param name="label">Label, if found; otherwise, null</param>
        /// <returns>True, if found; otherwise, false</returns>
        bool GetLabel(ushort address, out string label);

        /// <summary>
        /// Gets the comment for the specified address
        /// </summary>
        /// <param name="address">Address to get the annotation for</param>
        /// <param name="comment">Comment, if found; otherwise, null</param>
        /// <returns>True, if found; otherwise, false</returns>
        bool GetComment(ushort address, out string comment);

        /// <summary>
        /// Gets the prefix comment for the specified address
        /// </summary>
        /// <param name="address">Address to get the annotation for</param>
        /// <param name="comment">Prefix comment, if found; otherwise, null</param>
        /// <returns>True, if found; otherwise, false</returns>
        bool GetPrefixComment(ushort address, out string comment);

        /// <summary>
        /// Gets the literal replacement for the specified address
        /// </summary>
        /// <param name="address">Address to get the annotation for</param>
        /// <param name="symbol">Symbol, if found; otherwise, null</param>
        /// <returns>True, if found; otherwise, false</returns>
        bool GetLiteralReplacement(ushort address, out string symbol);

        /// <summary>
        /// Checks if the specified address has a breakpoint
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>
        /// True, if the address has a breakpoint; otherwise, false
        /// </returns>
        bool HasBreakpoint(ushort address);

        /// <summary>
        /// Checks if the specified address is the current instruction
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>
        /// True, if the address is the current instruction; otherwise, false
        /// </returns>
        bool IsCurrentInstruction(ushort address);

        /// <summary>
        /// Tests if the specified address has a breakpoint condition
        /// </summary>
        /// <param name="itemAddress"></param>
        /// <returns></returns>
        bool HasCondition(ushort itemAddress);

        /// <summary>
        /// Toggles the breakpoint represented by the specified item
        /// </summary>
        /// <param name="item">Item to toggle the breakpoint for</param>
        void ToggleBreakpoint(DisassemblyItemViewModel item);
    }
}