using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Devices.Sound;
using Spect.Net.SpectrumEmu.Test.Helpers;

// ReSharper disable UseObjectOrCollectionInitializer

namespace Spect.Net.SpectrumEmu.Test.Devices.Sound
{
    [TestClass]
    public class PsgStateTest
    {
        [TestMethod]
        [DataRow(0x0000, 0x12, 0x12, 0x0012)]
        [DataRow(0x0023, 0x12, 0x12, 0x0012)]
        [DataRow(0x0c23, 0x12, 0x12, 0x0c12)]
        [DataRow(0x5c23, 0x12, 0x12, 0x0c12)]
        public void ChannelAFineTuneWorks(int orig, int value, int result, int channel)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register0 = (byte) orig;
            psg.Register1 = (byte) (orig >> 8);
            psg.Register0 = (byte)value;

            // --- Assert
            psg.Register0.ShouldBe((byte)result);
            psg.ChannelA.ShouldBe((ushort)channel);
            psg.ChannelAModified.ShouldBe(47);
        }

        [TestMethod]
        [DataRow(0x0000, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x2300, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x0A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x0A34, 0x2D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x4D, 0x0D, 0x0D34)]
        public void ChannelACoarseTuneWorks(int orig, int value, int result, int channel)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register0 = (byte)orig;
            psg.Register1 = (byte)(orig >> 8);
            psg.Register1 = (byte)value;

            // --- Assert
            psg.Register1.ShouldBe((byte)result);
            psg.ChannelA.ShouldBe((ushort)channel);
            psg.ChannelAModified.ShouldBe(47);
        }

        [TestMethod]
        [DataRow(0x0000, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x2300, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x0A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x0A34, 0x2D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x4D, 0x0D, 0x0D34)]
        public void ChannelALastModificationWithLsbFirstWorks(int orig, int value, int result, int channel)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);

            // --- Act
            hostVm.SetCurrentCpuTact(47);
            psg.Register0 = (byte)orig;
            psg.Register1 = (byte)(orig >> 8);
            hostVm.SetCurrentCpuTact(48);
            psg.Register1 = (byte)value;

            // --- Assert
            psg.Register1.ShouldBe((byte)result);
            psg.ChannelA.ShouldBe((ushort)channel);
            psg.ChannelAModified.ShouldBe(48);
        }

        [TestMethod]
        [DataRow(0x0000, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x2300, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x0A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x0A34, 0x2D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x4D, 0x0D, 0x0D34)]
        public void ChannelALastModificationWithMsbFirstWorks(int orig, int value, int result, int channel)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);

            // --- Act
            hostVm.SetCurrentCpuTact(47);
            psg.Register1 = (byte)(orig >> 8);
            psg.Register1 = (byte)value;
            hostVm.SetCurrentCpuTact(48);
            psg.Register0 = (byte)orig;

            // --- Assert
            psg.Register1.ShouldBe((byte)result);
            psg.ChannelA.ShouldBe((ushort)channel);
            psg.ChannelAModified.ShouldBe(48);
        }

        [TestMethod]
        [DataRow(0x0000, 0x12, 0x12, 0x0012)]
        [DataRow(0x0023, 0x12, 0x12, 0x0012)]
        [DataRow(0x0c23, 0x12, 0x12, 0x0c12)]
        [DataRow(0x5c23, 0x12, 0x12, 0x0c12)]
        public void ChannelBFineTuneWorks(int orig, int value, int result, int channel)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register2 = (byte)orig;
            psg.Register3 = (byte)(orig >> 8);
            psg.Register2 = (byte)value;

            // --- Assert
            psg.Register2.ShouldBe((byte)result);
            psg.ChannelB.ShouldBe((ushort)channel);
            psg.ChannelBModified.ShouldBe(47);
        }

        [TestMethod]
        [DataRow(0x0000, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x2300, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x0A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x0A34, 0x2D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x4D, 0x0D, 0x0D34)]
        public void ChannelBCoarseTuneWorks(int orig, int value, int result, int channel)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register2 = (byte)orig;
            psg.Register3 = (byte)(orig >> 8);
            psg.Register3 = (byte)value;

            // --- Assert
            psg.Register3.ShouldBe((byte)result);
            psg.ChannelB.ShouldBe((ushort)channel);
            psg.ChannelBModified.ShouldBe(47);
        }

        [TestMethod]
        [DataRow(0x0000, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x2300, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x0A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x0A34, 0x2D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x4D, 0x0D, 0x0D34)]
        public void ChannelBLastModificationWithLsbFirstWorks(int orig, int value, int result, int channel)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);

            // --- Act
            hostVm.SetCurrentCpuTact(47);
            psg.Register2 = (byte)orig;
            psg.Register3 = (byte)(orig >> 8);
            hostVm.SetCurrentCpuTact(48);
            psg.Register3 = (byte)value;

            // --- Assert
            psg.Register3.ShouldBe((byte)result);
            psg.ChannelB.ShouldBe((ushort)channel);
            psg.ChannelBModified.ShouldBe(48);
        }

        [TestMethod]
        [DataRow(0x0000, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x2300, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x0A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x0A34, 0x2D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x4D, 0x0D, 0x0D34)]
        public void ChannelBLastModificationWithMsbFirstWorks(int orig, int value, int result, int channel)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);

            // --- Act
            hostVm.SetCurrentCpuTact(47);
            psg.Register3 = (byte)(orig >> 8);
            psg.Register3 = (byte)value;
            hostVm.SetCurrentCpuTact(48);
            psg.Register2 = (byte)orig;

            // --- Assert
            psg.Register3.ShouldBe((byte)result);
            psg.ChannelB.ShouldBe((ushort)channel);
            psg.ChannelBModified.ShouldBe(48);
        }

        [TestMethod]
        [DataRow(0x0000, 0x12, 0x12, 0x0012)]
        [DataRow(0x0023, 0x12, 0x12, 0x0012)]
        [DataRow(0x0c23, 0x12, 0x12, 0x0c12)]
        [DataRow(0x5c23, 0x12, 0x12, 0x0c12)]
        public void ChannelCFineTuneWorks(int orig, int value, int result, int channel)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register4 = (byte)orig;
            psg.Register5 = (byte)(orig >> 8);
            psg.Register4 = (byte)value;

            // --- Assert
            psg.Register4.ShouldBe((byte)result);
            psg.ChannelC.ShouldBe((ushort)channel);
            psg.ChannelCModified.ShouldBe(47);
        }

        [TestMethod]
        [DataRow(0x0000, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x2300, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x0A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x0A34, 0x2D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x4D, 0x0D, 0x0D34)]
        public void ChannelCCoarseTuneWorks(int orig, int value, int result, int channel)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register4 = (byte)orig;
            psg.Register5 = (byte)(orig >> 8);
            psg.Register5 = (byte)value;

            // --- Assert
            psg.Register5.ShouldBe((byte)result);
            psg.ChannelC.ShouldBe((ushort)channel);
            psg.ChannelCModified.ShouldBe(47);
        }

        [TestMethod]
        [DataRow(0x0000, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x2300, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x0A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x0A34, 0x2D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x4D, 0x0D, 0x0D34)]
        public void ChannelCLastModificationWithLsbFirstWorks(int orig, int value, int result, int channel)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);

            // --- Act
            hostVm.SetCurrentCpuTact(47);
            psg.Register4 = (byte)orig;
            psg.Register5 = (byte)(orig >> 8);
            hostVm.SetCurrentCpuTact(48);
            psg.Register5 = (byte)value;

            // --- Assert
            psg.Register5.ShouldBe((byte)result);
            psg.ChannelC.ShouldBe((ushort)channel);
            psg.ChannelCModified.ShouldBe(48);
        }

        [TestMethod]
        [DataRow(0x0000, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x2300, 0x0D, 0x0D, 0x0D00)]
        [DataRow(0x0A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x0D, 0x0D, 0x0D34)]
        [DataRow(0x0A34, 0x2D, 0x0D, 0x0D34)]
        [DataRow(0x9A34, 0x4D, 0x0D, 0x0D34)]
        public void ChannelCLastModificationWithMsbFirstWorks(int orig, int value, int result, int channel)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);

            // --- Act
            hostVm.SetCurrentCpuTact(47);
            psg.Register5 = (byte)(orig >> 8);
            psg.Register5 = (byte)value;
            hostVm.SetCurrentCpuTact(48);
            psg.Register4 = (byte)orig;

            // --- Assert
            psg.Register5.ShouldBe((byte)result);
            psg.ChannelC.ShouldBe((ushort)channel);
            psg.ChannelCModified.ShouldBe(48);
        }

        [TestMethod]
        [DataRow(0x00, 0x00)]
        [DataRow(0x23, 0x23)]
        [DataRow(0xC3, 0x03)]
        [DataRow(0xF6, 0x36)]
        public void Register6Works(int value, int result)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register6 = (byte)value;

            // --- Assert
            psg.Register6.ShouldBe((byte)result);
            psg.NoisePeriodModified.ShouldBe(47);
        }

        [TestMethod]
        [DataRow(0x00, 0x00)]
        [DataRow(0x23, 0x23)]
        [DataRow(0xC3, 0x43)]
        [DataRow(0xF6, 0x76)]
        public void Register7Works(int value, int result)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register7 = (byte)value;

            // --- Assert
            psg.Register7.ShouldBe((byte)result);
            psg.MixerModified.ShouldBe(47);
        }

        [TestMethod]
        [DataRow(0x00, true)]
        [DataRow(0xC3, false)]
        [DataRow(0xF6, false)]
        [DataRow(0x84, true)]
        public void InputEnableWorks(int value, bool result)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register7 = (byte)value;

            // --- Assert
            psg.InputEnabled.ShouldBe(result);
        }

        [TestMethod]
        [DataRow(0x40, true)]
        [DataRow(0xa5, false)]
        [DataRow(0xF7, false)]
        [DataRow(0x84, true)]
        public void ToneAEnableWorks(int value, bool result)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register7 = (byte)value;

            // --- Assert
            psg.ToneAEnabled.ShouldBe(result);
        }

        [TestMethod]
        [DataRow(0x41, true)]
        [DataRow(0xa3, false)]
        [DataRow(0xF7, false)]
        [DataRow(0x85, true)]
        public void ToneBEnableWorks(int value, bool result)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register7 = (byte)value;

            // --- Assert
            psg.ToneBEnabled.ShouldBe(result);
        }

        [TestMethod]
        [DataRow(0x43, true)]
        [DataRow(0x44, false)]
        [DataRow(0x46, false)]
        [DataRow(0x83, true)]
        public void ToneCEnableWorks(int value, bool result)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register7 = (byte)value;

            // --- Assert
            psg.ToneCEnabled.ShouldBe(result);
        }

        [TestMethod]
        [DataRow(0x47, true)]
        [DataRow(0x48, false)]
        [DataRow(0x4A, false)]
        [DataRow(0x83, true)]
        public void NoiseAEnableWorks(int value, bool result)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register7 = (byte)value;

            // --- Assert
            psg.NoiseAEnabled.ShouldBe(result);
        }

        [TestMethod]
        [DataRow(0x46, true)]
        [DataRow(0x58, false)]
        [DataRow(0x14, false)]
        [DataRow(0x83, true)]
        public void NoiseBEnableWorks(int value, bool result)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register7 = (byte)value;

            // --- Assert
            psg.NoiseBEnabled.ShouldBe(result);
        }

        [TestMethod]
        [DataRow(0x16, true)]
        [DataRow(0x28, false)]
        [DataRow(0x74, false)]
        [DataRow(0x43, true)]
        public void NoiseCEnableWorks(int value, bool result)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register7 = (byte)value;

            // --- Assert
            psg.NoiseCEnabled.ShouldBe(result);
        }

        [TestMethod]
        [DataRow(0x00, 0x00, false)]
        [DataRow(0x0F, 0x0F, false)]
        [DataRow(0x12, 0x12, true)]
        [DataRow(0x53, 0x13, true)]
        public void Register8Works(int value, int result, bool envelope)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register8 = (byte)value;

            // --- Assert
            psg.Register8.ShouldBe((byte)result);
            psg.AmplitudeAModified.ShouldBe(47);
            psg.UseEnvelopeA.ShouldBe(envelope);
        }

        [TestMethod]
        [DataRow(0x00, 0x00, false)]
        [DataRow(0x0F, 0x0F, false)]
        [DataRow(0x12, 0x12, true)]
        [DataRow(0x53, 0x13, true)]
        public void Register9Works(int value, int result, bool envelope)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register9 = (byte)value;

            // --- Assert
            psg.Register9.ShouldBe((byte)result);
            psg.AmplitudeBModified.ShouldBe(47);
            psg.UseEnvelopeB.ShouldBe(envelope);
        }

        [TestMethod]
        [DataRow(0x00, 0x00, false)]
        [DataRow(0x0F, 0x0F, false)]
        [DataRow(0x12, 0x12, true)]
        [DataRow(0x53, 0x13, true)]
        public void Register10Works(int value, int result, bool envelope)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register10 = (byte)value;

            // --- Assert
            psg.Register10.ShouldBe((byte)result);
            psg.AmplitudeCModified.ShouldBe(47);
            psg.UseEnvelopeC.ShouldBe(envelope);
        }

        [TestMethod]
        [DataRow(0x00, 0x00, 0x0000)]
        [DataRow(0x12, 0x23, 0x2312)]
        [DataRow(0xAE, 0x5C, 0x5CAE)]
        public void Register11And12Works(int reg11, int reg12, int result)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register11 = (byte)reg11;
            psg.Register12 = (byte)reg12;

            // --- Assert
            psg.EnvelopePeriod.ShouldBe((ushort)result);
        }

        [TestMethod]
        [DataRow(0x00, 0x00, 0x0000)]
        [DataRow(0x12, 0x23, 0x2312)]
        [DataRow(0xAE, 0x5C, 0x5CAE)]
        public void EnvelopePeriodModifiedWithLsbFirstWorks(int reg11, int reg12, int result)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);

            // --- Act
            hostVm.SetCurrentCpuTact(47);
            psg.Register11 = (byte)reg11;
            hostVm.SetCurrentCpuTact(48);
            psg.Register12 = (byte)reg12;

            // --- Assert
            psg.EnvelopePeriod.ShouldBe((ushort)result);
            psg.EnvelopePeriodModified.ShouldBe(48);
        }

        [TestMethod]
        [DataRow(0x00, 0x00, 0x0000)]
        [DataRow(0x12, 0x23, 0x2312)]
        [DataRow(0xAE, 0x5C, 0x5CAE)]
        public void EnvelopePeriodModifiedWithMsbFirstWorks(int reg11, int reg12, int result)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);

            // --- Act
            hostVm.SetCurrentCpuTact(47);
            psg.Register12 = (byte)reg12;
            hostVm.SetCurrentCpuTact(48);
            psg.Register11 = (byte)reg11;

            // --- Assert
            psg.EnvelopePeriod.ShouldBe((ushort)result);
            psg.EnvelopePeriodModified.ShouldBe(48);
        }

        [TestMethod]
        [DataRow(0x00, 0x00, false, false, false, false)]
        [DataRow(0xF0, 0x00, false, false, false, false)]
        [DataRow(0x01, 0x01, false, false, false, true)]
        [DataRow(0x02, 0x02, false, false, true, false)]
        [DataRow(0x03, 0x03, false, false, true, true)]
        [DataRow(0x04, 0x04, false, true, false, false)]
        [DataRow(0x05, 0x05, false, true, false, true)]
        [DataRow(0x06, 0x06, false, true, true, false)]
        [DataRow(0x07, 0x07, false, true, true, true)]
        [DataRow(0x08, 0x08, true, false, false, false)]
        [DataRow(0x09, 0x09, true, false, false, true)]
        [DataRow(0x0A, 0x0A, true, false, true, false)]
        [DataRow(0x0B, 0x0B, true, false, true, true)]
        [DataRow(0x0C, 0x0C, true, true, false, false)]
        [DataRow(0x0D, 0x0D, true, true, false, true)]
        [DataRow(0x0E, 0x0E, true, true, true, false)]
        [DataRow(0x0F, 0x0F, true, true, true, true)]
        public void Register13Works(int value, int result, bool cont, bool attack, bool alternate, bool hold)
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg.Register13 = (byte)value;

            // --- Assert
            psg.Register13.ShouldBe((byte)result);
            psg.EnvelopeShapeModified.ShouldBe(47);
            psg.HoldFlag.ShouldBe(hold);
            psg.AlternateFlag.ShouldBe(alternate);
            psg.AttackFlag.ShouldBe(attack);
            psg.ContinueFlag.ShouldBe(cont);
        }

        [TestMethod]
        public void RegisterIndexerGetsValueProperly()
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);
            psg.Register0 = 0;
            psg.Register1 = 1;
            psg.Register2 = 2;
            psg.Register3 = 3;
            psg.Register4 = 4;
            psg.Register5 = 5;
            psg.Register6 = 6;
            psg.Register7 = 7;
            psg.Register8 = 8;
            psg.Register9 = 9;
            psg.Register10 = 10;
            psg.Register11 = 11;
            psg.Register12 = 12;
            psg.Register13 = 13;
            psg.Register14 = 14;

            // --- Act/Assert
            psg[0].ShouldBe((byte)0);
            psg[1].ShouldBe((byte)1);
            psg[2].ShouldBe((byte)2);
            psg[3].ShouldBe((byte)3);
            psg[4].ShouldBe((byte)4);
            psg[5].ShouldBe((byte)5);
            psg[6].ShouldBe((byte)6);
            psg[7].ShouldBe((byte)7);
            psg[8].ShouldBe((byte)8);
            psg[9].ShouldBe((byte)9);
            psg[10].ShouldBe((byte)10);
            psg[11].ShouldBe((byte)11);
            psg[12].ShouldBe((byte)12);
            psg[13].ShouldBe((byte)13);
            psg[14].ShouldBe((byte)14);
        }

        [TestMethod]
        public void RegisterIndexerSetsValueProperly()
        {
            // --- Arrange
            var hostVm = new SpectrumSoundTestMachine();
            var psg = new PsgState(hostVm);
            hostVm.SetCurrentCpuTact(47);

            // --- Act
            psg[0] = 0;
            psg[1] = 1;
            psg[2] = 2;
            psg[3] = 3;
            psg[4] = 4;
            psg[5] = 5;
            psg[6] = 6;
            psg[7] = 7;
            psg[8] = 8;
            psg[9] = 9;
            psg[10] = 10;
            psg[11] = 11;
            psg[12] = 12;
            psg[13] = 13;
            psg[14] = 14;

            // --- Assert
            psg.Register0.ShouldBe((byte)0);
            psg.Register1.ShouldBe((byte)1);
            psg.Register2.ShouldBe((byte)2);
            psg.Register3.ShouldBe((byte)3);
            psg.Register4.ShouldBe((byte)4);
            psg.Register5.ShouldBe((byte)5);
            psg.Register6.ShouldBe((byte)6);
            psg.Register7.ShouldBe((byte)7);
            psg.Register8.ShouldBe((byte)8);
            psg.Register9.ShouldBe((byte)9);
            psg.Register10.ShouldBe((byte)10);
            psg.Register11.ShouldBe((byte)11);
            psg.Register12.ShouldBe((byte)12);
            psg.Register13.ShouldBe((byte)13);
            psg.Register14.ShouldBe((byte)14);
        }

        /// <summary>
        /// VM to test the beeper
        /// </summary>
        private class SpectrumSoundTestMachine : Spectrum128AdvancedTestMachine
        {
            public void SetCurrentCpuTact(long tacts)
            {
                (Cpu as IZ80CpuTestSupport)?.SetTacts(tacts);
            }
        }
    }
}
