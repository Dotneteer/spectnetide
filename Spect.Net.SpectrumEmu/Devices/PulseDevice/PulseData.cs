namespace Spect.Net.SpectrumEmu.Devices.PulseDevice
{
    /// <summary>
    /// Represents the data of a simple pulse
    /// 
    /// </summary>
    public struct PulseData
    {
        /// <summary>
        /// True=High, False=Low
        /// </summary>
        public bool PulseBit;

        /// <summary>
        /// Lenght of the pulse (given in Z80 tacts)
        /// </summary>
        public int Lenght;
    }
}