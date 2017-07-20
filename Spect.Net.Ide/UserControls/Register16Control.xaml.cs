using System.Windows;

namespace Spect.Net.Z80Tests.UserControls
{
    /// <summary>
    /// Interaction logic for Register16Control.xaml
    /// </summary>
    public partial class Register16Control
    {
        public static readonly DependencyProperty RegNameProperty = DependencyProperty.Register(
            "RegName", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string RegName
        {
            get => (string) GetValue(RegNameProperty);
            set => SetValue(RegNameProperty, value);
        }

        public static readonly DependencyProperty RegValueProperty = DependencyProperty.Register(
            "RegValue", typeof(ushort), typeof(Register16Control), new PropertyMetadata(default(ushort), OnRegValueChanged));

        public ushort RegValue
        {
            get => (ushort) GetValue(RegValueProperty);
            set => SetValue(RegValueProperty, value);
        }

        public static readonly DependencyProperty RegValueHexProperty = DependencyProperty.Register(
            "RegValueHex", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string RegValueHex
        {
            get => (string) GetValue(RegValueHexProperty);
            set => SetValue(RegValueHexProperty, value);
        }

        public static readonly DependencyProperty RegValueDecProperty = DependencyProperty.Register(
            "RegValueDec", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string RegValueDec
        {
            get => (string) GetValue(RegValueDecProperty);
            set => SetValue(RegValueDecProperty, value);
        }

        public static readonly DependencyProperty Bit0Property = DependencyProperty.Register(
            "Bit0", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit0
        {
            get => (string)GetValue(Bit0Property);
            set => SetValue(Bit0Property, value);
        }

        public static readonly DependencyProperty Bit1Property = DependencyProperty.Register(
            "Bit1", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit1
        {
            get => (string)GetValue(Bit1Property);
            set => SetValue(Bit1Property, value);
        }

        public static readonly DependencyProperty Bit2Property = DependencyProperty.Register(
            "Bit2", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit2
        {
            get => (string)GetValue(Bit2Property);
            set => SetValue(Bit2Property, value);
        }

        public static readonly DependencyProperty Bit3Property = DependencyProperty.Register(
            "Bit3", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit3
        {
            get => (string)GetValue(Bit3Property);
            set => SetValue(Bit3Property, value);
        }

        public static readonly DependencyProperty Bit4Property = DependencyProperty.Register(
            "Bit4", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit4
        {
            get => (string)GetValue(Bit4Property);
            set => SetValue(Bit4Property, value);
        }

        public static readonly DependencyProperty Bit5Property = DependencyProperty.Register(
            "Bit5", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit5
        {
            get => (string)GetValue(Bit5Property);
            set => SetValue(Bit5Property, value);
        }

        public static readonly DependencyProperty Bit6Property = DependencyProperty.Register(
            "Bit6", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit6
        {
            get => (string)GetValue(Bit6Property);
            set => SetValue(Bit6Property, value);
        }

        public static readonly DependencyProperty Bit7Property = DependencyProperty.Register(
            "Bit7", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit7
        {
            get => (string)GetValue(Bit7Property);
            set => SetValue(Bit7Property, value);
        }

        public static readonly DependencyProperty Bit8Property = DependencyProperty.Register(
            "Bit8", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit8
        {
            get => (string)GetValue(Bit8Property);
            set => SetValue(Bit8Property, value);
        }

        public static readonly DependencyProperty Bit9Property = DependencyProperty.Register(
            "Bit9", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit9
        {
            get => (string)GetValue(Bit9Property);
            set => SetValue(Bit9Property, value);
        }

        public static readonly DependencyProperty Bit10Property = DependencyProperty.Register(
            "Bit10", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit10
        {
            get => (string)GetValue(Bit10Property);
            set => SetValue(Bit10Property, value);
        }

        public static readonly DependencyProperty Bit11Property = DependencyProperty.Register(
            "Bit11", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit11
        {
            get => (string)GetValue(Bit11Property);
            set => SetValue(Bit11Property, value);
        }

        public static readonly DependencyProperty Bit12Property = DependencyProperty.Register(
            "Bit12", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit12
        {
            get => (string)GetValue(Bit12Property);
            set => SetValue(Bit12Property, value);
        }

        public static readonly DependencyProperty Bit13Property = DependencyProperty.Register(
            "Bit13", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit13
        {
            get => (string)GetValue(Bit13Property);
            set => SetValue(Bit13Property, value);
        }

        public static readonly DependencyProperty Bit14Property = DependencyProperty.Register(
            "Bit14", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit14
        {
            get => (string)GetValue(Bit14Property);
            set => SetValue(Bit14Property, value);
        }

        public static readonly DependencyProperty Bit15Property = DependencyProperty.Register(
            "Bit15", typeof(string), typeof(Register16Control), new PropertyMetadata(default(string)));

        public string Bit15
        {
            get => (string) GetValue(Bit15Property);
            set => SetValue(Bit15Property, value);
        }

        public Register16Control()
        {
            InitializeComponent();
            Update(this, 0);
        }

        private static void OnRegValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var reg = d as Register16Control;
            Update(reg, (ushort)e.NewValue);
        }

        private static void Update(Register16Control reg, ushort value)
        {
            if (reg == null) return;
            reg.Bit0 = Bit(value, 0);
            reg.Bit1 = Bit(value, 1);
            reg.Bit2 = Bit(value, 2);
            reg.Bit3 = Bit(value, 3);
            reg.Bit4 = Bit(value, 4);
            reg.Bit5 = Bit(value, 5);
            reg.Bit6 = Bit(value, 6);
            reg.Bit7 = Bit(value, 7);
            reg.Bit8 = Bit(value, 8);
            reg.Bit9 = Bit(value, 9);
            reg.Bit10 = Bit(value, 10);
            reg.Bit11 = Bit(value, 11);
            reg.Bit12 = Bit(value, 12);
            reg.Bit13 = Bit(value, 13);
            reg.Bit14 = Bit(value, 14);
            reg.Bit15 = Bit(value, 15);
            reg.RegValueHex = $"0x{value:X4}";
            reg.RegValueDec = $"{value}";
        }

        private static string Bit(ushort value, byte bitNo)
        {
            return (value & (1 << bitNo)) != 0 ? "1" : "0";
        }

    }
}
