using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Devices.Screen
{
    /// <summary>
    /// This class is responsible to render a single frame of the screen
    /// </summary>
    public class Spectrum48ScreenDevice : IScreenDevice, IScreenDeviceTestSupport
    {
        public  static readonly ReadOnlyCollection<uint> SpectrumColors = new ReadOnlyCollection<uint>(new List<uint>
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
        });

        private byte[] _pixelBuffer;
        private int[] _flashOffColors;
        private int[] _flashOnColors;

        /// <summary>
        /// Defines the action that accesses the screen memory
        /// </summary>
        private Func<ushort, byte> _fetchScreenMemory;

        /// <summary>
        /// The devices that physically renders the screen
        /// </summary>
        private IScreenFrameProvider _pixelRenderer;

        /// <summary>
        /// Gets or sets the current border color
        /// </summary>
        public int BorderColor { get; set; }

        /// <summary>
        /// Table of ULA tact action information entries
        /// </summary>
        public RenderingTact[] RenderingTactTable { get; private set; }

        /// <summary>
        /// Indicates the refresh rate calculated from the base clock frequency
        /// of the CPU and the screen configuration (total #of ULA tacts per frame)
        /// </summary>
        public decimal RefreshRate { get; private set; }

        /// <summary>
        /// The number of frames when the flash flag should be toggles
        /// </summary>
        public int FlashToggleFrames { get; private set; }

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
        private int _screenWidth;

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
            var screenInfo = hostVm.GetDeviceInfo<IScreenDevice>();
            ScreenConfiguration = hostVm.ScreenConfiguration;
            _pixelRenderer = (IScreenFrameProvider)screenInfo.Provider ?? new NoopPixelRenderer();

            _fetchScreenMemory = hostVm.MemoryDevice.UlaRead;
            InitializeUlaTactTable();
            _flashPhase = false;
            FrameCount = 0;

            // --- Calculate refresh rate related values
            RefreshRate = (decimal) hostVm.BaseClockFrequency / ScreenConfiguration.UlaFrameTactCount;
            FlashToggleFrames = (int) Math.Round(RefreshRate/2);

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

            _screenWidth = hostVm.ScreenDevice.ScreenConfiguration.ScreenWidth;
            _pixelBuffer = new byte[_screenWidth * hostVm.ScreenDevice.ScreenConfiguration.ScreenLines];
        }

        /// <summary>
        /// Gets the current frame count
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// Overflow from the previous frame, given in #of tacts 
        /// </summary>
        public int Overflow { get; set; }

        /// <summary>
        /// Allow the device to react to the start of a new frame
        /// </summary>
        public void OnNewFrame()
        {
            FrameCount++;
            if (FrameCount % FlashToggleFrames == 0)
            {
                _flashPhase = !_flashPhase;
            }
            _pixelRenderer?.StartNewFrame();
            RenderScreen(0, Overflow);
        }

        /// <summary>
        /// Allow the device to react to the completion of a frame
        /// </summary>
        public void OnFrameCompleted()
        {
            _pixelRenderer?.DisplayFrame(_pixelBuffer);
            FrameCompleted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Allow external entities respond to frame completion
        /// </summary>
        public event EventHandler FrameCompleted;

        public ScreenConfiguration ScreenConfiguration { get; private set; }

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
        /// Executes the ULA rendering actions between the specified tacts
        /// </summary>
        /// <param name="fromTact">First ULA tact</param>
        /// <param name="toTact">Last ULA tact</param>
        public void RenderScreen(int fromTact, int toTact)
        {
            // --- Adjust the tact boundaries
            fromTact = fromTact % ScreenConfiguration.UlaFrameTactCount;
            toTact = toTact % ScreenConfiguration.UlaFrameTactCount;

            // --- Carry out each tact action according to the rendering phase
            for (var currentTact = fromTact; currentTact <= toTact; currentTact++)
            {
                var ulaTact = RenderingTactTable[currentTact];
                _xPos = ulaTact.XPos;
                _yPos = ulaTact.YPos;

                switch (ulaTact.Phase)
                {
                    case ScreenRenderingPhase.None:
                        // --- Invisible screen area, nothing to do
                        break;

                    case ScreenRenderingPhase.Border:
                        // --- Fetch the border color from ULA and set the corresponding border pixels
                        SetPixels(BorderColor, BorderColor);
                        break;

                    case ScreenRenderingPhase.BorderFetchPixel:
                        // --- Fetch the border color from ULA and set the corresponding border pixels
                        SetPixels(BorderColor, BorderColor);
                        // --- Obtain the future pixel byte
                        _pixelByte1 = _fetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case ScreenRenderingPhase.BorderFetchPixelAttr:
                        // --- Fetch the border color from ULA and set the corresponding border pixels
                        SetPixels(BorderColor, BorderColor);
                        // --- Obtain the future attribute byte
                        _attrByte1 = _fetchScreenMemory(ulaTact.AttributeToFetchAddress);
                        break;

                    case ScreenRenderingPhase.DisplayB1:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels( 
                            GetColor(_pixelByte1 & 0x80, _attrByte1),
                            GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        break;

                    case ScreenRenderingPhase.DisplayB1FetchB2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels( 
                            GetColor(_pixelByte1 & 0x80, _attrByte1),
                            GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        // --- Obtain the next pixel byte
                        _pixelByte2 = _fetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case ScreenRenderingPhase.DisplayB1FetchA2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(
                            GetColor(_pixelByte1 & 0x80, _attrByte1),
                            GetColor(_pixelByte1 & 0x40, _attrByte1));
                        // --- Shift in the subsequent bits
                        _pixelByte1 <<= 2;
                        // --- Obtain the next attribute
                        _attrByte2 = _fetchScreenMemory(ulaTact.AttributeToFetchAddress);
                        break;

                    case ScreenRenderingPhase.DisplayB2:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(
                            GetColor(_pixelByte2 & 0x80, _attrByte2),
                            GetColor(_pixelByte2 & 0x40, _attrByte2));
                        // --- Shift in the subsequent bits
                        _pixelByte2 <<= 2;
                        break;

                    case ScreenRenderingPhase.DisplayB2FetchB1:
                        // --- Display bit 7 and 6 according to the corresponding color
                        SetPixels(
                            GetColor(_pixelByte2 & 0x80, _attrByte2),
                            GetColor(_pixelByte2 & 0x40, _attrByte2));
                        // --- Shift in the subsequent bits
                        _pixelByte2 <<= 2;
                        // --- Obtain the next pixel byte
                        _pixelByte1 = _fetchScreenMemory(ulaTact.PixelByteToFetchAddress);
                        break;

                    case ScreenRenderingPhase.DisplayB2FetchA1:
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
        /// Gets the memory contention value for the specified tact
        /// </summary>
        /// <param name="tact">ULA tact</param>
        /// <returns></returns>
        public byte GetContentionValue(int tact)
        {
            return RenderingTactTable[tact%ScreenConfiguration.UlaFrameTactCount].ContentionDelay;
        }

        /// <summary>
        /// Gets the buffer that holds the screen pixels
        /// </summary>
        /// <returns></returns>
        public byte[] GetPixelBuffer()
        {
            return _pixelBuffer;
        }

        /// <summary>
        /// Sets the two adjacent screen pixels belonging to the specified tact to the given
        /// color
        /// </summary>
        /// <param name="colorIndex1">Color index of the first pixel</param>
        /// <param name="colorIndex2">Color index of the second pixel</param>
        private void SetPixels(int colorIndex1, int colorIndex2)
        {
            var pos = _yPos * _screenWidth + _xPos;
            _pixelBuffer[pos++] = (byte) colorIndex1;
            _pixelBuffer[pos] = (byte) colorIndex2;
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
        /// Initializes the ULA Tacts table, which is the pivotal piece of
        /// screen rendering
        /// </summary>
        private void InitializeUlaTactTable()
        {
            // --- Reset the tact information table
            RenderingTactTable = new RenderingTact[ScreenConfiguration.UlaFrameTactCount];

            // --- Iterate through tacts
            for (var tact = 0; tact < ScreenConfiguration.UlaFrameTactCount; tact++)
            {
                // --- We can put a tact shift logic here in the future
                // ...

                // --- calculate screen line and tact in line values here
                var line = tact/ScreenConfiguration.ScreenLineTime;
                var tactInLine = tact%ScreenConfiguration.ScreenLineTime;

                // --- Default tact description
                var tactItem = new RenderingTact
                {
                   Phase = ScreenRenderingPhase.None,
                   ContentionDelay = 0
                };

                if (ScreenConfiguration.IsTactVisible(line, tactInLine))
                {
                    // --- Calculate the pixel positions of the area
                    tactItem.XPos = (ushort)((tactInLine - ScreenConfiguration.HorizontalBlankingTime) * 2);
                    tactItem.YPos = (ushort)(line - ScreenConfiguration.VerticalSyncLines - ScreenConfiguration.NonVisibleBorderTopLines);

                    // --- The current tact is in a visible screen area (border or display area)
                    if (!ScreenConfiguration.IsTactInDisplayArea(line, tactInLine))
                    {
                        // --- Set the current border color
                        tactItem.Phase = ScreenRenderingPhase.Border;
                        if (line >= ScreenConfiguration.FirstDisplayLine && line <= ScreenConfiguration.LastDisplayLine)
                        {
                            // --- Left or right border area beside the display area
                            if (tactInLine == ScreenConfiguration.FirstPixelTactInLine - ScreenConfiguration.PixelDataPrefetchTime)
                            {
                                // --- Fetch the first pixel data byte of the current line (2 tacts away)
                                tactItem.Phase = ScreenRenderingPhase.BorderFetchPixel;
                                tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine + 2);
                                tactItem.ContentionDelay = 6;
                            }
                            else if (tactInLine == ScreenConfiguration.FirstPixelTactInLine - ScreenConfiguration.AttributeDataPrefetchTime)
                            {
                                // --- Fetch the first attribute data byte of the current line (1 tact away)
                                tactItem.Phase = ScreenRenderingPhase.BorderFetchPixelAttr;
                                tactItem.AttributeToFetchAddress = CalculateAttributeAddress(line, tactInLine + 1);
                                tactItem.ContentionDelay = 5;
                            }
                        }
                    }
                    else
                    {
                        // --- According to the tact, the ULA does separate actions
                        var pixelTact = tactInLine - ScreenConfiguration.FirstPixelTactInLine;
                        switch (pixelTact & 7)
                        {
                            case 0:
                                // --- Display the current tact pixels
                                tactItem.Phase = ScreenRenderingPhase.DisplayB1;
                                tactItem.ContentionDelay = 4;
                                break;
                            case 1:
                                // --- Display the current tact pixels
                                tactItem.Phase = ScreenRenderingPhase.DisplayB1;
                                tactItem.ContentionDelay = 3;
                                break;
                            case 2:
                                // --- While displaying the current tact pixels, we need to prefetch the
                                // --- pixel data byte 2 tacts away
                                tactItem.Phase = ScreenRenderingPhase.DisplayB1FetchB2;
                                tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine + 2);
                                tactItem.ContentionDelay = 2;
                                break;
                            case 3:
                                // --- While displaying the current tact pixels, we need to prefetch the
                                // --- attribute data byte 1 tacts away
                                tactItem.Phase = ScreenRenderingPhase.DisplayB1FetchA2;
                                tactItem.AttributeToFetchAddress = CalculateAttributeAddress(line, tactInLine + 1);
                                tactItem.ContentionDelay = 1;
                                break;
                            case 4:
                            case 5:
                                // --- Display the current tact pixels
                                tactItem.Phase = ScreenRenderingPhase.DisplayB2;
                                break;
                            case 6:
                                if (tactInLine < ScreenConfiguration.FirstPixelTactInLine + ScreenConfiguration.DisplayLineTime - 2)
                                {
                                    // --- There are still more bytes to display in this line.
                                    // --- While displaying the current tact pixels, we need to prefetch the
                                    // --- pixel data byte 2 tacts away
                                    tactItem.Phase = ScreenRenderingPhase.DisplayB2FetchB1;
                                    tactItem.PixelByteToFetchAddress = CalculatePixelByteAddress(line, tactInLine + 2);
                                    tactItem.ContentionDelay = 6;
                                }
                                else
                                {
                                    // --- Last byte in this line.
                                    // --- Display the current tact pixels
                                    tactItem.Phase = ScreenRenderingPhase.DisplayB2;
                                }
                                break;
                            case 7:
                                if (tactInLine < ScreenConfiguration.FirstPixelTactInLine + ScreenConfiguration.DisplayLineTime - 1)
                                {
                                    // --- There are still more bytes to display in this line.
                                    // --- While displaying the current tact pixels, we need to prefetch the
                                    // --- attribute data byte 1 tacts away
                                    tactItem.Phase = ScreenRenderingPhase.DisplayB2FetchA1;
                                    tactItem.AttributeToFetchAddress = CalculateAttributeAddress(line, tactInLine + 1);
                                    tactItem.ContentionDelay = 5;
                                }
                                else
                                {
                                    // --- Last byte in this line.
                                    // --- Display the current tact pixels
                                    tactItem.Phase = ScreenRenderingPhase.DisplayB2;
                                }
                                break;
                        }
                    }
                }

                // --- Calculation is ready, let's store the calculated tact item
                RenderingTactTable[tact] = tactItem;
            }
        }

        /// <summary>
        /// Calculates the pixel address for the specified line and tact within 
        /// the line
        /// </summary>
        /// <param name="line">Line index</param>
        /// <param name="tactInLine">Tacts index within the line</param>
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
            var row = line - ScreenConfiguration.FirstDisplayLine;
            var column = 2 *(tactInLine - (ScreenConfiguration.HorizontalBlankingTime + ScreenConfiguration.BorderLeftTime));
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
        /// <param name="tactInLine">Tacts index within the line</param>
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
            var row = line - ScreenConfiguration.FirstDisplayLine;
            var column = 2 * (tactInLine - (ScreenConfiguration.HorizontalBlankingTime + ScreenConfiguration.BorderLeftTime));
            var da = (column >> 3) | ((row >> 3) << 5);
            return (ushort)(0x5800 + da);
        }

        /// <summary>
        /// Fills the entire screen buffer with the specified data
        /// </summary>
        /// <param name="data">Data to fill the pixel buffer</param>
        public void FillScreenBuffer(byte data)
        {
            for (var i = 0; i < _pixelBuffer.Length; i++)
            {
                _pixelBuffer[i] = data;
            }
        }

        /// <summary>
        /// No operation pixel renderer
        /// </summary>
        private class NoopPixelRenderer : VmComponentProviderBase, IScreenFrameProvider
        {
            /// <summary>
            /// The ULA signs that it's time to start a new frame
            /// </summary>
            public void StartNewFrame()
            {
            }

            /// <summary>
            /// Signs that the current frame is rendered and ready to be displayed
            /// </summary>
            /// <param name="frame">The buffer that contains the frame to display</param>
            public void DisplayFrame(byte[] frame)
            {
            }
        }
    }
}