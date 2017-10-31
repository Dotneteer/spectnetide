using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Debug the default Z80 program command
    /// </summary>
    [CommandId(0x0807)]
    public class DebugDefaultZ80CodeCommand : DebugZ80CodeCommand
    {
        /// <summary>
        /// Allows the project node to run the default Z80 code file
        /// </summary>
        public override bool AllowProjectItem => true;

        /// <summary>
        /// The item is allowed only when there is a default code file selected
        /// </summary>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            GetCodeItem(out var hierarchy, out _);
            mc.Enabled = hierarchy != null;
        }
    }
}