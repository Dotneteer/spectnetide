namespace Spect.Net.Z80Emu.Core
{
    /// <summary>
    /// 8-bit Register indexes
    /// </summary>
    /// <remarks>
    /// This enum defines indexes to access register values through the
    /// C# this[] operator.
    /// </remarks>
    public enum Reg8Index: byte 
    {
        B = 0,
        C = 1,
        D = 2,
        E = 3,
        H = 4,
        L = 5,
        F = 6,
        A = 7
    }
}