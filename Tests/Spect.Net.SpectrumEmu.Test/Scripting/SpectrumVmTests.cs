using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Scripting;

namespace Spect.Net.SpectrumEmu.Test.Scripting
{
    [TestClass]
    public class SpectrumVmTests
    {
        [TestMethod]
        public void Spectrum48PalVmCreationWorks()
        {
            // --- Act
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();

            // --- Assert
            sm.ModelKey.ShouldBe(SpectrumModels.ZX_SPECTRUM_48);
            sm.EditionKey.ShouldBe(SpectrumModels.PAL);
            sm.Cpu.ShouldNotBeNull();
            sm.Roms.ShouldNotBeNull();
            sm.RomCount.ShouldBe(1);
            sm.PagingInfo.ShouldNotBeNull();
            sm.RamBanks.ShouldNotBeNull();
            sm.RamBankCount.ShouldBe(0);
            sm.Keyboard.ShouldNotBeNull();
            sm.ScreenConfiguration.ShouldNotBeNull();
            sm.ScreenConfiguration.ScreenRenderingFrameTactCount.ShouldBe(69888);
            sm.ScreenBitmap.ShouldNotBeNull();
            sm.ScreenRenderingStatus.ShouldNotBeNull();
            sm.BeeperConfiguration.ShouldNotBeNull();
            sm.BeeperSamples.ShouldNotBeNull();
            sm.SoundConfiguration.ShouldBeNull();
            sm.SoundSamples.ShouldNotBeNull();
            sm.Breakpoints.ShouldNotBeNull();
            sm.MachineState.ShouldBe(VmState.None);
            sm.ExecutionCompletionReason.ShouldBe(ExecutionCompletionReason.None);
            sm.IsFirstStart.ShouldBeFalse();
            sm.IsFirstPause.ShouldBeFalse();
        }

        [TestMethod]
        public void Spectrum48PalTurboVmCreationWorks()
        {
            // --- Act
            var sm = SpectrumVmFactory.CreateSpectrum48PalTurbo();

            // --- Assert
            sm.ModelKey.ShouldBe(SpectrumModels.ZX_SPECTRUM_48);
            sm.EditionKey.ShouldBe(SpectrumModels.PAL_2_X);
            sm.Cpu.ShouldNotBeNull();
            sm.Roms.ShouldNotBeNull();
            sm.RomCount.ShouldBe(1);
            sm.PagingInfo.ShouldNotBeNull();
            sm.RamBanks.ShouldNotBeNull();
            sm.RamBankCount.ShouldBe(0);
            sm.Keyboard.ShouldNotBeNull();
            sm.ScreenConfiguration.ShouldNotBeNull();
            sm.ScreenConfiguration.ScreenRenderingFrameTactCount.ShouldBe(69888);
            sm.ScreenBitmap.ShouldNotBeNull();
            sm.ScreenRenderingStatus.ShouldNotBeNull();
            sm.BeeperConfiguration.ShouldNotBeNull();
            sm.BeeperSamples.ShouldNotBeNull();
            sm.SoundConfiguration.ShouldBeNull();
            sm.SoundSamples.ShouldNotBeNull();
            sm.Breakpoints.ShouldNotBeNull();
            sm.MachineState.ShouldBe(VmState.None);
            sm.ExecutionCompletionReason.ShouldBe(ExecutionCompletionReason.None);
            sm.IsFirstStart.ShouldBeFalse();
            sm.IsFirstPause.ShouldBeFalse();
        }

        [TestMethod]
        public void Spectrum48NtscVmCreationWorks()
        {
            // --- Act
            var sm = SpectrumVmFactory.CreateSpectrum48Ntsc();

            // --- Assert
            sm.ModelKey.ShouldBe(SpectrumModels.ZX_SPECTRUM_48);
            sm.EditionKey.ShouldBe(SpectrumModels.NTSC);
            sm.Cpu.ShouldNotBeNull();
            sm.Roms.ShouldNotBeNull();
            sm.RomCount.ShouldBe(1);
            sm.PagingInfo.ShouldNotBeNull();
            sm.RamBanks.ShouldNotBeNull();
            sm.RamBankCount.ShouldBe(0);
            sm.Keyboard.ShouldNotBeNull();
            sm.ScreenConfiguration.ShouldNotBeNull();
            sm.ScreenConfiguration.ScreenRenderingFrameTactCount.ShouldBe(59136);
            sm.ScreenBitmap.ShouldNotBeNull();
            sm.ScreenRenderingStatus.ShouldNotBeNull();
            sm.BeeperConfiguration.ShouldNotBeNull();
            sm.BeeperSamples.ShouldNotBeNull();
            sm.SoundConfiguration.ShouldBeNull();
            sm.SoundSamples.ShouldNotBeNull();
            sm.Breakpoints.ShouldNotBeNull();
            sm.MachineState.ShouldBe(VmState.None);
            sm.ExecutionCompletionReason.ShouldBe(ExecutionCompletionReason.None);
            sm.IsFirstStart.ShouldBeFalse();
            sm.IsFirstPause.ShouldBeFalse();
        }

        [TestMethod]
        public void Spectrum48NtscTurboVmCreationWorks()
        {
            // --- Act
            var sm = SpectrumVmFactory.CreateSpectrum48NtscTurbo();

            // --- Assert
            sm.ModelKey.ShouldBe(SpectrumModels.ZX_SPECTRUM_48);
            sm.EditionKey.ShouldBe(SpectrumModels.NTSC_2_X);
            sm.Cpu.ShouldNotBeNull();
            sm.Roms.ShouldNotBeNull();
            sm.RomCount.ShouldBe(1);
            sm.PagingInfo.ShouldNotBeNull();
            sm.RamBanks.ShouldNotBeNull();
            sm.RamBankCount.ShouldBe(0);
            sm.Keyboard.ShouldNotBeNull();
            sm.ScreenConfiguration.ShouldNotBeNull();
            sm.ScreenConfiguration.ScreenRenderingFrameTactCount.ShouldBe(59136);
            sm.ScreenBitmap.ShouldNotBeNull();
            sm.ScreenRenderingStatus.ShouldNotBeNull();
            sm.BeeperConfiguration.ShouldNotBeNull();
            sm.BeeperSamples.ShouldNotBeNull();
            sm.SoundConfiguration.ShouldBeNull();
            sm.SoundSamples.ShouldNotBeNull();
            sm.Breakpoints.ShouldNotBeNull();
            sm.MachineState.ShouldBe(VmState.None);
            sm.ExecutionCompletionReason.ShouldBe(ExecutionCompletionReason.None);
            sm.IsFirstStart.ShouldBeFalse();
            sm.IsFirstPause.ShouldBeFalse();
        }

        [TestMethod]
        public void Spectrum128PalVmCreationWorks()
        {
            // --- Act
            var sm = SpectrumVmFactory.CreateSpectrum128();

            // --- Assert
            sm.ModelKey.ShouldBe(SpectrumModels.ZX_SPECTRUM_128);
            sm.EditionKey.ShouldBe(SpectrumModels.PAL);
            sm.Cpu.ShouldNotBeNull();
            sm.Roms.ShouldNotBeNull();
            sm.RomCount.ShouldBe(2);
            sm.PagingInfo.ShouldNotBeNull();
            sm.RamBanks.ShouldNotBeNull();
            sm.RamBankCount.ShouldBe(8);
            sm.Keyboard.ShouldNotBeNull();
            sm.ScreenConfiguration.ShouldNotBeNull();
            sm.ScreenConfiguration.ScreenRenderingFrameTactCount.ShouldBe(70908);
            sm.ScreenBitmap.ShouldNotBeNull();
            sm.ScreenRenderingStatus.ShouldNotBeNull();
            sm.BeeperConfiguration.ShouldNotBeNull();
            sm.BeeperSamples.ShouldNotBeNull();
            sm.SoundConfiguration.ShouldNotBeNull();
            sm.SoundSamples.ShouldNotBeNull();
            sm.Breakpoints.ShouldNotBeNull();
            sm.MachineState.ShouldBe(VmState.None);
            sm.ExecutionCompletionReason.ShouldBe(ExecutionCompletionReason.None);
            sm.IsFirstStart.ShouldBeFalse();
            sm.IsFirstPause.ShouldBeFalse();
        }

        [TestMethod]
        public void SpectrumP3EPalVmCreationWorks()
        {
            // --- Act
            var sm = SpectrumVmFactory.CreateSpectrumP3E();

            // --- Assert
            sm.ModelKey.ShouldBe(SpectrumModels.ZX_SPECTRUM_P3_E);
            sm.EditionKey.ShouldBe(SpectrumModels.PAL);
            sm.Cpu.ShouldNotBeNull();
            sm.Roms.ShouldNotBeNull();
            sm.RomCount.ShouldBe(4);
            sm.PagingInfo.ShouldNotBeNull();
            sm.RamBanks.ShouldNotBeNull();
            sm.RamBankCount.ShouldBe(8);
            sm.Keyboard.ShouldNotBeNull();
            sm.ScreenConfiguration.ShouldNotBeNull();
            sm.ScreenConfiguration.ScreenRenderingFrameTactCount.ShouldBe(70908);
            sm.ScreenBitmap.ShouldNotBeNull();
            sm.ScreenRenderingStatus.ShouldNotBeNull();
            sm.BeeperConfiguration.ShouldNotBeNull();
            sm.BeeperSamples.ShouldNotBeNull();
            sm.SoundConfiguration.ShouldNotBeNull();
            sm.SoundSamples.ShouldNotBeNull();
            sm.Breakpoints.ShouldNotBeNull();
            sm.MachineState.ShouldBe(VmState.None);
            sm.ExecutionCompletionReason.ShouldBe(ExecutionCompletionReason.None);
            sm.IsFirstStart.ShouldBeFalse();
            sm.IsFirstPause.ShouldBeFalse();
        }
    }
}
