using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Spectrum.Machine;

namespace Spect.Net.Spectrum.Test.Machine
{
    [TestClass]
    public class Spectrum48Tests
    {
        [TestMethod]
        public void SpectrumIsInitializedProperly()
        {
            // --- Act
            var spectrum = new Spectrum48();

            // --- Assert
            spectrum.Cpu.ShouldNotBeNull();
            spectrum.Clock.ShouldNotBeNull();
            spectrum.BorderDevice.ShouldNotBeNull();
            spectrum.ScreenDevice.ShouldNotBeNull();

        }
    }
}
