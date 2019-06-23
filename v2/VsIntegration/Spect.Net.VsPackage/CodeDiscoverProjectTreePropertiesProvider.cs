using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ProjectSystem;

namespace ZXSpectrumCodeDiscover
{
    /// <summary>
    /// Updates nodes in the project tree by overriding property values calculated so far by lower priority providers.
    /// </summary>
    [Export(typeof(IProjectTreePropertiesProvider))]
    [AppliesTo(MyUnconfiguredProject.UNIQUE_CAPABILITY)]
    public class CodeDiscoverProjectTreePropertiesProvider : IProjectTreePropertiesProvider
    {
        /// <summary>
        /// Calculates new property values for each node in the project tree.
        /// </summary>
        /// <param name="propertyContext">Context information that can be used for the calculation.</param>
        /// <param name="propertyValues">Values calculated so far for the current node by lower priority tree properties providers.</param>
        public void CalculatePropertyValues(
            IProjectTreeCustomizablePropertyContext propertyContext,
            IProjectTreeCustomizablePropertyValues propertyValues)
        {
            // Only set the icon for the root project node.  We could choose to set different icons for nodes based
            // on various criteria, not just Capabilities, if we wished.
            if (propertyValues.Flags.Contains(ProjectTreeFlags.Common.ProjectRoot))
            {
                propertyValues.Icon = ImageMonikers.ProjectIconImageMoniker.ToProjectSystemType();
            }
        }
    }
}