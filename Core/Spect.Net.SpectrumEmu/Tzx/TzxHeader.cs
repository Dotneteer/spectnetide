using System.Runtime.InteropServices;

namespace Spect.Net.SpectrumEmu.Tzx
{
    [StructLayout(LayoutKind.Explicit)]
    public class TzxHeader
    {
        [FieldOffset(0)] private byte[] Signature;
    }
}