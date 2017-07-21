using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Spect.Net.SpectrumEmu.Devices.Border;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.SpectrumEmu.Devices.Screen
{
    /// <summary>
    /// This class is responsible to render a single frame of the screen
    /// </summary>
    public class ScreenDevice : IScreenDevice
    {
        private readonly uint[] _spectrumColors =
        {
            0xFF000000, // Black
            0xFF0000AA, // Blue
            0xFFAA0000, // Red
            0xFFAA00AA, // Magenta
            0xFF00AA00, // Green
            0xFF00AAAA, // Cyan
            0xFFAAAA00, // Yellow
            0xFFAAAAAA, // White
            0xFF000000, // Bright Black
            0xFF0000FF, // Bright Blue
            0xFFFF0000, // Bright Red
            0xFFFF00FF, // Bright Magenta
            0xFF00FF00, // Bright Green
            0xFF00FFFF, // Bright Cyan
            0xFFFFFF00, // Bright Yellow
            0xFFFFFFFF, // Bright White
        };

        private readonly int[] _flashOffColors;
        private readonly int[] _flashOnColors;

        /// <summary>
        /// The device that handles the border color
        /// </summary>
        private readonly IBorderDevice _borderDevice;

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
        private RenderingTact[] _renderingTactTable;

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
        private int _xPos;
        private int _yPos;

        /// <summary>
        /// Gets the current frame count
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// The ZX Spectrum color palette
        /// </summary>
        public IReadOnlyList<uint> SpectrumColors { get; }

        public IDisplayParameters DisplayParameters { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class 
        /// using the specified display parameters
        /// </summary>
        /// <param name="hostVm">Host Spectrum VM</param>
        /// <param name="pixelRenderer">Object that renders the screen pixels</param>
        /// <param name="borderDevice">The border device to use when rendering the screen</param>
        /// <param name="fetchFunction">The function to fetch screen memory values</param>
        /// "/>
        public ScreenDevice(Spectrum48 hostVm, 
            IScreenPixelRenderer pixelRenderer,
            IBorderDevice borderDevice = null,
            Func<ushort, byte> fetchFunction = null)
        {
            DisplayParameters = hostVm.DisplayPars;
            _borderDevice = borderDevice ?? hostVm.BorderDevice;
            _fetchScreenMemory = fetchFunction ?? hostVm.MemoryDevice.OnUlaReadMemory;
            InitializeUlaTactTable();
            _flashPhase = false;
            FrameCount = 0;
            _pixelRenderer = pixelRenderer ?? new NoopPixelRenderer();
            _pixelRenderer?.SetPalette(_spectrumColors);
            SpectrumColors = new ReadOnlyCollection<uint>(_spectrumColors);

            // --- Calculate color conversion table
            _flashOffColors = new int[0x200];
            _flashOnColors = new int[0x200];

            for (var attr = 0; attr < 0x100; attr++)
            {
                var ink = (attr & 0x07) | ((attr & 0x40) >> 3);
                var paper = ((attr & 0x38) >> 3) | ((attr & 0x40) >> 3);
                _flashOffColors[attr] = paper;
                _flashOffColors[0x100 + attr] = ink;
                _flashOnColors[attr] = (attr & 0x80) != 0 ? ink : paper;
                _flashOnColors[0x100 + attr] = (attr & 0x80) != 0 ? paper : ink;
            }
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
            fromTact = fromTact % DisplayParameters.UlaFrameTactCount;
            toTact = toTact % DisplayParameters.UlaFrameTactCount;

            // --- Carry out each tact action according to the rendering phase
            for (var currentTact = fromTact; currentTact <= toTact; currentTact++)
            {
                var ulaTact = _renderingTactTable[currentTact];
                _xPos = ulaTact.XPos;
                _yPos = ulaTact.YPos;

                switch (ulaTact.Phase)
                {
                    case ScreenRenderingPhase.None:
                        // --- Invisible screen area, nothing to do
                        break;

                    case ScreenRenderingPhase.Border:
                        // --- Fetch the border color from ULA and set the corresponding border pixels
                        SetPixels(_borderDevice.BorderColor, _borderDevice.BorderColor);
                        break;

                    case ScreenRenderingPhase.BorderAndFetchPixelByte:
                        // --- Fetch the border color from ULA and set the corresponding border pixels
                        SetPixels(_borderDevice.BorderColor, _borderDevice.BorderColor);
                        // --- Obtain the future pixel byte
                        _pixelByte1 = _fetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case ScreenRenderingPhase.BorderAndFetchPixelAttribute:
                        // --- Fetch the border color from ULA and set the corresponding border pixels
                        SetPixels(_borderDevice.BorderColor, _borderDevice.BorderColor);
                        // --- Obtain the future attribute byte
                        _attrByte1 = _fetchScreenMemory(ulaTact.AttributeToFetchAddress);
                        break;

                    case ScreenRenderingPhase.DisplayByte1:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels( 
                            GetColor(_pixelByte1 & 0x80, _attrByte1),
                            GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        break;

                    case ScreenRenderingPhase.DisplayByte1AndFetchByte2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels( 
                            GetColor(_pixelByte1 & 0x80, _attrByte1),
                            GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        // --- Obtain the next pixel byte
                        _pixelByte2 = _fetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case ScreenRenderingPhase.DisplayByte1AndFetchAttribute2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(
                            GetColor(_pixelByte1 & 0x80, _attrByte1),
                            GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        // --- Obtain the next attribute
                        _attrByte2 = _fetchScreenMemory(ulaTact.AttributeToFetchAddress);
                        break;

                    case ScreenRenderingPhase.DisplayByte2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(
                            GetColor(_pixelByte2 & 0x80, _attrByte2),
                            GetColor(_pixelByte2 & 0x40, _attrByte2));
                        // --- Shift in the subsequent bits
                        _pixelByte2 <<= 2;
                        break;

                    case ScreenRenderingPhase.DisplayByte2AndFetchByte1:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(
                            GetColor(_pixelByte2 & 0x80, _attrByte2),
                            GetColor(_pixelByte2 & 0x40, _attrByte2));
                        // --- Shift in the subsequent bits
                        _pixelByte2 <<= 2;
                        // --- Obtain the next pixel byte
                        _pixelByte1 = _fetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case ScreenRenderingPhase.DisplayByte2AndFetchAttribute1:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(
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
            return _renderingTactTable[tact%DisplayParameters.UlaFrameTactCount].ContentionDelay;
        }

        /// <summary>
        /// Sets the two adjacent screen pixels belonging to the specified tact to the given
        /// color
        /// </summary>
        /// <param name="colorIndex1">Color index of the first pixel</param>
        /// <param name="colorIndex2">Color index of the second pixel</param>
        private void SetPixels(int colorIndex1, int colorIndex2)
        {
            _pixelRenderer.RenderPixel(_xPos, _yPos, colorIndex1);
            _pixelRenderer.RenderPixel(_xPos + 1, _yPos, colorIndex2);
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
            var offset = (pixelValue == 0 ? 0 : 0x100) + attr;
            return _flashPhase
                ? _flashOnColors[offset]
                : _flashOffColors[offset];
        }

        /// <summary>
        /// Initializes the ULA Tact table, which is the pivotal piece of
        /// screen rendering
        /// </summary>
        private void InitializeUlaTactTable()
        {
            // --- Reset the tact information table
            _renderingTactTable = new RenderingTact[DisplayParameters.UlaFrameTactCount];

            // --- Iterate through tacts
            for (var tact = 0; tact < DisplayParameters.UlaFrameTactCount; tact++)
            {
                // --- We can put a tact shift logic here in the future
                // ...

                // --- calculate screen line and tact in line values here
                var line = tact/DisplayParameters.ScreenLineTime;
                var tactInLine = tact%DisplayParameters.ScreenLineTime;

                // --- Default tact description
                var tactItem = new RenderingTact
                {
                   Phase = ScreenRenderingPhase.None,
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
                        tactItem.Phase = ScreenRenderingPhase.Border;
                        if (line >= DisplayParameters.FirstDisplayLine && line <= DisplayParameters.LastDisplayLine)
                        {
                            // --- Left or right border area beside the display area
                            if (tactInLine == DisplayParameters.FirstPixelTactInLine - DisplayParameters.PixelDataPrefetchTime)
                            {
                                // --- Fetch the first pixel data byte of the current line (2 tacts away)
                                tactItem.Phase = ScreenRenderingPhase.BorderAndFetchPixelByte;
                                tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine + 2);
                                tactItem.ContentionDelay = 6;
                            }
                            else if (tactInLine == DisplayParameters.FirstPixelTactInLine - DisplayParameters.AttributeDataPrefetchTime)
                            {
                                // --- Fetch the first attribute data byte of the current line (1 tact away)
                                tactItem.Phase = ScreenRenderingPhase.BorderAndFetchPixelAttribute;
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
                                tactItem.Phase = ScreenRenderingPhase.DisplayByte1;
                                tactItem.ContentionDelay = 4;
                                break;
                            case 1:
                                // --- Display the current tact pixels
                                tactItem.Phase = ScreenRenderingPhase.DisplayByte1;
                                tactItem.ContentionDelay = 3;
                                break;
                            case 2:
                                // --- While displaying the current tact pixels, we need to prefetch the
                                // --- pixel data byte 2 tacts away
                                tactItem.Phase = ScreenRenderingPhase.DisplayByte1AndFetchByte2;
                                tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine + 2);
                                tactItem.ContentionDelay = 2;
                                break;
                            case 3:
                                // --- While displaying the current tact pixels, we need to prefetch the
                                // --- attribute data byte 1 tacts away
                                tactItem.Phase = ScreenRenderingPhase.DisplayByte1AndFetchAttribute2;
                                tactItem.AttributeToFetchAddress = CalculateAttributeAddress(line, tactInLine + 1);
                                tactItem.ContentionDelay = 1;
                                break;
                            case 4:
                            case 5:
                                // --- Display the current tact pixels
                                tactItem.Phase = ScreenRenderingPhase.DisplayByte2;
                                break;
                            case 6:
                                if (tactInLine < DisplayParameters.FirstPixelTactInLine + DisplayParameters.DisplayLineTime - 2)
                                {
                                    // --- There are still more bytes to display in this line.
                                    // --- While displaying the current tact pixels, we need to prefetch the
                                    // --- pixel data byte 2 tacts away
                                    tactItem.Phase = ScreenRenderingPhase.DisplayByte2AndFetchByte1;
                                    tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine + 2);
                                    tactItem.ContentionDelay = 6;
                                }
                                else
                                {
                                    // --- Last byte in this line.
                                    // --- Display the current tact pixels
                                    tactItem.Phase = ScreenRenderingPhase.DisplayByte2;
                                }
                                break;
                            case 7:
                                if (tactInLine < DisplayParameters.FirstPixelTactInLine + DisplayParameters.DisplayLineTime - 1)
                                {
                                    // --- There are still more bytes to display in this line.
                                    // --- While displaying the current tact pixels, we need to prefetch the
                                    // --- attribute data byte 1 tacts away
                                    tactItem.Phase = ScreenRenderingPhase.DisplayByte2AndFetchAttribute1;
                                    tactItem.AttributeToFetchAddress = CalculateAttributeAddress(line, tactInLine + 1);
                                    tactItem.ContentionDelay = 5;
                                }
                                else
                                {
                                    // --- Last byte in this line.
                                    // --- Display the current tact pixels
                                    tactItem.Phase = ScreenRenderingPhase.DisplayByte2;
                                }
                                break;
                        }
                    }
                }

                // --- Calculation is ready, let's store the calculated tact item
                _renderingTactTable[tact] = tactItem;
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

        /// <summary>
        /// No operation pixel renderer
        /// </summary>
        private class NoopPixelRenderer : IScreenPixelRenderer
        {
            /// <summary>
            /// The component provider should be able to reset itself
            /// </summary>
            public void Reset()
            {
            }

            /// <summary>
            /// Sets the palette that should be used with the renderer
            /// </summary>
            /// <param name="palette"></param>
            public void SetPalette(IList<uint> palette)
            {
            }

            /// <summary>
            /// The ULA signs that it's time to start a new frame
            /// </summary>
            public void StartNewFrame()
            {
            }

            /// <summary>
            /// Renders the (<paramref name="x"/>, <paramref name="y"/>) pixel
            /// on the screen with the specified <paramref name="colorIndex"/>
            /// </summary>
            /// <param name="x">Horizontal coordinate</param>
            /// <param name="y">Vertical coordinate</param>
            /// <param name="colorIndex">Index of the color (0x00..0x0F)</param>
            public void RenderPixel(int x, int y, int colorIndex)
            {
            }

            /// <summary>
            /// Signs that the current frame is rendered and ready to be displayed
            /// </summary>
            public void DisplayFrame()
            {
            }
        }
    }
}