using System.Collections.Generic;

namespace Spect.Net.VsPackage.Z80Programs
{
    /// <summary>
    /// This class represents a command that can be associated with a .z80asm file.
    /// </summary>
    public abstract class Z80ProgramCommandBase : SingleProjectItemCommandBase
    {
        /// <summary>
        /// This command accepts only Z80 code files
        /// </summary>
        protected override IEnumerable<string> ItemExtensionsAccepted =>
            new[] {".z80Asm"};
    }
}