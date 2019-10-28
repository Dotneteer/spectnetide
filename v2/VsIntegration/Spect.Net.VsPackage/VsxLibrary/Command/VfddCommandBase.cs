using System.Collections.Generic;
using Microsoft.VisualStudio.Shell;
using Spect.Net.SpectrumEmu.Devices.Floppy;
using Spect.Net.VsPackage.SolutionItems;

namespace Spect.Net.VsPackage.VsxLibrary.Command
{
    /// <summary>
    /// This class represents a command that handles virtual floppy disk files
    /// </summary>
    public abstract class VfddCommandBase : SingleProjectItemCommandBase
    {
        /// <summary>
        /// This command accepts only Z80 code files
        /// </summary>
        public override IEnumerable<string> ItemExtensionsAccepted =>
            new[]
            {
                VsHierarchyTypes.FLOPPY_ITEM,
            };

        /// <summary>
        /// Allows this command only within the active project
        /// </summary>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            if (!mc.Visible) return;

            mc.Visible = IsInActiveProject;
        }

        /// <summary>
        /// Gets the floppy device
        /// </summary>
        public FloppyDevice FloppyDevice => SpectNetPackage.Default.EmulatorViewModel.Machine
            .SpectrumVm.FloppyDevice as FloppyDevice;
    }
}