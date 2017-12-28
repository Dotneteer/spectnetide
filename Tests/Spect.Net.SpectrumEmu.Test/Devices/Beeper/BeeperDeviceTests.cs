using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
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
            var beeperDevice = new BeeperDevice();

            // --- Act
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Assert
            beeperDevice.LastEarBit.ShouldBeTrue();
            beeperDevice.FrameCount.ShouldBe(0);
            beeperDevice.LastSampleTact.ShouldBe(0);
            beeperDevice.AudioSamples.Length.ShouldBe(699);
        }

        [TestMethod]
        // --- True value means no pulse change, only initial sample is created
        [DataRow(true, 88, new float[0])]
        [DataRow(true, 112, new float[0])]
        [DataRow(true, 299, new float[0])]
        [DataRow(true, 300, new float[0])]
        [DataRow(true, 301, new float[0])]
        // --- Single sample
        [DataRow(false, 88, new[] { 1.0f } )]
        [DataRow(false, 99, new[] { 1.0f })]
        [DataRow(false, 100, new[] { 1.0f })]
        // --- Multiple samples
        [DataRow(false, 101, new[] { 1.0f, 1.0f })]
        [DataRow(false, 246, new[] { 1.0f, 1.0f, 1.0f })]
        [DataRow(false, 299, new[] { 1.0f, 1.0f, 1.0f })]
        [DataRow(false, 300, new[] { 1.0f, 1.0f, 1.0f })]
        [DataRow(false, 301, new[] { 1.0f, 1.0f, 1.0f, 1.0f })]
        public void FirstPulseIsProcessedProperly(bool pulse, int tact, float[] samples)
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Act
            spectrum.SetCurrentCpuTact(tact);
            beeperDevice.ProcessEarBitValue(false, pulse);

            // --- Assert
            beeperDevice.LastSampleTact.ShouldBe(beeperDevice.HostVm.BeeperConfiguration.TactsPerSample*samples.Length);
            for (var i = 0; i < samples.Length; i++)
            {
                samples[i].ShouldBe(beeperDevice.AudioSamples[i]);
            }
        }

        [TestMethod]
        // --- Only initial sample is created
        [DataRow(new[] { 88 }, new [] { 1.0f })]
        [DataRow(new[] { 88, 98 }, new[] { 1.0f })]
        [DataRow(new[] { 88, 99 }, new[] { 1.0f })]
        [DataRow(new[] { 88, 100 }, new[] { 1.0f })]
        // --- Multiple samples are created
        [DataRow(new[] { 88, 101 }, new[] { 1.0f, 0.0f })]
        [DataRow(new[] { 112, 246 }, new[] { 1.0f, 1.0f, 0.0f })]
        [DataRow(new[] { 112, 300 }, new[] { 1.0f, 1.0f, 0.0f })]
        [DataRow(new[] { 112, 301 }, new[] { 1.0f, 1.0f, 0.0f, 0.0f })]
        [DataRow(new[] { 112, 334, 610 }, new[] { 1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f })]
        public void MultiplePulsesAreProcessedProperly(int[] tacts, float[] samples)
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);
            var initialBit = false;
            // --- Act
            foreach (var tact in tacts)
            {
                spectrum.SetCurrentCpuTact(tact);
                beeperDevice.ProcessEarBitValue(false, initialBit);
                initialBit = !initialBit;
            }

            // --- Assert
            beeperDevice.LastSampleTact.ShouldBe(beeperDevice.HostVm.BeeperConfiguration.TactsPerSample * samples.Length);
            for (var i = 0; i < samples.Length; i++)
            {
                samples[i].ShouldBe(beeperDevice.AudioSamples[i]);
            }
        }

        [TestMethod]
        public void OnFrameCompletedWorksWithNoPulseEndExactEnding()
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Act
            spectrum.SetCurrentCpuTact(69888);
            beeperDevice.OnFrameCompleted();

            // --- Assert
            beeperDevice.LastEarBit.ShouldBeTrue();
            beeperDevice.FrameCount.ShouldBe(0);
            beeperDevice.Overflow.ShouldBe(0);
            beeperDevice.AudioSamples.Length.ShouldBe(699);

            foreach (var sample in beeperDevice.AudioSamples)
            {
                sample.ShouldBe(1.0f);
            }
        }

        [TestMethod]
        public void OnFrameCompletedWorksWithNoPulseEndOverflow()
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Act
            spectrum.SetCurrentCpuTact(69888 + 11);
            beeperDevice.OnFrameCompleted();

            // --- Assert
            beeperDevice.LastEarBit.ShouldBeTrue();
            beeperDevice.FrameCount.ShouldBe(0);
            beeperDevice.Overflow.ShouldBe(11);
            beeperDevice.AudioSamples.Length.ShouldBe(699);

            foreach (var sample in beeperDevice.AudioSamples)
            {
                sample.ShouldBe(1.0f);
            }
        }

        [TestMethod]
        // --- Only initial sample is created
        [DataRow(new[] { 88 }, new[] { 1.0f })]
        [DataRow(new[] { 88, 98 }, new[] { 1.0f })]
        [DataRow(new[] { 88, 99 }, new[] { 1.0f })]
        [DataRow(new[] { 88, 100 }, new[] { 1.0f })]
        // --- Multiple samples are created
        [DataRow(new[] { 88, 101 }, new[] { 1.0f, 0.0f })]
        [DataRow(new[] { 112, 246 }, new[] { 1.0f, 1.0f, 0.0f })]
        [DataRow(new[] { 112, 300 }, new[] { 1.0f, 1.0f, 0.0f })]
        [DataRow(new[] { 112, 301 }, new[] { 1.0f, 1.0f, 0.0f, 0.0f })]
        [DataRow(new[] { 112, 334, 610 }, new[] { 1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f })]
        public void OnFrameCompletedWorksWithMultiplePulses(int[] tacts, float[] samples)
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);
            var initialBit = false;

            // --- Act
            foreach (var tact in tacts)
            {
                spectrum.SetCurrentCpuTact(tact);
                beeperDevice.ProcessEarBitValue(false, initialBit);
                initialBit = !initialBit;
            }
            spectrum.SetCurrentCpuTact(69888);
            beeperDevice.OnFrameCompleted();

            // --- Assert
            beeperDevice.LastEarBit.ShouldBe(!initialBit);
            beeperDevice.FrameCount.ShouldBe(0);
            beeperDevice.Overflow.ShouldBe(0);
            beeperDevice.LastSampleTact.ShouldBe(beeperDevice.HostVm.BeeperConfiguration.TactsPerSample * 699);
            for (var i = 0; i < samples.Length; i++)
            {
                samples[i].ShouldBe(beeperDevice.AudioSamples[i]);
            }
            var remainingSample = beeperDevice.LastEarBit ? 1.0f : 0.0f;
            for (var i = samples.Length; i < 699; i++)
            {
                beeperDevice.AudioSamples[i].ShouldBe(remainingSample);
            }
        }

        [TestMethod]
        // --- Only initial sample is created
        [DataRow(new[] { 88 }, new[] { 1.0f })]
        [DataRow(new[] { 88, 98 }, new[] { 1.0f })]
        [DataRow(new[] { 88, 99 }, new[] { 1.0f })]
        [DataRow(new[] { 88, 100 }, new[] { 1.0f })]
        // --- Multiple samples are created
        [DataRow(new[] { 88, 101 }, new[] { 1.0f, 0.0f })]
        [DataRow(new[] { 112, 246 }, new[] { 1.0f, 1.0f, 0.0f })]
        [DataRow(new[] { 112, 300 }, new[] { 1.0f, 1.0f, 0.0f })]
        [DataRow(new[] { 112, 301 }, new[] { 1.0f, 1.0f, 0.0f, 0.0f })]
        [DataRow(new[] { 112, 334, 610 }, new[] { 1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f })]
        public void OnFrameCompletedWorksWithMultiplePulsesAndOverflow(int[] tacts, float[] samples)
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);
            var initialBit = false;

            // --- Act
            foreach (var tact in tacts)
            {
                spectrum.SetCurrentCpuTact(tact);
                beeperDevice.ProcessEarBitValue(false, initialBit);
                initialBit = !initialBit;
            }
            spectrum.SetCurrentCpuTact(69888 + 11);
            beeperDevice.OnFrameCompleted();

            // --- Assert
            beeperDevice.LastEarBit.ShouldBe(!initialBit);
            beeperDevice.FrameCount.ShouldBe(0);
            beeperDevice.Overflow.ShouldBe(11);
            beeperDevice.LastSampleTact.ShouldBe(beeperDevice.HostVm.BeeperConfiguration.TactsPerSample * 699);
            for (var i = 0; i < samples.Length; i++)
            {
                samples[i].ShouldBe(beeperDevice.AudioSamples[i]);
            }
            var remainingSample = beeperDevice.LastEarBit ? 1.0f : 0.0f;
            for (var i = samples.Length; i < 699; i++)
            {
                beeperDevice.AudioSamples[i].ShouldBe(remainingSample);
            }
        }

        [TestMethod]
        [DataRow(1, new [] { 699 })]
        [DataRow(2, new[] { 699, 699 })]
        [DataRow(3, new[] { 699, 699, 699 })]
        [DataRow(4, new[] { 699, 699, 699, 699 })]
        [DataRow(5, new[] { 699, 699, 699, 699, 699 })]
        [DataRow(6, new[] { 699, 699, 699, 699, 699, 699 })]
        [DataRow(7, new[] { 699, 699, 699, 699, 699, 699, 699 })]
        [DataRow(8, new[] { 699, 699, 699, 699, 699, 699, 699, 699 })]
        // --- Here's a 698 at the end!
        [DataRow(9, new[] { 699, 699, 699, 699, 699, 699, 699, 699, 698 })]
        [DataRow(10, new[] { 699, 699, 699, 699, 699, 699, 699, 699, 698, 699 })]
        public void SampleLengthIsCalculatedProperly(int frames, int[] lenghts)
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);

            // --- Act/Assert
            for (var i = 1; i <= frames; i++)
            {
                spectrum.SetCurrentCpuTact(69888*i);
                beeperDevice.OnFrameCompleted();
                beeperDevice.AudioSamples.Length.ShouldBe(lenghts[i - 1]);
                beeperDevice.OnNewFrame();
            }
        }

        [TestMethod]
        // --- No sample when overflow is less than 13
        [DataRow(1, new[] { 88 }, null)]
        [DataRow(7, new[] { 88 }, null)]
        [DataRow(10, new[] { 88 }, null)]
        [DataRow(11, new[] { 88 }, null)]
        [DataRow(12, new[] { 88 }, null)]
        // --- Overflow occurs from 13
        [DataRow(13, new[] { 88 }, 0.0f)]
        [DataRow(14, new[] { 88 }, 0.0f)]
        [DataRow(23, new[] { 88 }, 0.0f)]

        // --- Multiple samples
        [DataRow(11, new[] { 88, 101 }, null)]
        [DataRow(13, new[] { 88, 101 }, 1.0f)]
        [DataRow(11, new[] { 112, 246 }, null)]
        [DataRow(15, new[] { 112, 300 }, 1.0f)]
        [DataRow(11, new[] { 112, 301 }, null)]
        [DataRow(17, new[] { 112, 334, 610 }, 0.0f)]
        public void OnNewFrameCreatesOverflowSample(int overflow, int[] tacts, float? sample)
        {
            // --- Arrange
            var spectrum = new SpectrumBeepTestMachine();
            var beeperDevice = new BeeperDevice();
            beeperDevice.OnAttachedToVm(spectrum);
            var initialBit = false;

            // --- Act
            foreach (var tact in tacts)
            {
                spectrum.SetCurrentCpuTact(tact);
                beeperDevice.ProcessEarBitValue(false, initialBit);
                initialBit = !initialBit;
            }
            spectrum.SetCurrentCpuTact(69888 + overflow);
            beeperDevice.OnFrameCompleted();
            var overflowBefore = beeperDevice.Overflow;
            beeperDevice.OnNewFrame();

            // --- Assert
            beeperDevice.SamplesIndex.ShouldBe(sample == null ? 0 : 1);
            overflowBefore.ShouldBe(overflow);
            beeperDevice.Overflow.ShouldBe(0);
            if (sample.HasValue)
            {
                beeperDevice.AudioSamples[0].ShouldBe(sample.Value);
            }
        }


        /// <summary>
        /// VM to test the beeper
        /// </summary>
        private class SpectrumBeepTestMachine : SpectrumAdvancedTestMachine
        {
            public void SetCurrentCpuTact(long tacts)
            {
                (Cpu as IZ80CpuTestSupport)?.SetTacts(tacts);
            }
        }

    }
}
