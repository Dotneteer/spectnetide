using Newtonsoft.Json;
using Spect.Net.SpectrumEmu;
using Spect.Net.VsPackage.CustomEditors;

namespace Spect.Net.VsPackage.SolutionItems
{
    /// <summary>
    /// Represents the view model of the Spectrum model inventory
    /// </summary>
    public class SpectrumProjectConfigSerializer
    {
        /// <summary>
        /// Serializes the configuration data of the specified view model
        /// </summary>
        /// <param name="vm">Spectrum configuration view model</param>
        /// <returns></returns>
        public static string Serialize(SpectrumConfEditorViewModel vm)
        {
            var data = new ProjectConfigData(vm.ModelName, vm.EditionName);
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
        public static bool Deserialize(string data, out SpectrumConfEditorViewModel vm)
        {
            try
            {
                var configData = JsonConvert.DeserializeObject<ProjectConfigData>(data);
                vm = new SpectrumConfEditorViewModel
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
                vm = new SpectrumConfEditorViewModel
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
        private class ProjectConfigData
        {
            public string Model { get; }
            public string Edition { get; }

            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public ProjectConfigData(string model, string edition)
            {
                Model = model;
                Edition = edition;
            }
        }
    }
}