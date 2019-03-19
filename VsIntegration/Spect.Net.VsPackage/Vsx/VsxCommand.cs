using System;
using System.ComponentModel.Design;
using System.Reflection;
using Microsoft.VisualStudio.Shell;

namespace Spect.Net.VsPackage.Vsx
{
    /// <summary>
    /// This class represents a command that belongs to the
    /// specified command set within a particular package
    /// </summary>
    /// <typeparam name="TPackage">Package type</typeparam>
    /// <typeparam name="TCommandSet">Command set type</typeparam>
    public abstract class VsxCommand<TPackage, TCommandSet> : IVsxCommand
        where TPackage: VsxPackage
        where TCommandSet: VsxCommandSet<TPackage>
    {
        /// <summary>
        /// Gets the package that holds this command
        /// </summary>
        public TPackage Package { get; private set; }

        /// <summary>
        /// Gets the command set that holds this command
        /// </summary>
        public TCommandSet CommandSet { get; private set; }

        /// <summary>
        /// The id of the command
        /// </summary>
        public int CommandId { get; private set; }

        /// <summary>
        /// Obtains the OleMenuCommand behind this VsxCommand
        /// </summary>
        public OleMenuCommand OleMenuCommand { get; private set; }

        /// <summary>
        /// Sites the instance of this class
        /// </summary>
        /// <param name="commandSet">The command set that holds this command</param>
        public virtual async void Site(IVsxCommandSet commandSet)
        {
            // --- Obtain and store command properties
            CommandSet = (TCommandSet)commandSet;
            Package = CommandSet.Package;
            var idAttr = GetType().GetTypeInfo().GetCustomAttribute<CommandIdAttribute>();
            if (idAttr != null)
            {
                CommandId = idAttr.Value;
            }

            // --- Register the command
            await Package.JoinableTaskFactory.SwitchToMainThreadAsync();
            var commandService = ((IServiceProvider)Package).GetService(
                typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService == null) return;

            var commandId = new CommandID(CommandSet.Guid, CommandId);
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
        /// Gets the type of command set
        /// </summary>
        Type IVsxCommand.CommandSetType => typeof(TCommandSet);

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