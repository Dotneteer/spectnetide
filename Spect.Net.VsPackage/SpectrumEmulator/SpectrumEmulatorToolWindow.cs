using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.Wpf.SpectrumControl;

// ReSharper disable VirtualMemberCallInConstructor

namespace Spect.Net.VsPackage.SpectrumEmulator
{
    /// <summary>
    /// This class implements the Spectrum emulator tool window.
    /// </summary>
    [Guid("de41a21a-714d-495e-9b3f-830965f9332b")]
    public class SpectrumEmulatorToolWindow : ToolWindowPane
    {
        /// <summary>
        /// The ID of the emulator toolbar within this tool window
        /// </summary>
        public const int EMULATOR_TOOLBAR_ID = 0x1000;
        public const int EMULATOR_START_ID = 0x1101;
        public const int EMULATOR_STOP_ID = 0x1102;
        public const int EMULATOR_PAUSE_ID = 0x1103;
        public const int EMULATOR_RESET_ID = 0x1104;
        public const int EMULATOR_STEP_INTO_ID = 0x1201;
        public const int EMULATOR_STEP_OVER_ID = 0x1202;

        private readonly SpectrumEmulatorToolWindowControl _contentControl;
        private OleMenuCommandService _commandService;
        private OleMenuCommand _startCmd;
        private OleMenuCommand _stopCmd;
        private OleMenuCommand _pauseCmd;
        private OleMenuCommand _resetCmd;

        /// <summary>
        /// The view model behind the emulator
        /// </summary>
        public SpectrumVmViewModel ViewModel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpectrumEmulatorToolWindow"/> class.
        /// </summary>
        public SpectrumEmulatorToolWindow() : base(null)
        {
            Caption = "ZX Spectrum Emulator";
            Content = _contentControl = new SpectrumEmulatorToolWindowControl();
            ToolBar = new CommandID(SpectrumEmulatorToolWindowCommand.CommandSet, EMULATOR_TOOLBAR_ID);
            ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
        }

        /// <summary>
        /// Now, the tool window is sites, so we can add menu commands
        /// </summary>
        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();

            ViewModel = _contentControl.ViewModel;

            _commandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (_commandService == null) return;

            _startCmd = RegisterCommand(EMULATOR_START_ID,
                (s, o) => ViewModel.StartVmCommand.Execute(null),
                () => ViewModel.VmState != SpectrumVmState.Running);
            _stopCmd = RegisterCommand(EMULATOR_STOP_ID,
                (s, o) => ViewModel.StopVmCommand.Execute(null),
                () => ViewModel.VmState == SpectrumVmState.Running);
            _pauseCmd = RegisterCommand(EMULATOR_PAUSE_ID,
                (s, o) => ViewModel.PauseVmCommand.Execute(null),
                () => ViewModel.VmState == SpectrumVmState.Running);
            _resetCmd = RegisterCommand(EMULATOR_RESET_ID,
                (s, o) => ViewModel.ResetVmCommand.Execute(null),
                () => ViewModel.VmState == SpectrumVmState.Running);
        }


        private OleMenuCommand RegisterCommand(uint id, EventHandler callback, Func<bool> enableFunc)
        {
            if (_commandService == null) return null;
            var commandId = new CommandID(SpectrumEmulatorToolWindowCommand.CommandSet, (int)id);
            var menuItem = new OleMenuCommand(callback, commandId);
            menuItem.BeforeQueryStatus += (sender, args) =>
            {
                if (sender is OleMenuCommand mc)
                {
                    mc.Enabled = enableFunc();
                }
            } ;
            _commandService.AddCommand(menuItem);
            return menuItem;
        }
    }
}
