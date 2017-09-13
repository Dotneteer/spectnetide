using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.SpectrumEmu.Mvvm;
using Spect.Net.VsPackage.ProjectStructure;
using Spect.Net.VsPackage.Vsx;

// ReSharper disable VirtualMemberCallInConstructor

namespace Spect.Net.VsPackage.Tools.SpectrumEmulator
{
    /// <summary>
    /// This class implements the Spectrum emulator tool window.
    /// </summary>
    [Guid("de41a21a-714d-495e-9b3f-830965f9332b")]
    [Caption("ZX Spectrum Emulator")]
    [ToolWindowToolbar(typeof(SpectNetCommandSet), 0x1010)]
    public class SpectrumEmulatorToolWindow : 
        SpectrumToolWindowPane<SpectrumEmulatorToolWindowControl, SpectrumGenericToolWindowViewModel>
    {
        /// <summary>
        /// Creates a new view model every time a new solution is opened.
        /// </summary>
        /// <param name="msg">Solution opened message</param>
        protected override void OnSolutionOpened(SolutionOpenedMessage msg)
        {
            base.OnSolutionOpened(msg);
            (Content as ISupportsMvvm<SpectrumGenericToolWindowViewModel>)
                .SetVm(new SpectrumGenericToolWindowViewModel());
        }

        /// <summary>Called when the active IVsWindowFrame changes.</summary>
        /// <param name="oldFrame">The old active frame.</param>
        /// <param name="newFrame">The new active frame.</param>
        public override void OnActiveFrameChanged(IVsWindowFrame oldFrame, IVsWindowFrame newFrame)
        {
            Content.Vm.MachineViewModel.AllowKeyboardScan = newFrame == Frame;
        }

        public static VmState GetVmState(SpectNetPackage package) 
            => package.MachineViewModel?.VmState ?? VmState.None;

        /// <summary>
        /// Starts the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1081)]
        public class StartVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
            {
                var state = GetVmState(Package);
                if (state == VmState.None || state == VmState.Stopped)
                {
                    WorkspaceInfo.RefreshFromSolution(Package.CurrentWorkspace, Package.CodeDiscoverySolution);
                    Package.MachineViewModel.FastTapeMode = Package.Options.UseFastLoad;
                }
                Package.MachineViewModel.StartVmCommand.Execute(null);
            }

            protected override void OnQueryStatus(OleMenuCommand mc) 
                => mc.Enabled = GetVmState(Package) != VmState.Running;
        }

        /// <summary>
        /// Stops the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1082)]
        public class StopVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute() => 
                Package.MachineViewModel.StopVmCommand.Execute(null);

            protected override void OnQueryStatus(OleMenuCommand mc)
            {
                var state = GetVmState(Package);
                mc.Enabled = state == VmState.Running
                             || state == VmState.Paused;
            }
        }

        /// <summary>
        /// Pauses the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1083)]
        public class PauseVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
                => Package.MachineViewModel.PauseVmCommand.Execute(null);

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = GetVmState(Package) == VmState.Running;
        }

        /// <summary>
        /// Resets the ZX Spectrum virtual machine
        /// </summary>
        [CommandId(0x1084)]
        public class ResetVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
                => Package.MachineViewModel.ResetVmCommand.Execute(null);

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = GetVmState(Package) == VmState.Running;
        }

        /// <summary>
        /// Starts the ZX Spectrum virtual machine in debug mode
        /// </summary>
        [CommandId(0x1085)]
        public class StartDebugVmCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
                => Package.MachineViewModel.StartDebugVmCommand.Execute(null);

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = GetVmState(Package) != VmState.Running;
        }

        /// <summary>
        /// Steps into the next Z80 instruction in debug mode
        /// </summary>
        [CommandId(0x1086)]
        public class StepIntoCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
                => Package.MachineViewModel.StepIntoCommand.Execute(null);

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = GetVmState(Package) == VmState.Paused;
        }

        /// <summary>
        /// Steps into the next Z80 instruction in debug mode
        /// </summary>
        [CommandId(0x1087)]
        public class StepOverCommand :
            VsxCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnExecute()
                => Package.MachineViewModel.StepOverCommand.Execute(null);

            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = GetVmState(Package) == VmState.Paused;
        }
    }
}
