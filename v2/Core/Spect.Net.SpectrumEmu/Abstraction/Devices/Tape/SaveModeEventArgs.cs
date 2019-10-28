using System;

namespace Spect.Net.SpectrumEmu.Abstraction.Devices.Tape
{
    /// <summary>
    /// This class represents the event arguments of SAVE events
    /// </summary>
    public class SaveModeEventArgs: EventArgs
    {
        public SaveModeEventArgs(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        /// The name of the saved file
        /// </summary>
        public string FileName { get; }
    }
}