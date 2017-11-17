using Newtonsoft.Json;
using Spect.Net.SpectrumEmu;

namespace Spect.Net.VsPackage.CustomEditors.SpConfEditor
{
    /// <summary>
    /// This class provides serialization functions to obtain .spconf
    /// information
    /// </summary>
    public static class SpConfSerializer
    {
        /// <summary>
        /// Serializes the configuration data of the specified view model
        /// </summary>
        /// <param name="vm">Spectrum configuration view model</param>
        /// <returns></returns>
        public static string Serialize(SpConfEditorViewModel vm)
        {
            var data = new SpConfData(vm.ModelName, vm.EditionName);
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        /// <summary>
        /// Deserializes the specified string data into the configuration view
        /// model information
        /// </summary>
        /// <param name="data">String data to deserialize</param>
        /// <param name="vm">Deserialized view model</param>
        /// <returns>
        /// True, if deserialization is successful; otherwise, false
        /// </returns>
        /// <remarks>
        /// If deserialization fails, the method retrieves the ZX Spectrum 48K PAL
        /// edition configuration data
        /// </remarks>
        public static bool Deserialize(string data, out SpConfEditorViewModel vm)
        {
            try
            {
                var configData = JsonConvert.DeserializeObject<SpConfData>(data);
                vm = new SpConfEditorViewModel
                {
                    ModelName = configData.Model,
                    EditionName = configData.Edition,
                    ConfigurationData = SpectrumModels.StockModels[configData.Model]
                        .Editions[configData.Edition].Clone()
                };
                return true;
            }
            catch
            {
                // --- In case of issues, we retrieve the ZX Spectrum 48 PAL model
                // --- as the default one.
                vm = new SpConfEditorViewModel
                {
                    ModelName = SpectrumModels.ZX_SPECTRUM_48,
                    EditionName = SpectrumModels.PAL,
                    ConfigurationData = SpectrumModels.ZxSpectrum48Pal
                };
                return false;
            }
        }

        /// <summary>
        /// Data class with the serialization data
        /// </summary>
        private class SpConfData
        {
            public string Model { get; }
            public string Edition { get; }

            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public SpConfData(string model, string edition)
            {
                Model = model;
                Edition = edition;
            }
        }
    }
}