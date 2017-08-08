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
        /// The description of the operating system
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The contents of the ROM
        /// </summary>
        [JsonIgnore]
        public byte[] RomBytes { get; set; }

        /// <summary>
        /// The memory map of the ROM + the system area
        /// </summary>
        public List<MemorySection> MemoryMap { get; set; }

        /// <summary>
        /// Custom labels associated with the ROM
        /// </summary>
        public Dictionary<ushort, CustomLabel> CustomLabels { get; set; }

        /// <summary>
        /// Custom comments associated with the ROM
        /// </summary>
        public Dictionary<ushort, CustomComment> CustomComments { get; set; }

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

        public RomInfo()
        {
            MemoryMap = new List<MemorySection>();
            CustomLabels = new Dictionary<ushort, CustomLabel>();
            CustomComments = new Dictionary<ushort, CustomComment>();
        }

        /// <summary>
        /// Serializes the OS information into a JSON string
        /// </summary>
        /// <param name="info">OS information to serialize</param>
        /// <returns>JSON string</returns>
        public static string SerializeToJson(RomInfo info)
        {
            return JsonConvert.SerializeObject(info, Formatting.Indented);
        }

        /// <summary>
        /// Deserializes the OS information from a JSON string
        /// </summary>
        /// <param name="info">JSON string to deserialize from</param>
        /// <returns>OS information</returns>
        public static RomInfo DeserializeFromJson(string info)
        {
            return JsonConvert.DeserializeObject<RomInfo>(info);
        }
    }
}