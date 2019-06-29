using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace Spect.Net.VsPackage.VsxLibrary.ToolWindow
{
    /// <summary>
    /// You can use this attribute to declare a tool window toolbar
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ToolWindowToolbarAttribute : Attribute
    {
        /// <summary>
        /// The command ID of the toolbar
        /// </summary>
        public int CommandId { get; }

        /// <summary>
        /// The location of the toolbar
        /// </summary>
        public VSTWT_LOCATION Location { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Attribute" /> class.
        /// </summary>
        public ToolWindowToolbarAttribute(int commandId)
        {
            CommandId = commandId;
            Location = VSTWT_LOCATION.VSTWT_TOP;
        }
    }

}