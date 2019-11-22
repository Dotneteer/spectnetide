using Microsoft.VisualStudio.Shell;
using Spect.Net.VsPackage.VsxLibrary.Command;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Sets the specified annotation file as the default one to use
    /// </summary>
    [CommandId(0x0805)]
    public class SetAsDefaultCodeFileCommand : SpectrumProgramCommandBase
    {
        /// <summary>
        /// Override this method to define the status query action
        /// </summary>
        /// <param name="mc"></param>
        protected override void OnQueryStatus(OleMenuCommand mc)
        {
            base.OnQueryStatus(mc);
            if (!mc.Visible) return;
            var item = ItemPath;
            var dteItem = SpectNetPackage.Default.Solution.GetProjectOfFile(item)?.GetDteProjectItem(item);
            if (dteItem == null || dteItem.Properties.Item("IsDependentFile").Value.Equals(true))
            {
                mc.Visible = false;
            }
        }

        /// <summary>
        /// Override this method to define the async command body te execute on the
        /// background thread
        /// </summary>
        protected override Task ExecuteAsync()
        {
            SpectrumProject?.SetDefaultCodeItem(this);
            return Task.FromResult(0);
        }
    }
}