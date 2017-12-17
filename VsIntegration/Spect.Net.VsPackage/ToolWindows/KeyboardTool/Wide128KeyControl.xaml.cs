using System.Windows;
using System.Windows.Input;
using Spect.Net.SpectrumEmu.Devices.Keyboard;

namespace Spect.Net.VsPackage.ToolWindows.KeyboardTool
{
    /// <summary>
    /// Interaction logic for Single128KeyControl.xaml
    /// </summary>
    public partial class Wide128KeyControl : IKeyCodeProvider
    {
        /// <summary>
        /// Signs simple key mode
        /// </summary>
        public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.Register(
            "ButtonWidth", typeof(double), typeof(Wide128KeyControl), new PropertyMetadata(110.0));

        /// <summary>
        /// The key contains simple text
        /// </summary>
        public double ButtonWidth
        {
            get => (double)GetValue(ButtonWidthProperty);
            set => SetValue(ButtonWidthProperty, value);
        }

        /// <summary>
        /// The main key letter dependecy property
        /// </summary>
        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register(
            "Code", typeof(SpectrumKeyCode), typeof(Wide128KeyControl), new PropertyMetadata(default(SpectrumKeyCode)));

        /// <summary>
        /// The main key letter
        /// </summary>
        public SpectrumKeyCode Code
        {
            get => (SpectrumKeyCode)GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
        }

        /// <summary>
        /// The secondary key letter dependecy property
        /// </summary>
        public static readonly DependencyProperty SecondaryCodeProperty = DependencyProperty.Register(
            "SecondaryCode", typeof(SpectrumKeyCode?), typeof(Wide128KeyControl), new PropertyMetadata(default(SpectrumKeyCode?)));

        /// <summary>
        /// The main key letter
        /// </summary>
        public SpectrumKeyCode? SecondaryCode
        {
            get => (SpectrumKeyCode?)GetValue(SecondaryCodeProperty);
            set => SetValue(SecondaryCodeProperty, value);
        }

        /// <summary>
        /// The key contains simple text
        /// </summary>
        public bool NumericMode { get; set; } = false;

        /// <summary>
        /// The main key letter dependecy property
        /// </summary>
        public static readonly DependencyProperty MainKeyProperty = DependencyProperty.Register(
            "MainKey", typeof(string), typeof(Wide128KeyControl), new PropertyMetadata(default(string)));

        /// <summary>
        /// The main key letter
        /// </summary>
        public string MainKey
        {
            get => (string)GetValue(MainKeyProperty);
            set => SetValue(MainKeyProperty, value);
        }

        /// <summary>
        /// The keyword dependecy property
        /// </summary>
        public static readonly DependencyProperty KeyWordProperty = DependencyProperty.Register(
            "Keyword", typeof(string), typeof(Wide128KeyControl), new PropertyMetadata(default(string)));

        /// <summary>
        /// The keyword
        /// </summary>
        public string Keyword
        {
            get => (string)GetValue(KeyWordProperty);
            set => SetValue(KeyWordProperty, value);
        }

        /// <summary>
        /// Responds to the event when the main key is clicked
        /// </summary>
        public event MouseButtonEventHandler MainKeyClicked;

        /// <summary>
        /// Responds to the event when the last key is released
        /// </summary>
        public event MouseButtonEventHandler KeyReleased;

        public Wide128KeyControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            ButtonBack1.Fill = Single128KeyControl.MouseOverButtonBack;
            ButtonBack2.Fill = Single128KeyControl.MouseOverButtonBack;
            ButtonBack3.Fill = Single128KeyControl.MouseOverButtonBack;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            ButtonBack1.Fill = Single128KeyControl.NormalButtonBack;
            ButtonBack2.Fill = Single128KeyControl.NormalButtonBack;
            ButtonBack3.Fill = Single128KeyControl.NormalButtonBack;
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

    /// <summary>
    /// Design time sample data for SingleKeyControl
    /// </summary>
    public class Wide128KeyControlSampleData
    {
        public double ButtonWidth { get; set; } = 120;
        public string MainKey { get; set; } = "EXTEND";
        public string Keyword { get; set; } = "MODE";
    }
}
