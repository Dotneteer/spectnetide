using System;

namespace Spect.Net.Spectrum.Ula
{
    /// <summary>
    /// This class is responsible to render a single frame of the screen
    /// </summary>
    public class UlaScreenRenderer
    {
        private readonly UlaChip _ulaChip;
        private UlaVideoDisplayParameters _displayPars;
        private UlaBorderDevice _borderDevice;
        private bool _tableInitialized;
        private UlaTact[] _ulaTactTable;

        private int _currentScreenTact;
        private byte _pixelByte1;
        private byte _pixelByte2;
        private byte _attrByte1;
        private byte _attrByte2;


        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class 
        /// using the specified display parameters
        /// </summary>
        public UlaScreenRenderer(UlaChip ulaChip)
        {
            _ulaChip = ulaChip;
            _tableInitialized = false;
        }

        /// <summary>
        /// Defines the action that accesses the screen memory
        /// </summary>
        public Func<ushort, byte> FetchScreenMemory { get; set; }

        /// <summary>
        /// Starts rendering a new frame from the first tact
        /// </summary>
        public void StartNewFrame()
        {
            if (!_tableInitialized)
            {
                throw new InvalidOperationException("UlaTactTable is not initialized yet.");
            }
        }

        /// <summary>
        /// Executes the ULA rendering actions between the specified tacts
        /// </summary>
        /// <param name="fromTact"></param>
        /// <param name="toTact"></param>
        public void RenderScreen(int fromTact, int toTact)
        {
            // --- Adjust the tact boundaries
            if (fromTact < 0) fromTact = 0;
            if (toTact > _displayPars.UlaFrameTactCount)
            {
                toTact = _displayPars.UlaFrameTactCount - 1;
            }

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
                        SetPixels(currentTact, _borderDevice.BorderColor);
                        break;

                    case UlaRenderingPhase.BorderAndFetchPixelByte:
                        // --- Fetch the border color from ULA and set the corresponding border pixels
                        SetPixels(currentTact, _borderDevice.BorderColor);
                        // --- Obtain the future pixel byte
                        _pixelByte1 = FetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case UlaRenderingPhase.BorderAndFetchPixelAttribute:
                        // --- Fetch the border color from ULA and set the corresponding border pixels
                        SetPixels(currentTact, _borderDevice.BorderColor);
                        // --- Obtain the future attribute byte
                        _attrByte1 = FetchScreenMemory(ulaTact.AttributeToFetchAddress);
                        break;

                    case UlaRenderingPhase.DisplayByte1:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(currentTact, GetColor(_pixelByte1 & 0x80, _attrByte1));
                        SetPixels(currentTact, GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        break;

                    case UlaRenderingPhase.DisplayByte1AndFetchByte2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(currentTact, GetColor(_pixelByte1 & 0x80, _attrByte1));
                        SetPixels(currentTact, GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        // --- Obtain the next pixel byte
                        _pixelByte2 = FetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case UlaRenderingPhase.DisplayByte1AndFetchAttribute2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(currentTact, GetColor(_pixelByte1 & 0x80, _attrByte1));
                        SetPixels(currentTact, GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        // --- Obtain the next attribute
                        _attrByte2 = FetchScreenMemory(ulaTact.AttributeToFetchAddress);
                        break;

                    case UlaRenderingPhase.DisplayByte2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(currentTact, GetColor(_pixelByte2 & 0x80, _attrByte2));
                        SetPixels(currentTact, GetColor(_pixelByte2 & 0x40, _attrByte2));
                        // --- Shift in the subsequent bits
                        _pixelByte2 <<= 2;
                        break;

                    case UlaRenderingPhase.DisplayByte2AndFetchByte1:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(currentTact, GetColor(_pixelByte2 & 0x80, _attrByte2));
                        SetPixels(currentTact, GetColor(_pixelByte2 & 0x40, _attrByte2));
                        // --- Shift in the subsequent bits
                        _pixelByte2 <<= 2;
                        // --- Obtain the next pixel byte
                        _pixelByte1 = FetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case UlaRenderingPhase.DisplayByte2AndFetchAttribute1:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(currentTact, GetColor(_pixelByte2 & 0x80, _attrByte2));
                        SetPixels(currentTact, GetColor(_pixelByte2 & 0x40, _attrByte2));
                        // --- Shift in the subsequent bits
                        _pixelByte2 <<= 2;
                        // --- Obtain the next attribute
                        _attrByte1 = FetchScreenMemory(ulaTact.AttributeToFetchAddress);
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the two adjacent screen pixels belonging to the specified tact to the given
        /// color
        /// </summary>
        /// <param name="currentTact">Tact to set the corresponding pixels to</param>
        /// <param name="colorIndex"></param>
        private void SetPixels(int currentTact, byte colorIndex)
        {
            // TODO: Implement this method
        }

        /// <summary>
        /// Gets the color index for the specified pixel value according
        /// to the given color attribute
        /// </summary>
        /// <param name="pixelValue">0 for paper pixel, non-zero for ink pixel</param>
        /// <param name="attr">Color attribute</param>
        /// <returns></returns>
        private byte GetColor(int pixelValue, byte attr)
        {
            // TODO: Implement this method
            return 0;
        }

        /// <summary>
        /// Initializes the ULA Tact table, which is the pivotal piece of
        /// screen rendering
        /// </summary>
        public void InitializeScreenRenderer()
        {
            // --- Obtain ULA resources
            _displayPars = _ulaChip.DisplayParameters;
            _borderDevice = _ulaChip.BorderDevice;

            // --- Reset the tact information table
            _ulaTactTable = new UlaTact[_displayPars.UlaFrameTactCount];

            // --- Iterate through tacts
            for (var tact = 0; tact < _displayPars.UlaFrameTactCount; tact++)
            {
                // --- We can put a tact shift logic here in the future
                // ...

                // --- calculate screen line and tact in line values here
                var line = tact/_displayPars.ScreenLineTime;
                var tactInLine = tact%_displayPars.ScreenLineTime;

                // --- Default tact description
                var tactItem = new UlaTact
                {
                   Phase = UlaRenderingPhase.None,
                   ContentionDelay = 0
                };

                if (_displayPars.IsTactVisible(line, tactInLine))
                {
                    // --- The current tact is in a visible screen area (border or display area)
                    if (_displayPars.IsTactInBorderArea(line, tactInLine))
                    {
                        // --- Border area
                        if (tactInLine == _displayPars.FirstPixelTact - _displayPars.PixelDataPrefetchTime)
                        {
                            // --- Fetch the first pixel data byte of the current line
                            tactItem.Phase = UlaRenderingPhase.BorderAndFetchPixelByte;
                            tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine);
                            tactItem.ContentionDelay = 6;
                        }
                        else if (tactInLine == _displayPars.FirstPixelTact - _displayPars.AttributeDataPrefetchTime)
                        {
                            // --- Fetch the first attribute data byte of the current line
                            tactItem.Phase = UlaRenderingPhase.BorderAndFetchPixelAttribute;
                            tactItem.AttributeToFetchAddress = CalculateAttributeAddress(line, tactInLine);
                            tactItem.ContentionDelay = 5;
                        }
                        else
                        {
                            // --- Set the current border color
                            tactItem.Phase = UlaRenderingPhase.Border;
                        }
                    }
                    else
                    {
                        // --- Display area
                        // --- According to the tact, the ULA does separate actions
                        var pixelTact = tactInLine - _displayPars.FirstPixelTact;
                        switch (pixelTact % 7)
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
                                tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine + 1);
                                tactItem.ContentionDelay = 1;
                                break;
                            case 4:
                            case 5:
                                // --- Display the current tact pixels
                                tactItem.Phase = UlaRenderingPhase.DisplayByte2;
                                break;
                            case 6:
                                if (tactInLine < _displayPars.FirstPixelTact + _displayPars.DisplayLineTime - 2)
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
                                if (tactInLine < _displayPars.FirstPixelTact + _displayPars.DisplayLineTime - 1)
                                {
                                    // --- There are still more bytes to display in this line.
                                    // --- While displaying the current tact pixels, we need to prefetch the
                                    // --- attribute data byte 1 tacts away
                                    tactItem.Phase = UlaRenderingPhase.DisplayByte2AndFetchAttribute1;
                                    tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine + 1);
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

            _tableInitialized = true;
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
        /// | 0 | 0 | 0 | 1 | 1 | 0 | 0 | 0 | 0 | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 0x181F
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
            var da = (tactInLine >> 2) | (line << 5);
            return (ushort)((da & 0x181F) // --- Reset V5, V4, V3, V2, V1
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
        /// | 0 | 0 | 0 | 1 | 1 | 0 |V7 |V6 |V5 |V4 |V3 |C7 |C6 |C5 |C4 |C3 |
        /// =================================================================
        /// </remarks>
        private ushort CalculateAttributeAddress(int line, int tactInLine)
        {
            var da = (tactInLine >> 2) | ((line >> 3) << 5);
            return (ushort)(0x1800 + da);
        }
    }
}