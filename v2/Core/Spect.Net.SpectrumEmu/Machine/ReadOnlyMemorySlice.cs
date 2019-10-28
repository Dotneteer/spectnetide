namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// Represents a single MemorySlice of the machine
    /// </summary>
    public sealed class ReadOnlyMemorySlice
    {
        private readonly byte[] _bytes;

        public ReadOnlyMemorySlice(byte[] getRamBank)
        {
            _bytes = getRamBank;
        }

        /// <summary>
        /// Gets the size of the memory bank
        /// </summary>
        public int Size => _bytes.Length;

        /// <summary>
        /// Gets or sets the specified byte in the memory
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte this[int index] => _bytes[index];
    }
}