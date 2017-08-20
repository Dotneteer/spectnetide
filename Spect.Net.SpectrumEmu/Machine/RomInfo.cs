using System.Collections.Generic;
using Newtonsoft.Json;
using Spect.Net.SpectrumEmu.Disassembler;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class describes the information about a specific operation system (ROM)
    /// that is used by the Spectrum virtual machine
    /// </summary>
    public class RomInfo
    {
        /// <summary>
        /// The contents of the ROM
        /// </summary>
        [JsonIgnore]
        public byte[] RomBytes { get; set; }

        /// <summary>
        /// The memory map of the ROM + the system area
        /// </summary>
        public List<MemorySection> MemorySections { get; set; }

        /// <summary>
        /// The SAVE_BYTES routine address in the ROM
        /// </summary>
        public ushort SaveBytesRoutineAddress { get; set; }

        /// <summary>
        /// The address to resume after a hooked SAVE_BYTES operation
        /// </summary>
        public ushort SaveBytesResumeAddress { get; set; }

        /// <summary>
        /// The LOAD_BYTES routine address in the ROM
        /// </summary>
        public ushort LoadBytesRoutineAddress { get; set; }

        /// <summary>
        /// The address to resume after a hooked LOAD_BYTES operation
        /// </summary>
        public ushort LoadBytesResumeAddress { get; set; }

        /// <summary>
        /// The address to terminate the data block load when the header is
        /// invalid
        /// </summary>
        public ushort LoadBytesInvalidHeaderAddress { get; set; }

        /// <summary>
        /// The start address of the token table
        /// </summary>
        public ushort TokenTableAddress { get; set; }

        /// <summary>
        /// The number of tokent in the token table
        /// </summary>
        public ushort TokenCount { get; set; }

        public RomInfo()
        {
            MemorySections = new List<MemorySection>();
        }
    }
}