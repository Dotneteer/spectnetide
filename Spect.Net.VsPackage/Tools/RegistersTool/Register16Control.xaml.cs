using System.ComponentModel;
using System.Windows;

namespace Spect.Net.VsPackage.Tools.RegistersTool
{
    /// <summary>
    /// Interaction logic for Register16Control.xaml
    /// </summary>
    public partial class Register16Control
    {
        public static readonly DependencyProperty RegProperty = DependencyProperty.Register(
            "Reg", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string RegName
        {
            get => (string)GetValue(RegProperty);
            set => SetValue(RegProperty, value);
        }

        public static readonly DependencyProperty RegHProperty = DependencyProperty.Register(
            "RegH", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string RegH
        {
            get => (string)GetValue(RegHProperty);
            set => SetValue(RegHProperty, value);
        }

        public static readonly DependencyProperty RegLProperty = DependencyProperty.Register(
            "RegL", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string RegL
        {
            get => (string)GetValue(RegHProperty);
            set => SetValue(RegHProperty, value);
        }

        public static readonly DependencyProperty RegValueProperty = DependencyProperty.Register(
            "RegValue", typeof(ushort), typeof(Register16Control), new PropertyMetadata(default(ushort), OnRegValueChanged));

        public ushort RegValue
        {
            get => (ushort)GetValue(RegValueProperty);
            set => SetValue(RegValueProperty, value);
        }

        public static readonly DependencyProperty RegValueDecProperty = DependencyProperty.Register(
            "RegValueDec", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string RegValueDec
        {
            get => (string)GetValue(RegValueDecProperty);
            set => SetValue(RegValueDecProperty, value);
        }

        public static readonly DependencyProperty RegHHexProperty = DependencyProperty.Register(
            "RegHHex", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string RegHHex
        {
            get => (string)GetValue(RegHHexProperty);
            set => SetValue(RegHHexProperty, value);
        }

        public static readonly DependencyProperty RegLHexProperty = DependencyProperty.Register(
            "RegLHex", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string RegLHex
        {
            get => (string)GetValue(RegLHexProperty);
            set => SetValue(RegLHexProperty, value);
        }

        public static readonly DependencyProperty RegHDecProperty = DependencyProperty.Register(
            "RegHDec", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string RegHDec
        {
            get => (string)GetValue(RegHDecProperty);
            set => SetValue(RegHDecProperty, value);
        }

        public static readonly DependencyProperty RegLDecProperty = DependencyProperty.Register(
            "RegLDec", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string RegLDec
        {
            get => (string)GetValue(RegLDecProperty);
            set => SetValue(RegLDecProperty, value);
        }

        public Register16Control()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                RegName = "BC";
                RegL = "B";
                RegH = "C";
                RegValue = 0xAF26;
            }
        }

        private static void OnRegValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var reg = d as Register16Control;
            Update(reg, (byte)e.NewValue);
        }

        private static void Update(Register16Control reg, byte value)
        {
            var l = value & 0xFF;
            var h = value >> 8;
            reg.RegHHex = $"0x{h:X2}";
            reg.RegHDec = $"{h}";
            reg.RegLHex = $"0x{l:X2}";
            reg.RegLDec = $"{l}";
            reg.RegValueDec = $"{value}";
        }
    }
}
