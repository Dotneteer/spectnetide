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
            {Key.Space, SpectrumKeyCode.Space },
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
        };

        public IReadOnlyDictionary<Key, SpectrumKeyCode> Mappings { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public KeyMapper()
        {
            Mappings = new ReadOnlyDictionary<Key, SpectrumKeyCode>(_mappings);
        }

        public SpectrumKeyCode? GetSpectrumKeyCodeFor(Key inputKey)
        {
            SpectrumKeyCode keyCode;
            return _mappings.TryGetValue(inputKey, out keyCode) 
                ? (SpectrumKeyCode?) keyCode 
                : null;
        }
    }
}