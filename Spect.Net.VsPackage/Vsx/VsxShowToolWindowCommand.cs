using System;
using System.Reflection;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This class implements a command that shows a singleton tool window
    /// </summary>
    /// <typeparam name="TPackage"></typeparam>
    /// <typeparam name="TCommandSet"></typeparam>
    public abstract class VsxShowToolWindowCommand<TPackage, TCommandSet> : VsxCommand<TPackage, TCommandSet> 
        where TPackage : VsxPackage 
        where TCommandSet : VsxCommandSet<TPackage>
    {
        /// <summary>
        /// Type of tool window to display
        /// </summary>
        public Type ToolWindowType { get; private set; }

        /// <summary>
        /// Sites the instance of this class
        /// </summary>
        /// <param name="commandSet">The command set that holds this command</param>
        public override void Site(IVsxCommandSet commandSet)
        {
            base.Site(commandSet);
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
            if (ToolWindowType == null) return;
            var window = Package.FindToolWindow(ToolWindowType, 0, true);
            if (window?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}