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
                // --- If necessary, we can put a tact shift logic here in the future
                // ...

                // --- calculate screen (and not display!) coordinates here
                var line = tact/_displayPars.ScreenLineTime;
                var pixel = tact%_displayPars.ScreenLineTime;

                // --- Default tact description
                var tactItem = new UlaTact
                {
                   Phase = UlaRenderingPhase.None,
                   ContentionDelay = 0
                };




                // --- Calculation is ready, let's store the calculated tact item
                _ulaTactTable[tact] = tactItem;
            }
        }
    }
}