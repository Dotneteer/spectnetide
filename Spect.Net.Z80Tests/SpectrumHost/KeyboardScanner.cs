using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using Spect.Net.SpectrumEmu.Keyboard;
using Spect.Net.Z80Tests.ViewModels.SpectrumEmu;
// ReSharper disable InconsistentNaming

namespace Spect.Net.Z80Tests.SpectrumHost
{
    /// <summary>
    /// This class is responsible for scanning the entire keyboard
    /// </summary>
    public class KeyboardScanner
    {
        private const string ENG_US_LAYOUT = "00000409";
        private const string HUN_LAYOUT = "0000040E";
        private const string HUN_101_LAYOUT = "0001040E";

        private const string DEFAULT_LAYOUT = "default";

        [DllImport("user32.dll")]
        private static extern bool GetKeyboardLayoutName([Out] StringBuilder pwszKLID);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        private static readonly Dictionary<SpectrumKeyCode, List<Key>> s_SpectrumKeyMappings = 
            new Dictionary<SpectrumKeyCode, List<Key>>
            {
                { SpectrumKeyCode.SShift, new List<Key> { Key.LeftShift, Key.RightShift, Key.OemComma, Key.OemPeriod } },
                { SpectrumKeyCode.CShift, new List<Key> { Key.LeftCtrl, Key.RightCtrl, Key.Back,
                    Key.Left, Key.Up, Key.Down, Key.Right } },
                { SpectrumKeyCode.Space, new List<Key> { Key.Space} },
                { SpectrumKeyCode.Enter, new List<Key> { Key.Enter } },
                { SpectrumKeyCode.Q, new List<Key> { Key.Q } },
                { SpectrumKeyCode.W, new List<Key> { Key.W } },
                { SpectrumKeyCode.E, new List<Key> { Key.E } },
                { SpectrumKeyCode.R, new List<Key> { Key.R } },
                { SpectrumKeyCode.T, new List<Key> { Key.T } },
                { SpectrumKeyCode.Y, new List<Key> { Key.Y } },
                { SpectrumKeyCode.U, new List<Key> { Key.U } },
                { SpectrumKeyCode.I, new List<Key> { Key.I } },
                { SpectrumKeyCode.O, new List<Key> { Key.O } },
                { SpectrumKeyCode.P, new List<Key> { Key.P } },
                { SpectrumKeyCode.A, new List<Key> { Key.A } },
                { SpectrumKeyCode.S, new List<Key> { Key.S } },
                { SpectrumKeyCode.D, new List<Key> { Key.D } },
                { SpectrumKeyCode.F, new List<Key> { Key.F } },
                { SpectrumKeyCode.G, new List<Key> { Key.G } },
                { SpectrumKeyCode.H, new List<Key> { Key.H } },
                { SpectrumKeyCode.J, new List<Key> { Key.J } },
                { SpectrumKeyCode.K, new List<Key> { Key.K } },
                { SpectrumKeyCode.L, new List<Key> { Key.L } },
                { SpectrumKeyCode.Z, new List<Key> { Key.Z } },
                { SpectrumKeyCode.X, new List<Key> { Key.X } },
                { SpectrumKeyCode.C, new List<Key> { Key.C } },
                { SpectrumKeyCode.V, new List<Key> { Key.V } },
                { SpectrumKeyCode.B, new List<Key> { Key.B } },
                { SpectrumKeyCode.N, new List<Key> { Key.N, Key.OemComma } },
                { SpectrumKeyCode.M, new List<Key> { Key.M, Key.OemPeriod } },
                { SpectrumKeyCode.N0, new List<Key> { Key.D0, Key.NumPad0, Key.Back } },
                { SpectrumKeyCode.N1, new List<Key> { Key.D1, Key.NumPad1 } },
                { SpectrumKeyCode.N2, new List<Key> { Key.D2, Key.NumPad2 } },
                { SpectrumKeyCode.N3, new List<Key> { Key.D3, Key.NumPad3 } },
                { SpectrumKeyCode.N4, new List<Key> { Key.D4, Key.NumPad4 } },
                { SpectrumKeyCode.N5, new List<Key> { Key.D5, Key.NumPad5, Key.Left } },
                { SpectrumKeyCode.N6, new List<Key> { Key.D6, Key.NumPad6, Key.Down } },
                { SpectrumKeyCode.N7, new List<Key> { Key.D7, Key.NumPad7, Key.Up } },
                { SpectrumKeyCode.N8, new List<Key> { Key.D8, Key.NumPad8, Key.Right } },
                { SpectrumKeyCode.N9, new List<Key> { Key.D9, Key.NumPad9 } },
            };

        public SpectrumDebugViewModel Vm { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public KeyboardScanner(SpectrumDebugViewModel vm)
        {
            Vm = vm;
        }

        // --- Scans the keyboard
        public void Scan()
        {
            if (!ApplicationIsActivated())
            {
                return;
            }
            foreach (var keyInfo in s_SpectrumKeyMappings)
            {
                var keyState = keyInfo.Value.Any(Keyboard.IsKeyDown);
                Vm.SetKeyStatus(keyInfo.Key, keyState);
            }
        }

        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }
    }
}