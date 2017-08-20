using System;
using System.Runtime.InteropServices;
using EnvDTE;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.SpectrumEmu.Mvvm;
using Spect.Net.VsPackage.CustomEditors.RomEditor;
using Spect.Net.VsPackage.CustomEditors.TzxEditor;
using Spect.Net.VsPackage.Messages;
using Spect.Net.VsPackage.ProjectStructure;
using Spect.Net.VsPackage.Tools;
using Spect.Net.VsPackage.Tools.Disassembly;
using Spect.Net.VsPackage.Tools.Memory;
using Spect.Net.VsPackage.Tools.RegistersTool;
using Spect.Net.VsPackage.Tools.SpectrumEmulator;
using Spect.Net.VsPackage.Tools.TzxExplorer;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.Wpf.Providers;

namespace Spect.Net.VsPackage
{
    /// <summary>
    /// This class provides a Visual Studio package for the Spect.NET IDE.
    /// </summary>
    /// <remarks>
    /// This package holds a single instance of the Spectrum virtual machine that is
    /// recreated every time a new solution is opened. The VM is stopped and cleaned up
    /// whenever the solution is closed.
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid("1b214806-bc31-49bd-be5d-79ac4a189f3c")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(SpectrumEmulatorToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(RegistersToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(DisassemblyToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(MemoryToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(TzxExplorerToolWindow), Transient = true)]
    [ProvideAutoLoad(UIContextGuids.NoSolution)]

    [ProvideEditorExtension(typeof(RomEditorFactory), RomEditorFactory.EXTENSION, 0x40)]
    [ProvideEditorLogicalView(typeof(RomEditorFactory), LogicalViewID.Designer)]
    [ProvideEditorExtension(typeof(TzxEditorFactory), TzxEditorFactory.EXTENSION, 0x40)]
    [ProvideEditorLogicalView(typeof(TzxEditorFactory), LogicalViewID.Designer)]

    public sealed class SpectNetPackage : VsxPackage
    {
        /// <summary>
        /// Command set of the package
        /// </summary>
        public const string PACKAGE_COMMAND_SET = "234580c4-8a2c-4ae1-8e4f-5bc708b188fe";

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid(PACKAGE_COMMAND_SET);

        private DTEEvents _packageDteEvents;
        private SolutionEvents _solutionEvents;

        /// <summary>
        /// The view model of the spectrum emulator
        /// </summary>
        public MachineViewModel MachineViewModel { get; private set; }

        /// <summary>
        /// Keeps the currently loaded solution structure
        /// </summary>
        public SolutionStructure CodeDiscoverySolution { get; private set; }

        /// <summary>
        /// The current workspace
        /// </summary>
        public WorkspaceInfo CurrentWorkspace { get; set; }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void OnInitialize()
        {
            RegisterEditorFactory(new RomEditorFactory());
            RegisterEditorFactory(new TzxEditorFactory());

            // --- Let's create the ZX Spectrum virtual machine view model 
            // --- that is used all around in tool windows
            CodeDiscoverySolution = new SolutionStructure();

            // --- Prepare for package shutdown
            _packageDteEvents = ApplicationObject.Events.DTEEvents;
            _packageDteEvents.OnBeginShutdown += () =>
            {
                Messenger.Default.Send(new PackageShutdownMessage());
            };
            _solutionEvents = ApplicationObject.Events.SolutionEvents;
            _solutionEvents.Opened += OnSolutionOpened;
            _solutionEvents.AfterClosing += OnSolutionClosed;
        }

        /// <summary>
        /// Initializes the members used by a solution
        /// </summary>
        private void OnSolutionOpened()
        {
            // --- Every time a new solution has been opened, initialize the 
            // --- Spectrum virtual machine with all of its accessories
            var vm = MachineViewModel = new MachineViewModel();
            vm.RomProvider = new ProjectFileRomProvider();
            vm.ClockProvider = new ClockProvider();
            vm.KeyboardProvider = new KeyboardProvider();
            vm.AllowKeyboardScan = true;
            vm.ScreenFrameProvider = new DelegatingScreenFrameProvider();
            vm.EarBitFrameProvider = new WaveEarbitFrameProvider(new BeeperConfiguration());
            vm.LoadContentProvider = new ProjectFileTzxLoadContentProvider();
            vm.SaveContentProvider = new TzxTempFileSaveContentProvider();
            vm.DisplayMode = SpectrumDisplayMode.Fit;

            CodeDiscoverySolution.CollectProjects(ApplicationObject.DTE.Solution);
            CurrentWorkspace = WorkspaceInfo.CreateFromSolution(CodeDiscoverySolution);
            Messenger.Default.Send(new SolutionOpenedMessage());
        }

        /// <summary>
        /// Cleans up after closing a solution
        /// </summary>
        private void OnSolutionClosed()
        {
            // --- When the current solution has been closed, 
            // --- stop the virtual machnie and clean up
            Messenger.Default.Send(new SolutionClosedMessage());
            MachineViewModel?.StopVmCommand.Execute(null);
            CodeDiscoverySolution.Clear();
            CurrentWorkspace = null;
            MachineViewModel = null;
        }

        /// <summary>
        /// Displays the ZX Spectrum emulator tool window
        /// </summary>
        [CommandId(0x1000)]
        [ToolWindow(typeof(SpectrumEmulatorToolWindow))]
        public class ShowSpectrumEmulatorCommand : 
            VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = Package.CurrentWorkspace?.CurrentProject != null;
        }

        /// <summary>
        /// Displays the Z80 Registers tool window
        /// </summary>
        [CommandId(0x1100)]
        [ToolWindow(typeof(RegistersToolWindow))]
        public class ShowZ80RegistersCommand :
            VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = Package.CurrentWorkspace?.CurrentProject != null;
        }

        /// <summary>
        /// Displays the Z80 Registers tool window
        /// </summary>
        [CommandId(0x1200)]
        [ToolWindow(typeof(DisassemblyToolWindow))]
        public class ShowZ80DisassemblyCommand :
            VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = Package.CurrentWorkspace?.CurrentProject != null;
        }

        /// <summary>
        /// Displays the ZX Spectrum Memory tool window
        /// </summary>
        [CommandId(0x1300)]
        [ToolWindow(typeof(MemoryToolWindow))]
        public class ShowSpectrumMemoryCommand :
            VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = Package.CurrentWorkspace?.CurrentProject != null;
        }

        /// <summary>
        /// Displays the TZX Explorer tool window
        /// </summary>
        [CommandId(0x1400)]
        [ToolWindow(typeof(TzxExplorerToolWindow))]
        public class ShowTzxExplorerCommand :
            VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = Package.CurrentWorkspace?.CurrentProject != null;
        }
    }
}
