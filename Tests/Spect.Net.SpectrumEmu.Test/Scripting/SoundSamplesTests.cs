using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Scripting;

namespace Spect.Net.SpectrumEmu.Test.Scripting
{
    [TestClass]
    public class SoundSamplesTests
    {
        [TestMethod]
        public async Task BeeperSamplesCreatedForSpectrum48()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();

            // --- Act
            sm.RunUntilFrameCompletion();
            await sm.CompletionTask;

            // --- Assert
            sm.BeeperSamples.Count.ShouldBeGreaterThanOrEqualTo(sm.BeeperConfiguration.SamplesPerFrame);
            sm.BeeperSamples.Count.ShouldBeLessThanOrEqualTo(sm.BeeperConfiguration.SamplesPerFrame + 1);
        }

        [TestMethod]
        public async Task BeeperSamplesCreatedForSpectrum128()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum128();

            // --- Act
            sm.RunUntilFrameCompletion();
            await sm.CompletionTask;

            // --- Assert
            sm.BeeperSamples.Count.ShouldBeGreaterThanOrEqualTo(sm.BeeperConfiguration.SamplesPerFrame);
            sm.BeeperSamples.Count.ShouldBeLessThanOrEqualTo(sm.BeeperConfiguration.SamplesPerFrame + 1);
        }

        [TestMethod]
        public async Task SoundSamplesCreatedForSpectrum128()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum128();

            // --- Act
            sm.RunUntilFrameCompletion();
            await sm.CompletionTask;

            // --- Assert
            sm.SoundSamples.Count.ShouldBeGreaterThanOrEqualTo(sm.SoundConfiguration.SamplesPerFrame);
            sm.SoundSamples.Count.ShouldBeLessThanOrEqualTo(sm.SoundConfiguration.SamplesPerFrame + 1);
        }

        [TestMethod]
        public async Task FirstSoundFrameShouldBeSilent()
        {
            // --- Arrange
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();

            // --- Act
            sm.RunUntilFrameCompletion();
            await sm.CompletionTask;

            // --- Assert
            for (var i = 0; i < sm.BeeperSamples.Count; i++)
            {
                sm.BeeperSamples[i].ShouldBe(0.0f);
            }
        }
    }
}
