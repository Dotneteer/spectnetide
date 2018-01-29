using System.Collections.Generic;

namespace Spect.Net.VsPackage.Z80Programs.Commands
{
    /// <summary>
    /// This class represents a command that can be associated with a .z80asm file.
    /// </summary>
    public abstract class Z80TestCommandBase : SingleProjectItemCommandBase
    {
        /// <summary>
        /// This command accepts only Z80 code files
        /// </summary>
        public override IEnumerable<string> ItemExtensionsAccepted =>
            new[] {".z80test", ".z80cdproj"};
    }
}