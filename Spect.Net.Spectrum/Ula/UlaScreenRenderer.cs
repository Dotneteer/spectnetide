using System;

namespace Spect.Net.Spectrum.Ula
{
    /// <summary>
    /// This class is responsible to render a single frame of the screen
    /// </summary>
    public class UlaScreenRenderer
    {
        private readonly UlaVideoDisplayParameters _displayPars;
        private bool _tableInitialized;
        private UlaTact[] _ulaTactTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class 
        /// using the specified display parameters
        /// </summary>
        public UlaScreenRenderer(UlaVideoDisplayParameters displayPars)
        {
            _displayPars = displayPars;
            _tableInitialized = false;
        }

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
        /// Initializes the ULA Tact table, which is the pivotal piece of
        /// screen rendering
        /// </summary>
        public void InitializeUlaTactTable()
        {
            _ulaTactTable = new UlaTact[_displayPars.UlaFrameTactCount];
            _tableInitialized = true;

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
                    if (_displayPars.IsTactInBorderArea(line, tactInLine))
                    {
                        // --- Border area
                        if (tactInLine == _displayPars.FirstPixelTact - _displayPars.PixelDataPrefetchTime)
                        {
                            tactItem.Phase = UlaRenderingPhase.BorderAndFetchPixelByte;
                            // TODO: Calculate pixel byte address
                        }
                        else if (tactInLine == _displayPars.FirstPixelTact - _displayPars.AttributeDataPrefetchTime)
                        {
                            tactItem.Phase = UlaRenderingPhase.BorderAndFetchPixelAttribute;
                            // TODO: Calculate pixel attribute address
                        }
                        else
                        {
                            tactItem.Phase = UlaRenderingPhase.Border;
                        }
                    }
                    else
                    {
                        
                    }
                }

                // --- Calculation is ready, let's store the calculated tact item
                _ulaTactTable[tact] = tactItem;
            }
        }
    }
}