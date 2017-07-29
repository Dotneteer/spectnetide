using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

// ReSharper disable VirtualMemberCallInConstructor

namespace Spect.Net.VsPackage.Tools.Disassembly
{
    /// <summary>
    /// This class implements the Z80 Disassembly tool window.
    /// </summary>
    [Guid("149E947C-6296-4BCE-A939-A5CD3AA6195F")]
    public class DisassemblyToolWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisassemblyToolWindow"/> class.
        /// </summary>
        public DisassemblyToolWindow() : base(null)
        {
            Caption = "Z80 Disassembly";
            Content = new DisassemblyToolWindowControl();
        }
    }
}