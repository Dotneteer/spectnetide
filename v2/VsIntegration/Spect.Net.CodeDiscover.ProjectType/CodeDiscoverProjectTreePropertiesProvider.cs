using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ProjectSystem;

namespace ZXSpectrumCodeDiscover
{
    /// <summary>
    /// Updates nodes in the project tree by overriding property values calcuated so far by lower priority providers.
    /// </summary>
    [Export(typeof(IProjectTreePropertiesProvider))]
    [AppliesTo(MyUnconfiguredProject.UniqueCapability)]
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

            switch (propertyContext.ItemType)
            {
                case "DisassAnn":
                    propertyValues.Icon = ImageMonikers.DisassAnnIconImageMoniker.ToProjectSystemType();
                    break;
                case "Z80Asm":
                    propertyValues.Icon = ImageMonikers.Z80AsmIconImageMoniker.ToProjectSystemType();
                    break;
                case "Rom":
                    propertyValues.Icon = ImageMonikers.RomIconImageMoniker.ToProjectSystemType();
                    break;
                case "Tzx":
                    propertyValues.Icon = ImageMonikers.TzxIconImageMoniker.ToProjectSystemType();
                    break;
                case "Tap":
                    propertyValues.Icon = ImageMonikers.TapIconImageMoniker.ToProjectSystemType();
                    break;
                case "VmState":
                    propertyValues.Icon = ImageMonikers.VmStateIconImageMoniker.ToProjectSystemType();
                    break;
                case "SpConf":
                    propertyValues.Icon = ImageMonikers.SpConfIconImageMoniker.ToProjectSystemType();
                    break;
                case "Z80Test":
                    propertyValues.Icon = ImageMonikers.Z80TestIconImageMoniker.ToProjectSystemType();
                    break;
                case "Vfdd":
                    propertyValues.Icon = ImageMonikers.FloppyIconImageMoniker.ToProjectSystemType();
                    break;
            }
        }
    }
}