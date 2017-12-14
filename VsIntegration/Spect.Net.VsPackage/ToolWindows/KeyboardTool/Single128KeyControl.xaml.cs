﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Spect.Net.SpectrumEmu.Devices.Keyboard;

namespace Spect.Net.VsPackage.ToolWindows.KeyboardTool
{
    /// <summary>
    /// Interaction logic for Single128KeyControl.xaml
    /// </summary>
    public partial class Single128KeyControl
    {
        /// <summary>
        /// Use this brush for normal background color
        /// </summary>
        public static SolidColorBrush NormalButtonBack = new SolidColorBrush(Color.FromRgb(56, 56, 56));

        /// <summary>
        /// Use this brush when mouse is over
        /// </summary>
        public static SolidColorBrush MouseOverButtonBack = new SolidColorBrush(Color.FromRgb(64, 80, 80));

        /// <summary>
        /// The main key letter dependecy property
        /// </summary>
        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register(
            "Code", typeof(SpectrumKeyCode), typeof(Single128KeyControl), new PropertyMetadata(default(SpectrumKeyCode)));

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
            "SecondaryCode", typeof(SpectrumKeyCode?), typeof(Single128KeyControl), new PropertyMetadata(default(SpectrumKeyCode?)));

        /// <summary>
        /// The main key letter
        /// </summary>
        public SpectrumKeyCode? SecondaryCode
        {
            get => (SpectrumKeyCode?)GetValue(SecondaryCodeProperty);
            set => SetValue(SecondaryCodeProperty, value);
        }

        /// <summary>
        /// The main key letter dependecy property
        /// </summary>
        public static readonly DependencyProperty MainKeyProperty = DependencyProperty.Register(
            "MainKey", typeof(string), typeof(Single128KeyControl), new PropertyMetadata(default(string)));

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
            "Keyword", typeof(string), typeof(Single128KeyControl), new PropertyMetadata(default(string)));

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
            "SShiftKey", typeof(string), typeof(Single128KeyControl), new PropertyMetadata(default(string)));

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
            "ExtKey", typeof(string), typeof(Single128KeyControl), new PropertyMetadata(default(string)));

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
            "ExtShiftKey", typeof(string), typeof(Single128KeyControl), new PropertyMetadata(default(string)));

        /// <summary>
        /// The key in Ext mode with shift
        /// </summary>
        public string ExtShiftKey
        {
            get => (string)GetValue(ExtShiftKeyProperty);
            set => SetValue(ExtShiftKeyProperty, value);
        }

        /// <summary>
        /// Signs simple key mode
        /// </summary>
        public static readonly DependencyProperty SimpleModeProperty = DependencyProperty.Register(
            "SimpleMode", typeof(bool), typeof(Single128KeyControl), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The key contains simple text
        /// </summary>
        public bool SimpleMode
        {
            get => (bool)GetValue(SimpleModeProperty);
            set => SetValue(SimpleModeProperty, value);
        }

        /// <summary>
        /// Signs Clean key mode
        /// </summary>
        public static readonly DependencyProperty CleanModeProperty = DependencyProperty.Register(
            "CleanMode", typeof(bool), typeof(Single128KeyControl), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The key contains simple text
        /// </summary>
        public bool CleanMode
        {
            get => (bool)GetValue(CleanModeProperty);
            set => SetValue(CleanModeProperty, value);
        }

        /// <summary>
        /// Signs numeric key mode
        /// </summary>
        public static readonly DependencyProperty NumericModeProperty = DependencyProperty.Register(
            "NumericMode", typeof(bool), typeof(Single128KeyControl), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The key contains simple text
        /// </summary>
        public bool NumericMode
        {
            get => (bool)GetValue(NumericModeProperty);
            set => SetValue(NumericModeProperty, value);
        }

        /// <summary>
        /// Signs that the key has graphics
        /// </summary>
        public static readonly DependencyProperty CenteredProperty = DependencyProperty.Register(
            "Centered", typeof(bool), typeof(Single128KeyControl), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The shift text is centered
        /// </summary>
        public bool Centered
        {
            get => (bool)GetValue(HasGraphicsProperty);
            set => SetValue(HasGraphicsProperty, value);
        }



        /// <summary>
        /// Signs that the key has graphics
        /// </summary>
        public static readonly DependencyProperty HasGraphicsProperty = DependencyProperty.Register(
            "HasGraphics", typeof(bool), typeof(Single128KeyControl), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The key has graphics
        /// </summary>
        public bool HasGraphics
        {
            get => (bool)GetValue(HasGraphicsProperty);
            set => SetValue(HasGraphicsProperty, value);
        }

        /// <summary>
        /// Graphics code of the key
        /// </summary>
        public static readonly DependencyProperty GraphicsCodeProperty = DependencyProperty.Register(
            "GraphicsCode", typeof(int), typeof(Single128KeyControl), new PropertyMetadata(-1));

        /// <summary>
        /// Graphics code of the key
        /// </summary>
        public int GraphicsCode
        {
            get => (int)GetValue(GraphicsCodeProperty);
            set => SetValue(GraphicsCodeProperty, value);
        }

        /// <summary>
        /// Has the graphics Bit 0 set?
        /// </summary>
        public bool HasBit0 => (GraphicsCode & 0x01) != 0;

        /// <summary>
        /// Has the graphics Bit 1 set?
        /// </summary>
        public bool HasBit1 => (GraphicsCode & 0x02) != 0;

        /// <summary>
        /// Has the graphics Bit 2 set?
        /// </summary>
        public bool HasBit2 => (GraphicsCode & 0x04) != 0;

        /// <summary>
        /// Does the button has text for Extended key?
        /// </summary>
        public bool HidesExtCaption => string.IsNullOrWhiteSpace(ExtKey);

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


        public Single128KeyControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            ButtonBack.Fill = MouseOverButtonBack;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            ButtonBack.Fill = NormalButtonBack;
        }

        private void OnMainKeyMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainKeyClicked?.Invoke(this, e);
        }

        private void OnSShiftMouseDown(object sender, MouseButtonEventArgs e)
        {
            SymShiftKeyClicked?.Invoke(this, e);
        }

        private void OnExtKeyMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (NumericMode)
            {
                NumericControlKeyClicked?.Invoke(this, e);                
            }
            else
            {
                ExtKeyClicked?.Invoke(this, e);
            }
        }
    }

    /// <summary>
    /// Design time sample data for SingleKeyControl
    /// </summary>
    public class SingleKey128ControlSampleData
    {
        public string MainKey { get; set; } = "G";
        public string Keyword { get; set; } = "RETURN";
        public string SShiftKey { get; set; } = "@";
        public string ExtKey { get; set; } = "READ";
        public string ExtShiftKey { get; set; } = "CIRCLE";
        public bool SimpleMode { get; set; } = false;
        public bool CleanMode { get; set; } = false;
        public bool NumericMode { get; set; } = false;
        public bool Centered { get; set; } = true;
        public bool HasGraphics { get; set; } = true;
        public int GraphicsCode { get; set; } = 7;
        public bool HasBit0 { get; set; } = true;
        public bool HasBit1 { get; set; } = true;
        public bool HasBit2 { get; set; } = true;
        public bool HidesExtCaption { get; set; } = false;
    }
}