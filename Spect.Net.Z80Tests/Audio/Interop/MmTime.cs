using System.Runtime.InteropServices;

namespace Spect.Net.Z80Tests.Audio.Interop
{
    // http://msdn.microsoft.com/en-us/library/dd757347(v=VS.85).aspx
    [StructLayout(LayoutKind.Explicit)]
    internal struct MmTime
    {
        public const int TIME_MS = 0x0001;
        public const int TIME_SAMPLES = 0x0002;
        public const int TIME_BYTES = 0x0004;

        [FieldOffset(0)]
        public uint wType;
        [FieldOffset(4)]
        public uint ms;
        [FieldOffset(4)]
        public uint sample;
        [FieldOffset(4)]
        public uint cb;
        [FieldOffset(4)]
        public uint ticks;
        [FieldOffset(4)]
        public byte smpteHour;
        [FieldOffset(5)]
        public byte smpteMin;
        [FieldOffset(6)]
        public byte smpteSec;
        [FieldOffset(7)]
        public byte smpteFrame;
        [FieldOffset(8)]
        public byte smpteFps;
        [FieldOffset(9)]
        public byte smpteDummy;
        [FieldOffset(10)]
        public byte smptePad0;
        [FieldOffset(11)]
        public byte smptePad1;
        [FieldOffset(4)]
        public uint midiSongPtrPos;
    }
}