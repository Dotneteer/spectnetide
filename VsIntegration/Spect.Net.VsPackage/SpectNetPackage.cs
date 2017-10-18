using System;
using System.Runtime.InteropServices;
using EnvDTE;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.VsPackage.CustomEditors.RomEditor;
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
using Spect.Net.VsPackage.ToolWindows.TzxExplorer;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs;
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

    // --- Command context rules
    [ProvideUIContextRule(Z80ASM_SELECTED_CONTEXT,
        "Z80AsmFiles",
        expression: "DotZ80Asm",
        termNames: new[] { "DotZ80Asm" },
        termValues: new[] { "HierSingleSelectionName:.z80asm$" })]

    // --- Custom designers
    [ProvideEditorExtension(typeof(RomEditorFactory), RomEditorFactory.EXTENSION, 0x40)]
    [ProvideEditorLogicalView(typeof(RomEditorFactory), LogicalViewID.Designer)]
    [ProvideEditorExtension(typeof(TzxEditorFactory), TzxEditorFactory.EXTENSION, 0x40)]
    [ProvideEditorLogicalView(typeof(TzxEditorFactory), LogicalViewID.Designer)]

    // --- Option pages
    [ProvideOptionPage(typeof(SpectNetOptionsGrid), "Spect.Net IDE", "General options", 0, 0, true)]
    public sealed class SpectNetPackage : VsxPackage
    {
        /// <summary>
        /// GUID of the Spectrum project type
        /// </summary>
        public const string Z80ASM_SELECTED_CONTEXT = "051F4EEF-81C8-47DB-BA0B-0701F1C26836";

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
        /// The object responsible for managing Z80 program files
        /// </summary>
        public Z80CodeManager CodeManager { get; private set; }

        /// <summary>
        /// The error list provider accessible from this package
        /// </summary>
        public ErrorListWindow ErrorList { get; private set; }

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

            // --- Create other helper objects
            CodeManager = new Z80CodeManager();
            ErrorList = new ErrorListWindow();
        }

        /// <summary>
        /// Initializes the members used by a solution
        /// </summary>
        private void OnSolutionOpened()
        {
            // --- Every time a new solution has been opened, initialize the 
            // --- Spectrum virtual machine with all of its accessories
            var vm = MachineViewModel = new MachineViewModel();
            vm.RomProvider = new PackageRomProvider();
            vm.ClockProvider = new ClockProvider();
            vm.KeyboardProvider = new KeyboardProvider(vm);
            vm.AllowKeyboardScan = true;
            vm.ScreenFrameProvider = new DelegatingScreenFrameProvider();
            vm.EarBitFrameProvider = new WaveEarbitFrameProvider(new BeeperConfiguration());
            vm.LoadContentProvider = new ProjectFileTapeContentProvider();
            vm.SaveToTapeProvider = new TempFileSaveToTapeProvider();
            vm.StackDebugSupport = new SimpleStackDebugSupport();
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
            // --- stop the virtual machine and clean up
            Messenger.Default.Send(new SolutionClosedMessage());
            MachineViewModel?.StopVmCommand.Execute(null);
            CodeDiscoverySolution.Clear();
            CurrentWorkspace = null;
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

        #endregion

        #region Helpers

        /// <summary>
        /// This method checks if there is only a single item selected in Solution Explorer
        /// </summary>
        /// <param name="hierarchy">The selected hierarchy</param>
        /// <param name="itemid">The selected item in the hierarchy</param>
        /// <returns>
        /// True, if only a single item is selected; otherwise, false
        /// </returns>
        public static bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid)
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
                if (itemid == VSConstants.VSITEMID_ROOT) return false;

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

        #endregion
    }
}
