using System;
using System.ComponentModel.Design;
using System.Reflection;
using Microsoft.VisualStudio.Shell;

namespace Spect.Net.VsPackage.VsxLibrary.Command
{
    /// <summary>
    /// This class represents a UI command in SpectNetIDE
    /// </summary>
    public abstract class SpectNetCommandBase
    {
        /// <summary>
        /// The id of the command
        /// </summary>
        public int CommandId { get; }

        /// <summary>
        /// Obtains the OleMenuCommand behind this command
        /// </summary>
        public OleMenuCommand OleMenuCommand { get; }

        /// <summary>
        /// Sites the instance of this class
        /// </summary>
        /// <remarks>
        /// Commands should be initialized only from the main thread.
        /// </remarks>
        protected SpectNetCommandBase()
        {
            // --- Obtain and store command properties
            var idAttr = GetType().GetTypeInfo().GetCustomAttribute<CommandIdAttribute>();
            if (idAttr != null)
            {
                CommandId = idAttr.Value;
            }

            // --- Register the command
            var package = SpectNetPackage.Default;
            if (!(((IServiceProvider)package).GetService(
                typeof(IMenuCommandService)) is OleMenuCommandService commandService))
            {
                return;
            }

            var commandId = new CommandID(new Guid(SpectNetPackage.COMMAND_SET_GUID), CommandId);
            OleMenuCommand = new OleMenuCommand((s, e) => { OnExecute(); }, commandId);
            OleMenuCommand.BeforeQueryStatus += (s, e) =>
            {
                if (s is OleMenuCommand mc)
                {
                    OnQueryStatus(mc);
                }
            };
            commandService.AddCommand(OleMenuCommand);
        }

        /// <summary>
        /// Override this method to execute the command
        /// </summary>
        protected virtual void OnExecute()
        {
        }

        /// <summary>
        /// Override this method to define the status query action
        /// </summary>
        /// <param name="mc"></param>
        protected virtual void OnQueryStatus(OleMenuCommand mc)
        {
        }
    }
}