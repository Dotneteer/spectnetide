using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    /// <summary>
    /// This virtual machine can be used to test the behavior of the Spectrum ROM.
    /// </summary>
    public class SpectrumSimpleTestMachine: Z80TestMachine
    {
        public SpectrumSimpleTestMachine() : base(RunMode.UntilEnd)
        {
            InitRom("ZxSpectrum48");
        }

        public void InitRom(string romResourceName)
        {
            var osInfo = new DefaultRomProvider()
                .LoadRomBytes(romResourceName);
            osInfo.CopyTo(Memory, 0);
        }

        public void CallIntoRom(ushort addr, ushort chunkAddress = 0x8000)
        {
            InitCode(new byte[]
            {
                0xCD, (byte)(addr & 0xFF), (byte)(addr >> 8)
            }, chunkAddress, chunkAddress);
            Run();
        }

        protected override void WriteMemory(ushort addr, byte value)
        {
            if (addr < 0x4000) return;
            base.WriteMemory(addr, value);
        }

        protected override byte ReadPort(ushort addr)
        {
            return 0xFF;
        }
    }
}