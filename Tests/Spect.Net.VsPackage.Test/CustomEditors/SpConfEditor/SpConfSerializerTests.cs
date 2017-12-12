using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.SpectrumEmu;
using Spect.Net.VsPackage.CustomEditors.SpConfEditor;

namespace Spect.Net.VsPackage.Test.CustomEditors.SpConfEditor
{
    [TestClass]
    public class SpConfSerializerTests
    {
        [TestMethod]
        public void SerializationWorksAsExpected()
        {
            // --- Arrange
            var vm = new SpConfEditorViewModel
            {
                ModelName = SpectrumModels.ZX_SPECTRUM_48,
                EditionName = SpectrumModels.PAL
            };

            // --- Act
            var data = SpConfSerializer.Serialize(vm);
            Console.WriteLine(data);

            // --- Assert
            var success = SpConfSerializer.Deserialize(data, out var backVm);
            success.ShouldBeTrue();
            backVm.ModelName.ShouldBe(vm.ModelName);
            backVm.EditionName.ShouldBe(vm.EditionName);
        }

        [TestMethod]
        public void WrongSerializationDataGetBackZxSpectrum48K()
        {
            // --- Act
            var success = SpConfSerializer.Deserialize("wrong data", out var backVm);

            // --- Assert
            success.ShouldBeFalse();
            backVm.ModelName.ShouldBe(SpectrumModels.ZX_SPECTRUM_48);
            backVm.EditionName.ShouldBe(SpectrumModels.PAL);
            backVm.ConfigurationData.ShouldNotBeNull();
        }
    }
}
