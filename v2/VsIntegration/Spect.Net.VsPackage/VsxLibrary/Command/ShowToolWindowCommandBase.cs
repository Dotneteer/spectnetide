using System;
using System.Reflection;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.VsxLibrary.Command
{
    /// <summary>
    /// This class implements a command that shows a singleton tool window
    /// </summary>
    public abstract class ShowToolWindowCommandBase: SpectNetCommandBase
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
        /// Override this method to execute the command
        /// </summary>
        protected override void OnExecute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (ToolWindowType == null) return;
            var window = SpectNetPackage.Default.GetToolWindow(ToolWindowType);
            var windowFrame = (IVsWindowFrame)window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}