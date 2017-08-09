namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class describes a memory section with a start address and a length
    /// </summary>
    public class MemorySection
    {
        /// <summary>
        /// The start address of the section
        /// </summary>
        public ushort StartAddress { get; set; }

        /// <summary>
        /// The end address of the section (inclusive)
        /// </summary>
        public ushort EndAddress { get; set; }

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
        /// <param name="endAddress">Length</param>
        /// <param name="sectionType">Section type</param>
        public MemorySection(ushort startAddress, ushort endAddress, MemorySectionType sectionType = MemorySectionType.Disassemble)
        {
            StartAddress = startAddress;
            EndAddress = endAddress;
            SectionType = sectionType;
        }

        /// <summary>
        /// Checks if this memory section overlaps with the othe one
        /// </summary>
        /// <param name="other">Other memory section</param>
        /// <returns>True, if the sections overlap</returns>
        public bool Overlaps(MemorySection other)
        {
            return other.StartAddress >= StartAddress 
                   && other.StartAddress <= EndAddress
                   || other.EndAddress >= StartAddress 
                   && other.EndAddress <= EndAddress
                   || StartAddress >= other.StartAddress 
                   && StartAddress <= other.EndAddress
                   || EndAddress >= other.StartAddress
                   && EndAddress <= other.EndAddress;
        }

        /// <summary>
        /// Checks if this section has the same start and length than the other
        /// </summary>
        /// <param name="other">Other memory section</param>
        /// <returns>Thrue, if the sections have the same start and length</returns>
        public bool SameSection(MemorySection other)
        {
            return StartAddress == other.StartAddress && EndAddress == other.EndAddress;
        }

        /// <summary>
        /// Gets the intersection of the two memory sections
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Intersection, if exists; otherwise, null</returns>
        public MemorySection Intersect(MemorySection other)
        {
            var intStart = -1;
            var intEnd = -1;
            if (other.StartAddress >= StartAddress 
                && other.StartAddress <= EndAddress)
            {
                intStart = other.StartAddress;
            }
            if (other.EndAddress >= StartAddress
                && other.EndAddress <= EndAddress)
            {
                intEnd = other.EndAddress;
            }
            if (StartAddress >= other.StartAddress
                && StartAddress <= other.EndAddress)
            {
                intStart = StartAddress;
            }
            if (EndAddress >= other.StartAddress
                && EndAddress <= other.EndAddress)
            {
                intEnd = EndAddress;
            }
            return intStart < 0 || intEnd < 0
                ? null
                : new MemorySection((ushort) intStart, (ushort) intEnd);
        }

        protected bool Equals(MemorySection other)
        {
            return StartAddress == other.StartAddress 
                && EndAddress == other.EndAddress 
                && SectionType == other.SectionType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MemorySection) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                var hashCode = StartAddress.GetHashCode();
                hashCode = (hashCode * 397) ^ EndAddress.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) SectionType;
                // ReSharper restore NonReadonlyMemberInGetHashCode
                return hashCode;
            }
        }
    }
}