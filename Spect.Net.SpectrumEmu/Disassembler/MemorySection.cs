namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class describes a memory section with a start address and a length
    /// </summary>
    public class MemorySection
    {
        /// <summary>
        /// The start address of the memory
        /// </summary>
        public ushort StartAddress { get; set; }

        /// <summary>
        /// The length of the section
        /// </summary>
        public ushort Length { get; set; }

        /// <summary>
        /// The type of the memory section
        /// </summary>
        public MemorySectionType SectionType { get; set; }

        /// <summary>
        /// Creates an empty MemorySection
        /// </summary>
        public MemorySection()
        {
        }

        /// <summary>
        /// Creates a MemorySection with the specified properties
        /// </summary>
        /// <param name="startAddress">Starting address</param>
        /// <param name="length">Length</param>
        /// <param name="sectionType">Section type</param>
        public MemorySection(ushort startAddress, ushort length, MemorySectionType sectionType = MemorySectionType.Disassemble)
        {
            StartAddress = startAddress;
            Length = length;
            SectionType = sectionType;
            Adjust();
        }

        /// <summary>
        /// Adjusts the length of the memory section.
        /// </summary>
        /// <remarks>
        /// The lenght is truncated so that the 0xFFFF end address would not be exceeded
        /// </remarks>
        public void Adjust()
        {
            if (StartAddress + Length >= 0x10000)
            {
                Length = (ushort) (0x10000 - StartAddress);
            }
        }

        /// <summary>
        /// Checks if this memory section overlaps with the othe one
        /// </summary>
        /// <param name="other">Other memory section</param>
        /// <returns>True, if the sections overlap</returns>
        public bool Overlaps(MemorySection other)
        {
            return other.StartAddress >= StartAddress 
                   && other.StartAddress <= StartAddress + Length - 1
                   || other.StartAddress + other.Length - 1 >= StartAddress 
                   && other.StartAddress + other.Length - 1 <= StartAddress + Length - 1
                   || StartAddress >= other.StartAddress 
                   && StartAddress <= other.StartAddress + other.Length -1
                   || StartAddress + Length - 1 >= other.StartAddress
                   && StartAddress + Length - 1 <= other.StartAddress + other.Length - 1;
        }

        /// <summary>
        /// Checks if this section has the same start and length than the other
        /// </summary>
        /// <param name="other">Other memory section</param>
        /// <returns>Thrue, if the sections have the same start and length</returns>
        public bool SameSection(MemorySection other)
        {
            return StartAddress == other.StartAddress && Length == other.Length;
        }

        /// <summary>
        /// Gets the intersection of the two memory sections
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Intersection, if exists; otherwise, null</returns>
        public MemorySection Intersection(MemorySection other)
        {
            var intStart = -1;
            var intEnd = -1;
            if (other.StartAddress >= StartAddress 
                && other.StartAddress <= StartAddress + Length - 1)
            {
                intStart = other.StartAddress;
            }
            if (other.StartAddress + other.Length - 1 >= StartAddress
                && other.StartAddress + other.Length - 1 <= StartAddress + Length - 1)
            {
                intEnd = other.StartAddress + other.Length - 1;
            }
            if (StartAddress >= other.StartAddress
                && StartAddress <= other.StartAddress + other.Length - 1)
            {
                intStart = StartAddress;
            }
            if (StartAddress + Length - 1 >= other.StartAddress
                && StartAddress + Length - 1 <= other.StartAddress + other.Length - 1)
            {
                intEnd = StartAddress + Length - 1;
            }
            return intStart < 0 || intEnd < 0
                ? null
                : new MemorySection((ushort) intStart, (ushort) (intEnd - intStart + 1));
        }

        protected bool Equals(MemorySection other)
        {
            return StartAddress == other.StartAddress && Length == other.Length && SectionType == other.SectionType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MemorySection) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                var hashCode = StartAddress.GetHashCode();
                hashCode = (hashCode * 397) ^ Length.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) SectionType;
                // ReSharper restore NonReadonlyMemberInGetHashCode
                return hashCode;
            }
        }
    }
}