using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Spect.Net.Z80Tests.Audio.Interop;

namespace Spect.Net.Z80Tests.Audio
{
    internal class WaveWindowNative : NativeWindow
    {
        private readonly WaveInterop.WaveCallback _waveCallback;

        public WaveWindowNative(WaveInterop.WaveCallback waveCallback)
        {
            _waveCallback = waveCallback;
        }

        protected override void WndProc(ref Message m)
        {
            var message = (WaveInterop.WaveMessage) m.Msg;

            switch (message)
            {
                case WaveInterop.WaveMessage.WaveOutDone:
                case WaveInterop.WaveMessage.WaveInData:
                    var hOutputDevice = m.WParam;
                    var waveHeader = new WaveHeader();
                    Marshal.PtrToStructure(m.LParam, waveHeader);
                    _waveCallback(hOutputDevice, message, IntPtr.Zero, waveHeader, IntPtr.Zero);
                    break;
                case WaveInterop.WaveMessage.WaveOutOpen:
                case WaveInterop.WaveMessage.WaveOutClose:
                case WaveInterop.WaveMessage.WaveInClose:
                case WaveInterop.WaveMessage.WaveInOpen:
                    _waveCallback(m.WParam, message, IntPtr.Zero, null, IntPtr.Zero);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}