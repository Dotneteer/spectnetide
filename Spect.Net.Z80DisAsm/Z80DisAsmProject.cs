namespace Spect.Net.Z80DisAsm
{
    /// <summary>
    /// This class describes a project that is used as an input to the 
    /// disassembly process
    /// </summary>
    public class Z80DisAsmProject
    {
        public byte[] Z80Binary { get; }
        public ushort StartOffset { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="z80Binary">The binary code to disassemble</param>
        /// <param name="startOffset">The memory address at the firs byte</param>
        public Z80DisAsmProject(byte[] z80Binary, ushort startOffset = 0)
        {
            Z80Binary = z80Binary;
            StartOffset = startOffset;
        }
    }
}