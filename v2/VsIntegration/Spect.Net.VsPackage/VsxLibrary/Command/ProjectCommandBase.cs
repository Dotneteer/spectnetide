using Spect.Net.VsPackage.SolutionItems;
using System.Collections.Generic;

namespace Spect.Net.VsPackage.VsxLibrary.Command
{
    /// <summary>
    /// This class represents a command that can be associated with a ZX Spectrum
    /// Code Discovery project file.
    /// </summary>
    public abstract class ProjectCommandBase : SingleProjectItemCommandBase
    {
        /// <summary>
        /// We need to allow project items.
        /// </summary>
        public override bool AllowProjectItem => true;

        /// <summary>
        /// This command accepts only Z80 code files
        /// </summary>
        public override IEnumerable<string> ItemExtensionsAccepted =>
            new[]
            {
                VsHierarchyTypes.PROJECT_EXT
            };
    }
}