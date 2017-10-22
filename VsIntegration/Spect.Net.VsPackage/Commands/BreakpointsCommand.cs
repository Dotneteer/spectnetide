using EnvDTE;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Displays the BASIC List tool window
    /// </summary>
    [CommandId(0x1800)]
    public class BreakpointsCommand :
        VsxCommand<SpectNetPackage, SpectNetCommandSet>
    {
        /// <summary>
        /// Override this method to execute the command
        /// </summary>
        protected override void OnExecute()
        {
            var db = Package.ApplicationObject.Debugger;
            foreach (Breakpoint bp in db.Breakpoints)
            {
                
            }
        }
    }
}