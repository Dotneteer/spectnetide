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
        private WaveWindowNative _waveOutWindowNative;

        /// <summary>
        /// Sets up a new WaveCallbackInfo for function callbacks
        /// </summary>
        public static WaveCallbackInfo FunctionCallback()
        {
            return new WaveCallbackInfo(WaveCallbackStrategy.FunctionCallback, IntPtr.Zero);
        }

        /// <summary>
        /// Sets up a new WaveCallbackInfo to use a New Window
        /// IMPORTANT: only use this on the GUI thread
        /// </summary>
        public static WaveCallbackInfo NewWindow()
        {
            return new WaveCallbackInfo(WaveCallbackStrategy.NewWindow, IntPtr.Zero);
        }

        /// <summary>
        /// Sets up a new WaveCallbackInfo to use an existing window
        /// IMPORTANT: only use this on the GUI thread
        /// </summary>
        public static WaveCallbackInfo ExistingWindow(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                throw new ArgumentException("Handle cannot be zero");
            }
            return new WaveCallbackInfo(WaveCallbackStrategy.ExistingWindow, handle);
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
            else if (Strategy == WaveCallbackStrategy.ExistingWindow)
            {
                _waveOutWindowNative = new WaveWindowNative(callback);
                _waveOutWindowNative.AssignHandle(Handle);
            }
        }

        internal MmResult WaveOutOpen(out IntPtr waveOutHandle, int deviceNumber, WaveFormat waveFormat, WaveInterop.WaveCallback callback)
        {
            var result = Strategy == WaveCallbackStrategy.FunctionCallback 
                ? WaveInterop.waveOutOpen(out waveOutHandle, (IntPtr)deviceNumber, waveFormat, callback, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackFunction) 
                : WaveInterop.waveOutOpenWindow(out waveOutHandle, (IntPtr)deviceNumber, waveFormat, Handle, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackWindow);
            return result;
        }

        internal MmResult WaveInOpen(out IntPtr waveInHandle, int deviceNumber, WaveFormat waveFormat, WaveInterop.WaveCallback callback)
        {
            var result = Strategy == WaveCallbackStrategy.FunctionCallback 
                ? WaveInterop.waveInOpen(out waveInHandle, (IntPtr)deviceNumber, waveFormat, callback, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackFunction) 
                : WaveInterop.waveInOpenWindow(out waveInHandle, (IntPtr)deviceNumber, waveFormat, Handle, IntPtr.Zero, WaveInterop.WaveInOutOpenFlags.CallbackWindow);
            return result;
        }

        internal void Disconnect()
        {
            if (_waveOutWindow != null)
            {
                _waveOutWindow.Close();
                _waveOutWindow = null;
            }
            if (_waveOutWindowNative != null)
            {
                _waveOutWindowNative.ReleaseHandle();
                _waveOutWindowNative = null;
            }
        }
    }
}