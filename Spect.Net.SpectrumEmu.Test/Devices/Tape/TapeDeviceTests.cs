using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.SpectrumEmu.Providers;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Passive);
        }

        [TestMethod]
        public void SetTapeModeStaysInPassiveMode()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null, null);
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.LOAD_START_ROM_ADDRESS;

            // --- Act
            td.SetTapeMode();

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Load);
        }

        [TestMethod]
        public void SetTapeModeEntersSaveMode()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;

            // --- Act
            td.SetTapeMode();

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
        }

        [TestMethod]
        public void SetTapeModeLeavesLoadModeWhenError()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.LOAD_START_ROM_ADDRESS;
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
        public void SetTapeModeLeavesSaveModeWhenError()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
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
        public void SetTapeModeLeavesLoadModeWhenEof()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(new EmptyTzxContentProvider(), null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.LOAD_START_ROM_ADDRESS;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
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
        public void GetEarBitsReturnTrueInPassiveMode()
        {
            // --- Arrange
            var vm = new SpectrumTapeDeviceTestMachine();
            var td = new TapeDevice(null, null);
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.LOAD_START_ROM_ADDRESS;
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
            var td = new TapeDevice(null, null);
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.LOAD_START_ROM_ADDRESS;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;

            // --- Act
            debugCpu.SetTacts(TzxStandardSpeedDataBlock.PILOT_PL);
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;

            // --- Act
            debugCpu.SetTacts(TzxStandardSpeedDataBlock.PILOT_PL - TapeDevice.SAVE_PULSE_TOLERANCE - 1);
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;

            // --- Act
            debugCpu.SetTacts(TzxStandardSpeedDataBlock.PILOT_PL + TapeDevice.SAVE_PULSE_TOLERANCE + 1);
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;

            // --- Act
            for (var i = 0; i < 10; i++)
            {
                tacts += TzxStandardSpeedDataBlock.PILOT_PL;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < 10; i++)
            {
                tacts += TzxStandardSpeedDataBlock.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }

            // --- Act
            tacts += TzxStandardSpeedDataBlock.SYNC_1_PL;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < TapeDevice.MIN_PILOT_PULSE_COUNT + 10; i++)
            {
                tacts += TzxStandardSpeedDataBlock.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }

            // --- Act
            tacts += TzxStandardSpeedDataBlock.SYNC_1_PL;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < TapeDevice.MIN_PILOT_PULSE_COUNT + 10; i++)
            {
                tacts += TzxStandardSpeedDataBlock.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }

            // --- Act
            tacts += TzxStandardSpeedDataBlock.SYNC_1_PL - TapeDevice.SAVE_PULSE_TOLERANCE - 1;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < TapeDevice.MIN_PILOT_PULSE_COUNT + 10; i++)
            {
                tacts += TzxStandardSpeedDataBlock.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }

            // --- Act
            tacts += TzxStandardSpeedDataBlock.SYNC_1_PL + TapeDevice.SAVE_PULSE_TOLERANCE + 1;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < TapeDevice.MIN_PILOT_PULSE_COUNT + 10; i++)
            {
                tacts += TzxStandardSpeedDataBlock.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }
            tacts += TzxStandardSpeedDataBlock.SYNC_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;

            // --- Act
            tacts += TzxStandardSpeedDataBlock.SYNC_2_PL;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < TapeDevice.MIN_PILOT_PULSE_COUNT + 10; i++)
            {
                tacts += TzxStandardSpeedDataBlock.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }
            tacts += TzxStandardSpeedDataBlock.SYNC_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;

            // --- Act
            tacts += TzxStandardSpeedDataBlock.SYNC_2_PL - TapeDevice.SAVE_PULSE_TOLERANCE - 1;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < TapeDevice.MIN_PILOT_PULSE_COUNT + 10; i++)
            {
                tacts += TzxStandardSpeedDataBlock.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }
            tacts += TzxStandardSpeedDataBlock.SYNC_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;

            // --- Act
            tacts += TzxStandardSpeedDataBlock.SYNC_2_PL + TapeDevice.SAVE_PULSE_TOLERANCE + 1;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);

            // --- Act
            tacts += TzxStandardSpeedDataBlock.BIT_0_PL;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);

            // --- Act
            tacts += TzxStandardSpeedDataBlock.BIT_0_PL - TapeDevice.SAVE_PULSE_TOLERANCE - 1;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);

            // --- Act
            tacts += TzxStandardSpeedDataBlock.BIT_0_PL + TapeDevice.SAVE_PULSE_TOLERANCE + 1;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);
            tacts += TzxStandardSpeedDataBlock.BIT_0_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;

            // --- Act
            tacts += TzxStandardSpeedDataBlock.BIT_0_PL;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);

            // --- Act
            tacts += TzxStandardSpeedDataBlock.BIT_1_PL;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);

            // --- Act
            tacts += TzxStandardSpeedDataBlock.BIT_1_PL - TapeDevice.SAVE_PULSE_TOLERANCE - 1;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);

            // --- Act
            tacts += TzxStandardSpeedDataBlock.BIT_1_PL + TapeDevice.SAVE_PULSE_TOLERANCE + 1;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);
            tacts += TzxStandardSpeedDataBlock.BIT_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;

            // --- Act
            tacts += TzxStandardSpeedDataBlock.BIT_1_PL;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);

            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);
            tacts += TzxStandardSpeedDataBlock.BIT_0_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;

            // --- Act
            tacts += TzxStandardSpeedDataBlock.BIT_1_PL;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            (var debugCpu, var tacts, var pulse) = EmitHeaderWithSync(vm, td);
            tacts += TzxStandardSpeedDataBlock.BIT_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;

            // --- Act
            tacts += TzxStandardSpeedDataBlock.BIT_0_PL;
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
            var td = new TapeDevice(null, null);
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            var testData = new byte[] { 0x90, 0x02, 0x05, 0xAA, 0xFF, 0x63 };
            (var debugCpu, var tacts, var pulse) = EmitHeaderAndData(vm, td, testData);

            // --- Act
            tacts += TzxStandardSpeedDataBlock.TERM_SYNC;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            var testData = new byte[] { 0x90, 0x02, 0x05, 0xAA, 0xFF, 0x63 };
            (var debugCpu, var tacts, var pulse) = EmitHeaderAndData(vm, td, testData);

            // --- Act
            tacts += TzxStandardSpeedDataBlock.TERM_SYNC - TapeDevice.SAVE_PULSE_TOLERANCE - 1;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            var testData = new byte[] { 0x90, 0x02, 0x05, 0xAA, 0xFF, 0x63 };
            (var debugCpu, var tacts, var pulse) = EmitHeaderAndData(vm, td, testData);

            // --- Act
            tacts += TzxStandardSpeedDataBlock.TERM_SYNC + TapeDevice.SAVE_PULSE_TOLERANCE + 1;
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
            var td = new TapeDevice(null, null);
            td.OnAttachedToVm(vm);
            var testData = new byte[] { 0x90, 0x02, 0x05, 0xAA, 0xFF, 0x63 };

            // --- Act
            (var debugCpu, var tacts, var pulse) = EmitFullDataBlock(vm, td, testData);
            tacts += TzxStandardSpeedDataBlock.PILOT_PL * 5;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            (debugCpu, tacts, pulse) = EmitFullDataBlock(vm, td, testData);
            tacts += TzxStandardSpeedDataBlock.PILOT_PL * 5;
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
            var saveProvider = new FakeTzxSaveProvider();
            var td = new TapeDevice(null, saveProvider);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;

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
            var saveProvider = new FakeTzxSaveProvider();
            var td = new TapeDevice(null, saveProvider);
            td.OnAttachedToVm(vm);
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
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
            var saveProvider = new FakeTzxSaveProvider();
            var td = new TapeDevice(null, saveProvider);
            td.OnAttachedToVm(vm);
            var testData = new byte[] { 0x90, 0x02, 0x05, 0xAA, 0xFF, 0x63 };

            // --- Act
            EmitFullDataBlock(vm, td, testData);

            // --- Assert
            td.CurrentMode.ShouldBe(TapeOperationMode.Save);
            td.SavePhase.ShouldBe(SavePhase.None);
            saveProvider.SaveTzxBlockInvoked.ShouldBeTrue();
        }

        private (IZ80CpuTestSupport, int, bool) EmitHeaderWithSync(ISpectrumVm vm, TapeDevice td)
        {
            vm.Cpu.Registers.PC = TapeDevice.SAVE_BYTES_ROM_ADDRESS;
            td.SetTapeMode();
            var debugCpu = vm.Cpu as IZ80CpuTestSupport;
            var pulse = false;
            var tacts = 0;
            for (var i = 0; i < TapeDevice.MIN_PILOT_PULSE_COUNT + 10; i++)
            {
                tacts += TzxStandardSpeedDataBlock.PILOT_PL;
                debugCpu.SetTacts(tacts);
                td.ProcessMicBit(pulse);
                pulse = !pulse;
            }
            tacts += TzxStandardSpeedDataBlock.SYNC_1_PL;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            pulse = !pulse;
            tacts += TzxStandardSpeedDataBlock.SYNC_2_PL;
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
                        ? TzxStandardSpeedDataBlock.BIT_0_PL
                        : TzxStandardSpeedDataBlock.BIT_1_PL;
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
            tacts += TzxStandardSpeedDataBlock.TERM_SYNC;
            debugCpu.SetTacts(tacts);
            td.ProcessMicBit(pulse);
            return (debugCpu, tacts, pulse);
        }

        private class SpectrumTapeDeviceTestMachine : SpectrumAdvancedTestMachine
        {
        }

        private class EmptyTzxContentProvider : ITzxTapeContentProvider
        {
            public void Reset()
            {
            }

            /// <summary>
            /// Gets a binary reader that provider TZX content
            /// </summary>
            /// <returns></returns>
            public BinaryReader GetTzxContent()
            {
                return new BinaryReader(Stream.Null);
            }
        }

        private class FakeTzxSaveProvider : ITzxSaveProvider
        {
            public bool CreateTapeFileInvoked { get; private set; }
            public bool SaveTzxBlockInvoked { get; private set; }
            public bool FinalizeTapeFileInvoked { get; private set; }

            public void Reset()
            {
            }

            public void CreateTapeFile(string name = null)
            {
                CreateTapeFileInvoked = true;
            }

            public void SaveTzxBlock(ITzxSerialization block)
            {
                SaveTzxBlockInvoked = true;
            }

            public void FinalizeTapeFile(string name = null)
            {
                FinalizeTapeFileInvoked = true;
            }
        }
    }
}
