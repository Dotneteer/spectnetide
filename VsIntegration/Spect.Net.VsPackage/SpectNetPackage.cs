using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Floppy;
using Spect.Net.SpectrumEmu.Devices.Next;
using Spect.Net.SpectrumEmu.Devices.Ports;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.CustomEditors.DisannEditor;
using Spect.Net.VsPackage.CustomEditors.RomEditor;
using Spect.Net.VsPackage.CustomEditors.SpConfEditor;
using Spect.Net.VsPackage.CustomEditors.TzxEditor;
using Spect.Net.VsPackage.ProjectStructure;
using Spect.Net.VsPackage.ToolWindows.BasicList;
using Spect.Net.VsPackage.ToolWindows.CompilerOutput;
using Spect.Net.VsPackage.ToolWindows.Disassembly;
using Spect.Net.VsPackage.ToolWindows.KeyboardTool;
using Spect.Net.VsPackage.ToolWindows.Memory;
using Spect.Net.VsPackage.ToolWindows.RegistersTool;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.VsPackage.ToolWindows.StackTool;
using Spect.Net.VsPackage.ToolWindows.TapeFileExplorer;
using Spect.Net.VsPackage.ToolWindows.TestExplorer;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs;
using Spect.Net.VsPackage.Z80Programs.Debugging;
using Spect.Net.VsPackage.Z80Programs.Providers;
using Spect.Net.Wpf.Mvvm;
using Spect.Net.Wpf.Providers;
using OutputWindow = Spect.Net.VsPackage.Vsx.Output.OutputWindow;

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
    [ProvideToolWindow(typeof(TestExplorerToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(AssemblerOutputToolWindow), Transient = true)]

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
        /// CPS deplyment strings
        /// </summary>
        public const string CPS_FOLDER = @"CustomProjectSystems\Spect.Net.CodeDiscover";
        public const string CPS_VERSION_FILE = "cps.version";
        public const string CURRENT_CPS_VERSION = "1.8.0";
        public const string CPS_RESOURCE_PREFIX = "Spect.Net.VsPackage.DeploymentResources";
        public const string CPS_RULES = "Rules";

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
        /// Signs that a test file has been changed;
        /// </summary>
        public event EventHandler<FileChangedEventArgs> TestFileChanged;

        /// <summary>
        /// The object responsible for managing Z80 program files
        /// </summary>
        public Z80CodeManager CodeManager { get; private set; }

        /// <summary>
        /// The object responsible for managing Z80 unit test files
        /// </summary>
        public Z80TestManager TestManager { get; private set; }

        /// <summary>
        /// The object responsible for managing VMState files
        /// </summary>
        public VmStateFileManager StateFileManager { get; private set; }

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
            // --- Prepare project system extension files
            CheckCpsFiles();

            // --- We are going to use this singleton instance
            Default = this;

            RegisterEditorFactory(new RomEditorFactory());
            RegisterEditorFactory(new TzxEditorFactory());
            RegisterEditorFactory(new TapEditorFactory());
            RegisterEditorFactory(new DisAnnEditorFactory());
            RegisterEditorFactory(new SpConfEditorFactory());

            // --- Register providers
            SpectrumMachine.Reset();
            SpectrumMachine.RegisterProvider<IRomProvider>(() => new PackageRomProvider());
            SpectrumMachine.RegisterProvider<IKeyboardProvider>(() => new KeyboardProvider());
            SpectrumMachine.RegisterProvider<IBeeperProvider>(() => new AudioWaveProvider());
            SpectrumMachine.RegisterProvider<ITapeProvider>(() => new VsIntegratedTapeProvider());
            SpectrumMachine.RegisterProvider<ISoundProvider>(() => new AudioWaveProvider(AudioProviderType.Psg));
            DebugInfoProvider = new VsIntegratedSpectrumDebugInfoProvider();
            SpectrumMachine.RegisterProvider<ISpectrumDebugInfoProvider>(() => DebugInfoProvider);

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
            CodeManager = new Z80CodeManager();
            TestManager = new Z80TestManager();
            StateFileManager = new VmStateFileManager();
            ErrorList = new ErrorListWindow();
            TaskList = new TaskListWindow();
        }

        /// <summary>
        /// Checks CPS files, and refreshes them when it's time
        /// </summary>
        private static void CheckCpsFiles()
        {
            var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var cpsFolder = Path.Combine(localAppDataFolder, CPS_FOLDER);

            var versionFile = Path.Combine(cpsFolder, CPS_VERSION_FILE);
            if (File.Exists(versionFile))
            {
                var content = File.ReadAllText(versionFile);
                var parts = content.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0 && parts[0] == CURRENT_CPS_VERSION) return;
            }

            // --- Refresh CPS files
            var asm = typeof(SpectNetPackage).Assembly;
            var resources = asm.GetManifestResourceNames();
            foreach (var resource in resources)
            {
                string subFolder;
                if (resource.EndsWith(".props") || resource.EndsWith("targets"))
                {
                    subFolder = "";
                }
                else if (resource.EndsWith(".xaml"))
                {
                    subFolder = CPS_RULES;
                }
                else continue;

                var destFolder = Path.Combine(cpsFolder, subFolder);
                if (resource.StartsWith(CPS_RESOURCE_PREFIX))
                {
                    var destFile = resource.Substring(CPS_RESOURCE_PREFIX.Length + 1);
                    if (!Directory.Exists(destFolder))
                    {
                        Directory.CreateDirectory(destFolder);
                    }
                    var resMan = asm.GetManifestResourceStream(resource);
                    if (resMan == null) continue;
                    var fileReader = new StreamReader(resMan);
                    var fileContent = fileReader.ReadToEnd();
                    File.WriteAllText(Path.Combine(destFolder, destFile), fileContent);
                }
            }

            // --- Write back the version file
            File.WriteAllText(versionFile, CURRENT_CPS_VERSION);
        }

        /// <summary>
        /// Initializes the members used by a solution
        /// </summary>
        private async void OnSolutionOpened()
        {
            try
            {
                CheckCpsFiles();
                await System.Threading.Tasks.Task.Delay(2000);

                // --- Let's create the ZX Spectrum virtual machine view model
                // --- that is used all around in tool windows
                CodeDiscoverySolution = new SolutionStructure();
                CodeDiscoverySolution.CollectProjects();

                // --- Every time a new solution has been opened, initialize the
                // --- Spectrum virtual machine with all of its accessories
                var vm = MachineViewModel = CreateProjectMachine();

                // --- Set up the debug info provider
                DebugInfoProvider.Prepare();
                vm.DebugInfoProvider = DebugInfoProvider;

                SolutionOpened?.Invoke(this, EventArgs.Empty);

                // --- Let initialize these tool windows even before showing up them
                GetToolWindow(typeof(SpectrumEmulatorToolWindow));
                GetToolWindow(typeof(AssemblerOutputToolWindow));
                GetToolWindow(typeof(MemoryToolWindow));
                GetToolWindow(typeof(DisassemblyToolWindow));
            }
            catch (Exception e)
            {
                if (!(GetService(typeof(SVsActivityLog)) is IVsActivityLog log)) return;
                log.LogEntry((uint)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR,
                    "OnSolutionOpen", $"Exception raised: {e}");
            }
        }

        /// <summary>
        /// Cleans up after closing a solution
        /// </summary>
        private void OnSolutionClosed()
        {
            try
            {
                // --- When the current solution has been closed,
                // --- stop the virtual machine and clean up
                SolutionClosed?.Invoke(this, EventArgs.Empty);
                DebugInfoProvider.Clear();
                MachineViewModel?.Stop();
            }
            catch (Exception e)
            {
                if (!(GetService(typeof(SVsActivityLog)) is IVsActivityLog log)) return;
                log.LogEntry((uint)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR,
                    "OnSolutionClosed", $"Exception raised: {e}");
            }
        }

        /// <summary>
        /// Creates a machine view model with the currently loaded Spectrum model
        /// </summary>
        /// <returns>The view model that represents the machine</returns>
        private MachineViewModel CreateProjectMachine()
        {
            var machine = SpectrumMachine.CreateMachine(
                CodeDiscoverySolution.CurrentProject.ModelName,
                CodeDiscoverySolution.CurrentProject.EditionName);
            machine.ExecuteOnMainThread = ExecuteOnMainThread;

            // --- Set up diagnostics
            machine.VmStoppedWithException += MachineOnVmStoppedWithException;
            if (Options.LogIoAccess && machine.SpectrumVm.PortDevice is GenericPortDeviceBase genPort)
            {
                genPort.PortAccessLogger = new PortAccessLogger();
            }
            if (Options.LogNextRegAccess && machine.SpectrumVm.NextDevice is NextFeatureSetDevice nextDev)
            {
                nextDev.RegisterAccessLogger = new NextRegisterAccessLogger();
            }
            if (machine.SpectrumVm.FloppyDevice is FloppyDevice floppy)
            {
                floppy.FloppyLogger = new FloppyDeviceLogger();
            }

            var vm = new MachineViewModel(machine)
            {
                AllowKeyboardScan = true,
                StackDebugSupport = new SimpleStackDebugSupport(),
                DisplayMode = SpectrumDisplayMode.Fit,
            };
            return vm;
        }

        /// <summary>
        /// Logs an exception that stopped the virtual machine
        /// </summary>
        private static void MachineOnVmStoppedWithException(object sender, EventArgs eventArgs)
        {
            if (sender is SpectrumMachine machine)
            {
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                pane.WriteLine($"The ZX Spectrum virtual machine has stopped because of an exception\n{machine.ExecutionCycleException}");
            }
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

        /// <summary>
        /// Signs the the specified test file has been changed
        /// </summary>
        /// <param name="filename">Test file name</param>
        public void OnTestFileChanged(string filename)
        {
            TestFileChanged?.Invoke(this, new FileChangedEventArgs(filename));
        }

        #region Helpers

        /// <summary>
        /// Executes the specified action on the UI thread
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static async System.Threading.Tasks.Task ExecuteOnMainThread(Action action)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            action();
        }

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
            return SpectrumModels.IsModelCompatibleWith(modelName, type);
        }

        /// <summary>
        /// Gets the identifier of the currently used Spectrum model
        /// </summary>
        /// <returns></returns>
        public static SpectrumModelType GetCurrentSpectrumModelType()
        {
            var modelName = Default.CodeDiscoverySolution?.CurrentProject?.ModelName;
            return SpectrumModels.GetModelTypeFromName(modelName);
        }

        /// <summary>
        /// This class logs I/O port events
        /// </summary>
        private class PortAccessLogger : IPortAccessLogger
        {
            public void PortRead(ushort addr, byte value, bool handled)
            {
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                pane.WriteLine($"Port {addr:X4} read. Value: {value:X2}. Handled: {handled}");
            }

            public void PortWritten(ushort addr, byte value, bool handled)
            {
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                pane.WriteLine($"Port {addr:X4} written. Value: {value:X2}. Handled: {handled}");
            }
        }

        /// <summary>
        /// Tis class logs NEXT register events
        /// </summary>
        private class NextRegisterAccessLogger : INextRegisterAccessLogger
        {
            public void RegisterIndexSet(byte index)
            {
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                pane.WriteLine($"Next register index set: {index:X2}");
            }

            public void RegisterValueSet(byte value)
            {
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                pane.WriteLine($"Next register value set: {value:X2}");
            }

            public void RegisterValueObtained(byte value)
            {
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                pane.WriteLine($"Next register value obtained: {value:X2}");
            }
        }

        private class FloppyDeviceLogger : IFloppyDeviceLogger
        {
            /// <summary>
            /// Allows to trace a floppy message
            /// </summary>
            /// <param name="message"></param>
            public void Trace(string message)
            {
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                pane.WriteLine($"Floppy event: {message}");
            }

            /// <summary>
            /// A new command byte has been received
            /// </summary>
            /// <param name="cmd"></param>
            public void CommandReceived(byte cmd)
            {
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                pane.WriteLine($"FDC command: {cmd:X2}");
            }

            /// <summary>
            /// Command parameters received by the controller
            /// </summary>
            /// <param name="pars"></param>
            public void CommandParamsReceived(byte[] pars)
            {
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                pane.Write("FDC pars: ");
                foreach (var p in pars)
                {
                    pane.Write($"{p:X2} ");
                }
                pane.WriteLine();
            }

            /// <summary>
            /// Data received by the controller
            /// </summary>
            /// <param name="data"></param>
            public void DataReceived(byte[] data)
            {
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                pane.WriteLine("FDC data received: ");
                var count = 1;
                foreach (var p in data)
                {
                    pane.Write($"{p:X2} ");
                    if (count % 16 == 0)
                    {
                        pane.WriteLine();
                    }
                    count++;
                }
                pane.WriteLine();
            }

            /// <summary>
            /// Data sent back by the controller
            /// </summary>
            /// <param name="data"></param>
            public void DataSent(byte[] data)
            {
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                pane.WriteLine("FDC data sent back: ");
                var count = 1;
                foreach (var p in data)
                {
                    pane.Write($"{p:X2} ");
                    if (count % 16 == 0)
                    {
                        pane.WriteLine();
                    }
                    count++;
                }
                pane.WriteLine();
            }

            /// <summary>
            /// Result sent back by the controller
            /// </summary>
            /// <param name="result"></param>
            public void ResultSent(byte[] result)
            {
                var pane = OutputWindow.GetPane<SpectrumVmOutputPane>();
                pane.Write("FDC result: ");
                foreach (var p in result)
                {
                    pane.Write($"{p:X2} ");
                }
                pane.WriteLine();
            }
        }

        #endregion Helpers
    }
}