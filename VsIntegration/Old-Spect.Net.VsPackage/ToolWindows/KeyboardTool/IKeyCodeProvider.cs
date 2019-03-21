using Spect.Net.SpectrumEmu.Devices.Keyboard;

namespace Spect.Net.VsPackage.ToolWindows.KeyboardTool
{
    /// <summary>
    /// This interface declares a control that can provide Spectrum key codes
    /// </summary>
    public interface IKeyCodeProvider
    {
        /// <summary>
        /// The main key letter
        /// </summary>
        SpectrumKeyCode Code { get; set; }

        /// <summary>
        /// The main key letter
        /// </summary>
        SpectrumKeyCode? SecondaryCode { get; set; }

        /// <summary>
        /// The key contains simple text
        /// </summary>
        bool NumericMode { get; set; }
    }
}