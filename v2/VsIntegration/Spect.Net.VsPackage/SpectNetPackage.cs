using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.VsPackage.Debugging;
using Spect.Net.VsPackage.LanguageServices.Z80Asm;
using Spect.Net.VsPackage.LanguageServices.Z80Test;
using Spect.Net.VsPackage.VsxLibrary;
using Spect.Net.VsPackage.VsxLibrary.Output;
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
    [Guid(PACKAGE_GUID_STRING)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(
        "#110", 
        "#112", 
        "2.0.0", 
        IconResourceID = 400)]

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

    [ProvideAutoLoad(UIContextGuids.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class SpectNetPackage : VsxAsyncPackage
    {
        /// <summary>
        /// SpectNetPackage GUID string.
        /// </summary>
        public const string PACKAGE_GUID_STRING = "3690768a-3808-4afd-b5ff-db8b521e61f8";

        /// <summary>
        /// The GUID for this project type.  It is unique with the project file extension and
        /// appears under the VS registry hive's Projects key.
        /// </summary>
        public const string SPECTRUM_PROJECT_TYPE_GUID = "f16d4249-6279-474e-8826-742e7ff7445c";

        /// <summary>
        /// The singleton instance of this class, set in InitializeAsync
        /// </summary>
        public static SpectNetPackage Default { get; private set; }

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
            BreakpointChangeWatcher = new BreakpointChangeWatcher();
            BreakpointChangeWatcher.BreakpointsChanged += async (sender, args) =>
            {
                await OutputWindow.General.WriteLineAsync(
                    $"Added: {args.Added.Count}, modified: {args.Modified.Count} deleted: {args.Deleted.Count}");
            };
            BreakpointChangeWatcher.Start();


            // --- Main thread initialization part
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
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
                #pragma warning disable 4014
                BreakpointChangeWatcher.Stop();
                #pragma warning restore 4014
            }
        }
    }
}
