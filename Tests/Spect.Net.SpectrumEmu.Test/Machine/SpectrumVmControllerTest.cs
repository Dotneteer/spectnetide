using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Machine;
using System.Threading;
using Shouldly;
using Spect.Net.RomResources;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Devices.Rom;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.SpectrumEmu.Test.Machine
{
    [TestClass]
    public class SpectrumVmControllerTest
    {
        [TestMethod]
        public void ConstructionWorksAsExpected()
        {
            // --- Act 
            var mc = new SpectrumVmController();

            // --- Assert
            mc.VmState.ShouldBe(VmState.None);
            mc.SpectrumVm.ShouldBeNull();
            mc.IsOnMainThread().ShouldBeTrue();
        }

        [TestMethod]
        public async Task StartVmInitializesSpectrumVmFirstTime()
        {
            // --- Arrange
            var mc = GetMachineController();
            var before = mc.SpectrumVm;

            // --- Act
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;

            // --- Assert
            before.ShouldBeNull();
            mc.SpectrumVm.ShouldNotBeNull();
            mc.IsFirstStart.ShouldBeTrue();
        }

        [TestMethod]
        public async Task StartVmBuildsTheMachine()
        {
            // --- Arrange
            var mc = GetMachineController();
            VmState oldState = 0;
            VmState newState = 0;
            var msgCount = 0;
            mc.VmStateChanged += (sender, args) =>
            {
                if (++msgCount == 1)
                {
                    oldState = args.OldState;
                    newState = args.NewState;
                }
            };

            // --- Act
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;

            // --- Assert
            oldState.ShouldBe(VmState.None);
            newState.ShouldBe(VmState.BuildingMachine);
            mc.IsFirstStart.ShouldBeTrue();
        }

        [TestMethod]
        public async Task StartVmSignsBeforeRunningState()
        {
            // --- Arrange
            var mc = GetMachineController();
            VmState oldState = 0;
            VmState newState = 0;
            var msgCount = 0;
            mc.VmStateChanged += (sender, args) =>
            {
                if (++msgCount == 2)
                {
                    oldState = args.OldState;
                    newState = args.NewState;
                }
            };

            // --- Act
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;

            // --- Assert
            oldState.ShouldBe(VmState.BuildingMachine);
            newState.ShouldBe(VmState.BeforeRun);
            mc.IsFirstStart.ShouldBeTrue();
        }

        [TestMethod]
        public async Task StartVmSignsReachesRunningState()
        {
            // --- Arrange
            var mc = GetMachineController();
            var before = mc.SpectrumVm;
            VmState oldState = 0;
            VmState newState = 0;
            var msgCount = 0;
            mc.VmStateChanged += (sender, args) =>
            {
                if (++msgCount == 3)
                {
                    oldState = args.OldState;
                    newState = args.NewState;
                }
            };

            // --- Act
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;

            // --- Assert
            before.ShouldBeNull();
            mc.SpectrumVm.ShouldNotBeNull();
            mc.IsFirstStart.ShouldBeTrue();
            oldState.ShouldBe(VmState.BeforeRun);
            newState.ShouldBe(VmState.Running);
        }

        [TestMethod]
        public void PauseVmDoesNotPauseANonBuiltMachnie()
        {
            // --- Arrange
            var mc = GetMachineController();

            // --- Act
            mc.PauseVm();

            // --- Assert
            mc.VmState.ShouldBe(VmState.None);
        }

        [TestMethod]
        public async Task PauseVmSignsReachesPausingState()
        {
            // --- Arrange
            var mc = GetMachineController();
            var before = mc.SpectrumVm;
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;
            VmState oldState = 0;
            VmState newState = 0;
            var msgCount = 0;
            mc.VmStateChanged += (sender, args) =>
            {
                if (++msgCount == 1)
                {
                    oldState = args.OldState;
                    newState = args.NewState;
                }
            };

            // --- Act
            mc.PauseVm();
            await mc.CompletionTask;

            // --- Assert
            before.ShouldBeNull();
            mc.SpectrumVm.ShouldNotBeNull();
            mc.IsFirstStart.ShouldBeTrue();
            mc.IsFirstPause.ShouldBeTrue();
            oldState.ShouldBe(VmState.Running);
            newState.ShouldBe(VmState.Pausing);
        }

        [TestMethod]
        public async Task PauseVmSignsReachesPausedState()
        {
            // --- Arrange
            var mc = GetMachineController();
            var before = mc.SpectrumVm;
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;

            VmState oldState = 0;
            VmState newState = 0;
            var msgCount = 0;
            mc.VmStateChanged += (sender, args) =>
            {
                if (++msgCount == 2)
                {
                    oldState = args.OldState;
                    newState = args.NewState;
                }
            };

            // --- Act
            mc.PauseVm();
            await mc.CompletionTask;

            // --- Assert
            before.ShouldBeNull();
            mc.SpectrumVm.ShouldNotBeNull();
            mc.IsFirstStart.ShouldBeTrue();
            mc.IsFirstPause.ShouldBeTrue();
            oldState.ShouldBe(VmState.Pausing);
            newState.ShouldBe(VmState.Paused);
        }

        [TestMethod]
        public async Task StopVmSignsReachesStoppingState()
        {
            // --- Arrange
            var mc = GetMachineController();
            var before = mc.SpectrumVm;
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;
            VmState oldState = 0;
            VmState newState = 0;
            var msgCount = 0;
            mc.VmStateChanged += (sender, args) =>
            {
                if (++msgCount == 1)
                {
                    oldState = args.OldState;
                    newState = args.NewState;
                }
            };

            // --- Act
            mc.StopVm();
            await mc.CompletionTask;

            // --- Assert
            before.ShouldBeNull();
            mc.SpectrumVm.ShouldNotBeNull();
            mc.IsFirstStart.ShouldBeTrue();
            oldState.ShouldBe(VmState.Running);
            newState.ShouldBe(VmState.Stopping);
        }

        [TestMethod]
        public async Task StopVmSignsReachesStoppedState()
        {
            // --- Arrange
            var mc = GetMachineController();
            var before = mc.SpectrumVm;
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;

            VmState oldState = 0;
            VmState newState = 0;
            var msgCount = 0;
            mc.VmStateChanged += (sender, args) =>
            {
                if (++msgCount == 2)
                {
                    oldState = args.OldState;
                    newState = args.NewState;
                }
            };

            // --- Act
            mc.StopVm();
            await mc.CompletionTask;

            // --- Assert
            before.ShouldBeNull();
            mc.SpectrumVm.ShouldNotBeNull();
            mc.IsFirstStart.ShouldBeTrue();
            oldState.ShouldBe(VmState.Stopping);
            newState.ShouldBe(VmState.Stopped);
        }

        [TestMethod]
        public async Task StartDoesNotInitializeVmAfterPause()
        {
            // --- Arrange
            var mc = GetMachineController();
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;
            var before = mc.SpectrumVm;
            mc.PauseVm();
            await mc.CompletionTask;

            VmState oldState = 0;
            VmState newState = 0;
            var msgCount = 0;
            mc.VmStateChanged += (sender, args) =>
            {
                if (++msgCount == 1)
                {
                    oldState = args.OldState;
                    newState = args.NewState;
                }
            };

            // --- Act
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;

            // --- Assert
            before.ShouldNotBeNull();
            mc.SpectrumVm.ShouldBeSameAs(before);
            mc.IsFirstStart.ShouldBeFalse();
            oldState.ShouldBe(VmState.Paused);
            newState.ShouldBe(VmState.BeforeRun);
        }

        [TestMethod]
        public async Task StartDoesNotInitializeVmAfterStop()
        {
            // --- Arrange
            var mc = GetMachineController();
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;
            var before = mc.SpectrumVm;
            mc.StopVm();
            await mc.CompletionTask;

            VmState oldState = 0;
            VmState newState = 0;
            var msgCount = 0;
            mc.VmStateChanged += (sender, args) =>
            {
                if (++msgCount == 1)
                {
                    oldState = args.OldState;
                    newState = args.NewState;
                }
            };

            // --- Act
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;

            // --- Assert
            before.ShouldNotBeNull();
            mc.SpectrumVm.ShouldBeSameAs(before);
            oldState.ShouldBe(VmState.Stopped);
            newState.ShouldBe(VmState.BeforeRun);
        }

        [TestMethod]
        public async Task StorResetsTheIsFirstStartFlag()
        {
            // --- Arrange
            var mc = GetMachineController();
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;
            var before = mc.SpectrumVm;
            mc.StopVm();
            await mc.CompletionTask;

            // --- Act
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;

            // --- Assert
            before.ShouldNotBeNull();
            mc.SpectrumVm.ShouldBeSameAs(before);
            mc.IsFirstStart.ShouldBeTrue();
        }

        [TestMethod]
        public async Task StopVmStopsPausedMachine()
        {
            // --- Arrange
            var mc = GetMachineController();
            var before = mc.SpectrumVm;
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;
            mc.PauseVm();
            await mc.CompletionTask;

            VmState oldState = 0;
            VmState newState = 0;
            var msgCount = 0;
            mc.VmStateChanged += (sender, args) =>
            {
                if (++msgCount == 2)
                {
                    oldState = args.OldState;
                    newState = args.NewState;
                }
            };

            // --- Act
            mc.StopVm();
            await mc.CompletionTask;

            // --- Assert
            before.ShouldBeNull();
            mc.SpectrumVm.ShouldNotBeNull();
            mc.IsFirstStart.ShouldBeTrue();
            oldState.ShouldBe(VmState.Stopping);
            newState.ShouldBe(VmState.Stopped);
        }

        [TestMethod]
        public async Task SubsequentPausesDoNotSeemFirst()
        {
            // --- Arrange
            var mc = GetMachineController();
            var before = mc.SpectrumVm;
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;
            mc.PauseVm();
            await mc.CompletionTask;
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;

            // --- Act
            mc.PauseVm();
            await mc.CompletionTask;

            // --- Assert
            before.ShouldBeNull();
            mc.SpectrumVm.ShouldNotBeNull();
            mc.IsFirstStart.ShouldBeFalse();
            mc.IsFirstPause.ShouldBeFalse();
        }

        [TestMethod]
        public async Task StopAfterPauseDoesNotSeemFirst()
        {
            // --- Arrange
            var mc = GetMachineController();
            var before = mc.SpectrumVm;
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;
            mc.PauseVm();
            await mc.CompletionTask;
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;

            // --- Act
            mc.StopVm();
            await mc.CompletionTask;

            // --- Assert
            before.ShouldBeNull();
            mc.SpectrumVm.ShouldNotBeNull();
            mc.IsFirstStart.ShouldBeFalse();
        }

        [TestMethod]
        public async Task SubsequentStopsDoNotSeemFirst()
        {
            // --- Arrange
            var mc = new SpectrumVmController
            {
                StartupConfiguration = new MachineStartupConfiguration
                {
                    DeviceData = new DeviceInfoCollection
                    {
                        new RomDeviceInfo(new ResourceRomProvider(),
                            new RomConfigurationData
                            {
                                NumberOfRoms = 1,
                                RomName = "ZxSpectrum48",
                                Spectrum48RomIndex = 0
                            }, new SpectrumRomDevice()),
                        new BeeperDeviceInfo(new BeeperConfigurationData
                        {
                            AudioSampleRate = 35000,
                            SamplesPerFrame = 699,
                            TactsPerSample = 100
                        }, null),
                        new ClockDeviceInfo(new ClockProvider()),
                        new ScreenDeviceInfo(SpectrumModels.ZxSpectrum48Pal.Screen)
                    }
                }
            };

            var before = mc.SpectrumVm;
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;
            mc.StopVm();
            await mc.CompletionTask;
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;

            // --- Act
            mc.StopVm();
            await mc.CompletionTask;

            // --- Assert
            before.ShouldBeNull();
            mc.SpectrumVm.ShouldNotBeNull();
            mc.IsFirstStart.ShouldBeTrue();
        }

        private SpectrumVmController GetMachineController()
        {
            return new SpectrumVmController
            {
                StartupConfiguration = new MachineStartupConfiguration
                {
                    DeviceData = new DeviceInfoCollection
                    {
                        new ClockDeviceInfo(new ClockProvider()),
                        new RomDeviceInfo(new ResourceRomProvider(), 
                            new RomConfigurationData
                            {
                                NumberOfRoms = 1,
                                RomName = "ZxSpectrum48",
                                Spectrum48RomIndex = 0
                            }, 
                            new SpectrumRomDevice()),
                        new BeeperDeviceInfo(new BeeperConfigurationData
                        {
                            AudioSampleRate = 35000,
                            SamplesPerFrame = 699,
                            TactsPerSample = 100
                        }, null),
                        new ScreenDeviceInfo(SpectrumModels.ZxSpectrum48Pal.Screen)
                    }
                }
            };
        }

        private class SpectrumVmController: SpectrumVmControllerBase
        {
            private TaskScheduler _context;

            public override void SaveMainContext()
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                _context = TaskScheduler.FromCurrentSynchronizationContext();
            }

            public override bool IsOnMainThread()
            {
                return true;
            }

            public override Task ExecuteOnMainThread(Action action)
            {
                return Task.Factory.StartNew(() => { }).ContinueWith((t, o) => { action(); }, null, 
                    _context);
            }
        }
    }
}
