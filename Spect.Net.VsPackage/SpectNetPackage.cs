using System;
using System.Runtime.InteropServices;
using EnvDTE;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.Messages;
using Spect.Net.VsPackage.SpectrumEmulator;
using Spect.Net.VsPackage.Tools.Disassembly;
using Spect.Net.VsPackage.Tools.RegistersTool;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage
{
    /// <summary>
    /// This class provides a Visual Studio package for the Spect.NET IDE.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PACKAGE_GUID_STRING)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(SpectrumEmulatorToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(RegistersToolWindow), Transient = true)]
    [ProvideToolWindow(typeof(DisassemblyToolWindow), Transient = true)]
    public sealed class SpectNetPackage : VsxPackage
    {
        /// <summary>
        /// SpectNetPackage GUID string.
        /// </summary>
        public const string PACKAGE_GUID_STRING = "1b214806-bc31-49bd-be5d-79ac4a189f3c";

        /// <summary>
        /// Command set of the package
        /// </summary>
        public const string PACKAGE_COMMAND_SET = "234580c4-8a2c-4ae1-8e4f-5bc708b188fe";

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid(PACKAGE_COMMAND_SET);

        private DTEEvents _packageDteEvents;

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void OnInitialize()
        {
            // --- Initialize the main menu commands
            //SpectrumEmulatorToolWindowCommand.Initialize(this);
            RegistersToolWindowCommand.Initialize(this);
            DisassemblyToolWindowCommand.Initialize(this);

            // --- Prepare for pacakge shutdown
            _packageDteEvents = ApplicationObject.Events.DTEEvents;
            _packageDteEvents.OnBeginShutdown += () =>
            {
                Messenger.Default.Send(new PackageShutdownMessage());
            };
        }

        /// <summary>
        /// Displays the Spectrum emulator tool window
        /// </summary>
        [CommandId(0x1000)]
        [ToolWindow(typeof(SpectrumEmulatorToolWindow))]
        public class ShowSpectrumEmulatorCommand : 
            VsxShowToolWindowCommand<SpectNetPackage, SpectNetCommandSet>
        {
        }
    }
}
