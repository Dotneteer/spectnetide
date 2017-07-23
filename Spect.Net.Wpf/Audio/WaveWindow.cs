using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Spect.Net.Wpf.Audio.Interop;

namespace Spect.Net.Wpf.Audio
{
    internal class WaveWindow : Form
    {
        private readonly WaveInterop.WaveCallback _waveCallback;

        public WaveWindow(WaveInterop.WaveCallback waveCallback)
        {
            _waveCallback = waveCallback;
        }

        protected override void WndProc(ref Message m)
        {
            var message = (WaveInterop.WaveMessage)m.Msg;

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