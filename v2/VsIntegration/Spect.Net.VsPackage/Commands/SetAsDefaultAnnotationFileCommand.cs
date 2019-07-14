using System.Collections.Generic;
using System.Threading.Tasks;
using Spect.Net.VsPackage.SolutionItems;
using Spect.Net.VsPackage.VsxLibrary.Command;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Sets the specified annotation file as the default one to use
    /// </summary>
    [CommandId(0x0804)]
    public class SetAsDefaultAnnotationFileCommand : SingleProjectItemCommandBase
    {
        /// <summary>
        /// This command accepts only annotation files
        /// </summary>
        public override IEnumerable<string> ItemExtensionsAccepted =>
            new[] { VsHierarchyTypes.DISANN_ITEM };

        /// <summary>
        /// Override this method to define the async command body te execute on the
        /// background thread
        /// </summary>
        protected override Task ExecuteAsync()
        {
            SpectNetPackage.Default.ActiveProject.SetDefaultAnnotationItem(this);
            return Task.FromResult(0);
        }

        /// <summary>
        /// We conclude the command with sending the message to
        /// notify any views that use the annotation file
        /// </summary>
        protected override Task FinallyOnMainThreadAsync()
        {
            // TODO: Raise this event
            //Package.CodeManager.RaiseAnnotationFileChanged();
            return Task.FromResult(0);
        }
    }
}