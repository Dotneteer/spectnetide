using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.Wpf.Providers;
using Spect.Net.Wpf.SpectrumControl;

namespace Spect.Net.Wpf.Test.SpectrumControl
{
    [TestClass]
    public class SpectrumVmViewModelTests
    {
        [TestMethod]
        public void ConstructorInitializesVmProperly()
        {
            // --- Act
            var vm = new SpectrumVmViewModel();

            // --- Assert
            vm.DisplayMode.ShouldBe(SpectrumDisplayMode.Fit);
            vm.VmState.ShouldBe(SpectrumVmState.None);
            vm.StartVmCommand.ShouldNotBeNull();
            vm.PauseVmCommand.ShouldNotBeNull();
            vm.StopVmCommand.ShouldNotBeNull();
            vm.ResetVmCommand.ShouldNotBeNull();
            vm.CancellationTokenSource.ShouldBeNull();
            vm.SpectrumVm.ShouldBeNull();
        }

        [TestMethod]
        public void StartVmInitializesSpectrumVmFirstTime()
        {
            // --- Arrange
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            var before = vm.SpectrumVm;
            SpectrumVmStateChangedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.StartVmCommand.Execute(null);

            // --- Assert
            before.ShouldBeNull();
            vm.SpectrumVm.ShouldNotBeNull();
            vm.VmState.ShouldBe(SpectrumVmState.Running);
            vm.RunsInDebugMode.ShouldBeFalse();
            vm.RunnerTask.ShouldNotBeNull();
            messageReceived.NewState.ShouldBe(SpectrumVmState.Running);
        }

        [TestMethod]
        public void PauseVmShouldSuspendSpectrumVm()
        {
            // --- Arrange
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            vm.StartVmCommand.Execute(null);
            var before = vm.SpectrumVm;
            SpectrumVmStateChangedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.PauseVmCommand.Execute(null);

            // --- Assert
            before.ShouldNotBeNull();
            vm.SpectrumVm.ShouldBeSameAs(before);
            vm.VmState.ShouldBe(SpectrumVmState.Paused);
            vm.RunnerTask.ShouldBeNull();
            messageReceived.NewState.ShouldBe(SpectrumVmState.Paused);
        }

        [TestMethod]
        public void StartVmDoesNotInitializesSpectrumVmAfterPause()
        {
            // --- Arrange
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            vm.StartVmCommand.Execute(null);
            var before = vm.SpectrumVm;
            vm.PauseVmCommand.Execute(null);
            SpectrumVmStateChangedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.StartVmCommand.Execute(null);

            // --- Assert
            before.ShouldNotBeNull();
            vm.SpectrumVm.ShouldBeSameAs(before);
            vm.VmState.ShouldBe(SpectrumVmState.Running);
            vm.RunnerTask.ShouldNotBeNull();
            messageReceived.NewState.ShouldBe(SpectrumVmState.Running);
        }

        [TestMethod]
        public void PauseVmIgnoresNotStartedSpectrumVm()
        {
            // --- Arrange
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            SpectrumVmStateChangedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.PauseVmCommand.Execute(null);

            // --- Assert
            vm.SpectrumVm.ShouldBeNull();
            vm.VmState.ShouldBe(SpectrumVmState.None);
            vm.RunnerTask.ShouldBeNull();
            messageReceived.ShouldBeNull();
        }

        [TestMethod]
        public void StopVmWorksAsExpected()
        {
            // --- Arrange
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            SpectrumVmStateChangedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this,
                msg => { messageReceived = msg; });
            vm.StartVmCommand.Execute(null);

            // --- Act
            vm.StopVmCommand.Execute(null);

            // --- Assert
            vm.SpectrumVm.ShouldBeNull();
            vm.VmState.ShouldBe(SpectrumVmState.Stopped);
            vm.RunnerTask.ShouldBeNull();
            messageReceived.NewState.ShouldBe(SpectrumVmState.Stopped);
        }

        [TestMethod]
        public void PauseVmIgnoresStoppedSpectrumVm()
        {
            // --- Arrange
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            vm.StartVmCommand.Execute(null);
            vm.StopVmCommand.Execute(null);
            SpectrumVmStateChangedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.PauseVmCommand.Execute(null);

            // --- Assert
            vm.SpectrumVm.ShouldBeNull();
            vm.VmState.ShouldBe(SpectrumVmState.Stopped);
            vm.RunnerTask.ShouldBeNull();
            messageReceived.ShouldBeNull();
        }

        [TestMethod]
        public void StopVmIgnoresNotStartedVm()
        {
            // --- Arrange
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            SpectrumVmStateChangedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.StopVmCommand.Execute(null);

            // --- Assert
            vm.SpectrumVm.ShouldBeNull();
            vm.VmState.ShouldBe(SpectrumVmState.None);
            vm.RunnerTask.ShouldBeNull();
            messageReceived.ShouldBeNull();
        }

        [TestMethod]
        public void ResetVmIgnoresNotStartedVm()
        {
            // --- Arrange
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            SpectrumVmStateChangedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.ResetVmCommand.Execute(null);

            // --- Assert
            vm.SpectrumVm.ShouldBeNull();
            vm.VmState.ShouldBe(SpectrumVmState.None);
            vm.RunnerTask.ShouldBeNull();
            messageReceived.ShouldBeNull();
        }

        [TestMethod]
        public void ResetVmStartsAndResetsPausedVm()
        {
            // --- Arrange
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            vm.StartVmCommand.Execute(null);
            vm.PauseVmCommand.Execute(null);
            SpectrumVmStateChangedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this,
                msg => { messageReceived = msg; });
            var before = vm.SpectrumVm;

            // --- Act
            vm.ResetVmCommand.Execute(null);

            // --- Assert
            vm.SpectrumVm.ShouldNotBeNull();
            vm.SpectrumVm.ShouldBeSameAs(before);
            vm.VmState.ShouldBe(SpectrumVmState.Running);
            vm.RunnerTask.ShouldNotBeNull();
            messageReceived.NewState.ShouldBe(SpectrumVmState.Running);
        }

        [TestMethod]
        public void ResetVmWorksWithRunningVm()
        {
            // --- Arrange
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            vm.StartVmCommand.Execute(null);
            SpectrumVmStateChangedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this,
                msg => { messageReceived = msg; });
            var before = vm.SpectrumVm;

            // --- Act
            vm.ResetVmCommand.Execute(null);

            // --- Assert
            vm.SpectrumVm.ShouldNotBeNull();
            vm.SpectrumVm.ShouldBeSameAs(before);
            vm.VmState.ShouldBe(SpectrumVmState.Running);
            vm.RunnerTask.ShouldNotBeNull();
            messageReceived.ShouldBeNull();
        }

        [TestMethod]
        public void ResetVmIgnoresStoppedVm()
        {
            // --- Arrange
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            vm.StartVmCommand.Execute(null);
            vm.StopVmCommand.Execute(null);
            SpectrumVmStateChangedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.ResetVmCommand.Execute(null);

            // --- Assert
            vm.SpectrumVm.ShouldBeNull();
            vm.VmState.ShouldBe(SpectrumVmState.Stopped);
            vm.RunnerTask.ShouldBeNull();
            messageReceived.ShouldBeNull();
        }

        [TestMethod]
        public void SetZoomNotifiesAboutDisplayModeChange()
        {
            // --- Act
            var vm = new SpectrumVmViewModel {DisplayMode = SpectrumDisplayMode.Fit};
            SpectrumDisplayModeChangedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumDisplayModeChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.DisplayMode = SpectrumDisplayMode.Zoom2;

            // --- Assert
            messageReceived.DisplayMode.ShouldBe(SpectrumDisplayMode.Zoom2);
        }

        [TestMethod]
        public void StartDebugVmInitializesSpectrumVmFirstTime()
        {
            // --- Arrange
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            var before = vm.SpectrumVm;
            SpectrumVmStateChangedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.StartDebugVmCommand.Execute(null);

            // --- Assert
            before.ShouldBeNull();
            vm.SpectrumVm.ShouldNotBeNull();
            vm.VmState.ShouldBe(SpectrumVmState.Running);
            vm.RunsInDebugMode.ShouldBeTrue();
            vm.RunnerTask.ShouldNotBeNull();
            messageReceived.NewState.ShouldBe(SpectrumVmState.Running);
        }

        [TestMethod]
        public void StartDebugVmDoesNotInitializesSpectrumVmAfterPause()
        {
            // --- Arrange
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            vm.StartVmCommand.Execute(null);
            var before = vm.SpectrumVm;
            vm.PauseVmCommand.Execute(null);
            SpectrumVmStateChangedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.StartDebugVmCommand.Execute(null);

            // --- Assert
            before.ShouldNotBeNull();
            vm.SpectrumVm.ShouldBeSameAs(before);
            vm.VmState.ShouldBe(SpectrumVmState.Running);
            vm.RunnerTask.ShouldNotBeNull();
            messageReceived.NewState.ShouldBe(SpectrumVmState.Running);
        }

        [TestMethod]
        public void PauseVmDoesNotSendDebugMessageWhenNotDebugged()
        {
            // --- Arrange
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider()
            };
            vm.StartVmCommand.Execute(null);
            var before = vm.SpectrumVm;
            SpectrumDebugPausedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumDebugPausedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.PauseVmCommand.Execute(null);

            // --- Assert
            before.ShouldNotBeNull();
            vm.SpectrumVm.ShouldBeSameAs(before);
            vm.VmState.ShouldBe(SpectrumVmState.Paused);
            vm.RunnerTask.ShouldBeNull();
            messageReceived.ShouldBeNull();
        }

        [TestMethod]
        public void PauseVmSendsDebugMessageAfterStartDebug()
        {
            // --- Arrange
            const ushort BREAK_ADDR = 0x0008;
            var debugInfo = new SpectrumDebugInfoProvider();
            debugInfo.Breakpoints.Add(BREAK_ADDR);
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider(),
                DebugInfoProvider = debugInfo
            };
            vm.StartDebugVmCommand.Execute(null);
            var before = vm.SpectrumVm;
            SpectrumDebugPausedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumDebugPausedMessage>(this,
                msg => { messageReceived = msg; });

            // --- Act
            vm.PauseVmCommand.Execute(null);

            // --- Assert
            before.ShouldNotBeNull();
            vm.SpectrumVm.ShouldBeSameAs(before);
            vm.VmState.ShouldBe(SpectrumVmState.Paused);
            vm.RunsInDebugMode.ShouldBeTrue();
            vm.RunnerTask.ShouldBeNull();
            messageReceived.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task StepIntoSendsDebugMessageAfterCompleted()
        {
            // --- Arrange
            const ushort BREAK_ADDR_1 = 0x0002;
            const ushort BREAK_ADDR_2 = 0x0005;
            var debugInfo = new SpectrumDebugInfoProvider();
            debugInfo.Breakpoints.Add(BREAK_ADDR_1);
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider(),
                DebugInfoProvider = debugInfo
            };
            SpectrumDebugPausedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumDebugPausedMessage>(this,
                msg => { messageReceived = msg; });
            vm.StartDebugVmCommand.Execute(null);
            await vm.RunnerTask;

            // --- Act
            vm.StepIntoCommand.Execute(null);
            await vm.RunnerTask;
            await Task.Delay(10);

            // --- Assert
            vm.VmState.ShouldBe(SpectrumVmState.Paused);
            vm.RunsInDebugMode.ShouldBeTrue();
            vm.RunnerTask.ShouldBeNull();
            messageReceived.ShouldNotBeNull();
            vm.SpectrumVm.Cpu.Registers.PC.ShouldBe(BREAK_ADDR_2);
        }

        [TestMethod]
        public async Task StepOverSendsDebugMessageAfterCompleted()
        {
            // --- Arrange
            const ushort BREAK_ADDR_1 = 0x0002;
            const ushort BREAK_ADDR_2 = 0x0005;
            var debugInfo = new SpectrumDebugInfoProvider();
            debugInfo.Breakpoints.Add(BREAK_ADDR_1);
            var vm = new SpectrumVmViewModel
            {
                ClockProvider = new ClockProvider(),
                RomProvider = new ResourceRomProvider(),
                DebugInfoProvider = debugInfo
            };
            SpectrumDebugPausedMessage messageReceived = null;
            Messenger.Default.Register<SpectrumDebugPausedMessage>(this,
                msg => { messageReceived = msg; });
            vm.StartDebugVmCommand.Execute(null);
            await vm.RunnerTask;

            // --- Act
            vm.StepOverCommand.Execute(null);
            await vm.RunnerTask;

            // --- Assert
            vm.VmState.ShouldBe(SpectrumVmState.Paused);
            vm.RunsInDebugMode.ShouldBeTrue();
            vm.RunnerTask.ShouldBeNull();
            messageReceived.ShouldNotBeNull();
            vm.SpectrumVm.Cpu.Registers.PC.ShouldBe(BREAK_ADDR_2);
        }
    }
}
