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
        public void DeviceIsInitializedProperty()
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
            beeperDevice.ProcessEarBitValue(true);

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
            beeperDevice.ProcessEarBitValue(false);

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
            beeperDevice.ProcessEarBitValue(false);

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
            beeperDevice.ProcessEarBitValue(false);
            spectrum.SetCurrentFrameTact(140);
            beeperDevice.ProcessEarBitValue(true);
            spectrum.SetCurrentFrameTact(160);
            beeperDevice.ProcessEarBitValue(true);
            spectrum.SetCurrentFrameTact(190);
            beeperDevice.ProcessEarBitValue(false);


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
            beeperDevice.Pulses.Count.ShouldBe(0);
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
            beeperDevice.ProcessEarBitValue(true);
            beeperDevice.OnFrameCompleted();

            // --- Assert
            beeperDevice.FrameCount.ShouldBe(0);
            beeperDevice.Pulses.Count.ShouldBe(0);
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
            beeperDevice.ProcessEarBitValue(false);
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
            beeperDevice.ProcessEarBitValue(false);
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
            beeperDevice.ProcessEarBitValue(false);
            spectrum.SetCurrentFrameTact(spectrum.FrameTacts);
            beeperDevice.OnFrameCompleted();
            beeperDevice.OnNewFrame();

            // --- Assert
            beeperDevice.FrameCount.ShouldBe(1);
            beeperDevice.Pulses.Count.ShouldBe(0);
            beeperDevice.LastEarBit.ShouldBeFalse();
            beeperDevice.LastPulseTact.ShouldBe(0);
        }

        //[TestMethod]
        //public void RenderFloatWorksWithASingleLowPulse()
        //{
        //    // --- Arrange
        //    var spectrum = new SpectrumBeepTestMachine();
        //    var beeperDevice = new BeeperDevice();
        //    beeperDevice.OnAttachedToVm(spectrum);
        //    beeperDevice.OnNewFrame();
        //    spectrum.SetCurrentFrameTact(0);
        //    beeperDevice.ProcessEarBitValue(false);
        //    spectrum.SetCurrentFrameTact(68);
        //    beeperDevice.ProcessEarBitValue(true);
        //    beeperDevice.OnFrameCompleted();

        //    var buffer = new float[spectrum.BeeperDevice.BeeperConfiguration.SamplesPerFrame];

        //    // --- Act
        //    BeeperDevice.RenderFloat(beeperDevice.Pulses, spectrum.BeeperDevice.BeeperConfiguration, buffer, 0);

        //    // --- Assert
        //    buffer[0].ShouldBe(0F);
        //    for (var i = 1; i < buffer.Length; i++)
        //    {
        //        buffer[i].ShouldBe(1F);
        //    }
        //}

        //[TestMethod]
        //public void RenderFloatWorksWithAletrnatingPulses()
        //{
        //    // --- Arrange
        //    var spectrum = new SpectrumBeepTestMachine();
        //    var beeperDevice = new BeeperDevice();
        //    beeperDevice.OnAttachedToVm(spectrum);
        //    beeperDevice.OnNewFrame();
        //    var earbit = false;
        //    for (var i = 3; i < spectrum.FrameTacts; i += spectrum.BeeperDevice.BeeperConfiguration.TactsPerSample)
        //    {
        //        spectrum.SetCurrentFrameTact(i);
        //        beeperDevice.ProcessEarBitValue(earbit);
        //        earbit = !earbit;
        //    }
        //    beeperDevice.OnFrameCompleted();

        //    var buffer = new float[spectrum.BeeperDevice.BeeperConfiguration.SamplesPerFrame];

        //    // --- Act
        //    BeeperDevice.RenderFloat(beeperDevice.Pulses, spectrum.BeeperDevice.BeeperConfiguration, buffer, 0);

        //    // --- Assert
        //    for (var i = 0; i < buffer.Length; i++)
        //    {
        //        buffer[i].ShouldBe(i % 2 == 0 ? 0F : 1F);
        //    }
        //}

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
