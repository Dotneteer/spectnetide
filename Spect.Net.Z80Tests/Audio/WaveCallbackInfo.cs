using System;
using Spect.Net.Z80Tests.Audio.Interop;

namespace Spect.Net.Z80Tests.Audio
{
    /// <summary>
    /// Wave Callback Info
    /// </summary>
    public class WaveCallbackInfo
    {
        /// <summary>
        /// Callback Strategy
        /// </summary>
        public WaveCallbackStrategy Strategy { get; }
        /// <summary>
        /// Window Handle (if applicable)
        /// </summary>
        public IntPtr Handle { get; private set; }

        private WaveWindow _waveOutWindow;

        /// <summary>
        /// Sets up a new WaveCallbackInfo to use a New Window
        /// IMPORTANT: only use this on the GUI thread
        /// </summary>
        public static WaveCallbackInfo NewWindow()
        {
            return new WaveCallbackInfo(WaveCallbackStrategy.NewWindow, IntPtr.Zero);
        }

        private WaveCallbackInfo(WaveCallbackStrategy strategy, IntPtr handle)
        {
            Strategy = strategy;
            Handle = handle;
        }

        internal void Connect(WaveInterop.WaveCallback callback)
        {
            if (Strategy == WaveCallbackStrategy.NewWindow)
            {
                _waveOutWindow = new WaveWindow(callback);
                _waveOutWindow.CreateControl();
                Handle = _waveOutWindow.Handle;
            }
        }

        internal MmResult WaveOutOpen(out IntPtr waveOutHandle, int deviceNumber, WaveFormat waveFormat, WaveInterop.WaveCallback callback)
        {
            var result = Strategy == WaveCallbackStrategy.FunctionCallback 
                ? WaveInterop.waveOutOpen(out waveOutHandle, (IntPtr)deviceNumber, waveFormat, callback, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackFunction) 
                : WaveInterop.waveOutOpenWindow(out waveOutHandle, (IntPtr)deviceNumber, waveFormat, Handle, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackWindow);
            return result;
        }

        internal void Disconnect()
        {
            if (_waveOutWindow == null) return;
            _waveOutWindow.Close();
            _waveOutWindow = null;
        }
    }
}