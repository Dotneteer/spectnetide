namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    /// <summary>
    /// This class provides acces to the Windows Kernel32.dll methods
    /// </summary>
    public static class KernelMethods
    {
        /// <summary>
        /// QueryPerformanceCounter function
        /// Retrieves the current value of the performance counter, which is 
        /// a high resolution (less than 1us) time stamp that can be used for 
        /// time-interval measurements.
        /// </summary>
        /// <param name="lpPerformanceCount">
        /// A pointer to a variable that receives the current performance-
        /// counter value, in counts.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. On systems 
        /// that run Windows XP or later, the function will always succeed and 
        /// will thus never return zero.
        /// </returns>
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        /// <summary>
        /// Retrieves the frequency of the performance counter. The frequency 
        /// of the performance counter is fixed at system boot and is consistent 
        /// across all processors. Therefore, the frequency need only be queried 
        /// upon application initialization, and the result can be cached.
        /// </summary>
        /// <param name="lpFrequency">
        /// A pointer to a variable that receives the current performance-counter 
        /// frequency, in counts per second. If the installed hardware doesn't 
        /// support a high-resolution performance counter, this parameter can be 
        /// zero (this will not occur on systems that run Windows XP or later).
        /// </param>
        /// <returns>
        /// If the installed hardware supports a high-resolution performance 
        /// counter, the return value is nonzero. On systems that run Windows XP 
        /// or later, the function will always succeed and will thus never 
        /// return zero.
        /// </returns>
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(
            out long lpFrequency);
    }
}