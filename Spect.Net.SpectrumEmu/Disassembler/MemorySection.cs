namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class describes a memory section
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
            return false;
        }

        /// <summary>
        /// Checks if this section has the same start and length than the other
        /// </summary>
        /// <param name="other">Other memory section</param>
        /// <returns>Thrue, if the sections have the same start and length</returns>
        public bool SameSection(MemorySection other)
        {
            return false;
        }

        /// <summary>
        /// Gets the intersection of the two memory sections
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public MemorySection Intersection(MemorySection other)
        {
            return null;
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