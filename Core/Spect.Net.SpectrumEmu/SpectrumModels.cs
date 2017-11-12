using System.Collections.Generic;
using Newtonsoft.Json;
using Spect.Net.SpectrumEmu.Abstraction.Models;

namespace Spect.Net.SpectrumEmu
{
    /// <summary>
    /// This class is an inventory of available Spectrum models and
    /// revisions supported by SpectNetIde
    /// </summary>
    public static class SpectrumModels
    {
        private static readonly Dictionary<string, SpectrumModelRevisions> s_StockModels = 
            new Dictionary<string, SpectrumModelRevisions>
        {
            {
                ZX_SPECTRUM_48, new SpectrumModelRevisions
                {
                    Revisions = new Dictionary<string, SpectrumRevision>
                    {
                        {
                            PAL, new SpectrumRevision
                            {
                                NumberOfRoms = 1,
                                RomSize = 0x4000,
                                RamSize = 0xC000,
                                ScreenConfiguration = new ScreenConfigurationData
                                {
                                    RefreshRate = 50,
                                    FlashToggleFrames = 25,
                                    VerticalSyncLines = 8,
                                    NonVisibleBorderTopLines = 8,
                                    BorderTopLines = 48,
                                    BorderBottomLines = 48,
                                    NonVisibleBorderBottomLines = 8,
                                    DisplayLines = 192,
                                    BorderLeftTime = 24,
                                    BorderRightTime = 24,
                                    DisplayLineTime = 128,
                                    HorizontalBlankingTime = 40,
                                    NonVisibleBorderRightTime = 8,
                                    PixelDataPrefetchTime = 2,
                                    AttributeDataPrefetchTime = 1
                                }
                            }
                        }
                    }
                }
            }
        };

        /// <summary>
        /// Key for ZX Spectrum 48K
        /// </summary>
        public const string ZX_SPECTRUM_48 = "ZX Spectrum 48K";

        /// <summary>
        /// Key for PAL revisions
        /// </summary>
        public const string PAL = "PAL";

        /// <summary>
        /// The Spectrum models available 
        /// </summary>
        public static IReadOnlyDictionary<string, SpectrumModelRevisions> StockModels => s_StockModels;

        /// <summary>
        /// Shortcut to access ZX Spectrum 48K model PAL Revision
        /// </summary>
        public static SpectrumRevision ZxSpectrum48PalRev => 
            StockModels[ZX_SPECTRUM_48].Revisions[PAL];

        /// <summary>
        /// This class cen be used to serialize and deserialize models
        /// </summary>
        public class Serializer
        {
            /// <summary>
            /// The name of the current model
            /// </summary>
            public string ModelName { get; set; }

            /// <summary>
            /// The current revision name
            /// </summary>
            public string RevisionName { get; set; }

            /// <summary>
            /// The inventory of models
            /// </summary>
            public Dictionary<string, SpectrumModelRevisions> Models { get; set; }

            public Serializer(string model, string revision)
            {
                ModelName = model;
                RevisionName = revision;
                Models = s_StockModels;
            }

            /// <summary>
            /// Serialize the data into a string
            /// </summary>
            /// <returns></returns>
            public string Serialize()
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented);
            }

            /// <summary>
            /// Deserialize the string into data
            /// </summary>
            /// <param name="inut">Input string</param>
            /// <returns>Deserialzied data</returns>
            public static Serializer Deserialize(string inut)
            {
                return JsonConvert.DeserializeObject<Serializer>(inut);
            }
        }
    }
}