using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.VsPackage.LanguageServices.Z80Asm;
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
    [ProvideAutoLoad(UIContextGuids.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]

    public sealed class SpectNetPackage : AsyncPackage
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


        public static Z80AsmLanguageService Z80AsmLanguage { get; private set; }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Z80AsmLanguage = new Z80AsmLanguageService(this);
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }
    }
}
