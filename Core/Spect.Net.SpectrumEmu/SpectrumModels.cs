using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Models;

namespace Spect.Net.SpectrumEmu
{
    /// <summary>
    /// This class is an inventory of available Spectrum models and
    /// revisions supported by SpectNetIde
    /// </summary>
    public static class SpectrumModels
    {
        private static readonly Dictionary<string, SpectrumModelEditions> s_StockModels = 
            new Dictionary<string, SpectrumModelEditions>
        {
            {
                ZX_SPECTRUM_48, new SpectrumModelEditions
                {
                    Editions = new Dictionary<string, SpectrumEdition>
                    {
                        {
                            PAL, new SpectrumEdition
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
        public static IReadOnlyDictionary<string, SpectrumModelEditions> StockModels => s_StockModels;

        /// <summary>
        /// Shortcut to access ZX Spectrum 48K model PAL Revision
        /// </summary>
        public static SpectrumEdition ZxSpectrum48Pal => 
            StockModels[ZX_SPECTRUM_48].Editions[PAL].Clone();
    }
}