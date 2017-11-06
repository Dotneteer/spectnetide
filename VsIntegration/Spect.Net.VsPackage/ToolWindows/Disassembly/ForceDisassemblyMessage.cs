using GalaSoft.MvvmLight.Messaging;

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    /// <summary>
    /// This message signs that the whole memory should be disassembled
    /// again
    /// </summary>
    public class ForceDisassemblyMessage: MessageBase
    {
    }
}