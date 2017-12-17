using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Keyboard;

// ReSharper disable InconsistentNaming

namespace Spect.Net.Wpf.Providers
{
    /// <summary>
    /// This class is responsible for scanning the entire keyboard
    /// </summary>
    public class KeyboardProvider: VmComponentProviderBase, IKeyboardProvider
    {
        // --- Keyboard layout codes to define separate key mappings for each of them
        private const string ENG_US_LAYOUT = "00000409";
        private const string HUN_LAYOUT = "0000040E";
        private const string HUN_101_LAYOUT = "0001040E";

        // --- You can create a default layout, provided you have non-implemented custom layout
        private const string DEFAULT_LAYOUT = "default";

        // --- This method calls back the IKeyboardDevice of the Spectrum VM
        // --- whenever the state of a key changes
        private Action<SpectrumKeyCode, bool> _statusHandler;

        // --- Stores the key strokes to emulate
        private readonly Queue<EmulatedKeyStroke> _emulatedKeyStrokes = 
            new Queue<EmulatedKeyStroke>();

        public Queue<EmulatedKeyStroke> EmulatedKeyStrokes => _emulatedKeyStrokes;

        /// <summary>
        /// Maps Spectrum keys to the PC keyboard keys for Hungarian 101 keyboard layout
        /// </summary>
        /// <remarks>
        /// The key specifies the Spectrum keyboard code mapped to a physical key.
        /// The value is a collection of physical keys. If any of them changes 
        /// its state, the Spectrum key changes, too.
        /// </remarks>
        private static readonly Dictionary<SpectrumKeyCode, List<Key>> s_Hun101KeyMappings = 
            new Dictionary<SpectrumKeyCode, List<Key>>
            {
                { SpectrumKeyCode.SShift, new List<Key> { Key.LeftShift, Key.RightShift,
                    Key.OemComma, Key.OemPeriod, Key.Decimal, Key.Divide, Key.Multiply,
                    Key.Add, Key.Subtract } },
                { SpectrumKeyCode.CShift, new List<Key> { Key.RightAlt, Key.Back,
                    Key.Left, Key.Up, Key.Down, Key.Right, Key.Home } },
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
                { SpectrumKeyCode.J, new List<Key> { Key.J, Key.Subtract } },
                { SpectrumKeyCode.K, new List<Key> { Key.K, Key.Add } },
                { SpectrumKeyCode.L, new List<Key> { Key.L } },
                { SpectrumKeyCode.Z, new List<Key> { Key.Z } },
                { SpectrumKeyCode.X, new List<Key> { Key.X } },
                { SpectrumKeyCode.C, new List<Key> { Key.C } },
                { SpectrumKeyCode.V, new List<Key> { Key.V, Key.Divide } },
                { SpectrumKeyCode.B, new List<Key> { Key.B, Key.Multiply } },
                { SpectrumKeyCode.N, new List<Key> { Key.N, Key.OemComma } },
                { SpectrumKeyCode.M, new List<Key> { Key.M, Key.OemPeriod, Key.Decimal } },
                { SpectrumKeyCode.N0, new List<Key> { Key.D0, Key.NumPad0, Key.Back } },
                { SpectrumKeyCode.N1, new List<Key> { Key.D1, Key.NumPad1, Key.Home } },
                { SpectrumKeyCode.N2, new List<Key> { Key.D2, Key.NumPad2 } },
                { SpectrumKeyCode.N3, new List<Key> { Key.D3, Key.NumPad3 } },
                { SpectrumKeyCode.N4, new List<Key> { Key.D4, Key.NumPad4 } },
                { SpectrumKeyCode.N5, new List<Key> { Key.D5, Key.NumPad5, Key.Left } },
                { SpectrumKeyCode.N6, new List<Key> { Key.D6, Key.NumPad6, Key.Down } },
                { SpectrumKeyCode.N7, new List<Key> { Key.D7, Key.NumPad7, Key.Up } },
                { SpectrumKeyCode.N8, new List<Key> { Key.D8, Key.NumPad8, Key.Right } },
                { SpectrumKeyCode.N9, new List<Key> { Key.D9, Key.NumPad9 } },
            };

        /// <summary>
        /// Maps Spectrum keys to the PC keyboard keys for Hungarian keyboard layout
        /// </summary>
        /// <remarks>
        /// The key specifies the Spectrum keyboard code mapped to a physical key.
        /// The value is a collection of physical keys. If any of them changes 
        /// its state, the Spectrum key changes, too.
        /// </remarks>
        private static readonly Dictionary<SpectrumKeyCode, List<Key>> s_HunKeyMappings =
            new Dictionary<SpectrumKeyCode, List<Key>>
            {
                { SpectrumKeyCode.SShift, new List<Key> { Key.LeftShift, Key.RightShift,
                    Key.OemComma, Key.OemPeriod, Key.Decimal, Key.Divide, Key.Multiply,
                    Key.Add, Key.Subtract } },
                { SpectrumKeyCode.CShift, new List<Key> { Key.RightAlt, Key.Back,
                    Key.Left, Key.Up, Key.Down, Key.Right, Key.Home } },
                { SpectrumKeyCode.Space, new List<Key> { Key.Space} },
                { SpectrumKeyCode.Enter, new List<Key> { Key.Enter } },
                { SpectrumKeyCode.Q, new List<Key> { Key.Q } },
                { SpectrumKeyCode.W, new List<Key> { Key.W } },
                { SpectrumKeyCode.E, new List<Key> { Key.E } },
                { SpectrumKeyCode.R, new List<Key> { Key.R } },
                { SpectrumKeyCode.T, new List<Key> { Key.T } },
                { SpectrumKeyCode.Y, new List<Key> { Key.Z } },
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
                { SpectrumKeyCode.J, new List<Key> { Key.J, Key.Subtract } },
                { SpectrumKeyCode.K, new List<Key> { Key.K, Key.Add } },
                { SpectrumKeyCode.L, new List<Key> { Key.L } },
                { SpectrumKeyCode.Z, new List<Key> { Key.Y } },
                { SpectrumKeyCode.X, new List<Key> { Key.X } },
                { SpectrumKeyCode.C, new List<Key> { Key.C } },
                { SpectrumKeyCode.V, new List<Key> { Key.V, Key.Divide } },
                { SpectrumKeyCode.B, new List<Key> { Key.B, Key.Multiply } },
                { SpectrumKeyCode.N, new List<Key> { Key.N, Key.OemComma } },
                { SpectrumKeyCode.M, new List<Key> { Key.M, Key.OemPeriod, Key.Decimal } },
                { SpectrumKeyCode.N0, new List<Key> { Key.D0, Key.NumPad0, Key.Back } },
                { SpectrumKeyCode.N1, new List<Key> { Key.D1, Key.NumPad1, Key.Home } },
                { SpectrumKeyCode.N2, new List<Key> { Key.D2, Key.NumPad2 } },
                { SpectrumKeyCode.N3, new List<Key> { Key.D3, Key.NumPad3 } },
                { SpectrumKeyCode.N4, new List<Key> { Key.D4, Key.NumPad4 } },
                { SpectrumKeyCode.N5, new List<Key> { Key.D5, Key.NumPad5, Key.Left } },
                { SpectrumKeyCode.N6, new List<Key> { Key.D6, Key.NumPad6, Key.Down } },
                { SpectrumKeyCode.N7, new List<Key> { Key.D7, Key.NumPad7, Key.Up } },
                { SpectrumKeyCode.N8, new List<Key> { Key.D8, Key.NumPad8, Key.Right } },
                { SpectrumKeyCode.N9, new List<Key> { Key.D9, Key.NumPad9 } },
            };

        /// <summary>
        /// Maps Spectrum keys to the PC keyboard keys for English US keyboard layout
        /// </summary>
        /// <remarks>
        /// The key specifies the Spectrum keyboard code mapped to a physical key.
        /// The value is a collection of physical keys. If any of them changes 
        /// its state, the Spectrum key changes, too.
        /// </remarks>
        private static readonly Dictionary<SpectrumKeyCode, List<Key>> s_EngUsKeyMappings =
            new Dictionary<SpectrumKeyCode, List<Key>>
            {
                { SpectrumKeyCode.SShift, new List<Key> { Key.LeftShift, Key.RightShift,
                    Key.OemComma, Key.OemPeriod, Key.Decimal, Key.Divide, Key.Multiply,
                    Key.Add, Key.Subtract } },
                { SpectrumKeyCode.CShift, new List<Key> { Key.RightAlt, Key.Back,
                    Key.Left, Key.Up, Key.Down, Key.Right, Key.Home } },
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
                { SpectrumKeyCode.J, new List<Key> { Key.J, Key.Subtract } },
                { SpectrumKeyCode.K, new List<Key> { Key.K, Key.Add } },
                { SpectrumKeyCode.L, new List<Key> { Key.L } },
                { SpectrumKeyCode.Z, new List<Key> { Key.Z } },
                { SpectrumKeyCode.X, new List<Key> { Key.X } },
                { SpectrumKeyCode.C, new List<Key> { Key.C } },
                { SpectrumKeyCode.V, new List<Key> { Key.V, Key.Divide } },
                { SpectrumKeyCode.B, new List<Key> { Key.B, Key.Multiply } },
                { SpectrumKeyCode.N, new List<Key> { Key.N, Key.OemComma } },
                { SpectrumKeyCode.M, new List<Key> { Key.M, Key.OemPeriod, Key.Decimal } },
                { SpectrumKeyCode.N0, new List<Key> { Key.D0, Key.NumPad0, Key.Back } },
                { SpectrumKeyCode.N1, new List<Key> { Key.D1, Key.NumPad1, Key.Home } },
                { SpectrumKeyCode.N2, new List<Key> { Key.D2, Key.NumPad2 } },
                { SpectrumKeyCode.N3, new List<Key> { Key.D3, Key.NumPad3 } },
                { SpectrumKeyCode.N4, new List<Key> { Key.D4, Key.NumPad4 } },
                { SpectrumKeyCode.N5, new List<Key> { Key.D5, Key.NumPad5, Key.Left } },
                { SpectrumKeyCode.N6, new List<Key> { Key.D6, Key.NumPad6, Key.Down } },
                { SpectrumKeyCode.N7, new List<Key> { Key.D7, Key.NumPad7, Key.Up } },
                { SpectrumKeyCode.N8, new List<Key> { Key.D8, Key.NumPad8, Key.Right } },
                { SpectrumKeyCode.N9, new List<Key> { Key.D9, Key.NumPad9 } },
            };

        /// <summary>
        /// Stores keyboard layouts and related key mappings
        /// </summary>
        private static readonly Dictionary<string, Dictionary<SpectrumKeyCode, List<Key>>> s_LayoutMappings =
            new Dictionary<string, Dictionary<SpectrumKeyCode, List<Key>>>
            {
                { DEFAULT_LAYOUT, s_Hun101KeyMappings },
                { ENG_US_LAYOUT, s_EngUsKeyMappings },
                { HUN_101_LAYOUT, s_Hun101KeyMappings },
                { HUN_LAYOUT, s_HunKeyMappings },
            };

        /// <summary>
        /// Initiate scanning the entire keyboard
        /// </summary>
        /// <param name="allowPhysicalKeyboard">
        /// Indicates if scanning the physical keyboard is allowed
        /// </param>
        /// <remarks>
        /// If the physical keyboard is not allowed, the device can use other
        /// ways to emulate the virtual machine's keyboard
        /// </remarks>
        public void Scan(bool allowPhysicalKeyboard)
        {
            // --- Emulate a key stroke if there is something in the queue
            var hasEmulatedKeyStroke = EmulateKeyStroke();

            if (hasEmulatedKeyStroke || !ApplicationIsActivated() || !allowPhysicalKeyboard)
            {
                return;
            }

            // --- Obtain the layout mappings for the current keyboard layout
            var layoutBuilder = new StringBuilder(256);
            GetKeyboardLayoutName(layoutBuilder);
            var layoutId = layoutBuilder.ToString();

            // --- Obtain the mapping for the current layout
            if (!s_LayoutMappings.TryGetValue(layoutId, out var layoutMappings))
            {
                if (!s_LayoutMappings.TryGetValue(DEFAULT_LAYOUT, out layoutMappings))
                {
                    // --- No default layout 
                    return;
                }
            }

            // --- Check the state of the keys
            foreach (var keyInfo in layoutMappings)
            {
                var keyState = keyInfo.Value.Any(Keyboard.IsKeyDown);
                _statusHandler?.Invoke(keyInfo.Key, keyState);
            }
        }

        /// <summary>
        /// Emulates queued key strokes as if those were pressed by the user
        /// </summary>
        /// <returns>
        /// True, if any key stroke has been emulated; otherwise, false
        /// </returns>
        private bool EmulateKeyStroke()
        {
            // --- Exit, if no keystroke to emulate
            lock (_emulatedKeyStrokes)
            {
                if (_emulatedKeyStrokes.Count == 0) return false;
            }

            // --- Exit, if Spectrum virtual machine is not available
            var spectrumVm = HostVm;
            if (spectrumVm == null) return false;

            var currentTact = spectrumVm.Cpu.Tacts;

            // --- Check the next keystroke
            EmulatedKeyStroke keyStroke;
            lock (_emulatedKeyStrokes)
            {
                keyStroke = _emulatedKeyStrokes.Peek();
            }

            // --- Time has not come
            if (keyStroke.StartTact > currentTact) return false;

            if (keyStroke.EndTact < currentTact)
            {
                // --- End emulation of this very keystroke
                _statusHandler?.Invoke(keyStroke.PrimaryCode, false);
                if (keyStroke.SecondaryCode.HasValue)
                {
                    _statusHandler?.Invoke(keyStroke.SecondaryCode.Value, false);
                }
                lock (_emulatedKeyStrokes)
                {
                    _emulatedKeyStrokes.Dequeue();
                }

                // --- We emulated the release
                return true;
            }

            // --- Emulate this very keystroke, and leave it in the queue
            _statusHandler?.Invoke(keyStroke.PrimaryCode, true);
            if (keyStroke.SecondaryCode.HasValue)
            {
                _statusHandler?.Invoke(keyStroke.SecondaryCode.Value, true);
            }
            return true;
        }

        /// <summary>
        /// Adds an emulated keypress to the queue of the provider.
        /// </summary>
        /// <param name="keypress">Keystroke information</param>
        /// <remarks>The provider can play back emulated key strokes</remarks>
        public void QueueKeyPress(EmulatedKeyStroke keypress)
        {
            lock (_emulatedKeyStrokes)
            {
                if (_emulatedKeyStrokes.Count == 0)
                {
                    _emulatedKeyStrokes.Enqueue(keypress);
                    return;
                }

                EmulatedKeyStroke last;
                lock (_emulatedKeyStrokes)
                {
                    last = _emulatedKeyStrokes.Peek();
                }
                if (last.PrimaryCode == keypress.PrimaryCode
                    && last.SecondaryCode == keypress.SecondaryCode)
                {
                    // --- The same key has been clicked
                    if (keypress.StartTact >= last.StartTact && keypress.StartTact <= last.EndTact)
                    {
                        // --- Old and new click ranges overlap, lengthen the old click
                        last.EndTact = keypress.EndTact;
                        return;
                    }
                }
                lock (_emulatedKeyStrokes)
                {
                    _emulatedKeyStrokes.Enqueue(keypress);
                }
            }
        }

        /// <summary>
        /// Sets the method that can handle the status change of a Spectrum keyboard key
        /// </summary>
        /// <param name="statusHandler">Key status handler method</param>
        /// <remarks>
        /// The first argument of the handler method is the Spectrum key code. The
        /// second argument indicates if the specified key is down (true) or up (false)
        /// </remarks>
        public void SetKeyStatusHandler(Action<SpectrumKeyCode, bool> statusHandler)
        {
            _statusHandler = statusHandler;
        }

        /// <summary>
        /// Retrieves the name of the active input locale identifier 
        /// (formerly called the keyboard layout) for the system.
        /// </summary>
        /// <param name="pwszKLID">
        /// The buffer (of at least KL_NAMELENGTH characters in length) 
        /// that receives the name of the input locale identifier, including 
        /// the terminating null character. This will be a copy of the string 
        /// provided to the LoadKeyboardLayout function, unless layout 
        /// substitution took place.
        /// </param>
        [DllImport("user32.dll")]
        private static extern long GetKeyboardLayoutName(StringBuilder pwszKLID);

        /// <summary>
        /// Retrieves a handle to the foreground window (the window with which 
        /// the user is currently working). The system assigns a slightly higher 
        /// priority to the thread that creates the foreground window than it 
        /// does to other threads.
        /// </summary>
        /// <returns>
        /// The return value is a handle to the foreground window.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Retrieves the identifier of the thread that created the specified 
        /// window and, optionally, the identifier of the process that created 
        /// the window.
        /// </summary>
        /// <param name="handle">
        /// A handle to the window.
        /// </param>
        /// <param name="processId">
        /// A pointer to a variable that receives the process identifier.
        /// </param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        /// <summary>
        /// Checks if the current application is activated
        /// </summary>
        private static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;
            }
            var procId = Process.GetCurrentProcess().Id;
            GetWindowThreadProcessId(activatedHandle, out var activeProcId);
            return activeProcId == procId;
        }
    }
}