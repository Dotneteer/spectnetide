using System.Collections.Generic;

namespace Spect.Net.Z80DisAsm
{
    /// <summary>
    /// This class represents the output of the disassembly project
    /// </summary>
    public class Z80DisAsmOutput
    {
        public List<DisassemblyItem> OutputItems { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public Z80DisAsmOutput()
        {
            OutputItems = new List<DisassemblyItem>();
        }
    }
}