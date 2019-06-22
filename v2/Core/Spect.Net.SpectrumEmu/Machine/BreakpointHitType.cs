namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// Types of breakpoint hits
    /// </summary>
    public enum BreakpointHitType
    {
        None = 0,
        Less,
        LessOrEqual,
        Equal,
        Greater,
        GreaterOrEqual,
        Multiple
    }
}