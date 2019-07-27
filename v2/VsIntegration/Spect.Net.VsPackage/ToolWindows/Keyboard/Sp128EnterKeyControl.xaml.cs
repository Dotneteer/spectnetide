using System.Windows;
using System.Windows.Input;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Keyboard;

namespace Spect.Net.VsPackage.ToolWindows.Keyboard
{
    /// <summary>
    /// Interaction logic for Sp128KeyControl.xaml
    /// </summary>
    public partial class Sp128EnterKeyControl : IKeyCodeProvider
    {
        /// <summary>
        /// The main key letter
        /// </summary>
        public SpectrumKeyCode Code { get; set; } = SpectrumKeyCode.Enter;

        /// <summary>
        /// The main key letter
        /// </summary>
        public SpectrumKeyCode? SecondaryCode { get; set; } = null;

        /// <summary>
        /// The key contains simple text
        /// </summary>
        public bool NumericMode { get; set; } = false;

        /// <summary>
        /// Responds to the event when the main key is clicked
        /// </summary>
        public event MouseButtonEventHandler MainKeyClicked;

        /// <summary>
        /// Responds to the event when the last key is released
        /// </summary>
        public event MouseButtonEventHandler KeyReleased;

        public Sp128EnterKeyControl()
        {
            InitializeComponent();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            ButtonBack1.Fill = Sp128KeyControl.MouseOverButtonBack;
            ButtonBack2.Fill = Sp128KeyControl.MouseOverButtonBack;
            ButtonBack3.Fill = Sp128KeyControl.MouseOverButtonBack;
            ButtonBack4.Fill = Sp128KeyControl.MouseOverButtonBack;
            ButtonBack5.Fill = Sp128KeyControl.MouseOverButtonBack;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            ButtonBack1.Fill = Sp128KeyControl.NormalButtonBack;
            ButtonBack2.Fill = Sp128KeyControl.NormalButtonBack;
            ButtonBack3.Fill = Sp128KeyControl.NormalButtonBack;
            ButtonBack4.Fill = Sp128KeyControl.NormalButtonBack;
            ButtonBack5.Fill = Sp128KeyControl.NormalButtonBack;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement uiElement)
            {
                uiElement.CaptureMouse();
            }
            MainKeyClicked?.Invoke(this, e);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement uiElement)
            {
                uiElement.ReleaseMouseCapture();
            }
            KeyReleased?.Invoke(this, e);
        }
    }
}
