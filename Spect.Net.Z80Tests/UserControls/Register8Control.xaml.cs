using System.Windows;

namespace Spect.Net.Z80Tests.UserControls
{
    /// <summary>
    /// Interaction logic for Register8Control.xaml
    /// </summary>
    public partial class Register8Control
    {
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

        public Register8Control()
        {
            InitializeComponent();
            Update(this, 0);
        }


        private static void OnRegValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var reg = d as Register8Control;
            Update(reg, (byte)e.NewValue);
        }

        private static void Update(Register8Control reg, byte value)
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
            reg.RegValueHex = $"0x{value:X2}";
            reg.RegValueDec = $"{value}";
        }

        private static string Bit(byte value, byte bitNo)
        {
            return (value & (1 << bitNo)) != 0 ? "1" : "0";
        }
    }
}
