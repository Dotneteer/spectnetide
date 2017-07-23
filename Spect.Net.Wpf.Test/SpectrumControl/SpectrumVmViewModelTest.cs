using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Wpf.Providers;
using Spect.Net.Wpf.SpectrumControl;

namespace Spect.Net.Wpf.Test.SpectrumControl
{
    [TestClass]
    public class SpectrumVmViewModelTest
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
            messageReceived.State.ShouldBe(SpectrumVmState.Running);
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
            messageReceived.State.ShouldBe(SpectrumVmState.Paused);
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
            messageReceived.State.ShouldBe(SpectrumVmState.Running);
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
            messageReceived.State.ShouldBe(SpectrumVmState.Stopped);
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
            vm.SpectrumVm.Cpu.Registers.PC = 0x1000;
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
            messageReceived.State.ShouldBe(SpectrumVmState.Running);
            vm.SpectrumVm.Cpu.Registers.PC.ShouldBe((ushort)0);
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
            vm.SpectrumVm.Cpu.Registers.PC = 0x1000;
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
            messageReceived.ShouldBeNull();
            vm.SpectrumVm.Cpu.Registers.PC.ShouldBe((ushort)0);
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
            messageReceived.ShouldBeNull();
        }
    }
}
