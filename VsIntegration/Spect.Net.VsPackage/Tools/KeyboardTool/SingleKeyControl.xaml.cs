using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Spect.Net.SpectrumEmu.Devices.Keyboard;

// ReSharper disable PossibleNullReferenceException

namespace Spect.Net.VsPackage.Tools.KeyboardTool
{
    /// <summary>
    /// Interaction logic for SingleKeyControl.xaml
    /// </summary>
    public partial class SingleKeyControl
    {
        /// <summary>
        /// The main key letter dependecy property
        /// </summary>
        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register(
            "Code", typeof(SpectrumKeyCode), typeof(SingleKeyControl), new PropertyMetadata(default(SpectrumKeyCode)));

        /// <summary>
        /// The main key letter
        /// </summary>
        public SpectrumKeyCode Code
        {
            get => (SpectrumKeyCode)GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
        }

        /// <summary>
        /// The main key letter dependecy property
        /// </summary>
        public static readonly DependencyProperty MainKeyProperty = DependencyProperty.Register(
            "MainKey", typeof(string), typeof(SingleKeyControl), new PropertyMetadata(default(string)));

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
            "Keyword", typeof(string), typeof(SingleKeyControl), new PropertyMetadata(default(string)));

        /// <summary>
        /// The keyword
        /// </summary>
        public string Keyword
        {
            get => (string)GetValue(KeyWordProperty);
            set => SetValue(KeyWordProperty, value);
        }

        /// <summary>
        /// The key with symbol shift dependecy property
        /// </summary>
        public static readonly DependencyProperty SShiftKeyProperty = DependencyProperty.Register(
            "SShiftKey", typeof(string), typeof(SingleKeyControl), new PropertyMetadata(default(string)));

        /// <summary>
        /// The key with symbol shift
        /// </summary>
        public string SShiftKey
        {
            get => (string)GetValue(SShiftKeyProperty);
            set => SetValue(SShiftKeyProperty, value);
        }

        /// <summary>
        /// The key in Ext mode dependecy property
        /// </summary>
        public static readonly DependencyProperty ExtKeyProperty = DependencyProperty.Register(
            "ExtKey", typeof(string), typeof(SingleKeyControl), new PropertyMetadata(default(string)));

        /// <summary>
        /// The key in Ext mode
        /// </summary>
        public string ExtKey
        {
            get => (string)GetValue(ExtKeyProperty);
            set => SetValue(ExtKeyProperty, value);
        }

        /// <summary>
        /// The key in Ext mode with shift dependecy property
        /// </summary>
        public static readonly DependencyProperty ExtShiftKeyProperty = DependencyProperty.Register(
            "ExtShiftKey", typeof(string), typeof(SingleKeyControl), new PropertyMetadata(default(string)));

        /// <summary>
        /// The key in Ext mode with shift
        /// </summary>
        public string ExtShiftKey
        {
            get => (string)GetValue(ExtShiftKeyProperty);
            set => SetValue(ExtShiftKeyProperty, value);
        }

        /// <summary>
        /// The key in Ext mode dependecy property
        /// </summary>
        public static readonly DependencyProperty ColorKeyProperty = DependencyProperty.Register(
            "ColorKey", typeof(string), typeof(SingleKeyControl), new PropertyMetadata(default(string)));

        /// <summary>
        /// The key in Ext mode
        /// </summary>
        public string ColorKey
        {
            get => (string)GetValue(ColorKeyProperty);
            set => SetValue(ColorKeyProperty, value);
        }

        /// <summary>
        /// Signs simple key mode
        /// </summary>
        public static readonly DependencyProperty SimpleModeProperty = DependencyProperty.Register(
            "SimpleMode", typeof(bool), typeof(SingleKeyControl), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The key contains simple text
        /// </summary>
        public bool SimpleMode
        {
            get => (bool)GetValue(SimpleModeProperty);
            set => SetValue(SimpleModeProperty, value);
        }

        /// <summary>
        /// Signs numeric key mode
        /// </summary>
        public static readonly DependencyProperty NumericModeProperty = DependencyProperty.Register(
            "NumericMode", typeof(bool), typeof(SingleKeyControl), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The key contains simple text
        /// </summary>
        public bool NumericMode
        {
            get => (bool)GetValue(NumericModeProperty);
            set => SetValue(NumericModeProperty, value);
        }

        /// <summary>
        /// Signs SYM mode
        /// </summary>
        public static readonly DependencyProperty SymModeProperty = DependencyProperty.Register(
            "SymMode", typeof(bool), typeof(SingleKeyControl), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The key should be colored as SYM
        /// </summary>
        public bool SymMode
        {
            get => (bool)GetValue(SymModeProperty);
            set => SetValue(SymModeProperty, value);
        }

        /// <summary>
        /// Signs simple key mode
        /// </summary>
        public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.Register(
            "ButtonWidth", typeof(double), typeof(SingleKeyControl), new PropertyMetadata(76.0));

        /// <summary>
        /// The key contains simple text
        /// </summary>
        public double ButtonWidth
        {
            get => (double)GetValue(ButtonWidthProperty);
            set => SetValue(ButtonWidthProperty, value);
        }

        /// <summary>
        /// Signs simple key mode
        /// </summary>
        public static readonly DependencyProperty NumForegroundProperty = DependencyProperty.Register(
            "NumForeground", typeof(SolidColorBrush), typeof(SingleKeyControl));

        /// <summary>
        /// The key contains simple text
        /// </summary>
        public SolidColorBrush NumForeground
        {
            get => (SolidColorBrush)GetValue(NumForegroundProperty);
            set => SetValue(NumForegroundProperty, value);
        }

        /// <summary>
        /// Signs simple key mode
        /// </summary>
        public static readonly DependencyProperty NumBackgroundProperty = DependencyProperty.Register(
            "NumBackground", typeof(SolidColorBrush), typeof(SingleKeyControl));

        /// <summary>
        /// The key contains simple text
        /// </summary>
        public SolidColorBrush NumBackground
        {
            get => (SolidColorBrush)GetValue(NumBackgroundProperty);
            set => SetValue(NumBackgroundProperty, value);
        }

        /// <summary>
        /// Responds to the event when the main key is clicked
        /// </summary>
        public event MouseButtonEventHandler MainKeyClicked;

        /// <summary>
        /// Responds to the event when the symbol shift key is clicked
        /// </summary>
        public event MouseButtonEventHandler SymShiftKeyClicked;

        /// <summary>
        /// Responds to the event when the extended mode key is clicked
        /// </summary>
        public event MouseButtonEventHandler ExtKeyClicked;

        /// <summary>
        /// Responds to the event when the extended mode key is clicked with shift
        /// </summary>
        public event MouseButtonEventHandler ExtShiftKeyClicked;

        /// <summary>
        /// Responds to the event when the numeric control key is clicked
        /// </summary>
        public event MouseButtonEventHandler NumericControlKeyClicked;

        public SingleKeyControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Forward the main key clicked event
        /// </summary>
        private void MainKeyMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainKeyClicked?.Invoke(this, e);
        }

        /// <summary>
        /// Forward the symbol shift key clicked event
        /// </summary>
        private void SymShiftKeyMouseDown(object sender, MouseButtonEventArgs e)
        {
            SymShiftKeyClicked?.Invoke(this, e);
        }

        /// <summary>
        /// Forward the extended mode key clicked event
        /// </summary>
        private void ExtKeyMouseDown(object sender, MouseButtonEventArgs e)
        {
            ExtKeyClicked?.Invoke(this, e);
        }

        /// <summary>
        /// Forward the extended mode key clicked with shift event
        /// </summary>
        private void ExtShiftKeyMouseDown(object sender, MouseButtonEventArgs e)
        {
            ExtShiftKeyClicked?.Invoke(this, e);
        }

        /// <summary>
        /// Forward the numeric control key clicked event
        /// </summary>
        private void NumericControlKeyMouseDown(object sender, MouseButtonEventArgs e)
        {
            NumericControlKeyClicked?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Design time sample data for SingleKeyControl
    /// </summary>
    public class SingleKeyControlSampleData
    {
        public string MainKey { get; set; } = "G";
        public string Keyword { get; set; } = "RETURN";
        public string SShiftKey { get; set; } = "THEN";
        public string ExtKey { get; set; } = "READ";
        public string ExtShiftKey { get; set; } = "CIRCLE";
        public bool SimpleMode { get; set; } = false;
        public bool NumericMode { get; set; } = false;
        public bool SymMode { get; set; } = false;
        public double ButtonWidth { get; set; } = 100.0;
        public string ColorKey { get; set; } = "BLUE";
        public SolidColorBrush NumForeground { get; set; } = Brushes.Blue;
        public SolidColorBrush NumBackground { get; set; } = Brushes.Transparent;
    }
}
