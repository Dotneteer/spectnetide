using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Devices.Sound;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Devices.Sound
{
    [TestClass]
    public class SoundDeviceTest
    {
        [TestMethod]
        public void DeviceIsInitializedProperty()
        {
            // --- Arrange
            var spectrum = new SpectrumSoundTestMachine();

            // --- Act
            var soundDev = new SoundDevice();
            soundDev.OnAttachedToVm(spectrum);

            // --- Assert
            soundDev.LastRegisterIndex.ShouldBe((byte)0);
            soundDev.PsgSnapshots.Count.ShouldBe(1);
            for (var i = 0; i <= 0x0F; i++)
            {
                soundDev.LastPsgState[i].ShouldBe((byte)0);
            }
        }

        [TestMethod]
        [DataRow(0x00)]
        [DataRow(0x01)]
        [DataRow(0x02)]
        [DataRow(0x03)]
        [DataRow(0x04)]
        [DataRow(0x05)]
        [DataRow(0x06)]
        [DataRow(0x07)]
        [DataRow(0x08)]
        [DataRow(0x09)]
        [DataRow(0x0A)]
        [DataRow(0x0B)]
        [DataRow(0x0C)]
        [DataRow(0x0D)]
        [DataRow(0x0E)]
        [DataRow(0x0F)]
        public void RegisterIndexIsSetProperty(int regIndex)
        {
            // --- Arrange
            var spectrum = new SpectrumSoundTestMachine();
            var soundDev = new SoundDevice();
            soundDev.OnAttachedToVm(spectrum);

            // --- Act
            soundDev.SetRegisterIndex((byte)regIndex);

            // --- Assert
            soundDev.LastRegisterIndex.ShouldBe((byte)regIndex);
            soundDev.PsgSnapshots.Count.ShouldBe(1);
        }

        [TestMethod]
        [DataRow(0x00)]
        [DataRow(0x01)]
        [DataRow(0x02)]
        [DataRow(0x03)]
        [DataRow(0x04)]
        [DataRow(0x05)]
        [DataRow(0x06)]
        [DataRow(0x07)]
        [DataRow(0x08)]
        [DataRow(0x09)]
        [DataRow(0x0A)]
        [DataRow(0x0B)]
        [DataRow(0x0C)]
        [DataRow(0x0D)]
        [DataRow(0x0E)]
        [DataRow(0x0F)]
        public void RegisterValueIsSetProperty(int regIndex)
        {
            // --- Arrange
            var spectrum = new SpectrumSoundTestMachine();
            var soundDev = new SoundDevice();
            soundDev.OnAttachedToVm(spectrum);
            soundDev.SetRegisterIndex((byte)regIndex);

            // --- Act
            soundDev.SetRegisterValue((byte)(regIndex + 1));

            // --- Assert
            soundDev.LastRegisterIndex.ShouldBe((byte)regIndex);
            soundDev.LastPsgState[(byte)regIndex].ShouldBe((byte)(regIndex + 1));    
            soundDev.PsgSnapshots.Count.ShouldBe(2);
            for (var i = 0; i <= 0x0F; i++)
            {
                soundDev.PsgSnapshots[1][i].ShouldBe(soundDev.LastPsgState[i]);
            }
            soundDev.PsgSnapshots[1].ChangedRegisterIndex.ShouldBe(soundDev.LastRegisterIndex);
        }

        [TestMethod]
        [DataRow(0x00)]
        [DataRow(0x01)]
        [DataRow(0x02)]
        [DataRow(0x03)]
        [DataRow(0x04)]
        [DataRow(0x05)]
        [DataRow(0x06)]
        [DataRow(0x07)]
        [DataRow(0x08)]
        [DataRow(0x09)]
        [DataRow(0x0A)]
        [DataRow(0x0B)]
        [DataRow(0x0C)]
        [DataRow(0x0D)]
        [DataRow(0x0E)]
        [DataRow(0x0F)]
        public void OutOperationChangesRegisterIndex(int regIndex)
        {
            // --- Arrange
            var m = new SpectrumSoundTestMachine();
            var soundDev = m.SoundDevice;

            m.InitCode(new byte[]
            {
                0x01, 0xFD, 0xFF,     // LD BC,#FFFD
                0x3E, (byte)regIndex, // LD A,regIndex
                0xED, 0x79,           // OUT (C),A
                0x76                  // HALT
            });

            // --- Act
            m.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            soundDev.LastRegisterIndex.ShouldBe((byte)regIndex);
        }

        [TestMethod]
        [DataRow(0x00)]
        [DataRow(0x01)]
        [DataRow(0x02)]
        [DataRow(0x03)]
        [DataRow(0x04)]
        [DataRow(0x05)]
        [DataRow(0x06)]
        [DataRow(0x07)]
        [DataRow(0x08)]
        [DataRow(0x09)]
        [DataRow(0x0A)]
        [DataRow(0x0B)]
        [DataRow(0x0C)]
        [DataRow(0x0D)]
        [DataRow(0x0E)]
        [DataRow(0x0F)]
        public void OutOperationChangesRegisterValue(int regIndex)
        {
            // --- Arrange
            var m = new SpectrumSoundTestMachine();
            var soundDev = m.SoundDevice;

            m.InitCode(new byte[]
            {
                0x01, 0xFD, 0xFF,     // LD BC,#FFFD
                0x3E, (byte)regIndex, // LD A,regIndex
                0xED, 0x79,           // OUT (C),A
                0x06, 0xBF,           // LD B,#BF
                0x3C,                 // INC A
                0xED, 0x79,           // OUT (C),A
                0x76                  // HALT
            });

            // --- Act
            m.ExecuteCycle(CancellationToken.None, new ExecuteCycleOptions(EmulationMode.UntilHalt));

            // --- Assert
            soundDev.LastRegisterIndex.ShouldBe((byte)regIndex);
            soundDev.LastPsgState[(byte)regIndex].ShouldBe((byte)(regIndex + 1));
            soundDev.PsgSnapshots.Count.ShouldBe(2);
            for (var i = 0; i <= 0x0F; i++)
            {
                soundDev.PsgSnapshots[1][i].ShouldBe(soundDev.LastPsgState[i]);
            }
            soundDev.PsgSnapshots[1].ChangedRegisterIndex.ShouldBe((byte)regIndex);
            soundDev.PsgSnapshots[1].CpuTact.ShouldBe(52);
        }

        /// <summary>
        /// Test machine for checking the sound device
        /// </summary>
        private class SpectrumSoundTestMachine : Spectrum128AdvancedTestMachine
        {
        }

    }
}
