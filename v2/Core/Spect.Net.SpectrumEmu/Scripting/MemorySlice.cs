namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// Represents a single MemorySlice of the machine
    /// </summary>
    public sealed class MemorySlice
    {
        private readonly byte[] _bytes;

        public MemorySlice(byte[] getRamBank)
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
        public byte this[int index]
        {
            get => _bytes[index];
            set => _bytes[index] = value;
        } 
    }
}