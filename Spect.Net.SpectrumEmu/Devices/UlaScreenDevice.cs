using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Spect.Net.SpectrumEmu.Machine;

namespace Spect.Net.SpectrumEmu.Devices
{
    /// <summary>
    /// This class is responsible to render a single frame of the screen
    /// </summary>
    public class UlaScreenDevice : IFrameBoundDevice
    {
        private readonly uint[] _spectrumColors =
        {
            0xFF000000,
            0xFF0000AA,
            0xFFAA0000,
            0xFFAA00AA,
            0xFF00AA00,
            0xFF00AAAA,
            0xFFAAAA00,
            0xFFAAAAAA,
            0xFF000000,
            0xFF0000FF,
            0xFFFF0000,
            0xFFFF00FF,
            0xFF00FF00,
            0xFF00FFFF,
            0xFFFFFF00,
            0xFFFFFFFF,
        };

        /// <summary>
        /// The device that handles the border color
        /// </summary>
        private readonly UlaBorderDevice _borderDevice;

        /// <summary>
        /// Defines the action that accesses the screen memory
        /// </summary>
        private readonly Func<ushort, byte> _fetchScreenMemory;

        /// <summary>
        /// The devices that physically renders the screen
        /// </summary>
        private readonly IScreenPixelRenderer _pixelRenderer;

        /// <summary>
        /// Table of ULA tact action information entries
        /// </summary>
        private UlaTact[] _ulaTactTable;

        /// <summary>
        /// The current flash phase (normal/invert)
        /// </summary>
        private bool _flashPhase;

        /// <summary>
        /// Pixel and attribute info stored while rendering the screen
        /// </summary>
        private byte _pixelByte1;
        private byte _pixelByte2;
        private byte _attrByte1;
        private byte _attrByte2;

        /// <summary>
        /// Gets the current frame count
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// The ZX Spectrum color palette
        /// </summary>
        public IReadOnlyList<uint> SpectrumColors { get; }

        public DisplayParameters DisplayParameters { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class 
        /// using the specified display parameters
        /// </summary>
        /// <param name="hostVm">Host Spectrum VM</param>
        /// <param name="pixelRenderer">Object that renders the screen pixels</param>
        /// <param name="borderDevice">The border device to use when rendering the screen</param>
        /// <param name="fetchFunction">The function to fetch screen memory values</param>
        /// "/>
        public UlaScreenDevice(Spectrum48 hostVm, 
            IScreenPixelRenderer pixelRenderer,
            UlaBorderDevice borderDevice = null,
            Func<ushort, byte> fetchFunction = null)
        {
            DisplayParameters = hostVm.DisplayPars;
            _borderDevice = borderDevice ?? hostVm.BorderDevice;
            _fetchScreenMemory = fetchFunction ?? hostVm.UlaReadMemory;
            InitializeUlaTactTable();
            _flashPhase = false;
            FrameCount = 0;
            _pixelRenderer = pixelRenderer;
            _pixelRenderer?.SetPalette(_spectrumColors);
            SpectrumColors = new ReadOnlyCollection<uint>(_spectrumColors);
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            _flashPhase = false;
            _pixelRenderer?.Reset();
            FrameCount = 0;
        }

        /// <summary>
        /// Starts rendering a new frame from the first tact
        /// </summary>
        public void StartNewFrame()
        {
            FrameCount++;
            if (FrameCount%DisplayParameters.FlashToggleFrames == 0)
            {
                _flashPhase = !_flashPhase;
            }
            _pixelRenderer?.StartNewFrame();
        }

        /// <summary>
        /// Executes the ULA rendering actions between the specified tacts
        /// </summary>
        /// <param name="fromTact">First ULA tact</param>
        /// <param name="toTact">Last ULA tact</param>
        public void RenderScreen(int fromTact, int toTact)
        {
            // --- Adjust the tact boundaries
            if (fromTact < 0) fromTact = 0;
            fromTact = fromTact % DisplayParameters.UlaFrameTactCount;
            toTact = toTact % DisplayParameters.UlaFrameTactCount;

            // --- Carry out each tact action according to the rendering phase
            for (var currentTact = fromTact; currentTact <= toTact; currentTact++)
            {
                var ulaTact = _ulaTactTable[currentTact];
                switch (ulaTact.Phase)
                {
                    case UlaRenderingPhase.None:
                        // --- Invisible screen area, nothing to do
                        break;

                    case UlaRenderingPhase.Border:
                        // --- Fetch the border color from ULA and set the corresponding border pixels
                        SetPixels(ref ulaTact, _borderDevice.BorderColor, _borderDevice.BorderColor);
                        break;

                    case UlaRenderingPhase.BorderAndFetchPixelByte:
                        // --- Fetch the border color from ULA and set the corresponding border pixels
                        SetPixels(ref ulaTact, _borderDevice.BorderColor, _borderDevice.BorderColor);
                        // --- Obtain the future pixel byte
                        _pixelByte1 = _fetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case UlaRenderingPhase.BorderAndFetchPixelAttribute:
                        // --- Fetch the border color from ULA and set the corresponding border pixels
                        SetPixels(ref ulaTact, _borderDevice.BorderColor, _borderDevice.BorderColor);
                        // --- Obtain the future attribute byte
                        _attrByte1 = _fetchScreenMemory(ulaTact.AttributeToFetchAddress);
                        break;

                    case UlaRenderingPhase.DisplayByte1:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(ref ulaTact, 
                            GetColor(_pixelByte1 & 0x80, _attrByte1),
                            GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        break;

                    case UlaRenderingPhase.DisplayByte1AndFetchByte2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(ref ulaTact, 
                            GetColor(_pixelByte1 & 0x80, _attrByte1),
                            GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        // --- Obtain the next pixel byte
                        _pixelByte2 = _fetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case UlaRenderingPhase.DisplayByte1AndFetchAttribute2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(ref ulaTact, 
                            GetColor(_pixelByte1 & 0x80, _attrByte1),
                            GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        // --- Obtain the next attribute
                        _attrByte2 = _fetchScreenMemory(ulaTact.AttributeToFetchAddress);
                        break;

                    case UlaRenderingPhase.DisplayByte2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(ref ulaTact, 
                            GetColor(_pixelByte2 & 0x80, _attrByte2),
                            GetColor(_pixelByte2 & 0x40, _attrByte2));
                        // --- Shift in the subsequent bits
                        _pixelByte2 <<= 2;
                        break;

                    case UlaRenderingPhase.DisplayByte2AndFetchByte1:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(ref ulaTact, 
                            GetColor(_pixelByte2 & 0x80, _attrByte2),
                            GetColor(_pixelByte2 & 0x40, _attrByte2));
                        // --- Shift in the subsequent bits
                        _pixelByte2 <<= 2;
                        // --- Obtain the next pixel byte
                        _pixelByte1 = _fetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case UlaRenderingPhase.DisplayByte2AndFetchAttribute1:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(ref ulaTact, 
                            GetColor(_pixelByte2 & 0x80, _attrByte2),
                            GetColor(_pixelByte2 & 0x40, _attrByte2));
                        // --- Shift in the subsequent bits
                        _pixelByte2 <<= 2;
                        // --- Obtain the next attribute
                        _attrByte1 = _fetchScreenMemory(ulaTact.AttributeToFetchAddress);
                        break;
                }
            }
        }

        /// <summary>
        /// Signs that the current frame rendering is completed and the frame
        /// is ready to be displayed
        /// </summary>
        public void SignFrameCompleted()
        {
            _pixelRenderer?.DisplayFrame();    
        }

        /// <summary>
        /// Gets the memory contention value for the specified tact
        /// </summary>
        /// <param name="tact">ULA tact</param>
        /// <returns></returns>
        public byte GetContentionValue(int tact)
        {
            return _ulaTactTable[(ushort) (tact%DisplayParameters.UlaFrameTactCount)].ContentionDelay;
        }

        /// <summary>
        /// Sets the two adjacent screen pixels belonging to the specified tact to the given
        /// color
        /// </summary>
        /// <param name="currentTact">Tact to set the corresponding pixels to</param>
        /// <param name="colorIndex1">Color index of the first pixel</param>
        /// <param name="colorIndex2">Color index of the second pixel</param>
        private void SetPixels(ref UlaTact currentTact, int colorIndex1, int colorIndex2)
        {
            _pixelRenderer?.RenderPixel(currentTact.XPos, currentTact.YPos, colorIndex1);
            _pixelRenderer?.RenderPixel(currentTact.XPos + 1, currentTact.YPos, colorIndex2);
        }

        /// <summary>
        /// Gets the color index for the specified pixel value according
        /// to the given color attribute
        /// </summary>
        /// <param name="pixelValue">0 for paper pixel, non-zero for ink pixel</param>
        /// <param name="attr">Color attribute</param>
        /// <returns></returns>
        private int GetColor(int pixelValue, byte attr)
        {
            var ink = (attr & 0x07) | ((attr & 0x40) >> 3);
            var paper = ((attr & 0x38) >> 3) | ((attr & 0x40) >> 3);
            return _flashPhase && (attr & 0x80) != 0
                ? (pixelValue == 0 ? ink : paper)
                : (pixelValue == 0 ? paper : ink);
        }

        /// <summary>
        /// Initializes the ULA Tact table, which is the pivotal piece of
        /// screen rendering
        /// </summary>
        private void InitializeUlaTactTable()
        {
            // --- Reset the tact information table
            _ulaTactTable = new UlaTact[DisplayParameters.UlaFrameTactCount];

            // --- Iterate through tacts
            for (var tact = 0; tact < DisplayParameters.UlaFrameTactCount; tact++)
            {
                // --- We can put a tact shift logic here in the future
                // ...

                // --- calculate screen line and tact in line values here
                var line = tact/DisplayParameters.ScreenLineTime;
                var tactInLine = tact%DisplayParameters.ScreenLineTime;

                // --- Default tact description
                var tactItem = new UlaTact
                {
                   Phase = UlaRenderingPhase.None,
                   ContentionDelay = 0
                };

                if (DisplayParameters.IsTactVisible(line, tactInLine))
                {
                    // --- Calculate the pixel positions of the area
                    tactItem.XPos = (ushort)((tactInLine - DisplayParameters.HorizontalBlankingTime) * 2);
                    tactItem.YPos = (ushort)(line - DisplayParameters.VerticalSyncLines - DisplayParameters.NonVisibleBorderTopLines);

                    // --- The current tact is in a visible screen area (border or display area)
                    if (!DisplayParameters.IsTactInDisplayArea(line, tactInLine))
                    {
                        // --- Set the current border color
                        tactItem.Phase = UlaRenderingPhase.Border;
                        if (line >= DisplayParameters.FirstDisplayLine && line <= DisplayParameters.LastDisplayLine)
                        {
                            // --- Left or right border area beside the display area
                            if (tactInLine == DisplayParameters.FirstPixelTactInLine - DisplayParameters.PixelDataPrefetchTime)
                            {
                                // --- Fetch the first pixel data byte of the current line (2 tacts away)
                                tactItem.Phase = UlaRenderingPhase.BorderAndFetchPixelByte;
                                tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine + 2);
                                tactItem.ContentionDelay = 6;
                            }
                            else if (tactInLine == DisplayParameters.FirstPixelTactInLine - DisplayParameters.AttributeDataPrefetchTime)
                            {
                                // --- Fetch the first attribute data byte of the current line (1 tact away)
                                tactItem.Phase = UlaRenderingPhase.BorderAndFetchPixelAttribute;
                                tactItem.AttributeToFetchAddress = CalculateAttributeAddress(line, tactInLine + 1);
                                tactItem.ContentionDelay = 5;
                            }
                        }
                    }
                    else
                    {
                        // --- According to the tact, the ULA does separate actions
                        var pixelTact = tactInLine - DisplayParameters.FirstPixelTactInLine;
                        switch (pixelTact & 7)
                        {
                            case 0:
                                // --- Display the current tact pixels
                                tactItem.Phase = UlaRenderingPhase.DisplayByte1;
                                tactItem.ContentionDelay = 4;
                                break;
                            case 1:
                                // --- Display the current tact pixels
                                tactItem.Phase = UlaRenderingPhase.DisplayByte1;
                                tactItem.ContentionDelay = 3;
                                break;
                            case 2:
                                // --- While displaying the current tact pixels, we need to prefetch the
                                // --- pixel data byte 2 tacts away
                                tactItem.Phase = UlaRenderingPhase.DisplayByte1AndFetchByte2;
                                tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine + 2);
                                tactItem.ContentionDelay = 2;
                                break;
                            case 3:
                                // --- While displaying the current tact pixels, we need to prefetch the
                                // --- attribute data byte 1 tacts away
                                tactItem.Phase = UlaRenderingPhase.DisplayByte1AndFetchAttribute2;
                                tactItem.AttributeToFetchAddress = CalculateAttributeAddress(line, tactInLine + 1);
                                tactItem.ContentionDelay = 1;
                                break;
                            case 4:
                            case 5:
                                // --- Display the current tact pixels
                                tactItem.Phase = UlaRenderingPhase.DisplayByte2;
                                break;
                            case 6:
                                if (tactInLine < DisplayParameters.FirstPixelTactInLine + DisplayParameters.DisplayLineTime - 2)
                                {
                                    // --- There are still more bytes to display in this line.
                                    // --- While displaying the current tact pixels, we need to prefetch the
                                    // --- pixel data byte 2 tacts away
                                    tactItem.Phase = UlaRenderingPhase.DisplayByte2AndFetchByte1;
                                    tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine + 2);
                                    tactItem.ContentionDelay = 6;
                                }
                                else
                                {
                                    // --- Last byte in this line.
                                    // --- Display the current tact pixels
                                    tactItem.Phase = UlaRenderingPhase.DisplayByte2;
                                }
                                break;
                            case 7:
                                if (tactInLine < DisplayParameters.FirstPixelTactInLine + DisplayParameters.DisplayLineTime - 1)
                                {
                                    // --- There are still more bytes to display in this line.
                                    // --- While displaying the current tact pixels, we need to prefetch the
                                    // --- attribute data byte 1 tacts away
                                    tactItem.Phase = UlaRenderingPhase.DisplayByte2AndFetchAttribute1;
                                    tactItem.AttributeToFetchAddress = CalculateAttributeAddress(line, tactInLine + 1);
                                    tactItem.ContentionDelay = 5;
                                }
                                else
                                {
                                    // --- Last byte in this line.
                                    // --- Display the current tact pixels
                                    tactItem.Phase = UlaRenderingPhase.DisplayByte2;
                                }
                                break;
                        }
                    }
                }

                // --- Calculation is ready, let's store the calculated tact item
                _ulaTactTable[tact] = tactItem;
            }
        }

        /// <summary>
        /// Calculates the pixel address for the specified line and tact within 
        /// the line
        /// </summary>
        /// <param name="line">Line index</param>
        /// <param name="tactInLine">Tact index within the line</param>
        /// <returns>ZX spectrum screen memory address</returns>
        /// <remarks>
        /// Memory address bits: 
        /// C0-C2: pixel count within a byte -- not used in address calculation
        /// C3-C7: pixel byte within a line
        /// V0-V7: pixel line address
        /// 
        /// Direct Pixel Address (da)
        /// =================================================================
        /// |A15|A14|A13|A12|A11|A10|A9 |A8 |A7 |A6 |A5 |A4 |A3 |A2 |A1 |A0 |
        /// =================================================================
        /// | 0 | 0 | 0 |V7 |V6 |V5 |V4 |V3 |V2 |V1 |V0 |C7 |C6 |C5 |C4 |C3 |
        /// =================================================================
        /// | 1 | 1 | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 0xF81F
        /// =================================================================
        /// | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0x0700
        /// =================================================================
        /// | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0x00E0
        /// =================================================================
        /// 
        /// Spectrum Pixel Address
        /// =================================================================
        /// |A15|A14|A13|A12|A11|A10|A9 |A8 |A7 |A6 |A5 |A4 |A3 |A2 |A1 |A0 |
        /// =================================================================
        /// | 0 | 0 | 0 |V7 |V6 |V2 |V1 |V0 |V5 |V4 |V3 |C7 |C6 |C5 |C4 |C3 |
        /// =================================================================
        /// </remarks>
        private ushort CalculatePixelByteAddress(int line, int tactInLine)
        {
            var row = line - DisplayParameters.FirstDisplayLine;
            var column = 2 *(tactInLine - (DisplayParameters.HorizontalBlankingTime + DisplayParameters.BorderLeftTime));
            var da = 0x4000 | (column >> 3) | (row << 5);
            return (ushort)((da & 0xF81F) // --- Reset V5, V4, V3, V2, V1
                | ((da & 0x0700) >> 3)    // --- Keep V5, V4, V3 only
                | ((da & 0x00E0) << 3));  // --- Exchange the V2, V1, V0 bit 
                                          // --- group with V5, V4, V3
        }

        /// <summary>
        /// Calculates the pixel attribute address for the specified line and 
        /// tact within the line
        /// </summary>
        /// <param name="line">Line index</param>
        /// <param name="tactInLine">Tact index within the line</param>
        /// <returns>ZX spectrum screen memory address</returns>
        /// <remarks>
        /// Memory address bits: 
        /// C0-C2: pixel count within a byte -- not used in address calculation
        /// C3-C7: pixel byte within a line
        /// V0-V7: pixel line address
        /// 
        /// Spectrum Attribute Address
        /// =================================================================
        /// |A15|A14|A13|A12|A11|A10|A9 |A8 |A7 |A6 |A5 |A4 |A3 |A2 |A1 |A0 |
        /// =================================================================
        /// | 0 | 1 | 0 | 1 | 1 | 0 |V7 |V6 |V5 |V4 |V3 |C7 |C6 |C5 |C4 |C3 |
        /// =================================================================
        /// </remarks>
        private ushort CalculateAttributeAddress(int line, int tactInLine)
        {
            var row = line - DisplayParameters.FirstDisplayLine;
            var column = 2 * (tactInLine - (DisplayParameters.HorizontalBlankingTime + DisplayParameters.BorderLeftTime));
            var da = (column >> 3) | ((row >> 3) << 5);
            return (ushort)(0x5800 + da);
        }
    }
}