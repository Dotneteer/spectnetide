using System;
using System.Runtime.InteropServices;
using EnvDTE;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.SpectrumEmu.Mvvm;
using Spect.Net.VsPackage.CustomEditors.RomEditor;
using Spect.Net.VsPackage.CustomEditors.TzxEditor;
using Spect.Net.VsPackage.ProjectStructure;
using Spect.Net.VsPackage.Tools;
using Spect.Net.VsPackage.Tools.BasicList;
using Spect.Net.VsPackage.Tools.Disassembly;
using Spect.Net.VsPackage.Tools.KeyboardTool;
using Spect.Net.VsPackage.Tools.Memory;
using Spect.Net.VsPackage.Tools.RegistersTool;
using Spect.Net.VsPackage.Tools.SpectrumEmulator;
using Spect.Net.VsPackage.Tools.TzxExplorer;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs;
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
    [ProvideToolWindow(typeof(TzxExplorerToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(BasicListToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(KeyboardToolWindow), Transient = true)]

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
        public void ShowToolWindow<TWindow>()
        {
            var window = GetToolWindow(typeof(TWindow));
            var windowFrame = (IVsWindowFrame)window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        #region Command Handlers

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

        /// <summary>
        /// Displays the BASIC List tool window
        /// </summary>
        [CommandId(0x1500)]
        [ToolWindow(typeof(BasicListToolWindow))]
        public class ShowBasicListCommand :
            VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = Package.CurrentWorkspace?.CurrentProject != null;
        }

        /// <summary>
        /// Displays the BASIC List tool window
        /// </summary>
        [CommandId(0x1600)]
        [ToolWindow(typeof(KeyboardToolWindow))]
        public class ShowKeyboardCommand :
            VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
        {
            protected override void OnQueryStatus(OleMenuCommand mc)
                => mc.Enabled = Package.CurrentWorkspace?.CurrentProject != null;
        }

        /// <summary>
        /// Run a Z80 program command
        /// </summary>
        [CommandId(0x0800)]
        public class RunZ80ProgramCommand : Z80ProgramCommand
        {
            /// <summary>
            /// Override this method to execute the command
            /// </summary>
            protected override void OnExecute()
            {
                // --- Get the item
                GetItem(out var hierarchy, out var itemId);
                if (hierarchy == null) return;

                var manager = new Z80ProgramFileManager(hierarchy, itemId);
                if (manager.Compile())
                {
                    Package.ShowToolWindow<KeyboardToolWindow>();
                }                
            }
        }

        #endregion

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
