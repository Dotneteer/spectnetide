namespace Spect.Net.SpectrumEmu.Devices.Next.Palettes
{
    /// <summary>
    /// This class represents a Next palette with 256 colors
    /// addressable with a byte
    /// </summary>
    public class Palette
    {
        // --- Palette colors
        private readonly int[] _colors = new int[0x100];

        /// <summary>
        /// Gets or sets the specified pallette color
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int this[byte index]
        {
            get => _colors[index];
            set => _colors[index] = value;
        }
    }
}