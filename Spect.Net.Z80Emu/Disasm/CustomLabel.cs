namespace Spect.Net.Z80Emu.Disasm
{
    /// <summary>
    /// This class describes a custom label
    /// </summary>
    public class CustomLabel
    {
        /// <summary>
        /// Label address
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// Label name
        /// </summary>
        public string Name { get; }

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