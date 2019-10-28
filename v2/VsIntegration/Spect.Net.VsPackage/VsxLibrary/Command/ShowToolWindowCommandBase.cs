using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;
using System;
using System.Reflection;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.VsxLibrary.Command
{
    /// <summary>
    /// This class implements a command that shows a singleton tool window
    /// </summary>
    public abstract class ShowToolWindowCommandBase : SpectNetCommandBase
    {
        /// <summary>
        /// Type of tool window to display
        /// </summary>
        public Type ToolWindowType { get; }

        /// <summary>
        /// Initializes the command
        /// </summary>
        protected ShowToolWindowCommandBase()
        {
            var twAttr = GetType().GetTypeInfo().GetCustomAttribute<ToolWindowAttribute>();
            if (twAttr != null)
            {
                ToolWindowType = twAttr.Type;
            }
        }

        /// <summary>
        /// Override this method to define how to prepare the command on the
        /// main thread of Visual Studio
        /// </summary>
        protected override async Task ExecuteOnMainThreadAsync()
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);
            if (ToolWindowType != null)
            {
                var window = SpectNetPackage.Default.GetToolWindow(ToolWindowType);
                var windowFrame = (IVsWindowFrame)window.Frame;
                ErrorHandler.ThrowOnFailure(windowFrame.Show());
            }
        }
    }
}