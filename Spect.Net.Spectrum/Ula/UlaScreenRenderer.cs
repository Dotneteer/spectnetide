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

        public void InitializeUlaTactTable()
        {
            _ulaTactTable = new UlaTact[_displayPars.UlaFrameTactCount];
            _tableInitialized = true;
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
    }
}