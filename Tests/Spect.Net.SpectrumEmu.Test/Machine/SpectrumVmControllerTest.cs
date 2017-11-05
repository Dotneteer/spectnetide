using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spect.Net.SpectrumEmu.Machine;
using System.Threading;
using Shouldly;
using Spect.Net.RomResources;
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
            var mc = new SpectrumVmController
            {
                StartupConfiguration = new MachineStartupConfiguration
                {
                    ClockProvider = new ClockProvider(),
                    RomProvider = new ResourceRomProvider()
                }
            };
            var before = mc.SpectrumVm;

            // --- Act
            mc.StartVm(new ExecuteCycleOptions(EmulationMode.UntilFrameEnds));
            await mc.StarterTask;

            // --- Assert
            before.ShouldBeNull();
            mc.SpectrumVm.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task StartVmBuildsTheMachine()
        {
            // --- Arrange
            var mc = new SpectrumVmController
            {
                StartupConfiguration = new MachineStartupConfiguration
                {
                    ClockProvider = new ClockProvider(),
                    RomProvider = new ResourceRomProvider()
                }
            };
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
        }

        [TestMethod]
        public async Task StartVmSignsBeforeRunningState()
        {
            // --- Arrange
            var mc = new SpectrumVmController
            {
                StartupConfiguration = new MachineStartupConfiguration
                {
                    ClockProvider = new ClockProvider(),
                    RomProvider = new ResourceRomProvider()
                }
            };
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
        }

        [TestMethod]
        public async Task StartVmSignsReachesRunningState()
        {
            // --- Arrange
            var mc = new SpectrumVmController
            {
                StartupConfiguration = new MachineStartupConfiguration
                {
                    ClockProvider = new ClockProvider(),
                    RomProvider = new ResourceRomProvider()
                }
            };
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
            oldState.ShouldBe(VmState.BeforeRun);
            newState.ShouldBe(VmState.Running);
        }

        [TestMethod]
        public void PauseVmDoesNotPauseANonBuiltMachnie()
        {
            // --- Arrange
            var mc = new SpectrumVmController
            {
                StartupConfiguration = new MachineStartupConfiguration
                {
                    ClockProvider = new ClockProvider(),
                    RomProvider = new ResourceRomProvider()
                }
            };

            // --- Act
            mc.PauseVm();

            // --- Assert
            mc.VmState.ShouldBe(VmState.None);
        }

        [TestMethod]
        public async Task PauseVmSignsReachesPausingState()
        {
            // --- Arrange
            var mc = new SpectrumVmController
            {
                StartupConfiguration = new MachineStartupConfiguration
                {
                    ClockProvider = new ClockProvider(),
                    RomProvider = new ResourceRomProvider()
                }
            };
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
            oldState.ShouldBe(VmState.Running);
            newState.ShouldBe(VmState.Pausing);
        }

        [TestMethod]
        public async Task PauseVmSignsReachesPausedState()
        {
            // --- Arrange
            var mc = new SpectrumVmController
            {
                StartupConfiguration = new MachineStartupConfiguration
                {
                    ClockProvider = new ClockProvider(),
                    RomProvider = new ResourceRomProvider()
                }
            };
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
            oldState.ShouldBe(VmState.Pausing);
            newState.ShouldBe(VmState.Paused);
        }

        [TestMethod]
        public async Task StopVmSignsReachesStoppingState()
        {
            // --- Arrange
            var mc = new SpectrumVmController
            {
                StartupConfiguration = new MachineStartupConfiguration
                {
                    ClockProvider = new ClockProvider(),
                    RomProvider = new ResourceRomProvider()
                }
            };
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
            oldState.ShouldBe(VmState.Running);
            newState.ShouldBe(VmState.Stopping);
        }

        [TestMethod]
        public async Task StopVmSignsReachesStoppedState()
        {
            // --- Arrange
            var mc = new SpectrumVmController
            {
                StartupConfiguration = new MachineStartupConfiguration
                {
                    ClockProvider = new ClockProvider(),
                    RomProvider = new ResourceRomProvider()
                }
            };
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
            oldState.ShouldBe(VmState.Stopping);
            newState.ShouldBe(VmState.Stopped);
        }

        [TestMethod]
        public async Task StartDoesNotInitializeVmAfterPause()
        {
            // --- Arrange
            var mc = new SpectrumVmController
            {
                StartupConfiguration = new MachineStartupConfiguration
                {
                    ClockProvider = new ClockProvider(),
                    RomProvider = new ResourceRomProvider()
                }
            };
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
            oldState.ShouldBe(VmState.Paused);
            newState.ShouldBe(VmState.BeforeRun);
        }

        [TestMethod]
        public async Task StartDoesNotInitializeVmAfterStop()
        {
            // --- Arrange
            var mc = new SpectrumVmController
            {
                StartupConfiguration = new MachineStartupConfiguration
                {
                    ClockProvider = new ClockProvider(),
                    RomProvider = new ResourceRomProvider()
                }
            };
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
            newState.ShouldBe(VmState.BuildingMachine);
        }

        [TestMethod]
        public async Task StopVmStopsPausedMachine()
        {
            // --- Arrange
            var mc = new SpectrumVmController
            {
                StartupConfiguration = new MachineStartupConfiguration
                {
                    ClockProvider = new ClockProvider(),
                    RomProvider = new ResourceRomProvider()
                }
            };
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
            oldState.ShouldBe(VmState.Stopping);
            newState.ShouldBe(VmState.Stopped);
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
