using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Test.Helpers;

// ReSharper disable PossibleNullReferenceException

namespace Spect.Net.SpectrumEmu.Test.Devices.Tape
{
    [TestClass]
    public class TapeDeviceTests
    {
        [TestMethod]
        public void TapeModeIsPassiveByDefault()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Passive);
        }

        [TestMethod]
        public void SetTapeModeStaysInPassiveMode()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = 0x0038;

            // --- Act
            td.SetTapeMode();

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Passive);
        }

        [TestMethod]
        public void SetTapeModeEntersLoadMode()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.LoadBytesRoutineAddress;

            // --- Act
            td.SetTapeMode();

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Load);
        }

        [TestMethod]
        public void SetTapeModeInvokesEnteredLoadModeEvent()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.LoadBytesRoutineAddress;
            var invoked = false;
            td.EnteredLoadMode += (sender, args) => { invoked = true; };

            // --- Act
            td.SetTapeMode();

            // --- Assert
            invoked.ShouldBeTrue();
        }

        [TestMethod]
        public void SetTapeModeEntersSaveMode()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;

            // --- Act
            td.SetTapeMode();

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
        }

        [TestMethod]
        public void SetTapeModeInvokesEnteredSaveModeEvent()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            var invoked = false;
            td.EnteredSaveMode += (sender, args) => { invoked = true; };

            // --- Act
            td.SetTapeMode();

            // --- Assert
            invoked.ShouldBeTrue();
        }

        [TestMethod]
        public void SetTapeModeLeavesLoadModeWhenError()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.LoadBytesRoutineAddress;
            td.SetTapeMode();
            var before = td.CurrentMode;

            // --- Act
            vm.Cpu.Registers.PC = TapeDevice.ERROR_ROM_ADDRESS;
            td.SetTapeMode();

            // --- Assert
            before.ShouldBe(TapeOperationMode.Load);
            td.CurrentMode.ShouldBe(TapeOperationMode.Passive);
        }

        [TestMethod]
        public void SetTapeModeInvokesLeftLoadModeEventWhenError()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.LoadBytesRoutineAddress;
            td.SetTapeMode();
            var invoked = false;
            td.LeftLoadMode += (sender, args) => { invoked = true; };

            // --- Act
            vm.Cpu.Registers.PC = TapeDevice.ERROR_ROM_ADDRESS;
            td.SetTapeMode();

            // --- Assert
            invoked.ShouldBeTrue();
        }

        [TestMethod]
        public void SetTapeModeLeavesSaveModeWhenError()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var before = td.CurrentMode;

            // --- Act
            vm.Cpu.Registers.PC = TapeDevice.ERROR_ROM_ADDRESS;
            td.SetTapeMode();

            // --- Assert
            before.ShouldBe(TapeOperationMode.Save);
            td.CurrentMode.ShouldBe(TapeOperationMode.Passive);
        }

        [TestMethod]
        public void SetTapeModeInvokesLeftSaveModeWhenError()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var invoked = false;
            td.LeftSaveMode += (sender, args) => { invoked = true; };

            // --- Act
            vm.Cpu.Registers.PC = TapeDevice.ERROR_ROM_ADDRESS;
            td.SetTapeMode();

            // --- Assert
            invoked.ShouldBeTrue();
        }

        [TestMethod]
        public void SetTapeModeLeavesLoadModeWhenEof()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(new EmptyTapeContentProvider());
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.LoadBytesRoutineAddress;
            td.SetTapeMode();
            var before = td.CurrentMode;

            // --- Act
            td.SetTapeMode();

            // --- Assert
            before.ShouldBe(TapeOperationMode.Load);
            td.CurrentMode.ShouldBe(TapeOperationMode.Passive);
        }

        [TestMethod]
        public void SetTapeModeLeavesSaveModeAfterSilence()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var before = td.CurrentMode;

            // --- Act
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            debugCpu.SetTacts(2*TapeDevice.SAVE_STOP_SILENCE);
            td.SetTapeMode();

            // --- Assert
            before.ShouldBe(TapeOperationMode.Save);
            td.CurrentMode.ShouldBe(TapeOperationMode.Passive);
        }

        [TestMethod]
        public void SetTapeModeInvokesLedtSaveModeEventAfterSilence()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var invoked = false;
            td.LeftSaveMode += (sender, args) => { invoked = true; };

            // --- Act
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            debugCpu.SetTacts(2 * TapeDevice.SAVE_STOP_SILENCE);
            td.SetTapeMode();

            // --- Assert
            invoked.ShouldBeTrue();
        }

        [TestMethod]
        public void GetEarBitsReturnTrueInPassiveMode()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = 0x0038;
            td.SetTapeMode();

            // --- Act
            var bit = td.GetEarBit(0);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Passive);
            bit.ShouldBeTrue();
        }

        [TestMethod]
        public void GetEarBitsReturnTrueInSaveMode()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();

            // --- Act
            var bit = td.GetEarBit(0);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            bit.ShouldBeTrue();
        }

        [TestMethod]
        public void GetEarBitsReturnTrueInLoadModeWithNoPlayer()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.LoadBytesRoutineAddress;
            td.SetTapeMode();

            // --- Act
            var bit = td.GetEarBit(0);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Load);
            bit.ShouldBeTrue();
        }

        [TestMethod]
        public void ProcessMicBitAbortsInPassiveMode()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = 0x0038;
            td.SetTapeMode();

            // --- Act
            td.ProcessMicBit(true);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Passive);
            td.SavePhase.ShouldBe(SavePhase.None);
        }

        [TestMethod]
        public void ProcessMicBitAbortsInLoadMode()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.LoadBytesRoutineAddress;
            td.SetTapeMode();

            // --- Act
            td.ProcessMicBit(true);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Load);
            td.SavePhase.ShouldBe(SavePhase.None);
        }

        [TestMethod]
        public void ProcessMicBitWorksInSaveModeWithBitChange()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();

            // --- Act
            td.ProcessMicBit(false);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.None);
        }

        [TestMethod]
        public void ProcessMicBitCatchesFirstPilotPulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;

            // --- Act
            debugCpu.SetTacts(TapeDataBlockPlayer.PILOT_PL);
            td.ProcessMicBit(false);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Pilot);
            td.PilotPulseCount.ShouldBe(1);
        }

        [TestMethod]
        public void ProcessMicBitFailsWithShortPilotPulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;

            // --- Act
            debugCpu.SetTacts(TapeDataBlockPlayer.PILOT_PL - TapeDevice.SAVE_PULSE_TOLERANCE - 1);
            td.ProcessMicBit(false);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitFailsWithLongPilotPulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;

            // --- Act
            debugCpu.SetTacts(TapeDataBlockPlayer.PILOT_PL + TapeDevice.SAVE_PULSE_TOLERANCE + 1);
            td.ProcessMicBit(false);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitCatchesMultiplePilotPulses()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;

            // --- Act
            for (var i = 0; i < 10; i++)
            {
                tacts += TapeDataBlockPlayer.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Pilot);
            td.PilotPulseCount.ShouldBe(10);
        }

        [TestMethod]
        public void ProcessMicBitFailsWithEarlySync1Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < 10; i++)
            {
                tacts += TapeDataBlockPlayer.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }

            // --- Act
            tacts += TapeDataBlockPlayer.SYNC_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitWorksWithSync1Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < TapeDevice.MIN_PILOT_PULSE_COUNT + 10; i++)
            {
                tacts += TapeDataBlockPlayer.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }

            // --- Act
            tacts += TapeDataBlockPlayer.SYNC_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Sync1);
        }

        [TestMethod]
        public void ProcessMicBitFailsWithShortSync1Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < TapeDevice.MIN_PILOT_PULSE_COUNT + 10; i++)
            {
                tacts += TapeDataBlockPlayer.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }

            // --- Act
            tacts += TapeDataBlockPlayer.SYNC_1_PL - TapeDevice.SAVE_PULSE_TOLERANCE - 1;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitFailsWithLongSync1Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < TapeDevice.MIN_PILOT_PULSE_COUNT + 10; i++)
            {
                tacts += TapeDataBlockPlayer.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }

            // --- Act
            tacts += TapeDataBlockPlayer.SYNC_1_PL + TapeDevice.SAVE_PULSE_TOLERANCE + 1;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitWorksWithSync2Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < TapeDevice.MIN_PILOT_PULSE_COUNT + 10; i++)
            {
                tacts += TapeDataBlockPlayer.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }
            tacts += TapeDataBlockPlayer.SYNC_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;

            // --- Act
            tacts += TapeDataBlockPlayer.SYNC_2_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Sync2);
            td.PrevDataPulse.ShouldBe(MicPulseType.None);
        }

        [TestMethod]
        public void ProcessMicBitFailsWithShortSync2Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < TapeDevice.MIN_PILOT_PULSE_COUNT + 10; i++)
            {
                tacts += TapeDataBlockPlayer.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }
            tacts += TapeDataBlockPlayer.SYNC_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;

            // --- Act
            tacts += TapeDataBlockPlayer.SYNC_2_PL - TapeDevice.SAVE_PULSE_TOLERANCE - 1;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitFailsWithLongSync2Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < TapeDevice.MIN_PILOT_PULSE_COUNT + 10; i++)
            {
                tacts += TapeDataBlockPlayer.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }
            tacts += TapeDataBlockPlayer.SYNC_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;

            // --- Act
            tacts += TapeDataBlockPlayer.SYNC_2_PL + TapeDevice.SAVE_PULSE_TOLERANCE + 1;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitWorksWithBit0Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);

            // --- Act
            tacts += TapeDataBlockPlayer.BIT_0_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Data);
            td.BitOffset.ShouldBe(0);
            td.DataByte.ShouldBe((byte)0);
            td.PrevDataPulse.ShouldBe(MicPulseType.Bit0);
        }

        [TestMethod]
        public void ProcessMicBitFailsWithShortBit0Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);

            // --- Act
            tacts += TapeDataBlockPlayer.BIT_0_PL - TapeDevice.SAVE_PULSE_TOLERANCE - 1;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitFailsWithLongBit0Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);

            // --- Act
            tacts += TapeDataBlockPlayer.BIT_0_PL + TapeDevice.SAVE_PULSE_TOLERANCE + 1;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitWorksWithFullBit0Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);
            tacts += TapeDataBlockPlayer.BIT_0_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;

            // --- Act
            tacts += TapeDataBlockPlayer.BIT_0_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Data);
            td.BitOffset.ShouldBe(1);
            td.DataByte.ShouldBe((byte)0);
            td.PrevDataPulse.ShouldBe(MicPulseType.None);
        }

        [TestMethod]
        public void ProcessMicBitWorksWithBit1Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);

            // --- Act
            tacts += TapeDataBlockPlayer.BIT_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Data);
            td.BitOffset.ShouldBe(0);
            td.DataByte.ShouldBe((byte)0);
            td.PrevDataPulse.ShouldBe(MicPulseType.Bit1);
        }

        [TestMethod]
        public void ProcessMicBitFailsWithShortBit1Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);

            // --- Act
            tacts += TapeDataBlockPlayer.BIT_1_PL - TapeDevice.SAVE_PULSE_TOLERANCE - 1;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitFailsWithLongBit1Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);

            // --- Act
            tacts += TapeDataBlockPlayer.BIT_1_PL + TapeDevice.SAVE_PULSE_TOLERANCE + 1;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitWorksWithFullBit1Pulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);
            tacts += TapeDataBlockPlayer.BIT_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;

            // --- Act
            tacts += TapeDataBlockPlayer.BIT_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Data);
            td.BitOffset.ShouldBe(1);
            td.DataByte.ShouldBe((byte)1);
            td.PrevDataPulse.ShouldBe(MicPulseType.None);
        }

        [TestMethod]
        public void ProcessMicBitFailsWithBit0Bit1()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);

            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);
            tacts += TapeDataBlockPlayer.BIT_0_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;

            // --- Act
            tacts += TapeDataBlockPlayer.BIT_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitFailsWithBit1Bit0()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);
            tacts += TapeDataBlockPlayer.BIT_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;

            // --- Act
            tacts += TapeDataBlockPlayer.BIT_0_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitWorksWithDataBytes()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            var testData = new byte[] { 0x90, 0x02, 0x05, 0xAA, 0xFF, 0x63 };

            // --- Act
            EmitHeaderAndData(vm, td, testData);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Data);
            td.DataLength.ShouldBe(testData.Length);
            for (var i = 0; i < testData.Length; i++)
            {
                td.DataBuffer[i].ShouldBe(testData[i]);
            }
        }

        [TestMethod]
        public void ProcessMicBitWorksWithTermSyncPulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            var testData = new byte[] { 0x90, 0x02, 0x05, 0xAA, 0xFF, 0x63 };
            (var debugCpu, var tacts, var pulse) = EmitHeaderAndData(vm, td, testData);

            // --- Act
            tacts += TapeDataBlockPlayer.TERM_SYNC;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.None);
            for (var i = 0; i < testData.Length; i++)
            {
                td.DataBuffer[i].ShouldBe(testData[i]);
            }
        }

        [TestMethod]
        public void ProcessMicBitFailsWithShortTermSyncPulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            var testData = new byte[] { 0x90, 0x02, 0x05, 0xAA, 0xFF, 0x63 };
            (var debugCpu, var tacts, var pulse) = EmitHeaderAndData(vm, td, testData);

            // --- Act
            tacts += TapeDataBlockPlayer.TERM_SYNC - TapeDevice.SAVE_PULSE_TOLERANCE - 1;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitFailsWithLongTermSyncPulse()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            var testData = new byte[] { 0x90, 0x02, 0x05, 0xAA, 0xFF, 0x63 };
            (var debugCpu, var tacts, var pulse) = EmitHeaderAndData(vm, td, testData);

            // --- Act
            tacts += TapeDataBlockPlayer.TERM_SYNC + TapeDevice.SAVE_PULSE_TOLERANCE + 1;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.Error);
        }

        [TestMethod]
        public void ProcessMicBitWorksWithMultipleDataBlock()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null);
            td.OnAttachedToVm(vm);
            var testData = new byte[] { 0x90, 0x02, 0x05, 0xAA, 0xFF, 0x63 };

            // --- Act
            (var debugCpu, var tacts, var pulse) = EmitFullDataBlock(vm, td, testData);
            tacts += TapeDataBlockPlayer.PILOT_PL * 5;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            (debugCpu, tacts, pulse) = EmitFullDataBlock(vm, td, testData);
            tacts += TapeDataBlockPlayer.PILOT_PL * 5;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.None);
            td.DataBlockCount.ShouldBe(2);
        }

        [TestMethod]
        public void CreateTapeFileIsInvokedWhenEnteringSaveMode()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var saveProvider = new FakeSaveToTapeProvider();
            var td = new TapeDevice(saveProvider);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;

            // --- Act
            td.SetTapeMode();

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            saveProvider.CreateTapeFileInvoked.ShouldBeTrue();
        }

        [TestMethod]
        public void FinalizeTapeFileIsInvokedWhenLeavingSaveMode()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var saveProvider = new FakeSaveToTapeProvider();
            var td = new TapeDevice(saveProvider);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var before = td.CurrentMode;

            // --- Act
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            debugCpu.SetTacts(2 * TapeDevice.SAVE_STOP_SILENCE);
            td.SetTapeMode();

            // --- Assert
            before.ShouldBe(TapeOperationMode.Save);
            td.CurrentMode.ShouldBe(TapeOperationMode.Passive);
            saveProvider.FinalizeTapeFileInvoked.ShouldBeTrue();
        }

        [TestMethod]
        public void SaveTzxBlockIsCalledWhenCompletingDataBlock()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var saveProvider = new FakeSaveToTapeProvider();
            var td = new TapeDevice(saveProvider);
            td.OnAttachedToVm(vm);
            var testData = new byte[] { 0x90, 0x02, 0x05, 0xAA, 0xFF, 0x63 };

            // --- Act
            EmitFullDataBlock(vm, td, testData);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.None);
            saveProvider.SaveTzxBlockInvoked.ShouldBeTrue();
        }

        [TestMethod]
        public void SetNameIsCalledAtFirstDataBlock()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var saveProvider = new FakeSaveToTapeProvider();
            var td = new TapeDevice(saveProvider);
            td.OnAttachedToVm(vm);
            var testData = new byte[]
            {
                0x00, 0x00,
                0x42, 0x6F, 0x72, 0x64, 0x65, 0x72, 0x20, 0x20, 0x20, 0x20,
                0x4F, 0x00, 0x6F, 0x80, 0x4F, 0x00, 0xC3
            };

            // --- Act
            EmitFullDataBlock(vm, td, testData);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.None);
            saveProvider.SuggestedName.ShouldBe("Border");
        }

        [TestMethod]
        public void SetNameIsNotCalledWithInvalidHeader()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var saveProvider = new FakeSaveToTapeProvider();
            var td = new TapeDevice(saveProvider);
            td.OnAttachedToVm(vm);
            var testData = new byte[]
            {
                0x42, 0x6F, 0x72, 0x64, 0x65, 0x72, 0x20, 0x20, 0x20, 0x20
            };

            // --- Act
            EmitFullDataBlock(vm, td, testData);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.None);
            saveProvider.SuggestedName.ShouldBeNull();
        }

        [TestMethod]
        public void SetNameIsNotCalledAfterFirstDataBlock()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var saveProvider = new FakeSaveToTapeProvider();
            var td = new TapeDevice(saveProvider);
            td.OnAttachedToVm(vm);
            var testData = new byte[]
            {
                0x00, 0x00,
                0x42, 0x6F, 0x72, 0x64, 0x65, 0x72, 0x20, 0x20, 0x20, 0x20,
                0x4F, 0x00, 0x6F, 0x80, 0x4F, 0x00, 0xC3
            };
            EmitFullDataBlock(vm, td, testData);
            var before = saveProvider.SuggestedName;
            saveProvider.Reset();

            // --- Act
            EmitFullDataBlock(vm, td, testData);

            // --- Assert
            before.ShouldBe("Border");
            saveProvider.SuggestedName.ShouldBeNull();
        }

        private (IZ80CpuTestSupport, int, bool) EmitHeaderWithSync(ISpectrumVm vm, TapeDevice td)
        {
            vm.Cpu.Registers.PC = td.SaveBytesRoutineAddress;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < TapeDevice.MIN_PILOT_PULSE_COUNT + 10; i++)
            {
                tacts += TapeDataBlockPlayer.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }
            tacts += TapeDataBlockPlayer.SYNC_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;
            tacts += TapeDataBlockPlayer.SYNC_2_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;
            return (debugCpu, tacts, pulse);
        }

        private (IZ80CpuTestSupport, int, bool) EmitHeaderAndData(ISpectrumVm vm, TapeDevice td, byte[] data)
        {
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);
            foreach (var byteValue in data)
            {
                var dataByte = byteValue;
                for (var i = 0; i < 8; i++)
                {
                    var length = (dataByte & 0x80) == 0
                        ? TapeDataBlockPlayer.BIT_0_PL
                        : TapeDataBlockPlayer.BIT_1_PL;
                    tacts += length;
                    debugCpu.SetTacts(tacts);
                    td.ProcessMicBit(pulse);
                    pulse = !pulse;
                    tacts += length;
                    debugCpu.SetTacts(tacts);
                    td.ProcessMicBit(pulse);
                    pulse = !pulse;

                    dataByte = (byte) (dataByte << 1);
                }
            }
            return (debugCpu, tacts, pulse);
        }

        private (IZ80CpuTestSupport, int, bool) EmitFullDataBlock(ISpectrumVm vm, TapeDevice td, byte[] data)
        {
            (var debugCpu, var tacts, var pulse) = EmitHeaderAndData(vm, td, data);
            tacts += TapeDataBlockPlayer.TERM_SYNC;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            return (debugCpu, tacts, pulse);
        }

        private class SpectrumTapeDeviceTestMachine : SpectrumAdvancedTestMachine
        {
        }

        private class EmptyTapeContentProvider : VmComponentProviderBase, ITapeProvider
        {
            /// <summary>
            /// Tha tape set to load the content from
            /// </summary>
            public string TapeSetName { get; set; }

            /// <summary>
            /// Gets a binary reader that provider TZX content
            /// </summary>
            /// <returns></returns>
            public BinaryReader GetTapeContent()
            {
                return new BinaryReader(Stream.Null);
            }

            /// <summary>
            /// Creates a tape file with the specified name
            /// </summary>
            /// <returns></returns>
            public void CreateTapeFile()
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// This method sets the name of the file according to the 
            /// Spectrum SAVE HEADER information
            /// </summary>
            /// <param name="name"></param>
            public void SetName(string name)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Appends the TZX block to the tape file
            /// </summary>
            /// <param name="block"></param>
            public void SaveTapeBlock(ITapeDataSerialization block)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// The tape provider can finalize the tape when all 
            /// TZX blocks are written.
            /// </summary>
            public void FinalizeTapeFile()
            {
                throw new System.NotImplementedException();
            }
        }

        private class FakeSaveToTapeProvider : VmComponentProviderBase, ITapeProvider
        {
            public bool CreateTapeFileInvoked { get; private set; }
            public bool SaveTzxBlockInvoked { get; private set; }
            public string SuggestedName { get; private set; }
            public bool FinalizeTapeFileInvoked { get; private set; }

            public override void Reset()
            {
                SuggestedName = null;
            }

            /// <summary>
            /// Tha tape set to load the content from
            /// </summary>
            public string TapeSetName { get; set; }

            /// <summary>
            /// Gets a binary reader that provider TZX content
            /// </summary>
            /// <returns>BinaryReader instance to obtain the content from</returns>
            public BinaryReader GetTapeContent()
            {
                throw new System.NotImplementedException();
            }

            public void CreateTapeFile()
            {
                CreateTapeFileInvoked = true;
            }

            public void SetName(string name)
            {
                SuggestedName = name;
            }

            public void SaveTapeBlock(ITapeDataSerialization block)
            {
                SaveTzxBlockInvoked = true;
            }

            public void FinalizeTapeFile()
            {
                FinalizeTapeFileInvoked = true;
            }
        }
    }
}
