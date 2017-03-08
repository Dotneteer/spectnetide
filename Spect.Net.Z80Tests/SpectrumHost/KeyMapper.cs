using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Spect.Net.SpectrumEmu.Keyboard;

namespace Spect.Net.Z80Tests.SpectrumHost
{
    /// <summary>
    /// This class stores key mappings between Windows keys and Spectrum keys
    /// </summary>
    public class KeyMapper
    {
        private readonly Dictionary<Key, SpectrumKeyCode> _mappings = new Dictionary<Key, SpectrumKeyCode>
        {
            { Key.Space, SpectrumKeyCode.Space },
            { Key.Q, SpectrumKeyCode.Q },
            { Key.W, SpectrumKeyCode.W },
            { Key.E, SpectrumKeyCode.E },
            { Key.R, SpectrumKeyCode.R },
            { Key.T, SpectrumKeyCode.T },
            { Key.Y, SpectrumKeyCode.Y },
            { Key.U, SpectrumKeyCode.U },
            { Key.I, SpectrumKeyCode.I },
            { Key.O, SpectrumKeyCode.O },
            { Key.P, SpectrumKeyCode.P },
            { Key.A, SpectrumKeyCode.A },
            { Key.S, SpectrumKeyCode.S },
            { Key.D, SpectrumKeyCode.D },
            { Key.F, SpectrumKeyCode.F },
            { Key.G, SpectrumKeyCode.G },
            { Key.H, SpectrumKeyCode.H },
            { Key.J, SpectrumKeyCode.J },
            { Key.K, SpectrumKeyCode.K },
            { Key.L, SpectrumKeyCode.L },
            { Key.Enter, SpectrumKeyCode.Enter },
            { Key.LeftCtrl, SpectrumKeyCode.CShift },
            { Key.RightCtrl, SpectrumKeyCode.CShift },
            { Key.Z, SpectrumKeyCode.Z },
            { Key.X, SpectrumKeyCode.X },
            { Key.C, SpectrumKeyCode.C },
            { Key.V, SpectrumKeyCode.V },
            { Key.B, SpectrumKeyCode.B },
            { Key.N, SpectrumKeyCode.N },
            { Key.M, SpectrumKeyCode.M },
            { Key.LeftShift, SpectrumKeyCode.SShift },
            { Key.RightShift, SpectrumKeyCode.SShift },
            { Key.D0, SpectrumKeyCode.N0 },
            { Key.D1, SpectrumKeyCode.N1 },
            { Key.D2, SpectrumKeyCode.N2 },
            { Key.D3, SpectrumKeyCode.N3 },
            { Key.D4, SpectrumKeyCode.N4 },
            { Key.D5, SpectrumKeyCode.N5 },
            { Key.D6, SpectrumKeyCode.N6 },
            { Key.D7, SpectrumKeyCode.N7 },
            { Key.D8, SpectrumKeyCode.N8 },
            { Key.D9, SpectrumKeyCode.N9 },
            { Key.NumPad0, SpectrumKeyCode.N0 },
            { Key.NumPad1, SpectrumKeyCode.N1 },
            { Key.NumPad2, SpectrumKeyCode.N2 },
            { Key.NumPad3, SpectrumKeyCode.N3 },
            { Key.NumPad4, SpectrumKeyCode.N4 },
            { Key.NumPad5, SpectrumKeyCode.N5 },
            { Key.NumPad6, SpectrumKeyCode.N6 },
            { Key.NumPad7, SpectrumKeyCode.N7 },
            { Key.NumPad8, SpectrumKeyCode.N8 },
            { Key.NumPad9, SpectrumKeyCode.N9 }
        };

        private readonly Dictionary<Key, (SpectrumKeyCode, SpectrumKeyCode)> _extendedMappings = 
            new Dictionary<Key, (SpectrumKeyCode, SpectrumKeyCode)>
        {
                { Key.Back, (SpectrumKeyCode.CShift, SpectrumKeyCode.N0) },
                { Key.Left, (SpectrumKeyCode.CShift, SpectrumKeyCode.N5) },
                { Key.Down, (SpectrumKeyCode.CShift, SpectrumKeyCode.N6) },
                { Key.Up, (SpectrumKeyCode.CShift, SpectrumKeyCode.N7) },
                { Key.Right, (SpectrumKeyCode.CShift, SpectrumKeyCode.N8) },
        };

        /// <summary>
        /// Gets the list of simple key mappings
        /// </summary>
        public IReadOnlyDictionary<Key, SpectrumKeyCode> SimpleMappings { get; }

        /// <summary>
        /// Gets the list of simple key mappings
        /// </summary>
        public IReadOnlyDictionary<Key, (SpectrumKeyCode First, SpectrumKeyCode Second)> ExtendedMappings { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public KeyMapper()
        {
            SimpleMappings = new ReadOnlyDictionary<Key, SpectrumKeyCode>(_mappings);
            ExtendedMappings = new ReadOnlyDictionary<Key, ValueTuple<SpectrumKeyCode, SpectrumKeyCode>>(_extendedMappings);
        }

        /// <summary>
        /// Gets the spectrum key mappings
        /// </summary>
        /// <param name="inputKey">Input key code</param>
        /// <returns></returns>
        public (SpectrumKeyCode? First, SpectrumKeyCode? Second) GetSpectrumKeyCodeFor(Key inputKey)
        {
            if (_extendedMappings.TryGetValue(inputKey, out (SpectrumKeyCode first, SpectrumKeyCode second) outKeyCode))
            {
                return (outKeyCode.first, outKeyCode.second);
            }
            return (
                _mappings.TryGetValue(inputKey, out SpectrumKeyCode keyCode)
                    ? (SpectrumKeyCode?)keyCode
                    : null,
            null);
        }
    }
}