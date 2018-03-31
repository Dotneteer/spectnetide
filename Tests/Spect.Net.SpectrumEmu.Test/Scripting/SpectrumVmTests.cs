using System.Threading.Tasks;
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

        [TestMethod]
        public async Task FirstStartWorks()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            var stateBefore = sm.MachineState;
            var eventCounter = 0;
            var goesToBeforeRun = false;
            var goesToRunning = false;
            sm.VmStateChanged += (s, e) =>
            {
                eventCounter++;
                if (eventCounter == 1)
                {
                    goesToBeforeRun = e.OldState == VmState.None && e.NewState == VmState.BeforeRun;
                }
                else if (eventCounter == 2)
                {
                    goesToRunning = e.OldState == VmState.BeforeRun && e.NewState == VmState.Running;
                }
            };

            // --- Act
            await sm.Start();
            var stateAfter = sm.MachineState;

            // --- Assert
            stateBefore.ShouldBe(VmState.None);
            stateAfter.ShouldBe(VmState.Running);
            sm.IsFirstStart.ShouldBeTrue();
            goesToBeforeRun.ShouldBeTrue();
            goesToRunning.ShouldBeTrue();
        }

        [TestMethod]
        public async Task FirstPauseWorks()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            await sm.Start();
            var stateBefore = sm.MachineState;
            var eventCounter = 0;
            var goesToPausing = false;
            var goesToPaused = false;
            sm.VmStateChanged += (s, e) =>
            {
                eventCounter++;
                if (eventCounter == 1)
                {
                    goesToPausing = e.OldState == VmState.Running && e.NewState == VmState.Pausing;
                }
                else if (eventCounter == 2)
                {
                    goesToPaused = e.OldState == VmState.Pausing && e.NewState == VmState.Paused;
                }
            };

            // --- Act
            await sm.Pause();
            var stateAfter = sm.MachineState;

            // --- Assert
            stateBefore.ShouldBe(VmState.Running);
            stateAfter.ShouldBe(VmState.Paused);
            sm.IsFirstPause.ShouldBeTrue();
            goesToPausing.ShouldBeTrue();
            goesToPaused.ShouldBeTrue();
        }

        [TestMethod]
        public async Task PauseKeepsNonStartedState()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();

            // --- Act
            var stateBefore = sm.MachineState;
            await sm.Pause();
            var stateAfter = sm.MachineState;

            // --- Assert
            stateBefore.ShouldBe(VmState.None);
            stateAfter.ShouldBe(VmState.None);
        }

        [TestMethod]
        public async Task PauseKeepsStoppedState()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            await sm.Start();
            await sm.Stop();

            // --- Act
            var stateBefore = sm.MachineState;
            await sm.Pause();
            var stateAfter = sm.MachineState;

            // --- Assert
            stateBefore.ShouldBe(VmState.Stopped);
            stateAfter.ShouldBe(VmState.Stopped);
        }

        [TestMethod]
        public async Task StopAfterStartWorks()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            await sm.Start();
            var stateBefore = sm.MachineState;
            var eventCounter = 0;
            var goesToStopping = false;
            var goesToStopped = false;
            sm.VmStateChanged += (s, e) =>
            {
                eventCounter++;
                if (eventCounter == 1)
                {
                    goesToStopping = e.OldState == VmState.Running && e.NewState == VmState.Stopping;
                }
                else if (eventCounter == 2)
                {
                    goesToStopped = e.OldState == VmState.Stopping && e.NewState == VmState.Stopped;
                }
            };

            // --- Act
            await sm.Stop();
            var stateAfter = sm.MachineState;

            // --- Assert
            stateBefore.ShouldBe(VmState.Running);
            stateAfter.ShouldBe(VmState.Stopped);
            goesToStopping.ShouldBeTrue();
            goesToStopped.ShouldBeTrue();
        }

        [TestMethod]
        public async Task StopAfterPauseWorks()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            await sm.Start();
            await sm.Pause();
            var stateBefore = sm.MachineState;
            var eventCounter = 0;
            var goesToStopping = false;
            var goesToStopped = false;
            sm.VmStateChanged += (s, e) =>
            {
                eventCounter++;
                if (eventCounter == 1)
                {
                    goesToStopping = e.OldState == VmState.Paused && e.NewState == VmState.Stopping;
                }
                else if (eventCounter == 2)
                {
                    goesToStopped = e.OldState == VmState.Stopping && e.NewState == VmState.Stopped;
                }
            };

            // --- Act
            await sm.Stop();
            var stateAfter = sm.MachineState;

            // --- Assert
            stateBefore.ShouldBe(VmState.Paused);
            stateAfter.ShouldBe(VmState.Stopped);
            goesToStopping.ShouldBeTrue();
            goesToStopped.ShouldBeTrue();
        }

        [TestMethod]
        public async Task StopKeepsStoppedState()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            await sm.Start();
            await sm.Stop();

            // --- Act
            var stateBefore = sm.MachineState;
            await sm.Stop();
            var stateAfter = sm.MachineState;

            // --- Assert
            stateBefore.ShouldBe(VmState.Stopped);
            stateAfter.ShouldBe(VmState.Stopped);
        }

        [TestMethod]
        public async Task StartWorksAfterPause()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            await sm.Start();
            await sm.Pause();
            var stateBefore = sm.MachineState;
            var eventCounter = 0;
            var goesToBeforeRun = false;
            var goesToRunning = false;
            sm.VmStateChanged += (s, e) =>
            {
                eventCounter++;
                if (eventCounter == 1)
                {
                    goesToBeforeRun = e.OldState == VmState.Paused && e.NewState == VmState.BeforeRun;
                }
                else if (eventCounter == 2)
                {
                    goesToRunning = e.OldState == VmState.BeforeRun && e.NewState == VmState.Running;
                }
            };

            // --- Act
            await sm.Start();
            var stateAfter = sm.MachineState;

            // --- Assert
            stateBefore.ShouldBe(VmState.Paused);
            stateAfter.ShouldBe(VmState.Running);
            sm.IsFirstStart.ShouldBeFalse();
            goesToBeforeRun.ShouldBeTrue();
            goesToRunning.ShouldBeTrue();
        }

        [TestMethod]
        public async Task StartDebugWorks()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            var stateBefore = sm.MachineState;

            // --- Act
            sm.Breakpoints.AddBreakpoint(0x0001);
            await sm.StartDebug();
            await sm.CompletionTask;
            var stateAfter = sm.MachineState;

            // --- Assert
            stateBefore.ShouldBe(VmState.None);
            stateAfter.ShouldBe(VmState.Paused);
            sm.RunsInDebugMode.ShouldBeTrue();
            sm.Cpu.PC.ShouldBe((ushort)0x0001);
            sm.ExecutionCompletionReason.ShouldBe(ExecutionCompletionReason.BreakpointReached);
        }

        [TestMethod]
        public async Task MachineStopsAfterTimeout()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();
            var stateBefore = sm.MachineState;

            // --- Act
            sm.TimeoutInMs = 10;
            await sm.Start();
            await sm.CompletionTask;
            var stateAfter = sm.MachineState;

            // --- Assert
            stateBefore.ShouldBe(VmState.None);
            sm.ExecutionCompletionReason.ShouldBe(ExecutionCompletionReason.Timeout);
            stateAfter.ShouldBe(VmState.Paused);
        }

        [TestMethod]
        public async Task MachineRaisesFrameCompletedEvents()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();

            // --- Act
            sm.TimeoutInMs = 200;
            var counter = 0;
            sm.VmFrameCompleted += (s, e) => { counter++; };
            await sm.Start();
            await sm.CompletionTask;

            // --- Assert
            counter.ShouldBeGreaterThanOrEqualTo(9);
            counter.ShouldBeLessThanOrEqualTo(11);
        }
    }
}
