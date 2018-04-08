using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu.Scripting;

namespace Spect.Net.SpectrumEmu.Test.Scripting
{
    [TestClass]
    public class SpectrumVmFactoryTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            SpectrumVmFactory.Reset();
            SpectrumVmFactory.RegisterDefaultProviders();
        }

        [TestMethod]
        public void CreateSpectrum48PalWorks()
        {
            // --- Act
            var sm = SpectrumVmFactory.CreateSpectrum48Pal();

            // --- Assert
            sm.ShouldNotBeNull();
            sm.ModelKey.ShouldBe(SpectrumModels.ZX_SPECTRUM_48);
            sm.EditionKey.ShouldBe(SpectrumModels.PAL);
        }

        [TestMethod]
        public void CreateSpectrum48NtscWorks()
        {
            // --- Act
            var sm = SpectrumVmFactory.CreateSpectrum48Ntsc();

            // --- Assert
            sm.ShouldNotBeNull();
            sm.ModelKey.ShouldBe(SpectrumModels.ZX_SPECTRUM_48);
            sm.EditionKey.ShouldBe(SpectrumModels.NTSC);
        }

        [TestMethod]
        public void CreateSpectrum48PalTurboWorks()
        {
            // --- Act
            var sm = SpectrumVmFactory.CreateSpectrum48PalTurbo();

            // --- Assert
            sm.ShouldNotBeNull();
            sm.ModelKey.ShouldBe(SpectrumModels.ZX_SPECTRUM_48);
            sm.EditionKey.ShouldBe(SpectrumModels.PAL_2_X);
        }

        [TestMethod]
        public void CreateSpectrum48NtscTurboWorks()
        {
            // --- Act
            var sm = SpectrumVmFactory.CreateSpectrum48NtscTurbo();

            // --- Assert
            sm.ShouldNotBeNull();
            sm.ModelKey.ShouldBe(SpectrumModels.ZX_SPECTRUM_48);
            sm.EditionKey.ShouldBe(SpectrumModels.NTSC_2_X);
        }

        [TestMethod]
        public void CreateSpectrum128Works()
        {
            // --- Act
            var sm = SpectrumVmFactory.CreateSpectrum128();

            // --- Assert
            sm.ShouldNotBeNull();
            sm.ModelKey.ShouldBe(SpectrumModels.ZX_SPECTRUM_128);
            sm.EditionKey.ShouldBe(SpectrumModels.PAL);
        }

        [TestMethod]
        public void CreateSpectrumP3EWorks()
        {
            // --- Act
            var sm = SpectrumVmFactory.CreateSpectrumP3E();

            // --- Assert
            sm.ShouldNotBeNull();
            sm.ModelKey.ShouldBe(SpectrumModels.ZX_SPECTRUM_P3_E);
            sm.EditionKey.ShouldBe(SpectrumModels.PAL);
        }
    }
}
