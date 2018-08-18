using System.Threading.Tasks;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs.Commands;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Sets the specified annotation file as the default one to use
    /// </summary>
    [CommandId(0x0804)]
    public class SetAsDefaultAnnotationFileCommand : AnnotationCommandBase
    {
        /// <summary>
        /// Override this method to define the async command body te execute on the
        /// background thread
        /// </summary>
        protected override Task ExecuteAsync()
        {
            Package.CodeDiscoverySolution.CurrentProject.SetDefaultAnnotationItem(this);
            return Task.FromResult(0);
        }

        /// <summary>
        /// We conclude the command with sending the message to
        /// notify any views that use the annotation file
        /// </summary>
        protected override Task FinallyOnMainThreadAsync()
        {
            Package.CodeManager.RaiseAnnotationFileChanged();
            return Task.FromResult(0);
        }
    }
}