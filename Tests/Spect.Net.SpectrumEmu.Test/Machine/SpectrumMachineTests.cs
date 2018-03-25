using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.RomResources;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.SpectrumEmu.Devices.Memory;
using Spect.Net.SpectrumEmu.Devices.Ports;
using Spect.Net.SpectrumEmu.Devices.Sound;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Test.Machine
{
    [TestClass]
    public class SpectrumMachineTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            SpectrumMachine.Reset();
            SpectrumMachine.RegisterProvider<IRomProvider>(() => new ResourceRomProvider());
        }

        [TestMethod]
        public void CreatingSpectrum48Works()
        {
            // --- Act
            var sm = SpectrumMachine.CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL);

            // --- Assert
            sm.ShouldNotBeNull();
            sm.SpectrumVm.ShouldNotBeNull();
            sm.SpectrumVm.MemoryDevice.ShouldBeOfType<Spectrum48MemoryDevice>();
            sm.SpectrumVm.PortDevice.ShouldBeOfType<Spectrum48PortDevice>();
            sm.SpectrumVm.BeeperDevice.ShouldBeOfType<BeeperDevice>();
        }

        [TestMethod]
        public void CreatingSpectrum128Works()
        {
            // --- Act
            var sm = SpectrumMachine.CreateMachine(SpectrumModels.ZX_SPECTRUM_128, SpectrumModels.PAL);

            // --- Assert
            sm.ShouldNotBeNull();
            sm.SpectrumVm.ShouldNotBeNull();
            sm.SpectrumVm.MemoryDevice.ShouldBeOfType<Spectrum128MemoryDevice>();
            sm.SpectrumVm.PortDevice.ShouldBeOfType<Spectrum128PortDevice>();
            sm.SpectrumVm.BeeperDevice.ShouldBeOfType<BeeperDevice>();
            sm.SpectrumVm.SoundDevice.ShouldBeOfType<SoundDevice>();
        }

        [TestMethod]
        public void CreatingSpectrumP3Works()
        {
            // --- Act
            var sm = SpectrumMachine.CreateMachine(SpectrumModels.ZX_SPECTRUM_P3_E, SpectrumModels.PAL);

            // --- Assert
            sm.ShouldNotBeNull();
            sm.SpectrumVm.ShouldNotBeNull();
            sm.SpectrumVm.MemoryDevice.ShouldBeOfType<SpectrumP3MemoryDevice>();
            sm.SpectrumVm.PortDevice.ShouldBeOfType<SpectrumP3PortDevice>();
            sm.SpectrumVm.BeeperDevice.ShouldBeOfType<BeeperDevice>();
            sm.SpectrumVm.SoundDevice.ShouldBeOfType<SoundDevice>();
        }

        [TestMethod]
        public async Task FirstStartWorks()
        {
            // --- Arrange
            var sm = SpectrumMachine.CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL);
            var stateBefore = sm.VmState;
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
            await sm.Start(new ExecuteCycleOptions());
            var stateAfter = sm.VmState;

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
            var sm = SpectrumMachine.CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL);
            await sm.Start(new ExecuteCycleOptions());
            var stateBefore = sm.VmState;
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
            var stateAfter = sm.VmState;

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
            var sm = SpectrumMachine.CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL);

            // --- Act
            var stateBefore = sm.VmState;
            await sm.Pause();
            var stateAfter = sm.VmState;

            // --- Assert
            stateBefore.ShouldBe(VmState.None);
            stateAfter.ShouldBe(VmState.None);
        }

        [TestMethod]
        public async Task PauseKeepsStoppedState()
        {
            // --- Arrange
            var sm = SpectrumMachine.CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL);
            await sm.Start(new ExecuteCycleOptions());
            await sm.Stop();

            // --- Act
            var stateBefore = sm.VmState;
            await sm.Pause();
            var stateAfter = sm.VmState;

            // --- Assert
            stateBefore.ShouldBe(VmState.Stopped);
            stateAfter.ShouldBe(VmState.Stopped);
        }

        [TestMethod]
        public async Task StopAfterStartWorks()
        {
            // --- Arrange
            var sm = SpectrumMachine.CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL);
            await sm.Start(new ExecuteCycleOptions());
            var stateBefore = sm.VmState;
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
            var stateAfter = sm.VmState;

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
            var sm = SpectrumMachine.CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL);
            await sm.Start(new ExecuteCycleOptions());
            await sm.Pause();
            var stateBefore = sm.VmState;
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
            var stateAfter = sm.VmState;

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
            var sm = SpectrumMachine.CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL);
            await sm.Start(new ExecuteCycleOptions());
            await sm.Stop();

            // --- Act
            var stateBefore = sm.VmState;
            await sm.Stop();
            var stateAfter = sm.VmState;

            // --- Assert
            stateBefore.ShouldBe(VmState.Stopped);
            stateAfter.ShouldBe(VmState.Stopped);
        }

        [TestMethod]
        public async Task StartWorksAfterPause()
        {
            // --- Arrange
            var sm = SpectrumMachine.CreateMachine(SpectrumModels.ZX_SPECTRUM_48, SpectrumModels.PAL);
            await sm.Start(new ExecuteCycleOptions());
            await sm.Pause();
            var stateBefore = sm.VmState;
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
            await sm.Start(new ExecuteCycleOptions());
            var stateAfter = sm.VmState;

            // --- Assert
            stateBefore.ShouldBe(VmState.Paused);
            stateAfter.ShouldBe(VmState.Running);
            sm.IsFirstStart.ShouldBeFalse();
            goesToBeforeRun.ShouldBeTrue();
            goesToRunning.ShouldBeTrue();
        }
    }
}
