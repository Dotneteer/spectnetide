namespace Spect.Net.Z80TestHelpers
{
    /// <summary>
    /// This virtual machine can be used to test the behavior of the Spectrum ROM.
    /// </summary>
    public class SpectrumTestMachine: Z80TestMachine
    {
        public SpectrumTestMachine() : base(RunMode.UntilEnd)
        {
            InitRom("ZXSpectrum48.bin");
        }

        public void InitRom(string romResourceName)
        {
            var romBytes = FileHelper.ExtractResourceFile(romResourceName);
            romBytes.CopyTo(Memory, 0);
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

        #region Overrides of Z80TestMachine

        protected override byte ReadPort(ushort addr)
        {
            return 0xFF;
        }

        #endregion
    }
}