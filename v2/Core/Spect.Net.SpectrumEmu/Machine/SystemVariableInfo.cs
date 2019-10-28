namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class contains the description of s ZX Spectrum system variable
    /// </summary>
    public class SystemVariableInfo
    {
        /// <summary>
        /// Symbolic name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Memory address 
        /// </summary>
        public ushort Address { get; }

        /// <summary>
        /// Variable length in bytes
        /// </summary>
        public ushort Lenght { get; }

        /// <summary>
        /// Optional description
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a variable description
        /// </summary>
        public SystemVariableInfo(string name, ushort address, ushort lenght, string description)
        {
            Name = name;
            Address = address;
            Lenght = lenght;
            Description = description;
        }
    }
}