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
        private readonly List<MemorySection> _sections = new List<MemorySection>();

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
            // --- We store the items of the list in ascending order by StartAddress
            var overlapFound = false;
            do
            {
                // --- Adjust all old sections that overlap with the new one
                for (var i = 0; i < _sections.Count; i++)
                {
                    var oldSection = _sections[i];
                    if (item.Overlaps(oldSection))
                    {
                        // --- The new item overlaps with one of the exisitning ones
                        overlapFound = true;
                        var oldStart = oldSection.StartAddress;
                        var oldEndEx = oldStart + oldSection.Length;
                        var newStart = item.StartAddress;
                        var newEndEx = newStart + item.Length;

                        if (oldStart < newStart)
                        {
                            // --- Adjust the length of the old section: 
                            // --- it gets shorter
                            oldSection.Length = (ushort)(newStart - oldStart);
                            if (oldEndEx > newEndEx)
                            {
                                // --- The rightmost part of the old section becomes a new section
                                var newSection = new MemorySection((ushort)oldEndEx, (ushort)(oldEndEx - newEndEx));
                                _sections.Insert(i+1, newSection);
                                break;
                            }
                        }
                        else if (oldStart >= newStart)
                        {
                            if (oldEndEx <= newEndEx)
                            {
                                // --- The old section entirely intersects wiht the new section:
                                // --- Remove the old section
                                _sections.RemoveAt(i);
                                break;
                            }
                            // --- Change the old sections's start address
                            oldSection.StartAddress = (ushort)newEndEx;
                        }
                        else
                        {
                            overlapFound = false;
                        }
                    }
                }
            } while (overlapFound);

            // --- At this point we do not have no old overlapping section anymore.
            // --- Insert the nex section to its place according to its StartAddress
            var insertPos = _sections.Count;
            for (var i = 0; i < _sections.Count; i++)
            {
                if (_sections[i].StartAddress > item.StartAddress)
                {
                    // --- This is the right place to insert the new section
                    insertPos = i;
                    break;
                }
            }
            _sections.Insert(insertPos, item);
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