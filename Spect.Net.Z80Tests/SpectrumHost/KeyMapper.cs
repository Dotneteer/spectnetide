using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using Spect.Net.SpectrumEmu.Keyboard;
// ReSharper disable InconsistentNaming

namespace Spect.Net.Z80Tests.SpectrumHost
{
    /// <summary>
    /// This class stores key mappings between Windows keys and Spectrum keys
    /// </summary>
    public class KeyMapper
    {
        private const string ENG_US_LAYOUT = "00000409";
        private const string HUN_LAYOUT = "0000040E";
        private const string HUN_101_LAYOUT = "0001040E";

        private const string DEFAULT_LAYOUT = "default";

        [DllImport("user32.dll")]
        private static extern bool GetKeyboardLayoutName([Out] StringBuilder pwszKLID);

        private static List<KeyEventArgs> _processed = new List<KeyEventArgs>();

        private static readonly Dictionary<Key, (SpectrumKeyCode?, SpectrumKeyCode?)> _hunNormal = 
            new Dictionary<Key, (SpectrumKeyCode?, SpectrumKeyCode?)>
        {
            // --- Single keys
            { Key.Space, (SpectrumKeyCode.Space, null) },
            { Key.Q, (SpectrumKeyCode.Q, null) },
            { Key.W, (SpectrumKeyCode.W, null) },
            { Key.E, (SpectrumKeyCode.E, null) },
            { Key.R, (SpectrumKeyCode.R, null) },
            { Key.T, (SpectrumKeyCode.T, null) },
            { Key.Y, (SpectrumKeyCode.Y, null) },
            { Key.U, (SpectrumKeyCode.U, null) },
            { Key.I, (SpectrumKeyCode.I, null) },
            { Key.O, (SpectrumKeyCode.O, null) },
            { Key.P, (SpectrumKeyCode.P, null) },
            { Key.A, (SpectrumKeyCode.A, null) },
            { Key.S, (SpectrumKeyCode.S, null) },
            { Key.D, (SpectrumKeyCode.D, null) },
            { Key.F, (SpectrumKeyCode.F, null) },
            { Key.G, (SpectrumKeyCode.G, null) },
            { Key.H, (SpectrumKeyCode.H, null) },
            { Key.J, (SpectrumKeyCode.J, null) },
            { Key.K, (SpectrumKeyCode.K, null) },
            { Key.L, (SpectrumKeyCode.L, null) },
            { Key.Enter, (SpectrumKeyCode.Enter, null) },
            { Key.LeftCtrl, (SpectrumKeyCode.CShift, null) },
            { Key.RightCtrl, (SpectrumKeyCode.CShift, null) },
            { Key.Z, (SpectrumKeyCode.Z, null) },
            { Key.X, (SpectrumKeyCode.X, null) },
            { Key.C, (SpectrumKeyCode.C, null) },
            { Key.V, (SpectrumKeyCode.V, null) },
            { Key.B, (SpectrumKeyCode.B, null) },
            { Key.N, (SpectrumKeyCode.N, null) },
            { Key.M, (SpectrumKeyCode.M, null) },
            { Key.LeftShift, (SpectrumKeyCode.SShift, null) },
            { Key.RightShift, (SpectrumKeyCode.SShift, null) },
            { Key.D0, (SpectrumKeyCode.N0, null) },
            { Key.D1, (SpectrumKeyCode.N1, null) },
            { Key.D2, (SpectrumKeyCode.N2, null) },
            { Key.D3, (SpectrumKeyCode.N3, null) },
            { Key.D4, (SpectrumKeyCode.N4, null) },
            { Key.D5, (SpectrumKeyCode.N5, null) },
            { Key.D6, (SpectrumKeyCode.N6, null) },
            { Key.D7, (SpectrumKeyCode.N7, null) },
            { Key.D8, (SpectrumKeyCode.N8, null) },
            { Key.D9, (SpectrumKeyCode.N9, null) },
            { Key.NumPad0, (SpectrumKeyCode.N0, null) },
            { Key.NumPad1, (SpectrumKeyCode.N1, null) },
            { Key.NumPad2, (SpectrumKeyCode.N2, null) },
            { Key.NumPad3, (SpectrumKeyCode.N3, null) },
            { Key.NumPad4, (SpectrumKeyCode.N4, null) },
            { Key.NumPad5, (SpectrumKeyCode.N5, null) },
            { Key.NumPad6, (SpectrumKeyCode.N6, null) },
            { Key.NumPad7, (SpectrumKeyCode.N7, null) },
            { Key.NumPad8, (SpectrumKeyCode.N8, null) },
            { Key.NumPad9, (SpectrumKeyCode.N9, null) },

            // --- Double keys
            { Key.Back, (SpectrumKeyCode.CShift, SpectrumKeyCode.N0) },
            { Key.Left, (SpectrumKeyCode.CShift, SpectrumKeyCode.N5) },
            { Key.Down, (SpectrumKeyCode.CShift, SpectrumKeyCode.N6) },
            { Key.Up, (SpectrumKeyCode.CShift, SpectrumKeyCode.N7) },
            { Key.Right, (SpectrumKeyCode.CShift, SpectrumKeyCode.N8) },
        };

        private static readonly Dictionary<Key, (SpectrumKeyCode?, SpectrumKeyCode?)> _hunAltGr =
            new Dictionary<Key, (SpectrumKeyCode?, SpectrumKeyCode?)>
            {
                // --- Single keys
                { Key.Space, (SpectrumKeyCode.Enter, null) },
            };

        /// <summary>
        /// Keyboard layout mappings for spectrum key codes
        /// </summary>
        /// <remarks>Extend this layout according to your specific keyboard layout</remarks>
        private static readonly Dictionary<string, (
            Dictionary<Key, (SpectrumKeyCode?, SpectrumKeyCode?)> normal, 
            Dictionary<Key, (SpectrumKeyCode?, SpectrumKeyCode?)> shift,
            Dictionary<Key, (SpectrumKeyCode?, SpectrumKeyCode?)> altGr)> _layoutMappings = 
                new Dictionary<string, (
                    Dictionary<Key, (SpectrumKeyCode?, SpectrumKeyCode?)>, 
                    Dictionary<Key, (SpectrumKeyCode?, SpectrumKeyCode?)>, 
                    Dictionary<Key, (SpectrumKeyCode?, SpectrumKeyCode?)>)>
            {
                { DEFAULT_LAYOUT, (_hunNormal, null, _hunAltGr) },
                { ENG_US_LAYOUT, (_hunNormal, null, _hunAltGr) },
                { HUN_LAYOUT, (_hunNormal, null, _hunAltGr) },
                { HUN_101_LAYOUT, (_hunNormal, null, _hunAltGr) },
            };

        /// <summary>
        /// Functions that check key codes before processing them
        /// </summary>
        private static readonly Dictionary<string, Func<KeyEventArgs, KeyMeaning>> _keyCheckers = 
            new Dictionary<string, Func<KeyEventArgs, KeyMeaning>>
            {
                { DEFAULT_LAYOUT, CheckKey },
                { ENG_US_LAYOUT, CheckKey },
                { HUN_LAYOUT, CheckKey },
                { HUN_101_LAYOUT, CheckKey },
            };

        /// <summary>
        /// Gets the spectrum key mappings
        /// </summary>
        /// <param name="keyArgs">Input key event</param>
        /// <returns></returns>
        public (SpectrumKeyCode? First, SpectrumKeyCode? Second) GetSpectrumKeyCodeFor(KeyEventArgs keyArgs)
        {
            _processed.Add(keyArgs);
            var inputKey = keyArgs.Key;

            // --- Get the current keyboard layout
            var layout = new StringBuilder(256);
            GetKeyboardLayoutName(layout);
            var layoutCode = layout.ToString();

            // --- Get the layout table
            (Dictionary<Key, (SpectrumKeyCode?, SpectrumKeyCode?)> normal,
             Dictionary<Key, (SpectrumKeyCode?, SpectrumKeyCode?)> shift,
             Dictionary<Key, (SpectrumKeyCode?, SpectrumKeyCode?)> altGr) layoutMappings;

            if (!_layoutMappings.TryGetValue(layoutCode, out layoutMappings))
            {
                // --- Use the default layout table
                if(!_layoutMappings.TryGetValue(DEFAULT_LAYOUT, out layoutMappings))
                {
                    // --- No mapping table at all.
                    // TODO: Report this issue
                    return (null, null);
                }
            }

            // --- Get key checker function
            Func<KeyEventArgs, KeyMeaning> checker;
            if (!_keyCheckers.TryGetValue(layoutCode, out checker))
            {
                _keyCheckers.TryGetValue(DEFAULT_LAYOUT, out checker);
            }

            var meaning = checker?.Invoke(keyArgs) ?? KeyMeaning.Normal;
            if (meaning == KeyMeaning.ShiftModifier && layoutMappings.shift != null)
            {
                if (layoutMappings.shift.TryGetValue(inputKey, out (SpectrumKeyCode?, SpectrumKeyCode?) pressed))
                {
                    return pressed;
                }
            }
            if (meaning == KeyMeaning.AltGrModifier && layoutMappings.altGr != null)
            {
                if (layoutMappings.altGr.TryGetValue(inputKey, out (SpectrumKeyCode?, SpectrumKeyCode?) pressed))
                {
                    return pressed;
                }
            }

            return layoutMappings.normal != null &&
                layoutMappings.normal.TryGetValue(inputKey, out (SpectrumKeyCode?, SpectrumKeyCode?) normalPressed)
                    ? normalPressed
                    : (null, null);
        }

        /// <summary>
        /// Checks if the Shift key is down
        /// </summary>
        /// <param name="keyArgs"></param>
        /// <returns>True, if the shift key is down</returns>
        private static KeyMeaning CheckKey(KeyEventArgs keyArgs)
        {
            if (keyArgs.Key == Key.LeftCtrl && keyArgs.IsDown && keyArgs.IsToggled)
            {
                return KeyMeaning.Ignore;
            }

            if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                && keyArgs.Key != Key.LeftShift && keyArgs.Key != Key.RightShift)
            {
                return KeyMeaning.ShiftModifier;
            }

            if (Keyboard.IsKeyDown(Key.RightAlt) && keyArgs.Key != Key.RightAlt)
            {
                return KeyMeaning.AltGrModifier;
            }

            return KeyMeaning.Normal;
        }

        /// <summary>
        /// Represents the meaning of the key
        /// </summary>
        private enum KeyMeaning
        {
            /// <summary>Normal key, process as usual</summary>
            Normal = 0,

            /// <summary>Do not process this key</summary>
            Ignore,

            /// <summary>Event arguments means that Shift modifier is used</summary>
            ShiftModifier,

            /// <summary>Event arguments means that AltGr modifier is used</summary>
            AltGrModifier
        }
    }
}