using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.Shell.Interop;

namespace ZXSpectrumCodeDiscover
{
    /// <summary>
    /// Updates nodes in the project tree by overriding property values calcuated so far by lower priority providers.
    /// </summary>
    [Export(typeof(IProjectTreePropertiesProvider))]
    [AppliesTo(MyUnconfiguredProject.UniqueCapability)]
    internal class CodeDiscoverProjectTreePropertiesProvider : IProjectTreePropertiesProvider
    {
        // we want the "old" IVsHierarchy interface 
        [ImportMany(ExportContractNames.VsTypes.IVsHierarchy)]
        // ReSharper disable once InconsistentNaming
        private OrderPrecedenceImportCollection<IVsHierarchy> IVsHierarchies { get; }
        private IVsHierarchy VsHierarchy => IVsHierarchies.First().Value;

        [ImportingConstructor]
        public CodeDiscoverProjectTreePropertiesProvider(UnconfiguredProject unconfiguredProject)
        {
            IVsHierarchies = new OrderPrecedenceImportCollection<IVsHierarchy>(projectCapabilityCheckProvider: unconfiguredProject);
        }

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
            }
        }
    }
}