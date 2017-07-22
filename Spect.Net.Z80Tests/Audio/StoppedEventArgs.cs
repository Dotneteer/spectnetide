using System;

namespace Spect.Net.Z80Tests.Audio
{
    public class StoppedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of StoppedEventArgs
        /// </summary>
        /// <param name="exception">An exception to report (null if no exception)</param>
        public StoppedEventArgs(Exception exception = null)
        {
            this.Exception = exception;
        }

        /// <summary>
        /// An exception. Will be null if the playback or record operation stopped
        /// </summary>
        public Exception Exception { get; }
    }
}