using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.ProjectResources;
using Spect.Net.SpectrumEmu;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Commands;
using Spect.Net.VsPackage.Compilers;
using Spect.Net.VsPackage.CustomEditors.RomEditor;
using Spect.Net.VsPackage.CustomEditors.TzxEditor;
using Spect.Net.VsPackage.Debugging;
using Spect.Net.VsPackage.LanguageServices.Z80Asm;
using Spect.Net.VsPackage.LanguageServices.Z80Test;
using Spect.Net.VsPackage.LanguageServices.ZxBasic;
using Spect.Net.VsPackage.Machines;
using Spect.Net.VsPackage.SolutionItems;
using Spect.Net.VsPackage.ToolWindows.BasicList;
using Spect.Net.VsPackage.ToolWindows.Disassembly;
using Spect.Net.VsPackage.ToolWindows.Keyboard;
using Spect.Net.VsPackage.ToolWindows.Memory;
using Spect.Net.VsPackage.ToolWindows.Registers;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.VsPackage.ToolWindows.StackTool;
using Spect.Net.VsPackage.ToolWindows.TapeFileExplorer;
using Spect.Net.VsPackage.ToolWindows.Watch;
using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.Output;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;
using Spect.Net.Wpf.Providers;
using OutputWindow = Spect.Net.VsPackage.VsxLibrary.Output.OutputWindow;
using Task = System.Threading.Tasks.Task;

#pragma warning disable IDE0067
#pragma warning disable VSTHRD100
#pragma warning disable VSTHRD010

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
    [Guid(PACKAGE_GUID)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(
        "#110", 
        "#112", 
        "2.0.10", 
        IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(PACKAGE_GUID, PackageAutoLoadFlags.BackgroundLoad)]

    // --- Tool windows
    [ProvideToolWindow(typeof(SpectrumEmulatorToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(KeyboardToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(RegistersToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(DisassemblyToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(MemoryToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(StackToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(BasicListToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(TapeFileExplorerToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(WatchToolWindow), Transient = true)]

    // --- Language Services
    [ProvideLanguageService(
        typeof(Z80AsmLanguageService), 
        Z80AsmLanguageService.LANGUAGE_NAME, 
        100, 
        ShowDropDownOptions = true, 
        DefaultToInsertSpaces = true, 
        EnableCommenting = true, 
        AutoOutlining = true, 
        MatchBraces = true, 
        MatchBracesAtCaret = true, 
        ShowMatchingBrace = true, 
        ShowSmartIndent = true)]
    [ProvideLanguageExtension(typeof(Z80AsmLanguageService), ".z80asm")]

    [ProvideLanguageService(
        typeof(Z80TestLanguageService),
        Z80TestLanguageService.LANGUAGE_NAME,
        101,
        ShowDropDownOptions = true,
        DefaultToInsertSpaces = true,
        EnableCommenting = true,
        AutoOutlining = true,
        MatchBraces = true,
        MatchBracesAtCaret = true,
        ShowMatchingBrace = true,
        ShowSmartIndent = true)]
    [ProvideLanguageExtension(typeof(Z80TestLanguageService), ".z80test")]

    [ProvideLanguageService(
        typeof(ZxBasicLanguageService),
        ZxBasicLanguageService.LANGUAGE_NAME,
        102,
        ShowDropDownOptions = true,
        DefaultToInsertSpaces = true,
        EnableCommenting = true,
        AutoOutlining = true,
        MatchBraces = true,
        MatchBracesAtCaret = true,
        ShowMatchingBrace = true,
        ShowSmartIndent = true)]
    [ProvideLanguageExtension(typeof(ZxBasicLanguageService), ".zxbas")]

    [ProvideProjectFactory(typeof(ZxSpectrumProjectFactory), 
        "ZX Spectrum Code Discovery Project",
        "ZX Spectrum Project Files (*.z80dcproj);*.z80cdproj", "z80cdproj", "z80cdproj",
        @"ProjectTemplates\ZX Spectrum", LanguageVsTemplate = "ZX Spectrum Code Discovery Project")]
    [ProvideOptionPage(typeof(SpectNetOptionsGrid), "Spect.Net IDE", "General options", 0, 0, true)]

    // --- Custom editors
    [ProvideEditorExtension(typeof(RomEditorFactory), RomEditorFactory.EXTENSION, 0x40)]
    [ProvideEditorLogicalView(typeof(RomEditorFactory), LogicalViewID.Designer)]
    [ProvideEditorExtension(typeof(TzxEditorFactory), TzxEditorFactory.EXTENSION, 0x40)]
    [ProvideEditorLogicalView(typeof(TzxEditorFactory), LogicalViewID.Designer)]
    [ProvideEditorExtension(typeof(TapEditorFactory), TapEditorFactory.EXTENSION, 0x40)]
    [ProvideEditorLogicalView(typeof(TapEditorFactory), LogicalViewID.Designer)]

    public sealed class SpectNetPackage : VsxAsyncPackage
    {
        /// <summary>
        /// SpectNetPackage GUID string.
        /// </summary>
        public const string PACKAGE_GUID = "3690768a-3808-4afd-b5ff-db8b521e61f8";

        /// <summary>
        /// The GUID for this project type.  It is unique with the project file extension and
        /// appears under the VS registry hive's Projects key.
        /// </summary>
        public const string SPECTRUM_PROJECT_TYPE_GUID = "f16d4249-6279-474e-8826-742e7ff7445c";

        /// <summary>
        /// The GUID for the command set of this package.
        /// </summary>
        public const string COMMAND_SET_GUID = "234580c4-8a2c-4ae1-8e4f-5bc708b188fe";

        /// <summary>
        /// The GUID used by the project factory
        /// </summary>
        public const string PROJECT_FACTORY_GUID = "7F8C1ABC-9735-4A03-A7A7-9867750FDF50";

        /// <summary>
        /// The base URL for command help topics
        /// </summary>
        public const string COMMANDS_BASE_URL = "https://dotneteer.github.io/spectnetide/documents";

        /// <summary>
        /// CPS deployment strings
        /// </summary>
        public const string CPS_FOLDER = @"CustomProjectSystems\Spect.Net.CodeDiscover";
        public const string CPS_VERSION_FILE = "cps.version";
        public const string CURRENT_CPS_VERSION = "2.0.10";
        public const string CPS_RESOURCE_PREFIX = "Spect.Net.ProjectResources";
        public const string CPS_RULES = "Rules";

        /// <summary>
        /// The singleton instance of this class, set in InitializeAsync
        /// </summary>
        public static SpectNetPackage Default { get; private set; }

        /// <summary>
        /// Gets the options of this package
        /// </summary>
        public SpectNetOptionsGrid Options
            => GetDialogPage(typeof(SpectNetOptionsGrid)) as SpectNetOptionsGrid;

        /// <summary>
        /// An instance of the Z80 Assembly language service
        /// </summary>
        public static Z80AsmLanguageService Z80AsmLanguage { get; private set; }

        /// <summary>
        /// This object is responsible to watch breakpoint changes while the package
        /// is loaded
        /// </summary>
        public BreakpointChangeWatcher BreakpointChangeWatcher { get; private set; }

        /// <summary>
        /// Gets the current solution structure
        /// </summary>
        public SolutionStructure Solution { get; private set; }

        /// <summary>
        /// Gets the current ZX Spectrum project
        /// </summary>
        public SpectrumProject ActiveProject => Solution?.ActiveProject;

        /// <summary>
        /// Gets the current project root
        /// </summary>
        public Project ActiveRoot => ActiveProject?.Root;

        /// <summary>
        /// Indicates if compilation is in progress.
        /// </summary>
        public bool CompilationInProgress { get; set; }

        /// <summary>
        /// The error list provider accessible from this package
        /// </summary>
        public ErrorListWindow ErrorList { get; private set; }

        /// <summary>
        /// Gets the task list provider for this package.
        /// </summary>
        public TaskListWindow TaskList { get; private set; }

        /// <summary>
        /// Contains the view model for the singleton ZX Spectrum Emulator
        /// </summary>
        public EmulatorViewModel EmulatorViewModel { get; private set; }

        /// <summary>
        /// Contains the view model for the singleton ZX Spectrum Emulator tool window
        /// </summary>
        public SpectrumEmulatorToolWindowViewModel SpectrumViewModel { get; private set; }

        /// <summary>
        /// Contains the view model for the ZX Spectrum keyboard
        /// </summary>
        public KeyboardToolWindowViewModel KeyboardToolModel { get; private set; }

        /// <summary>
        /// Contains the view model for the Z80 Registers tool window
        /// </summary>
        public RegistersToolWindowViewModel RegistersViewModel { get; private set; }

        /// <summary>
        /// Contains the view model for the ZX Spectrum tool window
        /// </summary>
        public MemoryToolWindowViewModel MemoryViewModel { get; private set; }

        /// <summary>
        /// Contains the view model for the Z80 Disassembly tool window
        /// </summary>
        public DisassemblyToolWindowViewModel DisassemblyViewModel { get; private set; }

        /// <summary>
        /// Contains the view model for the Z80 Stack tool window
        /// </summary>
        public StackToolWindowViewModel StackViewModel { get; private set; }

        /// <summary>
        /// Contains the view model for the ZX Spectrum BASIC tool window
        /// </summary>
        public BasicListToolWindowViewModel BasicListViewModel { get; private set; }

        /// <summary>
        /// Contains the view model for the Watch memory tool window
        /// </summary>
        public WatchToolWindowViewModel WatchViewModel { get; private set; }

        /// <summary>
        /// Provides debug information while running the Spectrum virtual machine
        /// </summary>
        public VsIntegratedSpectrumDebugInfoProvider DebugInfoProvider { get; private set; }

        /// <summary>
        /// Use this object to raise events related to code
        /// </summary>
        public readonly CodeManager CodeManager = new CodeManager();

        #region Package lifecycle

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            // --- Background initialization part
            Default = this;

            // --- Prepare project system extension files
            CheckCpsFiles();

            Z80AsmLanguage = new Z80AsmLanguageService(this);

            // --- Special providers for the ZX Spectrum virtual machine
            DebugInfoProvider = new VsIntegratedSpectrumDebugInfoProvider();
            SpectrumMachine.Reset();
            SpectrumMachine.RegisterDefaultProviders();
            SpectrumMachine.RegisterProvider<IRomProvider>(() => new PackageRomProvider());
            SpectrumMachine.RegisterProvider<IBeeperProvider>(() => new AudioWaveProvider());
            SpectrumMachine.RegisterProvider<ISoundProvider>(() => new AudioWaveProvider(AudioProviderType.Psg));
            SpectrumMachine.RegisterProvider<ITapeLoadProvider>(() => new VsIntegratedTapeLoadProvider());
            SpectrumMachine.RegisterProvider<ITapeSaveProvider>(() => new VsIntegratedTapeSaveProvider());
            SpectrumMachine.RegisterProvider<IKempstonProvider>(() => new KempstonProvider());
            SpectrumMachine.RegisterProvider<ISpectrumDebugInfoProvider>(() => DebugInfoProvider);
            SpectrumMachine.RegisterStackDebugSupport(new SimpleStackDebugSupport());

            // --- Initialize other services
            BreakpointChangeWatcher = new BreakpointChangeWatcher();
            BreakpointChangeWatcher.Start();

            // --- Main thread initialization part
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // --- Initialize the package here
            InitializeCommands();

            // --- Register custom editors
            RegisterEditorFactory(new RomEditorFactory());
            RegisterEditorFactory(new TzxEditorFactory());
            RegisterEditorFactory(new TapEditorFactory());

            // --- Manage the solution and its events
            Solution = new SolutionStructure();
            Solution.ActiveProjectChanged += OnActiveProjectChanged;
            ErrorList = new ErrorListWindow();
            TaskList = new TaskListWindow();

            // --- Create view models
            EmulatorViewModel = new EmulatorViewModel();
            SpectrumViewModel = new SpectrumEmulatorToolWindowViewModel();
            KeyboardToolModel = new KeyboardToolWindowViewModel();
            RegistersViewModel = new RegistersToolWindowViewModel();
            MemoryViewModel = new MemoryToolWindowViewModel();
            DisassemblyViewModel = new DisassemblyToolWindowViewModel();
            StackViewModel = new StackToolWindowViewModel();
            BasicListViewModel = new BasicListToolWindowViewModel();
            WatchViewModel = new WatchToolWindowViewModel();

            // --- Prepare the provider for debugging
            DebugInfoProvider.Prepare();

            // --- Prepare language services
            Z80AsmClassifierProvider.AttachToPackage();
            Z80AsmViewTaggerProvider.AttachToPackage();

            // --- Prepare options
            _ = Options;
            Log("SpectNetIdePackage initialized.");
        }

        /// <summary>
        /// Initializes all commands that can be used within SpectNetIDE
        /// </summary>
        private static void InitializeCommands()
        {
            // ReSharper disable ObjectCreationAsStatement

            // --- Main menu commands
            new ShowSpectrumEmulatorCommand();
            SpectrumEmulatorToolWindow.InitializeToolbarCommands();
            new ShowSpectrumKeyboardCommand();
            new ShowRegistersCommand();
            new ShowDisassemblyCommand();
            new ShowMemoryCommand();
            new ShowZ80CpuStackCommand();
            StackToolWindow.InitializeToolbarCommands();
            new ShowBasicListCommand();
            BasicListToolWindow.InitializeToolbarCommands();
            new ShowTapeExplorerCommand();
            TapeFileExplorerToolWindow.InitializeToolbarCommands();
            new ShowWatchMemoryCommand();

            // --- Solution Explorer project commands
            new SetAsActiveProjectCommand();
            new RunDefaultProgramCommand();
            new CompileDefaultCodeCommand();
            new DebugDefaultProgramCommand();
            new InjectDefaultProgramCommand();
            new ExportDefaultProgramCommand();
            new CompileAllUnitTestsCommand();
            new RunAllUnitTestsCommand();

            // --- Solution Explorer item commands
            new SetAsDefaultCodeFileCommand();
            new RunProgramCommand();
            new DebugProgramCommand();
            new CompileCodeCommand();
            new InjectProgramCommand();
            new ExportProgramCommand();
            new CompileUnitTestCommand();
            new RunUnitTestCommand();
            new LoadVmStateCommand();
            new SetAsDefaultTapeFileCommand();
            new SetAsDefaultAnnotationFileCommand();
            new InsertDriveACommand();
            new InsertDriveBCommand();
            new EjectFloppyCommand();
            new WriteProtectFloppyCommand();
            new CreateVfddFileCommand();

            // ReSharper restore ObjectCreationAsStatement
        }

        /// <devdoc>
        /// This method will be called by Visual Studio in response to a package close
        /// (disposing will be true in this case).  The default implementation revokes all
        /// services and calls Dispose() on any created services that implement IDisposable.
        /// </devdoc>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Solution.Dispose();
                _ = BreakpointChangeWatcher.StopAsync();
                Z80AsmClassifierProvider.DetachFromPackage();
                Z80AsmViewTaggerProvider.DetachFromPackage();
            }
            base.Dispose(disposing);
        }

        #endregion

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
        /// Logs a message to the SpectNetIDE output pane
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void Log(string message)
        {
            var pane = OutputWindow.GetPane<SpectNetIdeOutputPane>();
            pane.WriteLine(message);
        }

        /// <summary>
        /// Tests if the current model is a ZX Spectrum 48K
        /// </summary>
        /// <returns>True, if the current model = ZX Spectrum 48K; otherwise, false</returns>
        public static bool IsSpectrum48Model()
        {
            return Default.Solution?.ActiveProject?.ModelName == SpectrumModels.ZX_SPECTRUM_48;
        }

        /// <summary>
        /// Handles the event when the active project changes within the solution
        /// </summary>
        private async void OnActiveProjectChanged(object sender, ActiveProjectChangedEventArgs e)
        {
            // --- Suspend the old machine
            if (e.OldProject != null)
            {
                var oldMachine = Solution.Machines.GetMachine(e.OldProject.Root);
                if (oldMachine != null)
                {
                    await oldMachine.Pause();
                }
            }

            // --- Set the new machine
            if (e.NewProject != null)
            {
                var newMachine = Solution.Machines.GetOrCreateMachine(e.NewProject.Root,
                    e.NewProject.ModelName,
                    e.NewProject.EditionName);
                if (newMachine != null)
                {
                    EmulatorViewModel.SetMachine(newMachine);
                    WatchViewModel.EvalContext = 
                        new SymbolAwareSpectrumEvaluationContext(EmulatorViewModel.Machine.SpectrumVm);
                }
            }
        }

        /// <summary>
        /// Checks CPS files, and refreshes them when it's time
        /// </summary>
        private static void CheckCpsFiles()
        {
            var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var cpsFolder = Path.Combine(localAppDataFolder, CPS_FOLDER);
            if (!Directory.Exists(cpsFolder))
            {
                Directory.CreateDirectory(cpsFolder);
            }

            var versionFile = Path.Combine(cpsFolder, CPS_VERSION_FILE);
            var logFile = Path.Combine(cpsFolder, "cpslog.txt");
            if (File.Exists(versionFile))
            {
                var content = File.ReadAllText(versionFile);
                var parts = content.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0 && parts[0] == CURRENT_CPS_VERSION) return;
            }

            // --- Refresh CPS files
            var asm = typeof(ResourceMarker).Assembly;
            var resources = asm.GetManifestResourceNames();
            File.WriteAllText(logFile, string.Join("\n", resources));
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
    }

    /// <summary>
    /// This class is used to register the ZX Spectrum project factory
    /// </summary>
    [Guid(SpectNetPackage.PROJECT_FACTORY_GUID)]
    public class ZxSpectrumProjectFactory
    {
    }
}

#pragma warning restore IDE0067 // Dispose objects before losing scope
#pragma warning restore VSTHRD100
#pragma warning restore VSTHRD010
