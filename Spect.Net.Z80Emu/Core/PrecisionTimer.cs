namespace Spect.Net.Z80Emu.Core
{
    internal class PrecisionTimer
    {
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(
            out long lpFrequency);

        private long _startTime, _stopTime;
        private static long s_Freq;
        private static bool s_FreqIsInitialized;

        public double DurationInSeconds { get; private set; }

        public double DurationInMilliseconds => DurationInSeconds * 1000;

        // Constructor
        public PrecisionTimer()
        {
            _startTime = 0;
            _stopTime = 0;
            DurationInSeconds = 0;
            if (QueryPerformanceFrequency(out s_Freq) == false)
            {
                // high-performance counter not supported
                throw new System.ComponentModel.Win32Exception();
            }
            s_FreqIsInitialized = true;
        }

        // Start the timer
        public void Start()
        {
            QueryPerformanceCounter(out _startTime);
            System.Threading.Thread.Sleep(0);
        }

        // Stop the timer
        public void Stop()
        {
            System.Threading.Thread.Sleep(0);
            QueryPerformanceCounter(out _stopTime);
            DurationInSeconds = (_stopTime - _startTime) / (double)s_Freq; //save the difference
            System.Threading.Thread.Sleep(0);
        }

        // Returns the current time
        public static double TimeInSeconds()
        {
            if (!s_FreqIsInitialized)
            {
                if (QueryPerformanceFrequency(out s_Freq) == false)
                {
                    // high-performance counter not supported
                    throw new System.ComponentModel.Win32Exception();
                }
                s_FreqIsInitialized = true;
            }
            long currentTime;
            System.Threading.Thread.Sleep(0);
            QueryPerformanceCounter(out currentTime);
            return currentTime / (double)s_Freq; //save the difference
        }

        public static double TimeInMilliseconds()
        {
            if (!s_FreqIsInitialized)
            {
                if (QueryPerformanceFrequency(out s_Freq) == false)
                {
                    // high-performance counter not supported
                    throw new System.ComponentModel.Win32Exception();
                }
                s_FreqIsInitialized = true;
            }
            long currentTime;
            System.Threading.Thread.Sleep(0);
            QueryPerformanceCounter(out currentTime);
            return (double)currentTime * 1000 / s_Freq; //save the difference
        }
    }
}