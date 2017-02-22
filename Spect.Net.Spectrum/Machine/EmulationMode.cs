namespace Spect.Net.Spectrum.Machine
{
    public enum EmulationMode
    {
        Continuous,
        SingleZ80Instruction,
        UntilFrameTimeExpired,
        UntilFrameEnds
    }
}