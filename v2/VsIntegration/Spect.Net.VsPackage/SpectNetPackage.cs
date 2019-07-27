using System;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.SpectrumEmu;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.Commands;
using Spect.Net.VsPackage.Debugging;
using Spect.Net.VsPackage.LanguageServices.Z80Asm;
using Spect.Net.VsPackage.LanguageServices.Z80Test;
using Spect.Net.VsPackage.Machines;
using Spect.Net.VsPackage.SolutionItems;
using Spect.Net.VsPackage.ToolWindows.Disassembly;
using Spect.Net.VsPackage.ToolWindows.Keyboard;
using Spect.Net.VsPackage.ToolWindows.Registers;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;
using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;
using Spect.Net.Wpf.Providers;
using OutputWindow = Spect.Net.VsPackage.VsxLibrary.Output.OutputWindow;
using Task = System.Threading.Tasks.Task;

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
        "2.0.0", 
        IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(PACKAGE_GUID, PackageAutoLoadFlags.BackgroundLoad)]

    // --- Tool windows
    [ProvideToolWindow(typeof(SpectrumEmulatorToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(KeyboardToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(RegistersToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(DisassemblyToolWindow), Transient = true)]

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
    [ProvideProjectFactory(typeof(ZxSpectrumProjectFactory), 
        "ZX Spectrum Code Discovery Project",
        "ZX Spectrum Project Files (*.z80dcproj);*.z80cdproj", "z80cdproj", "z80cdproj",
        @"ProjectTemplates\ZX Spectrum", LanguageVsTemplate = "ZX Spectrum Code Discovery Project")]
    [ProvideOptionPage(typeof(SpectNetOptionsGrid), "Spect.Net IDE", "General options", 0, 0, true)]
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
        /// Provides debug information while running the Spectrum virtual machine
        /// </summary>
        public VsIntegratedSpectrumDebugInfoProvider DebugInfoProvider { get; private set; }

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
            Z80AsmLanguage = new Z80AsmLanguageService(this);

            // --- Special providers for the ZX Spectrum virtual machine
            SpectrumMachine.Reset();
            SpectrumMachine.RegisterDefaultProviders();
            SpectrumMachine.RegisterProvider<IBeeperProvider>(() => new AudioWaveProvider());
            SpectrumMachine.RegisterProvider<ISoundProvider>(() => new AudioWaveProvider(AudioProviderType.Psg));
            SpectrumMachine.RegisterProvider<ITapeLoadProvider>(() => new VsIntegratedTapeLoadProvider());
            SpectrumMachine.RegisterProvider<ITapeSaveProvider>(() => new VsIntegratedTapeSaveProvider());
            EmulatorViewModel = new EmulatorViewModel();

            // --- Initialize other services
            BreakpointChangeWatcher = new BreakpointChangeWatcher();
            BreakpointChangeWatcher.BreakpointsChanged += async (sender, args) =>
            {
                await OutputWindow.General.WriteLineAsync(
                    $"Added: {args.Added.Count}, modified: {args.Modified.Count} deleted: {args.Deleted.Count}");
            };
            BreakpointChangeWatcher.Start();

            // --- Main thread initialization part
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // --- Initialize the package here
            InitializeCommands();

            // --- Manage the solution and its events
            Solution = new SolutionStructure();
            Solution.ActiveProjectChanged += OnActiveProjectChanged;
            ErrorList = new ErrorListWindow();
            TaskList = new TaskListWindow();
            DebugInfoProvider = new VsIntegratedSpectrumDebugInfoProvider();
        }

        /// <summary>
        /// Initializes all commands that can be used within SpectNetIDE
        /// </summary>
        private void InitializeCommands()
        {
            // ReSharper disable ObjectCreationAsStatement

            // --- Main menu commands
            new ShowSpectrumEmulatorCommand();
            SpectrumEmulatorToolWindow.InitializeToolbarCommands();
            new ShowSpectrumKeyboardCommand();
            new ShowRegistersCommand();
            new ShowDisassemblyCommand();

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

            // ReSharper restore ObjectCreationAsStatement
        }

        /// <summary>
        /// Tests if the current model is a ZX Spectrum 48K
        /// </summary>
        /// <returns>True, if the current model = ZX Spectrum 48K; otherwise, false</returns>
        public static bool IsSpectrum48Model()
        {
            return Default.Solution?.ActiveProject?.ModelName == SpectrumModels.ZX_SPECTRUM_48;
        }


        /// <devdoc>
        /// This method will be called by Visual Studio in response to a package close
        /// (disposing will be true in this case).  The default implementation revokes all
        /// services and calls Dispose() on any created services that implement IDisposable.
        /// </devdoc>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _ = BreakpointChangeWatcher.StopAsync();
            }
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
                }
            }
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
