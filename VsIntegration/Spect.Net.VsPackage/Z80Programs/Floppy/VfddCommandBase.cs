using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Z80Programs.Commands;

namespace Spect.Net.VsPackage.Z80Programs.Floppy
{
    /// <summary>
    /// This class represents a command that can be associated with a .vfdd file.
    /// </summary>
    public abstract class VfddCommandBase : SingleProjectItemCommandBase
    {
        /// <summary>
        /// This command accepts only Z80 code files
        /// </summary>
        public override IEnumerable<string> ItemExtensionsAccepted =>
            new[] {".vfdd"};

        /// <summary>
        /// Gets the hierarchy information for the selected item.
        /// </summary>
        /// <param name="hierarchy">Hierarchy object</param>
        /// <param name="itemId">Hierarchy item id</param>
        /// <remarks>
        /// If the selected item is the project, it retrieves the hierarchy information for the
        /// default code file
        /// </remarks>
        public void GetCodeItem(out IVsHierarchy hierarchy, out uint itemId)
        {
            SpectNetPackage.IsSingleItemSelection(AllowProjectItem, out hierarchy, out itemId);
        }

        /// <summary>
        /// Override this method to define the status query action
        /// </summary>
        /// <param name="mc"></param>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            if (!mc.Visible) return;

            var state = Package.MachineViewModel.MachineState;
            mc.Visible = state == VmState.Paused || state == VmState.Running;
        }
    }
}