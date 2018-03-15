using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;

namespace Spect.Net.VsPackage.Z80Programs.Commands
{
    /// <summary>
    /// This class represents a command that can be associated with a .z80asm file.
    /// </summary>
    public abstract class VmStateCommandBase : SingleProjectItemCommandBase
    {
        /// <summary>
        /// This command accepts only VM state files
        /// </summary>
        public override IEnumerable<string> ItemExtensionsAccepted =>
            new[] {".vmstate"};

        /// <summary>Override this method to define the status query action</summary>
        /// <param name="mc"></param>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            mc.Enabled = true;
        }
    }
}