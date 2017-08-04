namespace Spect.Net.SpectrumEmu.Disassembler
{
    /// <summary>
    /// This class describes a custom label
    /// </summary>
    public class CustomLabel
    {
        /// <summary>
        /// Label address
        /// </summary>
        public ushort Address { get; set; }

        /// <summary>
        /// Label name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public CustomLabel(ushort address, string name)
        {
            Address = address;
            Name = name;
        }
    }
}