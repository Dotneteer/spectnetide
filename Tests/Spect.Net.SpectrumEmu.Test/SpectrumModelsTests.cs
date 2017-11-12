using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Spect.Net.SpectrumEmu.Test
{
    [TestClass]
    public class SpectrumModelsTests
    {
        [TestMethod]
        public void SerializationWorksAsExpected()
        {
            // --- Arrange
            var serializer = new SpectrumModels.Serializer(
                SpectrumModels.ZX_SPECTRUM_48,
                SpectrumModels.PAL);

            // --- Act
            var data = serializer.Serialize();
            Console.WriteLine(data);

            // --- Assert
            var back = SpectrumModels.Serializer.Deserialize(data);
            back.ModelName.ShouldBe(serializer.ModelName);
            back.RevisionName.ShouldBe(serializer.RevisionName);
            back.Models.Count.ShouldBe(serializer.Models.Count);
        }
    }
}
