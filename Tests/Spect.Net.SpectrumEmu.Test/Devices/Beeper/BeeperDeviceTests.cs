using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.SpectrumEmu.Test.Helpers;

namespace Spect.Net.SpectrumEmu.Test.Devices.Beeper
{
    [TestClass]
    public class BeeperDeviceTests
    {
        [TestMethod]
        public void DeviceIsInitializedProperly()
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();

            // --- Act
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Assert
            beeperDevice.Pulses.Count.ShouldBe(0);
            beeperDevice.LastEarBit.ShouldBeTrue();
            beeperDevice.LastPulseTact.ShouldBe(0);
            beeperDevice.FrameCount.ShouldBe(0);
        }

        [TestMethod]
        public void ProcessEarBitWorksForTheFirstHighPulse()
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Act
            beeperDevice.ProcessEarBitValue(false, true);

            // --- Assert
            beeperDevice.Pulses.Count.ShouldBe(0);
            beeperDevice.LastEarBit.ShouldBeTrue();
            beeperDevice.LastPulseTact.ShouldBe(0);
        }

        [TestMethod]
        public void ProcessEarBitWorksForTheFirstLowPulse()
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Act
            spectrum.SetCurrentFrameTact(100);
            beeperDevice.ProcessEarBitValue(false, false);

            // --- Assert
            beeperDevice.Pulses.Count.ShouldBe(1);
            beeperDevice.LastEarBit.ShouldBeFalse();
            beeperDevice.LastPulseTact.ShouldBe(100);
            var pulse = beeperDevice.Pulses[0];
            pulse.EarBit.ShouldBeTrue();
            pulse.Lenght.ShouldBe(100);
        }

        [TestMethod]
        public void ProcessEarBitWorksWithLowPulseAtTact0()
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Act
            spectrum.SetCurrentFrameTact(0);
            beeperDevice.ProcessEarBitValue(false, false);

            // --- Assert
            beeperDevice.Pulses.Count.ShouldBe(0);
            beeperDevice.LastEarBit.ShouldBeFalse();
            beeperDevice.LastPulseTact.ShouldBe(0);
        }

        [TestMethod]
        public void ProcessEarBitWorksForFourPulses()
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Act
            spectrum.SetCurrentFrameTact(100);
            beeperDevice.ProcessEarBitValue(false, false);
            spectrum.SetCurrentFrameTact(140);
            beeperDevice.ProcessEarBitValue(false, true);
            spectrum.SetCurrentFrameTact(160);
            beeperDevice.ProcessEarBitValue(false, true);
            spectrum.SetCurrentFrameTact(190);
            beeperDevice.ProcessEarBitValue(false, false);


            // --- Assert
            beeperDevice.Pulses.Count.ShouldBe(3);
            beeperDevice.LastEarBit.ShouldBeFalse();
            beeperDevice.LastPulseTact.ShouldBe(190);
            var pulse1 = beeperDevice.Pulses[0];
            pulse1.EarBit.ShouldBeTrue();
            pulse1.Lenght.ShouldBe(100);
            var pulse2 = beeperDevice.Pulses[1];
            pulse2.EarBit.ShouldBeFalse();
            pulse2.Lenght.ShouldBe(40);
            var pulse3 = beeperDevice.Pulses[2];
            pulse3.EarBit.ShouldBeTrue();
            pulse3.Lenght.ShouldBe(50);
        }

        [TestMethod]
        public void OnFrameCompletedWorksWithNoPulse()
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Act
            beeperDevice.OnFrameCompleted();

            // --- Assert
            beeperDevice.FrameCount.ShouldBe(0);
            beeperDevice.Pulses.Count.ShouldBe(1);
            beeperDevice.LastEarBit.ShouldBeTrue();
            beeperDevice.LastPulseTact.ShouldBe(0);
        }

        [TestMethod]
        public void OnFrameCompletedWorksWithHighPulse()
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Act
            beeperDevice.ProcessEarBitValue(false, true);
            beeperDevice.OnFrameCompleted();

            // --- Assert
            beeperDevice.FrameCount.ShouldBe(0);
            beeperDevice.Pulses.Count.ShouldBe(1);
            beeperDevice.LastEarBit.ShouldBeTrue();
            beeperDevice.LastPulseTact.ShouldBe(0);
        }

        [TestMethod]
        public void OnFrameCompletedWorksWithLowPulseAtTact0()
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Act
            spectrum.SetCurrentFrameTact(0);
            beeperDevice.ProcessEarBitValue(false, false);
            spectrum.SetCurrentFrameTact(spectrum.FrameTacts);
            beeperDevice.OnFrameCompleted();

            // --- Assert
            beeperDevice.FrameCount.ShouldBe(0);
            beeperDevice.Pulses.Count.ShouldBe(1);
            beeperDevice.LastEarBit.ShouldBeFalse();
            beeperDevice.LastPulseTact.ShouldBe(0);
            var pulse = beeperDevice.Pulses[0];
            pulse.EarBit.ShouldBeFalse();
            pulse.Lenght.ShouldBe(spectrum.FrameTacts);
        }

        [TestMethod]
        public void OnFrameCompletedWorksWithLowPulseAfterTheFirsttact()
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Act
            spectrum.SetCurrentFrameTact(100);
            beeperDevice.ProcessEarBitValue(false, false);
            spectrum.SetCurrentFrameTact(spectrum.FrameTacts);
            beeperDevice.OnFrameCompleted();

            // --- Assert
            beeperDevice.FrameCount.ShouldBe(0);
            beeperDevice.Pulses.Count.ShouldBe(2);
            beeperDevice.LastEarBit.ShouldBeFalse();
            beeperDevice.LastPulseTact.ShouldBe(100);
            var pulse1 = beeperDevice.Pulses[0];
            pulse1.EarBit.ShouldBeTrue();
            pulse1.Lenght.ShouldBe(100);
            var pulse2 = beeperDevice.Pulses[1];
            pulse2.EarBit.ShouldBeFalse();
            pulse2.Lenght.ShouldBe(spectrum.FrameTacts - 100);
        }

        [TestMethod]
        public void OnNewFrameInitsNextFrame()
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Act
            beeperDevice.OnNewFrame();

            // --- Assert
            beeperDevice.FrameCount.ShouldBe(1);
            beeperDevice.Pulses.Count.ShouldBe(0);
            beeperDevice.LastEarBit.ShouldBeTrue();
            beeperDevice.LastPulseTact.ShouldBe(0);
        }

        [TestMethod]
        public void OnNewFrameKeepsLastEarBitValue()
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Act
            spectrum.SetCurrentFrameTact(100);
            beeperDevice.ProcessEarBitValue(false, false);
            spectrum.SetCurrentFrameTact(spectrum.FrameTacts);
            beeperDevice.OnFrameCompleted();
            beeperDevice.OnNewFrame();

            // --- Assert
            beeperDevice.FrameCount.ShouldBe(1);
            beeperDevice.Pulses.Count.ShouldBe(0);
            beeperDevice.LastEarBit.ShouldBeFalse();
            beeperDevice.LastPulseTact.ShouldBe(0);
        }

        private class SpectrumBeepTestMachine : SpectrumAdvancedTestMachine
        {
            private int _tact;

            /// <summary>
            /// Override this property for mocking its value
            /// </summary>
            public override int CurrentFrameTact => _tact;

            public void SetCurrentFrameTact(int tact)
            {
                _tact = tact;
            }
        }
    }
}
