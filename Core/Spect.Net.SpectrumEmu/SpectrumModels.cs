using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
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
                                Cpu = new CpuConfigurationData
                                {
                                    BaseClockFrequency = 3_500_000,
                                    ClockMultiplier = 1,
                                    SupportsNextOperations = false
                                },
                                Rom = new RomConfigurationData
                                {
                                    RomName = "ZxSpectrum48",
                                    NumberOfRoms = 1,
                                    Spectrum48RomIndex = 0
                                },
                                Memory = new MemoryConfigurationData
                                {
                                    SupportsBanking = false,
                                    ContentionType = MemoryContentionType.Ula
                                },
                                Screen = new ScreenConfigurationData
                                {
                                    InterruptTact = 24,
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
                                },
                                Beeper = new AudioConfigurationData
                                {
                                    AudioSampleRate = 35000,
                                    SamplesPerFrame = 699,
                                    TactsPerSample = 100
                                }
                            }
                        },
                        {
                            NTSC, new SpectrumEdition
                            {
                                Cpu = new CpuConfigurationData
                                {
                                    BaseClockFrequency = 3_500_000,
                                    ClockMultiplier = 1,
                                    SupportsNextOperations = false
                                },
                                Rom = new RomConfigurationData
                                {
                                    RomName = "ZxSpectrum48",
                                    NumberOfRoms = 1,
                                    Spectrum48RomIndex = 0
                                },
                                Memory = new MemoryConfigurationData
                                {
                                    SupportsBanking = false,
                                    ContentionType = MemoryContentionType.Ula
                                },
                                Screen = new ScreenConfigurationData
                                {
                                    InterruptTact = 24,
                                    VerticalSyncLines = 8,
                                    NonVisibleBorderTopLines = 16,
                                    BorderTopLines = 24,
                                    BorderBottomLines = 24,
                                    NonVisibleBorderBottomLines = 0,
                                    DisplayLines = 192,
                                    BorderLeftTime = 24,
                                    BorderRightTime = 24,
                                    DisplayLineTime = 128,
                                    HorizontalBlankingTime = 40,
                                    NonVisibleBorderRightTime = 8,
                                    PixelDataPrefetchTime = 2,
                                    AttributeDataPrefetchTime = 1
                                },
                                Beeper = new AudioConfigurationData
                                {
                                    AudioSampleRate = 35000,
                                    SamplesPerFrame = 591,
                                    TactsPerSample = 100
                                }
                            }
                        },
                        {
                            PAL_2_X, new SpectrumEdition
                            {
                                Cpu = new CpuConfigurationData
                                {
                                    BaseClockFrequency = 3_500_000,
                                    ClockMultiplier = 2,
                                    SupportsNextOperations = false
                                },
                                Rom = new RomConfigurationData
                                {
                                    RomName = "ZxSpectrum48",
                                    NumberOfRoms = 1,
                                    Spectrum48RomIndex = 0
                                },
                                Memory = new MemoryConfigurationData
                                {
                                    SupportsBanking = false,
                                    ContentionType = MemoryContentionType.Ula
                                },
                                Screen = new ScreenConfigurationData
                                {
                                    InterruptTact = 24,
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
                                },
                                Beeper = new AudioConfigurationData
                                {
                                    AudioSampleRate = 35000,
                                    SamplesPerFrame = 699,
                                    TactsPerSample = 100
                                }
                            }
                        },
                        {
                            NTSC_2_X, new SpectrumEdition
                            {
                                Cpu = new CpuConfigurationData
                                {
                                    BaseClockFrequency = 3_500_000,
                                    ClockMultiplier = 2,
                                    SupportsNextOperations = false
                                },
                                Rom = new RomConfigurationData
                                {
                                    RomName = "ZxSpectrum48",
                                    NumberOfRoms = 1,
                                    Spectrum48RomIndex = 0
                                },
                                Memory = new MemoryConfigurationData
                                {
                                    SupportsBanking = false,
                                    ContentionType = MemoryContentionType.Ula
                                },
                                Screen = new ScreenConfigurationData
                                {
                                    InterruptTact = 24,
                                    VerticalSyncLines = 8,
                                    NonVisibleBorderTopLines = 16,
                                    BorderTopLines = 24,
                                    BorderBottomLines = 24,
                                    NonVisibleBorderBottomLines = 0,
                                    DisplayLines = 192,
                                    BorderLeftTime = 24,
                                    BorderRightTime = 24,
                                    DisplayLineTime = 128,
                                    HorizontalBlankingTime = 40,
                                    NonVisibleBorderRightTime = 8,
                                    PixelDataPrefetchTime = 2,
                                    AttributeDataPrefetchTime = 1
                                },
                                Beeper = new AudioConfigurationData
                                {
                                    AudioSampleRate = 35000,
                                    SamplesPerFrame = 699,
                                    TactsPerSample = 100
                                }
                            }
                        }
                    }
                }
            },
            {
                ZX_SPECTRUM_128, new SpectrumModelEditions
                {
                    Editions = new Dictionary<string, SpectrumEdition>
                    {
                        {
                            PAL, new SpectrumEdition
                            {
                                Cpu = new CpuConfigurationData
                                {
                                    BaseClockFrequency = 3_546_900,
                                    ClockMultiplier = 1,
                                    SupportsNextOperations = false
                                },
                                Rom = new RomConfigurationData
                                {
                                    RomName = "ZxSpectrum128",
                                    NumberOfRoms = 2,
                                    Spectrum48RomIndex = 1
                                },
                                Memory = new MemoryConfigurationData
                                {
                                    SupportsBanking = true,
                                    RamBanks = 8,
                                    ContentionType = MemoryContentionType.Ula
                                },
                                Screen = new ScreenConfigurationData
                                {
                                    InterruptTact = 26,
                                    VerticalSyncLines = 8,
                                    NonVisibleBorderTopLines = 7,
                                    BorderTopLines = 48,
                                    BorderBottomLines = 48,
                                    NonVisibleBorderBottomLines = 8,
                                    DisplayLines = 192,
                                    BorderLeftTime = 24,
                                    BorderRightTime = 24,
                                    DisplayLineTime = 128,
                                    HorizontalBlankingTime = 40,
                                    NonVisibleBorderRightTime = 12,
                                    PixelDataPrefetchTime = 2,
                                    AttributeDataPrefetchTime = 1
                                },
                                Beeper = new AudioConfigurationData
                                {
                                    AudioSampleRate = 35469,
                                    SamplesPerFrame = 709,
                                    TactsPerSample = 100
                                },
                                Sound = new AudioConfigurationData
                                {
                                    AudioSampleRate = 27710,
                                    SamplesPerFrame = 553,
                                    TactsPerSample = 128
                                }
                            }
                        }
                    }
                }
            },
            {
                ZX_SPECTRUM_P3_E, new SpectrumModelEditions
                {
                    Editions = new Dictionary<string, SpectrumEdition>
                    {
                        {
                            PAL, new SpectrumEdition
                            {
                                Cpu = new CpuConfigurationData
                                {
                                    BaseClockFrequency = 3_546_900,
                                    ClockMultiplier = 1,
                                    SupportsNextOperations = false
                                },
                                Rom = new RomConfigurationData
                                {
                                    RomName = "ZxSpectrumP3E",
                                    NumberOfRoms = 4,
                                    Spectrum48RomIndex = 3
                                },
                                Memory = new MemoryConfigurationData
                                {
                                    SupportsBanking = true,
                                    RamBanks = 8,
                                    ContentionType = MemoryContentionType.GateArray
                                },
                                Screen = new ScreenConfigurationData
                                {
                                    InterruptTact = 26,
                                    VerticalSyncLines = 8,
                                    NonVisibleBorderTopLines = 7,
                                    BorderTopLines = 48,
                                    BorderBottomLines = 48,
                                    NonVisibleBorderBottomLines = 8,
                                    DisplayLines = 192,
                                    BorderLeftTime = 24,
                                    BorderRightTime = 24,
                                    DisplayLineTime = 128,
                                    HorizontalBlankingTime = 40,
                                    NonVisibleBorderRightTime = 12,
                                    PixelDataPrefetchTime = 2,
                                    AttributeDataPrefetchTime = 1
                                },
                                Beeper = new AudioConfigurationData
                                {
                                    AudioSampleRate = 35469,
                                    SamplesPerFrame = 709,
                                    TactsPerSample = 100
                                },
                                Sound = new AudioConfigurationData
                                {
                                    AudioSampleRate = 27710,
                                    SamplesPerFrame = 553,
                                    TactsPerSample = 128
                                }
                            }
                        }
                    }
                }
            },
            {
                ZX_SPECTRUM_NEXT, new SpectrumModelEditions
                {
                    Editions = new Dictionary<string, SpectrumEdition>
                    {
                        {
                            PAL, new SpectrumEdition
                            {
                                Cpu = new CpuConfigurationData
                                {
                                    BaseClockFrequency = 3_546_900,
                                    ClockMultiplier = 1,
                                    SupportsNextOperations = true
                                },
                                Rom = new RomConfigurationData
                                {
                                    RomName = "ZxSpectrumNext",
                                    NumberOfRoms = 4,
                                    Spectrum48RomIndex = 3
                                },
                                Memory = new MemoryConfigurationData
                                {
                                    SupportsBanking = true,
                                    RamBanks = 8,
                                    ContentionType = MemoryContentionType.GateArray
                                },
                                Screen = new ScreenConfigurationData
                                {
                                    InterruptTact = 26,
                                    VerticalSyncLines = 8,
                                    NonVisibleBorderTopLines = 7,
                                    BorderTopLines = 48,
                                    BorderBottomLines = 48,
                                    NonVisibleBorderBottomLines = 8,
                                    DisplayLines = 192,
                                    BorderLeftTime = 24,
                                    BorderRightTime = 24,
                                    DisplayLineTime = 128,
                                    HorizontalBlankingTime = 40,
                                    NonVisibleBorderRightTime = 12,
                                    PixelDataPrefetchTime = 2,
                                    AttributeDataPrefetchTime = 1
                                },
                                Beeper = new AudioConfigurationData
                                {
                                    AudioSampleRate = 35469,
                                    SamplesPerFrame = 709,
                                    TactsPerSample = 100
                                },
                                Sound = new AudioConfigurationData
                                {
                                    AudioSampleRate = 27710,
                                    SamplesPerFrame = 553,
                                    TactsPerSample = 128
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
        /// Key for ZX Spectrum 128K
        /// </summary>
        public const string ZX_SPECTRUM_128 = "ZX Spectrum 128K";

        /// <summary>
        /// Key for ZX Spectrum +3E
        /// </summary>
        public const string ZX_SPECTRUM_P3_E = "ZX Spectrum +3E";

        /// <summary>
        /// Key for ZX Spectrum Next
        /// </summary>
        public const string ZX_SPECTRUM_NEXT = "ZX Spectrum Next";

        /// <summary>
        /// Key for PAL revisions
        /// </summary>
        public const string PAL = "PAL";

        /// <summary>
        /// Key for NTSC revisions
        /// </summary>
        public const string NTSC = "NTSC";

        /// <summary>
        /// Key for PAL turbo revisions
        /// </summary>
        public const string PAL_2_X = "PAL2X";

        /// <summary>
        /// Key for NTSC revisions
        /// </summary>
        public const string NTSC_2_X = "NTSC2X";

        /// <summary>
        /// The Spectrum models available 
        /// </summary>
        public static IReadOnlyDictionary<string, SpectrumModelEditions> StockModels => s_StockModels;

        /// <summary>
        /// Shortcut to access ZX Spectrum 48K model PAL Revision
        /// </summary>
        public static SpectrumEdition ZxSpectrum48Pal => 
            StockModels[ZX_SPECTRUM_48].Editions[PAL].Clone();

        /// <summary>
        /// Shortcut to access ZX Spectrum 48K model PAL Revision
        /// </summary>
        public static SpectrumEdition ZxSpectrum48Ntsc =>
            StockModels[ZX_SPECTRUM_48].Editions[NTSC].Clone();

        /// <summary>
        /// Shortcut to access ZX Spectrum 48K model PAL Revision
        /// </summary>
        public static SpectrumEdition ZxSpectrum48Pal2X =>
            StockModels[ZX_SPECTRUM_48].Editions[PAL_2_X].Clone();

        /// <summary>
        /// Shortcut to access ZX Spectrum 48K model PAL Revision
        /// </summary>
        public static SpectrumEdition ZxSpectrum48Ntsc2X =>
            StockModels[ZX_SPECTRUM_48].Editions[NTSC_2_X].Clone();

        /// <summary>
        /// Shortcut to access ZX Spectrum 128K model PAL Revision
        /// </summary>
        public static SpectrumEdition ZxSpectrum128Pal =>
            StockModels[ZX_SPECTRUM_128].Editions[PAL].Clone();

        /// <summary>
        /// Shortcut to access ZX Spectrum +3E model PAL Revision
        /// </summary>
        public static SpectrumEdition ZxSpectrumP3EPal =>
            StockModels[ZX_SPECTRUM_P3_E].Editions[PAL].Clone();

        /// <summary>
        /// Shortcut to access ZX Spectrum Next model PAL Revision
        /// </summary>
        public static SpectrumEdition ZxSpectrumNextEPal =>
            StockModels[ZX_SPECTRUM_NEXT].Editions[PAL].Clone();

    }
}