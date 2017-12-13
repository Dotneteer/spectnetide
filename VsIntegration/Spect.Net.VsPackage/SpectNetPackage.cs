using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Models;
using Spect.Net.SpectrumEmu.Devices.Keyboard;
using Spect.Net.SpectrumEmu.Devices.Memory;
using Spect.Net.SpectrumEmu.Devices.Rom;
using Spect.Net.SpectrumEmu.Providers;
using Spect.Net.VsPackage.CustomEditors.DisannEditor;
using Spect.Net.VsPackage.CustomEditors.RomEditor;
using Spect.Net.VsPackage.CustomEditors.SpConfEditor;
using Spect.Net.VsPackage.CustomEditors.TzxEditor;
using Spect.Net.VsPackage.ProjectStructure;
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
        private DTEEvents _packageDteEvents;
        private SolutionEvents _solutionEvents;

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
        /// The singleton instance of this package
        /// </summary>
        public static SpectNetPackage Default { get; private set; }

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid(PACKAGE_COMMAND_SET);

        /// <summary>
        /// Keeps the currently loaded solution structure
        /// </summary>
        public SolutionStructure CodeDiscoverySolution { get; private set; }

        /// <summary>
        /// The view model of the spectrum emulator
        /// </summary>
        public MachineViewModel MachineViewModel { get; private set; }

        /// <summary>
        /// This event fires when the package has opened a solution and
        /// prepared the virtual machine to work with
        /// </summary>
        public event EventHandler SolutionOpened;

        /// <summary>
        /// This event fires when the solution has been closed
        /// </summary>
        public event EventHandler SolutionClosed;

        /// <summary>
        /// Signs that the package started closing
        /// </summary>
        public event EventHandler PackageClosing;

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
            // --- We are going to use this singleton instance
            Default = this;

            RegisterEditorFactory(new RomEditorFactory());
            RegisterEditorFactory(new TzxEditorFactory());
            RegisterEditorFactory(new TapEditorFactory());
            RegisterEditorFactory(new DisAnnEditorFactory());
            RegisterEditorFactory(new SpConfEditorFactory());

            // --- Prepare for package shutdown
            _packageDteEvents = ApplicationObject.Events.DTEEvents;
            _packageDteEvents.OnBeginShutdown += () =>
            {
                PackageClosing?.Invoke(this, EventArgs.Empty);
            };
            _solutionEvents = ApplicationObject.Events.SolutionEvents;
            _solutionEvents.Opened += OnSolutionOpened;
            _solutionEvents.AfterClosing += OnSolutionClosed;

            // --- Create other helper objects
            DebugInfoProvider = new VsIntegratedSpectrumDebugInfoProvider();
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

            // --- Every time a new solution has been opened, initialize the
            // --- Spectrum virtual machine with all of its accessories
            var spectrumConfig = CodeDiscoverySolution.CurrentProject.SpectrumConfiguration;
            var vm = MachineViewModel = new MachineViewModel();
            vm.MachineController = new MachineController();
            vm.ScreenConfiguration = spectrumConfig.Screen;

            // --- Create devices according to the project's Spectrum model
            var modelName = CodeDiscoverySolution.CurrentProject.ModelName;
            switch (modelName)
            {
                case SpectrumModels.ZX_SPECTRUM_128:
                    vm.DeviceData = CreateSpectrum128Devices(spectrumConfig);
                    break;
                case SpectrumModels.ZX_SPECTRUM_P3:
                    vm.DeviceData = CreateSpectrum48Devices(spectrumConfig);
                    break;
                case SpectrumModels.ZX_SPECTRUM_NEXT:
                    vm.DeviceData = CreateSpectrum48Devices(spectrumConfig);
                    break;
                default:
                    vm.DeviceData = CreateSpectrum48Devices(spectrumConfig);
                    break;
            }

            vm.AllowKeyboardScan = true;
            vm.StackDebugSupport = new SimpleStackDebugSupport();
            vm.DisplayMode = SpectrumDisplayMode.Fit;

            // --- Set up the debug info provider
            DebugInfoProvider.Prepare();
            vm.DebugInfoProvider = DebugInfoProvider;

            // --- Prepare the virtual machine
            vm.PrepareStartupConfig();
            vm.MachineController.EnsureMachine();
            SolutionOpened?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create the collection of devices for the Spectrum 48K virtual machine
        /// </summary>
        /// <param name="spectrumConfig">Machine configuration</param>
        /// <returns></returns>
        private DeviceInfoCollection CreateSpectrum48Devices(SpectrumEdition spectrumConfig)
        {
            return new DeviceInfoCollection
            {
                new CpuDeviceInfo(spectrumConfig.Cpu),
                new RomDeviceInfo(new PackageRomProvider(), spectrumConfig.Rom, new SpectrumRomDevice()),
                new MemoryDeviceInfo(spectrumConfig.Memory, new Spectrum48MemoryDevice()),
                new PortDeviceInfo(null, new Spectrum48PortDevice()),
                new ClockDeviceInfo(new ClockProvider()),
                new KeyboardDeviceInfo(new KeyboardProvider(), new KeyboardDevice()),
                new ScreenDeviceInfo(spectrumConfig.Screen),
                new BeeperDeviceInfo(spectrumConfig.Beeper, new BeeperWaveProvider()),
                new TapeDeviceInfo(new VsIntegratedTapeProvider())
            };
        }

        /// <summary>
        /// Create the collection of devices for the Spectrum 48K virtual machine
        /// </summary>
        /// <param name="spectrumConfig">Machine configuration</param>
        /// <returns></returns>
        private DeviceInfoCollection CreateSpectrum128Devices(SpectrumEdition spectrumConfig)
        {
            return new DeviceInfoCollection
            {
                new CpuDeviceInfo(spectrumConfig.Cpu),
                new RomDeviceInfo(new PackageRomProvider(), spectrumConfig.Rom, new SpectrumRomDevice()),
                new MemoryDeviceInfo(spectrumConfig.Memory, new Spectrum128MemoryDevice()),
                new PortDeviceInfo(null, new Spectrum128PortDevice()),
                new ClockDeviceInfo(new ClockProvider()),
                new KeyboardDeviceInfo(new KeyboardProvider(), new KeyboardDevice()),
                new ScreenDeviceInfo(spectrumConfig.Screen),
                new BeeperDeviceInfo(spectrumConfig.Beeper, new BeeperWaveProvider()),
                new TapeDeviceInfo(new VsIntegratedTapeProvider())
            };
        }

        /// <summary>
        /// Cleans up after closing a solution
        /// </summary>
        private void OnSolutionClosed()
        {
            // --- When the current solution has been closed,
            // --- stop the virtual machine and clean up
            SolutionClosed?.Invoke(this, EventArgs.Empty);
            DebugInfoProvider.Clear();
            MachineViewModel?.StopVm();
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

        /// <summary>
        /// Tests if the current model is a ZX Spectrum 48K
        /// </summary>
        /// <returns>True, if the current model = ZX Spectrum 48K; otherwise, false</returns>
        public static bool IsSpectrum48Model()
        {
            return Default.CodeDiscoverySolution?.CurrentProject?.ModelName
                   == SpectrumModels.ZX_SPECTRUM_48;
        }

        /// <summary>
        /// Checks if the current Spectrum model of the project in compatible with the 
        /// specified model type
        /// </summary>
        /// <param name="type">Model type</param>
        /// <returns>
        /// True, if the project's Spectrum model is compatible with the specified one;
        /// otherwise, false
        /// </returns>
        public static bool IsCurrentModelCompatibleWith(SpectrumModelType type)
        {
            var modelName = Default.CodeDiscoverySolution?.CurrentProject?.ModelName;
            switch (type)
            {
                case SpectrumModelType.Next:
                    return modelName == SpectrumModels.ZX_SPECTRUM_NEXT;
                case SpectrumModelType.SpectrumP3:
                    return modelName == SpectrumModels.ZX_SPECTRUM_P3 
                        || modelName == SpectrumModels.ZX_SPECTRUM_NEXT;
                case SpectrumModelType.Spectrum128:
                    return modelName == SpectrumModels.ZX_SPECTRUM_128
                        || modelName == SpectrumModels.ZX_SPECTRUM_P3
                        || modelName == SpectrumModels.ZX_SPECTRUM_NEXT;
                case SpectrumModelType.Spectrum48:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the identifier of the currently used Spectrum model
        /// </summary>
        /// <returns></returns>
        public static SpectrumModelType GetCurrentSpectrumModelType()
        {
            var modelName = Default.CodeDiscoverySolution?.CurrentProject?.ModelName;
            switch (modelName)
            {
                case SpectrumModels.ZX_SPECTRUM_NEXT:
                    return SpectrumModelType.Next;
                case SpectrumModels.ZX_SPECTRUM_P3:
                    return SpectrumModelType.SpectrumP3;
                case SpectrumModels.ZX_SPECTRUM_128:
                    return SpectrumModelType.Spectrum128;
                default:
                    return SpectrumModelType.Spectrum48;
            }
        }

        #endregion Helpers
    }
}