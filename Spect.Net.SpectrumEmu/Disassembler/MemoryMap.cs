using System.Collections;
using System.Collections.Generic;

namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class implements a memory map of the ZX Spectrum virtual machine
    /// </summary>
    /// <remarks>
    /// Internally, the sections of the memory map are kept ordered by the section's 
    /// start addresses.
    /// </remarks>
    public class MemoryMap: IList<MemorySection>
    {
        private List<MemorySection> _sections;

        /// <summary>
        /// A memory map that contains only the ZX Spectrum ROM
        /// </summary>
        public static MemoryMap RomOnly { get; }

        /// <summary>
        /// A memory map that contains the ZX Spectrum ROM + system variables
        /// </summary>
        public static MemoryMap System { get; }

        static MemoryMap()
        {
            RomOnly = new MemoryMap {new MemorySection {StartAddress = 0x0000, Length = 0x3FFF}};
            System = new MemoryMap { new MemorySection { StartAddress = 0x0000, Length = 0x3FFF } };
        }

        public IEnumerator<MemorySection> GetEnumerator()
        {
            return _sections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(MemorySection item)
        {
            //
        }

        public void Clear() => _sections.Clear();

        public bool Contains(MemorySection item) => _sections.Contains(item);

        public void CopyTo(MemorySection[] array, int arrayIndex) => _sections.CopyTo(array, arrayIndex);

        public bool Remove(MemorySection item) => _sections.Remove(item);

        public int Count => _sections.Count;

        public bool IsReadOnly => false;

        public int IndexOf(MemorySection item) => _sections.IndexOf(item);

        public void Insert(int index, MemorySection item) => _sections.Add(item);

        public void RemoveAt(int index) => _sections.RemoveAt(index);

        public MemorySection this[int index]
        {
            get => _sections[index];
            set => _sections[index] = value;
        }
    }
}