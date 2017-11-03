using System.Collections.Generic;

namespace Spect.Net.VsPackage.Z80Programs.Commands
{
    /// <summary>
    /// This class represents a command that can be associated with
    /// an annotation file.
    /// </summary>
    public abstract class AnnotationCommandBase : SingleProjectItemCommandBase
    {
        /// <summary>
        /// This command accepts only Z80 code files
        /// </summary>
        public override IEnumerable<string> ItemExtensionsAccepted =>
            new[] { ".disann" };
    }
}