using System.Windows;

namespace Spect.Net.Z80Tests.UserControls
{
    /// <summary>
    /// Interaction logic for Register8Control.xaml
    /// </summary>
    public partial class Register8Control
    {
        public Register8Control()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty RegNameProperty = DependencyProperty.Register(
            "RegName", typeof(string), typeof(Register8Control), new PropertyMetadata(default(string)));

        public string RegName
        {
            get { return (string) GetValue(RegNameProperty); }
            set { SetValue(RegNameProperty, value); }
        }

        public static readonly DependencyProperty RegValueProperty = DependencyProperty.Register(
            "RegValue", typeof(byte), typeof(Register8Control), new PropertyMetadata(default(byte), OnRegValueChanged));

        public byte RegValue
        {
            get { return (byte) GetValue(RegValueProperty); }
            set { SetValue(RegValueProperty, value); }
        }

        public static readonly DependencyProperty Bit0Property = DependencyProperty.Register(
            "Bit0", typeof(string), typeof(Register8Control), new PropertyMetadata(default(string)));

        public string Bit0
        {
            get { return (string)GetValue(Bit0Property); }
            set { SetValue(Bit0Property, value); }
        }

        public static readonly DependencyProperty Bit1Property = DependencyProperty.Register(
            "Bit1", typeof(string), typeof(Register8Control), new PropertyMetadata(default(string)));

        public string Bit1
        {
            get { return (string) GetValue(Bit1Property); }
            set { SetValue(Bit1Property, value); }
        }

        public static readonly DependencyProperty Bit2Property = DependencyProperty.Register(
            "Bit2", typeof(string), typeof(Register8Control), new PropertyMetadata(default(string)));

        public string Bit2
        {
            get { return (string) GetValue(Bit2Property); }
            set { SetValue(Bit2Property, value); }
        }

        public static readonly DependencyProperty Bit3Property = DependencyProperty.Register(
            "Bit3", typeof(string), typeof(Register8Control), new PropertyMetadata(default(string)));

        public string Bit3
        {
            get { return (string) GetValue(Bit3Property); }
            set { SetValue(Bit3Property, value); }
        }

        public static readonly DependencyProperty Bit4Property = DependencyProperty.Register(
            "Bit4", typeof(string), typeof(Register8Control), new PropertyMetadata(default(string)));

        public string Bit4
        {
            get { return (string) GetValue(Bit4Property); }
            set { SetValue(Bit4Property, value); }
        }

        public static readonly DependencyProperty Bit5Property = DependencyProperty.Register(
            "Bit5", typeof(string), typeof(Register8Control), new PropertyMetadata(default(string)));

        public string Bit5
        {
            get { return (string) GetValue(Bit5Property); }
            set { SetValue(Bit5Property, value); }
        }

        public static readonly DependencyProperty Bit6Property = DependencyProperty.Register(
            "Bit6", typeof(string), typeof(Register8Control), new PropertyMetadata(default(string)));

        public string Bit6
        {
            get { return (string) GetValue(Bit6Property); }
            set { SetValue(Bit6Property, value); }
        }

        public static readonly DependencyProperty Bit7Property = DependencyProperty.Register(
            "Bit7", typeof(string), typeof(Register8Control), new PropertyMetadata(default(string)));

        public string Bit7
        {
            get { return (string) GetValue(Bit7Property); }
            set { SetValue(Bit7Property, value); }
        }

        public static readonly DependencyProperty RegValueHexProperty = DependencyProperty.Register(
            "RegValueHex", typeof(string), typeof(Register8Control), new PropertyMetadata(default(string)));

        public string RegValueHex
        {
            get { return (string) GetValue(RegValueHexProperty); }
            set { SetValue(RegValueHexProperty, value); }
        }

        public static readonly DependencyProperty RegValueDecProperty = DependencyProperty.Register(
            "RegValueDec", typeof(string), typeof(Register8Control), new PropertyMetadata(default(string)));

        public string RegValueDec
        {
            get { return (string) GetValue(RegValueDecProperty); }
            set { SetValue(RegValueDecProperty, value); }
        }
        private static void OnRegValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var reg = d as Register8Control;
            if (reg == null) return;
            reg.Bit0 = Bit((byte)e.NewValue, 0);
            reg.Bit1 = Bit((byte)e.NewValue, 1);
            reg.Bit2 = Bit((byte)e.NewValue, 2);
            reg.Bit3 = Bit((byte)e.NewValue, 3);
            reg.Bit4 = Bit((byte)e.NewValue, 4);
            reg.Bit5 = Bit((byte)e.NewValue, 5);
            reg.Bit6 = Bit((byte)e.NewValue, 6);
            reg.Bit7 = Bit((byte)e.NewValue, 7);
            reg.RegValueHex = $"0x{e.NewValue:X4}";
            reg.RegValueDec = $"{e.NewValue}";
        }

        private static string Bit(byte value, byte bitNo)
        {
            return (value & (1 << bitNo)) != 0 ? "1" : "0";
        }
    }
}
