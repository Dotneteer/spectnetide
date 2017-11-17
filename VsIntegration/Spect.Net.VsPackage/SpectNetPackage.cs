using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using EnvDTE;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Providers;
using Spect.Net.VsPackage.CustomEditors.DisannEditor;
using Spect.Net.VsPackage.CustomEditors.RomEditor;
using Spect.Net.VsPackage.CustomEditors.SpConfEditor;
using Spect.Net.VsPackage.CustomEditors.TzxEditor;
using Spect.Net.VsPackage.ProjectStructure;
using Spect.Net.VsPackage.ToolWindows;
using Spect.Net.VsPackage.ToolWindows.BasicList;
using Spect.Net.VsPackage.ToolWindows.Disassembly;
using Spect.Net.VsPackage.ToolWindows.KeyboardTool;
using Spect.Net.VsPackage.ToolWindows.Memory;
using Spect.Net.VsPackage.ToolWindows.RegistersTool;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.VsPackage.ToolWindows.StackTool;
using Spect.Net.VsPackage.ToolWindows.TapeFileExplorer;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs;
using Spect.Net.VsPackage.Z80Programs.Debugging;
using Spect.Net.VsPackage.Z80Programs.Providers;
using Spect.Net.Wpf.Mvvm;
using Spect.Net.Wpf.Providers;
using MachineViewModel = Spect.Net.VsPackage.ToolWindows.SpectrumEmulator.MachineViewModel;

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
    [Export(typeof(SpectNetPackage))]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid("1b214806-bc31-49bd-be5d-79ac4a189f3c")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids.NoSolution)]

    // --- Tool windows
    [ProvideToolWindow(typeof(SpectrumEmulatorToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(RegistersToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(DisassemblyToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(MemoryToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(TapeFileExplorerToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(BasicListToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(KeyboardToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(StackToolWindow), Transient = true)]

    // --- Custom designers
    [ProvideEditorExtension(typeof(RomEditorFactory), RomEditorFactory.EXTENSION, 0x40)]
    [ProvideEditorLogicalView(typeof(RomEditorFactory), LogicalViewID.Designer)]
    [ProvideEditorExtension(typeof(TzxEditorFactory), TzxEditorFactory.EXTENSION, 0x40)]
    [ProvideEditorLogicalView(typeof(TzxEditorFactory), LogicalViewID.Designer)]
    [ProvideEditorExtension(typeof(TapEditorFactory), TapEditorFactory.EXTENSION, 0x40)]
    [ProvideEditorLogicalView(typeof(TapEditorFactory), LogicalViewID.Designer)]
    [ProvideEditorExtension(typeof(DisAnnEditorFactory), DisAnnEditorFactory.EXTENSION, 0x40)]
    [ProvideEditorLogicalView(typeof(DisAnnEditorFactory), LogicalViewID.Designer)]
    [ProvideEditorExtension(typeof(SpConfEditorFactory), SpConfEditorFactory.EXTENSION, 0x40)]
    [ProvideEditorLogicalView(typeof(SpConfEditorFactory), LogicalViewID.Designer)]

    // --- Option pages
    [ProvideOptionPage(typeof(SpectNetOptionsGrid), "Spect.Net IDE", "General options", 0, 0, true)]
    public sealed class SpectNetPackage : VsxPackage
    {
        /// <summary>
        /// Command set of the package
        /// </summary>
        public const string PACKAGE_COMMAND_SET = "234580c4-8a2c-4ae1-8e4f-5bc708b188fe";

        /// <summary>
        /// The base URL for command help topics
        /// </summary>
        public const string COMMANDS_BASE_URL = "https://github.com/Dotneteer/spectnetide/tree/master/Documentation/References";

        /// <summary>
        /// The base URL for ZX Spectrum-related help topics
        /// </summary>
        public const string SPECTRUM_REF_BASE_URL = "https://github.com/Dotneteer/spectnetide/tree/master/Documentation";

        /// <summary>
        /// The base URL for SpectNetIde help topics
        /// </summary>
        public const string DOCUMENTATION_BASE_URL = "https://github.com/Dotneteer/spectnetide/tree/master/Documentation";

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid(PACKAGE_COMMAND_SET);

        private DTEEvents _packageDteEvents;
        private SolutionEvents _solutionEvents;

        /// <summary>
        /// Keeps the currently loaded solution structure
        /// </summary>
        public SolutionStructure CodeDiscoverySolution { get; private set; }

        /// <summary>
        /// The view model of the spectrum emulator
        /// </summary>
        public MachineViewModel MachineViewModel { get; private set; }

        /// <summary>
        /// The object responsible for managing Z80 program files
        /// </summary>
        public Z80CodeManager CodeManager { get; private set; }

        /// <summary>
        /// Provides debug information while running the Spectrum virtual machine
        /// </summary>
        public VsIntegratedSpectrumDebugInfoProvider DebugInfoProvider { get; private set; }

        /// <summary>
        /// The error list provider accessible from this package
        /// </summary>
        public ErrorListWindow ErrorList { get; private set; }

        /// <summary>
        /// Gets the task list provider for this package.
        /// </summary>
        public TaskListWindow TaskList { get; private set; }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void OnInitialize()
        {
            RegisterEditorFactory(new RomEditorFactory());
            RegisterEditorFactory(new TzxEditorFactory());
            RegisterEditorFactory(new TapEditorFactory());
            RegisterEditorFactory(new DisAnnEditorFactory());
            RegisterEditorFactory(new SpConfEditorFactory());

            // --- Prepare for package shutdown
            _packageDteEvents = ApplicationObject.Events.DTEEvents;
            _packageDteEvents.OnBeginShutdown += () =>
            {
                Messenger.Default.Send(new PackageShutdownMessage());
            };
            _solutionEvents = ApplicationObject.Events.SolutionEvents;
            _solutionEvents.Opened += OnSolutionOpened;
            _solutionEvents.AfterClosing += OnSolutionClosed;

            // --- Create other helper objects
            DebugInfoProvider = new VsIntegratedSpectrumDebugInfoProvider(this);
            CodeManager = new Z80CodeManager();
            ErrorList = new ErrorListWindow();
            TaskList = new TaskListWindow();
        }

        /// <summary>
        /// Initializes the members used by a solution
        /// </summary>
        private void OnSolutionOpened()
        {
            // --- Let's create the ZX Spectrum virtual machine view model
            // --- that is used all around in tool windows
            CodeDiscoverySolution = new SolutionStructure();
            CodeDiscoverySolution.CollectProjects();
            CodeDiscoverySolution.LoadRom();

            // --- Every time a new solution has been opened, initialize the
            // --- Spectrum virtual machine with all of its accessories
            var spectrumConfig = CodeDiscoverySolution.CurrentProject.SpectrumConfiguration;
            var vm = MachineViewModel = new MachineViewModel();
            vm.MachineController = new MachineController();
            vm.EarBitFrameProvider = new WaveEarbitFrameProvider(spectrumConfig.Beeper);
            vm.TapeProvider = new VsIntegratedTapeProvider(this);
            vm.KeyboardProvider = new KeyboardProvider();
            vm.ScreenConfiguration = spectrumConfig.Screen;
            vm.DeviceData = new DeviceInfoCollection
            {
                new CpuDeviceInfo(spectrumConfig.Cpu),
                new RomDeviceInfo(new PackageRomProvider(), spectrumConfig.Rom),
                new ClockDeviceInfo(new ClockProvider()),
                new KeyboardDeviceInfo(vm.KeyboardProvider),
                new ScreenDeviceInfo(spectrumConfig.Screen,
                    new DelegatingScreenFrameProvider()),
                new BeeperDeviceInfo(spectrumConfig.Beeper, vm.EarBitFrameProvider),
                new TapeDeviceInfo(vm.TapeProvider)
            };

            vm.AllowKeyboardScan = true;
            vm.StackDebugSupport = new SimpleStackDebugSupport();
            vm.DisplayMode = SpectrumDisplayMode.Fit;
            vm.DebugInfoProvider = DebugInfoProvider;

            Messenger.Default.Send(new SolutionOpenedMessage());
        }

        /// <summary>
        /// Cleans up after closing a solution
        /// </summary>
        private void OnSolutionClosed()
        {
            // --- When the current solution has been closed,
            // --- stop the virtual machine and clean up
            Messenger.Default.Send(new SolutionClosedMessage());
            DebugInfoProvider.Clear();
            MachineViewModel?.StopVmCommand.Execute(null);
            CodeDiscoverySolution.Dispose();
            CodeDiscoverySolution = null;
            MachineViewModel?.Dispose();
            MachineViewModel = null;
        }

        /// <summary>
        /// Gets the options of this package
        /// </summary>
        public SpectNetOptionsGrid Options
            => GetDialogPage(typeof(SpectNetOptionsGrid)) as SpectNetOptionsGrid;

        /// <summary>
        /// Displays the tool window with the specified type
        /// </summary>
        /// <typeparam name="TWindow">Tool window type</typeparam>
        public void ShowToolWindow<TWindow>(int instanceId = 0)
            where TWindow : ToolWindowPane
        {
            var window = GetToolWindow(typeof(TWindow), instanceId);
            var windowFrame = (IVsWindowFrame)window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        #region Helper types

        /// <summary>
        /// This message is sent when the package is about to be closed.
        /// </summary>
        public class PackageShutdownMessage : MessageBase
        {
        }

        #endregion Helper types

        #region Helpers

        /// <summary>
        /// This method checks if there is only a single item selected in Solution Explorer
        /// </summary>
        /// <param name="allowProject">Signs if project selection is allowed</param>
        /// <param name="hierarchy">The selected hierarchy</param>
        /// <param name="itemid">The selected item in the hierarchy</param>
        /// <returns>
        /// True, if only a single item is selected; otherwise, false
        /// </returns>
        public static bool IsSingleItemSelection(bool allowProject, out IVsHierarchy hierarchy, out uint itemid)
        {
            hierarchy = null;
            itemid = VSConstants.VSITEMID_NIL;

            var monitorSelection = GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            var solution = GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            if (monitorSelection == null || solution == null)
            {
                return false;
            }

            var hierarchyPtr = IntPtr.Zero;
            var selectionContainerPtr = IntPtr.Zero;

            try
            {
                // --- Obtain the current selection
                var hr = monitorSelection.GetCurrentSelection(out hierarchyPtr,
                    out itemid,
                    out var multiItemSelect,
                    out selectionContainerPtr);

                if (ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL)
                {
                    // --- There is no selection
                    return false;
                }

                // --- Multiple items are selected
                if (multiItemSelect != null) return false;

                // --- There is a hierarchy root node selected, thus it is not a single item inside a project
                if (itemid == VSConstants.VSITEMID_ROOT && !allowProject) return false;

                // --- No hierarchy, no selection
                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;
                if (hierarchy == null) return false;

                // --- Return true only when the hierarchy is a project inside the Solution
                // --- and it has a ProjectID Guid
                return !ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out _));
            }
            finally
            {
                // --- Release unmanaged resources
                if (selectionContainerPtr != IntPtr.Zero)
                {
                    Marshal.Release(selectionContainerPtr);
                }
                if (hierarchyPtr != IntPtr.Zero)
                {
                    Marshal.Release(hierarchyPtr);
                }
            }
        }

        /// <summary>
        /// Forces the update of command UI
        /// </summary>
        public static void UpdateCommandUi()
        {
            var uiShell = GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            uiShell?.UpdateCommandUI(0);
        }

        #endregion Helpers
    }
}