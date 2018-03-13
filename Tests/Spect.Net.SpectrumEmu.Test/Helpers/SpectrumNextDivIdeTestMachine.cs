using System.Collections.Generic;
using Spect.Net.RomResources;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Devices.DivIde;
using Spect.Net.SpectrumEmu.Devices.Memory;
using Spect.Net.SpectrumEmu.Devices.Next;
using Spect.Net.SpectrumEmu.Devices.Ports;
using Spect.Net.SpectrumEmu.Devices.Rom;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class SpectrumNextDivIdeTestMachine: Spectrum48
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SpectrumNextDivIdeTestMachine() : 
            base(new DeviceInfoCollection
            {
                new CpuDeviceInfo(SpectrumModels.ZxSpectrumNextPal.Cpu),
                new RomDeviceInfo(new ResourceRomProvider(), 
                    new RomConfigurationData
                    {
                        NumberOfRoms = 5,
                        RomName = "ZxSpectrumNext",
                        Spectrum48RomIndex = 3
                    }, 
                    new SpectrumRomDevice()),
                new ClockDeviceInfo(new ClockProvider()),
                new MemoryDeviceInfo(new MemoryConfigurationData
                {
                    SupportsBanking = true,
                    RamBanks = 8,
                    NextMemorySize = 0
                }, new SpectrumNextMemoryDevice()),
                new PortDeviceInfo(null, new SpectrumNextPortDevice()),
                new BeeperDeviceInfo(new AudioConfigurationData
                {
                    AudioSampleRate = 35000,
                    SamplesPerFrame = 699,
                    TactsPerSample = 100
                }, null),
                new ScreenDeviceInfo(SpectrumModels.ZxSpectrum128Pal.Screen, 
                    new TestPixelRenderer(SpectrumModels.ZxSpectrumNextPal.Screen)),
                new SoundDeviceInfo(new AudioConfigurationData
                {
                    AudioSampleRate = 55420,
                    SamplesPerFrame = 1107,
                    TactsPerSample = 64
                }, null),
                new NextDeviceInfo(new NextFeatureSetDevice()),
                new DivIdeDeviceInfo(new DivIdeDevice())
            })
        {
        }

        /// <summary>
        /// Initializes the code passed in <paramref name="programCode"/>. This code
        /// is put into the memory from <paramref name="codeAddress"/> and
        /// </summary>
        /// <param name="programCode">Program code</param>
        /// <param name="codeAddress">Address of first code byte</param>
        /// <param name="startAddress">Code start address, null if same as the first byte</param>
        public void InitCode(IEnumerable<byte> programCode = null, ushort codeAddress = 0x8000, 
            ushort? startAddress = null)
        {
            if (programCode == null) return;
            if (startAddress == null) startAddress = codeAddress;

            // --- Initialize the code
            foreach (var op in programCode)
            {
                WriteSpectrumMemory(codeAddress++, op);
            }
            while (codeAddress != 0)
            {
                WriteSpectrumMemory(codeAddress++, 0);
            }

            Cpu.Reset();
            Cpu.Registers.PC = startAddress.Value;
        }
    }
}