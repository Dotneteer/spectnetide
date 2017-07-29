using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

// ReSharper disable VirtualMemberCallInConstructor

namespace Spect.Net.VsPackage.Tools.RegistersTool
{
    /// <summary>
    /// This class implements the Z80 Registers tool window.
    /// </summary>
    [Guid("6379892a-87a5-4b9b-b98c-1c094501a735")]
    public class RegistersToolWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistersToolWindow"/> class.
        /// </summary>
        public RegistersToolWindow() : base(null)
        {
            Caption = "Z80 Registers";
            Content = new RegistersToolWindowControl();
        }
    }
}
