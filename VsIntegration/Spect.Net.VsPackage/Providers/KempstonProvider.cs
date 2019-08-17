using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Keyboard;

namespace Spect.Net.VsPackage.Providers
{
    public class KempstonProvider: VmComponentProviderBase, IKempstonProvider
    {
        private KempstonEmulationOptions _oldState;
        private IKeyboardDevice _keyboardDevice;
        private SpectrumKeyCode _leftKey;
        private SpectrumKeyCode _rightKey;
        private SpectrumKeyCode _upKey;
        private SpectrumKeyCode _downKey;
        private SpectrumKeyCode _fireKey;

        /// <summary>
        /// Signs that the provider has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _oldState = KempstonEmulationOptions.Off;
            _keyboardDevice = hostVm.KeyboardDevice;
        }

        /// <summary>
        /// Indicates if the Kempston device is present.
        /// </summary>
        public bool IsPresent
        {
            get
            {
                var newState = SpectNetPackage.Default.Options.KempstonEmulation;
                if (_oldState != newState)
                {
                    switch (newState)
                    {
                        case KempstonEmulationOptions.Left:
                            _leftKey = SpectrumKeyCode.N5;
                            _rightKey = SpectrumKeyCode.N8;
                            _downKey = SpectrumKeyCode.N6;
                            _upKey = SpectrumKeyCode.N7;
                            _fireKey = SpectrumKeyCode.N0;
                            break;
                        case KempstonEmulationOptions.Middle:
                            _leftKey = SpectrumKeyCode.N1;
                            _rightKey = SpectrumKeyCode.N2;
                            _downKey = SpectrumKeyCode.N3;
                            _upKey = SpectrumKeyCode.N4;
                            _fireKey = SpectrumKeyCode.N5;
                            break;
                        case KempstonEmulationOptions.Right:
                            _leftKey = SpectrumKeyCode.N6;
                            _rightKey = SpectrumKeyCode.N7;
                            _downKey = SpectrumKeyCode.N8;
                            _upKey = SpectrumKeyCode.N9;
                            _fireKey = SpectrumKeyCode.N0;
                            break;
                    }
                }
                return newState != KempstonEmulationOptions.Off;
            }
        }

        /// <summary>
        /// The flag that indicates if the left button is pressed.
        /// </summary>
        public bool LeftPressed => _keyboardDevice.GetStatus(_leftKey);

        /// <summary>
        /// The flag that indicates if the right button is pressed.
        /// </summary>
        public bool RightPressed => _keyboardDevice.GetStatus(_rightKey);

        /// <summary>
        /// The flag that indicates if the up button is pressed.
        /// </summary>
        public bool UpPressed => _keyboardDevice.GetStatus(_upKey);

        /// <summary>
        /// The flag that indicates if the down button is pressed.
        /// </summary>
        public bool DownPressed => _keyboardDevice.GetStatus(_downKey);

        /// <summary>
        /// The flag that indicates if the fire button is pressed.
        /// </summary>
        public bool FirePressed => _keyboardDevice.GetStatus(_fireKey);
    }
}