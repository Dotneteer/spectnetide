using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.VsPackage.Tools.RegistersTool;

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class DisassemblyToolWindowCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int DISASSEMBLY_TW_COMMAND_ID = 0x1200;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisassemblyToolWindowCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private DisassemblyToolWindowCommand(Package package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService == null) return;

            var menuCommandId = new CommandID(SpectNetPackage.CommandSet, DISASSEMBLY_TW_COMMAND_ID);
            var menuItem = new MenuCommand(ShowToolWindow, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static DisassemblyToolWindowCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new DisassemblyToolWindowCommand(package);
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            var window = _package.FindToolWindow(typeof(DisassemblyToolWindow), 0, true);
            if (window?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}