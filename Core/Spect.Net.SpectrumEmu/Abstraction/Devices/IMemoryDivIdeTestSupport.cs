namespace Spect.Net.SpectrumEmu.Abstraction.Devices
{
    /// <summary>
    /// This interface provides memory test support for DivIDE
    /// </summary>
    public interface IMemoryDivIdeTestSupport
    {
        /// <summary>
        /// Allows access to the DivIde RAM memory
        /// </summary>
        /// <param name="bank">Bank index</param>
        /// <param name="addr">Memory address</param>
        /// <returns>RAM byte</returns>
        byte this[int bank, ushort addr] { get; set; }

        /// <summary>
        /// Allows reading the DivIDE ROM
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <returns>ROM byte</returns>
        byte this[ushort addr] { get; }
    }
}